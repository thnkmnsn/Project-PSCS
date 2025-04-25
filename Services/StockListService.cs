using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class StockListService
    {
        private PSCSEntities db;

        public StockListService(PSCSEntities pDb)
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

        public List<StockList> GetStockList(DateTime pYearMonth, Nullable<DateTime> pReceiveDate, string pYardID, string pLocationCode, string pItemCode, string pDescription,
                                            string pHeatNo, string pOD, string pWT, string pLength, string pGrade, string pMaker, string pStandardName, 
                                            string pOrderBy, string pSortBy,
                                            Boolean pIsShowZero)
        {
            List<StockList> result = new List<StockList>();
            decimal decOD = 0;
            decimal decWT = 0;
            decimal decLT = 0;

            try
            {
                using (this.db)
                {
                    decOD = Common.Common.ToDecimal(pOD);
                    decWT = Common.Common.ToDecimal(pWT);
                    decLT = Common.Common.ToDecimal(pLength);

                    db.Configuration.LazyLoadingEnabled = false;

                    if(pReceiveDate == null)
                    {
                        var objStockList1 = (from sl in this.db.PSC2010_T_STOCK
                                             join pi in db.PSC8010_M_PIPE_ITEM on new { sl.ITEM_CODE, sl.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                                             join lo in db.PSC8020_M_LOCATION on sl.LOCATION_CODE equals lo.LOCATION_CODE
                                             join yd in db.PSC8022_M_YARD on lo.YARD equals yd.YARD
                                             join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                                             join mg in db.PSC8025_M_GRADE on pi.GRADE equals mg.GRADE
                                             join st in db.PSC8024_M_STANDARD on pi.STANDARD equals st.STANDARD
                                             where sl.LOCATION_CODE != Common.Constants.LocationCodeRelease
                                             && (string.IsNullOrEmpty(pYardID) || yd.YARD == pYardID)
                                             && (string.IsNullOrEmpty(pLocationCode) || lo.LOCATION_CODE == pLocationCode)
                                             && (string.IsNullOrEmpty(pItemCode) || sl.ITEM_CODE.Contains(pItemCode))
                                             && (string.IsNullOrEmpty(pHeatNo) || sl.HEAT_NO.Contains(pHeatNo))
                                             && (string.IsNullOrEmpty(pDescription) || pi.DESCRIPTION.Contains(pDescription))
                                             && (string.IsNullOrEmpty(pOD) || pi.OD == decOD)
                                             && (string.IsNullOrEmpty(pWT) || pi.WT == decWT)
                                             && (string.IsNullOrEmpty(pLength) || pi.LT == decLT)
                                             && (string.IsNullOrEmpty(pGrade) || mg.GRADE == pGrade)
                                             && (string.IsNullOrEmpty(pMaker) || mm.MAKER == pMaker)
                                             && (string.IsNullOrEmpty(pStandardName) || st.STANDARD == pStandardName)

                                             && (pYearMonth == DateTime.MinValue || sl.YEAR_MONTH == pYearMonth)

                                             select new
                                             {
                                                 sl.ITEM_CODE,
                                                 pi.DESCRIPTION,
                                                 sl.HEAT_NO,
                                                 sl.LOCATION_CODE,
                                                 LOCATION_NAME = lo.NAME,
                                                 sl.STOCK_QTY,
                                                 pi.OD,
                                                 pi.WT,
                                                 pi.LT,
                                                 mg.GRADE_NAME,
                                                 mm.MAKER_NAME,
                                                 yd.YARD,
                                                 YARD_NAME = yd.NAME,
                                                 pi.UNIT_WEIGHT,
                                                 pi.Gerab_PO,
                                                 pi.Singapore,
                                                 pi.C21_SHL1,
                                                 pi.MN,
                                                 pi.C
                                             }).AsEnumerable();

                        var objStockList = (from sl in objStockList1
                                            where pIsShowZero ? sl.STOCK_QTY >= 0 : sl.STOCK_QTY > 0
                                            select new
                                            {
                                                sl.ITEM_CODE,
                                                sl.DESCRIPTION,
                                                sl.HEAT_NO,
                                                sl.LOCATION_CODE,
                                                sl.LOCATION_NAME,
                                                sl.STOCK_QTY,
                                                sl.OD,
                                                sl.WT,
                                                sl.LT,
                                                sl.GRADE_NAME,
                                                sl.MAKER_NAME,
                                                sl.YARD,
                                                sl.YARD_NAME,
                                                sl.UNIT_WEIGHT,
                                                sl.Gerab_PO,
                                                sl.Singapore,
                                                sl.C21_SHL1,
                                                sl.MN,
                                                sl.C
                                            }).AsEnumerable();

                        if (!string.IsNullOrEmpty(pOrderBy) && !string.IsNullOrEmpty(pSortBy))
                        {
                            if (pSortBy.Equals(Common.Constants.SortBy.ASC.ToString()))
                            {
                                if (pOrderBy.Equals(Common.Constants.OrderBy.OD.ToString()))
                                {
                                    objStockList = objStockList.OrderBy(x => x.OD);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.WT.ToString()))
                                {
                                    objStockList = objStockList.OrderBy(x => x.WT);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.LT.ToString()))
                                {
                                    objStockList = objStockList.OrderBy(x => x.LT);
                                }
                            }
                            else
                            {
                                if (pOrderBy.Equals(Common.Constants.OrderBy.OD.ToString()))
                                {
                                    objStockList = objStockList.OrderByDescending(x => x.OD);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.WT.ToString()))
                                {
                                    objStockList = objStockList.OrderByDescending(x => x.WT);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.LT.ToString()))
                                {
                                    objStockList = objStockList.OrderByDescending(x => x.LT);
                                }
                            }
                        }

                        result = objStockList.AsEnumerable().Select((x, index) => new StockList
                        {
                            RowNo = index + 1,
                            ItemCode = x.ITEM_CODE,
                            Description = x.DESCRIPTION,
                            HeatNo = x.HEAT_NO,
                            LocationName = x.LOCATION_NAME,
                            LocationCode = x.LOCATION_CODE,
                            Qty = x.STOCK_QTY,
                            OD = x.OD,
                            WT = x.WT,
                            Length = x.LT,
                            Grade = x.GRADE_NAME,
                            Maker = x.MAKER_NAME,
                            YardName = x.YARD_NAME,
                            YardID = x.YARD,
                            TotalWeight = ((x.UNIT_WEIGHT) * (x.STOCK_QTY)) / 1000,
                            Gerab_PO = x.Gerab_PO,
                            Singapore = x.Singapore,
                            C21_SHL1 = x.C21_SHL1,
                            MN = x.MN,
                            C = x.C,
                            MNDivC = x.C is null ? 0 : x.MN / x.C
                        }).ToList();
                    }
                    else
                    {
                        var objStockList1 = (from sl in this.db.PSC2010_T_STOCK
                                             join sd in db.PSC2011_T_STOCK_DETAIL on new { sl.ITEM_CODE, sl.HEAT_NO, sl.LOCATION_CODE } equals new { sd.ITEM_CODE, sd.HEAT_NO , sd.LOCATION_CODE}
                                             join pi in db.PSC8010_M_PIPE_ITEM on new { sl.ITEM_CODE, sl.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                                             join lo in db.PSC8020_M_LOCATION on sl.LOCATION_CODE equals lo.LOCATION_CODE
                                             join yd in db.PSC8022_M_YARD on lo.YARD equals yd.YARD
                                             join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                                             join mg in db.PSC8025_M_GRADE on pi.GRADE equals mg.GRADE
                                             join st in db.PSC8024_M_STANDARD on pi.STANDARD equals st.STANDARD
                                             where (string.IsNullOrEmpty(pYardID) || yd.YARD == pYardID)
                                             && (pReceiveDate == DateTime.MinValue || sd.STOCK_IN_DATE == pReceiveDate)
                                             && sd.STOCK_IO_TYPE == "0007"
                                             && (string.IsNullOrEmpty(pLocationCode) || lo.LOCATION_CODE == pLocationCode)
                                             && (string.IsNullOrEmpty(pItemCode) || sl.ITEM_CODE.Contains(pItemCode))
                                             && (string.IsNullOrEmpty(pHeatNo) || sl.HEAT_NO.Contains(pHeatNo))
                                             && (string.IsNullOrEmpty(pDescription) || pi.DESCRIPTION.Contains(pDescription))
                                             && (string.IsNullOrEmpty(pOD) || pi.OD == decOD)
                                             && (string.IsNullOrEmpty(pWT) || pi.WT == decWT)
                                             && (string.IsNullOrEmpty(pLength) || pi.LT == decLT)
                                             && (string.IsNullOrEmpty(pGrade) || mg.GRADE == pGrade)
                                             && (string.IsNullOrEmpty(pMaker) || mm.MAKER == pMaker)
                                             && (string.IsNullOrEmpty(pStandardName) || st.STANDARD == pStandardName)
                                             && (pYearMonth == DateTime.MinValue || sl.YEAR_MONTH == pYearMonth)
                                             //group sd by new { sd.LOCATION_CODE, sd.ITEM_CODE, sd.HEAT_NO } into sdg
                                             select new
                                             {
                                                 sl.ITEM_CODE,
                                                 pi.DESCRIPTION,
                                                 sl.HEAT_NO,
                                                 sl.LOCATION_CODE,
                                                 LOCATION_NAME = lo.NAME,
                                                 sl.STOCK_QTY,
                                                 STOCK_DETAIL_QTY = sd.QTY,
                                                 pi.OD,
                                                 pi.WT,
                                                 pi.LT,
                                                 mg.GRADE_NAME,
                                                 mm.MAKER_NAME,
                                                 yd.YARD,
                                                 YARD_NAME = yd.NAME,
                                                 pi.UNIT_WEIGHT,
                                                 pi.Gerab_PO,
                                                 pi.Singapore,
                                                 pi.C21_SHL1,
                                                 pi.MN,
                                                 pi.C

                                             }).AsEnumerable();

                        var objStockList = (from sl in objStockList1
                                            where pIsShowZero ? sl.STOCK_QTY >= 0 : sl.STOCK_QTY > 0
                                            select new
                                            {
                                                sl.ITEM_CODE,
                                                sl.DESCRIPTION,
                                                sl.HEAT_NO,
                                                sl.LOCATION_CODE,
                                                sl.LOCATION_NAME,
                                                sl.STOCK_QTY,
                                                STOCK_DETAIL_QTY = sl.STOCK_DETAIL_QTY,
                                                sl.OD,
                                                sl.WT,
                                                sl.LT,
                                                sl.GRADE_NAME,
                                                sl.MAKER_NAME,
                                                sl.YARD,
                                                sl.YARD_NAME,
                                                sl.UNIT_WEIGHT,
                                                sl.Gerab_PO,
                                                sl.Singapore,
                                                sl.C21_SHL1,
                                                sl.MN,
                                                sl.C
                                            }).AsEnumerable();

                        if (!string.IsNullOrEmpty(pOrderBy) && !string.IsNullOrEmpty(pSortBy))
                        {
                            if (pSortBy.Equals(Common.Constants.SortBy.ASC.ToString()))
                            {
                                if (pOrderBy.Equals(Common.Constants.OrderBy.OD.ToString()))
                                {
                                    objStockList = objStockList.OrderBy(x => x.OD);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.WT.ToString()))
                                {
                                    objStockList = objStockList.OrderBy(x => x.WT);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.LT.ToString()))
                                {
                                    objStockList = objStockList.OrderBy(x => x.LT);
                                }
                            }
                            else
                            {
                                if (pOrderBy.Equals(Common.Constants.OrderBy.OD.ToString()))
                                {
                                    objStockList = objStockList.OrderByDescending(x => x.OD);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.WT.ToString()))
                                {
                                    objStockList = objStockList.OrderByDescending(x => x.WT);
                                }
                                else if (pOrderBy.Equals(Common.Constants.OrderBy.LT.ToString()))
                                {
                                    objStockList = objStockList.OrderByDescending(x => x.LT);
                                }
                            }
                        }

                        result = objStockList.AsEnumerable().Select((x, index) => new StockList
                        {
                            RowNo = index + 1,
                            ItemCode = x.ITEM_CODE,
                            Description = x.DESCRIPTION,
                            HeatNo = x.HEAT_NO,
                            LocationName = x.LOCATION_NAME,
                            LocationCode = x.LOCATION_CODE,
                            //Qty = x.STOCK_QTY,
                            Qty = x.STOCK_DETAIL_QTY,
                            OD = x.OD,
                            WT = x.WT,
                            Length = x.LT,
                            Grade = x.GRADE_NAME,
                            Maker = x.MAKER_NAME,
                            YardName = x.YARD_NAME,
                            YardID = x.YARD,
                            TotalWeight = ((x.UNIT_WEIGHT) * (x.STOCK_QTY)) / 1000,
                            Gerab_PO = x.Gerab_PO,
                            Singapore = x.Singapore,
                            C21_SHL1 = x.C21_SHL1,
                            MN = x.MN,
                            C = x.C,
                            MNDivC = x.C is null ? 0 : x.MN / x.C
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
        
        public List<StockListPatial> GetStockListDetailPatial(string pYard, string pLocation, string pItemcode, string pHeatno, string pLang)
        {
            List<StockListPatial> result = new List<StockListPatial>();
                
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var objStockDetailList = (from sd in this.db.PSC2011_T_STOCK_DETAIL
                                              join lo in db.PSC8020_M_LOCATION on sd.LOCATION_CODE equals lo.LOCATION_CODE
                                              join mc in db.PSC8050_M_COMMON on sd.STOCK_IO_TYPE equals mc.COMMON_CODE 
                                        where (string.IsNullOrEmpty(pYard) || lo.YARD == pYard)
                                        && (string.IsNullOrEmpty(pLocation) || sd.LOCATION_CODE == pLocation)
                                        && (string.IsNullOrEmpty(pItemcode) || sd.ITEM_CODE == pItemcode)
                                        && (string.IsNullOrEmpty(pHeatno) || sd.HEAT_NO == pHeatno)
                                        && mc.PARENT_CODE == "2000"
                                        orderby sd.STOCK_IN_DATE descending, sd.ID
                                              select new
                                        {
                                           sd.YEAR_MONTH,
                                           sd.STOCK_IN_DATE,
                                           sd.QTY,
                                           mc.VALUE_EN,
                                           mc.VALUE_TH,
                                        }).AsEnumerable();                   

                    result = objStockDetailList.AsEnumerable().Select((x, index) => new StockListPatial
                    {
                        RowNo = index + 1,
                        Date = x.STOCK_IN_DATE.GetValueOrDefault(DateTime.MinValue),
                        Type = (pLang.Equals("Th") ? x.VALUE_TH : x.VALUE_EN),
                        Qty = x.QTY
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<StockList> GetStockListRelease(DateTime pStockDate, string pItemCode, string pHeatNo)
        {
            List<StockList> result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from s in this.db.PSC2010_T_STOCK
                              join lo in db.PSC8020_M_LOCATION on s.LOCATION_CODE equals lo.LOCATION_CODE
                              join yd in db.PSC8022_M_YARD on lo.YARD equals yd.YARD
                              join pi in db.PSC8010_M_PIPE_ITEM on new { s.ITEM_CODE, s.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                              where  s.YEAR_MONTH.Year == pStockDate.Year 
                                              && s.YEAR_MONTH.Month == pStockDate.Month
                                              && s.ITEM_CODE==pItemCode
                                              && s.HEAT_NO==pHeatNo
                                              && s.LOCATION_CODE != Common.Constants.LocationCodeRelease
                              orderby yd.YARD ascending
                              select new
                                              {
                                                  s.YEAR_MONTH,
                                                  s.ITEM_CODE,
                                                  s.HEAT_NO,
                                                  s.LOCATION_CODE,
                                                  lo.NAME,
                                                  lo.YARD,
                                                  YARD_NAME = yd.NAME,
                                                  pi.DESCRIPTION,
                                                  s.STOCK_QTY,
                                              }).AsEnumerable().Select((x, index) => new StockList
                                              {
                                                  YearMonth = x.YEAR_MONTH,
                                                  ItemCode = x.ITEM_CODE,
                                                  HeatNo = x.HEAT_NO,
                                                  YardID = x.YARD,
                                                  YardName = x.YARD_NAME,
                                                  LocationCode = x.LOCATION_CODE,
                                                  LocationName = x.NAME,
                                                  Description = x.DESCRIPTION,
                                                  Qty = x.STOCK_QTY
                                              }).ToList();

                    if (result != null)
                    {
                        foreach (StockList en in result)
                        {
                            var objStockDetailList = (from s in this.db.PSC2011_T_STOCK_DETAIL
                                                   where s.YEAR_MONTH.Year == en.YearMonth.Year
                                                                   && s.YEAR_MONTH.Month == en.YearMonth.Month
                                                                   && s.ITEM_CODE == en.ItemCode
                                                                   && s.HEAT_NO == en.HeatNo
                                                                   && s.LOCATION_CODE == en.LocationCode
                                                   orderby s.STOCK_IN_DATE ascending
                                                   select new
                                                   {
                                                       s.QTY,
                                                       s.STOCK_IN_DATE
                                                   }).AsEnumerable().ToList();
                            if(objStockDetailList != null)
                            {
                                
                                for (int index =0; index < objStockDetailList.Count; index++  )
                                {
                                    var objStockDetail = objStockDetailList[index];
                                    if(en.ReceiveDateText == null)
                                    {
                                        en.ReceiveDateText = Convert.ToDateTime(objStockDetail.STOCK_IN_DATE).ToString("dd/MM/yyyy") + " - (" + objStockDetail.QTY  +  ")";
                                    }
                                    else if (en.ReceiveDateText == string.Empty)
                                    {
                                        en.ReceiveDateText = Convert.ToDateTime(objStockDetail.STOCK_IN_DATE).ToString("dd/MM/yyyy") + " - (" + objStockDetail.QTY + ")";
                                    }
                                    else
                                    {
                                        en.ReceiveDateText = en.ReceiveDateText + "," + Convert.ToDateTime(objStockDetail.STOCK_IN_DATE).ToString("dd/MM/yyyy") + " - (" + objStockDetail.QTY + ")";
                                    }  
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<StockList> GetStockListRelease(DateTime pStockDate, string pHeatNo, decimal pOD, decimal pWT)
        {
            List<StockList> result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from s in this.db.PSC2010_T_STOCK
                              join lo in db.PSC8020_M_LOCATION on s.LOCATION_CODE equals lo.LOCATION_CODE
                              join yd in db.PSC8022_M_YARD on lo.YARD equals yd.YARD
                              join pi in db.PSC8010_M_PIPE_ITEM on new { s.ITEM_CODE, s.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                              where s.YEAR_MONTH.Year == pStockDate.Year
                                              && s.YEAR_MONTH.Month == pStockDate.Month
                                              && s.HEAT_NO == pHeatNo
                                              && s.LOCATION_CODE != Common.Constants.LocationCodeRelease
                                              && pi.OD == pOD
                                              && pi.WT == pWT
                              orderby yd.YARD ascending
                              select new
                              {
                                  s.YEAR_MONTH,
                                  s.ITEM_CODE,
                                  s.HEAT_NO,
                                  s.LOCATION_CODE,
                                  lo.NAME,
                                  lo.YARD,
                                  YARD_NAME = yd.NAME,
                                  pi.DESCRIPTION,
                                  s.STOCK_QTY,
                              }).AsEnumerable().Select((x, index) => new StockList
                              {
                                  YearMonth = x.YEAR_MONTH,
                                  ItemCode = x.ITEM_CODE,
                                  HeatNo = x.HEAT_NO,
                                  YardID = x.YARD,
                                  YardName = x.YARD_NAME,
                                  LocationCode = x.LOCATION_CODE,
                                  LocationName = x.NAME,
                                  Description = x.DESCRIPTION,
                                  Qty = x.STOCK_QTY
                              }).ToList();

                    if (result != null)
                    {
                        foreach (StockList en in result)
                        {
                            var objStockDetailList = (from s in this.db.PSC2011_T_STOCK_DETAIL
                                                      where s.YEAR_MONTH.Year == en.YearMonth.Year
                                                                      && s.YEAR_MONTH.Month == en.YearMonth.Month
                                                                      && s.ITEM_CODE == en.ItemCode
                                                                      && s.HEAT_NO == en.HeatNo
                                                                      && s.LOCATION_CODE == en.LocationCode
                                                      orderby s.STOCK_IN_DATE ascending
                                                      select new
                                                      {
                                                          s.QTY,
                                                          s.STOCK_IN_DATE
                                                      }).AsEnumerable().ToList();
                            if (objStockDetailList != null)
                            {

                                for (int index = 0; index < objStockDetailList.Count; index++)
                                {
                                    var objStockDetail = objStockDetailList[index];
                                    if (en.ReceiveDateText == null)
                                    {
                                        en.ReceiveDateText = Convert.ToDateTime(objStockDetail.STOCK_IN_DATE).ToString("dd/MM/yyyy") + " - (" + objStockDetail.QTY + ")";
                                    }
                                    else if (en.ReceiveDateText == string.Empty)
                                    {
                                        en.ReceiveDateText = Convert.ToDateTime(objStockDetail.STOCK_IN_DATE).ToString("dd/MM/yyyy") + " - (" + objStockDetail.QTY + ")";
                                    }
                                    else
                                    {
                                        en.ReceiveDateText = en.ReceiveDateText + "," + Convert.ToDateTime(objStockDetail.STOCK_IN_DATE).ToString("dd/MM/yyyy") + " - (" + objStockDetail.QTY + ")";
                                    }
                                }
                            }
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

        public Boolean ReceiveDataList(DateTime pYearMonth, DateTime pStockDate, List<HHTReceive> pHHTReceiveList, string pUserId)
        {
            Boolean result = false;

            try
            {    
                foreach (HHTReceive en in pHHTReceiveList)
                {
                    string itemCode = en.ItemCode;
                    string heatNo = en.HeatNo;
                    string locationCode = en.LocationCode;
                            
                    // Get data from PSC2010_T_STOCK
                    PSC2010_T_STOCK psc2010 = this.db.PSC2010_T_STOCK
                                .Select(x => x)
                                .Where(x => x.YEAR_MONTH.Year == pYearMonth.Year && x.YEAR_MONTH.Month  == pYearMonth.Month && x.ITEM_CODE.Equals(itemCode) && x.HEAT_NO.Equals(heatNo) && x.LOCATION_CODE.Equals(locationCode))
                                .FirstOrDefault();

                    if (psc2010 == null)
                    {
                        InsertDataNoTrans(pYearMonth, pStockDate, en.ItemCode, en.HeatNo, en.LocationCode, Convert.ToInt32(en.ActualQTY), pUserId);
                    }
                    else
                    {
                        UpdateInDataNoTrans(pYearMonth, pStockDate, en.ItemCode, en.HeatNo, en.LocationCode, Convert.ToInt32(en.ActualQTY), pUserId);
                    }
                }

                int intSaveChanges = this.db.SaveChanges();

                if (intSaveChanges > 0)
                {
                    //tran.Complete();
                    result = true;
                }
                else
                {
                    //tran.Dispose();
                    result = false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool UpdateOutData(DateTime pYearMonthDate,DateTime pStockInDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        DateTime insertDate = DateTime.Now;
                        
                        string itemCode = pItemCode;
                        string heatNo = pHeatNo;
                        string locationCode = pLocationCode;

                        UpdateStockOutData(pYearMonthDate, pStockInDate, pItemCode, pHeatNo, pLocationCode, Convert.ToInt32(pQty), pUserId);

                        int result = this.db.SaveChanges();

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

        public bool UpdateInData(DateTime pYearMonthDate, DateTime pStockInDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        DateTime insertDate = DateTime.Now;

                        string itemCode = pItemCode;
                        string heatNo = pHeatNo;
                        string locationCode = pLocationCode;

                        UpdateStockInData(pYearMonthDate, pStockInDate, pItemCode, pHeatNo, pLocationCode, Convert.ToInt32(pQty), pUserId);

                        int result = this.db.SaveChanges();

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

        

        private void InsertData(DateTime pYearMonthDate, DateTime pStockInDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            try
            {
                // 1. Insert PSC2010_T_STOCK (Current Year-Month)
                PSC2010_T_STOCK stock = new PSC2010_T_STOCK();
                stock.YEAR_MONTH = pYearMonthDate;
                stock.ITEM_CODE = pItemCode;
                stock.HEAT_NO = pHeatNo;
                stock.LOCATION_CODE = pLocationCode;
                stock.STOCK_START_QTY = 0;
                stock.STOCK_IN_QTY = 0;
                stock.STOCK_OUT_QTY = 0;
                stock.STOCK_ADJUST_IN_QTY = 0;
                stock.STOCK_ADJUST_OUT_QTY = 0;
                stock.STOCK_QTY = 0;
                stock.CREATE_USER_ID = pUserId;
                stock.UPDATE_USER_ID = pUserId;
                stock.CREATE_DATE = DateTime.Now;
                stock.UPDATE_DATE = DateTime.Now;

                // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = Actual Qty)
                PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
                detail.YEAR_MONTH = pYearMonthDate;
                detail.ITEM_CODE = pItemCode;
                detail.HEAT_NO = pHeatNo;
                detail.LOCATION_CODE = pLocationCode;
                detail.STOCK_IN_DATE = pStockInDate;
                //detail.UPDATE_TYPE = (int)Constants.STOCK_DETAIL_UPDATE_TYPE.NORMAL;
                detail.STOCK_IO_TYPE = Constants.STOCKTAKING_IN;
                detail.QTY = pQty;
                detail.CREATE_USER_ID = pUserId;
                detail.UPDATE_USER_ID = pUserId;
                detail.CREATE_DATE = DateTime.Now;
                detail.UPDATE_DATE = DateTime.Now;

                // 3. Update PSC2010_T_STOCK.STOCK_ADJUST_IN_QTY = PSC2011_T_STOCK_DETAIL.QTY
                stock.STOCK_ADJUST_IN_QTY = detail.QTY;

                // 4. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
                stock.STOCK_QTY = stock.STOCK_START_QTY + stock.STOCK_IN_QTY - stock.STOCK_OUT_QTY + stock.STOCK_ADJUST_IN_QTY - stock.STOCK_ADJUST_OUT_QTY;

                this.db.PSC2010_T_STOCK.Add(stock);
                this.db.PSC2011_T_STOCK_DETAIL.Add(detail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void InsertDataNoTrans(DateTime pYearMonthDate, DateTime pStockDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            try
            {
                // 1. Insert PSC2010_T_STOCK (Current Year-Month)
                PSC2010_T_STOCK stock = new PSC2010_T_STOCK();
                stock.YEAR_MONTH = pYearMonthDate;
                stock.ITEM_CODE = pItemCode;
                stock.HEAT_NO = pHeatNo;
                stock.LOCATION_CODE = pLocationCode;
                stock.STOCK_START_QTY = 0;
                stock.STOCK_IN_QTY = 0;
                stock.STOCK_OUT_QTY = 0;
                stock.STOCK_ADJUST_IN_QTY = 0;
                stock.STOCK_ADJUST_OUT_QTY = 0;
                stock.STOCK_QTY = 0;
                stock.CREATE_USER_ID = pUserId;
                stock.UPDATE_USER_ID = pUserId;
                stock.CREATE_DATE = DateTime.Now;
                stock.UPDATE_DATE = DateTime.Now;

                // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = Actual Qty)
                PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
                detail.YEAR_MONTH = pYearMonthDate;
                detail.ITEM_CODE = pItemCode;
                detail.HEAT_NO = pHeatNo;
                detail.LOCATION_CODE = pLocationCode;
                detail.STOCK_IN_DATE = pStockDate;
                //detail.UPDATE_TYPE = (int)Constants.STOCK_DETAIL_UPDATE_TYPE.NORMAL;
                detail.STOCK_IO_TYPE = Constants.STOCKTAKING_IN;
                detail.QTY = pQty;
                detail.CREATE_USER_ID = pUserId;
                detail.UPDATE_USER_ID = pUserId;
                detail.CREATE_DATE = DateTime.Now;
                detail.UPDATE_DATE = DateTime.Now;

                // 3. Update PSC2010_T_STOCK.STOCK_ADJUST_IN_QTY = PSC2011_T_STOCK_DETAIL.QTY
                stock.STOCK_ADJUST_IN_QTY = detail.QTY;

                // 4. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
                stock.STOCK_QTY = stock.STOCK_START_QTY + stock.STOCK_IN_QTY - stock.STOCK_OUT_QTY + stock.STOCK_ADJUST_IN_QTY - stock.STOCK_ADJUST_OUT_QTY;

                this.db.PSC2010_T_STOCK.Add(stock);
                this.db.PSC2011_T_STOCK_DETAIL.Add(detail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateInDataNoTrans(DateTime pYearMonthDate, DateTime pStockDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            try
            {
                PSC2010_T_STOCK psc2010 = this.db.PSC2010_T_STOCK
                                           .Select(x => x)
                                           .Where(x => x.YEAR_MONTH.Year == pYearMonthDate.Year && x.YEAR_MONTH.Month == pYearMonthDate.Month && x.ITEM_CODE.Equals(pItemCode) && x.HEAT_NO.Equals(pHeatNo) && x.LOCATION_CODE.Equals(pLocationCode))
                                           .FirstOrDefault();

                if (psc2010 == null)
                {
                    InsertDataNoTrans(pYearMonthDate, pStockDate, pItemCode, pHeatNo, pLocationCode, Convert.ToInt32(pQty), pUserId);
                }
                else
                {
                    //Insert PSC2011_T_STOCK_DETAIL
                    PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
                    detail.YEAR_MONTH = pYearMonthDate;
                    detail.ITEM_CODE = pItemCode;
                    detail.HEAT_NO = pHeatNo;
                    detail.LOCATION_CODE = pLocationCode;
                    detail.STOCK_IN_DATE = pStockDate;
                    detail.QTY = pQty;
                    detail.STOCK_IO_TYPE = Constants.STOCKTAKING_IN;
                    detail.CREATE_USER_ID = pUserId;
                    detail.UPDATE_USER_ID = pUserId;
                    detail.UPDATE_DATE = DateTime.Now;
                    detail.CREATE_DATE = DateTime.Now;

                    //Update PSC2010_T_STOCK
                    psc2010.STOCK_ADJUST_IN_QTY += detail.QTY;
                    psc2010.STOCK_QTY = psc2010.STOCK_START_QTY + psc2010.STOCK_IN_QTY - psc2010.STOCK_OUT_QTY + psc2010.STOCK_ADJUST_IN_QTY - psc2010.STOCK_ADJUST_OUT_QTY;

                    this.db.PSC2011_T_STOCK_DETAIL.Add(detail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateStockOutData(DateTime pYearMonthDate,DateTime pStockInDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            try
            {
                PSC2010_T_STOCK psc2010 = this.db.PSC2010_T_STOCK
                                           .Select(x => x)
                                           .Where(x => x.YEAR_MONTH.Year == pYearMonthDate.Year && x.YEAR_MONTH.Month == pYearMonthDate.Month && x.ITEM_CODE.Equals(pItemCode) && x.HEAT_NO.Equals(pHeatNo) && x.LOCATION_CODE.Equals(pLocationCode))
                                           .FirstOrDefault();
                
                if (psc2010 != null)
                {
                    // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY - PSC2010_T_STOCK.STOCK_QTY || PSC2010_T_STOCK.STOCK_QTY - PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY)
                    PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
                    detail.YEAR_MONTH = pYearMonthDate;
                    detail.ITEM_CODE = pItemCode;
                    detail.HEAT_NO = pHeatNo;
                    detail.LOCATION_CODE = pLocationCode;
                    detail.STOCK_IN_DATE = pStockInDate;
                    detail.QTY = pQty;
                    detail.STOCK_IO_TYPE = Constants.STOCKTAKING_OUT;

                    // 2. Update PSC2010_T_STOCK.STOCK_ADJUST_OUT_QTY = PSC2011_T_STOCK_DETAIL.QTY
                    psc2010.STOCK_ADJUST_OUT_QTY += detail.QTY;

                    detail.CREATE_USER_ID = pUserId;
                    detail.UPDATE_USER_ID = pUserId;
                    detail.UPDATE_DATE = DateTime.Now;
                    detail.CREATE_DATE = DateTime.Now;

                    // 3. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
                    psc2010.STOCK_QTY = psc2010.STOCK_START_QTY + psc2010.STOCK_IN_QTY - psc2010.STOCK_OUT_QTY + psc2010.STOCK_ADJUST_IN_QTY - psc2010.STOCK_ADJUST_OUT_QTY;

                    this.db.PSC2011_T_STOCK_DETAIL.Add(detail);
                }
                   
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateStockInData(DateTime pYearMonthDate, DateTime pStockInDate, string pItemCode, string pHeatNo, string pLocationCode, decimal pQty, string pUserId)
        {
            try
            { 
                PSC2010_T_STOCK psc2010 = this.db.PSC2010_T_STOCK
                                           .Select(x => x)
                                           .Where(x => x.YEAR_MONTH.Year == pYearMonthDate.Year && x.YEAR_MONTH.Month == pYearMonthDate.Month && x.ITEM_CODE.Equals(pItemCode) && x.HEAT_NO.Equals(pHeatNo) && x.LOCATION_CODE.Equals(pLocationCode))
                                           .FirstOrDefault();

                if (psc2010 == null)
                {
                    InsertData(pYearMonthDate, pStockInDate, pItemCode, pHeatNo, pLocationCode, Convert.ToInt32(pQty), pUserId);
                }
                else
                {
                    // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY - PSC2010_T_STOCK.STOCK_QTY || PSC2010_T_STOCK.STOCK_QTY - PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY) --- Cenceled 2019/05/10
                 
                    PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
                    detail.YEAR_MONTH = pYearMonthDate;
                    detail.ITEM_CODE = pItemCode;
                    detail.HEAT_NO = pHeatNo;
                    detail.LOCATION_CODE = pLocationCode;
                    detail.STOCK_IN_DATE = pStockInDate;
                    detail.QTY = pQty; //- psc2010.STOCK_QTY;
                    detail.STOCK_IO_TYPE = Constants.STOCKTAKING_IN;

                    // 2. Update PSC2010_T_STOCK.STOCK_ADJUST_IN_QTY = PSC2011_T_STOCK_DETAIL.QTY
                    psc2010.STOCK_ADJUST_IN_QTY += detail.QTY;

                    detail.CREATE_USER_ID = pUserId;
                    detail.UPDATE_USER_ID = pUserId;
                    detail.UPDATE_DATE = DateTime.Now;
                    detail.CREATE_DATE = DateTime.Now;

                    // 3. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
                    psc2010.STOCK_QTY = psc2010.STOCK_START_QTY + psc2010.STOCK_IN_QTY - psc2010.STOCK_OUT_QTY + psc2010.STOCK_ADJUST_IN_QTY - psc2010.STOCK_ADJUST_OUT_QTY;

                    this.db.PSC2011_T_STOCK_DETAIL.Add(detail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}