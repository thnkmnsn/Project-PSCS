using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using PSCS.Common;
using System.Transactions;
using System.Text;

namespace PSCS.Services
{
    public class ReceivePlanService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public ReceivePlanService(PSCSEntities pDb)
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

        public List<ReceivePlan> GetReceivePlanList(DateTime? pDeliveryDate, string pContainerNo, string language)
        {
            List<ReceivePlan> result = new List<ReceivePlan>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2110_T_RECEIVING_INSTRUCTION
                               where re.STATUS == (byte)Constants.ReceiveStatus.New || re.STATUS == (byte)Constants.ReceiveStatus.Receive
                               select new
                               {
                                   re.RECEIVE_ID,
                                   re.DELIVERY_DATE,
                                   re.RECEIVED_DATE,
                                   re.CONTAINER_NO,
                                   re.TRUCK_NUMBER,
                                   re.START_TIME,
                                   re.FINISHED_TIME,
                                   re.STATUS
                               }).AsQueryable();

                    // Delivery Date
                    if (pDeliveryDate != null)
                    {
                        DateTime deliveryDate = Convert.ToDateTime(pDeliveryDate).Date;
                        obj = obj.Where(x => x.DELIVERY_DATE == deliveryDate);
                    }

                    // Contain No
                    if (!string.IsNullOrEmpty(pContainerNo))
                    {
                        obj = obj.Where(x => x.CONTAINER_NO.Contains(pContainerNo));
                    }

                    result = obj.AsEnumerable()
                      .Select((x, index) => new ReceivePlan
                      {
                          RowNo = index + 1,
                          ReceiveId = x.RECEIVE_ID,
                          DeliveryDate = x.DELIVERY_DATE,
                          ContainerNo = x.CONTAINER_NO,
                          TruckNo = x.TRUCK_NUMBER,
                          Status = x.STATUS,
                          StatusText = GetStatus(x.STATUS)
                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReceivePlan> GetReceivedPlanList(DateTime? pDeliveryDate, string pContainerNo, string language)
        {
            List<ReceivePlan> result = new List<ReceivePlan>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2110_T_RECEIVING_INSTRUCTION
                               where re.STATUS == (byte)Constants.ReceiveStatus.Approve
                               select new
                               {
                                   re.RECEIVE_ID,
                                   re.DELIVERY_DATE,
                                   re.RECEIVED_DATE,
                                   re.CONTAINER_NO,
                                   re.TRUCK_NUMBER,
                                   re.START_TIME,
                                   re.FINISHED_TIME,
                                   re.STATUS
                               }).AsQueryable();

                    // Delivery Date
                    if (pDeliveryDate != null)
                    {
                        DateTime deliveryDate = Convert.ToDateTime(pDeliveryDate).Date;
                        obj = obj.Where(x => x.DELIVERY_DATE == deliveryDate);
                    }

                    // Contain No
                    if (!string.IsNullOrEmpty(pContainerNo))
                    {
                        obj = obj.Where(x => x.CONTAINER_NO.Contains(pContainerNo));
                    }

                    result = obj.AsEnumerable()
                      .Select((x, index) => new ReceivePlan
                      {
                          RowNo = index + 1,
                          ReceiveId = x.RECEIVE_ID,
                          DeliveryDate = x.DELIVERY_DATE,
                          ContainerNo = x.CONTAINER_NO,
                          TruckNo = x.TRUCK_NUMBER,
                          Status = x.STATUS,
                          StatusText = GetStatus(x.STATUS)
                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReceiveItemsPlan> GetReceivePlanAndDetailList(DateTime? pDeliveryDate, string pContainerNo, string language)
        {
            List<ReceiveItemsPlan> result = new List<ReceiveItemsPlan>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2110_T_RECEIVING_INSTRUCTION
                               join rd in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL on re.RECEIVE_ID equals rd.RECEIVE_ID
                               join pi in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                               where re.STATUS == (byte)Constants.ReceiveStatus.New || re.STATUS == (byte)Constants.ReceiveStatus.Receive
                               select new
                               {
                                   re.RECEIVE_ID,
                                   re.DELIVERY_DATE,
                                   re.CONTAINER_NO,
                                   rd.ITEM_CODE,
                                   rd.HEAT_NO,
                                   pi.OD,
                                   pi.WT,
                                   pi.LT,
                                   re.STATUS
                               }).AsQueryable();

                    // Delivery Date
                    if (pDeliveryDate != null)
                    {
                        DateTime deliveryDate = Convert.ToDateTime(pDeliveryDate).Date;
                        obj = obj.Where(x => x.DELIVERY_DATE == deliveryDate);
                    }

                    // Contain No
                    if (!string.IsNullOrEmpty(pContainerNo))
                    {
                        obj = obj.Where(x => x.CONTAINER_NO.Contains(pContainerNo));
                    }

                    result = obj.AsEnumerable()
                      .Select((x, index) => new ReceiveItemsPlan
                      {
                          RowNo = index + 1,
                          ReceiveId = x.RECEIVE_ID,
                          DeliveryDate = x.DELIVERY_DATE,
                          ContainerNo = x.CONTAINER_NO,
                          ItemCode = x.ITEM_CODE,
                          HeatNo = x.HEAT_NO,
                          OD = x.OD,
                          WT = x.WT,
                          Length = x.LT
                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReceiveItemsPlan> GetReceivedPlanAndDetailList(DateTime? pDeliveryDate, string pContainerNo, string language)
        {
            List<ReceiveItemsPlan> result = new List<ReceiveItemsPlan>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2110_T_RECEIVING_INSTRUCTION
                               join rd in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL on re.RECEIVE_ID equals rd.RECEIVE_ID
                               join pi in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                               where re.STATUS == (byte)Constants.ReceiveStatus.Approve
                               select new
                               {
                                   re.RECEIVE_ID,
                                   re.DELIVERY_DATE,
                                   re.CONTAINER_NO,
                                   rd.ITEM_CODE,
                                   rd.HEAT_NO,
                                   pi.OD,
                                   pi.WT,
                                   pi.LT,
                                   re.STATUS
                               }).AsQueryable();

                    // Delivery Date
                    if (pDeliveryDate != null)
                    {
                        DateTime deliveryDate = Convert.ToDateTime(pDeliveryDate).Date;
                        obj = obj.Where(x => x.DELIVERY_DATE == deliveryDate);
                    }

                    // Contain No
                    if (!string.IsNullOrEmpty(pContainerNo))
                    {
                        obj = obj.Where(x => x.CONTAINER_NO.Contains(pContainerNo));
                    }

                    result = obj.AsEnumerable()
                      .Select((x, index) => new ReceiveItemsPlan
                      {
                          RowNo = index + 1,
                          ReceiveId = x.RECEIVE_ID,
                          DeliveryDate = x.DELIVERY_DATE,
                          ContainerNo = x.CONTAINER_NO,
                          ItemCode = x.ITEM_CODE,
                          HeatNo = x.HEAT_NO,
                          OD = x.OD,
                          WT = x.WT,
                          Length = x.LT
                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ReceivingInstructionDetail GetReceive(int pReceiveId, string pItemCode, string pHeatNo)
        {
            ReceivingInstructionDetail result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    List<ReceivingInstructionDetail> queryResult =
                        (from re in this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                         where re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo && re.RECEIVE_ID == pReceiveId
                         select new ReceivingInstructionDetail
                         {
                             RecevedID = re.RECEIVE_ID,
                             ItemCode = re.ITEM_CODE,
                             HeatNo = re.HEAT_NO,
                             ActualQty = re.ACTUAL_QTY,
                             Status = re.STATUS,
                         }
                         ).ToList();

                    if(queryResult != null)
                    {
                        result = queryResult[0];
                        result.StatusText = GetStatus(Convert.ToByte(result.Status));
                        if (result.ActualQty == null)
                        {
                            result.ActualQty = 0;
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

        public Boolean SaveData(ReceivingInstruction pReceive, List<ReceivingInstructionDetail> pDetailList, string pUserId)
        {
            Boolean result = false;
            ReceivingInstructionService objReceivingInstructionService = null;
            ReceivingInstructionDetailService objReceivingInstructionDetailService = null;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        result = true;
                        objReceivingInstructionService = new ReceivingInstructionService(this.db);
                        objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.db);
                        foreach (ReceivingInstructionDetail en in pDetailList)
                        {
                            if (result)
                            {
                                result = objReceivingInstructionDetailService.Update(en.RecevedID, en.ItemCode, en.HeatNo, en.Remark,en.Status, pUserId);
                            }
                        }

                        if (result)
                        {
                            result = objReceivingInstructionService.UpdateReceiveData(pReceive, pUserId);
                        }

                        if (result)
                        {
                            tran.Complete();
                            result = true;
                        }
                        else
                        {
                            tran.Dispose();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }

            return result;
        }
        public Boolean SaveData1(ReceivingInstruction pReceive, List<ReceivingInstructionDetail> pDetailList, string pUserId)
        {
            Boolean result = false;
            ReceivingInstructionService objReceivingInstructionService = null;
            ReceivingInstructionDetailService objReceivingInstructionDetailService = null;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        result = true;
                        objReceivingInstructionService = new ReceivingInstructionService(this.db);
                        objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.db);
                        foreach (ReceivingInstructionDetail en in pDetailList)
                        {
                            if (result)
                            {
                                result = objReceivingInstructionDetailService.UpdateActualQty1((decimal)en.Id, en.LocationCode,en.LocationChange, (decimal)en.ActualHistory, (decimal)en.QtyChange, pUserId, en.RecevedID, en.ItemCode, en.HeatNo);
                            }
                        }

                        if (result)
                        {
                            result = objReceivingInstructionService.UpdateReceiveData(pReceive, pUserId);
                        }

                        if (result)
                        {
                            tran.Complete();
                            result = true;
                        }
                        else
                        {
                            tran.Dispose();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }

            return result;
        }


        public Boolean ApproveData(DateTime pYearMonth, DateTime pStockDate, ReceivingInstruction pReceive, List<ReceivingInstructionDetail> pDetailList, string pUserId)
        {
            Boolean result = false;
            List<HHTReceive> objHHTDistinct = null;
            StockListService objStockListService = null;
            ReceivingInstructionService objReceivingInstructionService  = null;
            ReceivingInstructionDetailService objReceivingInstructionDetailService = null;
            
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        result = true;
                        objStockListService = new StockListService(this.db);
                        objReceivingInstructionService = new ReceivingInstructionService(this.db);
                        objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.db);
                        foreach (ReceivingInstructionDetail en in pDetailList)
                        {
                            if (en.HHTReceiveList != null)
                            {
                                objHHTDistinct = (en.HHTReceiveList.GroupBy(r => new { r.RecevedID, r.ItemCode, r.HeatNo, r.LocationCode })
                                                        .Select((m, index) => new HHTReceive
                                                        {
                                                            RecevedID = m.Key.RecevedID,
                                                            ItemCode = m.Key.ItemCode,
                                                            HeatNo = m.Key.HeatNo,
                                                            LocationCode = m.Key.LocationCode,
                                                            ActualQTY = m.Select(r => r.ActualQTY).Sum(),
                                                        })).ToList();

                                if (objHHTDistinct != null)
                                {
                                    if(result)
                                    {
                                        result = objStockListService.ReceiveDataList(pYearMonth, pStockDate, objHHTDistinct, pUserId);
                                    }
                                }
                            }

                            if (result)
                            {
                                result = objReceivingInstructionDetailService.UpdateOnApprove(en.RecevedID, en.ItemCode, en.HeatNo, "", en.Remark, pUserId);
                            }
                        }

                        if (result)
                        {
                            result = objReceivingInstructionService.UpdateReceiveData(pReceive, pUserId);
                        }

                        if (result)
                        {
                            tran.Complete();
                            result = true;
                        }
                        else
                        {
                            tran.Dispose();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }

            return result;
        }
        
        #endregion


        #region Private
        private string GetStatus(byte? pStatus)
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
    }
}