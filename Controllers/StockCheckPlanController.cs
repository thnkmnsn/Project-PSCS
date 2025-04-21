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
    public class StockCheckPlanController : BaseController
    {
        public StockCheckPlanScreen model
        {
            get
            {
                if (Session["StockCheckPlanScreen"] == null)
                {
                    Session["StockCheckPlanScreen"] = new StockCheckPlanScreen();
                    return (StockCheckPlanScreen)Session["StockCheckPlanScreen"];
                }
                else
                {
                    return (StockCheckPlanScreen)Session["StockCheckPlanScreen"];
                }
            }
            set { Session["StockCheckPlanScreen"] = value; }
        }

        #region "Sub&Function"
        private void InitialPSC2310()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2310";

            // Parent view
            this.model.PipeYardList = GetYard();
            this.model.StatusList = GetStatus();
            //this.model.VisorStatusList = GetVisorStatus();
            //this.model.FilterStatus = "";

            // Patial view
            this.model.YardList = GetYard();
            this.model.LocationList = GetLocationList();
        }

        private void Initial_PSC2310()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2310";
            QueryStringList = new Dictionary<string, string>();
            //this.SetRoleFlag();
            //var initYard = "";

            this.model.AlertsType = Constants.AlertsType.None;
            this.model.Message = string.Empty;
        }

        private List<SelectListItem> GetYard()
        {
            List<SelectListItem> yard = new List<SelectListItem>();
            YardService service = new YardService(this.dbEntities);
            List<Yard> objLocationList = service.GetYardList();
            foreach (Yard objYard in objLocationList)
            {
                yard.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
            }

            return yard;
        }

        private List<SelectListItem> GetStatus()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.StockCheckStatus enStatus in (Constants.StockCheckStatus[])Enum.GetValues(typeof(Constants.StockCheckStatus)))
            {
                result.Add(new SelectListItem
                {
                    Text = (enStatus.ToString().Equals("Create") ? Resources.Common_cshtml.Created :
                            enStatus.ToString().Equals("Submit") ? Resources.Common_cshtml.Submit :
                            enStatus.ToString().Equals("Checked") ? Resources.Common_cshtml.Checked :
                            enStatus.ToString().Equals("Need_to_Check") ? Resources.Common_cshtml.NeedToCheck :
                            enStatus.ToString().Equals("Approve") ? Resources.Common_cshtml.Approve :
                            enStatus.ToString().Equals("Adjust") ? Resources.Common_cshtml.Adjust :
                            enStatus.ToString().Equals("Reject") ? Resources.Common_cshtml.Reject :
                            enStatus.ToString().Equals("Manager_Approve") ? Resources.Common_cshtml.MangerApprove : ""),
                    Value = ((int)enStatus).ToString()
                });
            }

            return result;
        }

        private List<SelectListItem> GetLocationList(string pYardList = "")
        {
            List<SelectListItem> result = new List<SelectListItem>();
            LocationService objLocationService = new LocationService(this.dbEntities);
            foreach (Location objLocation in objLocationService.GetLocationList(pYardList))
            {
                result.Add(new SelectListItem { Text = objLocation.Name, Value = objLocation.LocationCode.ToString() });
            }

            return result;
        }

        #endregion


        #region "PSC2310 Stock Check Plan"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2310()
        {
            //List<ReleaseYardDetail> objReleaseYardDetailList = null;
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                InitialPSC2310();

                this.model.ItemList = GetStockItemList("", "", "", "");

                //objReleaseYardDetailList = CreateReleaseDetailEachYard();

                //if (objReleaseYardDetailList == null)
                //{
                //    objReleaseYardDetailList = new List<ReleaseYardDetail>();
                //}
                //this.model.ReleaseYardDetailList = objReleaseYardDetailList;
                //this.model.ReleaseYardDetailListDisplay = this.model.ReleaseYardDetailList;
                //this.model.Total = this.model.ReleaseYardDetailListDisplay.Count.ToString();
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
            }

            return View(this.model);
        }

        [HttpGet]
        public PartialViewResult PSC2310PlanTable(DateTime? pDate, string pPipeYard, string pStatus)
        {
            this.Initial_PSC2310();

            // check language is Thai
            if (Request.Cookies["PSCS_culture"].Value.Equals("Th") && pDate != null)
            {
                // convert Buddhist era B.E.(+543) to A.D.(2019)
                pDate = Convert.ToDateTime(pDate).AddYears(-543);
            }
            if (pDate == null)
            {
                pDate = DateTime.Today;
            }
            this.model.FilterStockCheckList = GetStockCheckListByController(pDate, pPipeYard, pStatus);
            ViewBag.Total = this.model.FilterStockCheckList.Count.ToString();

            return PartialView("PSC2310PlanTable", this.model.FilterStockCheckList);
        }

        // Patial View: Modal Table (Filter) [Contoller]
        [HttpGet]
        public PartialViewResult PSC2310PartialTable(string pYard, string pLocation, string pHreatNo, string pDesc)
        {
            this.Initial_PSC2310();
            this.model.ItemList = GetStockItemList(pYard, pLocation, pHreatNo, pDesc);
            return PartialView("PSC2310PartialTable", this.model.ItemList);
        }

        // Parent View: Index(Post) [Contoller]
        [HttpPost]
        public ActionResult PSC2310(string submitButton, StockCheckScreen FilterModel)
        {
            // Initial View
            this.Initial_PSC2310();
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = this.LoginUser.UserId;
                    string message = string.Empty;
                    Boolean result = false;
                    Boolean chkfilter = false;

                    StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
                    var pDataList = this.model.StockCheckList;

                    switch (submitButton)
                    {
                        case "Save":
                            // Save from plan [Controller]
                            //result = objStockCheckService.InsertStockCheckData(pDataList, userId);
                            message = result ? Resources.Common_cshtml.EditSuccessMsg : Resources.Common_cshtml.EditFailMsg;
                            break;
                        case "Back":
                            // Back main mune [All role]
                            return RedirectToAction("PSC0100", "Menu");

                        case "ClearFilter":
                            string strToday = DateTime.Now.ToString(Common.Constants.DATE_HYPHEN);

                            ModelState.SetModelValue("FilterStockDate", new ValueProviderResult(strToday, strToday, CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterPipeYard", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterStatus", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

                            FilterModel.FilterStockDate = DateTime.Today;
                            FilterModel.FilterPipeYard = null;
                            FilterModel.FilterStatus = "";

                            result = true;
                            chkfilter = true;

                            break;
                        default:
                            break;
                    }

                    // Alert Message
                    if (chkfilter != true)
                    {
                        this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                        this.model.Message = message;
                        ModelState.Clear();
                    }


                    return View("PSC2310", this.model);
                }
                catch (Exception ex)
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = ex.Message;
                    this.PrintError(ex.Message);
                }
            }

            return View(this.model);
        }


        // SuperVisor View [Supervisor]
        [HttpGet]
        public ActionResult PSC2310_SuperVisor()
        {
            try
            {
                this.Initial_PSC2310_SuperVisor();
                this.Initial_DDL();

                var stockVisorList = GetStockCheckListByYardSuppervisor(DateTime.Today, "", "");
                //this.model.StockVisorList = stockVisorList;
                this.model.Total = stockVisorList.Count.ToString();

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
        public ActionResult GetActualQty(string pStockCheckId, string pItemCode, string pHeatNo)
        {
            string result = string.Empty;

            HHTReceiveService objHHTReceiveService = null;
            List<HHTReceive> objHHTResult = null;
            decimal decActualQTY = 0;
            ReceivingInstructionDetailService objReceivingInstructionDetailService = null;
            ReceivePlanService objReceivePlanService = null;
            ReceivingInstructionDetail objReceivingInstructionDetail = null;

            try
            {
                //objHHTReceiveService = new HHTReceiveService(this.dbEntities);
                //objHHTResult = objHHTReceiveService.GetHHTReceiveList(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo);

                //if (objHHTResult != null)
                //{
                //    foreach (HHTReceive en in objHHTResult)
                //    {
                //        decActualQTY = decActualQTY + en.ActualQTY;
                //    }
                //    objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
                //    if (objHHTResult.Count == 1)
                //    {
                //        objReceivingInstructionDetailService.Update(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo, this.LoginUser.UserId, decActualQTY, Convert.ToByte(Constants.ReceiveStatus.Submit));
                //    }
                //    else
                //    {
                //        objReceivingInstructionDetailService.Update(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo, this.LoginUser.UserId, decActualQTY);
                //    }
                //}

                //objReceivePlanService = new ReceivePlanService(this.dbEntities);
                //objReceivePlanDetail = objReceivePlanService.GetReceive(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo);

                //if (objReceivePlanDetail != null)
                //{
                //    //result = objReceivePlanDetail.ActualQty + "*" + objReceivePlanDetail.Status;
                //    result = 1 + "*" + 1;
                //}

                result = 1 + "*" + 1;

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        // Index(Post) [Supervisor]
        [HttpPost]
        public ActionResult PSC2310_SuperVisor(string submitButton, StockCheckScreen FilterModel)
        {
            // Initial View
            this.Initial_PSC2310_SuperVisor();
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = this.LoginUser.UserId;
                    string message = string.Empty;
                    Boolean result = false;
                    Boolean chkfilter = false;

                    StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
                    var pDataList = this.model.StockVisorList;

                    switch (submitButton)
                    {
                        case "Filter":
                            //return Filter_VisorTable(FilterModel);
                        case "Save":
                            // Save from plan [Controller]
                            result = objStockCheckService.UpdateStockCheckData(pDataList, userId);
                            message = result ? Resources.Common_cshtml.EditSuccessMsg : Resources.Common_cshtml.EditFailMsg;
                            break;
                        case "Approve":
                            result = objStockCheckService.ApproveStockCheckData(pDataList, userId);
                            message = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;

                            var stockVisorList = GetStockCheckListByYardSuppervisor(FilterModel.FilterStockDate, FilterModel.FilterPipeYard, FilterModel.FilterStatus);
                            //this.model.StockVisorList = stockVisorList;
                            this.model.Total = stockVisorList.Count.ToString();
                            break;
                        case "Back":
                            // Back main mune [All role]
                            return RedirectToAction("PSC0100", "Menu");

                        case "ClearFilter":
                            string strToday = DateTime.Now.ToString(Common.Constants.DATE_HYPHEN);

                            ModelState.SetModelValue("FilterStockDate", new ValueProviderResult(strToday, strToday, CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterPipeYard", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterStatus", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

                            FilterModel.FilterStockDate = DateTime.Today;
                            FilterModel.FilterPipeYard = null;
                            FilterModel.FilterStatus = "";
                            //this.model.FilterStockDate = DateTime.Today;
                            //this.model.FilterPipeYard = null;
                            //this.model.FilterStatus = "";

                            result = true;
                            chkfilter = true;

                            //Filter_VisorTable(FilterModel);

                            break;
                        default:
                            break;
                    }

                    // Alert Message
                    if (chkfilter != true)
                    {
                        this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                        this.model.Message = message;
                        ModelState.Clear();
                    }


                    return View("PSC2310_SuperVisor", this.model);
                }
                catch (Exception ex)
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = ex.Message;
                    this.PrintError(ex.Message);
                }
            }

            return View(this.model);
        }


        // Adjust View [Adjust]
        [HttpGet]
        public ActionResult PSC2310_Adjust()
        {
            try
            {
                this.Initial_PSC2310_Adjust();
                this.Initial_DDL();

                if (this.LoginUser.RoleId == Constants.ROLE_MANAGER)
                {
                    this.model.FilterStatus = "5";
                }
                else
                {
                    this.model.FilterStatus = "4";
                }

                // Default
                var stockChkList = GetStockCheckListByController(DateTime.Today, "", this.model.FilterStatus);
                this.model.Total = stockChkList.Count.ToString();

                //setButton(stockChkList.Count());

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

        [HttpPost]
        public ActionResult PSC2310_Adjust(string submitButton, StockCheckScreen FilterModel)
        {
            try
            {
                this.Initial_PSC2310_Adjust();
                this.Initial_DDL();


                if (ModelState.IsValid)
                {
                    try
                    {
                        string userId = this.LoginUser.UserId;
                        string message = string.Empty;
                        Boolean result = false;
                        Boolean chkfilter = false;

                        switch (submitButton)
                        {

                            case "ClearFilter":
                                string strToday = DateTime.Now.ToString(Common.Constants.DATE_HYPHEN);

                                ModelState.SetModelValue("FilterStockDate", new ValueProviderResult(strToday, strToday, CultureInfo.InvariantCulture));
                                ModelState.SetModelValue("FilterPipeYard", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                                ModelState.SetModelValue("FilterStatus", new ValueProviderResult(((int)Common.Constants.StockCheckStatus.Approve).ToString(), ((int)Common.Constants.StockCheckStatus.Approve).ToString(), CultureInfo.InvariantCulture));

                                FilterModel.FilterStockDate = DateTime.Today;
                                FilterModel.FilterPipeYard = null;
                                FilterModel.FilterStatus = ((int)Common.Constants.StockCheckStatus.Approve).ToString();

                                result = true;
                                chkfilter = true;

                                break;
                            default:
                                break;
                        }

                        // Alert Message
                        if (chkfilter != true)
                        {
                            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            this.model.Message = message;
                            ModelState.Clear();
                        }

                    }
                    catch (Exception ex)
                    {
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = ex.Message;
                        this.PrintError(ex.Message);
                    }
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



        //// Adjust Table (initial / Filter) [Adjust]
        //[HttpGet]
        //public PartialViewResult PSC2310AdjustTable(DateTime? pDate, string pPipeYard, string pStatus)
        //{
        //    this.Initial_PSC2310_Adjust();

        //    // check language is Thai
        //    if (Request.Cookies["PSCS_culture"].Value.Equals("Th") && pDate != null)
        //    {
        //        // convert Buddhist era B.E.(+543) to A.D.(2019)
        //        pDate = Convert.ToDateTime(pDate).AddYears(-543);
        //    }

        //    // Default
        //    pDate = pDate != null ? pDate : DateTime.Today;
        //    pStatus = !string.IsNullOrEmpty(pStatus) ? pStatus : "4";

        //    var stockChkList = GetStockCheckListByController(pDate, pPipeYard, pStatus);
        //    this.model.StockAdjustList = stockChkList;
        //    this.model.Total = this.model.StockAdjustList.Count.ToString();

        //    setButton(this.model.StockAdjustList.Count());

        //    return PartialView("PSC2310AdjustTable", this.model.StockAdjustList);
        //}


        //// Method Zone
        //#region Private
        //private void setButton(int intTotal)
        //{
        //    if (intTotal > 0)
        //    {
        //        this.model.isDisable = "";
        //    }
        //    else
        //    {
        //        this.model.isDisable = "disabled";
        //    }
        //}

        private void Initial_PSC2310_SuperVisor()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2310_SuperVisor";
            QueryStringList = new Dictionary<string, string>();
            this.SetRoleFlag();
            //var initYard = "";

            this.model.AlertsType = Constants.AlertsType.None;
            this.model.Message = string.Empty;
        }

        private void Initial_PSC2310_Adjust()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2310_Adjust";
            QueryStringList = new Dictionary<string, string>();
            this.SetRoleFlag();
            //var initYard = "";

            this.model.AlertsType = Constants.AlertsType.None;
            this.model.Message = string.Empty;
        }

        private void Initial_DDL()
        {
            // Parent view
            this.model.PipeYardList = GetYard();
            this.model.StatusList = GetStatus();
            //this.model.VisorStatusList = GetVisorStatus();
            this.model.FilterStatus = "";

            // Patial view
            this.model.YardList = GetYard();
            this.model.LocationList = GetLocationList();
        }


        // Parent view: Filter with Language [Controller]
        private List<StockCheck> GetStockCheckList(DateTime? pDate, string pPipeYard, string pStatus)
        {
            StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            string pLanguage = langCookie != null ? langCookie.Value : "En";
            List<StockCheck> result = new List<StockCheck>();

            switch (this.LoginUser.RoleId)
            {
                case Constants.ROLE_SYSTEMADMIN:
                case Constants.ROLE_ADMIN:
                case Constants.ROLE_MANAGER:
                case Constants.ROLE_CONTROLLER:
                    if (pStatus == "4" || pStatus == "5")
                    {
                        result = objStockCheckService.GetStockCheckListWithAdjust(pDate, pPipeYard, pStatus);
                    }
                    else
                    {
                        result = objStockCheckService.GetStockCheckListWithView(pDate, pPipeYard, pStatus, this.model.StockCheckList);
                    }

                    break;
                case Constants.ROLE_YARDSUPERVISOR:
                    result = objStockCheckService.GetStockCheckListWithDB(pDate, pPipeYard, pStatus);
                    break;
                case Constants.ROLE_WORKER:
                default:
                    break;
            }

            return result;
        }

        private List<StockCheck> GetStockCheckListByController(DateTime? pDate, string pPipeYard, string pStatus)
        {
            List<StockCheck> result = new List<StockCheck>();

            try
            {
                StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                string pLanguage = langCookie != null ? langCookie.Value : "En";

                if (pStatus == "4" || pStatus == "5")
                {
                    result = objStockCheckService.GetStockCheckListWithAdjust(pDate, pPipeYard, pStatus);
                }
                else
                {
                    //result = objStockCheckService.GetStockCheckListWithView(pDate, pPipeYard, pStatus, this.model.StockCheckList);
                    result = objStockCheckService.GetStockCheckList(pDate, pPipeYard, pStatus);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<StockCheck> GetStockCheckListByYardSuppervisor(DateTime? pDate, string pPipeYard, string pStatus)
        {
            List<StockCheck> result = new List<StockCheck>();

            try
            {
                StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                string pLanguage = langCookie != null ? langCookie.Value : "En";

                result = objStockCheckService.GetStockCheckListWithDB(pDate, pPipeYard, pStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //// Delete Onclick [Ajax]
        //[HttpPost]
        //public ActionResult Delete_StockCheckList(string Selected)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Selected))
        //        {
        //            StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
        //            var stockCheckList = objStockCheckService.RemoveFromGrid(Selected, this.model.StockCheckList);

        //            this.model.StockCheckList = stockCheckList;
        //            this.model.FilterStockCheckList = stockCheckList;
        //            this.model.Total = stockCheckList.Count.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.model.AlertsType = Constants.AlertsType.Danger;
        //        this.model.Message = ex.Message;
        //        this.PrintError(ex.Message);

        //        return new HttpStatusCodeResult(400, ex.Message);
        //    }

        //    return Json(this.model, JsonRequestBehavior.AllowGet);
        //}


        // Patial view: Filter with Language [Controller]
        private List<StockList> GetStockItemList(string pYardId, string pLocation, string pHeatNo, string pDesc)
        {
            StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            string pLanguage = langCookie != null ? langCookie.Value : "En";

            List<StockList> result = objStockCheckService.GetStockItemList(pYardId, pLocation, pHeatNo, pDesc, pLanguage);
            return result;
        }


        // Patial View: Ok Onclick [Controller]
        [HttpPost]
        public ActionResult Add_ParentGrid(string Selected)
        {
            try
            {
                if (!string.IsNullOrEmpty(Selected))
                {
                    StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
                    var stockCheckList = objStockCheckService.InsertStockcheckGrid(Selected, this.model.ItemList, this.model.StockCheckList);

                    this.model.HasAddSelected = true;
                    this.model.StockCheckList = stockCheckList;
                    this.model.FilterStockCheckList = stockCheckList;
                    this.model.Total = stockCheckList.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(this.model, JsonRequestBehavior.AllowGet);
        }


        // Table (Filter) [Supervisor]
        [HttpGet]
        public ActionResult Filter_VisorTable(StockCheckScreen FilterModel)
        {
            try
            {
                var stockVisorList = GetStockCheckListByYardSuppervisor(FilterModel.FilterStockDate, FilterModel.FilterPipeYard, FilterModel.FilterStatus);
                this.model.StockVisorList = stockVisorList;
                this.model.Total = stockVisorList.Count.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2310_SuperVisor", this.model);
        }

        //// Get Actual real time [Supervisor]
        //[HttpPost]
        //public ActionResult Get_ActualQty(string pRowNo)
        //{
        //    StockCheckService service = new StockCheckService(this.dbEntities);
        //    int row = Int32.Parse(pRowNo);
        //    decimal? actualQty = null;
        //    int? status = 0;
        //    string statusText = "";

        //    var item = this.model.StockVisorList.Where(x => x.RowNo == row).FirstOrDefault();

        //    if (item != null)
        //    {
        //        actualQty = service.GetActualQty(item.ItemCode, item.HeatNo, item.Location);
        //        status = item.Status > 0 ? item.Status : service.GetCheckStatus(actualQty, item.Qty);
        //        //status = 1;
        //        statusText = service.GetStatus(status);
        //    }

        //    return Json(new { ActualQty = actualQty, Status = status, StatusText = statusText });
        //}

        //// Model for receive data from view
        //public class UpdateModel
        //{
        //    public int Status { get; set; }
        //    public string Remark { get; set; }
        //}

        // Save Onclick [SuperVisor]
        [HttpGet]
        public ActionResult Update_StockCheckList(string pRow, string pStatus, string pRemark)
        {
            try
            {
                StockCheckService service = new StockCheckService(this.dbEntities);
                var i = Int32.Parse(pRow);
                var b = this.model.StockVisorList;

                if (!string.IsNullOrEmpty(pStatus))
                {
                    int status = Int32.Parse(pStatus);
                    //int status = Int32.Parse("1");
                    this.model.StockVisorList[i].Status = status;
                    this.model.StockVisorList[i].StatusText = service.GetStatus(status);
                    this.model.StockVisorList[i].IsEdit = true;
                }

                this.model.StockVisorList[i].Remark = pRemark;

                return Json(new { success = true, message = "Id: " + i + " Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, "Unable to find data.");
            }
        }

        //// Approve Onclick [SuperVisor]
        //[HttpPost]
        //public ActionResult Approve_StockCheckList()
        //{
        //    try
        //    {
        //        StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
        //        var pDataList = this.model.StockVisorList;
        //        string userId = this.LoginUser.UserId;

        //        // Approve from visor [Supervisor]
        //        var result = objStockCheckService.ApproveStockCheckData(pDataList, userId);
        //        var msg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;

        //        // Alert Message
        //        this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
        //        this.model.Message = msg;
        //        ModelState.Clear();

        //        return Json(new { success = result, message = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.model.AlertsType = Constants.AlertsType.Danger;
        //        this.model.Message = ex.Message;
        //        this.PrintError(ex.Message);

        //        return new HttpStatusCodeResult(400, ex.Message);
        //    }
        //}


        //// Adjust Onclick [Adjust]
        //[HttpPost]
        //public ActionResult Adjust_StockAdjustList(string Selected, string submitButton)
        //{
        //    try
        //    {
        //        StockCheckService objStockCheckService = new StockCheckService(this.dbEntities);
        //        string userId = this.LoginUser.UserId;
        //        var pDataList = this.model.StockAdjustList;
        //        Boolean result = false;
        //        var msg = string.Empty;

        //        if (!string.IsNullOrEmpty(Selected))
        //        {
        //            if (pDataList != null && LoginUser != null)
        //            {
        //                switch (submitButton)
        //                {
        //                    case "Adjust":
        //                        result = objStockCheckService.AdjustStockcheckData(Selected, pDataList, userId);
        //                        msg = result ? Resources.PSC2310_cshtml.AdjustSuccessMsg : Resources.PSC2310_cshtml.AdjustFailMsg;
        //                        break;
        //                    case "Reject":
        //                        result = objStockCheckService.RejectStockcheckData(Selected, pDataList, userId);
        //                        msg = result ? Resources.PSC2310_cshtml.RejectSuccessMsg : Resources.PSC2310_cshtml.RejectFailMsg;
        //                        break;
        //                    case "Approve":
        //                        result = objStockCheckService.ApproveStockcheckData(Selected, pDataList, userId);
        //                        msg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;
        //                        break;
        //                    default:
        //                        break;
        //                }

        //                // Default
        //                var stockChkList = GetStockCheckListByController(DateTime.Today, "", "4");
        //                this.model.Total = stockChkList.Count.ToString();
        //            }
        //        }

        //        return Json(new { success = result, message = msg });

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        // Ajax: ddl yard -> location
        public ActionResult ddlChangeYard(string Yard)
        {
            List<Location> objLocationList = new List<Location>();
            List<SelectListItem> mLocationList = new List<SelectListItem>();
            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);
                objLocationList = objLocationService.GetLocationList(Yard);

                // this make ddl display correctly, although refresh this view
                foreach (Location objLc in objLocationList)
                {
                    mLocationList.Add(new SelectListItem { Text = objLc.Name, Value = objLc.Name.ToString() });
                }

                this.model.LocationList = mLocationList;
                return Json(objLocationList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        // Ajax: ddl Status
        public ActionResult ddlStatus()
        {
            List<Location> objLocationList = new List<Location>();
            List<SelectListItem> mLocationList = new List<SelectListItem>();
            try
            {
                List<SelectListItem> result = new List<SelectListItem>();
                foreach (Constants.StockCheckStatus enStatus in (Constants.StockCheckStatus[])Enum.GetValues(typeof(Constants.StockCheckStatus)))
                {
                    result.Add(new SelectListItem { Text = enStatus.ToString(), Value = ((int)enStatus).ToString() });
                }

                //this.model.LocationList = mLocationList;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //// Ajax: Update Total [Controller]
        //[HttpGet]
        //public ActionResult UpdateTotal()
        //{
        //    try
        //    {
        //        var Total = this.model.FilterStockCheckList.Count.ToString();
        //        return Json(Total, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //// Ajax: Update Total [Supervisor]
        //[HttpGet]
        //public ActionResult UpdateTotal_Supervisor()
        //{
        //    try
        //    {
        //        var Total = this.model.StockVisorList.Count.ToString();
        //        return Json(Total, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////  Ajax: Update Total [Adjust]
        //[HttpGet]
        //public ActionResult UpdateTotal_Adjust()
        //{
        //    try
        //    {
        //        var Total = this.model.StockAdjustList.Count.ToString();
        //        return Json(Total, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //private List<SelectListItem> GetStatus()
        //{
        //    List<SelectListItem> result = new List<SelectListItem>();
        //    foreach (Constants.StockCheckStatus enStatus in (Constants.StockCheckStatus[])Enum.GetValues(typeof(Constants.StockCheckStatus)))
        //    {
        //        result.Add(new SelectListItem
        //        {
        //            Text = (enStatus.ToString().Equals("Create") ? Resources.Common_cshtml.Created :
        //                    enStatus.ToString().Equals("Submit") ? Resources.Common_cshtml.Submit :
        //                    enStatus.ToString().Equals("Check") ? Resources.Common_cshtml.Check :
        //                    enStatus.ToString().Equals("Need_to_Check") ? Resources.Common_cshtml.NeedToCheck :
        //                    enStatus.ToString().Equals("Approve") ? Resources.Common_cshtml.Approve :
        //                    enStatus.ToString().Equals("Adjust") ? Resources.Common_cshtml.Adjust :
        //                    enStatus.ToString().Equals("Reject") ? Resources.Common_cshtml.Reject :
        //                    enStatus.ToString().Equals("Manager_Approve") ? Resources.Common_cshtml.MangerApprove : ""),
        //            Value = ((int)enStatus).ToString()
        //        });
        //    }

        //    return result;
        //}

        private List<SelectListItem> GetVisorStatus()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.StockCheckStatus enStatus in (Constants.StockCheckStatus[])Enum.GetValues(typeof(Constants.StockCheckStatus)))
            {
                if ((int)enStatus == 1 || (int)enStatus == 3)
                {
                    result.Add(new SelectListItem
                    {
                        Text = (enStatus.ToString().Equals("Submit") ? Resources.Common_cshtml.Submit :
                                enStatus.ToString().Equals("Need_to_Check") ? Resources.Common_cshtml.NeedToCheck : ""),
                        Value = ((int)enStatus).ToString()
                    });
                }
            }

            return result;
        }

        private void SetRoleFlag()
        {
            Session["Role"] = null;
            ViewBag.IsAdmin = false;
            ViewBag.IsManager = false;
            ViewBag.IsController = false;
            ViewBag.IsCuttingSupervisor = false;
            ViewBag.IsYardSupervisor = false;
            ViewBag.IsWorker = false;

            switch (this.LoginUser.RoleId)
            {
                case Constants.ROLE_SYSTEMADMIN:
                    ViewBag.IsAdmin = true;
                    Session["Role"] = "Admin";
                    break;
                case Constants.ROLE_ADMIN:
                    ViewBag.IsAdmin = true;
                    Session["Role"] = "Admin";
                    break;
                case Constants.ROLE_MANAGER:
                    ViewBag.IsManager = true;
                    Session["Role"] = "Manager";
                    break;
                case Constants.ROLE_CONTROLLER:
                    ViewBag.IsController = true;
                    Session["Role"] = "Controller";
                    break;
                case Constants.ROLE_CUTTINGSUPERVISOR:
                    ViewBag.IsCuttingSupervisor = true;
                    Session["Role"] = "Supervisor";
                    break;
                case Constants.ROLE_YARDSUPERVISOR:
                    ViewBag.IsYardSupervisor = true;
                    Session["Role"] = "Supervisor";
                    break;
                case Constants.ROLE_WORKER:
                    ViewBag.IsWorker = true;
                    Session["Role"] = "Worker";
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}