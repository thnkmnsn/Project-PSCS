using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Transactions;
using System.Web;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class SyncReceiveDataService
    {
        private PSCSEntities db;

        public SyncReceiveDataService(PSCSEntities pDb)
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
        
        public Int32 GetMaxTranNo()
        {
            try
            {
                Int32 MaxTranNo = 0;
                using (this.db)
                {
                    //MaxTranNo = (from d in db.PSC3100_RECEIVEING_DATA select d.TRAN_NO).DefaultIfEmpty(0).Max();
                }
                return MaxTranNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SyncReceiveDataScreen> GetSyncReceiveData(int pTranNo)
        {
            List<SyncReceiveDataScreen> result = new List<SyncReceiveDataScreen>();
            int i = pTranNo;
            try
            {
                using (this.db)
                {
                    ObjectParameter returnMessage = new ObjectParameter("Returned", typeof(string));

                    while (true)
                    {
                        ObjectResult<PSCS.Models.GetReceive_Result> objTemp = db.GetReceive(i.ToString(), returnMessage);

                        SyncReceiveDataScreen objData = new SyncReceiveDataScreen();
                        foreach (GetReceive_Result objResult in objTemp.ToList())
                        {
                            //objData.RowNo = i;
                            objData.TransactionNo = objResult.TRAN_NO;
                            objData.ItemCode = objResult.ITEM_CODE;
                            objData.HeatNo = objResult.HEAT_NO;
                            objData.ContainerNo = objResult.CONTAINER_NO;
                            objData.DeliveryDate = objResult.DELIVERY_DATE;
                            objData.ReceivedDate = objResult.RECEIVED_DATE;
                            objData.Qty = objResult.QTY;
                            objData.Bundles = objResult.BUNDLES;
                            objData.PONo = objResult.PO_NO;
                            objData.Remark = "";

                            i++;
                            result.Add(objData);
                        }

                        if (returnMessage.Value.Equals("No Record"))
                        {
                            break;
                        }
                        else if (returnMessage.Value.Equals("can't access data"))
                        {
                            //objData.RowNo = i;
                            objData.TransactionNo = i;
                            objData.ItemCode = null;
                            objData.HeatNo = null;
                            objData.ContainerNo = null;
                            objData.DeliveryDate = null;
                            objData.ReceivedDate = null;
                            objData.Qty = null;
                            objData.Bundles = null;
                            objData.PONo = null;
                            objData.Remark = returnMessage.Value.ToString();

                            i++;
                            result.Add(objData);
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

        public List<SyncReceiveDataScreen> GetActiveSyncReceiveData()
        {
            List<SyncReceiveDataScreen> result = new List<SyncReceiveDataScreen>();
            try
            {
                using (this.db)
                {
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean AddData(List<SyncReceiveDataScreen> pReceiveData, User LoginUser)
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

                        foreach (SyncReceiveDataScreen enReceiveData in pReceiveData)
                        {
                            

                            //2. Check PSC8010_M_PIPE
                            PSC8010_M_PIPE psc3100 = db.PSC8010_M_PIPE
                                        .Select(x => x)
                                        .Where(x => x.ITEM_CODE.Equals(enReceiveData.ItemCode) && x.HEAT_NO.Equals(enReceiveData.HeatNo))
                                        .FirstOrDefault();
                            //3. Insert M_PIPE if don't have data
                            if(psc3100 == null)
                            {
                                // add PSC8010_M_PIPE
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
    }
}