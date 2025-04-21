using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.ModelsScreen;
using System.Net;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class MoveLocationController : BaseController
    {
        public MoveLocationScreen model
        {
            get
            {
                if (Session["MoveLocationScreen"] == null)
                {
                    Session["MoveLocationScreen"] = new MoveLocationScreen();
                    return (MoveLocationScreen)Session["MoveLocationScreen"];
                }
                else
                {
                    return (MoveLocationScreen)Session["MoveLocationScreen"];
                }
            }

            set { Session["MoveLocationScreen"] = value; }
        }

        public MoveLocationScreenEdit modelEdit
        {
            get
            {
                if (Session["MoveLocationScreenEdit"] == null)
                {
                    Session["MoveLocationScreenEdit"] = new MoveLocationScreenEdit();
                    return (MoveLocationScreenEdit)Session["MoveLocationScreenEdit"];
                }
                else
                {
                    return (MoveLocationScreenEdit)Session["MoveLocationScreenEdit"];
                }
            }

            set { Session["MoveLocationScreenEdit"] = value; }
        }


        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2210(MoveLocationScreen FilterModel)
        {
            try
            {
                // Initial model
                this.IntialPSC2210();
                if (modelEdit.HasError)
                {
                    modelEdit.HasError = false;                   
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;
                }

                if (this.LoginUser.RoleId == Constants.ROLE_YARDSUPERVISOR)
                {
                    this.model.IsYardSupervisorRole = true;
                    this.model.IsControllerRole = false;
                }
                else if(this.LoginUser.RoleId == Constants.ROLE_CONTROLLER)
                {
                    this.model.IsYardSupervisorRole = false;
                    this.model.IsControllerRole = true;
                }
                else 
                {
                    this.model.IsYardSupervisorRole = false;
                    this.model.IsControllerRole = false;
                }

                ViewBag.IsFilter = true;
                FilterModel.FilterInternalMoveDate = model.FilterInternalMoveDate;
                return Filter_OnClick(FilterModel);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Common.Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);         
            }

            this.model.Total = this.model.MoveLocationList.Count.ToString();

            return View(this.model);
        }     


        [HttpPost]
        public ActionResult PSC2210(MoveLocationScreen FilterModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                //this.model.AlertsType = Constants.AlertsType.None;
                //this.model.Message = string.Empty;
                Boolean result = false;
                string userId = this.LoginUser.UserId;
                string message = string.Empty;

                MoveLocationService objMoveLocation = new MoveLocationService(this.dbEntities);
                var pDataList = this.model.MoveLocationList;

                ViewBag.IsFilter = false;
                ClearGrid();
                switch (submitButton)
                {
                    case "Back":
                        this.model.AlertsType = Constants.AlertsType.None;
                        this.model.Message = string.Empty;
                        return RedirectToAction("PSC0100", "Menu");

                    case "Filter":
                        ViewBag.IsFilter = true;
                        this.model.AlertsType = Constants.AlertsType.None;
                        this.model.Message = "";
                        return Filter_OnClick(FilterModel);

                    case "LoadData":                        
                        result = LoadData();
                        break;

                    case "Save":
                        ViewBag.IsFilter = true;
                        //this.model.AlertsType = Constants.AlertsType.None;
                        //this.model.Message = "";
                        return Filter_OnClick(FilterModel);

                    case "Delete":
                        ViewBag.IsFilter = true;
                        //this.model.AlertsType = Constants.AlertsType.None;
                        //this.model.Message = "";
                        return Filter_OnClick(FilterModel);

                    case "Approve":
                        ViewBag.IsFilter = true;
                        //this.model.AlertsType = Constants.AlertsType.None;
                        //this.model.Message = "";
                        return Filter_OnClick(FilterModel);
                }

                return View(this.model);
                //return Filter_OnClick(FilterModel);

            }
            catch (Exception ex)
            {
                this.model.AlertsType = Common.Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.model);
            }
        }

        public class UpdateModel
        {                   
            public string Remark { get; set; }
            public string Destination { get; set; }
        }

        public bool LoadData()
        {
            bool result = true;
            DateTime dateMovement = DateTime.Now;

            try
            {
                dateMovement = this.model.FilterInternalMoveDate == null ? DateTime.Now : Convert.ToDateTime(this.model.FilterInternalMoveDate);

                //1. PSC2210_T_HHT_MOVE_OUT  =>   PSC2210_T_MOVE_LOCATION
                result = InsertOutToMoveLocation(dateMovement);

                //2. PSC2210_T_HHT_MOVE_IN  =>   PSC2210_T_MOVE_LOCATION
                result = UpdateINToMoveLocation(dateMovement);

                //3. PSC2410_T_HHT_MOVE_IN  =>   PSC2210_T_MOVE_LOCATION
                result = UpdateReleaseToMoveLocation(dateMovement);

                return result;
            }
            catch (Exception ex)
            {
                //this.model.AlertsType = Common.Constants.AlertsType.Danger;
                //this.model.Message = ex.Message;
                //this.PrintError(ex.Message);

                return false;
            }
        }

        private Boolean InsertOutToMoveLocation(DateTime pMovementDate)
        {
            Boolean result = true;
            Boolean IsUpdate = false;

            try
            {
                //1. PSC2210_T_HHT_MOVE_OUT  =>   PSC2210_T_MOVE_LOCATION
                List<MoveLocation> checkMoveLocation = new List<MoveLocation>();


                //1.1 Select PSC2210_T_HHT_MOVE_OUT where submit,moveid = null 
                List<MoveOut> moveOutList = new List<MoveOut>();
                MoveOutService objMoveOutService = new MoveOutService(this.dbEntities);
                moveOutList = objMoveOutService.GetMoveOutListByNullMoveIdandSubmitStatus(pMovementDate);

                //1.2 Check data with PSC2210_T_MOVE_LOCATION
                if (moveOutList != null)
                {
                    foreach (MoveOut enMoveOut in moveOutList)
                    {
                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        checkMoveLocation = objMoveLocationService.GetMoveLocationListByItemHeadLocationfrom(pMovementDate, enMoveOut.ITEM_CODE, enMoveOut.HEAT_NO, enMoveOut.LOCATION_CODE);

                        if (checkMoveLocation == null)
                        {
                            MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                            result = objMoveLocationService1.InsertFromMoveOut(pMovementDate, enMoveOut, this.LoginUser);
                        }
                        else
                        {
                            for (int index = 0; index < checkMoveLocation.Count; index++)
                            {
                                if (checkMoveLocation[index].Status == (int)Common.Constants.MoveLocationStatus.Submit)
                                {
                                    MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                    result = objMoveLocationService1.UpdateQtyFromMoveOut(pMovementDate, enMoveOut, this.LoginUser);
                                    IsUpdate = true;
                                }
                            }
                            if (IsUpdate == false)
                            {
                                MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                result = objMoveLocationService1.InsertFromMoveOut(pMovementDate, enMoveOut, this.LoginUser);
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

        private Boolean UpdateINToMoveLocation(DateTime pMovementDate)
        {
            //2. PSC2210_T_HHT_MOVE_IN  =>   PSC2210_T_MOVE_LOCATION
            Boolean result = true;
            Boolean IsUpdateMoveIn = false;

            try
            {
                List<MoveLocation> checkMoveLocation = new List<MoveLocation>();

                List<MoveIN> moveINList = new List<MoveIN>();
                MoveINService objMoveINService = new MoveINService(this.dbEntities);
                moveINList = objMoveINService.GetMoveINListByNullMoveIdandSubmitStatus(pMovementDate);

                if (moveINList != null)
                {
                    foreach (MoveIN enMoveIN in moveINList)
                    {
                        IsUpdateMoveIn = false;

                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        checkMoveLocation = objMoveLocationService.GetMoveLocationListByItemHeadLocationTo(pMovementDate, enMoveIN.ITEM_CODE, enMoveIN.HEAT_NO);

                        if (checkMoveLocation != null && checkMoveLocation.Count > 0)
                        {
                            foreach (MoveLocation enChkMoveLoc in checkMoveLocation)
                            {
                                if (enChkMoveLoc.Status == (int)Common.Constants.MoveLocationStatus.Submit)
                                {
                                    List<MoveOut> moveOutList = new List<MoveOut>();
                                    MoveOutService objMoveOutService = new MoveOutService(this.dbEntities);
                                    moveOutList = objMoveOutService.GetMoveOutListByMoveId(enChkMoveLoc.MoveId, Constants.HHTMoveOutStatus.SubmitTrans);

                                    if (moveOutList != null && moveOutList.Count > 0)
                                    {
                                        Boolean IsDataMatching = false;
                                        foreach (MoveOut enMoveOut in moveOutList)
                                        {
                                            if (enMoveOut.BARCODE == enMoveIN.BARCODE)
                                            {
                                                IsDataMatching = true;
                                                break;
                                            }
                                        }
                                        if (!IsDataMatching)
                                        {
                                            foreach (MoveOut enMoveOut in moveOutList)
                                            {
                                                String strOutBarcodeSub = enMoveOut.BARCODE.Length >= 46 ? enMoveOut.BARCODE.Substring(0, 46) : "";
                                                String strInBarcodeSub = enMoveIN.BARCODE.Length >= 46 ? enMoveIN.BARCODE.Substring(0, 46) : "";
                                                if (strOutBarcodeSub == strInBarcodeSub)
                                                {
                                                    IsDataMatching = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (IsDataMatching)
                                        {
                                            MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                            IsUpdateMoveIn = objMoveLocationService1.UpdateQtyFromMoveIN(enChkMoveLoc.MoveId, enMoveIN, this.LoginUser);

                                            if (IsUpdateMoveIn)
                                            {
                                                break;
                                            }
                                        }

                                    }
                                }
                                if (IsUpdateMoveIn)
                                {
                                    break;
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

        private Boolean UpdateReleaseToMoveLocation(DateTime pMovementDate)
        {
            Boolean result = true;
            Boolean IsUpdateRelease = false;

            try
            {
                List<MoveLocation> checkMoveLocation = new List<MoveLocation>();

                List<Release> ReleaseList = new List<Release>();
                ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                ReleaseList = objReleaseService.GetReleaseListByNullMoveIdandSubmitStatus(pMovementDate);

                if (ReleaseList != null && ReleaseList.Count > 0)
                {
                    foreach (Release enRelease in ReleaseList)
                    {
                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        checkMoveLocation = objMoveLocationService.GetMoveLocationListByItemHeadLocationTo(pMovementDate, enRelease.ItemCode, enRelease.HeatNo);

                        if (checkMoveLocation != null && checkMoveLocation.Count > 0)
                        {
                            foreach (MoveLocation enChkMoveLoc in checkMoveLocation)
                            {
                                if (enChkMoveLoc.Status == (int)Common.Constants.MoveLocationStatus.Submit)
                                {
                                    List<MoveOut> moveOutList = new List<MoveOut>();
                                    MoveOutService objMoveOutService = new MoveOutService(this.dbEntities);
                                    moveOutList = objMoveOutService.GetMoveOutListByMoveId(enChkMoveLoc.MoveId, Constants.HHTMoveOutStatus.SubmitTrans);

                                    if (moveOutList != null && moveOutList.Count > 0)
                                    {
                                        Boolean IsDataMatching = false;
                                        foreach (MoveOut enMoveOut in moveOutList)
                                        {
                                            if (enMoveOut.BARCODE == enRelease.Barcode)
                                            {
                                                IsDataMatching = true;
                                                break;
                                            }
                                        }
                                        if (!IsDataMatching)
                                        {
                                            foreach (MoveOut enMoveOut in moveOutList)
                                            {
                                                String strOutBarcodeSub = enMoveOut.BARCODE.Length >= 46 ? enMoveOut.BARCODE.Substring(0, 46) : "";
                                                String strInBarcodeSub = enRelease.Barcode.Length >= 46 ? enRelease.Barcode.Substring(0, 46) : "";
                                                if (strOutBarcodeSub == strInBarcodeSub)
                                                {
                                                    IsDataMatching = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (IsDataMatching)
                                        {
                                            MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                            IsUpdateRelease = objMoveLocationService1.UpdateQtyFromRelease(enChkMoveLoc.MoveId, enRelease, this.LoginUser);

                                            if (IsUpdateRelease)
                                            {
                                                break;
                                            }
                                        }

                                    }
                                }
                                if (IsUpdateRelease)
                                {
                                    break;
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
        //public bool SetIsFilter()
        //{
        //    bool result = true;

        //    ViewBag.IsFilter = "False";

        //    return result;
        //}

        // Save Onclick 
        [HttpPost]
        public ActionResult Update_MoveLocationList(List<UpdateModel> pUpdateList )
        {
            

            try
            {
                var result = true;
                var msg= string.Empty;
                Boolean checkRemark = true;
                
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var tempMoveLocationList = this.model.MoveLocationList;

                for (int i = 0; i < this.model.MoveLocationList.Count; i++)
                {
                    if (pUpdateList[i].Remark != null)
                    {
                        tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
                        checkRemark = true;
                    }
                    else
                    {
                        checkRemark = false;
                        break;
                    }
                }
                if (checkRemark == true)
                {
                    for (int i = 0; i < this.model.MoveLocationList.Count; i++)
                    {
                        tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
                    }

                    result = objMoveLocationService.UpdateMoveLocationData(tempMoveLocationList, userId);
                    msg  = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                    this.model.MoveLocationList = tempMoveLocationList;

                    // Alert Message
                    this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                    this.model.Message = msg;
                    ModelState.Clear();
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.Common_cshtml.CheckSaveAllRemarksMsg;
                    result = false;
                    msg = Resources.Common_cshtml.CheckAllRemarksMsg;
                }

                return Json(new { success = result, message = msg });
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, PSCS.Resources.PSC2210_cshtml.ErrorNoUser);
            }
        }

        // Approve Onclick 
        [HttpPost]
        public ActionResult Approve_MoveLocationList(List<UpdateModel> pUpdateList)
        {
            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var tempMoveLocationList = this.model.MoveLocationList;

                Boolean checkRemark = true;
                Boolean checkDestination = true;

                string MonthlyClose = string.Empty;
                MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                if (objMonthlyClose != null)
                {
                    DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                    string strFMonth = dFMonth.ToString("yyyy-MM");

                    MonthlyClose = strFMonth + "-01";
                }


                for (int i = 0; i < this.model.MoveLocationList.Count; i++)
                {
                    if (pUpdateList[i].Remark != null)
                    {
                        tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
                        checkRemark = true;
                    }
                    else
                    {
                        checkRemark = false;
                        break;
                    } 
                    
                    if (pUpdateList[i].Destination != null)
                    {
                        tempMoveLocationList[i].LocationCodeTo = pUpdateList[i].Destination;
                        checkDestination = true;
                    }
                     else
                    {
                        checkDestination = false;
                        break;
                    }

                }

                if (checkRemark == true && checkDestination == true)
                {
                    //Save
                    var result = objMoveLocationService.UpdateMoveLocationData(tempMoveLocationList, userId);
                    var msg = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                    this.model.MoveLocationList = tempMoveLocationList;



                    if (result == true)
                    {
                        List<MoveLocation> pDataList = new List<MoveLocation>();
                        pDataList = this.model.MoveLocationList;
                        //Approve
                        //Approve (Update Status)
                        foreach (MoveLocation en in pDataList)
                        {
                            DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);
                            DateTime stockDate = DateTime.Now;//DateTime.ParseExact(iString, "yyyy-MM-dd", null);

                            StockListService objStockListService = new StockListService(this.dbEntities);

                            List<MoveLocation> objMLoOut = new List<MoveLocation>();
                            MoveLocationService objMoveLocationServiceOut = new MoveLocationService(this.dbEntities);
                            objMLoOut = objMoveLocationServiceOut.GetMoveLocationByMoveId(en.MoveId);

                            if (objMLoOut != null && objMLoOut.Count >0)
                            {
                                result = objStockListService.UpdateOutData(dateYearMonth,stockDate, objMLoOut[0].ItemCode, objMLoOut[0].HeatNo, objMLoOut[0].LocationCodeFrom, Convert.ToDecimal(objMLoOut[0].QTY), userId);
                                msg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                if(!result)
                                {
                                    msg = msg + "  " + Resources.Common_cshtml.HeatNo + " = " + objMLoOut[0].HeatNo + " ("+ Resources.Common_cshtml.FromLocation + " : " + GetDisplayLocation(objMLoOut[0].LocationCodeFrom) + ", " + Resources.Common_cshtml.DestinationLocation + " : " + GetDisplayLocation(objMLoOut[0].LocationCodeTo) + ")";
                                    break;
                                }
                            }                         
                        }

                        if (result == true)
                        {
                            foreach (MoveLocation en in pDataList)
                            {
                                //string strYearMonth = "2019-04-01"; //PSC3010_M_MONTHLY_CLOSE
                                DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);
                                DateTime dateStock = DateTime.Now;

                                StockListService objStockListInService = new StockListService(this.dbEntities);

                                List<MoveLocation> objMLoIn = new List<MoveLocation>();
                                MoveLocationService objMoveLocationServiceIn = new MoveLocationService(this.dbEntities);
                                objMLoIn = objMoveLocationServiceIn.GetMoveLocationByMoveId(en.MoveId);
                                if (objMLoIn != null && objMLoIn.Count > 0)
                                {
                                    result = objStockListInService.UpdateInData(dateYearMonth, dateStock, objMLoIn[0].ItemCode, objMLoIn[0].HeatNo, objMLoIn[0].LocationCodeTo, Convert.ToDecimal(objMLoIn[0].QTY), userId);
                                }     
                            }

                            MoveLocationService objMoveLocation = new MoveLocationService(this.dbEntities);
                            result = objMoveLocation.ApproveMoveLocation(pDataList, userId);
                            msg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;
                        }

                        // if IN FROM Release (IS_RELEAE = 1) Update  PSC2411_T_RELEASE_DETAIL
                        decimal? MoveLoQTY = 0;
                        if (result == true)
                        {
                            foreach (MoveLocation en in pDataList)
                            {
                                List<MoveLocation> objMLoRelease = new List<MoveLocation>();
                                MoveLocationService objMoveLocationServiceRelease = new MoveLocationService(this.dbEntities);
                                objMLoRelease = objMoveLocationServiceRelease.GetMoveLocationByMoveId(en.MoveId);

                                if (objMLoRelease != null && objMLoRelease.Count > 0 )
                                {
                                    MoveLoQTY = objMLoRelease[0].QTY.HasValue ? objMLoRelease[0].QTY : 0;
                                    if (objMLoRelease[0].Is_Release == (int)Common.Constants.MoveLocationIn.Release)
                                    {
                                        result = UpdateReleaseDetail(en.MoveId, MoveLoQTY);

                                    }
                                }
                            }

                        }

                    }              


                        //Alert Message
                        this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                    this.model.Message = msg;
                    ModelState.Clear();
                    return Json(new { success = result, message = msg });
                }
                else
                {
                    string msg = string.Empty;

                    if (checkDestination == false)
                    {
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.Common_cshtml.CheckAllDestinationsMsg;
                        msg = Resources.Common_cshtml.CheckAllDestinationsMsg;
                        //this.PrintError(Resources.Common_cshtml.CheckAllDestinationsMsg);
                    }
                    else if (checkRemark == false)
                    {
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.Common_cshtml.CheckAllRemarksMsg;
                        msg = Resources.Common_cshtml.CheckAllRemarksMsg;
                        //this.PrintError(Resources.Common_cshtml.CheckAllRemarksMsg);
                    }

                   
                    return Json(new { success = false, message = msg });
                }
              
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        private string GetDisplayLocation(string pLocationCode)
        {
            string result = string.Empty;

            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);
                Location objLocation = objLocationService.GetLocationByLocationCode(pLocationCode);
                if (objLocation != null)
                {
                    result = objLocation.YardName + "-" + objLocation.Name;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean UpdateReleaseDetail(decimal moveId, decimal? actualQty)
        {
            Boolean result = false;
            //1. Find JOB_NO at PSC2410_T_HHT_RELEASE by MOVE_ID 
            List<Release> objReleaseJOBNo = new List<Release>();
            MoveLocationService objMoveLocationServiceReleaseJOBNO = new MoveLocationService(this.dbEntities);
            objReleaseJOBNo = objMoveLocationServiceReleaseJOBNO.Get2410ReleaseByMoveID(moveId);

            var jobNos = new List<string>();
            var ReqsQTYbyJobNos = new List<decimal>();
            var requestIDs = new List<decimal>();
            var releaseIDs = new List<decimal>();
            string jobNoTxt = "";
            

            if (objReleaseJOBNo != null && objReleaseJOBNo.Count > 0)
            {
                foreach (Release enJobNo in objReleaseJOBNo)
                {
                    jobNoTxt = enJobNo.JobNo;
                    if (jobNoTxt != "")
                    {
                        string[] selectedjobNoTxt = jobNoTxt.Split(',');
                        if (selectedjobNoTxt != null && selectedjobNoTxt.Length > 0)
                        {
                            for (int index = 0; index < selectedjobNoTxt.Length; ++index)
                            {
                                if (!jobNos.Contains(selectedjobNoTxt[index]))
                                {
                                    jobNos.Add(selectedjobNoTxt[index]);

                                    //2.Find Request_ID at PSC2420_T_REQUEST by JOB_NO
                                    List<Request> ListReleaseReqID = new List<Request>();
                                    MoveLocationService objMoveLocationServiceReqID = new MoveLocationService(this.dbEntities);
                                    ListReleaseReqID = objMoveLocationServiceReqID.Get2420RequestByJobNo(selectedjobNoTxt[index]);
                                    decimal? reqQTY = 0;

                                    if (ListReleaseReqID != null && ListReleaseReqID.Count > 0)
                                    {
                                        reqQTY = 0;
                                        foreach (Request enReqID in ListReleaseReqID)
                                        {
                                            if (!requestIDs.Contains(enReqID.RequestId))
                                            {
                                                requestIDs.Add(enReqID.RequestId);
                                            }

                                            //Collect Request_Qty 
                                            reqQTY += enReqID.RequestQTY.GetValueOrDefault(0);

                                            //3.Find Release_ID at PSC2421_T_REQUEST_RELEASE by REQUEST_ID
                                            List<RequestRelease> objReleaseReleaseID = new List<RequestRelease>();
                                            MoveLocationService objMoveLocationServiceREleaseID = new MoveLocationService(this.dbEntities);
                                            objReleaseReleaseID = objMoveLocationServiceREleaseID.Get2421RequestReleaseByRequestID(enReqID.RequestId);

                                            if (objReleaseReleaseID != null && objReleaseReleaseID.Count > 0)
                                            {
                                                foreach (RequestRelease enReleaseID in objReleaseReleaseID)
                                                {
                                                    if (!releaseIDs.Contains(enReleaseID.ReleaseId))
                                                    {
                                                        releaseIDs.Add(enReleaseID.ReleaseId);
                                                    }
                                                }
                                            }
                                        }
                                        ReqsQTYbyJobNos.Add(reqQTY.GetValueOrDefault(0));

                                    }
                                }
                            }
                        }
                      
                    }
                         
        
                }
            }


            //4. UPDATE QTY at PSC2411_T_RELEASE_DETAIL by RELEASE_ID
            // GET PSC2411_T_RELEASE_DETAIL BY RELEASE_ID 
            decimal? oldReleaseDetail_ActualQty = 0;
            decimal? oldReleaseDetail_RequestQty = 0;
            decimal? newReleaseDetail_ActualQty = 0;
            Boolean updateReleaseFlg = false;
            string userId = this.LoginUser.UserId;
            decimal pReleaseID = 0;

            if (releaseIDs != null && releaseIDs.Count > 0)
            {
                pReleaseID = releaseIDs[0];
                ReleaseDetail objReleaseDetail = null;
                ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                objReleaseDetail = objReleaseDetailService.GetReleaseDetail(pReleaseID);
                if (objReleaseDetail != null)
                {
                    oldReleaseDetail_ActualQty = objReleaseDetail.ActualQTY.HasValue ? objReleaseDetail.ActualQTY : 0;
                    oldReleaseDetail_RequestQty = objReleaseDetail.RequestQTY.HasValue ? objReleaseDetail.RequestQTY : 0;
                }

                newReleaseDetail_ActualQty = oldReleaseDetail_ActualQty + actualQty;               
                if (newReleaseDetail_ActualQty == oldReleaseDetail_RequestQty)
                {
                    //Update status
                    updateReleaseFlg = true;
                }

                //Update PSC2411_T_RELEASE_DETAIL               
                ReleaseDetailService objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                result = objUpReleaseDetail.UpdateReleaseDetailData(releaseIDs[0], newReleaseDetail_ActualQty, updateReleaseFlg, userId);                                                          
            }

            //5. UPDATE PSC2420_T_REQUEST by JOB_NO when releaseflg = true
            if (result == true && jobNos != null && jobNos.Count > 0 && updateReleaseFlg == true)
            {
                foreach (string strJobNo in jobNos)
                {   //UpdateRequestStatus
                    RequestService objUpRequest = new RequestService(this.dbEntities);
                    result = objUpRequest.UpdateRequestStatus(strJobNo, userId);
                   if  (result == false)
                    {
                        break;
                    }
                }
            }

            //6. UPDATE PSC2410_T_RELEASE by JobNo
            decimal relQTY = 0;
            decimal oldrelReleaseQTY = 0;
            decimal newrelReleaseQTY = 0;
            Boolean chkUpdateRelease = false;
            if (result == true && jobNos != null && jobNos.Count > 0 && updateReleaseFlg == true)
            {
                for (int i = 0; i < jobNos.Count; ++i)
                {
                    chkUpdateRelease = false;
                    Release getRelease = null;
                    //PSCS RELEASE TABLE
                    ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                    getRelease = objReleaseService.GetReleaseData(jobNos[i].ToString());

                   if (getRelease != null)
                    {
                        relQTY = getRelease.QTY;
                        oldrelReleaseQTY = getRelease.ReleaseQTY.GetValueOrDefault(0);
                        newrelReleaseQTY = ReqsQTYbyJobNos[i];

                        if (newrelReleaseQTY == relQTY)
                        {
                            chkUpdateRelease = true;
                        }

                        //UPDATE
                        //Update PSC2411_T_RELEASE_DETAIL               
                        ReleaseService objUpRelease = new ReleaseService(this.dbEntities);
                        result = objUpRelease.Update2410Release(jobNos[i].ToString(), newrelReleaseQTY, chkUpdateRelease, userId);
                        if (result == false)
                        {
                            break;
                        }
                    }                  
                }                  
            }
                       

            return result;

        }

        [HttpPost]
        public string GetMoveLocationdata(decimal pMoveId)
        {
            string result = string.Empty;

            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                result = objMoveLocationService.GetMoveLocationDataByMoveId(pMoveId);

                return result;
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return string.Empty;
            }
        }

        [HttpPost]
        public string GetMoveLocationdataNotMoveId(MoveLocationScreen FilterModel, string pAllMoveId, string pMovedate, string pYardId)
        {
            string result = string.Empty;
            List<decimal> MoveIdList = new List<decimal>();

            try
            {
                if (pAllMoveId != null && pAllMoveId != "")
                {
                    string[] AllMoveIdList = pAllMoveId.Split(',');

                    if (AllMoveIdList != null)
                    {
                        for (int i = 0; i < AllMoveIdList.Length; i++)
                        {
                            if (AllMoveIdList[i] != "")
                            {
                                MoveIdList.Add(Convert.ToDecimal(AllMoveIdList[i]));
                            }

                        }
                    }
                }

                DateTime deliveryDate = Convert.ToDateTime(pMovedate).Date;
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                result = objMoveLocationService.GetReceivePlanListNotMoveId(deliveryDate, pYardId, MoveIdList);

                if (result != "")
                {
                    // Add new record to model 
                    string[] selectedList = result.Split('*');
                    if (selectedList != null && selectedList.Length > 0)
                    {
                        var tempMoveLocationList = this.model.MoveLocationList;
                        MoveLocation newMoveLocation = new MoveLocation();
                        newMoveLocation.MoveId = decimal.Parse(selectedList[0]);
                        tempMoveLocationList.Add(newMoveLocation);
                        this.model.MoveLocationList = tempMoveLocationList;
                    }
                }

                return result;
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return string.Empty;
            }
        }

        public ActionResult DeleteMoveLocation(int rowId)
        {
            try
            {
                bool result = false;
                result = Delete_MoveLocationList(rowId);

            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            return View("", null);
        }

        // Filter ReceivePlan view
        private ActionResult Filter_OnClick(MoveLocationScreen FilterModel)
        {
            try
            {
                //this.model.MoveLocationList = GetMoveLocationList(FilterModel.FilterInternalMoveDate,
                //                                                  FilterModel.FilterYardID);

                //this.model.FilterInternalMoveDate = FilterModel.FilterInternalMoveDate;
                //this.model.FilterYardID = FilterModel.FilterYardID;
                //this.model.Total = this.model.MoveLocationList.Count.ToString();

                SearchDataToGrid(FilterModel.FilterInternalMoveDate, FilterModel.FilterYardID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2210", this.model);

        }

        private Boolean SearchDataToGrid(DateTime pMoveDate, string pYardId)
        {
            Boolean result= false;

            this.model.MoveLocationList = GetMoveLocationList(pMoveDate, pYardId);

            this.model.FilterInternalMoveDate = pMoveDate;
            this.model.FilterYardID = pYardId;
            this.model.Total = this.model.MoveLocationList.Count.ToString();

            return result;
        }

        private Boolean ClearGrid()
        {
            Boolean result = false;

            List<MoveLocation> obj = new List<MoveLocation>();
            this.model.MoveLocationList = obj;          
            this.model.Total = "0";

            return result;
        }

        // //Refresh
        //// [HttpPost]
        // public ActionResult Refresh_MoveLocationList(List<UpdateModelRefresh> pUpdateList, DateTime pMoveDate, string pYardId)
        // {
        //     try
        //     {
        //         MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
        //         string userId = this.LoginUser.UserId;
        //         //var tempMoveLocationList = this.model.MoveLocationList;

        //         //// Get MoveLocationList 
        //         List<MoveLocation> newMoveLocationList = new List<MoveLocation>();

        //         newMoveLocationList = GetMoveLocationList(pMoveDate, pYardId);

        //         if (newMoveLocationList.Count > 0)
        //         {
        //             for (int i = 0; i < newMoveLocationList.Count; i++)
        //             {

        //                 //tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
        //                 List<UpdateModelRefresh> objTemp = pUpdateList.Where(lo => lo.MoveId == newMoveLocationList[i].MoveId).ToList();

        //                 if (objTemp != null && objTemp.Count > 0)
        //                 {
        //                     newMoveLocationList[i].Remark = objTemp[0].Remark;
        //                 }

        //             }
        //         }

        //         this.model.MoveLocationList = newMoveLocationList;
        //         return View("PSC2210", this.model);

        //         //var result = true;//objMoveLocationService.UpdateMoveLocationData(tempMoveLocationList, userId);
        //         //var msg = result ? Resources.Common_cshtml.EditSuccessMsg : Resources.Common_cshtml.EditFailMsg;
        //         //this.model.MoveLocationList = newMoveLocationList;

        //         //// Alert Message
        //         //this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
        //         //this.model.Message = msg;
        //         //ModelState.Clear();

        //         //return Json(new { success = result, message = msg });

        //     }
        //     catch (Exception ex)
        //     {
        //         this.model.AlertsType = Constants.AlertsType.Danger;
        //         this.model.Message = ex.Message;
        //         this.PrintError(ex.Message);

        //         return new HttpStatusCodeResult(400, "Unable to find customer.");
        //     }
        // }


        [HttpGet]
        public ActionResult PSC2211(string _id, string _date, string _start_time, string _end_time, string _yard)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;

                this.modelEdit.YardList = GetYard();
                this.modelEdit.HoursList = GetHours();
                this.modelEdit.MinuteList = GetMinute();
                string[] startTime = "09:00".Split(':');
                string[] endTime = "11:30".Split(':');

                // Initial service
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                var result = objMoveLocationService.GetReceiveDetailList(_id);

                if (result != null)
                {
                    this.modelEdit.MoveLocationList = result;
                    this.modelEdit.DetailInternalMoveDate = _date;
                    this.modelEdit.DetailStartHours = startTime[0];
                    this.modelEdit.DetailStartMinute = startTime[1];
                    this.modelEdit.DetailFinishHours = endTime[0];
                    this.modelEdit.DetailFinishMinute = endTime[1];
                    this.modelEdit.DetailYardID = _yard;

                    return View(this.modelEdit);
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.Common_cshtml.NoDataFound;
                    this.modelEdit.HasError = true;

                    return View("PSC2210", this.model);
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
                this.modelEdit.HasError = true;

                return View("PSC2210", this.model);
            }
        }

        [HttpPost]
        public ActionResult PSC2211(string submitButton, MoveLocationScreenEdit modelEdit)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = this.LoginUser.UserId;
                    string message = string.Empty;
                    Boolean result = false;

                    // Initial model
                    ViewBag.LoginUserName = this.LoginUser.UserId;
                    ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;

                    // Initial service
                    MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                 
                    switch (submitButton)
                    {
                        case "Save":
                            result = true;
                            message = result ? Resources.Common_cshtml.AddSuccessMsg : Resources.Common_cshtml.AddFailMsg;
                            break;
                        case "Approve":
                            result = true;
                            message = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;
                            break;
                        case "Delete":
                            result = true;
                            message = result ? Resources.Common_cshtml.DeleteSuccessMsg : Resources.Common_cshtml.DeleteFailMsg;
                            break;
                        case "Back":
                            this.model.AlertsType = Constants.AlertsType.None;
                            this.model.Message = "";
                            return View("PSC2210", this.model);
                        default:
                            result = false;
                            break;
                    }

                    // Alert Message
                    this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                    this.model.Message = message;

                    return View("PSC2110", this.model);
                }
                catch (Exception ex)
                {
                    this.model.AlertsType = Common.Constants.AlertsType.Danger;
                    this.model.Message = ex.Message;
                    this.PrintError(ex.Message);
                    this.modelEdit.HasError = true;

                    return RedirectToAction("PSC2210", "MoveLocation");
                }
            }

            this.modelEdit.YardList = GetYard();
            this.modelEdit.HoursList = GetHours();
            this.modelEdit.MinuteList = GetMinute();

            return View(modelEdit);
        }

        private void IntialPSC2210()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2210";
            QueryStringList = new Dictionary<string, string>();
            this.model.YardList = GetYard();
            this.model.MoveLocationList = this.GetMoveLocationList(model.FilterInternalMoveDate, "");
        }

        private List<MoveLocation> GetMoveLocationList(DateTime pMoveDate, string pYardId)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            //string pLanguage = langCookie != null ? langCookie.Value : "En";
            result = objMoveLocationService.GetMoveLocationList(pMoveDate, pYardId);

            return result;
        }

        // Parent view: Approve Onclick 
        private bool Approve_MoveLocationList()
        {
            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var pDataList = this.model.MoveLocationList;
                string message = string.Empty;
                Boolean result = false;

                if (pDataList != null && LoginUser != null)
                {
                    result = objMoveLocationService.ApproveMoveLocation(pDataList, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Parent view: Delete Onclick 
        private bool Delete_MoveLocationList(int rowId)
        {
            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var pDataList = this.model.MoveLocationList;
                string message = string.Empty;
                Boolean result = false;
                //decimal MoveId = 0;
                if (pDataList != null && LoginUser != null)
                {
                    //MoveId = pDataList[rowId - 1].MoveId;
                    result = objMoveLocationService.DeleteMoveLocation(rowId);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<SelectListItem> GetYard()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            YardService objYardService = new YardService(this.dbEntities);
            var objYardList = objYardService.GetYardList();

            foreach (Yard objYard in objYardList)
            {
                result.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
            }

            return result;
        }

        private List<SelectListItem> GetHours()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = 0; i < 24; i++)
            {
                result.Add(new SelectListItem { Text = i.ToString("00"), Value = i.ToString("00") });
            }

            return result;
        }

        private List<SelectListItem> GetMinute()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = 0; i < 60; i++)
            {
                result.Add(new SelectListItem { Text = i.ToString("00"), Value = i.ToString() });
            }

            return result;
        }

    }
}