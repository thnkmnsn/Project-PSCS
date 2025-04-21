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
    public class ReleaseApproveController : BaseController
    {
        public ReleaseApproveScreen model
        {
            get
            {
                if (Session["ReleaseApproveScreen"] == null)
                {
                    Session["ReleaseApproveScreen"] = new ReleaseApproveScreen();
                    return (ReleaseApproveScreen)Session["ReleaseApproveScreen"];
                }
                else
                {
                    return (ReleaseApproveScreen)Session["ReleaseApproveScreen"];
                }
            }
            set { Session["ReleaseApproveScreen"] = value; }
        }

        #region "Sub&Function"
        private void IntialPSC2412()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2412";
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
        
        private List<ReleaseYardDetail> CreateReleaseDetailEachYard(DateTime pRequestDate)
        {
            List<ReleaseYardDetail> result = null;
            List<ReleaseDetail> objReleaseDetail = null;
            //List<StockList> objStockListList = null;
            //Boolean IsBreak = false;
            //int intSUMQTY = 0;
            ReleaseYardDetail objNew = null;
           
            try
            {
                ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                //objReleaseDetail = objReleaseDetailService.GetReleaseDetailList(pRequestDate, Constants.ReleaseDetailStatus.Release);
                objReleaseDetail = objReleaseDetailService.GetHHTReleaseList(pRequestDate, Constants.HHTReleaseStatus.Approve);
                if (objReleaseDetail != null)
                {
                    foreach (ReleaseDetail en in objReleaseDetail)
                    {
                        if (result == null)
                        {
                            result = new List<ReleaseYardDetail>();
                        }
                        objNew = new ReleaseYardDetail();
                        objNew.RowNo = result.Count() + 1;
                        objNew.HHTReleaseId = en.HHTReleaseId;
                        objNew.TransDate = en.TransDate;
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
                        objNew.Status = en.Status;
                        objNew.Status_Name = GetStatusName((int)en.Status); //GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                        result.Add(objNew);
                    }
                }
                //if (objReleaseDetail != null)
                //{
                //    string iString = "2019-04-01";
                //    DateTime dateStock = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

                //    // check language is Thai
                //    if (Request.Cookies["PSCS_culture"].Value.Equals("Th") && dateStock != null)
                //    {
                //        // convert Buddhist era B.E.(+543) to A.D.(2019)
                //        dateStock = dateStock.AddYears(543);
                //    }

                //    foreach (ReleaseDetail en in objReleaseDetail)
                //    {
                //        intSUMQTY = Convert.ToInt32(en.RequestQTY);
                //        StockListService objStockListService = new StockListService(this.dbEntities);
                //        objStockListList = objStockListService.GetStockListRelease(dateStock, en.ItemCode, en.HeatNo);
                //        if (objStockListList != null)
                //        {
                //            if (objStockListList.Count() != 0)
                //            {
                //                foreach (StockList enSt in objStockListList)
                //                {
                //                    if (result == null)
                //                    {
                //                        result = new List<ReleaseYardDetail>();
                //                    }
                //                    objNew = new ReleaseYardDetail();
                //                    objNew.RowNo = result.Count() + 1;
                //                    objNew.ReleaseId = en.ReleaseId;
                //                    objNew.ItemCode = en.ItemCode;
                //                    objNew.HeatNo = en.HeatNo;
                //                    objNew.JobNo = en.JobNo;
                //                    objNew.MfgNo = en.MfgNo;
                //                    objNew.RequestDate = en.RequestDate;
                //                    objNew.RequestNo = en.RequestNo;
                //                    objNew.Description = en.Description;
                //                    objNew.Maker = en.Maker;
                //                    objNew.Maker_Name = en.Maker_Name;
                //                    objNew.Grade = en.Grade;
                //                    objNew.Grade_Name = en.Grade_Name;
                //                    objNew.YardID = enSt.YardID;
                //                    objNew.YardName = enSt.YardName;
                //                    objNew.LocationCode = enSt.LocationCode;
                //                    objNew.LocationName = enSt.LocationName;
                //                    if (enSt.Qty >= intSUMQTY)
                //                    {
                //                        objNew.RequestQTY = intSUMQTY;
                //                        intSUMQTY = 0;
                //                        IsBreak = true;
                //                    }
                //                    else
                //                    {
                //                        intSUMQTY = intSUMQTY - Convert.ToInt32(enSt.Qty);
                //                        objNew.RequestQTY = enSt.Qty;
                //                    }
                //                    objNew.ActualQTY = intSUMQTY;
                //                    objNew.Status = en.Status;
                //                    objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                //                    result.Add(objNew);

                //                    if (IsBreak)
                //                    {
                //                        break;
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                if (result == null)
                //                {
                //                    result = new List<ReleaseYardDetail>();
                //                }
                //                objNew = new ReleaseYardDetail();
                //                objNew.RowNo = result.Count() + 1;
                //                objNew.ReleaseId = en.ReleaseId;
                //                objNew.ItemCode = en.ItemCode;
                //                objNew.HeatNo = en.HeatNo;
                //                objNew.JobNo = en.JobNo;
                //                objNew.MfgNo = en.MfgNo;
                //                objNew.RequestDate = en.RequestDate;
                //                objNew.RequestNo = en.RequestNo;
                //                objNew.Description = en.Description;
                //                objNew.Maker = en.Maker;
                //                objNew.Maker_Name = en.Maker_Name;
                //                objNew.Grade = en.Grade;
                //                objNew.Grade_Name = en.Grade_Name;
                //                objNew.YardID = "";
                //                objNew.YardName = "";
                //                objNew.LocationCode = "";
                //                objNew.LocationName = "";
                //                objNew.RequestQTY = 0;
                //                objNew.Status = en.Status;
                //                objNew.Status_Name = GetStatusName(en.Status, Request.Cookies["PSCS_culture"].Value);
                //                result.Add(objNew);
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private string GetStatusName(int pStatus)
        {
            string result = string.Empty;
            if (pStatus == (int)Constants.HHTReleaseStatus.New)
            {
                result = Constants.HHTReleaseStatus.New.ToString();
                result = Resources.Common_cshtml.New;
            }
            else if (pStatus == (int)Constants.HHTReleaseStatus.Submit)
            {
                result = Constants.HHTReleaseStatus.Submit.ToString();
                result = Resources.Common_cshtml.Submit;
            }
            else if (pStatus == (int)Constants.HHTReleaseStatus.Approve)
            {
                result = Constants.HHTReleaseStatus.Approve.ToString();
                result = Resources.Common_cshtml.Approve;
            }
            else
            {
                result = "";
            }

            return result;
        }

        private ActionResult Approve_OnClick(DateTime pReleaseDate)
        {
            Boolean result = false;
            List<ReleaseYardDetail> objReleaseYardDetailList = null;

            ReleaseService objReleaseSyncTransService = null;
            Boolean IsSync = false;

            SetPipeReleaseToSyteLineService objSetPipeReleaseToSyteLineService = null;

            try
            {
                foreach (ReleaseYardDetail en in this.model.ReleaseYardDetailListDisplay)
                {
                    objReleaseSyncTransService = new ReleaseService(this.dbEntities);
                    IsSync = objReleaseSyncTransService.UpdateStatus(en.HHTReleaseId, Common.Constants.HHTReleaseStatus.Sync, this.LoginUser.UserId);
                    if(IsSync)
                    {
                        string[] objJobNo = en.JobNo.Split(',');
                        if (objJobNo.Length == 1)
                        {
                            Pipe_Release objPipe_Release = new Pipe_Release
                            {
                                Job = en.JobNo,
                                Item_PIpe = en.ItemCode,
                                qty_release = en.ActualQTY == null ? 0 : Convert.ToDecimal(en.ActualQTY),
                                Trans_Date = en.TransDate == null ? DateTime.Now : Convert.ToDateTime(en.TransDate),
                                release_status = true,
                                Create_date = DateTime.Now,
                            };

                            objSetPipeReleaseToSyteLineService = new SetPipeReleaseToSyteLineService(this.dbERPDEV01Entities);
                            result = objSetPipeReleaseToSyteLineService.InsertData(objPipe_Release, this.LoginUser.UserId);
                        }
                        else
                        {
                            foreach(string enJobNo in objJobNo)
                            {
                                Pipe_Release objPipe_Release = new Pipe_Release
                                {
                                    Job = enJobNo.Trim(),
                                    Item_PIpe = en.ItemCode,
                                    qty_release = en.ActualQTY == null ? 0 : Convert.ToDecimal(en.ActualQTY),
                                    Trans_Date = en.TransDate == null ? DateTime.Now : Convert.ToDateTime(en.TransDate),
                                    release_status = true,
                                    Create_date = DateTime.Now,
                                };

                                objSetPipeReleaseToSyteLineService = new SetPipeReleaseToSyteLineService(this.dbERPDEV01Entities);
                                result = objSetPipeReleaseToSyteLineService.InsertData(objPipe_Release, this.LoginUser.UserId);
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                {
                    this.model.AlertsType = Constants.AlertsType.Success;
                    this.model.Message = PSCS.Resources.PSC2411_cshtml.AppComplete;
                    
                    //ReLoad
                    objReleaseYardDetailList = CreateReleaseDetailEachYard(pReleaseDate);
                    if (objReleaseYardDetailList == null)
                    {
                        objReleaseYardDetailList = new List<ReleaseYardDetail>();
                    }
                    this.model.ReleaseYardDetailList = objReleaseYardDetailList;
                    FilterData(this.model.FilterRequestDate, this.model.FilterYardID);
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = PSCS.Resources.PSC2411_cshtml.AppError;
                }

                return View(this.model);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }
        }

        private void FilterData(DateTime? pReleaseDate, string pYard)
        {
            DateTime dateRelease = DateTime.Now;
            List<ReleaseYardDetail> objReleaseYardDetailList = null;

            try
            {
                if (pReleaseDate != null)
                {
                    dateRelease = Convert.ToDateTime(pReleaseDate);

                    objReleaseYardDetailList = CreateReleaseDetailEachYard(dateRelease);
                    if (objReleaseYardDetailList == null)
                    {
                        objReleaseYardDetailList = new List<ReleaseYardDetail>();
                    }
                    this.model.ReleaseYardDetailList = objReleaseYardDetailList;
                }

                if(pYard != string.Empty)
                {
                    objReleaseYardDetailList = (from rd in this.model.ReleaseYardDetailList
                                                      where (string.IsNullOrEmpty(pYard) || rd.YardID == pYard)
                                                      orderby rd.YardID ascending
                                                      select new
                                                      {
                                                          rd.YardID,
                                                          rd.YardName,
                                                          rd.HHTReleaseId,
                                                          rd.TransDate,
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
                                                          HHTReleaseId = x.HHTReleaseId,
                                                          TransDate = x.TransDate,
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

                    if (objReleaseYardDetailList == null)
                    {
                        objReleaseYardDetailList = new List<ReleaseYardDetail>();
                    }
                    this.model.ReleaseYardDetailListDisplay = objReleaseYardDetailList;
                }

                this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region "PSC2412 Release Daily Pipe Approve"
        [NoDirectAccess]
        [HttpGet]
        // GET: ReleaseApprove
        public ActionResult PSC2412()
        {
            List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                IntialPSC2412();

                this.model.FilterRequestDate = DateTime.Now;
                this.model.YardList = GetYard();

                objReleaseYardDetailList = CreateReleaseDetailEachYard(this.model.FilterRequestDate);

                if (objReleaseYardDetailList == null)
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
        public ActionResult PSC2412(ReleaseDailyPipeScreen pModel, string submitButton)
        {
            try
            {
                ModelState.Clear();

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

                        case "Approve":
                            return Approve_OnClick(Convert.ToDateTime(this.model.FilterRequestDate));

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