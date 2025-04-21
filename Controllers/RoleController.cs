using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using PagedList;
using System.Globalization;
using PSCS.ModelsScreen;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class RoleController : BaseController
    {
        public RoleScreen myModel
        {
            get
            {
                if (Session["RoleScreen"] == null)
                {
                    Session["RoleScreen"] = new RoleScreen();
                    return (RoleScreen)Session["RoleScreen"];
                }
                else
                {
                    return (RoleScreen)Session["RoleScreen"];
                }
            }
            set { Session["RoleScreen"] = value; }
        }

        public RoleEdit myModelEdit
        {
            get
            {
                if (Session["RoleEdit"] == null)
                {
                    Session["RoleEdit"] = new RoleEdit();
                    return (RoleEdit)Session["RoleEdit"];
                }
                else
                {
                    return (RoleEdit)Session["RoleEdit"];
                }
            }

            set { Session["RoleEdit"] = value; }
        }

        // GET: Role
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC8040(int? page)
        {
            try
            {
                // Initial model
                IntialPSC8040();

                if (myModelEdit.SubmitMode != null)
                {
                    myModelEdit.SubmitMode = null;
                }
                else
                {
                    this.myModel.AlertsType = Common.Constants.AlertsType.None;
                    this.myModel.Message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            this.myModel.Total = this.myModel.RoleList.Count.ToString();
            ViewBag.PageList = this.myModel.RoleList.ToPagedList(pageNumber, pageSize);
            var newlist = this.myModel.RoleList.ToPagedList(pageNumber, pageSize);
            this.myModel.RoleList = newlist.ToList();

            return View("PSC8040", this.myModel);
        }
   

        [HttpPost]
        public ActionResult PSC8040(RoleScreen FilterModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;

                switch (submitButton)
                {

                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");

                    case "ClearFilter":

                        ModelState.SetModelValue("FilterRoleID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterNameTh", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterNameEn", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

                        FilterModel.FilterRoleID = "";
                        FilterModel.FilterNameTh = "";
                        FilterModel.FilterNameEn = "";

                        break;
                }

                return Filter_OnClick(FilterModel);
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC8040_Edit(string role_id)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                RoleService objRoleService = new RoleService(this.dbEntities);
                RoleEdit myModelEdit = new RoleEdit();

                if (role_id == null)
                {
                    myModelEdit.SubmitMode = "Insert";
                    return View(myModelEdit);
                }

                var result = objRoleService.GetRoleById(role_id);
                if (result != null)
                {
                    myModelEdit.InputRoleID = result.InputRoleID;
                    myModelEdit.InputNameTh = result.InputNameTh;
                    myModelEdit.InputNameEn = result.InputNameEn;
                    myModelEdit.SubmitMode = "Update";

                    return View(myModelEdit);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);

                //Set for show Message Error if EditMode value is null or not edit or Add
                if (this.myModelEdit.SubmitMode == null || !(this.myModelEdit.SubmitMode.Equals("Update") || this.myModelEdit.SubmitMode.Equals("Insert")))
                {
                    this.myModelEdit.SubmitMode = "Update";
                }

                return RedirectToAction("PSC8040", this.myModel);
            }
        }


        [HttpPost]
        public ActionResult PSC8040_Edit(string submitButton, RoleEdit editModel)
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            this.myModel.AlertsType = Common.Constants.AlertsType.None;
            this.myModel.Message = string.Empty;

            Boolean result = false;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    RoleService objRoleService = new RoleService(this.dbEntities);
                    string userId = this.LoginUser.UserId;

                    if (this.LoginUser != null)
                    {
                        switch (editModel.SubmitMode)
                        {
                            case "Insert":
                                result = objRoleService.InsertData(editModel, userId);
                                message = result ? Resources.Common_cshtml.AddSuccessMsg : Resources.Common_cshtml.AddFailMsg;
                                break;
                            case "Update":
                                result = objRoleService.UpdateData(editModel, userId);
                                message = result ? Resources.Common_cshtml.EditSuccessMsg : Resources.Common_cshtml.EditFailMsg;
                                break;
                            case "Delete":
                                result = objRoleService.DeleteData(editModel.InputRoleID);
                                message = result ? Resources.Common_cshtml.DeleteSuccessMsg : Resources.Common_cshtml.DeleteFailMsg;
                                break;
                            default:
                                result = false;
                                break;
                        }

                        // Alert Message
                        this.myModel.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                        this.myModel.Message = message;

                        return Filter_OnClick(this.myModel);
                    }
                }
                catch (Exception ex)
                {
                    this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                    this.myModel.Message = ex.Message;
                    this.PrintError(ex.Message);

                    //Set for show Message Error if EditMode value is null or not edit or Add
                    if (this.myModelEdit.SubmitMode == null || !(this.myModelEdit.SubmitMode.Equals("Update") || this.myModelEdit.SubmitMode.Equals("Insert")))
                    {
                        this.myModelEdit.SubmitMode = "Update";
                    }

                    return RedirectToAction("PSC8040", this.myModel);
                }
            }

            return View(editModel);
        }


        [HttpGet]
        public ActionResult Back()
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;

                return View("PSC8040", this.myModel);
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);

                //Set for show Message Error if EditMode value is null or not edit or Add
                if (this.myModelEdit.SubmitMode == null || !(this.myModelEdit.SubmitMode.Equals("Update") || this.myModelEdit.SubmitMode.Equals("Insert")))
                {
                    this.myModelEdit.SubmitMode = "Update";
                }

                return RedirectToAction("PSC8040", this.myModel);
            }
        }


        private void IntialPSC8040()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            InitializeActionName = "PSC8040";
            QueryStringList = new Dictionary<string, string>();

            this.myModel.FilterNameEn = "";
            this.myModel.FilterNameTh = "";
            this.myModel.FilterRoleID = "";

            this.myModel.RoleList = Search(this.myModel.FilterNameEn, this.myModel.FilterNameTh, this.myModel.FilterRoleID);
        }


        // Filter PSC8040 view
        private ActionResult Filter_OnClick(RoleScreen FilterModel)
        {
            try
            {
                RoleService objRoleService = new RoleService(this.dbEntities);

                this.myModel.RoleList = Search(FilterModel.FilterRoleID, 
                                               FilterModel.FilterNameTh,
                                               FilterModel.FilterNameEn);

                this.myModel.FilterRoleID = FilterModel.FilterRoleID;
                this.myModel.FilterNameTh = FilterModel.FilterNameTh;
                this.myModel.FilterNameEn = FilterModel.FilterNameEn;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return paging(1, FilterModel.FilterRoleID, FilterModel.FilterNameTh, FilterModel.FilterNameEn);
        }


        public List<Role> Search(string pRoleId, string pNameTh, string pNameEn)
        {
            List<Role> result = new List<Role>();

            try
            {
                result = GetRoleList(pRoleId, pNameTh, pNameEn);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        private List<Role> GetRoleList(string pRoleId, string pNameTh, string pNameEn)
        {
            RoleService objRoleService = new RoleService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            string pLanguage = langCookie != null ? langCookie.Value : "En";

            List<Role> result = objRoleService.GetRoleList(pRoleId, pNameTh, pNameEn);
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
        public ViewResult paging(int page, string RoleId, string NameTh, string NameEn)
        {
            ViewBag.RoleId = RoleId;
            ViewBag.NameTh = NameTh;
            ViewBag.NameEn = NameEn;
            this.myModel.RoleList = Search(RoleId, NameTh,NameEn);

            int pageSize = 50;
            int pageNumber = page;
            this.myModel.Total = this.myModel.RoleList.Count.ToString();
            ViewBag.PageList = this.myModel.RoleList.ToPagedList(pageNumber, pageSize);
            var newlist = this.myModel.RoleList.ToPagedList(pageNumber, pageSize);
            this.myModel.RoleList = newlist.ToList();
            return View("PSC8040", this.myModel);
        }

    }
}