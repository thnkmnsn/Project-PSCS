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

namespace PSCS.Controllers
{
    public class ReleaseSelectLocationController : BaseController
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
        private void IntialPSC2411()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2411";
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
                        foreach(StockList en in obkStockList)
                        {
                            if(en.Qty > 0)
                            {
                                if(result == null)
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

                return PartialView("_PSC2411Partial", patailModel);
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
                if(pIsQtyRemaining)
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
                        if(objReleaseLocationDetailList != null)
                        {
                            foreach(ReleaseLocationDetail enLocationCodeTemp in objReleaseLocationDetailList)
                            {
                                if(strLocationList == string.Empty)
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
        
        //private List<ReleaseYardDetail> FilterData(DateTime? pReleaseDate, string pYard)
        //{
        //    List<ReleaseYardDetail> result = null;
        //    DateTime dateRelease = DateTime.Now;
            
        //    try
        //    {
        //        if (pReleaseDate != null)
        //        {
        //            dateRelease = Convert.ToDateTime(pReleaseDate);
        //        }

        //        List<ReleaseYardDetail> objReleaseYardDetailList =  GetReleaseDetailByStatus(Constants.ReleaseDetailStatus.Request);

        //        if(objReleaseYardDetailList != null)
        //        {
        //            result = (from rd in objReleaseYardDetailList
        //                                              where (pReleaseDate == null ||
        //                                              (rd.RequestDate.Year == dateRelease.Year && rd.RequestDate.Month == dateRelease.Month && rd.RequestDate.Day == dateRelease.Day))
        //                                              && (string.IsNullOrEmpty(pYard) || rd.YardID == pYard)
        //                                              && rd.LocationCode != Common.Constants.LocationCodeRelease
        //                                              orderby rd.YardID ascending
        //                                              select new
        //                                              {
        //                                                  rd.YardID,
        //                                                  rd.YardName,
        //                                                  rd.ReleaseId,
        //                                                  rd.ItemCode,
        //                                                  rd.JobNo,
        //                                                  rd.MfgNo,
        //                                                  rd.HeatNo,
        //                                                  rd.RequestNo,
        //                                                  rd.RequestDate,
        //                                                  rd.ReceiveDate,
        //                                                  rd.LocationCode,
        //                                                  rd.LocationName,
        //                                                  rd.RequestQTY,
        //                                                  rd.ActualQTY,
        //                                                  rd.Status,
        //                                                  rd.Status_Name,
        //                                                  rd.Remark,
        //                                                  rd.Description,
        //                                                  rd.Maker,
        //                                                  rd.Maker_Name,
        //                                                  rd.Grade,
        //                                                  rd.Grade_Name,
        //                                              }).AsEnumerable().Select((x, index) => new ReleaseYardDetail
        //                                              {
        //                                                  RowNo = index + 1,
        //                                                  YardID = x.YardID,
        //                                                  YardName = x.YardName,
        //                                                  ReleaseId = x.ReleaseId,
        //                                                  ItemCode = x.ItemCode,
        //                                                  HeatNo = x.HeatNo,
        //                                                  JobNo = x.JobNo,
        //                                                  MfgNo = x.MfgNo,
        //                                                  RequestNo = x.RequestNo,
        //                                                  RequestDate = x.RequestDate,
        //                                                  ReceiveDate = x.ReceiveDate,
        //                                                  LocationCode = x.LocationCode,
        //                                                  LocationName = x.LocationName,
        //                                                  RequestQTY = Math.Round(Convert.ToDecimal(x.RequestQTY), 2),
        //                                                  ActualQTY = Math.Round(Convert.ToDecimal(x.ActualQTY), 2),
        //                                                  Status = x.Status,
        //                                                  Status_Name = x.Status_Name,
        //                                                  Remark = x.Remark,
        //                                                  Description = x.Description,
        //                                                  Maker = x.Maker,
        //                                                  Maker_Name = x.Maker_Name,
        //                                                  Grade = x.Grade,
        //                                                  Grade_Name = x.Grade_Name
        //                                              }).ToList();

        //            if (result == null)
        //            {
        //                result = new List<ReleaseYardDetail>();
        //            }
        //            else
        //            {
        //                foreach (ReleaseYardDetail en in result)
        //                {
        //                    string strLocationList = string.Empty;

        //                    ReleaseLocationDetailService objReleaseLocationDetailService = new ReleaseLocationDetailService(this.dbEntities);
        //                    List<ReleaseLocationDetail> objReleaseLocationDetailList = objReleaseLocationDetailService.GetReleaseLocationDetailList(en.ReleaseId);
        //                    if (objReleaseLocationDetailList != null)
        //                    {
        //                        foreach (ReleaseLocationDetail enLocationCodeTemp in objReleaseLocationDetailList)
        //                        {
        //                            if (strLocationList == string.Empty)
        //                            {
        //                                strLocationList = enLocationCodeTemp.LocationCode;
        //                            }
        //                            else
        //                            {
        //                                strLocationList = strLocationList + "," + enLocationCodeTemp.LocationCode;
        //                            }

        //                        }
        //                    }

        //                    en.LocationCodeList = strLocationList;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        #endregion

        #region "PSC2411 Release Daily Pipe"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2411()
        {
            List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                IntialPSC2411();

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
        public ActionResult PSC2411(ReleaseSelectLocationScreen pModel, string submitButton)
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

                            ModelState.SetModelValue("ReleaseYardDetailListDisplay", new ValueProviderResult( null, null, CultureInfo.InvariantCulture));
                            this.model.ReleaseYardDetailListDisplay = null;

                            objReleaseYardDetailList = GetReleaseDetailByStatus(this.model.FilterQtyRemaining, this.model.FilterRequestDate, Constants.ReleaseDetailStatus.Request);
                            if(objReleaseYardDetailList != null)
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