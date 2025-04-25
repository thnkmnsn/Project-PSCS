using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace PSCS.Services
{
    public class MoveOutService
    {
        private PSCSEntities db;

        public MoveOutService(PSCSEntities pDb)
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

        public List<MoveOut> GetMoveOutListByNullMoveIdandSubmitStatus(DateTime pMovementDate)
        {
            List<MoveOut> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from mo in db.PSC2210_T_HHT_MOVE_OUT
                          where mo.TRAN_DATE.Day == pMovementDate.Day &&
                                mo.TRAN_DATE.Month == pMovementDate.Month &&
                                mo.TRAN_DATE.Year == pMovementDate.Year &&
                                mo.MOVE_ID == null &&
                                mo.STATUS == (int)Common.Constants.HHTMoveOutStatus.SubmitTrans
                          orderby mo.ID
                          select new MoveOut
                          {
                              ID = mo.ID,
                              TRAN_DATE = mo.TRAN_DATE,
                              MOVE_ID = mo.MOVE_ID,
                              ITEM_CODE = mo.ITEM_CODE,                          
                              HEAT_NO = mo.HEAT_NO,
                              BARCODE = mo.BARCODE,
                              LOCATION_CODE = mo.LOCATION_CODE,
                              ACTUAL_QTY = mo.ACTUAL_QTY,                            
                              STATUS = mo.STATUS,
                              CreateDate = mo.CREATE_DATE,
                              CreateUserID = mo.CREATE_USER_ID,
                              UpdateDate = mo.UPDATE_DATE,
                              UpdateUserID = mo.UPDATE_USER_ID
                          }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<MoveOut> GetMoveOutListByMoveId(decimal pMoveId, Common.Constants.HHTMoveOutStatus pStatus)
        {
            List<MoveOut> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from mo in db.PSC2210_T_HHT_MOVE_OUT
                          where mo.MOVE_ID == pMoveId &&
                                mo.STATUS == (int)pStatus
                          orderby mo.ID
                          select new MoveOut
                          {
                              ID = mo.ID,
                              TRAN_DATE = mo.TRAN_DATE,
                              MOVE_ID = mo.MOVE_ID,
                              ITEM_CODE = mo.ITEM_CODE,
                              HEAT_NO = mo.HEAT_NO,
                              BARCODE = mo.BARCODE,
                              LOCATION_CODE = mo.LOCATION_CODE,
                              ACTUAL_QTY = mo.ACTUAL_QTY,
                              STATUS = mo.STATUS,
                              CreateDate = mo.CREATE_DATE,
                              CreateUserID = mo.CREATE_USER_ID,
                              UpdateDate = mo.UPDATE_DATE,
                              UpdateUserID = mo.UPDATE_USER_ID
                          }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean UpdateStatus(decimal pId, Common.Constants.HHTMoveOutStatus pStatus, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2210_T_HHT_MOVE_OUT.SingleOrDefault(x => x.ID == pId);

                        if (update != null)
                        {
                            update.STATUS = (int)pStatus;

                            update.UPDATE_USER_ID = userId;
                            update.UPDATE_DATE = DateTime.Now;
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

        public Boolean UpdateDeleteStatus(decimal pId, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2210_T_HHT_MOVE_OUT.SingleOrDefault(x => x.ID == pId &&
                                                                                    x.STATUS == (int)Common.Constants.HHTMoveOutStatus.SubmitTrans);

                        if (update != null)
                        {
                            update.STATUS = (int)Common.Constants.HHTMoveOutStatus.DeleteTrans;

                            update.UPDATE_USER_ID = userId;
                            update.UPDATE_DATE = DateTime.Now;
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

    }
}