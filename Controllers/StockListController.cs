using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Services;
using PSCS.Models;
using PSCS.ModelsScreen;
using System.IO;
using System.Linq.Expressions;
using PSCS.Common;
using PSCS.Excels;

using PagedList;
using System.Globalization;
using System.Net;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class StockListController : BaseController
    {
        public StockListScreen Model
        {
            get
            {
                if (Session["ModelStockListScreen"] == null)
                {
                    Session["ModelStockListScreen"] = new StockListScreen();
                    return (StockListScreen)Session["ModelStockListScreen"];
                }
                else
                {
                    return (StockListScreen)Session["ModelStockListScreen"];
                }
            }
            set { Session["ModelStockListScreen"] = value; }
        }

        // GET: StockList
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2010(int? page)
        {
            //StockList model = new StockList();

            try
            {

                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                InitializeActionName = "PSC2010";
                QueryStringList = new Dictionary<string, string>();
                this.Model.AlertsType = Constants.AlertsType.None;
                this.Model.Message = string.Empty;

                this.Model.FilterReceiveDate = null;

                if( page == null )
                {
                    MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                    MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                    if (objMonthlyClose != null)
                    {
                        //Get DropdownData
                        this.Model.YardList = GetYardList();
                        DdlYard_Change(null);
                        this.Model.GradeList = GetGradeList();
                        this.Model.MakerList = GetMakerList();
                        this.Model.StandardList = GetSelectListItemListStandard();
                        this.Model.SortByList = GetSortByList();
                        this.Model.OrderByList = GetOrderByList();

                        this.Model.GerabPOList = GetLotAttributesList();
                        this.Model.SingaporeList = GetLotAttributesList();
                        this.Model.C21SHL1List = GetLotAttributesList();

                        //DefaultFiltter
                        this.Model.FilterYearMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                        this.Model.FilterOrderBy = Constants.OrderBy.OD.ToString();
                        this.Model.FilterSortBy = Constants.SortBy.ASC.ToString();
                        ViewBag.pYearMonth = Model.FilterYearMonth.ToString("yyyy-MM");

                        //Get DatainGrid
                        var dataList = GetStockList(Model.FilterYearMonth, null, "", "", "", "", "", "", "", "", "", "", "", Model.FilterOrderBy, Model.FilterSortBy, Model.FilterIsShowZero);

                        // Get Paging
                        int pageSize = 50;
                        int pageNumber = (page ?? 1);
                        ViewBag.OnePageOfProducts = dataList.ToPagedList(pageNumber, pageSize);
                        this.Model.DataList = dataList;
                        this.Model.Total = dataList.Count().ToString();
                        decimal? decTotalWeight = dataList.Sum(x => x.TotalWeight);
                        this.Model.TotalWeigth = decTotalWeight != null ? ((decimal)decTotalWeight).ToString("#,##0.00") : "0.00";
                        var newlist = dataList.ToPagedList(pageNumber, pageSize);
                        this.Model.DisplayDataList = newlist.ToList();
                        this.Model.FilterIsShowZero = true;
                    }
                    else
                    {
                        this.Model.AlertsType = Constants.AlertsType.Danger;
                        this.Model.Message = Resources.PSC2010_cshtml.MonthlyCloseError;
                    }
                }
                else
                {
                    if (this.Model.DataList != null && this.Model.DataList.Count > 0)
                    {
                        // Get Paging
                        int pageSize = 50;
                        int pageNumber = (page ?? 1);
                        ViewBag.OnePageOfProducts = this.Model.DataList.ToPagedList(pageNumber, pageSize);
                        decimal? decTotalWeight = this.Model.DataList.Sum(x => x.TotalWeight);
                        this.Model.TotalWeigth = decTotalWeight != null ? ((decimal)decTotalWeight).ToString("#,##0.00") : "0.00";
                        var newlist = this.Model.DataList.ToPagedList(pageNumber, pageSize);
                        this.Model.DisplayDataList = newlist.ToList();
                        this.Model.FilterIsShowZero = true;
                    }
                }

                return View(this.Model);
            }
            catch (Exception ex)
            {
                Model.AlertsType = Constants.AlertsType.Danger;
                Model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(Model);
            }
        }

        [HttpPost]
        public ActionResult PSC2010(string submitButton, StockListScreen ModelFilter)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;

                switch (submitButton)
                {
                    case "Filter":
                        return (Filter_OnClick(ModelFilter));

                    case "Export":
                        return Export(ModelFilter);

                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");

                    case "ClearFilter":
                        //ModelState.Clear();
                        MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                        MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();

                        if(objMonthlyClose != null)
                        {
                            DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                            string strFMonth = dFMonth.ToString("yyyy-MM");

                            ModelState.SetModelValue("FilterYearMonth", new ValueProviderResult(strFMonth, strFMonth, CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterReceiveDate", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterYardID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterLocationID", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterItemCode", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterDescription", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterHeatNo", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterOD", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterWT", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterLength", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterGrade", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterMaker", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterStandardName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterOrderBy", new ValueProviderResult("OD", "OD", CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("FilterSortBy", new ValueProviderResult("ASC", "ASC", CultureInfo.InvariantCulture));

                            ModelFilter.FilterYearMonth = dFMonth;
                            ModelFilter.FilterReceiveDate = null;
                            ModelFilter.FilterYardID = null;
                            ModelFilter.FilterLocationID = null;
                            ModelFilter.FilterItemCode = null;
                            ModelFilter.FilterDescription = null;
                            ModelFilter.FilterHeatNo = null;
                            ModelFilter.FilterOD = null;
                            ModelFilter.FilterWT = null;
                            ModelFilter.FilterLength = null;
                            ModelFilter.FilterGrade = null;
                            ModelFilter.FilterMaker = null;
                            ModelFilter.FilterStandardName = null;
                            ModelFilter.FilterOrderBy = Constants.OrderBy.OD.ToString();
                            ModelFilter.FilterSortBy = Constants.SortBy.ASC.ToString();
                            ViewBag.pYearMonth = ModelFilter.FilterYearMonth.ToString("yyyy-MM");
                        }


                        return (Filter_OnClick(ModelFilter));

                    default:
                        return (DdlYard_Change(ModelFilter.FilterYardID));
                }
            }
            catch (Exception ex)
            {
                DdlYard_Change(ModelFilter.FilterYardID);
                Model.AlertsType = Constants.AlertsType.Danger;
                Model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(Model);
            }
        }

        //Export
        public ActionResult Export(StockListScreen ModelFilter)
        {
            this.Model.AlertsType = Constants.AlertsType.None;
            this.Model.Message = string.Empty;

            StockListExcel excel = new StockListExcel(this.dbEntities, this.LoginUser);

            //Get DatainGrid
            DateTime dateFrom = new DateTime(ModelFilter.FilterYearMonth.Year, ModelFilter.FilterYearMonth.Month, 1);
            var dataList = GetStockList(dateFrom, ModelFilter.FilterReceiveDate, ModelFilter.FilterYardID, ModelFilter.FilterLocationID,
                                        ModelFilter.FilterItemCode, ModelFilter.FilterDescription, ModelFilter.FilterHeatNo, ModelFilter.FilterOD, ModelFilter.FilterWT,
                                        ModelFilter.FilterLength, ModelFilter.FilterGrade, ModelFilter.FilterMaker, ModelFilter.FilterStandardName, 
                                        ModelFilter.FilterOrderBy, ModelFilter.FilterSortBy,
                                        ModelFilter.FilterIsShowZero);

            if(dataList != null && dataList.Count > 0)
            {
                excel.stockLists = dataList; 

                excel.ReceiveDate = ModelFilter.FilterReceiveDate;

                MemoryStream ms = new MemoryStream();
                if (!excel.Export(ms))
                {
                    Response.Clear();
                    PrintError(this.Model.Message);
                }
                DateTime dt = DateTime.Today;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "Stock List_"
                    + this.Model.FilterYearMonth.ToString("yyyy-MM") + dt.ToString(Constants.DATE_FILENAME) + Constants.EXCEL_EXTENSION);

                 new FileStreamResult(ms, Constants.EXCEL_CONTENTTYPE);
            }
            else
            {
                this.Model.AlertsType = Constants.AlertsType.Danger;
                this.Model.Message = "Unable to export stock list data. ";
            }

            //return View(this.Model);
            return paging(1, this.Model);
        }


        private ActionResult Filter_OnClick(StockListScreen ModelFilter)
        {
            try
            {
                this.Model.AlertsType = Constants.AlertsType.None;
                this.Model.Message = string.Empty;

                DdlYard_Change(ModelFilter.FilterYardID);
                
                this.Model.DataList = GetStockList(ModelFilter.FilterYearMonth, ModelFilter.FilterReceiveDate, ModelFilter.FilterYardID, ModelFilter.FilterLocationID,
                                                   ModelFilter.FilterItemCode, ModelFilter.FilterDescription, ModelFilter.FilterHeatNo, ModelFilter.FilterOD, ModelFilter.FilterWT,
                                                   ModelFilter.FilterLength, ModelFilter.FilterGrade, ModelFilter.FilterMaker, ModelFilter.FilterStandardName, 
                                                   ModelFilter.FilterOrderBy, ModelFilter.FilterSortBy,
                                                   ModelFilter.FilterIsShowZero);
                
                   

                if (!string.IsNullOrEmpty(ModelFilter.FilterGerabPO))
                {
                    this.Model.DataList = this.Model.DataList.Where(x => x.Gerab_PO == Convert.ToInt32(ModelFilter.FilterGerabPO)).ToList();
                }

                if (!string.IsNullOrEmpty(ModelFilter.FilterSingapore))
                {
                    this.Model.DataList = this.Model.DataList.Where(x => x.Singapore == Convert.ToInt32(ModelFilter.FilterSingapore)).ToList();
                }

                if (!string.IsNullOrEmpty(ModelFilter.FilterC21SHL1))
                {
                    this.Model.DataList = this.Model.DataList.Where(x => x.C21_SHL1 == Convert.ToInt32(ModelFilter.FilterC21SHL1)).ToList();
                }

                this.Model.Total = this.Model.DataList.Count().ToString();
                decimal? decTotalWeight = this.Model.DataList.Sum(x => x.TotalWeight);
                this.Model.TotalWeigth = decTotalWeight != null ? ((decimal)decTotalWeight).ToString("#,##0.00") : "0.00";
                ViewBag.data = Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return View("PSC2010", this.Model);
            return paging(1, ModelFilter);
        }

        public ActionResult DdlYard_Change(string pYardList)
        {
            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);
                List<SelectListItem> objLocationList = GetLocationList(pYardList).ToList();
                this.Model.LocationList = objLocationList;
                return Json(objLocationList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetStockListDetail(string yard, string location, string itemcode, string heatno)
        {
            string lang = string.Empty;
            try
            {
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                if (langCookie != null)
                {
                    lang = langCookie.Value;
                }
                else
                {
                    lang = "En";
                }

                // Initial service
                StockListService objStockListService = new StockListService(this.dbEntities);
                var result = objStockListService.GetStockListDetailPatial(yard, location, itemcode, heatno, lang);

                StockListPatialScreen patailModel = new StockListPatialScreen();
                patailModel.DataList = result;

                return PartialView("_PSC2010Partial", patailModel);
            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<SelectListItem> GetLotAttributesList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                result.Add(new SelectListItem { Text = "Yes", Value = "1" });
                result.Add(new SelectListItem { Text = "No", Value = "0" });
                result.Add(new SelectListItem { Text = "Un Check", Value = "2" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private List<SelectListItem> GetYardList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                YardService objYardService = new YardService(this.dbEntities);
                foreach (Yard objYard in objYardService.GetYardList())
                {
                    result.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private List<SelectListItem> GetLocationList(string pYardList)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);
                foreach (Location objLocation in objLocationService.GetLocationList(pYardList))
                {
                    result.Add(new SelectListItem { Text = objLocation.Name, Value = objLocation.LocationCode.ToString() });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private List<SelectListItem> GetGradeList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                GradeService objGradeService = new GradeService(this.dbEntities);
                foreach (Grade objGrade in objGradeService.GetGradeList())
                {
                    result.Add(new SelectListItem { Text = objGrade.GradeName, Value = objGrade.GradeCode.ToString() });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<SelectListItem> GetMakerList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                MakerService objMakerService = new MakerService(this.dbEntities);
                foreach (Maker objMaker in objMakerService.GetMakerList())
                {
                    result.Add(new SelectListItem { Text = objMaker.MakerName, Value = objMaker.MakerCode.ToString() });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        // ddl Standard datasource
        private List<SelectListItem> GetSelectListItemListStandard()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            StandardService objStandardService = new StandardService(this.dbEntities);
            var objStandardList = objStandardService.GetStandardList();

            foreach (Standard objStandard in objStandardList)
            {
                result.Add(new SelectListItem { Text = objStandard.StandardName, Value = objStandard.StandardCode.ToString() });
            }

            return result;
        }

        private List<StockList> GetStockList(DateTime pYearMonth, Nullable<DateTime> pReceiveDate, string pYardID, string pLocationID, string pItemCode, string pDescription,
                                            string pHeatNo, string pOD, string pWT, string pLength, string pGrade, string pMaker, string pStandardName, 
                                            string pOrderBy, string pSortBy,
                                            Boolean IsShowZero)
        {
            List<StockList> result = new List<StockList>();
            try
            {
                StockListService objStockListService = new StockListService(this.dbEntities);
                result = objStockListService.GetStockList(pYearMonth, pReceiveDate, pYardID, pLocationID, pItemCode,
                                                           pDescription, pHeatNo, pOD, pWT, pLength, pGrade, pMaker, pStandardName, 
                                                           pOrderBy, pSortBy,
                                                           IsShowZero);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<SelectListItem> GetSortByList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                foreach (Constants.SortBy enStatus in (Constants.SortBy[])Enum.GetValues(typeof(Constants.SortBy)))
                {
                    result.Add(new SelectListItem
                    {
                        Text = (enStatus.ToString().Equals(Constants.SortBy.ASC.ToString()) ? Resources.PSC2010_cshtml.ASC :
                                enStatus.ToString().Equals(Constants.SortBy.DESC.ToString()) ? Resources.PSC2010_cshtml.DESC : ""),
                        Value = (enStatus).ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private List<SelectListItem> GetOrderByList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                foreach (Constants.OrderBy enStatus in (Constants.OrderBy[])Enum.GetValues(typeof(Constants.OrderBy)))
                {
                    result.Add(new SelectListItem
                    {
                        Text = (enStatus.ToString().Equals(Constants.OrderBy.OD.ToString()) ? Resources.PSC2010_cshtml.OD :
                                enStatus.ToString().Equals(Constants.OrderBy.WT.ToString()) ? Resources.PSC2010_cshtml.WT :
                                enStatus.ToString().Equals(Constants.OrderBy.LT.ToString()) ? Resources.PSC2010_cshtml.LT : ""),
                        //Text = (enStatus.ToString()),
                        Value = (enStatus).ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Ajax
        public ActionResult ddlChangePipYard(string Yard)
        {
            List<Location> objLocationList = new List<Location>();
            List<SelectListItem> mLocationList = new List<SelectListItem>();
            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);
                objLocationList = objLocationService.GetLocationList(Yard);

                // this make ddl display correctly, although refresh this view
                foreach (Location objLc in objLocationList)
                {
                    mLocationList.Add(new SelectListItem { Text = objLc.Name, Value = objLc.Name.ToString() });
                }

                this.Model.LocationList = mLocationList;
                return Json(objLocationList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        //add paging to view ex.add another one in OnClick_Filter
        public ViewResult paging(int page, StockListScreen pModel)
        {
            //ViewBag.pYardID = pModel.FilterYardID;
            //ViewBag.pLocationID = pModel.FilterLocationID;
            //ViewBag.pItemCode = pModel.FilterItemCode;
            //ViewBag.pHeatNo = pModel.FilterHeatNo;
            //ViewBag.pDescription = pModel.FilterDescription;
            //ViewBag.pOD = pModel.FilterOD;
            //ViewBag.pWT = pModel.FilterWT;
            //ViewBag.pLength = pModel.FilterLength;
            //ViewBag.pGrade = pModel.FilterGrade;
            //ViewBag.pMaker = pModel.FilterMaker;
            //ViewBag.pStandardName = pModel.FilterStandardName;
            //ViewBag.pOrderBy = pModel.FilterOrderBy;
            //ViewBag.pSortBy = pModel.FilterSortBy;
            ////ViewBag.pYearMonthFrom = Model.FilterYearMonthFrom = Model.FilterYearMonthFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ////ViewBag.pYearMonthTo = Model.FilterYearMonthTo = Model.FilterYearMonthTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            ////Model.FilterYearMonth = pModel.FilterYearMonth;
            ////ViewBag.pYearMonth = pModel.FilterYearMonth.ToString("yyyy-MM");

            ////DateTime dateFrom = new DateTime(pModel.FilterYearMonth.Year, pModel.FilterYearMonth.Month, 1);

            //MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
            //MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
            //if (objMonthlyClose != null)
            //{
            //    //DefaultFiltter
            //    Model.FilterYearMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
            //    ViewBag.pYearMonth = pModel.FilterYearMonth.ToString("yyyy-MM");

            //    //DateTime dateFrom = new DateTime(pModel.FilterYearMonth.Year, pModel.FilterYearMonth.Month, 1);
            //}



            //var dataList = GetStockList(Model.FilterYearMonth, pModel.FilterYardID, pModel.FilterLocationID,
            //                            pModel.FilterItemCode, pModel.FilterDescription, pModel.FilterHeatNo, pModel.FilterOD, pModel.FilterWT,
            //                            pModel.FilterLength, pModel.FilterGrade, pModel.FilterMaker, pModel.FilterStandardName, pModel.FilterOrderBy, pModel.FilterSortBy,
            //                            pModel.FilterIsShowZero);

            //int pageSize = 50;
            //int pageNumber = page;

            //ViewBag.OnePageOfProducts = dataList.ToPagedList(pageNumber, pageSize);
            //this.Model.Total = dataList.Count().ToString();

            //var newlist = dataList.ToPagedList(pageNumber, pageSize);
            //Model.DataList = newlist.ToList();
           
            if (this.Model.DataList != null)
            {
                
                //this.Model.DataList.

                int pageSize = 50;
                int pageNumber = page;

                ViewBag.OnePageOfProducts = this.Model.DataList.ToPagedList(pageNumber, pageSize);

                var newlist = this.Model.DataList.ToPagedList(pageNumber, pageSize);
                this.Model.DisplayDataList = newlist.ToList();
                
            }
            
            return View("PSC2010", this.Model);
        }

        ////add paging to view ex.add another one in OnClick_Filter
        //public ViewResult paging(int page, string pYardID, string pLocationID, string pItemCode,string pHeatNo,string pDescription,
        //                              decimal? pOD, decimal? pWT, decimal? pLT,string pGrade,string pMaker, string pStandardName, string pOrderBy,string pSortBy,
        //                              decimal? pYearMonthFrom 
        //                                 )
        //{
        //    ViewBag.pYardID = pYardID;
        //    ViewBag.pLocationID = pLocationID;
        //    ViewBag.pItemCode = pItemCode;
        //    ViewBag.pHeatNo = pHeatNo;
        //    ViewBag.pDescription = pDescription;
        //    ViewBag.pOD = pOD;
        //    ViewBag.pWT = pWT;
        //    ViewBag.pLength = pLT;
        //    ViewBag.pGrade = pGrade;
        //    ViewBag.pMaker = pMaker;
        //    ViewBag.pStandardName = pStandardName;
        //    ViewBag.pOrderBy = pOrderBy;
        //    ViewBag.pSortBy = pSortBy;
        //    //ViewBag.pYearMonthFrom = Model.FilterYearMonthFrom = Model.FilterYearMonthFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //    //ViewBag.pYearMonthTo = Model.FilterYearMonthTo = Model.FilterYearMonthTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        //    //Model.FilterYearMonth = pModel.FilterYearMonth;
        //    //ViewBag.pYearMonth = pModel.FilterYearMonth.ToString("yyyy-MM");

        //    //DateTime dateFrom = new DateTime(pModel.FilterYearMonth.Year, pModel.FilterYearMonth.Month, 1);

        //    MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
        //    MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
        //    if (objMonthlyClose != null)
        //    {
        //        //DefaultFiltter
        //        Model.FilterYearMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
        //        ViewBag.pYearMonth = pYearMonthFrom;

        //        //DateTime dateFrom = new DateTime(pModel.FilterYearMonth.Year, pModel.FilterYearMonth.Month, 1);
        //    }



        //    var dataList = GetStockList(Model.FilterYearMonth, ViewBag.pYardID, ViewBag.pLocationID,
        //                                ViewBag.pItemCode, ViewBag.pDescription, ViewBag.pHeatNo, ViewBag.pOD, ViewBag.pWT,
        //                                ViewBag.pLength, ViewBag.pGrade, ViewBag.pMaker, ViewBag.pStandardName, ViewBag.pOrderBy, ViewBag.pSortBy);

        //    int pageSize = 50;
        //    int pageNumber = page;

        //    ViewBag.OnePageOfProducts = dataList.ToPagedList(pageNumber, pageSize);
        //    this.Model.Total = dataList.Count().ToString();

        //    var newlist = dataList.ToPagedList(pageNumber, pageSize);
        //    Model.DataList = newlist.ToList();

        //    return View("PSC2010", Model);
        //}
    }
}