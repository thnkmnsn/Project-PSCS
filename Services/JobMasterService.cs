using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Common;
using System.Transactions;
using PSCS.Models;
using PSCS.ModelERPDEV01;

namespace PSCS.Services
{
    public class JobMasterService
    {
        // Attribute 
        private PSCSEntities db;
        private string UserId { get; set; }

        // Constructor 
        public JobMasterService(PSCSEntities pDb, string pUserId)
        {
            try
            {
                this.db = pDb;
                this.UserId = pUserId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean JobMaster(List<JobMasterData> pJobMasterDataList)
        {
            Boolean result = false;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime insertDate = DateTime.Now;

                        if (pJobMasterDataList != null)
                        {
                            int? intTransNo = this.db.PSC3300_T_SYTELINE_JOB_MASTER.Max(pi => (int?)pi.TRANS_NO);
                            int intNewTransNo = 0;
                            foreach (JobMasterData en in pJobMasterDataList)
                            {
                                intNewTransNo = intNewTransNo == 0 ? (Convert.ToInt32(intTransNo == null ? 1 : intTransNo + 1)) : intNewTransNo + 1;
                                PSC3300_T_SYTELINE_JOB_MASTER insert = new PSC3300_T_SYTELINE_JOB_MASTER();

                                insert.TRANS_NO = intNewTransNo;
                                insert.JOB_NO = en.Job;
                                insert.MFG_NO = en.MFG;
                                insert.ITEM_CODE = en.ItemCode;
                                insert.HEAT_NO = en.Heat;
                                insert.QTY =  Convert.ToDecimal(en.Qty);
                                insert.DESCRIPTION = en.Description;
                                insert.Product = en.Product;
                                insert.GroupMFGno = en.GroupMFGno;
                                insert.MFGnum = en.MFGnum;
                                insert.CREATE_USER_ID = this.UserId;
                                insert.UPDATE_USER_ID = this.UserId;
                                insert.CREATE_DATE = DateTime.Now;
                                insert.UPDATE_DATE = DateTime.Now;

                                this.db.PSC3300_T_SYTELINE_JOB_MASTER.Add(insert);

                                var objUpdate = this.db.PSC2410_T_RELEASE.SingleOrDefault(pi => pi.JOB_NO == en.Job);

                                // Check duplicate data before insert
                                if (objUpdate == null)
                                {
                                    PSC2410_T_RELEASE insertItem = new PSC2410_T_RELEASE();

                                    insertItem.JOB_NO = en.Job;
                                    //insertItem.RELEASE_DATE = DateTime.Now;
                                    insertItem.ITEM_CODE = en.ItemCode;
                                    insertItem.MFG_NO = en.MFG;
                                    insertItem.ITEM_CODE = en.ItemCode;
                                    insertItem.HEAT_NO = en.Heat;
                                    insertItem.QTY = Convert.ToDecimal(en.Qty);
                                    insertItem.RELEASE_QTY = 0;
                                    insertItem.REQUEST_QTY = 0;
                                    insertItem.STATUS = 1;
                                    insertItem.CREATE_USER_ID = this.UserId;
                                    insertItem.CREATE_DATE = DateTime.Now;
                                    insertItem.UPDATE_USER_ID = this.UserId;
                                    insertItem.UPDATE_DATE = DateTime.Now;

                                    this.db.PSC2410_T_RELEASE.Add(insertItem);
                                }
                                else
                                {
                                    //objUpdate.ITEM_CODE = en.ItemCode;
                                    //objUpdate.MFG_NO = en.MFG;
                                    //objUpdate.ITEM_CODE = en.ItemCode;
                                    //objUpdate.HEAT_NO = en.Heat;
                                    //objUpdate.QTY = Convert.ToDecimal(en.Qty);
                                    //objUpdate.RELEASE_QTY = 0;
                                    //objUpdate.RECEIVE_QTY = 0;
                                    //objUpdate.STATUS = 1;
                                    //objUpdate.UPDATE_USER_ID = this.UserId;
                                    //objUpdate.UPDATE_DATE = DateTime.Now;
                                }
                            }
                        }
                        
                        int SaveChangeResult = this.db.SaveChanges();

                        if (SaveChangeResult > 0)
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

                return result;
            }
        }
    }
}