using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;


namespace PSCS.Services
{
    public class MoveLocationService
    {
        private PSCSEntities db;

        public MoveLocationService(PSCSEntities pDb)
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


        public List<MoveLocation> GetMoveLocationList(DateTime pMovementDate, string pYardId)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            List<MoveLocation> resultBySelectedYard = new List<MoveLocation>();
            Boolean IsFound = false;
            MoveOutService objMoveOutSubmitTransService = null;
            MoveOutService objMoveOutApproveTransService = null;
            MoveINService objMoveINSubmitTransService = null;
            MoveINService objMoveINApproveTransService = null;
            ReleaseService objReleaseSubmitTransService = null;
            ReleaseService objReleaseApproveTransService = null;
            List<MoveOut> moveOutList = null;
            List<MoveIN> moveINList = null;
            List<Release> ReleaseList = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                 
                    if (pYardId == null || pYardId == string.Empty)
                    {
                        var obj = (from re in db.PSC2210_T_MOVE_LOCATION   
                                   join pi in db.PSC8010_M_PIPE_ITEM on new { re.ITEM_CODE, re.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                                   where re.MOVE_DATE.Day == pMovementDate.Day &&
                                         re.MOVE_DATE.Month == pMovementDate.Month &&
                                         re.MOVE_DATE.Year == pMovementDate.Year &&
                        re.STATUS == (int)Common.Constants.MoveLocationStatus.Submit 
                                   select new
                                   {
                                       re.MOVE_ID,
                                       re.MOVE_DATE,
                                       re.ITEM_CODE,
                                       re.HEAT_NO,
                                       LOCATION_CODE_FROM_NAME = "",
                                       re.QTY,
                                       LOCATION_CODE_TO_NAME = "",
                                       re.STATUS,
                                       re.REMARK,
                                       pi.DESCRIPTION,
                                       re.LOCATION_CODE_FROM,
                                       re.LOCATION_CODE_TO,
                                       re.IS_RELEASE
                                   }).AsQueryable();

                        result = obj.AsEnumerable()
                          .Select((x, index) => new MoveLocation
                          {
                              RowNo = index + 1,
                              MoveId = x.MOVE_ID,
                              MoveDate = x.MOVE_DATE,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              LocationCodeFrom = x.LOCATION_CODE_FROM,
                              QTY = x.QTY,
                              LocationCodeTo = x.LOCATION_CODE_TO,
                              Status = x.STATUS,
                              Remark = x.REMARK,
                              Description = x.DESCRIPTION,
                              LocationCodeFromName = x.LOCATION_CODE_FROM_NAME,
                              LocationCodeToName = x.LOCATION_CODE_TO_NAME,
                              Is_Release = x.IS_RELEASE

                          }).ToList();
                    }
                    else
                    {
                        var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                                   join pi in db.PSC8010_M_PIPE_ITEM on new { re.ITEM_CODE, re.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                                   where re.MOVE_DATE.Day == pMovementDate.Day &&
                                         re.MOVE_DATE.Month == pMovementDate.Month &&
                                         re.MOVE_DATE.Year == pMovementDate.Year && 
                                         re.STATUS == (int)Common.Constants.MoveLocationStatus.Submit
                                   select new
                                   {
                                       re.MOVE_ID,
                                       re.MOVE_DATE,
                                       re.ITEM_CODE,
                                       re.HEAT_NO,
                                       LOCATION_CODE_FROM_NAME = "",
                                       re.QTY,
                                       LOCATION_CODE_TO_NAME = "",
                                       re.STATUS,
                                       re.REMARK,
                                       pi.DESCRIPTION,
                                       re.LOCATION_CODE_FROM,
                                       re.LOCATION_CODE_TO,
                                       re.IS_RELEASE
                                   }).AsQueryable();

                        result = obj.AsEnumerable()
                          .Select((x, index) => new MoveLocation
                          {
                              RowNo = index + 1,
                              MoveId = x.MOVE_ID,
                              MoveDate = x.MOVE_DATE,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              LocationCodeFrom = x.LOCATION_CODE_FROM,
                              QTY = x.QTY,
                              LocationCodeTo = x.LOCATION_CODE_TO,
                              Status = x.STATUS,
                              Remark = x.REMARK,
                              Description = x.DESCRIPTION,
                              LocationCodeFromName = x.LOCATION_CODE_FROM_NAME,
                              LocationCodeToName = x.LOCATION_CODE_TO_NAME,
                              Is_Release = x.IS_RELEASE

                          }).ToList();

                        if (result != null)
                        {
                            resultBySelectedYard = new List<MoveLocation>();

                            List<string> objLocationCodeOnSelectdYardList = (from lo in db.PSC8020_M_LOCATION
                                                                             where lo.YARD == pYardId
                                                                             select lo.LOCATION_CODE).ToList();

                            foreach (MoveLocation en in result)
                            {
                                IsFound = false;

                                objMoveOutSubmitTransService = new MoveOutService(db);
                                moveOutList = objMoveOutSubmitTransService.GetMoveOutListByMoveId(en.MoveId, Common.Constants.HHTMoveOutStatus.SubmitTrans);
                                if (moveOutList != null)
                                {
                                    var objList = moveOutList.Where(m => objLocationCodeOnSelectdYardList.Contains(m.LOCATION_CODE));
                                    if (objList != null && objList.Count() > 0)
                                    {
                                        IsFound = true;
                                    }
                                }
                                if (!IsFound)
                                {
                                    objMoveOutApproveTransService = new MoveOutService(db);
                                    moveOutList = objMoveOutApproveTransService.GetMoveOutListByMoveId(en.MoveId, Common.Constants.HHTMoveOutStatus.ApproveTrans);
                                    if (moveOutList != null)
                                    {
                                        var objList = moveOutList.Where(m => objLocationCodeOnSelectdYardList.Contains(m.LOCATION_CODE));
                                        if (objList != null && objList.Count() > 0)
                                        {
                                            IsFound = true;
                                        }
                                    }
                                }
                                if (!IsFound)
                                {
                                    objMoveINSubmitTransService = new MoveINService(db);
                                    moveINList = objMoveINSubmitTransService.GetMoveINByMoveId(en.MoveId, Common.Constants.HHTMoveInStatus.SubmitTrans);
                                    if (moveINList != null)
                                    {
                                        var objList = moveINList.Where(m => objLocationCodeOnSelectdYardList.Contains(m.LOCATION_CODE));
                                        if (objList != null && objList.Count() > 0)
                                        {
                                            IsFound = true;
                                        }
                                    }
                                }
                                if (!IsFound)
                                {
                                    objMoveINApproveTransService = new MoveINService(db);
                                    moveINList = objMoveINApproveTransService.GetMoveINByMoveId(en.MoveId, Common.Constants.HHTMoveInStatus.ApproveTrans);
                                    if (moveINList != null)
                                    {
                                        var objList = moveINList.Where(m => objLocationCodeOnSelectdYardList.Contains(m.LOCATION_CODE));
                                        if (objList != null && objList.Count() > 0)
                                        {
                                            IsFound = true;
                                        }
                                    }
                                }
                                if (!IsFound)
                                {
                                    objReleaseSubmitTransService = new ReleaseService(db);
                                    ReleaseList = objReleaseSubmitTransService.GetReleaseListByMoveId(en.MoveId, Common.Constants.HHTReleaseStatus.Submit);
                                    if (ReleaseList != null)
                                    {
                                        var objList = ReleaseList.Where(m => objLocationCodeOnSelectdYardList.Contains(m.LocationCode));
                                        if (objList != null && objList.Count() > 0)
                                        {
                                            IsFound = true;
                                        }
                                    }
                                }
                                if (!IsFound)
                                {
                                    objReleaseApproveTransService = new ReleaseService(db);
                                    ReleaseList = objReleaseApproveTransService.GetReleaseListByMoveId(en.MoveId, Common.Constants.HHTReleaseStatus.Approve);
                                    if (ReleaseList != null)
                                    {
                                        var objList = ReleaseList.Where(m => objLocationCodeOnSelectdYardList.Contains(m.LocationCode));
                                        if (objList != null && objList.Count() > 0)
                                        {
                                            IsFound = true;
                                        }
                                    }
                                }

                                if (IsFound)
                                {
                                    resultBySelectedYard.Add(en);
                                }
                            }

                            result = resultBySelectedYard;
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

        public bool CheckAllRemark(List<MoveLocation> pMoveLocation)
        {
            Boolean result = false;
            foreach (MoveLocation en in pMoveLocation)
            {
                if (en.Remark != null && en.Remark != "")
                {
                    //OK
                    result = true;
                }
                else
                {
                    //Alert
                    return false;
                }
            }

                return result;

        }

        public bool ApproveMoveLocation(List<MoveLocation> pMoveLocation, string userId)
        {
            Boolean result = false;


            
            int flag = 0;
            try
            {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        foreach (MoveLocation en in pMoveLocation)
                        {

                            var update = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == en.MoveId);
                            if (update != null)
                            {
                                update.STATUS = (int)Common.Constants.MoveLocationStatus.Approve;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = userId;
                            }
                        }

                        flag = this.db.SaveChanges();
                        if (flag >= 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return result;
        }


        public bool DeleteMoveLocation(decimal pMoveId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    int result = 0;                    

                    var objMove = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == pMoveId);
                     
                        if (objMove != null)
                        {
                           
                            db.PSC2210_T_MOVE_LOCATION.Remove(objMove);
                            result = db.SaveChanges();
                        }
                        else
                        {
                            result = 0;
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
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }

        // Update Move Location (Save Remark)
        public bool UpdateMoveLocationData(List<MoveLocation> pMoveLocation, string userId)
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
                        DateTime updateDate = DateTime.Now;

                        foreach (MoveLocation en in pMoveLocation)
                        {
                            var update = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == en.MoveId);
                            if (update != null)
                            {                              
                                update.REMARK = en.Remark;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = userId;
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

        public string GetMoveLocationDataByMoveId(decimal pMoveId)
        {
            string result = string.Empty;
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var queryResult =
                        (from re in this.db.PSC2210_T_MOVE_LOCATION
                         join loT in db.PSC8020_M_LOCATION on re.LOCATION_CODE_TO equals loT.LOCATION_CODE into lj
                         from loT in lj.DefaultIfEmpty()
                         where re.MOVE_ID == pMoveId 
                         select new { re.QTY,
                                      re.LOCATION_CODE_TO,
                                      LOCATION_CODE_TO_NAME = loT.NAME
                         }).SingleOrDefault();
                                    
                    string strDestination = "";
                    string strDestinationName = "";

                    if (queryResult != null)
                    {
                        if (queryResult.LOCATION_CODE_TO != null)
                        {
                            strDestination = queryResult.LOCATION_CODE_TO.ToString();
                        }
                        if (queryResult.LOCATION_CODE_TO_NAME != null)
                        {
                            //strDestinationName = queryResult.LOCATION_CODE_TO_NAME.ToString();
                            strDestinationName = GetLocationYardDisplay(strDestination);
                        }

                        result = string.Format("{0:#,##0}", queryResult.QTY) + '*' + strDestination + '*' + strDestinationName;
                    }                           

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        
        private string GetLocationYardDisplay(string pLocationCode)
        {
            string result = string.Empty;

            try
            {
                var objLocationYardList = (from lo in db.PSC8020_M_LOCATION
                                           join ya in db.PSC8022_M_YARD on lo.YARD equals ya.YARD
                                           select new
                                           {
                                               lo.LOCATION_CODE,
                                               lo.LOCATION,
                                               LOCATION_NAME = lo.NAME,
                                               ya.YARD,
                                               YARD_NAME = ya.NAME
                                           }).AsQueryable().ToList();

                if (objLocationYardList != null)
                {
                    var objLocationYard = objLocationYardList.Where(x => x.LOCATION_CODE == pLocationCode).FirstOrDefault();
                    if (objLocationYard != null)
                    {
                        result = objLocationYard.YARD_NAME + "-" + objLocationYard.LOCATION_NAME;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string GetReceivePlanListNotMoveId(DateTime? pMoveDate, string pYardId, List<decimal> MoveIdList)
        {
            string strRemark = string.Empty;
            string strObj = string.Empty;
            string strLocationCodeTo = string.Empty;
            string strLocationCodeToName = string.Empty;
            List<MoveLocation> result = new List<MoveLocation>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    if (MoveIdList.Count == 0)
                    {
                        MoveIdList.Add(0);
                    }
                    if (!string.IsNullOrEmpty(pYardId))
                    {
                        var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                                   join pi in db.PSC8010_M_PIPE_ITEM on new { re.ITEM_CODE, re.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                                   join lo in db.PSC8020_M_LOCATION on re.LOCATION_CODE_FROM equals lo.LOCATION_CODE                                  
                                   join loT in db.PSC8020_M_LOCATION on re.LOCATION_CODE_TO equals loT.LOCATION_CODE into lj
                                   from loT in lj.DefaultIfEmpty()
                                   where lo.YARD == pYardId
                                   where re.STATUS == (int)Common.Constants.MoveLocationStatus.Submit && !MoveIdList.Contains(re.MOVE_ID)
                                   orderby re.MOVE_ID ascending
                                   select new
                                   {
                                       re.MOVE_ID,
                                       re.MOVE_DATE,
                                       re.ITEM_CODE,
                                       re.HEAT_NO,
                                       LOCATION_CODE_FROM_NAME = lo.NAME,
                                       re.QTY,
                                       LOCATION_CODE_TO_NAME = loT.NAME,
                                       re.STATUS,
                                       re.REMARK,
                                       pi.DESCRIPTION,
                                       re.LOCATION_CODE_FROM,
                                       re.LOCATION_CODE_TO,
                                       re.IS_RELEASE
                                   }).AsQueryable();

                        // Move Date
                        if (pMoveDate != null)
                        {
                            DateTime deliveryDate = Convert.ToDateTime(pMoveDate).Date;
                            obj = obj.Where(
                                  //x => x.MOVE_DATE == deliveryDate
                                  x => x.MOVE_DATE.Year == deliveryDate.Year
                                      && x.MOVE_DATE.Month == deliveryDate.Month
                                      && x.MOVE_DATE.Day == deliveryDate.Day
                                );
                        }



                        result = obj.AsEnumerable()
                          .Select((x, index) => new MoveLocation
                          {
                              RowNo = index + 1,
                              MoveId = x.MOVE_ID,
                              MoveDate = x.MOVE_DATE,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              LocationCodeFrom = x.LOCATION_CODE_FROM,
                              QTY = x.QTY,
                              LocationCodeTo = x.LOCATION_CODE_TO,
                              Status = x.STATUS,
                              Remark = x.REMARK,
                              Description = x.DESCRIPTION,
                              LocationCodeFromName = x.LOCATION_CODE_FROM_NAME,
                              LocationCodeToName = x.LOCATION_CODE_TO_NAME,
                              Is_Release = x.IS_RELEASE

                          }).ToList();
                    }
                    else
                    {
                        var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                                   join pi in db.PSC8010_M_PIPE_ITEM on new { re.ITEM_CODE, re.HEAT_NO } equals new { pi.ITEM_CODE, pi.HEAT_NO }
                                   //join lo in db.PSC8020_M_LOCATION on re.LOCATION_CODE_FROM equals lo.LOCATION_CODE
                                   join lo in db.PSC8020_M_LOCATION on re.LOCATION_CODE_FROM equals lo.LOCATION_CODE
                                   join loT in db.PSC8020_M_LOCATION on re.LOCATION_CODE_TO equals loT.LOCATION_CODE into lj
                                   from loT in lj.DefaultIfEmpty()
                                   where re.STATUS == (int)Common.Constants.MoveLocationStatus.Submit 
                                   && !MoveIdList.Contains(re.MOVE_ID)
                                   orderby re.MOVE_ID ascending
                                   select new
                                   {
                                       re.MOVE_ID,
                                       re.MOVE_DATE,
                                       re.ITEM_CODE,
                                       re.HEAT_NO,
                                       LOCATION_CODE_FROM_NAME = lo.NAME,
                                       re.QTY,
                                       LOCATION_CODE_TO_NAME = loT.NAME,
                                       re.STATUS,
                                       re.REMARK,
                                       pi.DESCRIPTION,
                                       re.LOCATION_CODE_FROM,
                                       re.LOCATION_CODE_TO,
                                       re.IS_RELEASE
                                   }).AsQueryable();

                        // Move Date
                        if (pMoveDate != null)
                        {
                            DateTime deliveryDate = Convert.ToDateTime(pMoveDate).Date;
                            obj = obj.Where(
                                //x => x.MOVE_DATE == deliveryDate
                                  x => x.MOVE_DATE.Year == deliveryDate.Year
                                      && x.MOVE_DATE.Month == deliveryDate.Month
                                      && x.MOVE_DATE.Day == deliveryDate.Day
                                );
                        }



                        result = obj.AsEnumerable()
                          .Select((x, index) => new MoveLocation
                          {
                              RowNo = index + 1,
                              MoveId = x.MOVE_ID,
                              MoveDate = x.MOVE_DATE,
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              LocationCodeFrom = x.LOCATION_CODE_FROM,
                              QTY = x.QTY,
                              LocationCodeTo = x.LOCATION_CODE_TO,
                              Status = x.STATUS,
                              Remark = x.REMARK,
                              Description = x.DESCRIPTION,
                              LocationCodeFromName = x.LOCATION_CODE_FROM_NAME,
                              LocationCodeToName = x.LOCATION_CODE_TO_NAME,
                              Is_Release = x.IS_RELEASE

                          }).ToList();
                    }

                    //Convert to string 

                   if (result != null && result.Count > 0)
                    {
                      
                        //for (int i = 0; i < result.Count; i++)
                        //{
                            strObj = "";
                            strRemark = "";
                            strLocationCodeTo = "??";
                            strLocationCodeToName = "??";


                            if (result[0].Remark != null)
                            {
                                strRemark = result[0].Remark.ToString();
                            }

                        if (result[0].LocationCodeTo != null)
                        {
                            strLocationCodeTo = result[0].LocationCodeTo.ToString();
                        }
                        if (result[0].LocationCodeToName != null)
                        {
                            strLocationCodeToName = result[0].LocationCodeToName.ToString();
                        }


                        strObj = result[0].MoveId.ToString() + "**" 
                               + result[0].MoveDate.ToString() + "**"
                               + result[0].ItemCode.ToString() + "**" 
                               + result[0].HeatNo.ToString() + "**"
                               + result[0].LocationCodeFromName.ToString() + "**" 
                               + string.Format("{0:#,##0}", result[0].QTY).ToString() + "**" 
                               + strLocationCodeToName + "**" 
                               + result[0].Status.ToString() + "**" 
                               + strRemark + "**" 
                               + result[0].Description.ToString() + "**" 
                               + result[0].LocationCodeFrom.ToString() + "**" 
                               + strLocationCodeTo;
                                                       
                        //}

                    }                   

                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return strObj;
        }


        //public List<MoveLocation> GetMoveLocationListByItemHeadLofrom(DateTime pMoveDate, string pItemCode, string pHeadNo, string pBarcode, string pLocationFrom)
        //{
        //    List<MoveLocation> result = new List<MoveLocation>();
        //    try
        //    {
        //        using (this.db)
        //        {
        //            db.Configuration.LazyLoadingEnabled = false;
                                      
        //            var obj = (from re in db.PSC2210_T_MOVE_LOCATION   
        //                        where re.MOVE_DATE.Day == pMoveDate.Day
        //                        && re.MOVE_DATE.Month == pMoveDate.Month
        //                        && re.MOVE_DATE.Year == pMoveDate.Year
        //                        && re.ITEM_CODE == pItemCode 
        //                        && re.HEAT_NO == pHeadNo
        //                        && re.BARCODE == pBarcode
        //                        && re.LOCATION_CODE_FROM == pLocationFrom
        //                        select new
        //                        {
        //                            re.MOVE_ID,
        //                            re.MOVE_DATE,
        //                            re.ITEM_CODE,
        //                            re.HEAT_NO,
        //                            re.BARCODE,
        //                            re.LOCATION_CODE_FROM,
        //                            re.QTY,
        //                            re.LOCATION_CODE_TO,
        //                            re.STATUS,
        //                            re.REMARK,
        //                            re.IS_RELEASE
        //                        }).AsQueryable();
                    
        //            result = obj.AsEnumerable()
        //                .Select((x, index) => new MoveLocation
        //                {
        //                    RowNo = index + 1,
        //                    MoveId = x.MOVE_ID,
        //                    MoveDate = x.MOVE_DATE,
        //                    ItemCode = x.ITEM_CODE,
        //                    HeatNo = x.HEAT_NO,
        //                    Barcode = x.BARCODE,
        //                    LocationCodeFrom = x.LOCATION_CODE_FROM,
        //                    QTY = x.QTY,
        //                    LocationCodeTo = x.LOCATION_CODE_TO,
        //                    Status = x.STATUS,
        //                    Remark = x.REMARK,
        //                    Is_Release = x.IS_RELEASE
        //                }).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        public List<MoveLocation> GetMoveLocationListByItemHeadLocationfrom(DateTime pMoveDate, string pItemCode, string pHeadNo , string pLocationFrom)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                               where re.MOVE_DATE.Day == pMoveDate.Day
                               && re.MOVE_DATE.Month == pMoveDate.Month
                               && re.MOVE_DATE.Year == pMoveDate.Year
                               && re.ITEM_CODE == pItemCode
                               && re.HEAT_NO == pHeadNo
                               && re.LOCATION_CODE_FROM == pLocationFrom
                               select new
                               {
                                   re.MOVE_ID,
                                   re.MOVE_DATE,
                                   re.ITEM_CODE,
                                   re.HEAT_NO,
                                   re.BARCODE,
                                   re.LOCATION_CODE_FROM,
                                   re.QTY,
                                   re.LOCATION_CODE_TO,
                                   re.STATUS,
                                   re.REMARK,
                                   re.IS_RELEASE
                               }).AsQueryable();

                    result = obj.AsEnumerable()
                        .Select((x, index) => new MoveLocation
                        {
                            RowNo = index + 1,
                            MoveId = x.MOVE_ID,
                            MoveDate = x.MOVE_DATE,
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            Barcode = x.BARCODE,
                            LocationCodeFrom = x.LOCATION_CODE_FROM,
                            QTY = x.QTY,
                            LocationCodeTo = x.LOCATION_CODE_TO,
                            Status = x.STATUS,
                            Remark = x.REMARK,
                            Is_Release = x.IS_RELEASE
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<MoveLocation> GetMoveLocationListByItemCodeAndHeatNo(DateTime pMoveDate, string pItemCode, string pHeadNo)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                               where re.MOVE_DATE.Day == pMoveDate.Day
                               && re.MOVE_DATE.Month == pMoveDate.Month
                               && re.MOVE_DATE.Year == pMoveDate.Year
                               && re.ITEM_CODE == pItemCode
                               && re.HEAT_NO == pHeadNo
                               select new
                               {
                                   re.MOVE_ID,
                                   re.MOVE_DATE,
                                   re.ITEM_CODE,
                                   re.HEAT_NO,
                                   re.BARCODE,
                                   re.LOCATION_CODE_FROM,
                                   re.QTY,
                                   re.LOCATION_CODE_TO,
                                   re.STATUS,
                                   re.REMARK,
                                   re.IS_RELEASE
                               }).AsQueryable();

                    result = obj.AsEnumerable()
                        .Select((x, index) => new MoveLocation
                        {
                            RowNo = index + 1,
                            MoveId = x.MOVE_ID,
                            MoveDate = x.MOVE_DATE,
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            Barcode = x.BARCODE,
                            LocationCodeFrom = x.LOCATION_CODE_FROM,
                            QTY = x.QTY,
                            LocationCodeTo = x.LOCATION_CODE_TO,
                            Status = x.STATUS,
                            Remark = x.REMARK,
                            Is_Release = x.IS_RELEASE
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //need modify to support put multiple location
        public List<MoveLocation> GetMoveLocationListByItemHeadLocationTo(DateTime pdateMove, string pItemCode, string pHeadNo)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                               where re.MOVE_DATE.Day == pdateMove.Day
                               && re.MOVE_DATE.Month == pdateMove.Month
                               && re.MOVE_DATE.Year == pdateMove.Year
                               && re.ITEM_CODE == pItemCode
                               && re.HEAT_NO == pHeadNo
                               && re.LOCATION_CODE_FROM != null
                               select new
                               {
                                   re.MOVE_ID,
                                   re.MOVE_DATE,
                                   re.ITEM_CODE,
                                   re.HEAT_NO,
                                   re.BARCODE,
                                   re.LOCATION_CODE_FROM,
                                   re.QTY,
                                   re.LOCATION_CODE_TO,
                                   re.STATUS,
                                   re.REMARK,
                                   re.IS_RELEASE
                               }).AsQueryable();

                    if(obj != null)
                    {
                        result = obj.AsEnumerable()
                        .Select((x, index) => new MoveLocation
                        {
                            RowNo = index + 1,
                            MoveId = x.MOVE_ID,
                            MoveDate = x.MOVE_DATE,
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            Barcode = x.BARCODE,
                            LocationCodeFrom = x.LOCATION_CODE_FROM,
                            QTY = x.QTY,
                            LocationCodeTo = x.LOCATION_CODE_TO,
                            Status = x.STATUS,
                            Remark = x.REMARK,
                            Is_Release = x.IS_RELEASE

                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<MoveLocation> GetMoveLocationByMoveId(decimal pMoveId)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from re in db.PSC2210_T_MOVE_LOCATION
                               where re.MOVE_ID == pMoveId
                               select new
                               {
                                   re.MOVE_ID,
                                   re.MOVE_DATE,
                                   re.ITEM_CODE,
                                   re.HEAT_NO,
                                   re.LOCATION_CODE_FROM,
                                   re.QTY,
                                   re.LOCATION_CODE_TO,
                                   re.STATUS,
                                   re.REMARK,
                                   re.IS_RELEASE
                               }).AsQueryable();


                    result = obj.AsEnumerable()
                      .Select((x, index) => new MoveLocation
                      {
                          RowNo = index + 1,
                          MoveId = x.MOVE_ID,
                          MoveDate = x.MOVE_DATE,
                          ItemCode = x.ITEM_CODE,
                          HeatNo = x.HEAT_NO,
                          LocationCodeFrom = x.LOCATION_CODE_FROM,
                          QTY = x.QTY,
                          LocationCodeTo = x.LOCATION_CODE_TO,
                          Status = x.STATUS,
                          Remark = x.REMARK,
                          Is_Release = x.IS_RELEASE

                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool UpdateQtyFromMoveIN(decimal pMoveId, MoveIN objUpdateMoveIN, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            DateTime updateDate = DateTime.Now;
          
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        //var updateMoveLo = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == pMoveId);
                        //if (updateMoveLo != null)
                        //{
                        //    if(updateMoveLo.QTY == null)
                        //    {
                        //        updateMoveLo.QTY = objUpdateMoveIN.ACTUAL_QTY.HasValue ? objUpdateMoveIN.ACTUAL_QTY.Value : 0;
                        //    }
                        //    else
                        //    {
                        //        updateMoveLo.QTY = updateMoveLo.QTY + (objUpdateMoveIN.ACTUAL_QTY.HasValue ? objUpdateMoveIN.ACTUAL_QTY.Value : 0);
                        //    }
                        //    updateMoveLo.LOCATION_CODE_TO = objUpdateMoveIN.LOCATION_CODE;
                        //    updateMoveLo.IS_RELEASE = (int)Common.Constants.MoveLocationIn.IN;
                        //    updateMoveLo.UPDATE_DATE = updateDate;
                        //    updateMoveLo.UPDATE_USER_ID = LoginUser.UserId;
                        //}
                        
                        //flag = this.db.SaveChanges();

                        var updateMoveIN = this.db.PSC2210_T_HHT_MOVE_IN.SingleOrDefault(x => x.ID == objUpdateMoveIN.ID);
                        if (updateMoveIN != null)
                        {
                            updateMoveIN.MOVE_ID = pMoveId;
                            updateMoveIN.UPDATE_DATE = updateDate;
                            updateMoveIN.UPDATE_USER_ID = LoginUser.UserId;
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
        
        public bool UpdateQtyFromRelease(decimal pMoveId, Release pRelease, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            DateTime updateDate = DateTime.Now;
            
            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        //var updateMoveLo = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == pMoveId);
                        //if (updateMoveLo != null)
                        //{
                        //    if (updateMoveLo.QTY == null)
                        //    {
                        //        updateMoveLo.QTY = Convert.ToDecimal(pRelease.QTY);
                        //    }
                        //    else
                        //    {
                        //        updateMoveLo.QTY = updateMoveLo.QTY + Convert.ToDecimal(pRelease.QTY);
                        //    }
                        //    updateMoveLo.LOCATION_CODE_TO = pRelease.LocationCode;
                        //    updateMoveLo.IS_RELEASE = (int)Common.Constants.MoveLocationIn.Release;
                        //    updateMoveLo.UPDATE_DATE = updateDate;
                        //    updateMoveLo.UPDATE_USER_ID = LoginUser.UserId;
                        //}

                        //flag = this.db.SaveChanges();
                        //if (flag >= 1)
                        //{

                        var updateMoveIN = this.db.PSC2410_T_HHT_RELEASE.SingleOrDefault(x => x.ID == pRelease.ID);
                        if (updateMoveIN != null)
                        {
                            updateMoveIN.MOVE_ID = pMoveId;
                            updateMoveIN.UPDATE_DATE = updateDate;
                            updateMoveIN.UPDATE_USER_ID = LoginUser.UserId;
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

        public bool UpdateQtyFromMoveOut(DateTime pMoveDareTime, MoveOut objInsertMoveOut, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;           
            DateTime updateDate = DateTime.Now;
            //decimal sumQTY = 0;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objMoveLocation = (from mo in this.db.PSC2210_T_MOVE_LOCATION
                                               where mo.MOVE_DATE.Day == pMoveDareTime.Day &&
                                               mo.MOVE_DATE.Month == pMoveDareTime.Month &&
                                               mo.MOVE_DATE.Year == pMoveDareTime.Year &&
                                               mo.ITEM_CODE == objInsertMoveOut.ITEM_CODE &&
                                               mo.HEAT_NO == objInsertMoveOut.HEAT_NO &&
                                               mo.LOCATION_CODE_FROM == objInsertMoveOut.LOCATION_CODE
                                               select new 
                                                {
                                                    mo.MOVE_ID,
                                                    mo.MOVE_DATE,
                                                    mo.ITEM_CODE,
                                                    mo.HEAT_NO,
                                                    mo.BARCODE,
                                                    mo.LOCATION_CODE_FROM,
                                                    mo.QTY,
                                                    mo.LOCATION_CODE_TO,
                                                    mo.IS_RELEASE,
                                                    mo.STATUS,
                                                    mo.CREATE_DATE,
                                                    mo.CREATE_USER_ID,
                                                    mo.UPDATE_DATE,
                                                    mo.UPDATE_USER_ID
                                                }).ToList();

                        if (objMoveLocation != null && objMoveLocation.Count > 0)
                        {
                            decimal decMoveID = objMoveLocation[0].MOVE_ID;
                            var updateMoveLo = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == decMoveID);
                            if (updateMoveLo != null)
                            {
                                updateMoveLo.QTY_FROM = updateMoveLo.QTY_FROM + (objInsertMoveOut.ACTUAL_QTY == null ? 0 : Convert.ToDecimal(objInsertMoveOut.ACTUAL_QTY));
                                updateMoveLo.QTY = updateMoveLo.QTY_FROM;
                                updateMoveLo.UPDATE_DATE = updateDate;
                                updateMoveLo.UPDATE_USER_ID = LoginUser.UserId;
                            }

                            flag = this.db.SaveChanges();
                            if (flag >= 1)
                            {
                                var updateMoveOut = this.db.PSC2210_T_HHT_MOVE_OUT.SingleOrDefault(x => x.ID == objInsertMoveOut.ID);
                                if (updateMoveOut != null)
                                {
                                    updateMoveOut.MOVE_ID = updateMoveLo.MOVE_ID;
                                    updateMoveOut.UPDATE_DATE = updateDate;
                                    updateMoveOut.UPDATE_USER_ID = LoginUser.UserId;
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
                            else
                            {
                                result = false;
                                tran.Dispose();
                            }
                            
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

        public bool UpdateQtyFromMoveOutItemcodeAndHeatNo(DateTime pMoveDareTime, MoveOut objInsertMoveOut, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            DateTime updateDate = DateTime.Now;
            //decimal sumQTY = 0;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objMoveLocation = (from mo in this.db.PSC2210_T_MOVE_LOCATION
                                               where mo.MOVE_DATE.Day == pMoveDareTime.Day &&
                                               mo.MOVE_DATE.Month == pMoveDareTime.Month &&
                                               mo.MOVE_DATE.Year == pMoveDareTime.Year &&
                                               mo.ITEM_CODE == objInsertMoveOut.ITEM_CODE &&
                                               mo.HEAT_NO == objInsertMoveOut.HEAT_NO 
                                               select new
                                               {
                                                   mo.MOVE_ID,
                                                   mo.MOVE_DATE,
                                                   mo.ITEM_CODE,
                                                   mo.HEAT_NO,
                                                   mo.BARCODE,
                                                   mo.LOCATION_CODE_FROM,
                                                   mo.QTY,
                                                   mo.LOCATION_CODE_TO,
                                                   mo.IS_RELEASE,
                                                   mo.STATUS,
                                                   mo.CREATE_DATE,
                                                   mo.CREATE_USER_ID,
                                                   mo.UPDATE_DATE,
                                                   mo.UPDATE_USER_ID
                                               }).ToList();

                        if (objMoveLocation != null && objMoveLocation.Count > 0)
                        {
                            decimal decMoveID = objMoveLocation[0].MOVE_ID;
                            var updateMoveLo = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(x => x.MOVE_ID == decMoveID);
                            if (updateMoveLo != null)
                            {
                                updateMoveLo.QTY_FROM = updateMoveLo.QTY_FROM + (objInsertMoveOut.ACTUAL_QTY == null ? 0 : Convert.ToDecimal(objInsertMoveOut.ACTUAL_QTY));
                                updateMoveLo.UPDATE_DATE = updateDate;
                                updateMoveLo.UPDATE_USER_ID = LoginUser.UserId;
                            }

                            flag = this.db.SaveChanges();
                            if (flag >= 1)
                            {
                                var updateMoveOut = this.db.PSC2210_T_HHT_MOVE_OUT.SingleOrDefault(x => x.ID == objInsertMoveOut.ID);
                                if (updateMoveOut != null)
                                {
                                    updateMoveOut.MOVE_ID = updateMoveLo.MOVE_ID;
                                    updateMoveOut.UPDATE_DATE = updateDate;
                                    updateMoveOut.UPDATE_USER_ID = LoginUser.UserId;
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
                            else
                            {
                                result = false;
                                tran.Dispose();
                            }

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

        public bool InsertFromMoveOut(DateTime pMoveDate, MoveOut objInsertMoveOut, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            int newRowNo = 0;
            DateTime updateDate = DateTime.Now;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objMoveLocation = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(mo => mo.MOVE_DATE.Day == pMoveDate.Day
                                                                                              && mo.MOVE_DATE.Month == pMoveDate.Month
                                                                                              && mo.MOVE_DATE.Year == pMoveDate.Year
                                                                                              && mo.ITEM_CODE == objInsertMoveOut.ITEM_CODE
                                                                                              && mo.HEAT_NO == objInsertMoveOut.HEAT_NO
                                                                                              && mo.BARCODE == objInsertMoveOut.BARCODE
                                                                                              && mo.LOCATION_CODE_FROM == objInsertMoveOut.LOCATION_CODE
                                                                                              && mo.STATUS == (int)Common.Constants.MoveLocationStatus.Submit);
                        if (objMoveLocation == null)
                        {
                            //1.INSERT INTO  PSC2210_T_MOVE_LOCATION  
                            PSC2210_T_MOVE_LOCATION objNew = new PSC2210_T_MOVE_LOCATION();

                            int? intRowNo = this.db.PSC2210_T_MOVE_LOCATION.Max(x => (int?)x.MOVE_ID);
                            newRowNo = newRowNo == 0 ? (Convert.ToInt32(intRowNo == null ? 1 : intRowNo + 1)) : newRowNo + 1;

                            objNew.MOVE_ID = newRowNo;
                            objNew.MOVE_DATE = pMoveDate;
                            objNew.ITEM_CODE = objInsertMoveOut.ITEM_CODE;
                            objNew.HEAT_NO = objInsertMoveOut.HEAT_NO;
                            objNew.BARCODE = objInsertMoveOut.BARCODE;
                            objNew.LOCATION_CODE_FROM = objInsertMoveOut.LOCATION_CODE;
                            objNew.QTY_FROM = objInsertMoveOut.ACTUAL_QTY.HasValue ? objInsertMoveOut.ACTUAL_QTY.Value : 0;
                            objNew.QTY = objNew.QTY_FROM;
                            objNew.LOCATION_CODE_TO = null;
                            objNew.IS_RELEASE = (int)Common.Constants.MoveLocationIn.IN;
                            objNew.STATUS = objInsertMoveOut.STATUS;
                            objNew.REMARK = null;
                            objNew.CREATE_USER_ID = LoginUser.UserId;
                            objNew.CREATE_DATE = DateTime.Now;
                            objNew.UPDATE_USER_ID = LoginUser.UserId;
                            objNew.UPDATE_DATE = DateTime.Now;

                            this.db.PSC2210_T_MOVE_LOCATION.Add(objNew);
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            var update = this.db.PSC2210_T_HHT_MOVE_OUT.SingleOrDefault(x => x.ID == objInsertMoveOut.ID);
                            if (update != null)
                            {
                                update.MOVE_ID = newRowNo;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = LoginUser.UserId;
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

        public bool InsertOutByItemcodeAndHeatNo(DateTime pMoveDate, MoveOut objInsertMoveOut, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            int newRowNo = 0;
            DateTime updateDate = DateTime.Now;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objMoveLocation = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(mo => mo.MOVE_DATE.Day == pMoveDate.Day 
                                                                                              && mo.MOVE_DATE.Month == pMoveDate.Month
                                                                                              && mo.MOVE_DATE.Year == pMoveDate.Year
                                                                                              && mo.ITEM_CODE == objInsertMoveOut.ITEM_CODE
                                                                                              && mo.HEAT_NO == objInsertMoveOut.HEAT_NO
                                                                                              && mo.STATUS == (int)Common.Constants.MoveLocationStatus.Submit);
                        if (objMoveLocation == null)
                        {
                            //1.INSERT INTO  PSC2210_T_MOVE_LOCATION  
                            PSC2210_T_MOVE_LOCATION objNew = new PSC2210_T_MOVE_LOCATION();

                            int? intRowNo = this.db.PSC2210_T_MOVE_LOCATION.Max(x => (int?)x.MOVE_ID);
                            newRowNo = newRowNo == 0 ? (Convert.ToInt32(intRowNo == null ? 1 : intRowNo + 1)) : newRowNo + 1;

                            objNew.MOVE_ID = newRowNo;
                            objNew.MOVE_DATE = pMoveDate;
                            objNew.ITEM_CODE = objInsertMoveOut.ITEM_CODE;
                            objNew.HEAT_NO = objInsertMoveOut.HEAT_NO;
                            objNew.BARCODE = "";
                            objNew.LOCATION_CODE_FROM = "";
                            objNew.QTY_FROM = 0;
                            objNew.QTY = null;
                            objNew.LOCATION_CODE_TO = null;
                            objNew.IS_RELEASE = (int)Common.Constants.MoveLocationIn.IN;
                            objNew.STATUS = (int)Common.Constants.MoveLocationStatus.Submit;
                            objNew.REMARK = null;
                            objNew.CREATE_USER_ID = LoginUser.UserId;
                            objNew.CREATE_DATE = DateTime.Now;
                            objNew.UPDATE_USER_ID = LoginUser.UserId;
                            objNew.UPDATE_DATE = DateTime.Now;

                            this.db.PSC2210_T_MOVE_LOCATION.Add(objNew);
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            var update = this.db.PSC2210_T_HHT_MOVE_OUT.SingleOrDefault(x => x.ID == objInsertMoveOut.ID);
                            if (update != null)
                            {
                                update.MOVE_ID = newRowNo;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = LoginUser.UserId;
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

        public bool InsertInByItemcodeAndHeatNo(DateTime pMoveDate, MoveIN objInsertMoveIn, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            int newRowNo = 0;
            DateTime updateDate = DateTime.Now;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objMoveLocation = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(mo => mo.MOVE_DATE.Day == pMoveDate.Day
                                                                                              && mo.MOVE_DATE.Month == pMoveDate.Month
                                                                                              && mo.MOVE_DATE.Year == pMoveDate.Year
                                                                                              && mo.ITEM_CODE == objInsertMoveIn.ITEM_CODE
                                                                                              && mo.HEAT_NO == objInsertMoveIn.HEAT_NO
                                                                                              && mo.STATUS == (int)Common.Constants.MoveLocationStatus.Submit);
                        if (objMoveLocation == null)
                        {
                            //1.INSERT INTO  PSC2210_T_MOVE_LOCATION  
                            PSC2210_T_MOVE_LOCATION objNew = new PSC2210_T_MOVE_LOCATION();

                            int? intRowNo = this.db.PSC2210_T_MOVE_LOCATION.Max(x => (int?)x.MOVE_ID);
                            newRowNo = newRowNo == 0 ? (Convert.ToInt32(intRowNo == null ? 1 : intRowNo + 1)) : newRowNo + 1;

                            objNew.MOVE_ID = newRowNo;
                            objNew.MOVE_DATE = pMoveDate;
                            objNew.ITEM_CODE = objInsertMoveIn.ITEM_CODE;
                            objNew.HEAT_NO = objInsertMoveIn.HEAT_NO;
                            objNew.BARCODE = "";
                            objNew.LOCATION_CODE_FROM = "";
                            objNew.QTY_FROM = 0;
                            objNew.QTY = null;
                            objNew.LOCATION_CODE_TO = null;
                            objNew.IS_RELEASE = (int)Common.Constants.MoveLocationIn.IN;
                            objNew.STATUS = (int)Common.Constants.MoveLocationStatus.Submit;
                            objNew.REMARK = null;
                            objNew.CREATE_USER_ID = LoginUser.UserId;
                            objNew.CREATE_DATE = DateTime.Now;
                            objNew.UPDATE_USER_ID = LoginUser.UserId;
                            objNew.UPDATE_DATE = DateTime.Now;

                            this.db.PSC2210_T_MOVE_LOCATION.Add(objNew);
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            var update = this.db.PSC2210_T_HHT_MOVE_IN.SingleOrDefault(x => x.ID == objInsertMoveIn.ID);
                            if (update != null)
                            {
                                update.MOVE_ID = newRowNo;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = LoginUser.UserId;
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

        public bool InsertReleaseByItemcodeAndHeatNo(DateTime pMoveDate, Release objInsertRelease, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;
            int newRowNo = 0;
            DateTime updateDate = DateTime.Now;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        var objMoveLocation = this.db.PSC2210_T_MOVE_LOCATION.SingleOrDefault(mo => mo.MOVE_DATE.Day == pMoveDate.Day
                                                                                              && mo.MOVE_DATE.Month == pMoveDate.Month
                                                                                              && mo.MOVE_DATE.Year == pMoveDate.Year
                                                                                              && mo.ITEM_CODE == objInsertRelease.ItemCode
                                                                                              && mo.HEAT_NO == objInsertRelease.HeatNo
                                                                                              && mo.STATUS == (int)Common.Constants.MoveLocationStatus.Submit);
                        if (objMoveLocation == null)
                        {
                            //1.INSERT INTO  PSC2210_T_MOVE_LOCATION  
                            PSC2210_T_MOVE_LOCATION objNew = new PSC2210_T_MOVE_LOCATION();

                            int? intRowNo = this.db.PSC2210_T_MOVE_LOCATION.Max(x => (int?)x.MOVE_ID);
                            newRowNo = newRowNo == 0 ? (Convert.ToInt32(intRowNo == null ? 1 : intRowNo + 1)) : newRowNo + 1;

                            objNew.MOVE_ID = newRowNo;
                            objNew.MOVE_DATE = pMoveDate;
                            objNew.ITEM_CODE = objInsertRelease.ItemCode;
                            objNew.HEAT_NO = objInsertRelease.HeatNo;
                            objNew.BARCODE = "";
                            objNew.LOCATION_CODE_FROM = "";
                            objNew.QTY_FROM = 0;
                            objNew.QTY = null;
                            objNew.LOCATION_CODE_TO = null;
                            objNew.IS_RELEASE = (int)Common.Constants.MoveLocationIn.Release;
                            objNew.STATUS = (int)Common.Constants.MoveLocationStatus.Submit;
                            objNew.REMARK = null;
                            objNew.CREATE_USER_ID = LoginUser.UserId;
                            objNew.CREATE_DATE = DateTime.Now;
                            objNew.UPDATE_USER_ID = LoginUser.UserId;
                            objNew.UPDATE_DATE = DateTime.Now;

                            this.db.PSC2210_T_MOVE_LOCATION.Add(objNew);
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            var update = this.db.PSC2410_T_HHT_RELEASE.SingleOrDefault(x => x.ID == objInsertRelease.ID);
                            if (update != null)
                            {
                                update.MOVE_ID = newRowNo;
                                update.UPDATE_DATE = updateDate;
                                update.UPDATE_USER_ID = LoginUser.UserId;
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

        public List<MoveLocation> GetReceiveDetailList(string pMoveId)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            try
            {
                //List<MoveLocation> obj = MoveLocationScreenEdit.GetMoveLocationDataEdit();

                //int _id = Int32.Parse(pMoveId);
                //obj = obj.Where(x => x.MoveId == _id).ToList();
                //result = obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<Release> Get2410ReleaseByMoveID(decimal moveId)
        {
            List<Release> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

              

                result = (from mo in db.PSC2410_T_HHT_RELEASE
                          where mo.MOVE_ID == moveId
                          orderby mo.ID
                          select new
                          {
                              mo.ID,
                              mo.MOVE_ID,
                              mo.ITEM_CODE,
                              mo.HEAT_NO,
                              mo.LOCATION_CODE,
                              mo.ACTUAL_QTY,
                              mo.STATUS,
                              mo.JOB_NO
                          }).AsEnumerable().Select((x, index) => new Release
                          {
                              ID = x.ID,
                              MoveId = Convert.ToDecimal(x.MOVE_ID),
                              ItemCode = x.ITEM_CODE,
                              HeatNo = x.HEAT_NO,
                              LocationCode = x.LOCATION_CODE,
                              QTY = Convert.ToDecimal(x.ACTUAL_QTY),
                              Status = Convert.ToByte(x.STATUS),
                              JobNo = x.JOB_NO
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Request> Get2420RequestByJobNo(string pJobNo)
        {
            List<Request> result = new List<Request>();

            try
            {
                result = (from rd in db.PSC2420_T_REQUEST                         
                          where rd.JOB_NO == pJobNo
                          select new
                          {
                              rd.REQUEST_ID,
                              rd.JOB_NO,
                              rd.REQUEST_DATE,                              
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


        public List<RequestRelease> Get2421RequestReleaseByRequestID(decimal pRequestID)
        {
            List<RequestRelease> result = new List<RequestRelease>();

            try
            {
                result = (from rr in db.PSC2421_T_REQUEST_RELEASE
                          join rq in db.PSC2420_T_REQUEST on rr.REQUEST_ID equals rq.REQUEST_ID
                          join re in db.PSC2410_T_RELEASE on rq.JOB_NO equals re.JOB_NO
                          where rr.REQUEST_ID == pRequestID
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




    }
}