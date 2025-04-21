using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;
using PSCS.Services;
using System.IO;
using PSCS.Managers;

namespace PSCS.Controllers
{
    public class BaseController : Controller
    {
        private Boolean _IsLogInPage;

       public Boolean IsLogInPage
        {
            get
            {
                return _IsLogInPage;
            }
            set
            {
                _IsLogInPage = value;
            }
        }

        public string Language
        {
            get
            {
                return (string)Session["Language"];
            }
            set
            {
                Session["Language"] = value;
            }
        }

        public string ConnetionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings.Get("ConnetionString");
            }
        }

        public string ERPDEV01ConnetionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings.Get("ERPDEV01ConnetionString");
            }
        }

        public PSCSEntities dbEntities
        {
            get
            {
                dbEntitiesConnect();
                return (PSCSEntities)Session["PSCSEntities"];
            }
        }

        public ModelERPDEV01.AMT_HistoryEntities dbERPDEV01Entities
        {
            get
            {
                dbERPDEV01EntitiesConnect();
                return (ModelERPDEV01.AMT_HistoryEntities)Session["ERPDEV01Entities"];
            }
        }

        public User LoginUser
        {
            get
            {
                return (User)Session["LoginUser"];
            }
            set
            {
                //CoolMenu.MenuHelper.UserName = value.UserName; 
                Session["LoginUserName"] = value.UserName;
                Session["LoginUser"] = value;
            }
        }

       

        public int CurrentPlace
        {
            get
            {
                return (int)Session["CurrentPlace"];
            }
            set
            {
                Session["CurrentPlace"] = value;
            }
        }

        protected string InitializeActionName
        {
            get
            {
                return Session["InitializeActionName"].ToString();
            }
            set
            {
                Session["InitializeActionName"] = value;
            }
        }

        protected Dictionary<string, string> QueryStringList
        {
            get
            {
                return (Dictionary<string, string>)Session["QueryStringList"];
            }
            set
            {
                Session["QueryStringList"] = value;
            }
        }

        private Boolean dbEntitiesConnect()
        {
            Boolean result = false;

            DatabaseService objDbService = new DatabaseService();

            Session["PSCSEntities"] = objDbService.Connect(this.ConnetionString);
            
            return result;
        }

        private Boolean dbERPDEV01EntitiesConnect()
        {
            Boolean result = false;

            ERPDEV01DatabaseService objDbService = new ERPDEV01DatabaseService();

            Session["ERPDEV01Entities"] = objDbService.Connect(this.ERPDEV01ConnetionString);
            
            return result;
        }
        public void PrintError(string msg)
        {
            PSCS_Common.LabelIssueLogger.SetConfigure(Server.MapPath("~/log4net.config"));
            PSCS_Common.LabelIssueLogger.PrintError(msg);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = LanguageManager.GetDefaultLanguage();
                }
            }
            this.Language = lang;
            CoolMenu.MenuHelper.Lanuage = this.Language;
            new LanguageManager().SetLanguage(lang);
            return base.BeginExecuteCore(callback, state);
        }

        [SessionExpire]
        public ActionResult ChangeLanguage(string lang)
        {
            this.Language = lang;
            CoolMenu.MenuHelper.Lanuage = this.Language;
            new LanguageManager().SetLanguage(lang);
            RedirectToRouteResult result =  RedirectToAction(InitializeActionName);
            foreach (KeyValuePair<string, string> kv in QueryStringList)
            {
                result.RouteValues.Add(kv.Key, kv.Value);
            }
            return result;
        }

        [SessionExpire]
        public ActionResult SessionTimeoutCheck()
        {
            // dummy
            return new JsonResult { Data = new { Message = "SessionTimeoutCheck" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; ;
        }
    }
}