using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PSCS.Services
{
    public class ReceivePlanManagementService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public ReceivePlanManagementService(PSCSEntities pDb)
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

        public List<ReceiveData> GetReceiveDataList(DateTime? pDeliveryDate, string pContainerNo, string pStatus, string pLanguage)
        {
            List<ReceiveData> result = new List<ReceiveData>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2110_T_RECEIVING_INSTRUCTION
                                where re.STATUS > 0
                                select new
                                {
                                    re.RECEIVE_ID,
                                    re.DELIVERY_DATE,
                                    re.RECEIVED_DATE,
                                    re.CONTAINER_NO,
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

                    // Status
                    if (!string.IsNullOrEmpty(pStatus))
                    {
                        int Status = Int32.Parse(pStatus);
                        obj = obj.Where(x => x.STATUS == Status);
                    }

                    result = obj.AsEnumerable()
                      .Select((x, index) => new ReceiveData
                      {
                          RowNo = index + 1,
                          ReceiveId = x.RECEIVE_ID,
                          DeliveryDate = x.DELIVERY_DATE,
                          ContainerNo = x.CONTAINER_NO,
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


        public List<ReceivePlanData> GetReceiveDetailList(string pReceiveId)
        {
            List<ReceivePlanData> result = new List<ReceivePlanData>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    int _id = Int32.Parse(pReceiveId);

                    var obj = (from de in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL
                               join it in db.PSC8010_M_PIPE
                                 on new { item = de.ITEM_CODE, heat = de.HEAT_NO } equals
                                    new { item = it.ITEM_CODE, heat = it.HEAT_NO }
                               where de.RECEIVE_ID == _id
                               orderby de.TRAN_NO
                               select new
                               {
                                   receive_id = de.RECEIVE_ID,
                                   tran_no = de.TRAN_NO,
                                   item_code = de.ITEM_CODE,
                                   heat_no = de.HEAT_NO,
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

                    result = obj.AsEnumerable().Select((x, index) => new ReceivePlanData
                    {
                        RowNo = index + 1,
                        ItemCode = x.item_code,
                        HeatNo = x.heat_no,
                        OD = x.od,
                        WT = x.wt,
                        Length = x.lt,
                        PlanQty = x.qty,
                        PlanBundles = x.bundel,
                        Status = x.status
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public bool UpdateData(int receiveId, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = db.PSC2110_T_RECEIVING_INSTRUCTION
                                     .SingleOrDefault(x => x.RECEIVE_ID == receiveId);

                        if (update != null)
                        {
                            update.STATUS = 2;
                            update.UPDATE_USER_ID = userId;
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
        #endregion


        #region Private
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