using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class MenuController : BaseController
    {
        // GET: Menu
        [NoDirectAccess]
        public ActionResult PSC0100()
        {
            if (this.LoginUser != null)
            {

                if (this.LoginUser.RoleId == Common.Constants.ROLE_WORKER)
                {
                    ViewBag.ShowMaster = false;
                    ViewBag.RoleID = null;

                    Session.Abandon();
                    return RedirectToAction("PSC0010", "Account");
                }

                if (this.LoginUser.RoleId == Common.Constants.ROLE_ADMIN)
                {
                    ViewBag.ShowMaster = true;
                    ViewBag.RoleID = this.LoginUser.RoleId;
                    InitializeActionName = "PSC0100";
                    QueryStringList = new Dictionary<string, string>();
                    return View();
                }
                else
                {
                    ViewBag.ShowMaster = false;
                    ViewBag.RoleID = this.LoginUser.RoleId;
                    InitializeActionName = "PSC0100";
                    QueryStringList = new Dictionary<string, string>();
                    return View();
                }
            }
            else
            {
                ViewBag.ShowMaster = false;
                ViewBag.RoleID = null;

                Session.Abandon();
                return RedirectToAction("PSC0010", "Account");

            }            
           
           
        }

        [NoDirectAccess]
        public ActionResult PSC0110()
        {
            InitializeActionName = "PSC0100";
            QueryStringList = new Dictionary<string, string>();
            return View();
        }
    }
}