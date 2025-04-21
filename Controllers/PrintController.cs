using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using System.Text;
using System.IO;
using System.Drawing;
using OfficeOpenXml;
using PSCS.Excels;
using System.Drawing.Imaging;
using System.Configuration;
using System.Net;
using PSCS.ModelsScreen;
using System.Management;

namespace PSCS.Controllers
{
    public class PrintController : BaseController
    {
        public PrintScreen myModel
        {
            get
            {
                if (Session["ModelPrintScreen"] == null)
                {
                    Session["ModelPrintScreen"] = new PrintScreen();
                    return (PrintScreen)Session["ModelPrintScreen"];
                }
                else
                {
                    return (PrintScreen)Session["ModelPrintScreen"];
                }
            }
            set { Session["ModelPrintScreen"] = value; }
        }

        // GET: Print
        public ActionResult Index()
        {
            try
            {
                InitializeActionName = "Index";
                QueryStringList = new Dictionary<string, string>();
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;
                //this.myModel.InputPrinter = "\\\\192.168.200.20\\ZDesigner ZT230-200dpi ZPL";
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            return View(this.myModel);
        }

        [HttpPost]
        public ActionResult Index(string submitButton, PrintTestScreen model)
        {
            try
            {
                switch (submitButton)
                {
                    case "Print":
                        //return (Print_OnClick(model.FilterPrinter));
                        return (Print_OnClick());
                    default:
                        return View(this.myModel);
                }
            }
            catch (Exception ex)
            {
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }
        }


        private void IntialPSC2510()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2510";
            QueryStringList = new Dictionary<string, string>();
            this.myModel.FilterDeliveryDate = null;
            this.myModel.FilterContainerNo = "";
            this.myModel.FilterHeatNo = "";
            this.myModel.FilterOD = "";
            this.myModel.FilterWT = "";
            this.myModel.FilterLength = "";
            //this.myModel.ReceivePlanList = this.GetReceiveList(null, "");
            //this.myModel.ReceivePlanList = Search(this.myModel);
        }


        private List<ReceiveItemsPlan> Search(PrintScreen FilterModel)
        {
            List<ReceiveItemsPlan> result = null;

            try
            {
                //result = GetReceiveList(FilterModel.FilterDeliveryDate,
                //                        FilterModel.FilterContainerNo);

                    List<ReceiveItemsPlan> objReceiveItemsPlanList = GetReceivePlanAndDetailList(FilterModel.FilterDeliveryDate,
                                                                                                 FilterModel.FilterContainerNo);

                    if (FilterModel.FilterHeatNo != null && FilterModel.FilterHeatNo != string.Empty)
                    {
                        if (objReceiveItemsPlanList != null)
                        {
                            objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.HeatNo == FilterModel.FilterHeatNo).ToList();
                        }
                    }
                    if (FilterModel.FilterOD != null && FilterModel.FilterOD != string.Empty)
                    {
                        if (objReceiveItemsPlanList != null)
                        {
                            objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.OD == Convert.ToDecimal(FilterModel.FilterOD)).ToList();
                        }
                    }
                    if (FilterModel.FilterWT != null && FilterModel.FilterWT != string.Empty)
                    {

                        if (objReceiveItemsPlanList != null)
                        {
                            objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.WT == Convert.ToDecimal(FilterModel.FilterWT)).ToList();
                        }
                    }
                    if (FilterModel.FilterLength != null && FilterModel.FilterLength != string.Empty)
                    {
                        if (objReceiveItemsPlanList != null)
                        {
                            objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.Length == Convert.ToDecimal(FilterModel.FilterLength)).ToList();
                        }
                    }

                result = objReceiveItemsPlanList;
                    //List<ReceiveItemsPlan> objReceiveItemsPlanListDistinct = (objReceiveItemsPlanList.GroupBy(r => new { r.ReceiveId })
                    //                        .Select((m, index) => new ReceiveItemsPlan
                    //                        {
                    //                            ReceiveId = m.Key.ReceiveId,
                    //                        })).ToList();

                //if (objReceiveItemsPlanListDistinct == null)
                //{
                //    result = new List<ReceivePlan>();
                //}
                //else
                //{
                //    List<ReceivePlan> objReceivePlanList = new List<ReceivePlan>();

                //    foreach (ReceiveItemsPlan en in objReceiveItemsPlanListDistinct)
                //    {
                //        foreach (ReceivePlan enReceivePlan in result)
                //        {
                //            if (en.ReceiveId == enReceivePlan.ReceiveId)
                //            {
                //                objReceivePlanList.Add(enReceivePlan);
                //            }
                //        }
                //    }

                //    result = objReceivePlanList;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<ReceiveItemsPlan> GetReceivePlanAndDetailList(DateTime? pDeliveryDate, string pContainerNo)
        {
            List<ReceiveItemsPlan> result = new List<ReceiveItemsPlan>();
            try
            {
                ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                string pLanguage = langCookie != null ? langCookie.Value : "En";

                result = objReceivePlanService.GetReceivePlanAndDetailList(pDeliveryDate, pContainerNo, pLanguage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        // Filter ReceivePlan view
        private ActionResult Filter_OnClick(PrintScreen FilterModel)
        {
            try
            {
                this.myModel.ReceiveItemsPlanList = Search(FilterModel);

                if (FilterModel.FilterHeatNo != null && FilterModel.FilterHeatNo != string.Empty)
                {
                    this.myModel.FilterHeatNo = FilterModel.FilterHeatNo;
                }
                if (FilterModel.FilterOD != null && FilterModel.FilterOD != string.Empty)
                {
                    this.myModel.FilterOD = FilterModel.FilterOD;
                }
                if (FilterModel.FilterWT != null && FilterModel.FilterWT != string.Empty)
                {
                    this.myModel.FilterWT = FilterModel.FilterWT;
                }
                if (FilterModel.FilterLength != null && FilterModel.FilterLength != string.Empty)
                {
                    this.myModel.FilterLength = FilterModel.FilterLength;
                }

                this.myModel.FilterDeliveryDate = FilterModel.FilterDeliveryDate;
                this.myModel.FilterContainerNo = FilterModel.FilterContainerNo;
                this.myModel.FilterHeatNo = FilterModel.FilterHeatNo;
                this.myModel.FilterOD = FilterModel.FilterOD;
                this.myModel.FilterWT = FilterModel.FilterWT;
                this.myModel.FilterLength = FilterModel.FilterLength;
                this.myModel.Total = this.myModel.ReceiveItemsPlanList.Count.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2510", this.myModel);
        }

        private List<ReceivePlan> GetReceiveList(DateTime? pDeliveryDate, string pContainerNo)
        {
            List<ReceivePlan> result = new List<ReceivePlan>();
            try
            {
                ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                string pLanguage = langCookie != null ? langCookie.Value : "En";

                result = objReceivePlanService.GetReceivePlanList(pDeliveryDate, pContainerNo, pLanguage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public ActionResult PSC2510()
        {
            try
            {
                InitializeActionName = "Index";
                QueryStringList = new Dictionary<string, string>();
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;

                // Initial View
                IntialPSC2510();
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            return View(this.myModel);
        }

        [HttpPost]
        public ActionResult PSC2510(string submitButton, PrintScreen FilterModel)
        {
            try
            {
                switch (submitButton)
                {
                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");

                    case "Filter":
                        return Filter_OnClick(FilterModel);

                    case "Print":
                        //return (Print_OnClick(model.FilterPrinter));
                        return (Print_OnClick());

                    default:
                        return View(this.myModel);
                }
            }
            catch (Exception ex)
            {
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }
        }

        private ActionResult Print_OnClick()
        {
            bool result = false;

            try
            {
                
            }
            catch (Exception ex)
            {
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            return View(this.myModel);
        }
    }
}