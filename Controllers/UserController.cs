using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using PagedList;
using PSCS.Excels;
using System.IO;
using System.Globalization;
using PSCS.ModelsScreen;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class UserController : BaseController
    {
        public UserScreen myModel
        {
            get
            {
                if (Session["UserScreen"] == null)
                {
                    Session["UserScreen"] = new UserScreen();
                    return (UserScreen)Session["UserScreen"];
                }
                else
                {
                    return (UserScreen)Session["UserScreen"];
                }
            }
            set { Session["UserScreen"] = value; }
        }

        public UserScreenEdit myModelEdit
        {
            get
            {
                if (Session["UserScreenEdit"] == null)
                {
                    Session["UserScreenEdit"] = new UserScreenEdit();
                    return (UserScreenEdit)Session["UserScreenEdit"];
                }
                else
                {
                    return (UserScreenEdit)Session["UserScreenEdit"];
                }
            }
            set { Session["UserScreenEdit"] = value; }
        }

        // GET: User
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC8030(int? page)
        {
            try
            {
                IntialPSC8030();
                if (myModelEdit.EditMode != null)
                {
                    myModelEdit.EditMode = null;
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
            this.myModel.Total = this.myModel.UserList.Count.ToString();
            ViewBag.PageList = this.myModel.UserList.ToPagedList(pageNumber, pageSize);
            var newlist = this.myModel.UserList.ToPagedList(pageNumber, pageSize);
            this.myModel.UserList = newlist.ToList();

            return View(this.myModel);
        }

        [HttpPost]
        public ActionResult PSC8030(string submitButton, string FilterUserID, string FilterUserName, string FilterLanguage, string FilterRole, string FilterActive
                                   , string ModalUserId, string ModalUserName, string ModalPassword, string ModalLanguageSelected, string ModalRoleSelected, string ModalActiveSelected, string hiddenActionType)
        {
            try
            {
                switch (submitButton)
                {

                    case "Filter":                      
                        return (Filter_OnClick(FilterUserID, FilterUserName, FilterLanguage, FilterRole, FilterActive));

                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");

                    case "ClearFilter":
                       
                        ModelState.SetModelValue("FilterUserID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterUserName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterLanguage", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterRole", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterActive", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

                        FilterUserID = "";
                        FilterUserName = "";
                        FilterLanguage = "";
                        FilterRole = "";
                        FilterActive = "";

                        return (Filter_OnClick(FilterUserID, FilterUserName, FilterLanguage, FilterRole, FilterActive));


                    default:
                        return View(this.myModel);
                }
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
        public ActionResult PSC8030_Edit(string id)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;

                this.myModelEdit = new UserScreenEdit();
                this.myModelEdit.LanguageList = GetSelectListItemListLanguage();
                this.myModelEdit.RoleList = GetSelectListItemListRole();
                this.myModelEdit.ActiveList = GetSelectListItemListActive();

                if (string.IsNullOrEmpty(id))
                {
                    this.myModelEdit.EditMode = "Add";
                    return View(this.myModelEdit);
                }

                string strUserId = id.Substring(3);
                UserService objUserService = new UserService(this.dbEntities);
                var result = objUserService.GetUserById(strUserId);

                if (result != null)
                {
                    this.myModelEdit.EditUserID = result.EditUserID;
                    this.myModelEdit.EditUserName = result.EditUserName;
                    this.myModelEdit.EditPassword = result.EditPassword;
                    this.myModelEdit.EditLanguageSelected = result.EditLanguageSelected;
                    this.myModelEdit.EditActiveSelected = result.EditActiveSelected;
                    this.myModelEdit.EditRoleIdSelected = result.EditRoleIdSelected;
                    this.myModelEdit.EditMode = "Edit";

                    return View(this.myModelEdit);
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
                if (this.myModelEdit.EditMode == null || !(this.myModelEdit.EditMode.Equals("Edit") || this.myModelEdit.EditMode.Equals("Add")))
                {
                    this.myModelEdit.EditMode = "Edit";
                }

                return RedirectToAction("PSC8030", "User");
            }
        }

        [HttpPost]
        public ActionResult PSC8030_Edit(string submitButton, UserScreenEdit editModel)
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            if (ModelState.IsValid || submitButton.Equals("Back"))
            {
                try
                {
                    switch (submitButton)
                    {
                        case "Save":
                            SaveModal_OnClick(editModel.EditUserID, editModel.EditUserName, editModel.EditPassword, editModel.EditLanguageSelected, editModel.EditRoleIdSelected, editModel.EditActiveSelected, editModel.EditMode);
                            return RedirectToAction("PSC8030", "User");

                        case "Delete":
                            Delete_OnClick(editModel.EditUserID);
                            return RedirectToAction("PSC8030", "User");

                        case "Back":
                            return RedirectToAction("PSC8030", "User");

                        case "Export":
                            return Export(editModel.EditUserID);

                        default:
                            editModel.LanguageList = GetSelectListItemListLanguage();
                            editModel.RoleList = GetSelectListItemListRole();
                            editModel.ActiveList = GetSelectListItemListActive();

                            return View(editModel);
                    }
                }

                catch (Exception ex)
                {
                    this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                    this.myModel.Message = ex.Message;
                    this.PrintError(ex.Message);
                    return RedirectToAction("PSC8030", "User");
                }
            }

            editModel.LanguageList = GetSelectListItemListLanguage();
            editModel.RoleList = GetSelectListItemListRole();
            editModel.ActiveList = GetSelectListItemListActive();
            return View(editModel);
        }

        public FileStreamResult Export(string UserId)
        {
            this.myModel.AlertsType = Constants.AlertsType.None;
            this.myModel.Message = string.Empty;

            UserEditExcel excel = new UserEditExcel(this.dbEntities, this.LoginUser);
            excel.UserId = UserId; //StockTakingDate;

            MemoryStream ms = new MemoryStream();
            if (!excel.Export(ms))
            {
                Response.Clear();
                this.myModel.AlertsType = Constants.AlertsType.Danger;
                this.myModel.Message = "Unable to export user data. " + excel.GetErrorMesssage();
                PrintError(this.myModel.Message);
                //return View("PSC2510", this.myModel);
            }

            DateTime dt = DateTime.Today;
            Response.AddHeader("Content-Disposition", "attachment; filename=User_" + UserId + "_" + dt.ToString(Constants.DATE_FILENAME) + Constants.EXCEL_EXTENSION);

            return new FileStreamResult(ms, Constants.EXCEL_CONTENTTYPE);
        }

       

        private ActionResult Filter_OnClick(string pFilterUserID, string pFilterUserName, string pFilterLanguage, string pFilterRole, string pFilterActive)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;

                this.myModel.UserList = Search(pFilterUserID, pFilterUserName, pFilterLanguage, pFilterRole, pFilterActive);
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return paging(1, pFilterUserID, pFilterUserName, pFilterLanguage, pFilterRole, pFilterActive);
        }

        private ActionResult SaveModal_OnClick(string pModalUserId, string pModalUserName, string pModalPassword, string pModalLanguageSelected, int pModalRoleIdSelected, string pModalActiveSelected, string pActionType)
        {
            Boolean result = false;
            try
            {
                UserService objUserService = new UserService(this.dbEntities);

                if (LoginUser != null)
                {
                    if(pModalPassword == null)
                    {
                        pModalPassword = "";
                    }
                    if (pActionType.Contains("Add"))
                    {
                        
                        result = objUserService.Insert(pModalUserId, pModalUserName, pModalPassword, pModalLanguageSelected, pModalRoleIdSelected, pModalActiveSelected, this.LoginUser);
                    }
                    else if (pActionType.Contains("Edit"))
                    {
                        result = objUserService.Update(pModalUserId, pModalUserName, pModalPassword, pModalLanguageSelected, pModalRoleIdSelected, pModalActiveSelected, this.LoginUser);
                    }

                    if (result)
                    {
                        this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                        if (pActionType.Contains("Add"))
                        {
                            this.myModel.Message = PSCS.Resources.Common_cshtml.AddSuccessMsg;
                        }
                        else
                        {
                            this.myModel.Message = PSCS.Resources.Common_cshtml.EditSuccessMsg;
                        }
                    }
                    else
                    {
                        this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                        if (pActionType.Contains("Add"))
                        {
                            this.myModel.Message = PSCS.Resources.Common_cshtml.AddFailMsg;
                        }
                        else
                        {
                            this.myModel.Message = PSCS.Resources.Common_cshtml.EditFailMsg;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC8030", this.myModel);
        }

        private ActionResult Delete_OnClick(string pModalUserId)
        {
            Boolean result = false;
            try
            {
                UserService objUserService = new UserService(this.dbEntities);

                if (LoginUser != null)
                {

                    result = objUserService.Delete(pModalUserId);

                    if (result)
                    {
                        this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                        this.myModel.Message = PSCS.Resources.Common_cshtml.DeleteSuccessMsg;
                    }
                    else
                    {
                        this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                        this.myModel.Message = PSCS.Resources.Common_cshtml.DeleteFailMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("PSC8030", this.myModel);
        }

        private void IntialPSC8030()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            InitializeActionName = "PSC8030";
            QueryStringList = new Dictionary<string, string>();

            this.myModel.LanguageList = GetSelectListItemListLanguage();
            this.myModel.RoleList = GetSelectListItemListRole();
            this.myModel.ActiveList = GetSelectListItemListActive();

            this.myModel.UserList = GetUserList("", "", "", "", GetActiveList());
        }

        private List<User> Search(string pFilterUserID, string pFilterUserName, string pFilterLanguage, string pFilterRole, string pFilterActive)
        {
            List<User> result = null;
            List<byte> active = null;

            try
            {
                if (pFilterActive == string.Empty)
                {
                    active = GetActiveList();
                }
                else
                {
                    active = new List<byte>();
                    active.Add(Convert.ToByte(pFilterActive));
                }

                result = GetUserList(pFilterUserID, pFilterUserName, pFilterLanguage, pFilterRole, active);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<SelectListItem> GetSelectListItemListActive()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.ActiveStatus enStatus in (Constants.ActiveStatus[])Enum.GetValues(typeof(Constants.ActiveStatus)))
            {
                result.Add(new SelectListItem { Text = (enStatus.ToString().Equals("Active")? Resources.Common_cshtml.Active : 
                                                        enStatus.ToString().Equals("InActive") ? Resources.Common_cshtml.InActive : "" )
                                                , Value = ((int)enStatus).ToString() });
            }
            return result;
        }

        private List<SelectListItem> GetSelectListItemListLanguage()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.LanguageStatus enStatus in (Constants.LanguageStatus[])Enum.GetValues(typeof(Constants.LanguageStatus)))
            {
                result.Add(new SelectListItem { Text = (enStatus.ToString().Equals("English") ? Resources.Common_cshtml.English :
                                                        enStatus.ToString().Equals("Thai") ? Resources.Common_cshtml.Thai : "")
                                               , Value = ((int)enStatus).ToString() });
            }
            return result;
        }

        private List<SelectListItem> GetSelectListItemListRole()
        {
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            List<SelectListItem> result = new List<SelectListItem>();
            RoleService objRoleService = new RoleService(this.dbEntities);
            if (langCookie != null && langCookie.Value.Equals("Th"))
            {
                foreach (Role objRole in objRoleService.GetRoleList())
                {
                    result.Add(new SelectListItem { Text = objRole.NameTh, Value = objRole.RoleID.ToString() });
                }
            }
            else
            {
                foreach (Role objRole in objRoleService.GetRoleList())
                {
                    result.Add(new SelectListItem { Text = objRole.NameEn, Value = objRole.RoleID.ToString() });
                }
            }
            return result;
        }

        private List<byte> GetActiveList()
        {
            List<byte> result = new List<byte>();
            foreach (Constants.ActiveStatus enStatus in (Constants.ActiveStatus[])Enum.GetValues(typeof(Constants.ActiveStatus)))
            {
                result.Add((byte)enStatus);
            }
            return result;
        }


        private List<User> GetUserList(string pFilterUserID, string pFilterUserName, string pFilterLanguage, string pFilterRole, List<byte> pFilterActive)
        {
            string lang = string.Empty;
            List<User> result = new List<User>();
            UserService objUserService = new UserService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                lang = "En";
            }
            result = objUserService.GetUserList(pFilterUserID, pFilterUserName, pFilterLanguage, pFilterRole, pFilterActive, lang);
            return result;
        }

        public ViewResult paging(int page, string UserID, string UserName, string Language, string Role, string Active)
        {
            ViewBag.UserID = UserID;
            ViewBag.UserName = UserName;
            ViewBag.Language = Language;
            ViewBag.Role = Role;
            ViewBag.Active = Active;

            if (UserID == null) UserID = "";
            if (UserName == null) UserName = "";
            if (Language == null) Language = "";
            if (Role == null) Role = "";
            if (Active == null) Active = "";


            this.myModel.UserList = Search(UserID, UserName,
                                     Language, Role, Active);
            int pageSize = 50;
            int pageNumber = page;
            this.myModel.Total = this.myModel.UserList.Count.ToString();
            ViewBag.PageList = this.myModel.UserList.ToPagedList(pageNumber, pageSize);
            var newlist = this.myModel.UserList.ToPagedList(pageNumber, pageSize);
            this.myModel.UserList = newlist.ToList();
            return View("PSC8030", this.myModel);
        }


    }
}