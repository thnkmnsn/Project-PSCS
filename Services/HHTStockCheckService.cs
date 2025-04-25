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
    public class HHTStockCheckService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public HHTStockCheckService(PSCSEntities pDb)
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
        
        public List<HHTStockCheck> GetHHTStockCheckByItemCodeHeadNoAndLocation(DateTime pStockCheckDate, string pItemCode, string pHeatNo, string pLocationCode, Constants.HHTStockCheckStatus pStatus)
        {
            List<HHTStockCheck> result = new List<HHTStockCheck>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = (from sc in db.PSC2310_T_HHT_STOCK_CHECK
                              where sc.STOCK_CHECK_DATE.Day == pStockCheckDate.Day
                              && sc.STOCK_CHECK_DATE.Month == pStockCheckDate.Month
                              && sc.STOCK_CHECK_DATE.Year == pStockCheckDate.Year
                              && sc.ITEM_CODE == pItemCode 
                              && sc.HEAT_NO == pHeatNo
                              && sc.LOCATION_CODE == pLocationCode
                              select new HHTStockCheck
                              {
                                  Id = sc.ID,
                                  StockCheckID = sc.STOCK_CHECK_ID,
                                  ItemCode = sc.ITEM_CODE,
                                  HeatNo = sc.HEAT_NO,
                                  LocationCode = sc.LOCATION_CODE,
                                  ActualQTY = (decimal)sc.ACTUAL_QTY,
                                  Status = sc.STATUS,
                                  CreateUserID = sc.CREATE_USER_ID,
                                  UpdateUserID = sc.UPDATE_USER_ID,
                                  CreateDate = sc.CREATE_DATE,
                                  UpdateDate = sc.UPDATE_DATE
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool Update(decimal pId, Common.Constants.HHTStockCheckStatus pStatus, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2310_T_HHT_STOCK_CHECK.SingleOrDefault(sc => sc.ID == pId && sc.STATUS == (byte)Common.Constants.HHTStockCheckStatus.NewTrans);
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