using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.ModelsScreen;
using PSCS.Models;
using System.Globalization;
using System.Net;
using ZXing;
using OfficeOpenXml.Table.PivotTable;
using System.Data.Entity.Infrastructure;
using System.Windows.Media.Animation;
using System.Security.AccessControl;
using System.Configuration;
using PSCS.ModelERPDEV01;

namespace PSCS.Controllers
{
    public class ReleaseEditController : BaseController
    {
        public ReleaseSelectLocationScreen model
        {
            get
            {
                if (Session["ReleaseSelectLocationScreen"] == null)
                {
                    Session["ReleaseSelectLocationScreen"] = new ReleaseSelectLocationScreen();
                    return (ReleaseSelectLocationScreen)Session["ReleaseSelectLocationScreen"];
                }
                else
                {
                    return (ReleaseSelectLocationScreen)Session["ReleaseSelectLocationScreen"];
                }
            }
            set { Session["ReleaseSelectLocationScreen"] = value; }
        }

        #region "Sub&Function"
        private void IntialPSC2414()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2414";
            QueryStringList = new Dictionary<string, string>();
            QueryStringList.Add("_roleid", Request.QueryString["_roleid"]);
            
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

        public ActionResult GetLocation(string itemcode, string heatno)
        {
            string lang = string.Empty;
            try
            {
                itemcode = itemcode.Trim();
                heatno = heatno.Trim();
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                if (langCookie != null)
                {
                    lang = langCookie.Value;
                }
                else
                {
                    lang = "En";
                }

                string iString = "2019-04-01";
                DateTime dateStock = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

                // check language is Thai
                if (Request.Cookies["PSCS_culture"].Value.Equals("Th") && dateStock != null)
                {
                    // convert Buddhist era B.E.(+543) to A.D.(2019)
                    dateStock = dateStock.AddYears(543);
                }

                // Initial service
                //StockListService objStockListService = new StockListService(this.dbEntities);
                //List<StockList> obkStockList = objStockListService.GetStockListRelease(dateStock, itemcode, heatno);

                PipeItemService objPipeService = new PipeItemService(this.dbEntities);
                PipeItem objPipeItem = objPipeService.GetPipeItem(itemcode, heatno);
                List<LocationPatial> result = null;

                if (objPipeItem != null)
                {
                    StockListService objStockListService = new StockListService(this.dbEntities);
                    List<StockList> obkStockList = objStockListService.GetStockListRelease(dateStock,
                                                                                           objPipeItem.HeatNo,
                                                                                           objPipeItem.OD == null ? 0 : Convert.ToDecimal(objPipeItem.OD),
                                                                                           objPipeItem.WT == null ? 0 : Convert.ToDecimal(objPipeItem.WT));

                    LocationPatial objLocationPatial = null;
                    if (obkStockList != null)
                    {
                        foreach (StockList en in obkStockList)
                        {
                            if (en.Qty > 0)
                            {
                                if (result == null)
                                {
                                    result = new List<LocationPatial>();
                                }
                                objLocationPatial = new LocationPatial();
                                objLocationPatial.RowNo = result.Count() + 1;
                                objLocationPatial.YardName = en.YardName;
                                objLocationPatial.Location = en.LocationCode;
                                objLocationPatial.LocationName = en.LocationName;
                                objLocationPatial.Description = en.Description;
                                objLocationPatial.Qty = en.Qty;
                                objLocationPatial.ReceiveDateText = en.ReceiveDateText;
                                result.Add(objLocationPatial);
                            }
                        }
                    }
                }

                ReleaseSelectLocationPatialScreen patailModel = new ReleaseSelectLocationPatialScreen();
                patailModel.DataList = result;

                return PartialView("_PSC2414Partial", patailModel);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult SetParentGrid(string Selected)
        {
            try
            {
                if (!string.IsNullOrEmpty(Selected))
                {

                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(this.model, JsonRequestBehavior.AllowGet);
        }

        private List<ReleaseYardDetail> GetReleaseDetailByStatus(Boolean pIsQtyRemaining, DateTime pReleaseDate, Constants.ReleaseDetailStatus pStatus)
        {
            List<ReleaseYardDetail> result = null;
            List<ReleaseDetail> objReleaseDetail = null;
            ReleaseYardDetail objNew = null;

            try
            {
                if (pIsQtyRemaining)
                {
                    ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                    objReleaseDetail = objReleaseDetailService.GetQtyRemainingReleaseDetailList(pStatus);
                }
                else
                {
                    ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                    objReleaseDetail = objReleaseDetailService.GetReleaseDetailList(pReleaseDate, pStatus);
                }

                if (objReleaseDetail != null)
                {
                    foreach (ReleaseDetail en in objReleaseDetail)
                    {
                        string strLocationList = string.Empty;

                        if (result == null)
                        {
                            result = new List<ReleaseYardDetail>();
                        }

                        ReleaseLocationDetailService objReleaseLocationDetailService = new ReleaseLocationDetailService(this.dbEntities);
                        List<ReleaseLocationDetail> objReleaseLocationDetailList = objReleaseLocationDetailService.GetReleaseLocationDetailList(en.ReleaseId);
                        if (objReleaseLocationDetailList != null)
                        {
                            foreach (ReleaseLocationDetail enLocationCodeTemp in objReleaseLocationDetailList)
                            {
                                if (strLocationList == string.Empty)
                                {
                                    strLocationList = enLocationCodeTemp.LocationCode;
                                }
                                else
                                {
                                    strLocationList = strLocationList + ",\n" + enLocationCodeTemp.LocationCode;
                                }
                            }
                        }

                        objNew = new ReleaseYardDetail();
                        objNew.RowNo = result.Count() + 1;
                        objNew.ReleaseId = en.ReleaseId;
                        objNew.ItemCode = en.ItemCode;
                        objNew.HeatNo = en.HeatNo;
                        objNew.JobNo = en.JobNo;
                        objNew.MfgNo = en.MfgNo;
                        objNew.RequestDate = en.RequestDate;
                        objNew.RequestNo = en.RequestNo;
                        objNew.Description = en.Description;
                        objNew.Maker = en.Maker;
                        objNew.Maker_Name = en.Maker_Name;
                        objNew.Grade = en.Grade;
                        objNew.Grade_Name = en.Grade_Name;
                        objNew.YardID = "";
                        objNew.YardName = "";
                        objNew.LocationCode = "";
                        objNew.LocationName = "";
                        objNew.RequestQTY = en.RequestQTY;
                        objNew.ActualQTY = en.ActualQTY;
                        objNew.RemainQTY = (en.RequestQTY == null ? 0 : Convert.ToDecimal(en.RequestQTY)) - (en.ActualQTY == null ? 0 : Convert.ToDecimal(en.ActualQTY));
                        objNew.ChangeReleaseQty = en.ActualQTY;
                        objNew.LocationCodeList = strLocationList;
                        objNew.Status = en.Status;
                        objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                        objNew.Yard1Remark = en.Yard1Remark;
                        objNew.Yard2Remark = en.Yard2Remark;
                        objNew.CuttingRemark = en.CuttingRemark;
                        result.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        
        private string GetStatusName(byte? pStatus, string lng)
        {
            string result = string.Empty;
            if (pStatus == 1)
            {
                result = Constants.ReleaseDetailStatus.Request.ToString();
                result = Resources.Common_cshtml.Request;
            }
            else if (pStatus == 2)
            {
                result = Constants.ReleaseDetailStatus.Release.ToString();
                result = Resources.Common_cshtml.Release;
            }
            else if (pStatus == 3)
            {
                result = Constants.ReleaseDetailStatus.Release.ToString();
                result = Resources.Common_cshtml.Approve;
            }
            else
            {
                result = "";
            }

            return result;
        }

        

        #endregion

        #region "PSC2414 Release Daily Pipe"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2414()
        {
            List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                IntialPSC2414();

                this.model.FilterRequestDate = DateTime.Now;
                this.model.FilterYardID = "";
                this.model.FilterQtyRemaining = true;
                this.model.YardList = GetYard();

                objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                if (objReleaseYardDetailList == null)
                {
                    objReleaseYardDetailList = new List<ReleaseYardDetail>();
                }

                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
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
        public ActionResult PSC2414(ReleaseSelectLocationScreen pModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;
                List<ReleaseYardDetail> objReleaseYardDetailList = null;

                try
                {
                    switch (submitButton)
                    {
                        case "Back":
                            return RedirectToAction("PSC0100", "Menu");

                        case "ClearFilter":
                            ModelState.SetModelValue("FilterReleaseDate", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterYardID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            pModel.FilterRequestDate = DateTime.Now;
                            pModel.FilterYardID = "";
                            this.model.FilterRequestDate = DateTime.Now;
                            this.model.FilterYardID = "";
                            this.model.FilterQtyRemaining = false;

                            ModelState.SetModelValue("ReleaseYardDetailListDisplay", new ValueProviderResult(null, null, CultureInfo.InvariantCulture));
                            this.model.ReleaseYardDetailListDisplay = null;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = objReleaseYardDetailList.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }

                            return View(this.model);

                        case "Save":
                            Boolean result = false;
                            string message = string.Empty;
                            ReleaseYardDetail objReleaseYardDetail = null;
                            List<string> LocationCodeList = null;
                            string[] strList = null;
                            decimal decReleaseId = 0;
                            string strYard1Remark = string.Empty;
                            string strYard2Remark = string.Empty;
                            string strCuttingRemark = string.Empty;
                            ReleaseLocationDetailService objReleaseLocationDetailService = null;
                            ReleaseDetailService objUpReleaseDetail = null;

                            if (pModel.ReleaseYardDetailListDisplay != null && this.model.ReleaseYardDetailListDisplay != null)
                            {
                                for (int index = 0; index < pModel.ReleaseYardDetailListDisplay.Count; index++)
                                {
                                    objReleaseYardDetail = pModel.ReleaseYardDetailListDisplay[index];

                                    //if(objReleaseYardDetail.IsChange == 1)
                                    //{

                                    //}

                                    LocationCodeList = new List<string>();
                                    if (objReleaseYardDetail.LocationCodeList != null)
                                    {
                                        if (objReleaseYardDetail.LocationCodeList != string.Empty)
                                        {
                                            objReleaseYardDetail.LocationCodeList = objReleaseYardDetail.LocationCodeList.Replace("\r", "");
                                            objReleaseYardDetail.LocationCodeList = objReleaseYardDetail.LocationCodeList.Replace("\n", "");
                                            strList = objReleaseYardDetail.LocationCodeList.Split(',');
                                            if (strList != null)
                                            {
                                                foreach (string strTemp in strList)
                                                {
                                                    LocationCodeList.Add(strTemp);
                                                }
                                            }
                                        }
                                    }

                                    decReleaseId = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].ReleaseId : 0;
                                    strYard1Remark = objReleaseYardDetail.Yard1Remark;
                                    strYard2Remark = objReleaseYardDetail.Yard2Remark;
                                    strCuttingRemark = objReleaseYardDetail.CuttingRemark;

                                    objReleaseLocationDetailService = new ReleaseLocationDetailService(this.dbEntities);
                                    result = objReleaseLocationDetailService.SaveData(decReleaseId, LocationCodeList, this.LoginUser.UserId);

                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateYard1Remark(decReleaseId, strYard1Remark, this.LoginUser.UserId);
                                    }

                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateYard2Remark(decReleaseId, strYard2Remark, this.LoginUser.UserId);
                                    }

                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateCuttingRemark(decReleaseId, strCuttingRemark, this.LoginUser.UserId);
                                    }

                                }
                            }

                            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            message = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                            this.model.Message = message;

                            this.model.FilterRequestDate = pModel.FilterRequestDate;
                            this.model.FilterYardID = pModel.FilterYardID;
                            this.model.FilterQtyRemaining = pModel.FilterQtyRemaining;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }
                            return View(this.model);

                        default:
                            this.model.FilterRequestDate = pModel.FilterRequestDate;
                            this.model.FilterYardID = pModel.FilterYardID;
                            this.model.FilterQtyRemaining = pModel.FilterQtyRemaining;

                            ModelState.Clear();
                            //ModelState.SetModelValue("ReleaseYardDetailListDisplay", new ValueProviderResult(null, null, CultureInfo.InvariantCulture));
                            //this.model.ReleaseYardDetailListDisplay = null;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }

                            return View(this.model);

                        case "Edit":
                            return RedirectToAction("PSC2414_Edit", "ReleaseEdit");

                        case "Close":
                            return RedirectToAction("PSC2414_Close", "ReleaseEdit");
                        case "Delete":
                            return RedirectToAction("PSC2414_Delete", "ReleaseEdit");
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

        #region "PSC2414 Edit"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2414_Edit()
        {
            List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                IntialPSC2414();
                this.model.FilterRequestDate = DateTime.Now;
                this.model.FilterYardID = "";
                this.model.FilterQtyRemaining = true;
                this.model.YardList = GetYard();
                this.model.FilterJobs = ""; // กำหนดค่าเริ่มต้นสำหรับ FilterJobs

                // Temporarily assign "x" to FilterJobs if it is " "
                string originalFilterJobs = this.model.FilterJobs;
                if (this.model.FilterJobs == "")
                {
                    this.model.FilterJobs = "$";
                }

                // Perform the search
                objReleaseYardDetailList = Search(this.model);

                // Restore the original FilterJobs value
                this.model.FilterJobs = originalFilterJobs;

                if (objReleaseYardDetailList == null)
                {
                    objReleaseYardDetailList = new List<ReleaseYardDetail>();
                }

                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
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
        public ActionResult PSC2414_Edit(ReleaseSelectLocationScreen pModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;
                List<ReleaseYardDetail> objReleaseYardDetailList = null;

                try
                {
                    switch (submitButton)
                    {
                        case "Back":
                            return RedirectToAction("PSC2414", "ReleaseEdit");

                        case "Filter":
                            //this.model.FilterRequestDate = pModel.FilterRequestDate;
                            //this.model.FilterYardID = pModel.FilterYardID;
                            //this.model.FilterJobs = pModel.FilterJobs;
                            Filter_Onclick(pModel);
                            return View(this.model);
                        case "ClearFilter":
                            //ModelState.SetModelValue("FilterReleaseDate", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            //ModelState.SetModelValue("FilterYardID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            //ModelState.SetModelValue("FilterJobs", new ValueProviderResult("x", "x", CultureInfo.InvariantCulture));
                            //pModel.FilterRequestDate = DateTime.Now;
                            //pModel.FilterYardID = "";
                            this.model.FilterRequestDate = DateTime.Now;
                            this.model.FilterYardID = "";
                            this.model.FilterQtyRemaining = true;
                            //pModel.FilterJobs = "x";
                            this.model.FilterJobs = "x";

                            if (!string.IsNullOrWhiteSpace(this.model.FilterJobs))
                            {
                                objReleaseYardDetailList = Search(this.model);
                            }
                            else
                            {
                                objReleaseYardDetailList = GetReleaseDetailByStatus1(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request & Constants.ReleaseDetailStatus.Release);
                            }

                            if (objReleaseYardDetailList == null)
                            {
                                objReleaseYardDetailList = new List<ReleaseYardDetail>();
                            }

                            this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                            this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();

                            return View(this.model);

                        case "Save":
                            Boolean result = false;
                            string message = string.Empty;
                            ReleaseYardDetail objReleaseYardDetail = null;
                            List<string> LocationCodeList = null;
                            string[] strList = null;
                            decimal decReleaseId = 0;
                            string pJobNo = string.Empty;
                            decimal pChangeQty = 0;
                            string strYard1Remark = string.Empty;
                            string strYard2Remark = string.Empty;
                            string strCuttingRemark = string.Empty;
                            string pItemCode = string.Empty;
                            string pHeat = string.Empty;
                            string pMfgNo = string.Empty;
                            decimal pHistory = 0;
                            //ReleaseLocationDetailService objReleaseLocationDetailService = null;
                            ReleaseDetailService objUpReleaseDetail = null;

                            if (pModel.ReleaseYardDetailListDisplay != null && this.model.ReleaseYardDetailListDisplay != null)
                            {
                                for (int index = 0; index < pModel.ReleaseYardDetailListDisplay.Count; index++)
                                {
                                    objReleaseYardDetail = pModel.ReleaseYardDetailListDisplay[index];

                                    //if(objReleaseYardDetail.IsChange == 1)
                                    //{

                                    //}

                                    LocationCodeList = new List<string>();
                                    if (objReleaseYardDetail.LocationCodeList != null)
                                    {
                                        if (objReleaseYardDetail.LocationCodeList != string.Empty)
                                        {
                                            objReleaseYardDetail.LocationCodeList = objReleaseYardDetail.LocationCodeList.Replace("\r", "");
                                            objReleaseYardDetail.LocationCodeList = objReleaseYardDetail.LocationCodeList.Replace("\n", "");
                                            strList = objReleaseYardDetail.LocationCodeList.Split(',');
                                            if (strList != null)
                                            {
                                                foreach (string strTemp in strList)
                                                {
                                                    LocationCodeList.Add(strTemp);
                                                }
                                            }
                                        }
                                    }

                                    decReleaseId = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].ReleaseId : 0;
                                    pJobNo = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].JobNo : string.Empty;
                                    pChangeQty = objReleaseYardDetail.ChangeReleaseQty ?? 0;
                                    pHistory = objReleaseYardDetail.RequestQTY ?? 0;
                                    pMfgNo = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].MfgNo : string.Empty;
                                    pHeat = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].HeatNo : string.Empty;
                                    pItemCode = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].ItemCode : string.Empty;
                                    objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                    result = objUpReleaseDetail.UpdateChangeReleaseQty(pJobNo, pChangeQty, this.LoginUser.UserId);

                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateActualRelease(decReleaseId, this.LoginUser.UserId);
                                    }
                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateChangeReleaseQty1(pJobNo, pChangeQty, this.LoginUser.UserId);
                                    }

                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateHistoryChange(pJobNo, decReleaseId, pItemCode, pHeat, pMfgNo, pHistory, pChangeQty, this.LoginUser.UserId);
                                    }
                                }
                            }


                            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            message = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                            this.model.Message = message;

                            IntialPSC2414();
                            this.model.FilterRequestDate = DateTime.Now;
                            this.model.FilterYardID = "";
                            this.model.FilterQtyRemaining = true;
                            this.model.YardList = GetYard();
                            this.model.FilterJobs = ""; // กำหนดค่าเริ่มต้นสำหรับ FilterJobs

                            // Temporarily assign "x" to FilterJobs if it is " "
                            string originalFilterJobs = this.model.FilterJobs;
                            if (this.model.FilterJobs == "")
                            {
                                this.model.FilterJobs = "$";
                            }

                            // Perform the search
                            objReleaseYardDetailList = Search(this.model);

                            // Restore the original FilterJobs value
                            this.model.FilterJobs = originalFilterJobs;

                            if (objReleaseYardDetailList == null)
                            {
                                objReleaseYardDetailList = new List<ReleaseYardDetail>();
                            }

                            this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                            this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();

                            return View(this.model);


                        default:
                            this.model.FilterRequestDate = pModel.FilterRequestDate;
                            this.model.FilterYardID = pModel.FilterYardID;
                            this.model.FilterQtyRemaining = pModel.FilterQtyRemaining;

                            ModelState.Clear();
                            //ModelState.SetModelValue("ReleaseYardDetailListDisplay", new ValueProviderResult(null, null, CultureInfo.InvariantCulture));
                            //this.model.ReleaseYardDetailListDisplay = null;

                            objReleaseYardDetailList = GetReleaseDetailByStatus1(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request & Constants.ReleaseDetailStatus.Release);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }

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
        private List<ReleaseYardDetail> GetReleaseDetailByStatus1(Boolean pIsQtyRemaining, DateTime pReleaseDate, Constants.ReleaseDetailStatus pStatus)
        {
            List<ReleaseYardDetail> result = null;
            List<ReleaseDetail> objReleaseDetail = null;
            ReleaseYardDetail objNew = null;

            try
            {
                if (pIsQtyRemaining)
                {
                    ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                    objReleaseDetail = objReleaseDetailService.GetQtyRemainingReleaseDetailList1(pStatus);
                }
                else
                {
                    ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                    objReleaseDetail = objReleaseDetailService.GetReleaseDetailList(pReleaseDate, pStatus);
                }

                if (objReleaseDetail != null)
                {
                    foreach (ReleaseDetail en in objReleaseDetail)
                    {
                        string strLocationList = string.Empty;

                        if (result == null)
                        {
                            result = new List<ReleaseYardDetail>();
                        }

                        ReleaseLocationDetailService objReleaseLocationDetailService = new ReleaseLocationDetailService(this.dbEntities);
                        List<ReleaseLocationDetail> objReleaseLocationDetailList = objReleaseLocationDetailService.GetReleaseLocationDetailList(en.ReleaseId);
                        if (objReleaseLocationDetailList != null)
                        {
                            foreach (ReleaseLocationDetail enLocationCodeTemp in objReleaseLocationDetailList)
                            {
                                if (strLocationList == string.Empty)
                                {
                                    strLocationList = enLocationCodeTemp.LocationCode;
                                }
                                else
                                {
                                    strLocationList = strLocationList + ",\n" + enLocationCodeTemp.LocationCode;
                                }
                            }
                        }

                        objNew = new ReleaseYardDetail();
                        objNew.RowNo = result.Count() + 1;
                        objNew.ReleaseId = en.ReleaseId;
                        objNew.ItemCode = en.ItemCode;
                        objNew.HeatNo = en.HeatNo;
                        objNew.JobNo = en.JobNo;
                        objNew.MfgNo = en.MfgNo;
                        objNew.RequestDate = en.RequestDate;
                        objNew.RequestNo = en.RequestNo;
                        objNew.Description = en.Description;
                        objNew.Maker = en.Maker;
                        objNew.Maker_Name = en.Maker_Name;
                        objNew.Grade = en.Grade;
                        objNew.Grade_Name = en.Grade_Name;
                        objNew.YardID = "";
                        objNew.YardName = "";
                        objNew.LocationCode = "";
                        objNew.LocationName = "";
                        objNew.RequestQTY = en.RequestQTY;
                        objNew.ActualQTY = en.ActualQTY;
                        objNew.ProductName = en.ProductName;
                        objNew.RemainQTY = (en.RequestQTY == null ? 0 : Convert.ToDecimal(en.RequestQTY)) - (en.ActualQTY == null ? 0 : Convert.ToDecimal(en.ActualQTY));
                        objNew.ChangeReleaseQty = en.RequestQTY;
                        objNew.LocationCodeList = strLocationList;
                        objNew.Status = en.Status;
                        objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                        objNew.Yard1Remark = en.Yard1Remark;
                        objNew.Yard2Remark = en.Yard2Remark;
                        objNew.CuttingRemark = en.CuttingRemark;
                        result.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        private List<ReleaseYardDetail> GetReleaseSearch(string pJobs)
        {
            List<ReleaseYardDetail> result = new List<ReleaseYardDetail>();
            try
            {
                ReleaseDetailService objReleaseYardDetailService = new ReleaseDetailService(this.dbEntities);
                HttpCookie laCookie = Request.Cookies["PSCS_culture"];
                string pLanguage = laCookie != null ? laCookie.Value : "En";

                result = objReleaseYardDetailService.GetReleaseFilter(pJobs);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private List<ReleaseYardDetail> Search(ReleaseSelectLocationScreen FilterModel)
        {
            List<ReleaseYardDetail> result = null;
            try
            {
                result = GetReleaseSearch(FilterModel.FilterJobs);

                if (FilterModel.FilterJobs != string.Empty)
                {
                    List<ReleaseYardDetail> objReleaseList = GetReleasesSearch(FilterModel.FilterJobs);
                    List<ReleaseYardDetail> objReleaseDisticnt = (objReleaseList.GroupBy(r => new { r.JobNo })
                                        .Select((m, index) => new ReleaseYardDetail
                                        {
                                            JobNo = m.Key.JobNo,
                                        })).ToList();
                    if (objReleaseDisticnt == null)
                    {
                        result = new List<ReleaseYardDetail>();
                    }
                    else
                    {
                        List<ReleaseYardDetail> objReleaseDetailList = new List<ReleaseYardDetail>();
                        foreach (ReleaseYardDetail en in objReleaseDisticnt)
                        {
                            foreach (ReleaseYardDetail enReleaseDetail in result)
                            {
                                if (en.JobNo == enReleaseDetail.JobNo)
                                {
                                    objReleaseDetailList.Add(enReleaseDetail);
                                }
                            }
                        }
                        result = objReleaseDetailList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private List<ReleaseYardDetail> GetReleasesSearch(string pJobs)
        {
            List<ReleaseYardDetail> result = new List<ReleaseYardDetail>();
            try
            {
                ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                string pLanguage = langCookie != null ? langCookie.Value : "En";

                result = objReleaseService.GetJobsFilter(pJobs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        [HttpPost]
        public ActionResult Filter_Onclick(ReleaseSelectLocationScreen FilterModel)
        {
            List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {
                // Ensure that FilterJobs is set correctly
                if (string.IsNullOrWhiteSpace(FilterModel.FilterJobs))
                {
                    // Handle the case where the FilterJobs is empty or null
                    this.model.ReleaseYardDetailListDisplay = new List<ReleaseYardDetail>();
                }
                else
                {
                    this.model.ReleaseYardDetailListDisplay = Search(FilterModel);
                }

                this.model.FilterJobs = FilterModel.FilterJobs;
                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("PSC2414_Edit", this.model);
        }

        #endregion

        #region "PSC2414 Close"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2414_Close()
        {
            List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {

                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;
                this.model.SelectStatus = GetStatusDropdown();
                // Initial View
                IntialPSC2414();

                this.model.FilterRequestDate = DateTime.Now;
                this.model.FilterYardID = "";
                this.model.FilterQtyRemaining = true;
                this.model.YardList = GetYard();


                objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                if (objReleaseYardDetailList == null)
                {
                    objReleaseYardDetailList = new List<ReleaseYardDetail>();
                }

                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
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
        public ActionResult PSC2414_Close(ReleaseSelectLocationScreen pModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                List<ReleaseYardDetail> objReleaseYardDetailList = null;


                try
                {
                    switch (submitButton)
                    {
                        case "Back":
                            return RedirectToAction("PSC2414", "ReleaseEdit");

                        case "ClearFilter":
                            ModelState.SetModelValue("FilterReleaseDate", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterYardID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            pModel.FilterRequestDate = DateTime.Now;
                            pModel.FilterYardID = "";
                            this.model.FilterRequestDate = DateTime.Now;
                            this.model.FilterYardID = "";
                            this.model.FilterQtyRemaining = false;

                            ModelState.SetModelValue("ReleaseYardDetailListDisplay", new ValueProviderResult(null, null, CultureInfo.InvariantCulture));
                            this.model.ReleaseYardDetailListDisplay = null;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = objReleaseYardDetailList.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }

                            return View(this.model);

                        case "Save":
                            Boolean result = false;
                            string message = string.Empty;
                            ReleaseYardDetail objReleaseYardDetail = null;
                            List<string> LocationCodeList = null;
                            string[] strList = null;
                            decimal decReleaseId = 0;
                            string strYard1Remark = string.Empty;
                            string strYard2Remark = string.Empty;
                            string strCuttingRemark = string.Empty;
                            byte pStatus = 0;
                            ReleaseLocationDetailService objReleaseLocationDetailService = null;
                            ReleaseDetailService objUpReleaseDetail = null;

                            if (pModel.ReleaseYardDetailListDisplay != null && this.model.ReleaseYardDetailListDisplay != null)
                            {
                                for (int index = 0; index < pModel.ReleaseYardDetailListDisplay.Count; index++)
                                {
                                    objReleaseYardDetail = pModel.ReleaseYardDetailListDisplay[index];

                                    //if(objReleaseYardDetail.IsChange == 1)
                                    //{

                                    //}

                                    LocationCodeList = new List<string>();
                                    if (objReleaseYardDetail.LocationCodeList != null)
                                    {
                                        if (objReleaseYardDetail.LocationCodeList != string.Empty)
                                        {
                                            objReleaseYardDetail.LocationCodeList = objReleaseYardDetail.LocationCodeList.Replace("\r", "");
                                            objReleaseYardDetail.LocationCodeList = objReleaseYardDetail.LocationCodeList.Replace("\n", "");
                                            strList = objReleaseYardDetail.LocationCodeList.Split(',');
                                            if (strList != null)
                                            {
                                                foreach (string strTemp in strList)
                                                {
                                                    LocationCodeList.Add(strTemp);
                                                }
                                            }
                                        }
                                    }

                                    decReleaseId = index < this.model.ReleaseYardDetailListDisplay.Count ? this.model.ReleaseYardDetailListDisplay[index].ReleaseId : 0;
                                    strYard1Remark = objReleaseYardDetail.Yard1Remark;
                                    strYard2Remark = objReleaseYardDetail.Yard2Remark;
                                    strCuttingRemark = objReleaseYardDetail.CuttingRemark;
                                    pStatus = (Byte)objReleaseYardDetail.Status;

                                    objReleaseLocationDetailService = new ReleaseLocationDetailService(this.dbEntities);
                                    result = objReleaseLocationDetailService.SaveData(decReleaseId, LocationCodeList, this.LoginUser.UserId);

                                    if (result)
                                    {
                                        objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                                        result = objUpReleaseDetail.UpdateChangeReleaseStatus(decReleaseId, pStatus, this.LoginUser.UserId);
                                    }
                                }
                            }

                            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            message = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                            this.model.Message = message;

                            this.model.FilterRequestDate = pModel.FilterRequestDate;
                            this.model.FilterYardID = pModel.FilterYardID;
                            this.model.FilterQtyRemaining = pModel.FilterQtyRemaining;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }
                            return View(this.model);

                        default:
                            this.model.FilterRequestDate = pModel.FilterRequestDate;
                            this.model.FilterYardID = pModel.FilterYardID;
                            this.model.FilterQtyRemaining = pModel.FilterQtyRemaining;

                            ModelState.Clear();
                            //ModelState.SetModelValue("ReleaseYardDetailListDisplay", new ValueProviderResult(null, null, CultureInfo.InvariantCulture));
                            //this.model.ReleaseYardDetailListDisplay = null;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if (objReleaseYardDetailList != null)
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
                            }
                            else
                            {
                                this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                                this.model.Total = "0";
                            }

                            return View(this.model);

                        case "Edit":
                            return RedirectToAction("PSC2414_Edit", "ReleaseEdit");
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
        public List<SelectListItem> GetStatusDropdown()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem
            {
                Value = "1",
                Text = "Request"
            });

            // Add "Close" option
            result.Add(new SelectListItem
            {
                Value = "0",
                Text = "Close"
            });

            return result;
        }

        #endregion

        #region PSC2415 Delete
        private Release GetRelease(string pJobNo)
        {
            Release result = null;
            List<ReleaseYardDetail> objJobMasterDataList = null;

            try
            {
                //PSCS RELEASE TABLE
                ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                result = objReleaseService.GetReleaseData1(pJobNo);

                if (result == null)
                {
                    ReleaseDetailService objGetWIFromSyteLineService = new ReleaseDetailService(this.dbEntities);
                    objJobMasterDataList = objGetWIFromSyteLineService.GetSearchJobs(pJobNo);

                    if (objJobMasterDataList != null)
                    {
                        if (objJobMasterDataList.Count > 0)
                        {
                            //PSCS RELEASE TABLE
                            ReleaseService objRelease1Service = new ReleaseService(this.dbEntities);
                            result = objRelease1Service.GetReleaseData1(pJobNo);
                            
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
            List<ReleaseYardDetail> objJobMasterDataList = null;

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
                        objRelease = objReleaseService.GetReleaseData1(pWIBarcode);
                        if (objRelease == null)
                        {
                            ReleaseDetailService objGetWIFromSyteLineService = new ReleaseDetailService(this.dbEntities);
                            objJobMasterDataList = objGetWIFromSyteLineService.GetSearchJobs(pWIBarcode);

                            if (objJobMasterDataList != null)
                            {
                                if (objJobMasterDataList.Count > 0)
                                {
                                        ReleaseService objRelease1Service = new ReleaseService(this.dbEntities);
                                        objRelease = objRelease1Service.GetReleaseData1(pWIBarcode);
                                }
                            }
                        }

                        if (objRelease != null)
                        {
                            decimal decSumRequestQTY = 0;
                            RequestService objRequestService = new RequestService(this.dbEntities);
                            List<Request> objRequestList = objRequestService.GetRequestsList1(objRelease.JobNo);

                            if (objRequestList != null)
                            {
                                foreach (Request en in objRequestList)
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

                                if (decSumRequestQTY == 0)
                                {
                                    decRereaseQTY = Convert.ToDecimal(objRelease.QTY);
                                    decRequestQTY = decRereaseQTY;
                                    decRemainQTY = 0;
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
                                objRequest.ReleaseId = objRelease.ReleaseID;

                                IsInsert = objRequestService.Insert1(objRequest, this.LoginUser.UserId);
                                if (IsInsert)
                                {
                                    RequestService objGetRequestService = new RequestService(this.dbEntities);
                                    this.model.WIRequestList = objGetRequestService.GetRequestsList1(Constants.RequestStatus.Draft);
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

        private ActionResult Delete_OnClick(List<decimal> pDelId)
        {
            Boolean result = false;
            RequestService objRequestService = null;
            string message = string.Empty;

            try
            {
                if (pDelId != null)
                {
                    foreach (decimal pDel in pDelId)
                    {
                        objRequestService = new RequestService(this.dbEntities);
                        result = objRequestService.Delete1(pDel);
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
                        this.model.WIRequestList = objRequestService.GetRequestsList1(Constants.RequestStatus.Draft);
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

        private ActionResult Request_OnClick(List<Request> pRequestList, string pUserId)
        {
            bool result = false;
            //bool result1 = false;

            try
            {
                RequestService objRequestService = new RequestService(this.dbEntities);
                result = objRequestService.UpdateData1(pRequestList, pUserId);
                //result1 = objRequestService.UpdateData2(pRequestList, pUserId);
                if (result)
                {
                    return RedirectToAction("PSC2414", "ReleaseEdit");
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Warning;
                    this.model.Message = PSCS.Resources.PSC2414_cshtml.ErrorJob;
                    this.PrintError(PSCS.Resources.PSC2414_cshtml.ErrorJob);
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


        #endregion

        #region "PSC2414Delete"
        // GET: Release
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2414_Delete()
        {
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                IntialPSC2414();
                if (!this.model.HasRequest)
                {
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;
                    this.model.WIRequestList = new List<Request>();
                }

                RequestService objRequestService = new RequestService(this.dbEntities);
                this.model.WIRequestList = objRequestService.GetRequestsList1(Constants.RequestStatus.Draft);
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
        public ActionResult PSC2414_Delete(ReleaseScreen pModel, string submitButton)
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
                            return RedirectToAction("PSC2414", "ReleaseEdit");

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

                            return Delete_OnClick(objRequestIdList);

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