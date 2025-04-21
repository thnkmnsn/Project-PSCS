using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.ModelsScreen;

namespace PSCS.Controllers
{
    public class StockCheckMonitorController : BaseController
    {
        public StockCheckMonitorScreen myModel
        {
            get
            {
                if (Session["StockCheckMonitorScreen"] == null)
                {
                    Session["StockCheckMonitorScreen"] = new StockCheckMonitorScreen();
                    return (StockCheckMonitorScreen)Session["StockCheckMonitorScreen"];
                }
                else
                {
                    return (StockCheckMonitorScreen)Session["StockCheckMonitorScreen"];
                }
            }

            set { Session["StockCheckMonitorScreen"] = value; }
        }

        public StockCheckMonitorScreenDetail myModelDetail
        {
            get
            {
                if (Session["StockCheckMonitorScreenDetail"] == null)
                {
                    Session["StockCheckMonitorScreenDetail"] = new StockCheckMonitorScreenDetail();
                    return (StockCheckMonitorScreenDetail)Session["StockCheckMonitorScreenDetail"];
                }
                else
                {
                    return (StockCheckMonitorScreenDetail)Session["StockCheckMonitorScreenDetail"];
                }
            }

            set { Session["StockCheckMonitorScreenDetail"] = value; }
        }

        // GET: StockCheckMonitorController
        [HttpGet]
        public ActionResult PSC2300()
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                InitializeActionName = "PSC2300";
                QueryStringList = new Dictionary<string, string>();
                myModel.YardList = GetYard();
                myModel.StatusList = GetStatus();
                myModel.DataList = GetStockCheckMonitor();

                if (myModel.hasMessage == false)
                {
                    this.myModel.AlertsType = Common.Constants.AlertsType.None;
                    this.myModel.Message = "";
                }

                myModel.hasMessage = false;
            }
            catch (Exception ex)
            {
                this.myModel.hasMessage = true;
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
            }

            return View(this.myModel);
        }

        [HttpPost]
        public ActionResult PSC2300(string submitButton, StockCheckMonitorScreen vm)            
        {
            try
            {
                switch (submitButton)
                {
                    case "Filter":
                        return Filter(vm);
                }
            }
            catch (Exception ex)
            {
                this.myModel.hasMessage = true;
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
            }

            return View(this.myModel);
        }

        [HttpGet]
        public ActionResult PSC2300_Detail(string row_no)
        {
            try
            {
                this.myModel.hasMessage = false;
                int rowNo = 0;
                if(!int.TryParse(row_no, out rowNo))
                {
                    this.myModel.hasMessage = true;
                    this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                    this.myModel.Message = "Don't have Description of Stock Check.";
                    return RedirectToAction("PSC2300", "StockCheckMonitor");
                }

                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;

                if (this.myModel.DataList.Count() == 0)
                {
                    this.myModel.hasMessage = true;
                    this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                    this.myModel.Message = "Don't have Description of Stock Check.";
                    return RedirectToAction("PSC2300", "StockCheckMonitor");
                }

                List<StockCheckMonitor> lst = this.myModel.DataList;
                StockCheckMonitor target = lst.Where(x => x.RowNo == rowNo).FirstOrDefault();
                if(target == null)
                {
                    this.myModel.hasMessage = true;
                    this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                    this.myModel.Message = "Don't have Description of Stock Check.";
                    return RedirectToAction("PSC2300", "StockCheckMonitor");
                }

                myModelDetail.DataList = GetStockCheckMonitorDetail(target.Yard, target.Location);
                if (myModelDetail.DataList.Count() == 0)
                {
                    this.myModel.hasMessage = true;
                    this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                    this.myModel.Message = "Don't have Description of Stock Check.";
                    return RedirectToAction("PSC2300", "StockCheckMonitor");
                }

                myModelDetail.DetailYard = target.YardName;
                myModelDetail.DetailStockCheckDate = target.StockCheckDate.ToString(Common.Constants.DATE_HYPHEN);
                myModelDetail.DetailLocation = target.LocationName;
                myModelDetail.DetailStatus = target.Status;
            }
            catch (Exception ex)
            {
                this.myModel.hasMessage = true;
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return RedirectToAction("PSC2300", "StockCheckMonitor");
            }

            return View(this.myModelDetail);
        }

        [HttpPost]
        [ActionName("PSC2300_Detail")]
        public ActionResult PSC2300_DetailPost(string submitButton, StockCheckMonitorScreenDetail vm)
        {
            try
            {
                switch (submitButton)
                {
                    case "Back":
                        return RedirectToAction("PSC2300", "StockCheckMonitor");
                }
            }
            catch (Exception ex)
            {
                this.myModel.hasMessage = true;
                this.myModelDetail.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModelDetail.Message = ex.Message;
                this.PrintError(ex.Message);
                return RedirectToAction("PSC2300", "StockCheckMonitor");
            }

            return View(this.myModelDetail);
        }

        private ActionResult Filter(StockCheckMonitorScreen vm)
        {
            // Initial model
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;

            var dataList = GetStockCheckMonitor();

            if(vm.FilterStockCheckDate != null){
                dataList = dataList.Where(x => x.StockCheckDate == vm.FilterStockCheckDate).ToList();
            }
            if (vm.FilterYard != null)
            {
                dataList = dataList.Where(x => x.Yard == vm.FilterYard).ToList();
            }
            if (vm.FilterStatus != null)
            {
                dataList = dataList.Where(x => x.Status == vm.FilterStatus).ToList();
            }

            this.myModel.DataList = dataList;
            return View(this.myModel);
        }

        private List<SelectListItem> GetYard()
        {
            List<SelectListItem> yard = new List<SelectListItem>();

            YardService service = new YardService(this.dbEntities);
            List<Yard> objYardList = service.GetYardList();
            foreach (Yard objYard in objYardList)
            {
                yard.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
            }

            return yard;
        }

        private List<SelectListItem> GetLocation(string yard)
        {
            List<SelectListItem> location = new List<SelectListItem>();

            LocationService service = new LocationService(this.dbEntities);
            List<Location> objLocationList = service.GetLocationList(yard);
            foreach (Location objLocation in objLocationList)
            {
                location.Add(new SelectListItem { Text = objLocation.Name, Value = objLocation.LocationCode.ToString() });
            }

            return location;
        }

        private List<SelectListItem> GetStatus()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.StockTakingStatus enStstus in (Constants.StockTakingStatus[])Enum.GetValues(typeof(Constants.StockTakingStatus)))
            {
                result.Add(new SelectListItem { Text = enStstus.ToString(), Value = ((int)enStstus).ToString() });
            }

            return result;
        }

        private List<StockCheckMonitor> GetStockCheckMonitor()
        {
            StockCheckMonitorService service = new StockCheckMonitorService(this.dbEntities);

            return service.GetStockCheckMonitorList();
        }

        private List<StockCheckMonitorDetail> GetStockCheckMonitorDetail(string yard, string locaton)
        {
            StockCheckMonitorService service = new StockCheckMonitorService(this.dbEntities);
            List<StockCheckMonitorDetail> lst = service.GetStockCheckMonitorDetailList();

            return lst.Where(x => x.Yard.Equals(yard) && x.Location.Equals(locaton)).ToList();
        }
    }
}