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

namespace PSCS.Controllers
{
    [SessionExpire]
    public class ReleaseDailyPipeController : BaseController
    {
        public ReleaseDailyPipeScreen model
        {
            get
            {
                if (Session["ReleaseDailyPipeScreen"] == null)
                {
                    Session["ReleaseDailyPipeScreen"] = new ReleaseDailyPipeScreen();
                    return (ReleaseDailyPipeScreen)Session["ReleaseDailyPipeScreen"];
                }
                else
                {
                    return (ReleaseDailyPipeScreen)Session["ReleaseDailyPipeScreen"];
                }
            }
            set { Session["ReleaseDailyPipeScreen"] = value; }
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

        //private List<ReleaseYardDetail> CreateReleaseDetailEachYard(DateTime? pReleaseDate)
        //{
        //    List<ReleaseYardDetail> result = null;
        //    List<ReleaseDetail> objReleaseDetail = null;
        //    List<StockList> objStockListList = null;
        //    ReleaseYardDetail objNew = null;
        //    Boolean IsBreak = false;
        //    int intSUMQTY = 0;

        //    try
        //    {
        //        ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
        //        objReleaseDetail = objReleaseDetailService.GetReleaseDetailList(pReleaseDate);
        //        if(objReleaseDetail != null)
        //        {
        //            string iString = "2019-04-01";
        //            DateTime dateStock = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

        //            // check language is Thai
        //            if (Request.Cookies["PSCS_culture"].Value.Equals("Th") && dateStock != null)
        //            {
        //                // convert Buddhist era B.E.(+543) to A.D.(2019)
        //                dateStock = dateStock.AddYears(543);
        //            }

        //            foreach (ReleaseDetail en in objReleaseDetail)
        //            {
        //                intSUMQTY = Convert.ToInt32(en.RequestQTY);
        //                StockListService objStockListService = new StockListService(this.dbEntities);
        //                objStockListList = objStockListService.GetStockListRelease(dateStock, en.ItemCode, en.HeatNo);
        //                if(objStockListList != null)
        //                {
        //                    if(objStockListList.Count() != 0)
        //                    {
        //                        foreach (StockList enSt in objStockListList)
        //                        {
        //                            if (result == null)
        //                            {
        //                                result = new List<ReleaseYardDetail>();
        //                            }
        //                            objNew = new ReleaseYardDetail();
        //                            objNew.RowNo = result.Count() + 1;
        //                            objNew.ItemCode = en.ItemCode;
        //                            objNew.HeatNo = en.HeatNo;
        //                            objNew.JobNo = en.JobNo;
        //                            objNew.MfgNo = en.MfgNo;
        //                            objNew.RequestDate = en.RequestDate;
        //                            objNew.RequestNo = en.RequestNo;
        //                            objNew.Description = en.Description;
        //                            objNew.Maker = en.Maker;
        //                            objNew.Maker_Name = en.Maker_Name;
        //                            objNew.Grade = en.Grade;
        //                            objNew.Grade_Name = en.Grade_Name;
        //                            objNew.YardID = enSt.YardID;
        //                            objNew.YardName = enSt.YardName;
        //                            objNew.LocationCode = enSt.LocationCode;
        //                            objNew.LocationName = enSt.LocationName;
        //                            if(enSt.Qty >= intSUMQTY)
        //                            {
        //                                objNew.RequestQTY = intSUMQTY;
        //                                intSUMQTY = 0;
        //                                IsBreak = true;  
        //                            }
        //                            else
        //                            {
        //                                intSUMQTY = intSUMQTY -Convert.ToInt32(enSt.Qty);
        //                                objNew.RequestQTY = enSt.Qty;
        //                            }
        //                            objNew.Status = en.Status;
        //                            objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
        //                            result.Add(objNew);

        //                            if(IsBreak)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (result == null)
        //                        {
        //                            result = new List<ReleaseYardDetail>();
        //                        }
        //                        objNew = new ReleaseYardDetail();
        //                        objNew.RowNo = result.Count() + 1;
        //                        objNew.ItemCode = en.ItemCode;
        //                        objNew.HeatNo = en.HeatNo;
        //                        objNew.JobNo = en.JobNo;
        //                        objNew.MfgNo = en.MfgNo;
        //                        objNew.RequestDate = en.RequestDate;
        //                        objNew.RequestNo = en.RequestNo;
        //                        objNew.Description = en.Description;
        //                        objNew.Maker = en.Maker;
        //                        objNew.Maker_Name = en.Maker_Name;
        //                        objNew.Grade = en.Grade;
        //                        objNew.Grade_Name = en.Grade_Name;
        //                        objNew.YardID = "";
        //                        objNew.YardName = "";
        //                        objNew.LocationCode = "";
        //                        objNew.LocationName = "";
        //                        objNew.RequestQTY = 0;
        //                        objNew.Status = en.Status;
        //                        objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
        //                        result.Add(objNew);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        private List<ReleaseYardDetail> CreateReleaseDetailEachYard(DateTime pRequestDate)
        {
            List<ReleaseYardDetail> result = null;
            List<ReleaseDetail> objReleaseDetail = null;
            List<StockList> objStockListList = null;
            ReleaseYardDetail objNew = null;
            Boolean IsBreak = false;
            int intSUMQTY = 0;

            try
            {
                ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                objReleaseDetail = objReleaseDetailService.GetReleaseDetailList(pRequestDate, Constants.ReleaseDetailStatus.Request);
                if (objReleaseDetail != null)
                {
                    string iString = "2019-04-01";
                    DateTime dateStock = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

                    // check language is Thai
                    if (Request.Cookies["PSCS_culture"].Value.Equals("Th") && dateStock != null)
                    {
                        // convert Buddhist era B.E.(+543) to A.D.(2019)
                        dateStock = dateStock.AddYears(543);
                    }

                    foreach (ReleaseDetail en in objReleaseDetail)
                    {
                        intSUMQTY = Convert.ToInt32(en.RequestQTY);
                        IsBreak = false;

                        StockListService objStockListService = new StockListService(this.dbEntities);
                        objStockListList = objStockListService.GetStockListRelease(dateStock, en.ItemCode, en.HeatNo);
                        if (objStockListList != null)
                        {
                            if (objStockListList.Count() != 0)
                            {
                                foreach (StockList enSt in objStockListList)
                                {
                                    if (result == null)
                                    {
                                        result = new List<ReleaseYardDetail>();
                                    }
                                    objNew = new ReleaseYardDetail();
                                    objNew.RowNo = result.Count() + 1;
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
                                    objNew.YardID = enSt.YardID;
                                    objNew.YardName = enSt.YardName;
                                    objNew.LocationCode = enSt.LocationCode;
                                    objNew.LocationName = enSt.LocationName;
                                    if (enSt.Qty >= intSUMQTY)
                                    {
                                        objNew.RequestQTY = intSUMQTY;
                                        intSUMQTY = 0;
                                        IsBreak = true;
                                    }
                                    else
                                    {
                                        intSUMQTY = intSUMQTY - Convert.ToInt32(enSt.Qty);
                                        objNew.RequestQTY = enSt.Qty;
                                    }
                                    objNew.Status = en.Status;
                                    objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                                    result.Add(objNew);

                                    if (IsBreak)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (result == null)
                                {
                                    result = new List<ReleaseYardDetail>();
                                }
                                objNew = new ReleaseYardDetail();
                                objNew.RowNo = result.Count() + 1;
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
                                objNew.RequestQTY = 0;
                                objNew.Status = en.Status;
                                objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                                result.Add(objNew);
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

        //private string GetStatus(byte? pStatus)
        //{
        //    string result = string.Empty;
        //    if (pStatus == 1)
        //    {
        //        result = Constants.ReleaseDetailStatus.Request.ToString();
        //    }
        //    else if (pStatus == 2)
        //    {
        //        result = Constants.ReleaseDetailStatus.Release.ToString();
        //    }
        //    else if (pStatus == 3)
        //    {
        //        result = Constants.ReleaseDetailStatus.Approve.ToString();
        //    }
        //    else
        //    {
        //        result = "";
        //    }

        //    return result;
        //}

        
        private void FilterData(DateTime? pReleaseDate, string pYard)
        {
            DateTime dateRelease = DateTime.Now;

            try
            {
                if (pReleaseDate != null)
                {
                    dateRelease = Convert.ToDateTime(pReleaseDate);
                }

                List<ReleaseYardDetail> result = (from rd in this.model.ReleaseYardDetailList
                                                  where (pReleaseDate == null ||
                                                  (rd.RequestDate.Year == dateRelease.Year && rd.RequestDate.Month == dateRelease.Month && rd.RequestDate.Day == dateRelease.Day))
                                                  && (string.IsNullOrEmpty(pYard) || rd.YardID == pYard)
                                                  && rd.LocationCode != Common.Constants.LocationCodeRelease
                                                  orderby rd.YardID ascending
                                                  select new
                                                  {
                                                      rd.YardID,
                                                      rd.YardName,
                                                      rd.ReleaseId,
                                                      rd.ItemCode,
                                                      rd.JobNo,
                                                      rd.MfgNo,
                                                      rd.HeatNo,
                                                      rd.RequestNo,
                                                      rd.RequestDate,
                                                      rd.ReceiveDate,
                                                      rd.LocationCode,
                                                      rd.LocationName,
                                                      rd.RequestQTY,
                                                      rd.ActualQTY,
                                                      rd.Status,
                                                      rd.Status_Name,
                                                      rd.Yard1Remark,
                                                      rd.Yard2Remark,
                                                      rd.CuttingRemark,
                                                      rd.Description,
                                                      rd.Maker,
                                                      rd.Maker_Name,
                                                      rd.Grade,
                                                      rd.Grade_Name,
                                                  }).AsEnumerable().Select((x, index) => new ReleaseYardDetail
                                                  {
                                                      RowNo = index + 1,
                                                      YardID = x.YardID,
                                                      YardName = x.YardName,
                                                      ReleaseId = x.ReleaseId,
                                                      ItemCode = x.ItemCode,
                                                      HeatNo = x.HeatNo,
                                                      JobNo = x.JobNo,
                                                      MfgNo = x.MfgNo,
                                                      RequestNo = x.RequestNo,
                                                      RequestDate = x.RequestDate,
                                                      ReceiveDate = x.ReceiveDate,
                                                      LocationCode = x.LocationCode,
                                                      LocationName = x.LocationName,
                                                      RequestQTY = Math.Round(Convert.ToDecimal(x.RequestQTY), 2),
                                                      ActualQTY = Math.Round(Convert.ToDecimal(x.ActualQTY), 2),
                                                      Status = x.Status,
                                                      Status_Name = x.Status_Name,
                                                      Yard1Remark = x.Yard1Remark,
                                                      Yard2Remark = x.Yard2Remark,
                                                      CuttingRemark = x.CuttingRemark,
                                                      Description = x.Description,
                                                      Maker = x.Maker,
                                                      Maker_Name = x.Maker_Name,
                                                      Grade = x.Grade,
                                                      Grade_Name = x.Grade_Name
                                                  }).ToList();

                if (result == null)
                {
                    result = new List<ReleaseYardDetail>();
                }

                this.model.ReleaseYardDetailListDisplay = result;
                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


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
                this.model.YardList = GetYard();
                
                objReleaseYardDetailList =CreateReleaseDetailEachYard(this.model.FilterRequestDate);
                
                if (objReleaseYardDetailList ==null)
                {
                    objReleaseYardDetailList = new List<ReleaseYardDetail>();
                }
                this.model.ReleaseYardDetailList = objReleaseYardDetailList;
                this.model.ReleaseYardDetailListDisplay = this.model.ReleaseYardDetailList;
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
        public ActionResult PSC2411(ReleaseDailyPipeScreen pModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

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

                            FilterData(this.model.FilterRequestDate, pModel.FilterYardID);
                            return View(this.model);

                        default:
                            this.model.FilterRequestDate = pModel.FilterRequestDate;
                            this.model.FilterYardID = pModel.FilterYardID;

                            FilterData(pModel.FilterRequestDate, pModel.FilterYardID);
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