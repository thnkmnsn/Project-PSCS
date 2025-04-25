using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using PSCS.Common;
using System.Transactions;
using System.Text;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class ReceiveMonitorService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public ReceiveMonitorService(PSCSEntities pDb)
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
        public List<ReceiveMonitorScreen> GetActiveReceiveData()
        {
            List<ReceiveMonitorScreen> result = new List<ReceiveMonitorScreen>();
            try
            {
                using (this.db)
                {
                    //result = db.PSC3100_RECEIVEING_DATA
                    //        .Where(x => x.STATUS > 0)
                    //        .OrderByDescending(x => x.TRAN_NO)
                    //        .AsEnumerable()
                    //        .Select((x, index) => new ReceiveMonitor
                    //        {
                    //            RowNo = index + 1,
                    //            DeliveryDate = x.DELIVERY_DATE,
                    //            ContainerNo = x.CONTAINER_NO,
                    //            ItemCode = x.ITEM_CODE,
                    //            HeatNo = x.HEAT_NO,
                    //            Status = x.STATUS ,
                    //            StatusText = x.STATUS == (int)Constants.ReceiveStatus.New ? Constants.ReceiveStatus.New.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Plan ? Constants.ReceiveStatus.Plan.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Receive ? Constants.ReceiveStatus.Receive.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Submit ? Constants.ReceiveStatus.Submit.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Approve ? Constants.ReceiveStatus.Approve.ToString() : ""
                    //        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<ReceiveMonitorScreen> GetReceiveDataList(DateTime? pDeliveryDate, string pContainerNo, string pStatus)
        {
            List<ReceiveMonitorScreen> result = new List<ReceiveMonitorScreen>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    //var obj = (from re in db.PSC3100_RECEIVEING_DATA
                    //           where re.STATUS > 0
                    //           orderby re.TRAN_NO descending
                    //           select new
                    //           {
                    //               re.TRAN_NO,
                    //               re.ITEM_CODE,
                    //               re.HEAT_NO,
                    //               re.CONTAINER_NO,
                    //               re.DELIVERY_DATE,
                    //               re.RECEIVED_DATE,
                    //               re.QTY,
                    //               re.BUNDLES,
                    //               re.PO_NO,
                    //               re.REMARK,
                    //               re.STATUS
                    //           }).AsQueryable();


                    //// Delivery Date
                    //if (pDeliveryDate != null)
                    //{
                    //    DateTime deliveryDate = Convert.ToDateTime(pDeliveryDate).Date;
                    //    obj = obj.Where(x => x.DELIVERY_DATE == deliveryDate);
                    //}

                    //// Contain No
                    //if (!string.IsNullOrEmpty(pContainerNo))
                    //{
                    //    obj = obj.Where(x => x.CONTAINER_NO.Contains(pContainerNo));
                    //}

                    //// Status
                    //if (pStatus != null)
                    //{
                    //    int Status = Int32.Parse(pStatus);
                    //    obj = obj.Where(x => x.STATUS == Status);
                    //}

                    //result = obj.AsEnumerable()
                    //        .Select((x, index) => new ReceiveMonitor
                    //        {
                    //            RowNo = index + 1,
                    //            DeliveryDate = x.DELIVERY_DATE,
                    //            ContainerNo = x.CONTAINER_NO,
                    //            ItemCode = x.ITEM_CODE,
                    //            HeatNo = x.HEAT_NO,
                    //            Status = x.STATUS,
                    //            StatusText = x.STATUS == (int)Constants.ReceiveStatus.New ? Constants.ReceiveStatus.New.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Plan ? Constants.ReceiveStatus.Plan.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Receive ? Constants.ReceiveStatus.Receive.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Submit ? Constants.ReceiveStatus.Submit.ToString() :
                    //                         x.STATUS == (int)Constants.ReceiveStatus.Approve ? Constants.ReceiveStatus.Approve.ToString() : ""
                    //        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<ReceiveMonitorScreen> Search(ReceiveMonitorScreen FilterModel)
        {
            List<ReceiveMonitorScreen> result = new List<ReceiveMonitorScreen>();

            try
            {
                result = GetReceiveDataList(FilterModel.FilterDeliveryDate, FilterModel.FilterContainerNo, FilterModel.FilterStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        #endregion
    }
}