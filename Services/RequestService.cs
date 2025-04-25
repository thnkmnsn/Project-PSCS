using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using PSCS.Common;
using System.Transactions;
using System.Text;
using System.Data;


namespace PSCS.Services
{
    public class RequestService
    {
        private PSCSEntities db;
        
        public RequestService(PSCSEntities pDb)
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

        public bool Insert(Request pRequest, string pUserId)
        {
            Boolean result = false;
            int intNewRequestId = 0;
            int intNewTranNo = 0;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    if(pRequest != null)
                    {
                        using (this.db)
                        {
                            this.db.Configuration.LazyLoadingEnabled = false;

                            int? intRequestId = this.db.PSC2420_T_REQUEST.Max(pi => (int?)pi.REQUEST_ID);
                            intNewRequestId = Convert.ToInt32(intRequestId == null ? 1 : intRequestId + 1);

                            PSC2420_T_REQUEST insert = new PSC2420_T_REQUEST();
                            insert.REQUEST_ID = intNewRequestId;
                            insert.JOB_NO = pRequest.JobNo;
                            insert.REQUEST_DATE = DateTime.Now;
                            insert.RELEASE_QTY = pRequest.ReleaseQTY;
                            insert.REQUEST_QTY = pRequest.RequestQTY;
                            insert.REMAIN_QTY = pRequest.RemainQTY;
                            insert.STATUS = 1;
                            insert.CREATE_USER_ID = pUserId;
                            insert.UPDATE_USER_ID = pUserId;
                            insert.CREATE_DATE = DateTime.Now;
                            insert.UPDATE_DATE = DateTime.Now;
                            this.db.PSC2420_T_REQUEST.Add(insert);

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

        public bool UpdateData(List<Request> pRequestList, string pUserId)
        {
            Boolean result = false;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        foreach (Request en in pRequestList)
                        {
                            var update = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == en.RequestId);

                            if (update != null)
                            {
                                update.REQUEST_QTY = en.RequestQTY;
                                update.REMAIN_QTY = en.ReleaseQTY - en.RequestQTY;
                                update.UPDATE_USER_ID = pUserId;
                                update.UPDATE_DATE = DateTime.Now;
                            }
                        }

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
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }

            return result;
        }

        public List<Request> GetRequestsList(Common.Constants.RequestStatus pStatus)
        {
            List<Request> result = new List<Request>();

            try
            {
                result = (from rd in db.PSC2420_T_REQUEST
                             join r in db.PSC2410_T_RELEASE on rd.JOB_NO equals r.JOB_NO
                             join mp in db.PSC8010_M_PIPE_ITEM on new { r.ITEM_CODE, r.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                             join m in db.PSC8027_M_MAKER on mp.MAKER equals m.MAKER
                             where rd.STATUS == (int)pStatus
                          select new {
                                rd.REQUEST_ID,
                                rd.JOB_NO,
                                rd.REQUEST_DATE,
                                r.MFG_NO,
                                mp.DESCRIPTION,
                                m.MAKER,
                                m.MAKER_NAME,
                                r.ITEM_CODE,
                                r.HEAT_NO,
                                rd.REQUEST_QTY,
                                rd.RELEASE_QTY,
                                rd.REMAIN_QTY,
                                rd.STATUS,
                             }).AsEnumerable().Select((x, index) => new Request
                             {
                                 RowNo = index + 1,
                                 RequestId = x.REQUEST_ID,
                                 JobNo = x.JOB_NO,
                                 RequestDate = x.REQUEST_DATE,
                                 MfgNo = x.MFG_NO,
                                 Description = x.DESCRIPTION,
                                 Maker = x.MAKER,
                                 Maker_Name = x.MAKER_NAME,
                                 ItemCode=x.ITEM_CODE,
                                 HeatNo=x.HEAT_NO,
                                 RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                                 ReleaseQTY = Math.Round(Convert.ToDecimal(x.RELEASE_QTY), 2),
                                 RemainQTY = Math.Round(Convert.ToDecimal(x.REMAIN_QTY), 2),
                                 Status = x.STATUS,
                             }).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return result;
        }

        public List<Request> GetRequestsList(string pJobNo)
        {
            List<Request> result = new List<Request>();

            try
            {
                result = (from rd in db.PSC2420_T_REQUEST
                          join r in db.PSC2410_T_RELEASE on rd.JOB_NO equals r.JOB_NO
                          join mp in db.PSC8010_M_PIPE_ITEM on new { r.ITEM_CODE, r.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join m in db.PSC8027_M_MAKER on mp.MAKER equals m.MAKER
                          where rd.JOB_NO == pJobNo
                          select new
                          {
                              rd.REQUEST_ID,
                              rd.JOB_NO,
                              rd.REQUEST_DATE,
                              r.MFG_NO,
                              mp.DESCRIPTION,
                              m.MAKER,
                              m.MAKER_NAME,
                              r.ITEM_CODE,
                              r.HEAT_NO,
                              rd.REQUEST_QTY,
                              rd.RELEASE_QTY,
                              rd.REMAIN_QTY,
                              rd.STATUS,
                          }).AsEnumerable().Select((x, index) => new Request
                          {
                              RowNo = index + 1,
                              RequestId = x.REQUEST_ID,
                              JobNo = x.JOB_NO,
                              RequestDate = x.REQUEST_DATE,
                              MfgNo = x.MFG_NO,
                              Description = x.DESCRIPTION,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                              ReleaseQTY = Math.Round(Convert.ToDecimal(x.RELEASE_QTY), 2),
                              RemainQTY = Math.Round(Convert.ToDecimal(x.REMAIN_QTY), 2),
                              Status = x.STATUS,
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<Request> AddIntoGrid(List<Request> pGridList, Release pAddList)
        {
            List<Request> result = new List<Request>();
            try
            {
                List<Request> objAddListTemp = new List<Request>();
                Request objRequest = new Request();
                objRequest.JobNo = pAddList.JobNo;
                objRequest.ItemCode = pAddList.ItemCode;
                objRequest.HeatNo = pAddList.HeatNo;
                objRequest.MfgNo = pAddList.MfgNo;
                objRequest.Description = pAddList.Description;
                objRequest.Maker = pAddList.Maker;
                objRequest.Maker_Name = pAddList.Maker_Name;
                objRequest.ReleaseQTY = Convert.ToDecimal(pAddList.QTY);
                objAddListTemp.Add(objRequest);
                pGridList.AddRange(objAddListTemp);

                result = pGridList.AsEnumerable()
                        .Select((x, index) => new Request
                        {
                            RowNo = index + 1,
                            RequestDate = DateTime.Now,
                            ItemCode = x.ItemCode,
                            JobNo = x.JobNo,
                            MfgNo = x.MfgNo,
                            HeatNo = x.HeatNo,
                            Description = x.Description,
                            Maker = x.Maker,
                            Maker_Name = x.Maker_Name,
                            ReleaseQTY = x.ReleaseQTY,
                            RequestQTY = x.RequestQTY != null ? Math.Round(Convert.ToDecimal(x.RequestQTY), 2) : Decimal.Parse(x.ReleaseQTY.ToString("0.00")),
                            RemainQTY = x.RemainQTY != null ? Math.Round(Convert.ToDecimal(x.RemainQTY), 2) : 0,
                            RequestId = x.RequestId
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        
        public List<Request> RemoveFromGrid(List<Request> pGridList, string rowNo)
        {
            List<Request> result = new List<Request>();
            try
            {
                int row = Int32.Parse(rowNo);
                var item = pGridList.Where(x => x.RowNo == row).FirstOrDefault();
                pGridList.Remove(item);

                result = pGridList.AsEnumerable()
                       .Select((x, index) => new Request
                       {
                           RowNo = index + 1,
                           RequestDate = DateTime.Now,
                           ItemCode = x.ItemCode,
                           JobNo = x.JobNo,
                           MfgNo = x.MfgNo,
                           HeatNo = x.HeatNo,
                           Description = x.Description,
                           Maker = x.Maker,
                           Maker_Name = x.Maker_Name,
                           RequestQTY = x.RequestQTY != null ? Math.Round(Convert.ToDecimal(x.RequestQTY), 2) : 0,
                           ReleaseQTY = Decimal.Parse(x.ReleaseQTY.ToString("0.00")),
                           RemainQTY = 0,
                           RequestId = x.RequestId,
                       }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        
        public bool SaveData(List<Request> pRequestList, string pUserId)
        {
            Boolean result = false;
            int intNewRequestId = 0;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        int? intRequestId = this.db.PSC2420_T_REQUEST.Max(pi => (int?)pi.REQUEST_ID);
                        intNewRequestId = Convert.ToInt32(intRequestId == null ? 1 : intRequestId + 1);

                        foreach (Request en in pRequestList)
                        {
                            var update = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == en.RequestId);
                           
                            if (update == null)
                            {                              
                                PSC2420_T_REQUEST insert = new PSC2420_T_REQUEST();
                                insert.REQUEST_ID = intNewRequestId;
                                insert.JOB_NO = en.JobNo;
                                insert.REQUEST_DATE = DateTime.Now; 
                                insert.RELEASE_QTY = en.ReleaseQTY;
                                insert.REQUEST_QTY = en.RequestQTY;
                                insert.REMAIN_QTY = en.ReleaseQTY - en.RequestQTY;
                                insert.STATUS = 1;
                                insert.CREATE_USER_ID = pUserId;
                                insert.UPDATE_USER_ID = pUserId;
                                insert.CREATE_DATE = DateTime.Now; 
                                insert.UPDATE_DATE = DateTime.Now; 

                                this.db.PSC2420_T_REQUEST.Add(insert);

                                intNewRequestId = intNewRequestId + 1;
                            }
                            else
                            {
                                update.REQUEST_QTY = en.RequestQTY;
                                update.REMAIN_QTY = en.ReleaseQTY - en.RequestQTY;
                                update.UPDATE_USER_ID = pUserId;
                                update.UPDATE_DATE = DateTime.Now; 
                            }
                        }
                       
                        int SavRresult = this.db.SaveChanges();

                        if (SavRresult > 0)
                        {
                            tran.Complete();
                            result= true;
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
        public bool Delete2(decimal pRequestId)
        {
            bool result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        var objreq = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == pRequestId);
                        if (objreq != null)
                        {
                            var pJobs = objreq.JOB_NO;
                            if (pJobs != null)
                            {
                                var Del1 = this.db.PSC3300_T_SYTELINE_JOB_MASTER.SingleOrDefault(pa => pa.JOB_NO == pJobs);
                                if (Del1 != null)
                                {
                                    db.PSC3300_T_SYTELINE_JOB_MASTER.Remove(Del1);
                                }
                            }
                            flag = this.db.SaveChanges();
                            if (flag > 0)
                            {
                                result = true;
                                tran.Complete();
                            }
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

        public bool Delete(List<Request> pRequestId) //Update V19 02102024
        {
            bool result = false;
            TransactionScope tran = null;
            int flag = 0;
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        foreach (Request en in pRequestId)
                        {
                            var objUsers = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == en.RequestId);
                            if (objUsers != null)
                            {
                                var objJobs = this.db.PSC3300_T_SYTELINE_JOB_MASTER
                                                .Where(x => x.JOB_NO == objUsers.JOB_NO)
                                                .ToList();

                                if (objJobs != null && objJobs.Count > 0)
                                {
                                    foreach (var job in objJobs)
                                    {
                                        this.db.PSC3300_T_SYTELINE_JOB_MASTER.Remove(job);
                                    }
                                }
                                var objJobs1 = this.db.PSC2410_T_RELEASE.Where(pi => pi.JOB_NO == objUsers.JOB_NO).ToList();
                                if(objJobs1 != null && objJobs1.Count > 0)
                                {
                                    foreach(var job1 in objJobs1)
                                    {
                                        this.db.PSC2410_T_RELEASE.Remove(job1);
                                    }
                                }

                                this.db.PSC2420_T_REQUEST.Remove(objUsers);
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag > 0)
                        {
                            tran.Complete();
                            result = true;
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
        public Boolean Delete(decimal pRequestId)
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
                        var objUser = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == pRequestId);
                        if (objUser != null)
                        {
                            var objJobs = this.db.PSC3300_T_SYTELINE_JOB_MASTER
                                                .Where(x => x.JOB_NO == objUser.JOB_NO)
                                                .ToList();

                            if (objJobs != null && objJobs.Count > 0)
                            {
                                foreach (var job in objJobs)
                                {
                                    this.db.PSC3300_T_SYTELINE_JOB_MASTER.Remove(job);
                                }
                            }
                            var objJobs1 = this.db.PSC2410_T_RELEASE.Where(pi => pi.JOB_NO == objUser.JOB_NO).ToList();
                            if (objJobs1 != null && objJobs1.Count > 0)
                            {
                                foreach (var job1 in objJobs1)
                                {
                                    this.db.PSC2410_T_RELEASE.Remove(job1);
                                }
                            }
                            this.db.PSC2420_T_REQUEST.Remove(objUser);
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

        public Boolean Delete4(List<Request> pRequesDeleteList)
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
                        foreach (Request en in pRequesDeleteList)
                        {
                            var objUser = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == en.RequestId);
                            if (objUser != null)
                            {
                                this.db.PSC2420_T_REQUEST.Remove(objUser);                                
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


        public bool UpdateRequestStatus(string pJobNo, string pUserId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;
                        int result = 0;

                        var lstRequest = this.db.PSC2420_T_REQUEST.Where(x => x.JOB_NO == pJobNo).ToList();

                        if (lstRequest != null && lstRequest.Count > 0)
                        {                         
                            foreach (PSC2420_T_REQUEST en in lstRequest)
                            {
                                en.STATUS = (int)Common.Constants.ReleaseDetailStatus.Release;
                                en.UPDATE_USER_ID = pUserId;
                                en.UPDATE_DATE = DateTime.Now;

                                 result = db.SaveChanges();

                                if (result <= 0)
                                {
                                    break;
                                }
                            }

                        }

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
        public bool DeleteMasAndRel(List<PSC2420_T_REQUEST> pRequestId)
        {
            bool result = false;
            string error = string.Empty;
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    this.db.Configuration.LazyLoadingEnabled = false;
                    foreach (PSC2420_T_REQUEST en in pRequestId)
                    {
                        var objRequest = db.PSC2420_T_REQUEST.SingleOrDefault(x => x.JOB_NO == en.JOB_NO);
                        if (objRequest != null)
                        {
                            this.db.PSC2420_T_REQUEST.Remove(objRequest);
                        }
                        var objMaster = db.PSC3300_T_SYTELINE_JOB_MASTER.LastOrDefault(x => x.JOB_NO == en.JOB_NO);
                        if (objMaster != null)
                        {
                            this.db.PSC3300_T_SYTELINE_JOB_MASTER.Remove(objMaster);
                        }
                        var objRelease = db.PSC2410_T_RELEASE.SingleOrDefault(x => x.JOB_NO == en.JOB_NO);
                        if (objRelease != null)
                        {
                            db.PSC2410_T_RELEASE.Remove(objRelease);
                        }
                    }
                    int saveResult = this.db.SaveChanges();

                    if (saveResult > 0)
                    {
                        tran.Complete();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    result = false;
                    throw ex;
                }
            }
        
                return result;
        }
        public bool UpdateData1(List<Request> pRequestList, string pUserId)
        {
            bool result = false;
            string error = string.Empty;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    this.db.Configuration.LazyLoadingEnabled = false;

                    foreach (Request en in pRequestList)
                    {
                        var search1 = db.PSC2410_T_HHT_RELEASE.SingleOrDefault(x => x.JOB_NO == en.JobNo);
                        if (search1 != null)
                        {
                            error = "It has a JOB that has already been released.";
                            return false;
                        }
                        else
                        {
                            var updateDetails = (from rl in db.PSC2411_T_RELEASE_DETAIL
                                                 join rq in db.PSC2421_T_REQUEST_RELEASE on rl.RELEASE_ID equals rq.RELEASE_ID
                                                 join jo in db.PSC2420_T_REQUEST on rq.REQUEST_ID equals jo.REQUEST_ID
                                                 where jo.JOB_NO == en.JobNo
                                                 select rl).ToList();

                            if (updateDetails.Any())
                            {
                                foreach (var update in updateDetails)
                                {
                                    update.STATUS = 0;
                                    update.UPDATE_DATE = DateTime.Now;
                                    update.UPDATE_USER_ID = pUserId;

                                    this.db.Entry(update).State = EntityState.Modified;
                                }
                            }

                            var update1 = this.db.PSC2415_T_DELETE_HISTORY.SingleOrDefault(x => x.DEL_ID == en.RequestId);
                            if (update1 != null)
                            {
                                update1.STATUS = 0;
                                update1.UPDATE_USER_ID = pUserId;
                                update1.UPDATE_DATE = DateTime.Now;
                            }

                            var del1 = db.PSC3300_T_SYTELINE_JOB_MASTER.SingleOrDefault(x => x.JOB_NO == en.JobNo);
                            if (del1 != null)
                            {
                                this.db.PSC3300_T_SYTELINE_JOB_MASTER.Remove(del1);
                            }

                            var del2 = db.PSC2410_T_RELEASE.SingleOrDefault(x => x.JOB_NO == en.JobNo);
                            if (del2 != null)
                            {
                                this.db.PSC2410_T_RELEASE.Remove(del2);
                            }

                            var del3 = db.PSC2420_T_REQUEST.SingleOrDefault(x => x.JOB_NO == en.JobNo);
                            if (del3 != null)
                            {
                                this.db.PSC2420_T_REQUEST.Remove(del3);
                            }
                        }
                    }

                    int saveResult = this.db.SaveChanges();

                    if (saveResult >0)
                    {
                        tran.Complete();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    result = false;
                    throw ex;
                }
            }


            return result;
        }
        public bool UpdateData2(List<Request> pRequestList, string pUserId)
        {
            bool result = false;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    this.db.Configuration.LazyLoadingEnabled = false;

                    foreach (Request en in pRequestList)
                    {
                        var updateDetails = (from rl in db.PSC2411_T_RELEASE_DETAIL
                                             join rq in db.PSC2421_T_REQUEST_RELEASE on rl.RELEASE_ID equals rq.RELEASE_ID
                                             join jo in db.PSC2420_T_REQUEST on rq.REQUEST_ID equals jo.REQUEST_ID
                                             where jo.JOB_NO == en.JobNo
                                             select rl).ToList();

                        if (updateDetails.Any())
                        {
                            foreach (var update in updateDetails)
                            {
                                update.STATUS = 0;
                                update.UPDATE_DATE = DateTime.Now;
                                update.UPDATE_USER_ID = pUserId;

                                this.db.Entry(update).State = EntityState.Modified;
                            }
                        }

                        var update1 = this.db.PSC2415_T_DELETE_HISTORY.SingleOrDefault(x => x.JOB_NO == en.JobNo);

                        if (update1 != null)
                        {
                            //update1.STATUS = 0;
                            update1.UPDATE_USER_ID = pUserId;
                            update1.UPDATE_DATE = DateTime.Now;
                        }
                    }

                    int saveResult = this.db.SaveChanges();

                    if (saveResult >= 1)
                    {
                        tran.Complete();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception message
                    Console.WriteLine($"Error in UpdateData2: {ex.Message}");
                    throw;
                }
            }
            return result;
        }

        public bool Insert1(Request pRequest, string pUserId)
        {
            Boolean result = false;
            int intNewRequestId = 0;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    if (pRequest != null)
                    {
                        using (this.db)
                        {
                            this.db.Configuration.LazyLoadingEnabled = false;

                            int? intRequestId = this.db.PSC2415_T_DELETE_HISTORY.Max(pi => (int?)pi.DEL_ID);
                            intNewRequestId = Convert.ToInt32(intRequestId == null ? 1 : intRequestId + 1);

                            PSC2415_T_DELETE_HISTORY insert = new PSC2415_T_DELETE_HISTORY();
                            insert.DEL_ID = intNewRequestId;
                            insert.JOB_NO = pRequest.JobNo;
                            insert.MFG_NO = pRequest.MfgNo;
                            insert.ITEM_CODE = pRequest.ItemCode;
                            insert.HEAT_NO = pRequest.HeatNo;
                            insert.DESCRIPTION = pRequest.Description;
                            insert.RELEASE_QTY = pRequest.ReleaseQTY;
                            insert.STATUS = 1;
                            insert.CREATE_USER_ID = pUserId;
                            insert.CREATE_DATE = DateTime.Now;
                            this.db.PSC2415_T_DELETE_HISTORY.Add(insert);

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
        public List<Request> GetRequestsList1(Common.Constants.RequestStatus pStatus)
        {
            List<Request> result = new List<Request>();

            try
            {
                result = (from rd in db.PSC2415_T_DELETE_HISTORY
                          join r in db.PSC2410_T_RELEASE on rd.JOB_NO equals r.JOB_NO
                          join mp in db.PSC8010_M_PIPE_ITEM on new { r.ITEM_CODE, r.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join m in db.PSC8027_M_MAKER on mp.MAKER equals m.MAKER
                          join q in db.PSC2420_T_REQUEST on r.JOB_NO equals q.JOB_NO
                          join e in db.PSC2421_T_REQUEST_RELEASE on q.REQUEST_ID equals e.REQUEST_ID

                          where rd.STATUS == (int)pStatus
                          select new
                          {
                              rd.DEL_ID,
                              rd.JOB_NO,
                              r.MFG_NO,
                              mp.DESCRIPTION,
                              m.MAKER,
                              m.MAKER_NAME,
                              r.ITEM_CODE,
                              r.HEAT_NO,
                              rd.RELEASE_QTY,
                              rd.STATUS,
                              e.RELEASE_ID,
                          }).AsEnumerable().Select((x, index) => new Request
                          {
                              RowNo = index + 1,
                              RequestId = x.DEL_ID,
                              JobNo = x.JOB_NO,
                              MfgNo = x.MFG_NO,
                              Description = x.DESCRIPTION,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              ReleaseQTY = Math.Round(Convert.ToDecimal(x.RELEASE_QTY), 2),
                              Status = x.STATUS,
                              ReleaseId = x.RELEASE_ID
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public List<Request> GetRequestsList1(string pJobNo)
        {
            List<Request> result = new List<Request>();

            try
            {
                result = (from rd in db.PSC2415_T_DELETE_HISTORY
                          join mp in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join m in db.PSC8027_M_MAKER on mp.MAKER equals m.MAKER
                          join q in db.PSC2420_T_REQUEST on rd.JOB_NO equals q.JOB_NO
                          join i in db.PSC2421_T_REQUEST_RELEASE on q.REQUEST_ID equals i.REQUEST_ID
                          where rd.JOB_NO == pJobNo
                          select new
                          {
                              rd.DEL_ID,
                              rd.JOB_NO,
                              rd.MFG_NO,
                              mp.DESCRIPTION,
                              m.MAKER,
                              m.MAKER_NAME,
                              rd.ITEM_CODE,
                              rd.HEAT_NO,
                              rd.RELEASE_QTY,
                              rd.STATUS,
                              i.RELEASE_ID
                          }).AsEnumerable().Select((x, index) => new Request
                          {
                              RowNo = index + 1,
                              RequestId = x.DEL_ID,
                              JobNo = x.JOB_NO,
                              MfgNo = x.MFG_NO,
                              Description = x.DESCRIPTION,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              ReleaseQTY = Math.Round(Convert.ToDecimal(x.RELEASE_QTY), 2),
                              Status = x.STATUS,
                              ReleaseId = x.RELEASE_ID
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean Delete1(decimal pDelId)
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
                        var objUser = this.db.PSC2415_T_DELETE_HISTORY.SingleOrDefault(x => x.DEL_ID == pDelId);
                        if (objUser != null)
                        {
                            this.db.PSC2415_T_DELETE_HISTORY.Remove(objUser);
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

        #endregion
    }
}