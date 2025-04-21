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
    public class SyncPipeStockDataController : BaseController
    {
        public SyncPipeStockDataScreen myModel
        {
            get
            {
                if (Session["SyncPipeStockDataScreen"] == null)
                {
                    Session["SyncPipeStockDataScreen"] = new SyncPipeStockDataScreen();
                    return (SyncPipeStockDataScreen)Session["SyncPipeStockDataScreen"];
                }
                else
                {
                    return (SyncPipeStockDataScreen)Session["SyncPipeStockDataScreen"];
                }
            }
            set { Session["SyncPipeStockDataScreen"] = value; }
        }

        // GET: SyncReceiveData
        public ActionResult PSC3200()
        {
            try
            {
                InitializeActionName = "PSC3200";
                QueryStringList = new Dictionary<string, string>();
                this.myModel.AlertsType = Common.Constants.AlertsType.None;
                this.myModel.Message = string.Empty;
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
    }
}