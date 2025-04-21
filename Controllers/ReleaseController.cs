using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.ModelsScreen;
using PSCS.Models;
using PSCS.ModelERPDEV01;
using System.Globalization;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class ReleaseController : BaseController
    {
        public ReleaseScreen model
        {
            get
            {
                if (Session["ReleaseScreen"] == null)
                {
                    Session["ReleaseScreen"] = new ReleaseScreen();
                    return (ReleaseScreen)Session["ReleaseScreen"];
                }
                else
                {
                    return (ReleaseScreen)Session["ReleaseScreen"];
                }
            }
            set { Session["ReleaseScreen"] = value; }
        }

        #region "Sub&Function"
        private void IntialPSC2410()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            InitializeActionName = "PSC2410";
            QueryStringList = new Dictionary<string, string>();
        }

        private Release GetRelease(string pJobNo)
        {
            Release result = null;
            List<JobMasterData> objJobMasterDataList = null;

            try
            {
                //PSCS RELEASE TABLE
                ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                result = objReleaseService.GetReleaseData(pJobNo);

                if (result == null)
                {
                    GetJobMasterFromSyteLineService objGetWIFromSyteLineService = new GetJobMasterFromSyteLineService(this.dbERPDEV01Entities);
                    objJobMasterDataList = objGetWIFromSyteLineService.GetWIFromSyteLine(pJobNo);

                    if (objJobMasterDataList != null)
                    {
                        if (objJobMasterDataList.Count > 0)
                        {
                            JobMasterService objJobMasterService = new JobMasterService(this.dbEntities, this.LoginUser.UserId);
                            Boolean IsUpdate = objJobMasterService.JobMaster(objJobMasterDataList);
                            if (IsUpdate)
                            {
                                //PSCS RELEASE TABLE
                                ReleaseService objRelease1Service = new ReleaseService(this.dbEntities);
                                result = objRelease1Service.GetReleaseData(pJobNo);
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

        private Boolean CheckDuplicateWIBarcodeOnGrid(string pWIBarcode)
        {
            Boolean blnResult = false;

            if (this.model.WIRequestList != null && this.model.WIRequestList.Count > 0)
            {
                foreach (Request en in this.model.WIRequestList)
                {
                    if (en.JobNo.Equals(pWIBarcode))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }

            return blnResult;

        }

        private ActionResult WIBarcodeSearch(string pWIBarcode, List<Request> pRequestList)
        {
            Release objRelease = null;
            List<JobMasterData> objJobMasterDataList = null;

            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;
                
                if (!string.IsNullOrEmpty(pWIBarcode))
                {
                    //Check duplicate 
                    Boolean chk = false;
                    chk = CheckDuplicateWIBarcodeOnGrid(pWIBarcode);

                    if (chk == false)
                    {
                        ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                        objRelease = objReleaseService.GetReleaseData(pWIBarcode);
                        if (objRelease == null)
                        {
                            GetJobMasterFromSyteLineService objGetWIFromSyteLineService = new GetJobMasterFromSyteLineService(this.dbERPDEV01Entities);
                            objJobMasterDataList = objGetWIFromSyteLineService.GetWIFromSyteLine(pWIBarcode);

                            if (objJobMasterDataList != null)
                            {
                                if (objJobMasterDataList.Count > 0)
                                {
                                    JobMasterService objJobMasterService = new JobMasterService(this.dbEntities, this.LoginUser.UserId);
                                    Boolean IsUpdate = objJobMasterService.JobMaster(objJobMasterDataList);
                                    if (IsUpdate)
                                    {
                                        ReleaseService objRelease1Service = new ReleaseService(this.dbEntities);
                                        objRelease = objRelease1Service.GetReleaseData(pWIBarcode);
                                    }
                                }
                            }
                        }

                        if (objRelease != null)
                        {
                            decimal decSumRequestQTY = 0;
                            RequestService objRequestService = new RequestService(this.dbEntities);
                            List<Request> objRequestList = objRequestService.GetRequestsList(objRelease.JobNo);

                            if(objRequestList != null)
                            {
                                foreach(Request en in objRequestList)
                                {
                                    decSumRequestQTY = decSumRequestQTY + Convert.ToDecimal(en.RequestQTY);
                                }
                            }
                            if (decSumRequestQTY < objRelease.QTY)
                            {
                                ////Release objRelease = GetRelease(pWIBarcode);
                                //this.model.WIRequestList = objRequestService.AddIntoGrid(this.model.WIRequestList, objRelease);
                                //this.model.Total = this.model.WIRequestList.Count.ToString();
                                decimal decRereaseQTY = 0;
                                decimal decRequestQTY = 0;
                                decimal decRemainQTY = 0;

                                if (decSumRequestQTY == 0 )
                                {
                                    decRereaseQTY = Convert.ToDecimal(objRelease.QTY);
                                    decRequestQTY = decRereaseQTY;
                                    decRemainQTY =  0;
                                }
                                else
                                {
                                    decRereaseQTY = Convert.ToDecimal(objRelease.QTY);
                                    decRequestQTY = decRereaseQTY - decSumRequestQTY;
                                    decRemainQTY = decRereaseQTY - (decSumRequestQTY + decRequestQTY);
                                }

                                //New Record
                                Boolean IsInsert = false;
                                Request objRequest = new Request();
                                objRequest.JobNo = objRelease.JobNo;
                                objRequest.ItemCode = objRelease.ItemCode;
                                objRequest.HeatNo = objRelease.HeatNo;
                                objRequest.MfgNo = objRelease.MfgNo;
                                objRequest.Description = objRelease.Description;
                                objRequest.Maker = objRelease.Maker;
                                objRequest.Maker_Name = objRelease.Maker_Name;
                                objRequest.ReleaseQTY = decRereaseQTY;
                                objRequest.RequestQTY = decRequestQTY;
                                objRequest.RemainQTY = decRemainQTY;

                                IsInsert = objRequestService.Insert(objRequest, this.LoginUser.UserId);
                                if(IsInsert)
                                {
                                    RequestService objGetRequestService = new RequestService(this.dbEntities);
                                    this.model.WIRequestList = objGetRequestService.GetRequestsList(Constants.RequestStatus.Draft);
                                    this.model.Total = this.model.WIRequestList.Count.ToString();
                                }
                            }
                            else
                            {
                                //Warning 
                                this.model.AlertsType = Constants.AlertsType.Warning;
                                this.model.Message = PSCS.Resources.PSC2410_cshtml.ErrorJobNo;
                                this.PrintError(PSCS.Resources.PSC2410_cshtml.ErrorJobNo);
                            }
                        }
                    }
                    else
                    {
                        //Warning 
                        this.model.AlertsType = Constants.AlertsType.Warning;
                        this.model.Message = PSCS.Resources.PSC2410_cshtml.ErrorDuplicode;
                        this.PrintError(PSCS.Resources.PSC2410_cshtml.ErrorDuplicode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            this.model.HasRequest = true;
            ModelState.Clear();
            return View(this.model);
        }
        private ActionResult Delete_OnClick1(List<decimal> pRequestIdList)
        {
            Boolean result = false;
            RequestService objRequestService = null;
            string message = string.Empty;

            try
            {
                if (pRequestIdList != null)
                {
                    foreach (decimal decRequestId in pRequestIdList)
                    {
                        objRequestService = new RequestService(this.dbEntities);
                        result = objRequestService.Delete(decRequestId);
                        if (!result)
                        {
                            break;
                        }
                    }

                    if (result)
                    {
                        message = result ? Resources.Common_cshtml.DeleteSuccessMsg : Resources.Common_cshtml.DeleteFailMsg;

                        // Filter
                        objRequestService = new RequestService(this.dbEntities);
                        this.model.WIRequestList = objRequestService.GetRequestsList(Constants.RequestStatus.Draft);
                        this.model.Total = this.model.WIRequestList.Count.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                this.model.SelectedRequestId = string.Empty;
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
            }

            this.model.SelectedRequestId = string.Empty;

            // Alert Message
            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
            this.model.Message = message;

            return View(this.model);
        }
        private ActionResult Delete_OnClick(List<Request> pRequestIdList)
        {
            Boolean result = false;
            RequestService objRequestService = null;
            string message = string.Empty;

            try
            {
                if (pRequestIdList != null)
                {
                    foreach(Request decRequestId in pRequestIdList)
                    {
                        objRequestService = new RequestService(this.dbEntities);
                        result = objRequestService.Delete(pRequestIdList);
                        if (!result)
                        {
                            break;
                        }
                    }
                    
                    if (result)
                    {
                        message = result ? Resources.Common_cshtml.DeleteSuccessMsg : Resources.Common_cshtml.DeleteFailMsg;
                        
                        // Filter
                        objRequestService = new RequestService(this.dbEntities);
                        this.model.WIRequestList = objRequestService.GetRequestsList(Constants.RequestStatus.Draft);
                        this.model.Total = this.model.WIRequestList.Count.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                this.model.SelectedRequestId = string.Empty;
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
            }

            this.model.SelectedRequestId = string.Empty;

            // Alert Message
            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
            this.model.Message = message;

            return View(this.model);
        }

        private Boolean CheckSumByItemCodeHeatNoNotDecimal()
        {
            Boolean result = true;
            string strItemCode = string.Empty;
            string strHeatNo = string.Empty;

            decimal? deSum;

            // 1. Check Sum (ITEMcode,HeatNo) == integer , not decimal 
            foreach (Request en in this.model.WIRequestList)
            {
                strItemCode = en.ItemCode;
                strHeatNo = en.HeatNo;
                deSum = 0;

                var item = this.model.WIRequestList.Where(x => x.ItemCode == strItemCode && x.HeatNo == strHeatNo).ToList();

                if (item != null && item.Count > 0)
                {
                    foreach (var ob in item)
                    {
                        if (ob.RequestQTY != null)
                        {
                            deSum = deSum + ob.RequestQTY;
                        }
                        else
                        {
                            return false;
                        }
                      
                    }

                    if ((deSum % 1) > 0)
                    {
                        //is decimal
                        return false;
                    }
                    else
                    {
                        //is int                        
                    }
                }
            }

            return result;
        }

        private ActionResult Request_OnClick(List<Request> pRequestList, string pUserId)
        {
            Boolean result = false;
            Boolean resultInteger = true;

            try
            {             
                resultInteger = CheckSumByItemCodeHeatNoNotDecimal();
                if (resultInteger == false)
                {
                    this.model.AlertsType = Constants.AlertsType.Warning;
                    this.model.Message = PSCS.Resources.PSC2410_cshtml.ErrorDecimal;
                    this.PrintError(PSCS.Resources.PSC2410_cshtml.ErrorDecimal);
                }
                else
                {
                    //if (this.model.WIRequestDelete != null && this.model.WIRequestDelete.Count > 0)
                    //{
                    //    RequestService objRequestServiceDel = new RequestService(this.dbEntities);
                    //    resultDel = objRequestServiceDel.Delete(this.model.WIRequestDelete);
                    //}

                    //if (resultDel == true)
                    //{
                    //    RequestService objRequestService = new RequestService(this.dbEntities);
                    //    result = objRequestService.SaveData(pRequestList, pUserId);
                    //    if (result)
                    //    {
                    //        //Redirect to 2420
                    //        return RedirectToAction("PSC2420", "ReleaseComfirm");
                    //    }
                    //}

                    RequestService objRequestService = new RequestService(this.dbEntities);
                    result = objRequestService.UpdateData(pRequestList, pUserId);
                    if (result)
                    {
                        return RedirectToAction("PSC2420", "ReleaseComfirm");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            this.model.HasRequest = true;
            ModelState.Clear();
            return View(this.model);
        }

        #endregion

        #region "PSC2410 WI Request"
        // GET: Release
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2410()
        {
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                //DefaultFiltter
                //model.FilterReleaseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //FilterModel.FilterReleaseDate = model.FilterReleaseDate;

                // Initial View
                IntialPSC2410();
                if (!this.model.HasRequest)
                {
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;
                    this.model.WIRequestList = new List<Request>();                  
                }

                RequestService objRequestService = new RequestService(this.dbEntities);
                this.model.WIRequestList = objRequestService.GetRequestsList(Constants.RequestStatus.Draft);
                this.model.Total = this.model.WIRequestList.Count.ToString();
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
            }

            return View(this.model);
        }

        [HttpPost]
        public ActionResult PSC2410(ReleaseScreen pModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                try
                {
                    ViewBag.LoginUserName = this.LoginUser.UserId;
                    switch (submitButton)
                    {
                        case null:
                            return WIBarcodeSearch(pModel.WiBarcode, pModel.WIRequestList);

                        case "Back":
                            return RedirectToAction("PSC0100", "Menu");

                        case "Delete":
                            //int intSelectedRowNo = pModel.SelectedRowNo;

                            string[] strRequestIdList = pModel.SelectedRequestId.Split(',');
                            List<decimal> objRequestIdList = null;

                            if (strRequestIdList != null)
                            {
                                foreach (string strRequestId in strRequestIdList)
                                {
                                    if (objRequestIdList == null)
                                    {
                                        objRequestIdList = new List<decimal>();
                                    }
                                    objRequestIdList.Add(Convert.ToDecimal(strRequestId));
                                }
                            }

                            return Delete_OnClick1(objRequestIdList);

                        case "Request":
                            return Request_OnClick(pModel.WIRequestList, this.LoginUser.UserId);

                        default:
                            return View(this.model);

                    }
                }
                catch (Exception ex)
                {
                    this.model.AlertsType = Common.Constants.AlertsType.Danger;
                    this.model.Message = ex.Message;
                    this.PrintError(ex.Message);
                    return View(this.model);
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }
        }
        #endregion

    }
}