using OfficeOpenXml.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Transactions;
using ZXing;

namespace PSCS.Services
{
    public class ReleaseDetailService
    {
        private PSCSEntities db;

        public ReleaseDetailService(PSCSEntities pDb)
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

        public ReleaseDetail GetReleaseDetail(decimal pReleaseId)
        {
            ReleaseDetail result = new ReleaseDetail();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = this.db.PSC2411_T_RELEASE_DETAIL
                              .SingleOrDefault(x => x.RELEASE_ID == pReleaseId);

                    result = new ReleaseDetail()
                    {
                        ReleaseId = obj.RELEASE_ID,
                        ItemCode = obj.ITEM_CODE,
                        HeatNo = obj.HEAT_NO,
                        RequestNo = obj.REQUEST_NO,
                        RequestDate = obj.REQUEST_DATE,
                        ReceiveDate = obj.RECEIVE_DATE,
                        RequestQTY = obj.REQUEST_QTY,
                        ActualQTY = obj.ACTUAL_QTY,
                        Status = obj.STATUS,
                        Yard1Remark = obj.Y1_REMARK,
                        Yard2Remark = obj.Y2_REMARK,
                        CuttingRemark = obj.C_REMARK,
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReleaseYardDetail> GetSearchJobs(string pJob)
        {
            List<ReleaseYardDetail> result = new List<ReleaseYardDetail>();

            try
            {
                using (this.db)
                {

                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from jb in db.PSC3300_T_SYTELINE_JOB_MASTER
                               where jb.JOB_NO == pJob
                               orderby jb.JOB_NO
                               select new ReleaseYardDetail
                               {
                                   JobNo = jb.JOB_NO,
                                   MfgNo = jb.MFG_NO,
                                   ItemCode = jb.ITEM_CODE,
                                   HeatNo = jb.HEAT_NO,
                                   RequestQTY = jb.QTY,
                                   Description = jb.DESCRIPTION,
                                   ProductName = jb.Product,
                               }).AsQueryable();

                    result = obj.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean UpdateStatusReleaseDetailList(List<ReleaseDetail> pDetailList, User LoginUser)
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
                        foreach (ReleaseDetail en in pDetailList)
                        {
                            PSC2411_T_RELEASE_DETAIL psc2411 = this.db.PSC2411_T_RELEASE_DETAIL
                                           .Select(x => x)
                                           .Where(x => x.RELEASE_ID == en.ReleaseId)
                                           .FirstOrDefault();

                            if (psc2411 != null)
                            {
                                //Update PSC2411_T_RELEASE_DETAIL
                                psc2411.STATUS = (byte)Constants.ReleaseDetailStatus.Approve;
                                psc2411.UPDATE_USER_ID = LoginUser.UserId;
                                psc2411.UPDATE_DATE = DateTime.Now;
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
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

        public List<ReleaseDetail> GetReleaseDetailList(DateTime pRequestDate, Constants.ReleaseDetailStatus pStatus)
        {
            List<ReleaseDetail> result = new List<ReleaseDetail>();
            DateTime dateRelease = DateTime.Now;

            try
            {
                result = (from rd in db.PSC2411_T_RELEASE_DETAIL
                          join mp in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join mm in db.PSC8027_M_MAKER on mp.MAKER equals mm.MAKER
                          join mg in db.PSC8025_M_GRADE on mp.GRADE equals mg.GRADE
                          where rd.REQUEST_DATE.Day == pRequestDate.Day &&
                          rd.REQUEST_DATE.Month == pRequestDate.Month &&
                          rd.REQUEST_DATE.Year == pRequestDate.Year &&
                          rd.STATUS == (byte)pStatus
                          select new
                          {
                              rd.RELEASE_ID,
                              rd.ITEM_CODE,
                              rd.HEAT_NO,
                              rd.REQUEST_NO,
                              rd.REQUEST_DATE,
                              rd.RECEIVE_DATE,
                              rd.REQUEST_QTY,
                              rd.ACTUAL_QTY,
                              rd.STATUS,
                              rd.Y1_REMARK,
                              rd.Y2_REMARK,
                              rd.C_REMARK,
                              mp.DESCRIPTION,
                              mm.MAKER,
                              mm.MAKER_NAME,
                              mg.GRADE,
                              mg.GRADE_NAME,
                          }).AsEnumerable().Select((x, index) => new ReleaseDetail
                          {
                              RowNo = index + 1,
                              ReleaseId = x.RELEASE_ID,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              RequestNo = x.REQUEST_NO,
                              RequestDate = x.REQUEST_DATE,
                              ReceiveDate = x.RECEIVE_DATE,
                              RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                              ActualQTY = Math.Round(Convert.ToDecimal(x.ACTUAL_QTY), 2),
                              Status = x.STATUS,
                              Yard1Remark = x.Y1_REMARK,
                              Yard2Remark = x.Y2_REMARK,
                              CuttingRemark = x.C_REMARK,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              Grade = x.GRADE,
                              Grade_Name = x.GRADE_NAME,
                              Description = x.DESCRIPTION
                          }).ToList();

                if (result != null)
                {
                    foreach (ReleaseDetail en in result)
                    {
                        RequestReleaseService objRequestReleaseService = new RequestReleaseService(this.db);
                        List<RequestRelease> objRequestReleaseList = objRequestReleaseService.GetRequestReleaseList(Convert.ToInt32(en.ReleaseId));
                        if (objRequestReleaseList != null)
                        {
                            foreach (RequestRelease enRequestRelease in objRequestReleaseList)
                            {
                                if (en.JobNo == null)
                                {
                                    en.JobNo = enRequestRelease.JobNo;
                                }
                                else
                                {
                                    en.JobNo = en.JobNo + ",\n" + enRequestRelease.JobNo;
                                }

                                if (en.MfgNo == null)
                                {
                                    en.MfgNo = enRequestRelease.MfgNo;
                                }
                                else
                                {
                                    en.MfgNo = en.MfgNo + ",\n" + enRequestRelease.MfgNo;
                                }
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

        public List<ReleaseDetail> GetHHTReleaseList(DateTime pRequestDate, Constants.HHTReleaseStatus pStatus)
        {
            List<ReleaseDetail> result = new List<ReleaseDetail>();
            DateTime dateRelease = DateTime.Now;

            try
            {
                var obj = (from re in db.PSC2410_T_HHT_RELEASE
                               //from rd in db.PSC2411_T_RELEASE_DETAIL
                           join mp in db.PSC8010_M_PIPE_ITEM on new { re.ITEM_CODE, re.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                           join mm in db.PSC8027_M_MAKER on mp.MAKER equals mm.MAKER
                           join mg in db.PSC8025_M_GRADE on mp.GRADE equals mg.GRADE
                           where re.TRAN_DATE.Day == pRequestDate.Day &&
                           re.TRAN_DATE.Month == pRequestDate.Month &&
                           re.TRAN_DATE.Year == pRequestDate.Year &&
                           re.STATUS == (byte)pStatus
                           select new
                           {
                               re.ID,
                               re.TRAN_DATE,
                               RELEASE_ID = 0,//rd.RELEASE_ID,
                               re.ITEM_CODE,
                               re.HEAT_NO,
                               REQUEST_NO = "",//rd.REQUEST_NO,
                               REQUEST_DATE = re.TRAN_DATE, //rd.REQUEST_DATE,
                               RECEIVE_DATE = re.TRAN_DATE, //rd.RECEIVE_DATE,
                               REQUEST_QTY = 0, //rd.REQUEST_QTY,
                               ACTUAL_QTY = re.ACTUAL_QTY, // rd.ACTUAL_QTY,
                                                           //STATUS = Convert.ToByte(0) , //rd.STATUS,
                               Y1_REMARK = "", //rd.Y1_REMARK,
                               Y2_REMARK = "", //rd.Y2_REMARK,
                               C_REMARK = "",
                               mp.DESCRIPTION,
                               mm.MAKER,
                               mm.MAKER_NAME,
                               mg.GRADE,
                               mg.GRADE_NAME,
                               JOB_NO = re.JOB_NO,
                           }).AsEnumerable();


                result = obj.Select((x, index) => new ReleaseDetail
                {
                    RowNo = index + 1,
                    HHTReleaseId = x.ID,
                    TransDate = x.TRAN_DATE,
                    ReleaseId = x.RELEASE_ID,
                    ItemCode = x.ITEM_CODE,
                    HeatNo = x.HEAT_NO,
                    RequestNo = x.REQUEST_NO,
                    RequestDate = x.REQUEST_DATE,
                    ReceiveDate = x.RECEIVE_DATE,
                    RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                    ActualQTY = Math.Round(Convert.ToDecimal(x.ACTUAL_QTY), 2),
                    Status = 3, //x.STATUS,
                    Yard1Remark = x.Y1_REMARK,
                    Yard2Remark = x.Y2_REMARK,
                    CuttingRemark = x.C_REMARK,
                    Maker = x.MAKER,
                    Maker_Name = x.MAKER_NAME,
                    Grade = x.GRADE,
                    Grade_Name = x.GRADE_NAME,
                    Description = x.DESCRIPTION,
                    JobNo = x.JOB_NO,
                }).ToList();

                if (result != null)
                {
                    foreach (ReleaseDetail en in result)
                    {
                        if (en.JobNo != string.Empty)
                        {
                            string[] objJobNo = en.JobNo.Split(',');
                            if (objJobNo != null)
                            {
                                foreach (string strJobNo in objJobNo)
                                {
                                    ReleaseService objReleaseService = new ReleaseService(this.db);
                                    string strMfgNo = objReleaseService.GetMfgNoReleaseData(strJobNo.Trim());
                                    if (strMfgNo != string.Empty)
                                    {
                                        if (en.MfgNo == null)
                                        {
                                            en.MfgNo = strMfgNo;
                                        }
                                        else
                                        {
                                            en.MfgNo = en.MfgNo + ",\n" + strMfgNo;
                                        }
                                    }
                                }
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

        public List<ReleaseDetail> GetQtyRemainingReleaseDetailList(Constants.ReleaseDetailStatus pStatus)
        {
            List<ReleaseDetail> result = new List<ReleaseDetail>();
            DateTime dateRelease = DateTime.Now;

            try
            {
                result = (from rd in db.PSC2411_T_RELEASE_DETAIL
                          join mp in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join mm in db.PSC8027_M_MAKER on mp.MAKER equals mm.MAKER
                          join mg in db.PSC8025_M_GRADE on mp.GRADE equals mg.GRADE
                          where rd.REQUEST_QTY > rd.ACTUAL_QTY &&
                          rd.STATUS == (byte)pStatus
                          select new
                          {
                              rd.RELEASE_ID,
                              rd.ITEM_CODE,
                              rd.HEAT_NO,
                              rd.REQUEST_NO,
                              rd.REQUEST_DATE,
                              rd.RECEIVE_DATE,
                              rd.REQUEST_QTY,
                              rd.ACTUAL_QTY,
                              rd.STATUS,
                              rd.Y1_REMARK,
                              rd.Y2_REMARK,
                              rd.C_REMARK,
                              mp.DESCRIPTION,
                              mm.MAKER,
                              mm.MAKER_NAME,
                              mg.GRADE,
                              mg.GRADE_NAME,
                          }).AsEnumerable().Select((x, index) => new ReleaseDetail
                          {
                              RowNo = index + 1,
                              ReleaseId = x.RELEASE_ID,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              RequestNo = x.REQUEST_NO,
                              RequestDate = x.REQUEST_DATE,
                              ReceiveDate = x.RECEIVE_DATE,
                              RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                              ActualQTY = Math.Round(Convert.ToDecimal(x.ACTUAL_QTY), 2),
                              Status = x.STATUS,
                              Yard1Remark = x.Y1_REMARK,
                              Yard2Remark = x.Y2_REMARK,
                              CuttingRemark = x.C_REMARK,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              Grade = x.GRADE,
                              Grade_Name = x.GRADE_NAME,
                              Description = x.DESCRIPTION
                          }).ToList();

                if (result != null)
                {
                    foreach (ReleaseDetail en in result)
                    {
                        RequestReleaseService objRequestReleaseService = new RequestReleaseService(this.db);
                        List<RequestRelease> objRequestReleaseList = objRequestReleaseService.GetRequestReleaseList(Convert.ToInt32(en.ReleaseId));
                        if (objRequestReleaseList != null)
                        {
                            foreach (RequestRelease enRequestRelease in objRequestReleaseList)
                            {
                                if (en.JobNo == null)
                                {
                                    en.JobNo = enRequestRelease.JobNo;
                                }
                                else
                                {
                                    en.JobNo = en.JobNo + ",\n" + enRequestRelease.JobNo;
                                }

                                if (en.MfgNo == null)
                                {
                                    en.MfgNo = enRequestRelease.MfgNo;
                                }
                                else
                                {
                                    en.MfgNo = en.MfgNo + ",\n" + enRequestRelease.MfgNo;
                                }
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

        public List<ReleaseDetail> GetQtyRemainingReleaseDetailList1(Constants.ReleaseDetailStatus pStatus)
        {
            List<ReleaseDetail> result = new List<ReleaseDetail>();
            DateTime dateRelease = DateTime.Now;
            try
            {
                result = (from rd in db.PSC2411_T_RELEASE_DETAIL
                          join rr in db.PSC2421_T_REQUEST_RELEASE on rd.RELEASE_ID equals rr.RELEASE_ID
                          join rq in db.PSC2420_T_REQUEST on rr.REQUEST_ID equals rq.REQUEST_ID
                          join jo in db.PSC3300_T_SYTELINE_JOB_MASTER on rq.JOB_NO equals jo.JOB_NO
                          join mp in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join mm in db.PSC8027_M_MAKER on mp.MAKER equals mm.MAKER
                          join mg in db.PSC8025_M_GRADE on mp.GRADE equals mg.GRADE
                          where rd.REQUEST_QTY >= rd.ACTUAL_QTY &&
                          rd.STATUS == (byte)pStatus
                          
                          orderby rd.REQUEST_DATE
                          select new
                          {
                              rd.RELEASE_ID,
                              rd.ITEM_CODE,
                              rd.HEAT_NO,
                              rd.REQUEST_NO,
                              rd.REQUEST_DATE,
                              rd.RECEIVE_DATE,
                              rq.REQUEST_QTY,
                              rq.RELEASE_QTY,
                              rd.STATUS,
                              rd.Y1_REMARK,
                              rd.Y2_REMARK,
                              rd.C_REMARK,
                              mp.DESCRIPTION,
                              mm.MAKER,
                              mm.MAKER_NAME,
                              mg.GRADE,
                              mg.GRADE_NAME,
                              rq.JOB_NO,
                              jo.MFGnum,
                              jo.Product
                          }).AsEnumerable().Select((x, index) => new ReleaseDetail
                          {
                              RowNo = index + 1,
                              ReleaseId = x.RELEASE_ID,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              RequestNo = x.REQUEST_NO,
                              RequestDate = x.REQUEST_DATE,
                              ReceiveDate = x.RECEIVE_DATE,
                              RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                              ActualQTY = Math.Round(Convert.ToDecimal(x.RELEASE_QTY), 2),
                              Status = x.STATUS,
                              Yard1Remark = x.Y1_REMARK,
                              Yard2Remark = x.Y2_REMARK,
                              CuttingRemark = x.C_REMARK,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              Grade = x.GRADE,
                              Grade_Name = x.GRADE_NAME,
                              Description = x.DESCRIPTION,
                              JobNo = x.JOB_NO,
                              MfgNo = x.MFGnum,
                              ProductName = x.Product,
                              ChangeReleaseQty = x.REQUEST_QTY,
                          }).ToList();

                /*if (result != null)
                {
                    foreach (ReleaseDetail en in result)
                    {
                        RequestReleaseService objRequestReleaseService = new RequestReleaseService(this.db);
                        List<RequestRelease> objRequestReleaseList = objRequestReleaseService.GetRequestReleaseList(Convert.ToInt32(en.ReleaseId));
                        if (objRequestReleaseList != null)
                        {
                            foreach (RequestRelease enRequestRelease in objRequestReleaseList)
                            {
                                if (en.JobNo == null)
                                {
                                    en.JobNo = enRequestRelease.JobNo;
                                }
                                else
                                {
                                    en.JobNo = en.JobNo + ",\n" + enRequestRelease.JobNo;
                                }

                                if (en.MfgNo == null)
                                {
                                    en.MfgNo = enRequestRelease.MfgNo;
                                }
                                else
                                {
                                    en.MfgNo = en.MfgNo + ",\n" + enRequestRelease.MfgNo;
                                }
                            }
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        /*public List<ReleaseDetail> GetQtyRemainingReleaseDetailList1(Constants.ReleaseDetailStatus pStatus)
        {
            List<ReleaseDetail> result = new List<ReleaseDetail>();
            DateTime dateRelease = DateTime.Now;

            try
            {
                result = (from rd in db.PSC2411_T_RELEASE_DETAIL
                          join mp in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                          join mm in db.PSC8027_M_MAKER on mp.MAKER equals mm.MAKER
                          join mg in db.PSC8025_M_GRADE on mp.GRADE equals mg.GRADE
                          where rd.REQUEST_QTY > rd.ACTUAL_QTY &&
                          rd.STATUS == (byte)pStatus
                          select new
                          {
                              rd.RELEASE_ID,
                              rd.ITEM_CODE,
                              rd.HEAT_NO,
                              rd.REQUEST_NO,
                              rd.REQUEST_DATE,
                              rd.RECEIVE_DATE,
                              rd.REQUEST_QTY,
                              rd.ACTUAL_QTY,
                              rd.STATUS,
                              rd.Y1_REMARK,
                              rd.Y2_REMARK,
                              rd.C_REMARK,
                              mp.DESCRIPTION,
                              mm.MAKER,
                              mm.MAKER_NAME,
                              mg.GRADE,
                              mg.GRADE_NAME,
                          }).AsEnumerable().Select((x, index) => new ReleaseDetail
                          {
                              RowNo = index + 1,
                              ReleaseId = x.RELEASE_ID,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              RequestNo = x.REQUEST_NO,
                              RequestDate = x.REQUEST_DATE,
                              ReceiveDate = x.RECEIVE_DATE,
                              RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                              ActualQTY = Math.Round(Convert.ToDecimal(x.ACTUAL_QTY), 2),
                              Status = x.STATUS,
                              Yard1Remark = x.Y1_REMARK,
                              Yard2Remark = x.Y2_REMARK,
                              CuttingRemark = x.C_REMARK,
                              Maker = x.MAKER,
                              Maker_Name = x.MAKER_NAME,
                              Grade = x.GRADE,
                              Grade_Name = x.GRADE_NAME,
                              Description = x.DESCRIPTION
                          }).ToList();

                if (result != null)
                {
                    foreach (ReleaseDetail en in result)
                    {
                        RequestReleaseService objRequestReleaseService = new RequestReleaseService(this.db);
                        List<RequestRelease> objRequestReleaseList = objRequestReleaseService.GetRequestReleaseList(Convert.ToInt32(en.ReleaseId));
                        if (objRequestReleaseList != null)
                        {
                            foreach (RequestRelease enRequestRelease in objRequestReleaseList)
                            {
                                if (en.JobNo == null)
                                {
                                    en.JobNo = enRequestRelease.JobNo;
                                }
                                else
                                {
                                    en.JobNo = en.JobNo + ",\n" + enRequestRelease.JobNo;
                                }

                                if (en.MfgNo == null)
                                {
                                    en.MfgNo = enRequestRelease.MfgNo;
                                }
                                else
                                {
                                    en.MfgNo = en.MfgNo + ",\n" + enRequestRelease.MfgNo;
                                }
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
        }*/

        public bool SaveData(List<ReleaseDetail> pReleaseDetail, List<Request> pRequest, string pUserId)
        {
            Boolean result = false;
            int intNewReleaseId = 0;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        int? intReleaseId = this.db.PSC2411_T_RELEASE_DETAIL.Max(pi => (int?)pi.RELEASE_ID);

                        foreach (ReleaseDetail en in pReleaseDetail)
                        {
                            var update = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == en.ReleaseId);

                            if (update == null)
                            {
                                intNewReleaseId = intNewReleaseId == 0 ? (Convert.ToInt32(intReleaseId == null ? 1 : intReleaseId + 1)) : intNewReleaseId + 1;
                                PSC2411_T_RELEASE_DETAIL insert = new PSC2411_T_RELEASE_DETAIL();
                                insert.RELEASE_ID = intNewReleaseId;
                                insert.ITEM_CODE = en.ItemCode;
                                insert.HEAT_NO = en.HeatNo;
                                insert.REQUEST_NO = "";
                                insert.REQUEST_DATE = DateTime.Now;
                                insert.RECEIVE_DATE = null;
                                insert.REQUEST_QTY = en.RequestQTY;
                                insert.ACTUAL_QTY = 0;
                                insert.STATUS = 1;
                                insert.REQUEST_NO = en.RequestNo;
                                insert.CREATE_USER_ID = pUserId;
                                insert.UPDATE_USER_ID = pUserId;
                                insert.CREATE_DATE = DateTime.Now;
                                insert.UPDATE_DATE = DateTime.Now;

                                this.db.PSC2411_T_RELEASE_DETAIL.Add(insert);

                                foreach (Request enReQuest in pRequest)
                                {
                                    if (en.ItemCode == enReQuest.ItemCode && en.HeatNo == enReQuest.HeatNo)
                                    {
                                        PSC2421_T_REQUEST_RELEASE insertRR = new PSC2421_T_REQUEST_RELEASE();
                                        insertRR.RELEASE_ID = intNewReleaseId;
                                        insertRR.REQUEST_ID = enReQuest.RequestId;
                                        insertRR.STATUS = 1;
                                        insertRR.CREATE_USER_ID = pUserId;
                                        insertRR.UPDATE_USER_ID = pUserId;
                                        insertRR.CREATE_DATE = DateTime.Now;
                                        insertRR.UPDATE_DATE = DateTime.Now;
                                        this.db.PSC2421_T_REQUEST_RELEASE.Add(insertRR);

                                        var updateRequest = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.REQUEST_ID == enReQuest.RequestId);

                                        if (updateRequest != null)
                                        {
                                            updateRequest.STATUS = 2;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //update.REQUEST_QTY = en.RequestQTY;
                                //update.UPDATE_USER_ID = pUserId;
                                //update.UPDATE_DATE = DateTime.Now;
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

        public int GetLastRequestNo(string pdate)
        {
            List<ReleaseDetail> result = new List<ReleaseDetail>();
            string strlastReqNo = string.Empty;
            int intlastReqNo = 0;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;


                    var obj = this.db.PSC2411_T_RELEASE_DETAIL
                        .Where(x => x.REQUEST_NO.StartsWith(pdate)).OrderByDescending(x => x.REQUEST_QTY);

                    obj = obj.OrderByDescending(x => x.REQUEST_NO);

                    result = (from x in db.PSC2411_T_RELEASE_DETAIL
                              where x.REQUEST_NO.StartsWith(pdate)
                              select new
                              {
                                  x.REQUEST_NO,

                              }).AsEnumerable().Select((x, index) => new ReleaseDetail
                              {
                                  RequestNo = x.REQUEST_NO,

                              }).ToList();


                    if (result != null && result.Count() > 0)
                    {
                        if (result[0].RequestNo != null && result[0].RequestNo != "")
                        {
                            strlastReqNo = result[0].RequestNo;
                            strlastReqNo = strlastReqNo.Replace(pdate, "");
                            intlastReqNo = int.Parse(strlastReqNo);

                            return intlastReqNo;
                        }
                        else
                        {
                            return 0;
                        }

                    }
                    else
                    {
                        return 0;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateHistoryChange(string pJobNo, decimal pReleaseId, string pItemCode, string pHeat, string pMfgNo, decimal pHistory, decimal pChangeQty, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2414_T_RELEASE_HISTORY.SingleOrDefault(x => x.JOB_NO == pJobNo);

                        if (update != null)
                        {
                            // Update existing record if pChangeQty is different from pHistory
                            if (pChangeQty != update.REQUEST_CHANGE)
                            {
                                update.REQUEST_QTY = pHistory;
                                update.RELEASE_QTY = pHistory;
                                update.REQUEST_CHANGE = pChangeQty;
                                update.RELEASE_CHANGE = pChangeQty;
                                update.UPDATE_USER_ID = userId;
                                update.UPDATE_DATE = updateDate;

                                int resultRelease = db.SaveChanges();

                                if (resultRelease >= 0)
                                {
                                    tran.Complete();
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            // Insert new record if pJobNo does not exist in the database and pHistory is different from pChangeQty
                            if (pHistory != pChangeQty)
                            {
                                var newRecord = new PSC2414_T_RELEASE_HISTORY
                                {
                                    JOB_NO = pJobNo,
                                    RELEASE_ID = pReleaseId,
                                    ITEM_CODE = pItemCode,
                                    HEAT_NO = pHeat,
                                    MFG_NO = pMfgNo,
                                    REQUEST_QTY = pHistory,
                                    RELEASE_QTY = pHistory,
                                    REQUEST_CHANGE = pChangeQty,
                                    RELEASE_CHANGE = pChangeQty,
                                    CREATE_USER_ID = userId,
                                    CREATE_DATE = updateDate,
                                    UPDATE_USER_ID = userId,
                                    UPDATE_DATE = updateDate
                                };

                                db.PSC2414_T_RELEASE_HISTORY.Add(newRecord);

                                int resultInsert = db.SaveChanges();

                                if (resultInsert >= 0)
                                {
                                    tran.Complete();
                                    return true;
                                }
                            }
                        }

                        return true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public bool UpdateChangeReleaseQty(string pjobNo, decimal pChangeQty, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                            var release = this.db.PSC2420_T_REQUEST.SingleOrDefault(x => x.JOB_NO == pjobNo);
                            if (release != null && release.REQUEST_QTY != pChangeQty)
                            {
                                release.REQUEST_QTY = pChangeQty;
                                release.RELEASE_QTY = pChangeQty;
                                release.UPDATE_USER_ID = userId;
                                release.UPDATE_DATE = updateDate;
                            }

                        int resultRelease = db.SaveChanges();

                        if (resultRelease >= 0)
                        {
                            tran.Complete();
                            return true;
                        }
                    }

                    tran.Dispose();
                    return false;
                }

                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public bool UpdateChangeReleaseQty1(string pjobNo, decimal pChangeQty, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var release = this.db.PSC2410_T_RELEASE.SingleOrDefault(x => x.JOB_NO == pjobNo);
                        if (release != null && release.QTY != pChangeQty)
                        {
                            release.QTY = pChangeQty;
                            release.STATUS = 1;
                            release.UPDATE_USER_ID = userId;
                            release.UPDATE_DATE = updateDate;
                        }

                        int resultRelease = db.SaveChanges();

                        if (resultRelease >= 0)
                        {
                            tran.Complete();
                            return true;
                        }
                    }

                    tran.Dispose();
                    return false;
                }

                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public List<ReleaseYardDetail> GetReleaseStatus()
        {
            List<ReleaseYardDetail> result = null;
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                result = (from sa in db.PSC2411_T_RELEASE_DETAIL
                          where sa.RELEASE_ID == sa.RELEASE_ID
                          select new ReleaseYardDetail
                          {
                              ReleaseId = sa.RELEASE_ID,
                              Status = sa.STATUS,
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public bool UpdateChangeReleaseStatus(decimal pReleaseId, byte pStatus, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        var psc2411Record = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == pReleaseId);
                        if (psc2411Record != null)
                        {
                            if (pStatus == 0 || pStatus == 1)
                            {
                                psc2411Record.STATUS = pStatus;

                                if (pStatus == 0)
                                {
                                    psc2411Record.UPDATE_USER_ID = userId;
                                    psc2411Record.UPDATE_DATE = DateTime.Now;
                                }

                                db.Entry(psc2411Record).State = EntityState.Modified;

                                int result = db.SaveChanges();
                                if (result > 0)
                                {
                                    tran.Complete();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateActualRelease(decimal pReleaseID, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var totalQty = (from rd in db.PSC2420_T_REQUEST
                                        join rr in db.PSC2421_T_REQUEST_RELEASE on rd.REQUEST_ID equals rr.REQUEST_ID
                                        join rq in db.PSC2411_T_RELEASE_DETAIL on rr.RELEASE_ID equals rq.RELEASE_ID
                                        where rq.RELEASE_ID == pReleaseID
                                        group rd by rq.RELEASE_ID into g
                                        select g.Sum(rd => rd.RELEASE_QTY)).FirstOrDefault();

                        var release = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == pReleaseID);
                        if (release != null && release.REQUEST_QTY != totalQty)
                        {
                            release.REQUEST_QTY = totalQty;
                            release.STATUS = 1;
                            release.UPDATE_USER_ID = userId;
                            release.UPDATE_DATE = updateDate;
                        }

                        int resultRelease = db.SaveChanges();

                        if (resultRelease >= 0)
                        {
                            tran.Complete();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool UpdateYard1Remark(decimal pReleaseID, string pRemark, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == pReleaseID);

                        if (update != null)
                        {
                            update.Y1_REMARK = pRemark;
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

        public bool UpdateYard2Remark(decimal pReleaseID, string pRemark, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == pReleaseID);

                        if (update != null)
                        {
                            update.Y2_REMARK = pRemark;
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

        public bool UpdateCuttingRemark(decimal pReleaseID, string pRemark, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == pReleaseID);

                        if (update != null)
                        {
                            update.C_REMARK = pRemark;
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

        public bool UpdateReleaseDetailData(decimal pReleaseID, decimal? pActualQTY, Boolean pUpdateReleaseFlg, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2411_T_RELEASE_DETAIL.SingleOrDefault(x => x.RELEASE_ID == pReleaseID);

                        if (update != null)
                        {
                            update.ACTUAL_QTY = pActualQTY;
                            if (pUpdateReleaseFlg == true)
                            {
                                update.STATUS = (int)Common.Constants.ReleaseDetailStatus.Release;
                            }

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

        public List<ReleaseYardDetail> GetReleaseFilter(string pJobs)
        {
            List<ReleaseYardDetail> result = new List<ReleaseYardDetail> ();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from rd in db.PSC2411_T_RELEASE_DETAIL
                               join rr in db.PSC2421_T_REQUEST_RELEASE on rd.RELEASE_ID equals rr.RELEASE_ID
                               join rq in db.PSC2420_T_REQUEST on rr.REQUEST_ID equals rq.REQUEST_ID
                               join jo in db.PSC3300_T_SYTELINE_JOB_MASTER on rq.JOB_NO equals jo.JOB_NO
                               join mp in db.PSC8010_M_PIPE_ITEM on new { rd.ITEM_CODE, rd.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
                               join mm in db.PSC8027_M_MAKER on mp.MAKER equals mm.MAKER
                               join mg in db.PSC8025_M_GRADE on mp.GRADE equals mg.GRADE
                               where jo.JOB_NO == pJobs && jo.JOB_NO.Contains(pJobs)


                               orderby rd.REQUEST_DATE
                               select new
                               {
                                   rd.RELEASE_ID,
                                   rd.ITEM_CODE,
                                   rd.HEAT_NO,
                                   rd.REQUEST_NO,
                                   rd.REQUEST_DATE,
                                   rd.RECEIVE_DATE,
                                   rq.REQUEST_QTY,
                                   rq.RELEASE_QTY,
                                   rd.STATUS,
                                   rd.Y1_REMARK,
                                   rd.Y2_REMARK,
                                   rd.C_REMARK,
                                   mp.DESCRIPTION,
                                   mm.MAKER,
                                   mm.MAKER_NAME,
                                   mg.GRADE,
                                   mg.GRADE_NAME,
                                   rq.JOB_NO,
                                   jo.MFGnum,
                                   jo.Product
                               }).AsQueryable();
                    
                    result = obj.AsEnumerable().Select((x, index) => new ReleaseYardDetail
                    {
                            RowNo = index + 1,
                            ReleaseId = x.RELEASE_ID,
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            RequestNo = x.REQUEST_NO,
                            RequestDate = x.REQUEST_DATE,
                            ReceiveDate = x.RECEIVE_DATE,
                            RequestQTY = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                            ActualQTY = Math.Round(Convert.ToDecimal(x.RELEASE_QTY), 2),
                            Status = x.STATUS,
                            Yard1Remark = x.Y1_REMARK,
                            Yard2Remark = x.Y2_REMARK,
                            CuttingRemark = x.C_REMARK,
                            Maker = x.MAKER,
                            Maker_Name = x.MAKER_NAME,
                            Grade = x.GRADE,
                            Grade_Name = x.GRADE_NAME,
                            Description = x.DESCRIPTION,
                            JobNo = x.JOB_NO,
                            MfgNo = x.MFGnum,
                            ProductName = x.Product,
                            ChangeReleaseQty = Math.Round(Convert.ToDecimal(x.REQUEST_QTY), 2),
                            }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
        }
        
    }
}
        


        #endregion
    
