using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using PagedList;
using PSCS.ModelsScreen;
using System.Globalization;
using System.Net;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class LocationController : BaseController
    {
        public LocationScreen model
        {
            get
            {
                if (Session["ModelLocationScreen"] == null)
                {
                    Session["ModelLocationScreen"] = new LocationScreen();
                    return (LocationScreen)Session["ModelLocationScreen"];
                }
                else
                {
                    return (LocationScreen)Session["ModelLocationScreen"];
                }
            }

            set { Session["ModelLocationScreen"] = value; }
        }

        public LocationEditScreen modelEdit
        {
            get
            {
                if (Session["ModelLocationEditScreen"] == null)
                {
                    Session["ModelLocationEditScreen"] = new LocationEditScreen();
                    return (LocationEditScreen)Session["ModelLocationEditScreen"];
                }
                else
                {
                    return (LocationEditScreen)Session["ModelLocationEditScreen"];
                }
            }

            set { Session["ModelLocationEditScreen"] = value; }
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC8020(int? page)
        {
            this.model.FilterPlace = null;
            this.model.FilterYard = null;
            this.model.FilterActive = null;
            try
            {
                // Initial model
                IntialPSC8020();
                if (modelEdit.SubmitMode != null)
                {
                    modelEdit.SubmitMode = null;
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
            }
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            this.model.Total = this.model.LocationList.Count.ToString();
            ViewBag.OnePageOfProducts = this.model.LocationList.ToPagedList(pageNumber, pageSize);
            var newlist = this.model.LocationList.ToPagedList(pageNumber, pageSize);
            this.model.LocationList = newlist.ToList();

            return View("PSC8020", this.model);
            //return View(this.model);
        }


        [HttpPost]
        public ActionResult PSC8020(LocationScreen FilterModel, string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                //this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                switch (submitButton)
                {

                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");

                    case "ClearFilter":
                        ModelState.SetModelValue("FilterPlace", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterYard", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("FilterActive", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

                        FilterModel.FilterPlace = "";
                        FilterModel.FilterYard = "";
                        FilterModel.FilterName = "";
                        FilterModel.FilterActive = "";

                        break;

                }

                return Filter_OnClick(FilterModel);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC8020_Edit(string location_code)
        {
            try
            {
                // Initial View
                this.model.AlertsType = 0;
                ViewBag.LoginUserName = this.LoginUser.UserId;       
                this.modelEdit = new LocationEditScreen();
                this.modelEdit.PlaceList = GetSelectListItemListPlace();
                this.modelEdit.ActiveList = GetSelectListItemListActive();

                if (location_code == null)
                {
                    var placeFirst = modelEdit.PlaceList.FirstOrDefault().Value;
                    this.modelEdit.YardList = GetSelectListItemListYard(placeFirst);
                    this.modelEdit.SubmitMode = "Insert";

                    return View(modelEdit);
                }

                // Initial service
                LocationService objLocationService = new LocationService(this.dbEntities);
                var result = objLocationService.GetLocationById(location_code);

                if (result != null)
                {
                    this.modelEdit.InputPlace = result.InputPlace;
                    this.modelEdit.InputYard = result.InputYard;
                    this.modelEdit.InputLocation = result.InputLocation;
                    this.modelEdit.InputName = result.InputName;
                    this.modelEdit.InputActive = result.InputActive;
                    this.modelEdit.YardList = GetSelectListItemListYard(result.InputPlace);
                    this.modelEdit.SubmitMode = "Update";

                    return View(modelEdit);
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.Common_cshtml.NoDataFound;
                    this.modelEdit.HasError = true;

                    return View("PSC8020", this.model);
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
                this.modelEdit.HasError = true;

                return View("PSC8020", this.model);
            }
        }


        [HttpPost]
        public ActionResult PSC8020_Edit(string submitButton, LocationEditScreen modelEdit)
        {
            if (ModelState.IsValid && this.LoginUser != null)
            {
                try
                {
                    string userId = this.LoginUser.UserId;
                    string message = string.Empty;
                    Boolean result = false;

                    // Initial View
                    ViewBag.LoginUserName = this.LoginUser.UserId;
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;

                    LocationService objLocationService = new LocationService(this.dbEntities);

                    switch (submitButton)
                    {
                        case "Insert":
                            result = objLocationService.InsertData(modelEdit, userId);
                            message = result ? Resources.Common_cshtml.AddSuccessMsg : Resources.Common_cshtml.AddFailMsg;
                            break;
                        case "Update":
                            result = objLocationService.UpdateData(modelEdit, userId);
                            message = result ? Resources.Common_cshtml.EditSuccessMsg : Resources.Common_cshtml.EditFailMsg;
                            break;
                        case "Delete":
                            result = objLocationService.DeleteData(modelEdit.InputLocation);
                            message = result ? Resources.Common_cshtml.DeleteSuccessMsg : Resources.Common_cshtml.DeleteFailMsg;
                            break;
                        case "Back":
                            //return View("PSC8020", this.model);
                            this.model.AlertsType = 0;
                            return RedirectToAction("PSC8020", "Location");
                       
                        default:
                            result = false;
                            break;
                    }

                    // Alert Message
                    this.model.AlertsType = 0;
                    this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                    this.model.Message = message;
                    //return View("PSC8020", this.model);
                    return RedirectToAction("PSC8020","Location");
                }
                catch (Exception ex)
                {
                    this.model.AlertsType = Common.Constants.AlertsType.Danger;
                    this.model.Message = ex.Message;
                    this.PrintError(ex.Message);

                    //Set for show Message Error if EditMode value is null or not edit or Add
                    if (this.modelEdit.SubmitMode == null || !(this.modelEdit.SubmitMode.Equals("Update") || this.modelEdit.SubmitMode.Equals("Insert")))
                    {
                        this.modelEdit.SubmitMode = "Update";
                    }
                    return RedirectToAction("PSC8020", "Location");
                    //return View("PSC8020", this.model);
                }
            }

            modelEdit.PlaceList = GetSelectListItemListPlace();
            modelEdit.YardList = GetSelectListItemListYard();
            modelEdit.ActiveList = GetSelectListItemListActive();
            //return RedirectToAction("PSC8020", "Location");
            return View(modelEdit);
        }
        

        private void IntialPSC8020()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            InitializeActionName = "PSC8020";
            QueryStringList = new Dictionary<string, string>();

            // initial dropdownlist
            this.model.PlaceList = GetSelectListItemListPlace();
            this.model.YardList = GetSelectListItemListYard();
            this.model.ActiveList = GetSelectListItemListActive();

            var placeFirst = this.model.PlaceList.FirstOrDefault().Value;
            this.model.LocationList = this.GetLocationList(placeFirst, "", "", "");
        }


        // Filter PSC8020 view
        private ActionResult Filter_OnClick(LocationScreen FilterModel)
        {
            try
            {
                this.model.LocationList = this.GetLocationList(FilterModel.FilterPlace,
                                                              FilterModel.FilterYard,
                                                              FilterModel.FilterName,
                                                              FilterModel.FilterActive);

                this.model.FilterPlace = FilterModel.FilterPlace;
                this.model.FilterYard = FilterModel.FilterYard;
                this.model.FilterName = FilterModel.FilterName;
                this.model.FilterActive = FilterModel.FilterActive;
                this.model.Message = string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return paging(1, FilterModel);
            //return View("PSC8020", this.model);
        } 


        private List<Location> GetLocationList(string pFilterPlace, string pFilterYard, string pFilterName, string pFilterActive)
        {
            LocationService objLocationService = new LocationService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            string pLanguage = langCookie != null ? langCookie.Value : "En";
            List<byte> active = new List<byte>();

            if (string.IsNullOrEmpty(pFilterActive))
            {
                active = GetActiveList();
            }
            else
            {
                active = new List<byte>();
                active.Add(Convert.ToByte(pFilterActive));
            }

            List<Location> result = objLocationService.GetLocationList(pFilterPlace, pFilterYard, pFilterName, active);
            return result;
        }
        

        // Ajax
        public ActionResult ddlChangePlace(string Place)
        {
            List<Yard> objYardList = new List<Yard>();
            List<SelectListItem> mYardList = new List<SelectListItem>();

            try
            {
                YardService objYardService = new YardService(this.dbEntities);
                objYardList = objYardService.GetYardList(Place).ToList();

                // this make ddl display correctly, although refresh this view
                foreach (Yard objYard in objYardList)
                {
                    mYardList.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
                }

                this.model.YardList = mYardList;
                return Json(objYardList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        // ddl Place datasource
        private List<SelectListItem> GetSelectListItemListPlace()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            PlaceService objPlaceService = new PlaceService(this.dbEntities);
            var objPlaceList = objPlaceService.GetPlaceList();

            foreach (Place objPlace in objPlaceList)
            {
                result.Add(new SelectListItem { Text = objPlace.Name, Value = objPlace.PlaceID.ToString() });
            }

            return result;
        }

        // ddl Yard datasource
        private List<SelectListItem> GetSelectListItemListYard(string pPlaceID = "")
        {
            List<SelectListItem> result = new List<SelectListItem>();
            YardService objYardService = new YardService(this.dbEntities);
            var objYardList = objYardService.GetYardList(pPlaceID);

            foreach (Yard objYard in objYardList)
            {
                result.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
            }

            return result;
        }

        // ddl Active
        private List<SelectListItem> GetSelectListItemListActive()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.ActiveStatus enStatus in (Constants.ActiveStatus[])Enum.GetValues(typeof(Constants.ActiveStatus)))
            {
                result.Add(new SelectListItem
                {
                    Text = (enStatus.ToString().Equals("Active") ? Resources.Common_cshtml.Active :
                                                        enStatus.ToString().Equals("InActive") ? Resources.Common_cshtml.InActive : "")
                                                ,
                    Value = ((int)enStatus).ToString()
                });
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

        public ViewResult paging(int page, LocationScreen FilterModel)
        {
            this.model.AlertsType = 0;
            ViewBag.FilterPlace = FilterModel.FilterPlace;
            ViewBag.FilterYard = FilterModel.FilterYard;
            ViewBag.FilterName = FilterModel.FilterName;
            ViewBag.FilterActive = FilterModel.FilterActive;
            this.model.LocationList = GetLocationList(FilterModel.FilterPlace, FilterModel.FilterYard, FilterModel.FilterName, FilterModel.FilterActive);

            int pageSize = 50;
            int pageNumber = page;
            this.model.Total = this.model.LocationList.Count.ToString();
            ViewBag.OnePageOfProducts = this.model.LocationList.ToPagedList(pageNumber, pageSize);
            var newlist = this.model.LocationList.ToPagedList(pageNumber, pageSize);
            this.model.LocationList = newlist.ToList();

            return View("PSC8020", this.model);
        }
    }
}