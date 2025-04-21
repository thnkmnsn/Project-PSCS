using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.ModelsScreen;
using PSCS.Models;
using PSCS.Services;

namespace PSCS.Controllers
{
    public class StockCheckApproveController : BaseController
    {
        public StockCheckApproveScreen myModel
        {
            get
            {
                if (Session["StockCheckApproveScreen"] == null)
                {
                    Session["StockCheckApproveScreen"] = new StockCheckApproveScreen();
                    return (StockCheckApproveScreen)Session["StockCheckApproveScreen"];
                }
                else
                {
                    return (StockCheckApproveScreen)Session["StockCheckApproveScreen"];
                }
            }
            set { Session["StockCheckApproveScreen"] = value; }
        }

        //GET: StockCheckApprove
        public ActionResult Index()
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;

                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;
                PipeItemService objPipeItemService = new PipeItemService(this.dbEntities);

                string lang = string.Empty;
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                if (langCookie != null)
                {
                    lang = langCookie.Value;
                }
                else
                {
                    lang = "En";
                }
                this.myModel.PipeItemList = objPipeItemService.GetPipeList("", "", "",
                                          null, "", "",
                                          "", "", null, null,
                                          null, "", "OD", "ASC", lang);
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
        public ActionResult Index(string submitButton)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                switch (submitButton)
                {
                    case "OK":
                        return (OK_OnClick());
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

        private ActionResult OK_OnClick()
        {
            return View("Index", this.myModel);
        }
    }
}