using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.ModelsScreen;
using PSCS.Models;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class MonthlyCloseController : BaseController
    {
        public MonthlyCloseScreen model
        {
            get
            {
                if (Session["MonthlyCloseScreen"] == null)
                {
                    Session["MonthlyCloseScreen"] = new MonthlyCloseScreen();
                    return (MonthlyCloseScreen)Session["MonthlyCloseScreen"];
                }
                else
                {
                    return (MonthlyCloseScreen)Session["MonthlyCloseScreen"];
                }
            }

            set { Session["MonthlyCloseScreen"] = value; }
        }

        #region "Sub&Function"
        private void IntialPSC3010()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC3010";
            QueryStringList = new Dictionary<string, string>();

            this.model.AlertsType = Constants.AlertsType.None;
            this.model.Message = string.Empty;
        }

        private ActionResult MonthlyClose_OnClick(DateTime? pYearMonth,string pUserId, string pwd)
        {
            int result = 5; //Save Fail
            DateTime date = new DateTime(1753, 1, 1);
            try
            {
                if (!CheckPassword(pwd))
                {
                    this.model.MonthlyClosePassword = string.Empty;
                    this.model.AlertsType = PSCS.Common.Constants.AlertsType.Danger;
                    this.model.Message = PSCS.Resources.PSC3010_cshtml.Msg_PasswordError;
                    return View(this.model);
                }

                MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                if (pYearMonth == null)
                {
                    this.model.MonthlyClosePassword = string.Empty;
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.PSC3010_cshtml.Msg_SelectMonthlyClose;
                    return View(this.model);
                }
                else
                {
                    date = Convert.ToDateTime(pYearMonth);
                }
                result = objMonthlyCloseService.UpdateMonthlyClose(date, pUserId);

                switch (result)
                {
                    case 1:
                        //MonthlyClose complete.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Success;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_MonthlyClose_1;
                        break;
                    case 2:
                        //Please check data in database.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_MonthlyClose_2;
                        break;
                    case 3:
                        //YearMonth in MontnlyClose filter isn't Used now.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_MonthlyClose_3;
                        break;
                    case 4:
                        //Please close previous month.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_MonthlyClose_4;                
                        break;
                    default:
                        //MonthlyClose fail.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_MonthlyClose_5;
                        break;
                }
            }
            catch (Exception ex)
            {
                this.model.MonthlyClosePassword = string.Empty;
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }

            return View(this.model);
        }

        private ActionResult Restore_OnClick(DateTime? pYearMonth, string pUserId, string pwd)
        {
            int result = 6; //Save Fail
            DateTime date = new DateTime(1753, 1, 1);

            try
            {
                if (!CheckPassword(pwd))
                {
                    this.model.MonthlyClosePassword = string.Empty;
                    this.model.AlertsType = PSCS.Common.Constants.AlertsType.Danger;
                    this.model.Message = PSCS.Resources.PSC3010_cshtml.Msg_PasswordError;
                    return View(this.model);
                }

                MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                if (pYearMonth == null)
                {
                    this.model.MonthlyClosePassword = string.Empty;
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.PSC3010_cshtml.Msg_SelectMonthlyClose;
                    return View(this.model);
                }
                else
                {
                    date = Convert.ToDateTime(pYearMonth);
                }
                result = objMonthlyCloseService.RestoreMonthlyClose(date, pUserId);

                switch (result)
                {
                    case 1:
                        //Restore complete.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Success;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_Restore_1;
                        break;
                    case 2:
                        //Please check data in database.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_Restore_2;
                        break;
                    case 3:
                        //Can restore only 1 month from current month.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_Restore_3;
                        break;
                    case 4:
                        //Already restore.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_Restore_4;
                        break;
                    case 5:
                        //YearMonth in MontnlyClose filter isn't Used now.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_Restore_5;
                        break;
                    default:
                        //Restore fail.
                        this.model.MonthlyClosePassword = string.Empty;
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.PSC3010_cshtml.Msg_Restore_6;
                        break;
                }
            }
            catch (Exception ex)
            {
                this.model.MonthlyClosePassword = string.Empty;
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }

            return View(this.model);
        }

        private bool CheckPassword(string pwd)
        {
            string correctPwd = System.Configuration.ConfigurationManager.AppSettings.Get("MonthlyClosePWD");

            return correctPwd.Equals(pwd);
        }

        #endregion

        #region "PSC3010 Monthly Close"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC3010()
        {
            try
            {
                // Initial model
                IntialPSC3010();
                
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                if (objMonthlyClose != null)
                {
                    this.model.IsMonthlyClose = true;
                    this.model.IsRestore = true;

                    this.model.FilterMonthlyDate = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1); 
                    ViewBag.pMonthlyDate = Convert.ToDateTime(this.model.FilterMonthlyDate).ToString("yyyy-MM");
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.PSC2010_cshtml.MonthlyCloseError;
                    this.model.IsMonthlyClose = false;
                    this.model.IsRestore = false;

                    this.model.FilterMonthlyDate = DateTime.Today;
                    ViewBag.pMonthlyDate = Convert.ToDateTime(this.model.FilterMonthlyDate).ToString("yyyy-MM");
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Common.Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.model);
            }

            return View(this.model);
        }


        [HttpPost]
        public ActionResult PSC3010(MonthlyCloseScreen pModel, string submitButton)
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
                    ViewBag.LoginUserName = this.LoginUser.UserId;
                    switch (submitButton)
                    {
                        case "Back":
                            return RedirectToAction("PSC0100", "Menu");
                        case "MonthlyClose":
                            return MonthlyClose_OnClick(pModel.FilterMonthlyDate,this.LoginUser.UserId, pModel.MonthlyClosePassword);
                        case "Restore":
                            return Restore_OnClick(pModel.FilterMonthlyDate, this.LoginUser.UserId, pModel.MonthlyClosePassword);
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