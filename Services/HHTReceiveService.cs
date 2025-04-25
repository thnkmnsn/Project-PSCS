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
    public class HHTReceiveService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public HHTReceiveService(PSCSEntities pDb)
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
        public List<HHTReceive> GetHHTReceiveList(int pRecevedID, string pItemCode, string pHeatNo, Common.Constants.HHTReceiveStatus pStatus)
        {
            List<HHTReceive> result = new List<HHTReceive>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from re in db.PSC2110_T_HHT_RECEIVE
                               where re.RECEIVE_ID == pRecevedID && re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo
                               && re.STATUS == (byte)pStatus
                              select new HHTReceive
                               {
                                  Id = re.ID,
                                  RecevedID = re.RECEIVE_ID,
                                  ItemCode =  re.ITEM_CODE,
                                  HeatNo = re.HEAT_NO,
                                  LocationCode =  re.LOCATION_CODE,
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

        public List<HHTReceive> GetHHTReceiveListByItemCodeHeadNo(string pItemCode, string pHeatNo)
        {
            List<HHTReceive> result = new List<HHTReceive>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from re in db.PSC2110_T_HHT_RECEIVE
                              where  re.ITEM_CODE == pItemCode && re.HEAT_NO == pHeatNo
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

        public List<HHTReceive> GetHHTReceiveListByReceiveID(int pReceiveID)
        {
            List<HHTReceive> result = new List<HHTReceive>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from re in db.PSC2110_T_HHT_RECEIVE
                              where re.RECEIVE_ID == pReceiveID 
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

        public bool Update(decimal pId, Common.Constants.HHTReceiveStatus pStatus, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2110_T_HHT_RECEIVE.SingleOrDefault(re => re.ID == pId && re.STATUS == (byte)Common.Constants.HHTReceiveStatus.NewTrans);
                        if (update != null)
                        {
                            update.STATUS = Convert.ToByte(pStatus);
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

        #endregion
    }
}