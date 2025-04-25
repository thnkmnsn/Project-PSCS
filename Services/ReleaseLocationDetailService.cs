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
    public class ReleaseLocationDetailService
    {
        private PSCSEntities db;

        public ReleaseLocationDetailService(PSCSEntities pDb)
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
        
        public List<ReleaseLocationDetail> GetReleaseLocationDetailList(decimal pReleaseId)
        {
            List<ReleaseLocationDetail> result = new List<ReleaseLocationDetail>();

            try
            {
                result = (from rl in db.PSC2412_T_RELEASE_LOCATION_DETAIL
                          where rl.RELEASE_ID == pReleaseId
                          select new
                          {
                              rl.RELEASE_ID,
                              rl.LOCATION_CODE,
                          }).AsEnumerable().Select((x, index) => new ReleaseLocationDetail
                          {
                              RowNo = index + 1,
                              ReleaseId = x.RELEASE_ID,
                              LocationCode = x.LOCATION_CODE
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool SaveData(decimal pReleaseId, List<string> pReleaseLocationDetailList, string pUserId)
        {
            Boolean result = false;
            Boolean IsSaveChanges = false;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objReleaseLocationDatailList =this.db.PSC2412_T_RELEASE_LOCATION_DETAIL.Where(x => x.RELEASE_ID == pReleaseId).ToList();
                        if(objReleaseLocationDatailList != null)
                        {
                            if (objReleaseLocationDatailList.Count > 0)
                            {
                                IsSaveChanges = true;
                                foreach (PSC2412_T_RELEASE_LOCATION_DETAIL en in objReleaseLocationDatailList)
                                {
                                    this.db.PSC2412_T_RELEASE_LOCATION_DETAIL.Remove(en);
                                }
                            }
                        }                        

                        if(pReleaseLocationDetailList != null)
                        {
                            if (pReleaseLocationDetailList.Count > 0)
                            {
                                IsSaveChanges = true;
                                foreach (string en in pReleaseLocationDetailList)
                                {
                                    PSC2412_T_RELEASE_LOCATION_DETAIL insert = new PSC2412_T_RELEASE_LOCATION_DETAIL();
                                    insert.RELEASE_ID = pReleaseId;
                                    insert.LOCATION_CODE = en;
                                    insert.CREATE_USER_ID = pUserId;
                                    insert.UPDATE_USER_ID = pUserId;
                                    insert.CREATE_DATE = DateTime.Now;
                                    insert.UPDATE_DATE = DateTime.Now;

                                    this.db.PSC2412_T_RELEASE_LOCATION_DETAIL.Add(insert);
                                }
                            }
                        }
                        
                        if(IsSaveChanges)
                        {
                            int SavRresult = this.db.SaveChanges();

                            if (SavRresult > 0)
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
                        else
                        {
                            result = true;
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
        public bool SaveData1(decimal pReleaseId, List<string> pReleaseLocationDetailList, string pUserId)
        {
            Boolean result = false;
            Boolean IsSaveChanges = false;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objReleaseLocationDatailList = this.db.PSC2412_T_RELEASE_LOCATION_DETAIL.Where(x => x.RELEASE_ID == pReleaseId).ToList();
                        if (objReleaseLocationDatailList != null)
                        {
                            if (objReleaseLocationDatailList.Count > 0)
                            {
                                IsSaveChanges = true;
                                foreach (PSC2412_T_RELEASE_LOCATION_DETAIL en in objReleaseLocationDatailList)
                                {
                                    this.db.PSC2412_T_RELEASE_LOCATION_DETAIL.Remove(en);
                                }
                            }
                        }

                        if (pReleaseLocationDetailList != null)
                        {
                            if (pReleaseLocationDetailList.Count > 0)
                            {
                                IsSaveChanges = true;
                                foreach (string en in pReleaseLocationDetailList)
                                {
                                    PSC2412_T_RELEASE_LOCATION_DETAIL insert = new PSC2412_T_RELEASE_LOCATION_DETAIL();
                                    insert.RELEASE_ID = pReleaseId;
                                    insert.LOCATION_CODE = en;
                                    insert.CREATE_USER_ID = pUserId;
                                    insert.UPDATE_USER_ID = pUserId;
                                    insert.CREATE_DATE = DateTime.Now;
                                    insert.UPDATE_DATE = DateTime.Now;

                                    this.db.PSC2412_T_RELEASE_LOCATION_DETAIL.Add(insert);
                                }
                            }
                        }

                        if (IsSaveChanges)
                        {
                            int SavRresult = this.db.SaveChanges();

                            if (SavRresult > 0)
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
                        else
                        {
                            result = true;
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
    }
}