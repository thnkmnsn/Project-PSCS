using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using PSCS.ModelsScreen;

namespace PSCS.Controllers
{
    public class AccountController : BaseController
    {
        public ChangePasswordScreen changePasswordModel
        {
            get
            {
                if (Session["ModelChangePassword"] == null)
                {
                    Session["ModelChangePassword"] = new ChangePasswordScreen();
                    return (ChangePasswordScreen)Session["ModelChangePassword"];
                }
                else
                {
                    return (ChangePasswordScreen)Session["ModelChangePassword"];
                }
            }

            set { Session["ModelChangePassword"] = value; }
        }


        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PSC0010()
        {
            User model = new User();
            ViewBag.LoginPage = true;
            InitializeActionName = "PSC0010";
            QueryStringList = new Dictionary<string, string>();
            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        //public ActionResult Autherize(PSCS.Models.PSC8040_M_USER pUser)
        public ActionResult Autherize(PSCS.Models.User pUser)
        {
            if (this.LoginUser == null)
            {
                this.IsLogInPage = true;
                AccountService objService = new AccountService(this.dbEntities);
            
                User objUser = objService.GetUserLogin(pUser.UserId, pUser.Password);
                if (objUser.IsLogin)
                {
                    this.LoginUser = objUser;
                    this.SetRoleFlag();
                    ViewBag.MyAdmin = ViewBag.IsAdmin;
                    this.CurrentPlace = Convert.ToInt32(Constants.AMT_PLACE);
                    ChangeLanguage(Enum.GetName(typeof(Constants.LanguageCookie), Convert.ToInt32(objUser.Language)));

                    return RedirectToAction("PSC0100", "Menu");
                }
                else
                {
                    ViewBag.LoginPage = true;
                    pUser.LoginErrorMessage = Resources.PSC0010_cshtml.LoginFailMsg;
                    this.PrintError("Incorrect Username or Password");
                    return View("PSC0010", pUser);
                }
            }
            else
            {
                ViewBag.LoginPage = true;
                pUser.LoginErrorMessage = Resources.PSC0010_cshtml.LoginSameUserFailMsg;
                return View("PSC0010", pUser);
            }
                
        }


        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("PSC0010", "Account");
        }

        [SessionExpire]
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC0011()
        {
            try
            {  // Initial model
                InitializeActionName = "PSC0011";
                QueryStringList = new Dictionary<string, string>();
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.changePasswordModel.AlertsType = Common.Constants.AlertsType.None;
                this.changePasswordModel.Message = string.Empty;
                AccountService objService = new AccountService(this.dbEntities);
                ChangePasswordScreen model = new ChangePasswordScreen();

             

                User result = objService.GetUserById(this.LoginUser.UserId);
                if (result != null)
                {
                    return View(this.changePasswordModel);
                }
            }
            catch (Exception ex)
            {
                this.changePasswordModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.changePasswordModel.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.changePasswordModel);
            }

            return View(this.changePasswordModel);
        }


        [SessionExpire]
        [HttpPost]
        public ActionResult PSC0011(ChangePasswordScreen editModel, string submitButton)
        {
            // Initial model   
            ViewBag.LoginUserName = this.LoginUser.UserId;
            this.changePasswordModel.AlertsType = Common.Constants.AlertsType.None;
            this.changePasswordModel.Message = string.Empty;

            switch (submitButton)
            {
                case "Back":
                    return RedirectToAction("PSC0100", "Menu");
            }

            if (ModelState.IsValid)
            {
                try
                {                  

                    AccountService objService = new AccountService(this.dbEntities);
                    string userId = this.LoginUser.UserId;

                    var result = objService.ChangePassword(editModel, userId);
                    if (result == 1)
                    {
                        this.changePasswordModel.AlertsType = Common.Constants.AlertsType.Success;
                        this.changePasswordModel.Message = PSCS.Resources.Common_cshtml.SaveSuccessMsg;
                    }
                    else if (result == 0)
                    {
                        this.changePasswordModel.AlertsType = Common.Constants.AlertsType.Danger;
                        this.changePasswordModel.Message = PSCS.Resources.PSC0011_cshtml.CurrentPasswordnotMatchMsg;
                    }
                    else
                    {
                        this.changePasswordModel.AlertsType = Common.Constants.AlertsType.Danger;
                        this.changePasswordModel.Message = PSCS.Resources.Common_cshtml.SaveFailMsg;
                    }

                    ModelState.Clear();
                    this.changePasswordModel.CurrentPassword = string.Empty;
                    this.changePasswordModel.NewPassword = string.Empty;
                    this.changePasswordModel.ConfirmPassword = string.Empty;

                    return View("PSC0011", this.changePasswordModel);
                }
                catch (Exception ex)
                {
                    this.changePasswordModel.AlertsType = Common.Constants.AlertsType.Danger;
                    this.changePasswordModel.Message = ex.Message;
                    this.PrintError(ex.Message);

                    return View(this.changePasswordModel);
                }
            }

            return View(this.changePasswordModel);
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
                    ViewBag.IsYardSupervisorF = true;
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
    }


}