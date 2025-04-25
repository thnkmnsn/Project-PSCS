using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;
using PSCS.ModelERPDEV01;
using System.Transactions;
using System.Security.Permissions;
using ZXing;
using PSCS.Common;
using System.IO;

namespace PSCS.Services
{
    public class ReleaseService
    {
        private PSCSEntities db;
        private string userId;

        public ReleaseService(PSCSEntities pDb)
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

        public Release GetReleaseByJobNo(string pJobNo)
        {
            Release result = new Release();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    decimal requested = 0;

                    var obj = (from r in this.db.PSC2410_T_RELEASE
                               join mp in db.PSC8010_M_PIPE_ITEM on r.ITEM_CODE equals mp.ITEM_CODE
                               where r.JOB_NO == pJobNo
                               select new {
                                   //.SingleOrDefault(x => x.JOB_NO == pJobNo)
                                   JobNo = r.JOB_NO,
                                   ReleaseDate = r.RELEASE_DATE,
                                   MfgNo = r.MFG_NO,
                                   ItemCode = r.ITEM_CODE,
                                   HeatNo = r.HEAT_NO,
                                   Description = mp.DESCRIPTION,
                                   Maker_Name = mp.MAKER_NAME,
                                   QTY = r.QTY,
                                   ReleaseQTY = r.RELEASE_QTY,
                                   RequestQTY = r.REQUEST_QTY,
                                   Status = r.STATUS
                               }).ToList();
                    var RQ = (from rq in this.db.PSC2420_T_REQUEST
                              where rq.JOB_NO == pJobNo
                              group rq by rq.JOB_NO into x
                              select new
                              {
                                  SumRequest = x.Sum(z => z.REQUEST_QTY)
                              }).ToList();
                    if (RQ.Count >= 1)
                    {
                        requested = Convert.ToDecimal(RQ[0].SumRequest);
                        decimal REMAINING = obj[0].QTY - requested;
                        result = new Release()
                        {
                            JobNo = obj[0].JobNo,
                            ReleaseDate = obj[0].ReleaseDate,
                            MfgNo = obj[0].MfgNo,
                            ItemCode = obj[0].ItemCode,
                            HeatNo = obj[0].HeatNo,
                            Description = obj[0].Description,
                            Maker_Name = obj[0].Maker_Name,
                            QTY = REMAINING,
                            ReleaseQTY = obj[0].ReleaseQTY,
                            RequestQTY = obj[0].RequestQTY,
                            Status = (int)obj[0].Status
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        
        public Release GetReleaseData(string pJobNo)
        {
            Release result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var objRelease1 = (from r in db.PSC2410_T_RELEASE
                                       join pi in this.db.PSC8010_M_PIPE_ITEM on
                                         new { r.ITEM_CODE, r.HEAT_NO } equals
                                         new { pi.ITEM_CODE, pi.HEAT_NO }
                                       join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                                       where r.JOB_NO == pJobNo
                                       select new
                                       {
                                           JobNo = r.JOB_NO,
                                           ReleaseDate = r.RELEASE_DATE,
                                           MfgNo = r.MFG_NO,
                                           ItemCode = r.ITEM_CODE,
                                           HeatNo = r.HEAT_NO,
                                           Des = pi.DESCRIPTION,
                                           Maker = pi.MAKER,
                                           MakerName = mm.MAKER_NAME,
                                           Qty = r.QTY,
                                           ReleaseQTY = r.RELEASE_QTY,
                                           RequestQTY = r.REQUEST_QTY,
                                           Status = r.STATUS,
                                           CreateUserID = r.CREATE_USER_ID,
                                           UpdateUserID = r.UPDATE_USER_ID,
                                           CreateDate = r.CREATE_DATE,
                                           UpdateDate = r.UPDATE_DATE,
                                       }).ToList();

                    if (objRelease1.Count != 0)
                    {
                        result = new Release()
                        {
                            JobNo = objRelease1[0].JobNo,
                            ReleaseDate = objRelease1[0].ReleaseDate,
                            MfgNo = objRelease1[0].MfgNo,
                            ItemCode = objRelease1[0].ItemCode,
                            HeatNo = objRelease1[0].HeatNo,
                            Description = objRelease1[0].Des,
                            Maker = objRelease1[0].Maker,
                            Maker_Name = objRelease1[0].MakerName,
                            QTY = objRelease1[0].Qty,
                            ReleaseQTY = objRelease1[0].ReleaseQTY,
                            RequestQTY = objRelease1[0].RequestQTY,
                            RemainQTY = 0,
                            Status = (int)objRelease1[0].Status,
                            CreateUserID = objRelease1[0].CreateUserID,
                            UpdateUserID = objRelease1[0].UpdateUserID,
                            CreateDate = objRelease1[0].CreateDate,
                            UpdateDate = objRelease1[0].UpdateDate,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.db.Dispose();
            }

            return result;
        }

        public Release GetReleaseData1(string pJobNo)
        {
            Release result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var objRelease1 = (from r in db.PSC3300_T_SYTELINE_JOB_MASTER
                                       join pi in this.db.PSC8010_M_PIPE_ITEM on
                                         new { r.ITEM_CODE, r.HEAT_NO } equals
                                         new { pi.ITEM_CODE, pi.HEAT_NO }
                                       join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                                       join q in db.PSC2420_T_REQUEST on r.JOB_NO equals q.JOB_NO
                                       join i in db.PSC2421_T_REQUEST_RELEASE on q.REQUEST_ID equals i.REQUEST_ID
                                       where r.JOB_NO == pJobNo
                                       select new
                                       {
                                           JobNo = r.JOB_NO,
                                           MfgNo = r.MFG_NO,
                                           ItemCode = r.ITEM_CODE,
                                           HeatNo = r.HEAT_NO,
                                           Des = pi.DESCRIPTION,
                                           Maker = pi.MAKER,
                                           MakerName = mm.MAKER_NAME,
                                           Qty = r.QTY,
                                           CreateUserID = r.CREATE_USER_ID,
                                           UpdateUserID = r.UPDATE_USER_ID,
                                           CreateDate = r.CREATE_DATE,
                                           UpdateDate = r.UPDATE_DATE,
                                           ReleaseId = i.RELEASE_ID,
                                       }).ToList();

                    if (objRelease1.Count != 0)
                    {
                        result = new Release()
                        {
                            JobNo = objRelease1[0].JobNo,
                            MfgNo = objRelease1[0].MfgNo,
                            ItemCode = objRelease1[0].ItemCode,
                            HeatNo = objRelease1[0].HeatNo,
                            Description = objRelease1[0].Des,
                            Maker = objRelease1[0].Maker,
                            Maker_Name = objRelease1[0].MakerName,
                            QTY = (decimal)objRelease1[0].Qty,
                            RemainQTY = 0,
                            CreateUserID = objRelease1[0].CreateUserID,
                            UpdateUserID = objRelease1[0].UpdateUserID,
                            CreateDate = objRelease1[0].CreateDate,
                            UpdateDate = objRelease1[0].UpdateDate,
                            ReleaseID = objRelease1[0].ReleaseId,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.db.Dispose();
            }

            return result;
        }

        public string GetMfgNoReleaseData(string pJobNo)
        {
            string result = string.Empty;

            try
            {
                result = (from r in db.PSC2410_T_RELEASE
                                   join pi in this.db.PSC8010_M_PIPE_ITEM on
                                     new { r.ITEM_CODE, r.HEAT_NO } equals
                                     new { pi.ITEM_CODE, pi.HEAT_NO }
                                   join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                                   where r.JOB_NO == pJobNo
                                   select new
                                   {
                                       MfgNo = r.MFG_NO,
                                   }).FirstOrDefault().MfgNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        private Boolean UpdateJobMasterData(List<JobMasterData> pJobMasterDataList)
        {
            Boolean result = false;

            try
            {
                JobMasterService objJobMasterService = new JobMasterService(this.db, this.userId);
                result = objJobMasterService.JobMaster(pJobMasterDataList);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<Release> GetReleaseListByMoveId(decimal pMoveId, Common.Constants.HHTReleaseStatus pStatus) // Edit 24-04-25 by Art version 23
        {
            List<Release> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                //var releases = (from mo in db.PSC2410_T_HHT_RELEASE
                //                where mo.MOVE_ID == pMoveId && mo.STATUS == (int)pStatus
                //                orderby mo.ID
                //                select mo).ToList();

                result = (from mo in db.PSC2410_T_HHT_RELEASE
                          join jb in db.Subt_HHT_Jobs on mo.JOB_NO equals jb.JOB_NO_ORIGINAL
                          join mj in db.PSC3300_T_SYTELINE_JOB_MASTER on jb.JOB_NO_CLEAN equals mj.JOB_NO
                          join us in db.PSC8030_M_USER on mo.CREATE_USER_ID.Substring(0, mo.CREATE_USER_ID.Length - 2) equals us.USER_ID
                          where mo.MOVE_ID == pMoveId &&
                                mo.STATUS == (int)pStatus 
                          orderby mo.ID 
                          select new
                          {
                              mo.ID,
                              mo.TRAN_DATE,
                              mo.MOVE_ID,
                              mo.JOB_NO,
                              mo.ITEM_CODE,
                              mo.HEAT_NO,
                              mo.BARCODE,
                              mo.LOCATION_CODE,
                              mo.ACTUAL_QTY,
                              mo.STATUS,
                              mo.CREATE_DATE,
                              mo.CREATE_USER_ID,
                              mo.UPDATE_DATE,
                              mo.UPDATE_USER_ID,
                              mj.MFGnum,
                              us.USER_NAME,
                              jb.Product,
                          }).AsEnumerable().GroupBy(x => new { x.ID, x.TRAN_DATE, x.JOB_NO, x.ITEM_CODE, x.HEAT_NO, x.BARCODE, x.LOCATION_CODE, x.ACTUAL_QTY, x.STATUS, x.CREATE_DATE, x.CREATE_USER_ID, x.UPDATE_DATE, x.UPDATE_USER_ID, x.USER_NAME })
                          .Select(group => new Release
                          {
                               ID = group.Key.ID,
                               TRAN_DATE = group.Key.TRAN_DATE,
                               // MoveId ไม่ได้เป็นส่วนหนึ่งของการกลุ่ม (grouping)
                               MoveId = Convert.ToDecimal(group.Select(x => x.MOVE_ID).FirstOrDefault()),
                               JobNo = group.Key.JOB_NO,
                               ItemCode = group.Key.ITEM_CODE,
                               HeatNo = group.Key.HEAT_NO,
                               Barcode = group.Key.BARCODE,
                               LocationCode = "1A00", //x.LOCATION_CODE,
                               QTY = Convert.ToDecimal(group.Key.ACTUAL_QTY),
                               Status = Convert.ToByte(group.Key.STATUS),
                               CreateDate = group.Key.CREATE_DATE,
                               CreateUserID = group.Key.CREATE_USER_ID,
                               UpdateDate = group.Key.UPDATE_DATE,
                               UpdateUserID = group.Key.UPDATE_USER_ID,
                               MfgNo = string.Join(" , ", group.Select(g => g.MFGnum).Distinct()), // รวม MFG_NO ทั้งหมดในกลุ่มเดียวกัน
                               Operator = group.Key.USER_NAME,
                              //ProductName = "1\n2"
                               Product = string.Join("| ", group.Select(g => g.Product).Distinct()
                                 .Select((prod, index) => $"{index + 1}. {prod}"))
                          }).Take(100).ToList();
                          }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<Release> GetReleaseListByNullMoveIdandSubmitStatus(DateTime pMovementDate)
        {
            List<Release> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from mo in db.PSC2410_T_HHT_RELEASE
                          where mo.TRAN_DATE.Day == pMovementDate.Day &&
                                mo.TRAN_DATE.Month == pMovementDate.Month &&
                                mo.TRAN_DATE.Year == pMovementDate.Year &&
                                mo.MOVE_ID == null && 
                                mo.STATUS == (int)Common.Constants.HHTReleaseStatus.Submit
                          orderby mo.ID
                          select new
                          {
                              mo.ID,
                              mo.MOVE_ID,
                              mo.ITEM_CODE,
                              mo.HEAT_NO,
                              mo.BARCODE,
                              mo.LOCATION_CODE,
                              mo.ACTUAL_QTY,
                              mo.STATUS,
                              mo.CREATE_DATE,
                              mo.CREATE_USER_ID,
                              mo.UPDATE_DATE,
                              mo.UPDATE_USER_ID
                          }).AsEnumerable().Select((x, index) => new Release
                          {
                              ID = x.ID,
                              MoveId = Convert.ToDecimal(x.MOVE_ID),
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              Barcode = x.BARCODE,
                              LocationCode = "1A00",//x.LOCATION_CODE,
                              QTY = Convert.ToDecimal(x.ACTUAL_QTY),
                              Status = Convert.ToByte(x.STATUS),
                              CreateDate =  x.CREATE_DATE,
                              CreateUserID = x.CREATE_USER_ID,
                              UpdateDate = x.UPDATE_DATE,
                              UpdateUserID = x.UPDATE_USER_ID
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool Update2410Release(string pJobNo, decimal pReleaseQTY, Boolean updateStatusRelease, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2410_T_RELEASE.SingleOrDefault(x => x.JOB_NO == pJobNo);

                        if (update != null)
                        {
                            update.RELEASE_QTY = pReleaseQTY;
                            if (updateStatusRelease == true)
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

        public bool UpdateStatus(decimal pId, Common.Constants.HHTReleaseStatus pStatus, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2410_T_HHT_RELEASE.SingleOrDefault(x => x.ID == pId);

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

        public bool UpdateDeleteStatus(decimal pId, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC2410_T_HHT_RELEASE.SingleOrDefault(x => x.ID == pId &&
                                                                                   x.STATUS == (int)Common.Constants.HHTReleaseStatus.Submit);

                        if (update != null)
                        {
                            update.STATUS = (int)Common.Constants.HHTReleaseStatus.Delete;

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

        public List<ReleaseYardDetail> GetJobsFilter(string pJobs)
        {
            List<ReleaseYardDetail> result = new List<ReleaseYardDetail>();
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
                               where rq.JOB_NO == pJobs 
                               //&& rd.STATUS == (byte)Constants.ReleaseDetailStatus.Request


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
                                   jo.Product,
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



        #endregion
    }
}