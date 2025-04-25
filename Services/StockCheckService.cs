using Microsoft.Ajax.Utilities;
using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PSCS.Services
{
    public class StockCheckService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public StockCheckService(PSCSEntities pDb)
        {
            try
            {
                this.db = pDb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
               
        // Plan
        public List<StockCheck> GetStockCheckListWithView(DateTime? pDate, string pPipeYard, string pStatus, List<StockCheck> curModel)
        {
            List<StockCheck> result = new List<StockCheck>();
            try
            {
                var obj = (from x in curModel
                           select new
                           {
                               x.RowNo,
                               x.StockCheckDate,
                               x.Yard,
                               x.YardName,
                               x.Location,
                               x.LocationName,
                               x.ItemCode,
                               x.HeatNo,
                               x.Description,
                               x.Qty,
                               x.ActualQty,
                               x.Remark,
                               x.Status,
                               x.StatusText
                           }).AsQueryable();

                // Delivery Date
                if (pDate != null)
                {
                    DateTime date = Convert.ToDateTime(pDate).Date;
                    obj = obj.Where(x => x.StockCheckDate.Year == date.Year
                                    && x.StockCheckDate.Month == date.Month
                                    && x.StockCheckDate.Day == date.Day);
                }
                 
                //// Move Date
                //if (pMoveDate != null)
                //{
                //    DateTime deliveryDate = Convert.ToDateTime(pMoveDate).Date;
                //    //obj = obj.Where(x => x.MOVE_DATE == deliveryDate);
                //    obj = obj.Where(x => x.MOVE_DATE.Year == deliveryDate.Year
                //                && x.MOVE_DATE.Month == deliveryDate.Month
                //                && x.MOVE_DATE.Day == deliveryDate.Day);

                //}

                // Yard
                if (!string.IsNullOrEmpty(pPipeYard))
                {
                    obj = obj.Where(x => x.Yard == pPipeYard);
                }

                // Status
                if (!string.IsNullOrEmpty(pStatus))
                {
                    int Status = Int32.Parse(pStatus);
                    obj = obj.Where(x => x.Status == Status);
                }

                result = obj.AsEnumerable()
                        .Select((x, index) => new StockCheck
                        {
                            RowNo = index + 1,
                            StockCheckDate = x.StockCheckDate,
                            Yard = x.Yard,
                            YardName = x.YardName,
                            Location = x.Location,
                            LocationName = x.LocationName,
                            ItemCode = x.ItemCode,
                            HeatNo = x.HeatNo,
                            Description = x.Description,
                            Qty = x.Qty,
                            ActualQty = x.ActualQty,
                            Remark = x.Remark,
                            Status = x.Status,
                            StatusText = GetStatus(x.Status)
                        }).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }


        // Adjust
        public List<StockCheck> GetStockCheckListWithAdjust(DateTime? pDate, string pPipeYard, string pStatus)
        {
            List<StockCheck> result = new List<StockCheck>();
            int cnt = 0;
            try
            {
                var objStochk = (from ts in db.PSC2310_T_STOCK_CHECK
                                 join it in db.PSC8010_M_PIPE_ITEM
                                         on new { item = ts.ITEM_CODE, heat = ts.HEAT_NO } equals
                                            new { item = it.ITEM_CODE, heat = it.HEAT_NO } into tsit
                                 from tsResult in tsit.DefaultIfEmpty()
                                 join lc in db.PSC8020_M_LOCATION
                                     on ts.LOCATION_CODE equals lc.LOCATION_CODE
                                 join yd in db.PSC8022_M_YARD
                                     on lc.YARD equals yd.YARD
                                 select new
                                 {
                                     ts.STOCK_CHECK_ID,
                                     ts.STOCK_CHECK_DATE,
                                     ts.YARD,
                                     YARD_NAME = yd.NAME,
                                     ts.LOCATION_CODE,
                                     LOCATION_NAME = lc.NAME,
                                     ts.ITEM_CODE,
                                     ts.HEAT_NO,
                                     tsResult.DESCRIPTION,
                                     ts.QTY,
                                     ts.ACTUAL_QTY,
                                     ts.REMARK,
                                     ts.STATUS,
                                 }).AsQueryable();

                // Delivery Date
                if (pDate != null)
                {
                    DateTime date = Convert.ToDateTime(pDate).Date;
                    objStochk = objStochk.Where(x => x.STOCK_CHECK_DATE == date);
                }
                cnt = objStochk.Count();

                // Yard
                if (!string.IsNullOrEmpty(pPipeYard))
                {
                    objStochk = objStochk.Where(x => x.YARD == pPipeYard);
                }
                cnt = objStochk.Count();

                // Status
                //if (!string.IsNullOrEmpty(pStatus))
                //{
                //    int Status = Int32.Parse(pStatus);
                //    objStochk = objStochk.Where(x => x.STATUS == Status);
                //}

                //int Status = Int32.Parse(pStatus);
                objStochk = objStochk.Where(x => x.STATUS == (byte)Constants.StockCheckStatus.Approve || x.STATUS == (byte)Constants.StockCheckStatus.Reject);
                cnt = objStochk.Count();

                result = objStochk.AsEnumerable()
                        .Select((x, index) => new StockCheck
                        {
                            RowNo = index + 1,
                            StockCheckId = x.STOCK_CHECK_ID,
                            StockCheckDate = x.STOCK_CHECK_DATE,
                            Yard = x.YARD,
                            YardName = x.YARD_NAME,
                            Location = x.LOCATION_CODE,
                            LocationName = x.LOCATION_NAME,
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            Description = x.DESCRIPTION,
                            Qty = x.QTY,
                            ActualQty = x.ACTUAL_QTY != null ? (decimal)x.ACTUAL_QTY : (decimal)0.00000000,
                            Remark = x.REMARK,
                            Status = x.STATUS,
                            StatusText = GetStatus(x.STATUS),
                            StatusList = GetStatusList(x.STATUS),
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<StockCheck> GetStockCheckList(DateTime? pDate, string pPipeYard, string pStatus)
        {
            List<StockCheck> result = new List<StockCheck>();
            int cnt = 0;
            try
            {
                var objStochk = (from ts in db.PSC2310_T_STOCK_CHECK
                                 join it in db.PSC8010_M_PIPE_ITEM
                                         on new { item = ts.ITEM_CODE, heat = ts.HEAT_NO } equals
                                            new { item = it.ITEM_CODE, heat = it.HEAT_NO } into tsit
                                 from tsResult in tsit.DefaultIfEmpty()
                                 join lc in db.PSC8020_M_LOCATION
                                     on ts.LOCATION_CODE equals lc.LOCATION_CODE
                                 join yd in db.PSC8022_M_YARD
                                     on lc.YARD equals yd.YARD
                                 //where ts.STATUS <= 3
                                 select new
                                 {
                                     ts.STOCK_CHECK_ID,
                                     ts.STOCK_CHECK_DATE,
                                     ts.YARD,
                                     YARD_NAME = yd.NAME,
                                     ts.LOCATION_CODE,
                                     LOCATION_NAME = lc.NAME,
                                     ts.ITEM_CODE,
                                     ts.HEAT_NO,
                                     tsResult.DESCRIPTION,
                                     ts.QTY,
                                     ts.ACTUAL_QTY,
                                     ts.REMARK,
                                     ts.STATUS,
                                 }).AsQueryable();

                // Delivery Date
                if (pDate != null)
                {
                    DateTime date = Convert.ToDateTime(pDate).Date;
                    objStochk = objStochk.Where(x => x.STOCK_CHECK_DATE.Year == date.Year
                                                && x.STOCK_CHECK_DATE.Month == date.Month
                                                && x.STOCK_CHECK_DATE.Day == date.Day);
                }
                cnt = objStochk.Count();

                // Yard
                if (!string.IsNullOrEmpty(pPipeYard))
                {
                    objStochk = objStochk.Where(x => x.YARD == pPipeYard);
                }
                cnt = objStochk.Count();

                // Status
                if (!string.IsNullOrEmpty(pStatus))
                {
                    int Status = Int32.Parse(pStatus);
                    objStochk = objStochk.Where(x => x.STATUS == Status);
                }
                cnt = objStochk.Count();

                // Update ActualQty
                var prepareObj = objStochk.AsEnumerable()
                                .Select(x => new
                                {
                                    Id = x.STOCK_CHECK_ID,
                                    StockCheckDate = x.STOCK_CHECK_DATE,
                                    Yard = x.YARD,
                                    YardName = x.YARD_NAME,
                                    Location = x.LOCATION_CODE,
                                    LocationName = x.LOCATION_NAME,
                                    ItemCode = x.ITEM_CODE,
                                    HeatNo = x.HEAT_NO,
                                    Description = x.DESCRIPTION,
                                    Qty = x.QTY,
                                    ActualQty = x.ACTUAL_QTY,
                                    //ActualQty = GetActualQty(x.ITEM_CODE, x.HEAT_NO, x.LOCATION_CODE),
                                    Remark = x.REMARK,
                                    Status = x.STATUS
                                }).ToList();

                cnt = prepareObj.Count();

                result = prepareObj.AsEnumerable()
                        .Select((x, index) => new StockCheck
                        {
                            RowNo = index + 1,
                            StockCheckId = x.Id,
                            StockCheckDate = x.StockCheckDate,
                            Yard = x.Yard,
                            YardName = x.YardName,
                            Location = x.Location,
                            LocationName = x.LocationName,
                            ItemCode = x.ItemCode,
                            HeatNo = x.HeatNo,
                            Description = x.Description,
                            Qty = x.Qty,
                            ActualQty = Convert.ToInt32(x.ActualQty),
                            //ActualQty = x.ActualQty != null ? (int)x.ActualQty : new int?(),
                            Remark = x.Remark,
                            Status = x.Status > 0 ? x.Status : GetCheckStatus(x.ActualQty, x.Qty),
                            StatusText = GetStatus(x.Status > 0 ? x.Status : GetCheckStatus(x.ActualQty, x.Qty)),
                            StatusList = GetStatusList(x.Status > 0 ? x.Status : GetCheckStatus(x.ActualQty, x.Qty)),
                            IsEdit = false
                        }).ToList();

                cnt = prepareObj.Count();
                cnt = result.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public StockCheck GetStockCheck(DateTime pDate, string pItemCode, string pHeatNo, string pLocationCode)
        {
            StockCheck result = null;
            List<StockCheck> objStockCheck = null;
            try
            {
                var objStochk = (from ts in db.PSC2310_T_STOCK_CHECK
                                 where ts.STOCK_CHECK_DATE.Day == pDate.Day
                                 && ts.STOCK_CHECK_DATE.Month == pDate.Month
                                 && ts.STOCK_CHECK_DATE.Year == pDate.Year
                                 && ts.ITEM_CODE == pItemCode
                                 && ts.HEAT_NO == pHeatNo
                                 && ts.LOCATION_CODE == pLocationCode
                                 select new
                                 {
                                     ts.STOCK_CHECK_ID,
                                     ts.STOCK_CHECK_DATE,
                                     ts.YARD,
                                     ts.LOCATION_CODE,
                                     ts.ITEM_CODE,
                                     ts.HEAT_NO,
                                     ts.QTY,
                                     ts.ACTUAL_QTY,
                                     ts.REMARK,
                                     ts.STATUS,
                                 }).AsQueryable();


                // Update ActualQty
                var prepareObj = objStochk.AsEnumerable()
                                .Select(x => new
                                {
                                    Id = x.STOCK_CHECK_ID,
                                    StockCheckDate = x.STOCK_CHECK_DATE,
                                    Yard = x.YARD,
                                    Location = x.LOCATION_CODE,
                                    ItemCode = x.ITEM_CODE,
                                    HeatNo = x.HEAT_NO,
                                    Qty = x.QTY,
                                    ActualQty = x.ACTUAL_QTY,
                                    //ActualQty = GetActualQty(x.ITEM_CODE, x.HEAT_NO, x.LOCATION_CODE),
                                    Remark = x.REMARK,
                                    Status = x.STATUS
                                }).ToList();

                objStockCheck = prepareObj.AsEnumerable()
                        .Select((x, index) => new StockCheck
                        {
                            RowNo = index + 1,
                            StockCheckId = x.Id,
                            StockCheckDate = x.StockCheckDate,
                            Yard = x.Yard,
                            Location = x.Location,
                            ItemCode = x.ItemCode,
                            HeatNo = x.HeatNo,
                            Qty = x.Qty,
                            ActualQty = Convert.ToInt32(x.ActualQty),
                            Status = x.Status,
                            StatusText = GetStatus(x.Status)
                        }).ToList();

                if(objStockCheck != null)
                {
                    if(objStockCheck.Count>0)
                    {
                        result = objStockCheck[0];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        // SuperVisor
        public List<StockCheck> GetStockCheckListWithDB(DateTime? pDate, string pPipeYard, string pStatus)
        {
            List<StockCheck> result = new List<StockCheck>();
            int cnt = 0;
            try
             {
                var objStochk = (from ts in db.PSC2310_T_STOCK_CHECK
                                 join it in db.PSC8010_M_PIPE_ITEM on new { item = ts.ITEM_CODE, heat = ts.HEAT_NO } equals new { item = it.ITEM_CODE, heat = it.HEAT_NO } into tsit from tsResult in tsit.DefaultIfEmpty()
                                 join lc in db.PSC8020_M_LOCATION on ts.LOCATION_CODE equals lc.LOCATION_CODE
                                 join yd in db.PSC8022_M_YARD on lc.YARD equals yd.YARD
                                 where ts.STATUS == (byte)Constants.StockCheckStatus.Need_to_Check || ts.STATUS == (byte)Constants.StockCheckStatus.Checked
                                 orderby ts.YARD, ts.LOCATION_CODE, ts.HEAT_NO
                                 select new
                                   {
                                       ts.STOCK_CHECK_ID,
                                       ts.STOCK_CHECK_DATE,
                                       ts.YARD,
                                       YARD_NAME = yd.NAME,
                                       ts.LOCATION_CODE,
                                       LOCATION_NAME = lc.NAME,
                                       ts.ITEM_CODE,
                                       ts.HEAT_NO,
                                       tsResult.DESCRIPTION,
                                       ts.QTY,
                                       ts.ACTUAL_QTY,
                                       ts.REMARK,
                                       ts.STATUS,
                                   }).AsQueryable();

                // Delivery Date
                if (pDate != null)
                {
                    DateTime date = Convert.ToDateTime(pDate).Date;
                    objStochk = objStochk.Where(x => x.STOCK_CHECK_DATE == date);
                }
                cnt = objStochk.Count();

                // Yard
                if (!string.IsNullOrEmpty(pPipeYard))
                {
                    objStochk = objStochk.Where(x => x.YARD == pPipeYard);
                }
                cnt = objStochk.Count();

                // Status
                if (!string.IsNullOrEmpty(pStatus))
                {
                    int Status = Int32.Parse(pStatus);
                    objStochk = objStochk.Where(x => x.STATUS == Status);
                }
                cnt = objStochk.Count();

                // Update ActualQty
                var prepareObj = objStochk.AsEnumerable()                             
                                .Select( x => new
                                {
                                    Id = x.STOCK_CHECK_ID,
                                    StockCheckDate = x.STOCK_CHECK_DATE,
                                    Yard = x.YARD,
                                    YardName = x.YARD_NAME,
                                    Location = x.LOCATION_CODE,
                                    LocationName = x.LOCATION_NAME,
                                    ItemCode = x.ITEM_CODE,
                                    HeatNo = x.HEAT_NO,
                                    Description = x.DESCRIPTION,
                                    Qty = x.QTY,
                                    ActualQty = x.ACTUAL_QTY,
                                    //ActualQty = GetActualQty(x.ITEM_CODE, x.HEAT_NO, x.LOCATION_CODE),
                                    Remark = x.REMARK,
                                    Status = x.STATUS
                                }).ToList();

                cnt = prepareObj.Count();

                result = prepareObj.AsEnumerable()
                        .Select((x, index) => new StockCheck
                        {
                            RowNo = index + 1,
                            StockCheckId = x.Id,
                            StockCheckDate = x.StockCheckDate,
                            Yard = x.Yard,
                            YardName = x.YardName,
                            Location = x.Location,
                            LocationName = x.LocationName,
                            ItemCode = x.ItemCode,
                            HeatNo = x.HeatNo,
                            Description = x.Description,
                            Qty = decimal.Round((decimal)x.Qty, 2, MidpointRounding.AwayFromZero),
                            ActualQty = x.ActualQty != null ? decimal.Round((decimal)x.ActualQty, 2, MidpointRounding.AwayFromZero) : (decimal)0.00,
                            //ActualQty = x.ActualQty != null ? (int)x.ActualQty : new int?(),
                            Remark = x.Remark,
                            Status = x.Status > 0 ? x.Status : GetCheckStatus(x.ActualQty, x.Qty),
                            StatusText = GetStatus(x.Status > 0 ? x.Status : GetCheckStatus(x.ActualQty, x.Qty)),
                            StatusList = GetSupervisorStatusList(x.Status > 0 ? x.Status : GetCheckStatus(x.ActualQty, x.Qty)),
                            IsEdit = false
                        }).ToList();

                cnt = prepareObj.Count();
                cnt = result.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
   

        // Patial in Plan
        public List<StockList> GetStockItemList(string pYardId, string pLocation, string pHeatNo, string pDesc, string pLanguage)
        {
            List<StockList> result = new List<StockList>();
            int cnt = 0;
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from ts in db.PSC2010_T_STOCK
                               join it in db.PSC8010_M_PIPE_ITEM on new { item = ts.ITEM_CODE, heat = ts.HEAT_NO } equals new { item = it.ITEM_CODE, heat = it.HEAT_NO } into tsit from tsResult in tsit.DefaultIfEmpty()
                               join lc in db.PSC8020_M_LOCATION on ts.LOCATION_CODE equals lc.LOCATION_CODE
                               join yd in db.PSC8022_M_YARD on lc.YARD equals yd.YARD
                               where lc.LOCATION_CODE != "1A00"
                               select new
                               {
                                   YearMonth = ts.YEAR_MONTH,
                                   YardId = lc.YARD,
                                   YardName = yd.NAME,
                                   LocationCode = ts.LOCATION_CODE,
                                   LocationName = lc.NAME,
                                   ItemCode = ts.ITEM_CODE,
                                   HeatNo = ts.HEAT_NO,
                                   Description = tsResult.DESCRIPTION,                            
                                   StartQty = ts.STOCK_START_QTY,
                                   InQty = ts.STOCK_IN_QTY,
                                   OutQty = ts.STOCK_OUT_QTY,
                                   AdjustInQty = ts.STOCK_ADJUST_IN_QTY,
                                   AdjustOutQty = ts.STOCK_ADJUST_OUT_QTY,
                                   StockQty = ts.STOCK_QTY,
                                   CreateUserId = ts.CREATE_USER_ID,
                                   CreateDate = ts.CREATE_DATE,
                                   UpdateUserId = ts.UPDATE_USER_ID,
                                   UpdateDate = ts.UPDATE_DATE
                               }).AsQueryable();

                    cnt = obj.Count();

                    // Yard
                    if (!string.IsNullOrEmpty(pYardId))
                    {
                        obj = obj.Where(x => x.YardId == pYardId);
                    }
                    cnt = obj.Count();

                    // Location
                    if (!string.IsNullOrEmpty(pLocation))
                    {
                        obj = obj.Where(x => x.LocationCode.Contains(pLocation));
                    }
                    cnt = obj.Count();

                    // HeatNo
                    if (!string.IsNullOrEmpty(pHeatNo))
                    {
                        obj = obj.Where(x => x.HeatNo.Contains(pHeatNo));
                    }
                    cnt = obj.Count();

                    // Description
                    if (!string.IsNullOrEmpty(pDesc))
                    {
                        obj = obj.Where(x => x.Description.Contains(pDesc));
                    }
                    cnt = obj.Count();

                    result = obj.AsEnumerable().Select((x, index) => new StockList
                    {
                        RowNo = index + 1,
                        YardID = x.YardId,
                        YardName = x.YardName,
                        LocationCode = x.LocationCode,
                        LocationName = x.LocationName,
                        ItemCode = x.ItemCode,
                        HeatNo = x.HeatNo,
                        Description = x.Description,
                        Qty = x.StockQty
                    }).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }


        // Patial -> Plan Grid
        public List<StockCheck> InsertStockcheckGrid(string pRowNoSelected, List<StockList> pStockList, List<StockCheck> pStockCheckList)
        {
            List<StockCheck> result = new List<StockCheck>();
            try
            {
                // convert into list of row no selected
                string[] selectedList = pRowNoSelected.Split(',');

                foreach (var selected in selectedList)
                {
                    // if clickbox all, first selected in array is "true"
                    if (selected == "true") {
                        continue;
                    }

                    // get item in Stock grid by rowNo. selected
                    int se = Int32.Parse(selected);
                    var StockItem = pStockList.Where(x => x.RowNo == se).FirstOrDefault();

                    if (StockItem != null)
                    {
                        var StockCheckItem = pStockCheckList.Where(x => x.Location == StockItem.LocationCode &&
                                                                        x.ItemCode == StockItem.ItemCode && 
                                                                        x.HeatNo == StockItem.HeatNo);

                        // check a new item for add-in StockCheck grid not had before                    
                        if (StockCheckItem.Count() == 0)
                        {
                            pStockCheckList.Add(new StockCheck
                            {
                                StockCheckDate = DateTime.Today,
                                Yard = StockItem.YardID,
                                YardName = StockItem.YardName,
                                Location = StockItem.LocationCode,
                                LocationName = StockItem.LocationName,
                                ItemCode = StockItem.ItemCode,
                                HeatNo = StockItem.HeatNo,
                                Description = StockItem.Description,
                                Qty = (int)StockItem.Qty,
                                Status = (int)Constants.StockCheckStatus.Create
                            });
                        }

                        result = pStockCheckList.AsEnumerable()
                                .Select((x, index) => new StockCheck
                                {
                                    RowNo = index + 1,
                                    StockCheckDate = x.StockCheckDate,
                                    Yard = x.Yard,
                                    YardName = x.YardName,
                                    Location = x.Location,
                                    LocationName = x.LocationName,
                                    ItemCode = x.ItemCode,
                                    HeatNo = x.HeatNo,
                                    Description = x.Description,
                                    Qty = x.Qty,
                                    Status = x.Status,
                                    StatusText = GetStatus(x.Status)
                                }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool DeleteStockCheck(decimal pStockCheckId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    int result = 0;

                    var objStockCheck = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_ID == pStockCheckId);

                    if (objStockCheck != null)
                    {

                        db.PSC2310_T_STOCK_CHECK.Remove(objStockCheck);
                        result = db.SaveChanges();
                    }
                    else
                    {
                        result = 0;
                    }

                    if (result > 0)
                    {
                        tran.Complete();
                        return true;
                    }
                    else
                    {
                        tran.Dispose();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }

        // Patial -> Plan Grid
        public List<StockCheck> RemoveFromGrid(string pRowNoSelected, List<StockCheck> pGridList)
        {
            List<StockCheck> result = new List<StockCheck>();
            try
            {
                // convert into list of row no selected
                string[] selectedList = pRowNoSelected.Split(',');

                foreach (var selected in selectedList)
                {
                    // if clickbox all, first selected in array is "true"
                    if (selected == "true")
                    {
                        continue;
                    }

                    int row = Int32.Parse(selected);
                    var item = pGridList.Where(x => x.RowNo == row).FirstOrDefault();

                    if(item.StockCheckId > 0)
                    {
                        if (DeleteStockCheck(item.StockCheckId))
                        {
                            pGridList.Remove(item);

                            result = pGridList.AsEnumerable()
                                     .Select((x, index) => new StockCheck
                                     {
                                         RowNo = index + 1,
                                         StockCheckDate = x.StockCheckDate,
                                         Yard = x.Yard,
                                         YardName = x.YardName,
                                         Location = x.Location,
                                         LocationName = x.LocationName,
                                         ItemCode = x.ItemCode,
                                         HeatNo = x.HeatNo,
                                         Description = x.Description,
                                         Qty = x.Qty,
                                         ActualQty = x.ActualQty,
                                         Remark = x.Remark,
                                         Status = x.Status,
                                         StatusText = GetStatus(x.Status)
                                     }).ToList();
                        }
                    }
                    else
                    {
                        pGridList.Remove(item);

                        result = pGridList.AsEnumerable()
                                 .Select((x, index) => new StockCheck
                                 {
                                     RowNo = index + 1,
                                     StockCheckDate = x.StockCheckDate,
                                     Yard = x.Yard,
                                     YardName = x.YardName,
                                     Location = x.Location,
                                     LocationName = x.LocationName,
                                     ItemCode = x.ItemCode,
                                     HeatNo = x.HeatNo,
                                     Description = x.Description,
                                     Qty = x.Qty,
                                     ActualQty = x.ActualQty,
                                     Remark = x.Remark,
                                     Status = x.Status,
                                     StatusText = GetStatus(x.Status)
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        // Plan
        //public bool InsertStockCheckData(List<StockCheck> pStockCheck, string userId)
        //{
        //    Boolean result = false;
        //    TransactionScope tran = null;
        //    int flag = 0;
        //    try
        //    {
        //        using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
        //        {
        //            using (this.db)
        //            {
        //                this.db.Configuration.LazyLoadingEnabled = false;
        //                DateTime insertDate = DateTime.Now;
        //                int newRowNo = 0;

        //                foreach (StockCheck en in pStockCheck)
        //                {
        //                    var objStockCheck = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_DATE == en.StockCheckDate &&
        //                                                                                           x.LOCATION_CODE == en.Location &&
        //                                                                                           x.ITEM_CODE == en.ItemCode &&
        //                                                                                           x.HEAT_NO == en.HeatNo);
        //                    if (objStockCheck == null)
        //                    { 
        //                        int? intRowNo = this.db.PSC2310_T_STOCK_CHECK.Max(x => (int?)x.STOCK_CHECK_ID);
        //                        newRowNo = newRowNo == 0 ? (Convert.ToInt32(intRowNo == null ? 1 : intRowNo + 1)) : newRowNo + 1;

        //                        PSC2310_T_STOCK_CHECK insert = new PSC2310_T_STOCK_CHECK();
        //                        insert.STOCK_CHECK_ID = newRowNo;
        //                        insert.STOCK_CHECK_DATE = en.StockCheckDate;
        //                        insert.YARD = en.Yard;
        //                        insert.LOCATION_CODE = en.Location;
        //                        insert.ITEM_CODE = en.ItemCode;
        //                        insert.HEAT_NO = en.HeatNo;
        //                        insert.QTY = en.Qty;
        //                        insert.STATUS = (int)Constants.StockCheckStatus.Need_to_Check; 
        //                        insert.REMARK = en.Remark;
        //                        insert.CREATE_DATE = insertDate;
        //                        insert.CREATE_USER_ID = userId;
        //                        insert.UPDATE_DATE = insertDate;
        //                        insert.UPDATE_USER_ID = userId;

        //                        this.db.PSC2310_T_STOCK_CHECK.Add(insert);
        //                    }
        //                }

        //                flag = this.db.SaveChanges();
        //                if (flag >= 1)
        //                {
        //                    result = true;
        //                    tran.Complete();
        //                }
        //                else
        //                {
        //                    result = false;
        //                    tran.Dispose();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (tran != null)
        //        {
        //            tran.Dispose();
        //        }
        //    }

        //    return result;
        //}

        public bool SaveStockCheckData(DateTime pStockCheckDate, List<StockCheck> pStockCheckList, string userId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            Boolean IsSaveChange = false;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime insertDate = DateTime.Now;
                        int newRowNo = 0;

                        var objStockCheckList = this.db.PSC2310_T_STOCK_CHECK.Where(x => x.STOCK_CHECK_DATE == pStockCheckDate).ToList();
                        if(objStockCheckList != null)
                        {
                            foreach (StockCheck en in pStockCheckList)
                            {
                                var objStockCheck = objStockCheckList.SingleOrDefault(x => x.STOCK_CHECK_DATE == en.StockCheckDate &&
                                                                                                   x.LOCATION_CODE == en.Location &&
                                                                                                   x.ITEM_CODE == en.ItemCode &&
                                                                                                   x.HEAT_NO == en.HeatNo);
                                if (objStockCheck == null)
                                {
                                    int? intRowNo = this.db.PSC2310_T_STOCK_CHECK.Max(x => (int?)x.STOCK_CHECK_ID);
                                    newRowNo = newRowNo == 0 ? (Convert.ToInt32(intRowNo == null ? 1 : intRowNo + 1)) : newRowNo + 1;

                                    PSC2310_T_STOCK_CHECK insert = new PSC2310_T_STOCK_CHECK();
                                    insert.STOCK_CHECK_ID = newRowNo;
                                    insert.STOCK_CHECK_DATE = en.StockCheckDate;
                                    insert.YARD = en.Yard;
                                    insert.LOCATION_CODE = en.Location;
                                    insert.ITEM_CODE = en.ItemCode;
                                    insert.HEAT_NO = en.HeatNo;
                                    insert.QTY = en.Qty;
                                    insert.STATUS = (int)Constants.StockCheckStatus.Need_to_Check;
                                    insert.REMARK = en.Remark;
                                    insert.CREATE_DATE = insertDate;
                                    insert.CREATE_USER_ID = userId;
                                    insert.UPDATE_DATE = insertDate;
                                    insert.UPDATE_USER_ID = userId;

                                    this.db.PSC2310_T_STOCK_CHECK.Add(insert);

                                    IsSaveChange = true;
                                }
                                else
                                {
                                    objStockCheckList.Remove(objStockCheck);
                                }
                            }

                            if(objStockCheckList.Count > 0)
                            {
                                foreach (PSC2310_T_STOCK_CHECK en in objStockCheckList)
                                {
                                    this.db.PSC2310_T_STOCK_CHECK.Remove(en);

                                    IsSaveChange = true;
                                }
                            }
                        }

                        if(IsSaveChange)
                        {
                            flag = this.db.SaveChanges();
                            if (flag >= 1)
                            {
                                result = true;
                                tran.Complete();
                            }
                            else
                            {
                                result = false;
                                tran.Dispose();
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
            }

            return result;
        }

        // Plan
        public bool UpdateStockCheckData(List<StockCheck> pStockCheck, string userId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        foreach (StockCheck en in pStockCheck)
                        {
                            var update = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_ID == en.StockCheckId);
                            if (update != null)
                            {
                                //update.ACTUAL_QTY = en.ActualQty;
                                //update.STATUS = (int) en.Status;
                                update.REMARK = en.Remark;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = userId;
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
            }

            return result;
        }


        // SuperVisor
        public bool ApproveStockCheckData(List<StockCheck> pStockCheck, string userId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        foreach (StockCheck en in pStockCheck)
                        {
                            var update = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_ID == en.StockCheckId);
                            if (update != null)
                            {
                                update.STATUS = 4;
                                update.REMARK = en.Remark;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = userId;
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
            }

            return result;
        }


        // Adjust
        public bool AdjustStockcheckData(string pRowNoSelected, List<StockCheck> pStockAdjustList, string userId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        // convert into list of row no selected
                        string[] selectedList = pRowNoSelected.Split(',');

                        foreach (var selected in selectedList)
                        {
                            if (selected == "true")
                                continue;

                            // get item in Stock grid by rowNo. selected
                            int se = Int32.Parse(selected);
                            var _id = pStockAdjustList.Where(x => x.RowNo == se).FirstOrDefault().StockCheckId;

                            var update = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_ID == _id);
                            if (update != null)
                            {
                                update.STATUS = 5;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = userId;
                            }                      
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        // Adjust
        public bool RejectStockcheckData(string pRowNoSelected, List<StockCheck> pStockAdjustList, string userId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        // convert into list of row no selected
                        string[] selectedList = pRowNoSelected.Split(',');

                        foreach (var selected in selectedList)
                        {
                            if (selected == "true")
                                continue;

                            // get item in Stock grid by rowNo. selected
                            int se = Int32.Parse(selected);
                            var _id = pStockAdjustList.Where(x => x.RowNo == se).FirstOrDefault().StockCheckId;

                            var update = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_ID == _id);
                            if (update != null)
                            {
                                update.STATUS = 6;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = userId;
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        // Adjust
        public bool ApproveStockcheckData(string pRowNoSelected, List<StockCheck> pStockAdjustList, string userId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        //Get Monthly Close
                        string MonthlyClose = string.Empty;
                        MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.db);
                        MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                        if (objMonthlyClose != null)
                        {
                            DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                            string strFMonth = dFMonth.ToString("yyyy-MM");

                            MonthlyClose = strFMonth + "-01";
                        }

                        //Set Date
                        DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);
                        DateTime updateDate = DateTime.Now;

                        // convert into list of row no selected
                        string[] selectedList = pRowNoSelected.Split(',');

                        foreach (var selected in selectedList)
                        {
                            if (selected == "true")
                                continue;

                            // get item in Stock grid by rowNo. selected
                            int se = Int32.Parse(selected);
                            var objStockAdjust = pStockAdjustList.Where(x => x.RowNo == se).FirstOrDefault();

                            if(objStockAdjust != null)
                            {
                                PSC2010_T_STOCK psc2010 = this.db.PSC2010_T_STOCK
                                           .Select(x => x)
                                           .Where(x => x.YEAR_MONTH.Year == dateYearMonth.Year 
                                                    && x.YEAR_MONTH.Month == dateYearMonth.Month 
                                                    && x.ITEM_CODE.Equals(objStockAdjust.ItemCode) 
                                                    && x.HEAT_NO.Equals(objStockAdjust.HeatNo) 
                                                    && x.LOCATION_CODE.Equals(objStockAdjust.Location))
                                           .FirstOrDefault();

                                if (psc2010 != null)
                                {
                                    // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY - PSC2010_T_STOCK.STOCK_QTY || PSC2010_T_STOCK.STOCK_QTY - PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY)
                                    PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
                                    detail.YEAR_MONTH = dateYearMonth;
                                    detail.ITEM_CODE = objStockAdjust.ItemCode;
                                    detail.HEAT_NO = objStockAdjust.HeatNo;
                                    detail.LOCATION_CODE = objStockAdjust.Location;
                                    detail.STOCK_IN_DATE = objStockAdjust.StockCheckDate;
                                    detail.QTY = objStockAdjust.ActualQty;
                                    detail.STOCK_IO_TYPE = Constants.STOCKTAKING_ADJUST;

                                    // 2. Update PSC2010_T_STOCK.STOCK_ADJUST_OUT_QTY = PSC2011_T_STOCK_DETAIL.QTY

                                    decimal decStart = 0;
                                    decimal decIn = 0;
                                    decimal decOut = 0;
                                    decimal decAdjIn = 0;
                                    decimal decAdjOut = 0;
                                    decimal decStock = 0;

                                    if (psc2010.STOCK_START_QTY > 0)
                                    {
                                        decStart =psc2010.STOCK_START_QTY == null ? 0 : psc2010.STOCK_START_QTY.Value;
                                    }
                                    if (psc2010.STOCK_IN_QTY > 0)
                                    {
                                        decIn = psc2010.STOCK_IN_QTY == null ? 0 : psc2010.STOCK_IN_QTY.Value;
                                    }
                                    if (psc2010.STOCK_OUT_QTY > 0)
                                    {
                                        decOut = psc2010.STOCK_OUT_QTY == null ? 0 : psc2010.STOCK_OUT_QTY.Value;
                                    }
                                    if (psc2010.STOCK_ADJUST_IN_QTY > 0)
                                    {
                                        decAdjIn = psc2010.STOCK_ADJUST_IN_QTY == null ? 0 : psc2010.STOCK_ADJUST_IN_QTY.Value;
                                    }
                                    if (psc2010.STOCK_ADJUST_OUT_QTY > 0)
                                    {
                                        decAdjOut = psc2010.STOCK_ADJUST_OUT_QTY == null ? 0 : psc2010.STOCK_ADJUST_OUT_QTY.Value;
                                    }
                                    decStock = decStart + decIn - decOut + decAdjIn - decAdjOut;

                                    if(objStockAdjust.ActualQty != null)
                                    {
                                        if (decStock == objStockAdjust.ActualQty)
                                        {
                                            decStart = decStart;
                                            decIn = decIn;
                                            decOut = decOut;
                                            decAdjIn = decAdjIn;
                                            decAdjOut = decAdjOut;
                                            decStock = decStart + decIn - decOut + decAdjIn - decAdjOut;
                                        }
                                        else if (decStock > objStockAdjust.ActualQty)
                                        {
                                            decStart = decStart;
                                            decIn = decIn;
                                            decOut = decOut;
                                            decAdjIn = decAdjIn;
                                            decAdjOut = decAdjOut + (decStock - objStockAdjust.ActualQty.Value);
                                            decStock = decStart + decIn - decOut + decAdjIn - decAdjOut;
                                        }
                                        else if (decStock < objStockAdjust.ActualQty)
                                        {
                                            decStart = decStart;
                                            decIn = decIn;
                                            decOut = decOut;
                                            decAdjIn = decAdjIn + (objStockAdjust.ActualQty.Value - decStock);
                                            decAdjOut = decAdjOut ;
                                            decStock = decStart + decIn - decOut + decAdjIn - decAdjOut;
                                        }
                                    }                                    

                                    psc2010.STOCK_START_QTY = decStart;
                                    psc2010.STOCK_IN_QTY = decIn;
                                    psc2010.STOCK_OUT_QTY = decOut;
                                    psc2010.STOCK_ADJUST_IN_QTY = decAdjIn;
                                    psc2010.STOCK_ADJUST_OUT_QTY = decAdjOut;
                                    psc2010.STOCK_QTY = decStock;

                                    detail.CREATE_USER_ID = userId;
                                    detail.UPDATE_USER_ID = userId;
                                    detail.UPDATE_DATE = DateTime.Now;
                                    detail.CREATE_DATE = DateTime.Now;

                                    // 3. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
                                    psc2010.STOCK_QTY = psc2010.STOCK_START_QTY + psc2010.STOCK_IN_QTY - psc2010.STOCK_OUT_QTY + psc2010.STOCK_ADJUST_IN_QTY - psc2010.STOCK_ADJUST_OUT_QTY;

                                    this.db.PSC2011_T_STOCK_DETAIL.Add(detail);
                                }

                                var update = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(x => x.STOCK_CHECK_ID == objStockAdjust.StockCheckId);
                                if (update != null)
                                {
                                    update.STATUS = 7;
                                    update.UPDATE_DATE = updateDate;
                                    update.UPDATE_USER_ID = userId;
                                }
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        // Supervisor
        public decimal? GetActualQty(string ItemCode, string HeatNo, string LocationCode)
        {
            decimal? actualQty = null;

            try
            {
                var hht = (from x in db.PSC2310_T_HHT_STOCK_CHECK
                           where x.ITEM_CODE == ItemCode
                              && x.HEAT_NO == HeatNo
                              && x.LOCATION_CODE == LocationCode
                           select new { x.ACTUAL_QTY }).AsQueryable();

                if (hht.Count() > 0)
                {
                    if (hht.Count() > 1)
                    {
                        actualQty = 0;
                        foreach (var qt in hht)
                        {
                            actualQty += (int)qt.ACTUAL_QTY;
                        }
                    }
                    else
                    {
                        actualQty = (int)hht.FirstOrDefault().ACTUAL_QTY;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return actualQty;
        }

        public bool UpdateActualQtyAndStatus(decimal pStockCheckId, decimal pActualQty, Common.Constants.StockCheckStatus pStatus, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2310_T_STOCK_CHECK.SingleOrDefault(sc => sc.STOCK_CHECK_ID == pStockCheckId && sc.STATUS == (byte)Common.Constants.StockCheckStatus.Need_to_Check);
                        if (update != null)
                        {
                            update.ACTUAL_QTY = pActualQty;
                            update.STATUS = (byte)pStatus;
                            update.UPDATE_USER_ID = pUserId;
                            update.UPDATE_DATE = updateDate;
                        }
                        int result = db.SaveChanges();

                        if (result > 0)
                        {
                            tran.Complete();
                            return true;
                        }
                        else
                        {
                            tran.Dispose();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }

        public int? GetCheckStatus(decimal? ActualQty, decimal? Qty)
        {
            int? result = null;

            if (ActualQty == Qty)
            {
                // Submit
                result = 1;
            }
            else if (ActualQty == null)
            {
                // Check
                result = 2;
            }
            else if (ActualQty != Qty)
            {
                // Need to Check
                result = 3;
            }

            return result;
        }
        
        private List<SelectListItem> GetStatusList(int? status)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            foreach (Constants.StockCheckStatus enStatus in (Constants.StockCheckStatus[])Enum.GetValues(typeof(Constants.StockCheckStatus)))
            {
                bool selected = false;

                if ((int)enStatus == status)
                {
                    selected = true;
                }

                result.Add(new SelectListItem
                {
                    Text = (enStatus.ToString().Equals("Create") ? Resources.Common_cshtml.Created :
                        enStatus.ToString().Equals("Need_to_Check") ? Resources.Common_cshtml.NeedToCheck :
                        enStatus.ToString().Equals("Checked") ? Resources.Common_cshtml.Checked :
                        enStatus.ToString().Equals("Approve") ? Resources.Common_cshtml.Approve :
                        enStatus.ToString().Equals("Adjust") ? Resources.Common_cshtml.Adjust :
                        enStatus.ToString().Equals("Reject") ? Resources.Common_cshtml.Reject :
                        enStatus.ToString().Equals("Manager_Approve") ? Resources.Common_cshtml.MangerApprove : ""),
                    Value = ((int)enStatus).ToString(),
                    Selected = selected
                });
            }

            return result;
        }

        private List<SelectListItem> GetSupervisorStatusList(int? status)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            foreach (Constants.StockCheckStatus enStatus in (Constants.StockCheckStatus[])Enum.GetValues(typeof(Constants.StockCheckStatus)))
            {
                bool selected = false;

                if ((int)enStatus == status)
                {
                    selected = true;
                }

                if ((int)enStatus == (int)Constants.StockCheckStatus.Need_to_Check || (int)enStatus == (int)Constants.StockCheckStatus.Checked)
                {
                    result.Add(new SelectListItem
                    {
                        Text = (enStatus.ToString().Equals("Create") ? Resources.Common_cshtml.Created :
                            enStatus.ToString().Equals("Need_to_Check") ? Resources.Common_cshtml.NeedToCheck :
                            enStatus.ToString().Equals("Checked") ? Resources.Common_cshtml.Checked :
                            enStatus.ToString().Equals("Approve") ? Resources.Common_cshtml.Approve :
                            enStatus.ToString().Equals("Adjust") ? Resources.Common_cshtml.Adjust :
                            enStatus.ToString().Equals("Reject") ? Resources.Common_cshtml.Reject :
                            enStatus.ToString().Equals("Manager_Approve") ? Resources.Common_cshtml.MangerApprove : ""),
                        Value = ((int)enStatus).ToString(),
                        Selected = selected
                    });
                }
            }

            return result;
        }

        public string GetStatus(int? pStatus)
        {
            string result = string.Empty;

            if (pStatus == (int)Constants.StockCheckStatus.Create)
            {
                result = Resources.Common_cshtml.Created;
            }
            else if (pStatus == (int)Constants.StockCheckStatus.Need_to_Check)
            {
                result = Resources.Common_cshtml.NeedToCheck;
            }
            else if (pStatus == (int)Constants.StockCheckStatus.Checked)
            {
                result = Resources.Common_cshtml.Checked;
            }
            else if (pStatus == (int)Constants.StockCheckStatus.Approve)
            {
                result = Resources.Common_cshtml.Approve;
            }
            else if (pStatus == (int)Constants.StockCheckStatus.Adjust)
            {
                result = Resources.Common_cshtml.Adjust;
            }
            else if (pStatus == (int)Constants.StockCheckStatus.Reject)
            {
                result = Resources.Common_cshtml.Reject;
            }
            else if (pStatus == (int)Constants.StockCheckStatus.Manager_Approve)
            {
                result = Resources.Common_cshtml.MangerApprove;
            }
            else
            {
                result = "";
            }

            return result;
        }    
    }
}