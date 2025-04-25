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
    public class RequestReleaseService
    {
        private PSCSEntities db;

        public RequestReleaseService(PSCSEntities pDb)
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

        public RequestRelease GetRequestReleaseByRequestIdAndReleaseId(int pRequestId, int pReleaseId)
        {
            RequestRelease result = new RequestRelease();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = this.db.PSC2421_T_REQUEST_RELEASE
                              .SingleOrDefault(x => x.REQUEST_ID == pRequestId && x.REQUEST_ID == pReleaseId);

                    result = new RequestRelease()
                    {
                        RequestId = obj.REQUEST_ID,
                        ReleaseId = obj.RELEASE_ID,
                        Status = obj.STATUS,
                        CreateUserID = obj.CREATE_USER_ID,
                        CreateDate = obj.CREATE_DATE,
                        UpdateUserID = obj.UPDATE_USER_ID,
                        UpdateDate = obj.UPDATE_DATE
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<RequestRelease> GetRequestReleaseList(int pReleaseId)
        {
            List<RequestRelease> result = new List<RequestRelease>();

            try
            {
                result = (from rr in db.PSC2421_T_REQUEST_RELEASE
                          join rq in db.PSC2420_T_REQUEST on rr.REQUEST_ID equals rq.REQUEST_ID
                          join re in db.PSC2410_T_RELEASE on rq.JOB_NO equals re.JOB_NO
                          where rr.RELEASE_ID == pReleaseId
                          select new
                          {
                              rr.REQUEST_ID,
                              rr.RELEASE_ID,
                              rr.STATUS,
                              rr.CREATE_USER_ID,
                              rr.CREATE_DATE,
                              rr.UPDATE_USER_ID,
                              rr.UPDATE_DATE,
                              rq.JOB_NO,
                              re.MFG_NO
                          }).AsEnumerable().Select((x, index) => new RequestRelease
                          {
                              RequestId = x.REQUEST_ID,
                              ReleaseId = x.RELEASE_ID,
                              Status = x.STATUS,
                              CreateUserID = x.CREATE_USER_ID,
                              CreateDate = x.CREATE_DATE,
                              UpdateUserID = x.UPDATE_USER_ID,
                              UpdateDate = x.UPDATE_DATE,
                              JobNo = x.JOB_NO,
                              MfgNo = x.MFG_NO,
                          }).ToList();
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