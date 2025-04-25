using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using PSCS.Common;
using System.Transactions;
using System.Text;
using System.Data.Entity.Validation;
using System.Linq.Expressions;

namespace PSCS.Services
{
    public class ReceivingInstructionDetailService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public ReceivingInstructionDetailService(PSCSEntities pDb)
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

        #region Method
        
        public List<ReceivingInstructionDetail> GetReceivingInstructionDetailList1(int RecevedID)
        {
            List<ReceivingInstructionDetail> result = new List<ReceivingInstructionDetail>();
            
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from r in db.PSC2110_T_RECEIVING_INSTRUCTION
                               join rd in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                                 on r.RECEIVE_ID equals rd.RECEIVE_ID
                               join mp in db.PSC8010_M_PIPE
                                 on new { itemCode = rd.ITEM_CODE, heat = rd.HEAT_NO } equals
                                    new { itemCode = mp.ITEM_CODE, heat = mp.HEAT_NO }
                               join l in db.PSC8020_M_LOCATION
                                 on rd.LOCATION_CODE equals l.LOCATION_CODE into lj
                               from l in lj.DefaultIfEmpty()
                               select new
                               {
                                   
                                   RecevedID = rd.RECEIVE_ID,
                                   TranNo = rd.TRAN_NO,
                                   ItemCode = rd.ITEM_CODE,
                                   HeatNo = rd.HEAT_NO,
                                   OD = mp == null ? 0: mp.OD,
                                   Qty = mp == null ? 0: rd.QTY,
                                   LocationName = l == null ? " " : l.NAME,
                                   
                               }).AsQueryable();
            
                    obj = obj.Where(x => x.RecevedID == RecevedID);


                    result = obj.AsEnumerable().Select((x, index) => new ReceivingInstructionDetail
                    {
                        RowNo = index + 1,
                        HeatNo = x.HeatNo,
                        ItemCode = x.ItemCode,
                        Qty = x.Qty,
                        OD = Convert.ToInt32( x.OD == null ? 0 : x.OD),
                        LocationName = x.LocationName,
                    }).ToList();
                
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReceivingInstructionDetail> GetReceivingInstructionDetailList(int pReceiveId)
        {
            List<ReceivingInstructionDetail> result = new List<ReceivingInstructionDetail>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from de in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                               join it in db.PSC8010_M_PIPE_ITEM
                                 on new { item = de.ITEM_CODE, heat = de.HEAT_NO } equals
                                    new { item = it.ITEM_CODE, heat = it.HEAT_NO }
                               where de.RECEIVE_ID == pReceiveId
                               orderby de.TRAN_NO
                               select new
                               {
                                   receive_id = de.RECEIVE_ID,
                                   tran_no = de.TRAN_NO,
                                   item_code = de.ITEM_CODE,
                                   heat_no = de.HEAT_NO,
                                   description = it.DESCRIPTION,
                                   od = it.OD,
                                   wt = it.WT,
                                   lt = it.LT,
                                   qty = de.QTY,
                                   actual_qty = de.ACTUAL_QTY,
                                   location_code = de.LOCATION_CODE,
                                   bundel = de.BUNDLES,
                                   po_no = de.PO_NO,
                                   status = de.STATUS,
                                   remark = de.REMARK
                               }).AsQueryable();

                    result = obj.AsEnumerable().Select((x, index) => new ReceivingInstructionDetail
                    {
                        RowNo = index + 1,
                        RecevedID = x.receive_id,
                        TranNo = x.tran_no,
                        ItemCode = x.item_code,
                        HeatNo = x.heat_no,
                        Description = x.description,
                        OD = Convert.ToDecimal( x.od),
                        WT = Convert.ToDecimal(x.wt),
                        LT = Convert.ToDecimal(x.lt),
                        Qty = x.qty,
                        Bundles = x.bundel,
                        ActualQty = x.actual_qty != null? x.actual_qty : 0,
                        Status = x.status,
                        StatusText = GetStatus(Convert.ToByte(x.status)),
                        Remark = x.remark
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

         #region Private
        private string GetStatus(byte pStatus)
        {
            string result = string.Empty;
            if (pStatus == (byte)Constants.ReceiveStatus.New)
            {
                result = Resources.Common_cshtml.New;
            }
            else if (pStatus == (byte)Constants.ReceiveStatus.Receive)
            {
                result = Resources.Common_cshtml.Receive;
            }
            else if (pStatus == (byte)Constants.ReceiveStatus.Approve)
            {              
                result = Resources.Common_cshtml.Approve;
            }
            else
            {
                result = "";
            }

            return result;
        }
        #endregion

        public ReceivingInstructionDetail GetReceivingInstructionDetailList(int pRecevedID, string pItemCode, string pHeatNo)
        {
            ReceivingInstructionDetail result = null;
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                              .SingleOrDefault(x => x.RECEIVE_ID == pRecevedID && x.ITEM_CODE == pItemCode && x.HEAT_NO == pHeatNo);

                    result = new ReceivingInstructionDetail()
                    {
                        RecevedID = obj.RECEIVE_ID,
                        ItemCode = obj.ITEM_CODE,
                        HeatNo = obj.HEAT_NO,
                        Qty = obj.QTY,
                        ActualQty = obj.ACTUAL_QTY,
                        LocationCode = obj.LOCATION_CODE,
                        Bundles = obj.BUNDLES,
                        TranNo = obj.TRAN_NO,
                        PoNo = obj.PO_NO,
                        Status = obj.STATUS,
                        Remark = obj.REMARK
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public List<Location> GetLocationDropdown()
        {
            List<Location> result = new List<Location>();

            try
            {
                using (this.db)
                {
                    result = db.PSC8020_M_LOCATION
                        .Join(db.PSC8022_M_YARD,
                              pa => pa.YARD,
                              yd => yd.YARD,
                              (pa, yd) => new { pa, yd })
                        .OrderBy(joined => joined.pa.LOCATION_CODE)
                        .Select(joined => new Location
                        {
                            LocationID = joined.pa.LOCATION,
                            LocationCode = joined.pa.LOCATION_CODE,
                            Name = joined.pa.NAME,
                            Yard = joined.yd.YARD,
                            YardName = joined.yd.NAME
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<Location> GetLocationByReceiveID()
        {
            List <Location> result = new List<Location>();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                var obj = (from lo in db.PSC8020_M_LOCATION
                           join ya in db.PSC8022_M_YARD on lo.YARD equals ya.YARD
                           select new Location
                           {
                               LocationID = lo.LOCATION,
                               LocationCode = lo.LOCATION_CODE,
                               Name = lo.NAME,
                               Yard = ya.YARD,
                               YardName = ya.NAME,
                           }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<ReceivingInstructionDetail> GetReceivingInstructionDetailList3(int pReceiveId)
        {
            List<ReceivingInstructionDetail> result = new List<ReceivingInstructionDetail>();
            try
            {
                using (this.db)
                {
                    result = db.PSC2110_T_HHT_RECEIVE.Where(a=> a.RECEIVE_ID == pReceiveId ).Select(x=> new ReceivingInstructionDetail
                               {
                                   RecevedID = x.RECEIVE_ID,
                                   HeatNo = x.HEAT_NO,
                                   LocationChange = x.LOCATION_CODE,
                               }).ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public List<HHTReceive> GetHHTReceiveListByReceiveID1(int pReceiveID, string pHeatNo)
        {
            List<HHTReceive> result = new List<HHTReceive>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from re in db.PSC2110_T_HHT_RECEIVE
                              where re.RECEIVE_ID == pReceiveID &&
                                    re.HEAT_NO == pHeatNo
                              orderby re.CREATE_DATE
                              select new HHTReceive
                              {
                                  RecevedID = re.RECEIVE_ID,
                                  ItemCode = re.ITEM_CODE,
                                  HeatNo = re.HEAT_NO,
                                  LocationCode = re.LOCATION_CODE,
                                  ActualQTY = re.ACTUAL_QTY,
                                  Status = re.STATUS,
                                  CreateUserID = re.CREATE_USER_ID,
                                  UpdateUserID = re.UPDATE_USER_ID,
                                  CreateDate = re.CREATE_DATE,
                                  UpdateDate = re.UPDATE_DATE
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public List<ReceivingInstructionDetail> GetReceivingInstructionDetailList2(int pReceiveId)
        {
            List<ReceivingInstructionDetail> result = new List<ReceivingInstructionDetail>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from de in db.PSC2110_T_HHT_RECEIVE
                               join it in db.PSC8010_M_PIPE_ITEM
                                 on new { item = de.ITEM_CODE, heat = de.HEAT_NO } equals
                                    new { item = it.ITEM_CODE, heat = it.HEAT_NO }
                               join ne in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL on new { rec = de.RECEIVE_ID, heat = de.HEAT_NO, item = de.ITEM_CODE } equals new { rec = ne.RECEIVE_ID, heat = ne.HEAT_NO, item = ne.ITEM_CODE }
                               where de.RECEIVE_ID == pReceiveId //&& de.HEAT_NO == pHeatNo
                               orderby de.RECEIVE_ID
                               select new
                               {
                                   receive_id = de.RECEIVE_ID,
                                   //UID = de.ID,
                                   tran_no = ne.TRAN_NO,
                                   item_code = de.ITEM_CODE,
                                   heat_no = de.HEAT_NO,
                                   description = it.DESCRIPTION,
                                   UID = de.ID,
                                   od = it.OD,
                                   wt = it.WT,
                                   lt = it.LT,
                                   qty = ne.QTY,
                                   actual_qty = de.ACTUAL_QTY,
                                   Total = ne.ACTUAL_QTY,
                                   location_code = de.LOCATION_CODE,
                                   bundel = ne.BUNDLES,
                                   po_no = ne.PO_NO,
                                   status = de.STATUS,
                                   remark = ne.REMARK
                               }).AsQueryable();

                    result = obj.AsEnumerable().Select((x, index) => new ReceivingInstructionDetail
                    {
                        RowNo = index + 1,
                        Id = x.UID,
                        RecevedID = x.receive_id,
                        TranNo = x.tran_no,
                        ItemCode = x.item_code,
                        HeatNo = x.heat_no,
                        Description = x.description,
                        OD = Convert.ToDecimal(x.od),
                        WT = Convert.ToDecimal(x.wt),
                        LT = Convert.ToDecimal(x.lt),
                        Qty = x.qty,
                        Bundles = x.bundel,
                        TotalActualQty = x.Total,
                        ActualQty = x.actual_qty != null ? x.actual_qty : 0,
                        Status = x.status,
                        StatusText = GetStatus(Convert.ToByte(x.status)),
                        LocationCode = x.location_code,
                        LocationChange = x.location_code,
                        ActualHistory = x.Total,
                        QtyChange = x.actual_qty,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public bool UpdateActualQtyAndStatus(int pRecevedID, string pItemCode, string pHeatNo, decimal pActualQty, Common.Constants.ReceiveDetailStatus pStatus, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo && re.STATUS == (byte)Common.Constants.ReceiveDetailStatus.New);
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

        public bool UpdateActualQty(int pRecevedID, string pItemCode, string pHeatNo, decimal pActualQty, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo);
                        if (update != null)
                        {
                            update.ACTUAL_QTY = update.ACTUAL_QTY + pActualQty;
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
        public bool Update(int pRecevedID, string pItemCode, string pHeatNo, string pRemark, int pStatus, string pUserId)
        {
            //using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            //{
            //    try
            //    {
            //        using (this.db)
            //        {
            //            db.Configuration.LazyLoadingEnabled = false;
            //            DateTime updateDate = DateTime.Now;

            //            var update = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo);
            //            //var update2 = this.db.PSC2110_T_RECEIVING_INSTRUCTION.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID);
            //            //if (update2 != null)
            //            //{
            //            //    update2.TRUCK_NUMBER = pTruckNo;                         
            //            //    update2.UPDATE_USER_ID = pUserId;
            //            //    update2.UPDATE_DATE = updateDate;
            //            //}
            //            if (update != null)
            //            {
            //                update.REMARK = pRemark;
            //                update.STATUS = pStatus;
            //                update.UPDATE_USER_ID = pUserId;
            //                update.UPDATE_DATE = updateDate;
                           
            //            }
            //            int result = db.SaveChanges();

            //            if (result > 0)
            //            {
            //                tran.Complete();
            //                return true;
            //            }
            //            else
            //            {
            //                tran.Dispose();
            //                return false;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        tran.Dispose();
            //        throw ex;
            //    }
            //}


            Boolean result = false;

            try
            {
                DateTime updateDate = DateTime.Now;

                var update = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo);
                
                if (update != null)
                {
                    update.REMARK = pRemark;
                    update.UPDATE_USER_ID = pUserId;
                    update.UPDATE_DATE = updateDate;

                }
                int intSaveChanges = db.SaveChanges();

                if (intSaveChanges > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool UpdateOnApprove(int pRecevedID, string pItemCode, string pHeatNo, string pTruckNo, string pRemark, string pUserId)
        {
            Boolean result = false;

            try
            {
                DateTime updateDate = DateTime.Now;

                var update = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo);
                var update2 = this.db.PSC2110_T_RECEIVING_INSTRUCTION.SingleOrDefault(re => re.RECEIVE_ID == pRecevedID);
                if (update2 != null)
                {
                    update2.TRUCK_NUMBER = pTruckNo;
                    update2.UPDATE_USER_ID = pUserId;
                    update2.UPDATE_DATE = updateDate;
                }
                if (update != null)
                {
                    update.REMARK = pRemark;
                    update.UPDATE_USER_ID = pUserId;
                    update.UPDATE_DATE = updateDate;

                }
                int intSaveChanges = db.SaveChanges();

                if (intSaveChanges > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool UpdateActualQty1(decimal pId, string Location, string newLocationChange, decimal pActualHistory, decimal newActualQty, string pUserId, int pRecevedID, string pItemCode, string pHeatNo)
        {
            bool result = false;

            try
            {
                DateTime updateDate = DateTime.Now;

                // Retrieve the existing record to update in PSC2110_T_HHT_RECEIVE
                var updateRecord = this.db.PSC2110_T_HHT_RECEIVE
                    .SingleOrDefault(re => re.ID == pId && re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo);

                if (updateRecord != null)
                {
                    // Update the LOCATION_CODE, UPDATE_USER_ID, and UPDATE_DATE fields
                    updateRecord.LOCATION_CODE = newLocationChange;
                    updateRecord.UPDATE_USER_ID = pUserId;
                    updateRecord.UPDATE_DATE = updateDate;

                    // Update the Actual_QTY with the new value
                    updateRecord.ACTUAL_QTY = newActualQty;

                    // Save changes to PSC2110_T_HHT_RECEIVE
                    int saveChanges = this.db.SaveChanges();

                    // If the changes are saved successfully, update the PSC2111 table
                    if (saveChanges > 0)
                    {
                        // Calculate the sum of ACTUAL_QTY for all matching records in PSC2110
                        decimal totalActualQty = this.db.PSC2110_T_HHT_RECEIVE
                            .Where(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo)
                            .Sum(re => re.ACTUAL_QTY);

                        var updatePsc2111 = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                            .SingleOrDefault(re => re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo);

                        if (updatePsc2111 != null)
                        {
                            // Update the Actual_QTY in PSC2111 with the summed value
                            updatePsc2111.ACTUAL_QTY = totalActualQty;
                            updatePsc2111.UPDATE_USER_ID = pUserId;
                            updatePsc2111.UPDATE_DATE = updateDate;
                        }
                        else
                        {
                            // If the record doesn't exist in PSC2111, create a new one
                            updatePsc2111 = new PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                            {
                                RECEIVE_ID = pRecevedID,
                                ITEM_CODE = pItemCode,
                                HEAT_NO = pHeatNo,
                                ACTUAL_QTY = totalActualQty,
                                UPDATE_USER_ID = pUserId,
                                UPDATE_DATE = updateDate,
                                // Include other required fields for PSC2111 if necessary
                            };
                            this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.Add(updatePsc2111);
                        }

                        saveChanges = this.db.SaveChanges();

                        // Insert or update the record in PSC2113_T_RECEIVING_INSTRUCTION_ADJUST
                        var adjustRecord = this.db.PSC2113_T_RECEIVING_INSTRUCTION_ADJUST
                            .SingleOrDefault(adj => adj.RECEIVE_ID == pRecevedID && adj.ITEM_CODE == pItemCode && adj.HEAT_NO == pHeatNo);

                        if (adjustRecord != null)
                        {
                            // Update the existing record
                            adjustRecord.LOCATION_CODE = Location;
                            adjustRecord.ACTUAL_QTY = pActualHistory;
                            adjustRecord.LOCATION_CHANGE = newLocationChange;
                            adjustRecord.QTY_CHANGE = totalActualQty;
                            adjustRecord.CREATE_USER_ID = pUserId;
                            adjustRecord.CREATE_DATE = updateDate;
                            adjustRecord.UPDATE_USER_ID = pUserId;
                            adjustRecord.UPDATE_DATE = updateDate;
                        }
                        else
                        {
                            // Insert a new record
                            this.db.PSC2113_T_RECEIVING_INSTRUCTION_ADJUST.Add(new PSC2113_T_RECEIVING_INSTRUCTION_ADJUST
                            {
                                RECEIVE_ID = pRecevedID,
                                ITEM_CODE = pItemCode,
                                HEAT_NO = pHeatNo,
                                ACTUAL_QTY = totalActualQty,
                                CREATE_USER_ID = pUserId,
                                CREATE_DATE = updateDate,
                                QTY_CHANGE = totalActualQty,
                                UPDATE_USER_ID = pUserId,
                                UPDATE_DATE = updateDate,
                                LOCATION_CODE = Location,
                                LOCATION_CHANGE = newLocationChange,
                                // Include other required fields for PSC2113 if necessary
                            });
                        }

                        saveChanges = this.db.SaveChanges();
                        result = saveChanges > 0;
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private string GetStatus(byte? pStatus)
        {
            string result = string.Empty;
            if (pStatus == 1)
            {
                result = Constants.ReceiveStatus.New.ToString();
            }
            //else if (pStatus == 2)
            //{
            //    result = Constants.ReceiveStatus.Plan.ToString();
            //}
            //else if (pStatus == 3)
            //{
            //    result = Constants.ReceiveStatus.Receive.ToString();
            //}
            //else if (pStatus == 4)
            //{
            //    result = Constants.ReceiveStatus.Submit.ToString();
            //}
            //else if (pStatus == 5)
            //{
            //    result = Constants.ReceiveStatus.Approve.ToString();
            //}
            else
            {
                result = "";
            }

            return result;
        }

        #endregion
    }
}