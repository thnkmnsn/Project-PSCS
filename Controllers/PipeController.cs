using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Models;
using PSCS.ModelERPDEV01;
using PSCS.Services;
using PagedList;
using PSCS.ModelsScreen;

using System.Globalization;
namespace PSCS.Controllers
{
    [SessionExpire]
    public class PipeController : BaseController
    {
        public bool CurrentFilter = true;

        public PipeItemScreen PipeModel
        {
            get
            {
                if (Session["PipeItemScreen"] == null)
                {
                    Session["PipeItemScreen"] = new PipeItemScreen();
                    return (PipeItemScreen)Session["PipeItemScreen"];
                }
                else
                {
                    return (PipeItemScreen)Session["PipeItemScreen"];
                }
            }

            set { Session["PipeItemScreen"] = value; }
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC8010(int? page)
        {
            try
            {
                    IntialPSC8010();

                if (PipeModel.HasError)
                {
                    PipeModel.HasError = false;
                }
                else
                {
                    this.PipeModel.AlertsType = Common.Constants.AlertsType.None;
                    this.PipeModel.Message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.PipeModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.PipeModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.PipeModel);
            }


            int pageSize = 50;
            int pageNumber = (page ?? 1);

            this.PipeModel.Total = this.PipeModel.PipeList.Count.ToString();
            ViewBag.OnePageOfProducts = this.PipeModel.PipeList.ToPagedList(pageNumber, pageSize);
            var newlist = this.PipeModel.PipeList.ToPagedList(pageNumber, pageSize);
            this.PipeModel.DisplayPipeList = newlist.ToList();

            return View("PSC8010",this.PipeModel);
        }


        [HttpPost]
        public ActionResult PSC8010(PipeItemScreen FilterModel, string submitButton)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Initial model
                    ViewBag.LoginUserName = this.LoginUser.UserId;
                    this.PipeModel.AlertsType = Common.Constants.AlertsType.None;
                    this.PipeModel.Message = string.Empty;

                    switch (submitButton)
                    {

                        case "Back":
                            return RedirectToAction("PSC0100", "Menu");

                        case "Sync":
                            Boolean result =  Sync_OnClick();
                            ClearFilter_OnClick(FilterModel);
                            this.PipeModel.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            this.PipeModel.Message = "Get Item Master from S/L Complete."; ;
                            break;

                        case "Update":
                            Boolean result1 = Update_OnClick();
                            ClearFilter_OnClick(FilterModel);
                            this.PipeModel.AlertsType = result1 ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            this.PipeModel.Message = "Update Item Master from S/L Complete."; ;
                            break;

                        case "ClearFilter":
                            ClearFilter_OnClick(FilterModel);
                            break;
                    }

                    return Filter_OnClick(FilterModel.FilterItemCode,FilterModel.FilterDescription, FilterModel.FilterHeatNo,
                        FilterModel.FilterUnitWeight, FilterModel.FilterMaterialName, FilterModel.FilterStandardName,
                        FilterModel.FilterGradeName, FilterModel.FilterShapeName, FilterModel.FilterOD, FilterModel.FilterWT,
                        FilterModel.FilterLT, FilterModel.FilterMakerName, FilterModel.FilterOrderBy, FilterModel.FilterSortBy);
                }
                catch (Exception ex)
                {
                    this.PipeModel.AlertsType = Common.Constants.AlertsType.Danger;
                    this.PipeModel.Message = ex.Message;
                    this.PrintError(ex.Message);

                    //return View(this.PipeModel);
                    return Filter_OnClick(FilterModel.FilterItemCode, FilterModel.FilterDescription, FilterModel.FilterHeatNo,
                        FilterModel.FilterUnitWeight, FilterModel.FilterMaterialName, FilterModel.FilterStandardName,
                        FilterModel.FilterGradeName, FilterModel.FilterShapeName, FilterModel.FilterOD, FilterModel.FilterWT,
                        FilterModel.FilterLT, FilterModel.FilterMakerName, FilterModel.FilterOrderBy, FilterModel.FilterSortBy);
                }
            }

            return View(this.PipeModel);
        }

        private Boolean Sync_OnClick()
        {
            Boolean result = false;
            GetNewPipeFromSyteLineService objGetNewPipeFromSyteLineService = null;
            string strPipeError = string.Empty;

            try
            {
                objGetNewPipeFromSyteLineService = new GetNewPipeFromSyteLineService(this.dbERPDEV01Entities);
                List<PipeItemData> objPipeItemDataList = objGetNewPipeFromSyteLineService.GetPipeItemAndHeatNoFromSyteLineList();
                if (objPipeItemDataList != null)
                { 
                    PipeItemService objPipeItemService = new PipeItemService(this.dbEntities);
                    result = objPipeItemService.UpdatePipeItem(objPipeItemDataList, this.LoginUser.UserId);
                }
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }
        private Boolean Update_OnClick()
        {
            Boolean result = false;
            GetNewPipeFromSyteLineService objGetNewPipeFromSyteLineService = null;
            string strPipeError = string.Empty;

            try
            {
                objGetNewPipeFromSyteLineService = new GetNewPipeFromSyteLineService(this.dbERPDEV01Entities);
                List<PipeItemData> objPipeItemDataList = objGetNewPipeFromSyteLineService.GetPipeItemAndHeatNoFromSyteLineList();
                if (objPipeItemDataList != null)
                {
                    PipeItemService objPipeItemService = new PipeItemService(this.dbEntities);
                    result = objPipeItemService.UpdatePipeItem1(objPipeItemDataList, this.LoginUser.UserId);
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        [HttpGet]
        public ActionResult PSC8010_Detail(string item_code, string heat_no)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;

                PipeItemService objPipeService = new PipeItemService(this.dbEntities);
                PipeItemDetail newModel = new PipeItemDetail();
                //newModel.IsBundedList= GetSelectListItemIsBundedList();

                var result = objPipeService.GetPipeListById(item_code, heat_no);
                if (result != null)
                {
                    newModel = result;

                    //GetAttribute
                    PipeItemService objPipeItemService = new PipeItemService(this.dbEntities);
                    List<PipeAttribute> attributeList = objPipeItemService.GetAttributeList(item_code, heat_no);
                    newModel.PipeAttributeList = attributeList;

                    return View(newModel);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                this.PipeModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.PipeModel.Message = ex.Message;
                this.PrintError(ex.Message);
                this.PipeModel.HasError = true;

                return View("PSC8010", this.PipeModel);
            }
        }


        [HttpGet]
        public ActionResult Back()
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.PipeModel.AlertsType = Common.Constants.AlertsType.None;
                this.PipeModel.Message = string.Empty;

                return View("PSC8010", this.PipeModel);
            }
            catch (Exception ex)
            {
                this.PipeModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.PipeModel.Message = ex.Message;
                this.PrintError(ex.Message);
                this.PipeModel.HasError = true;

                return View("PSC8010", this.PipeModel);
            }
        }


        // Filter PSC8010 view
        private ActionResult Filter_OnClick(string pItemCode,string pDescription, string pHeatNo,
                                      decimal? pWeight, string pMaterialName, string pStandardName,
                                      string pGradeName, string pShapeName, decimal? pOD, decimal? pWT,
                                      decimal? pLT, string pMakerName, string pOrderBy, string pSortBy)
        {
            try
            {

                this.PipeModel.PipeList = Search(pItemCode, pDescription, pHeatNo,
                                       pWeight, pMaterialName, pStandardName,
                                       pGradeName, pShapeName, pOD, pWT,
                                       pLT, pMakerName, pOrderBy, pSortBy);
                ViewBag.data = this.PipeModel.PipeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
      
            return paging(1);
        }

        private void ClearFilter_OnClick(PipeItemScreen FilterModel)
        {
            try
            {

                //ModelState.Clear();
                ModelState.SetModelValue("FilterItemCode", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterDescription", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterHeatNo", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterUnitWeight", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterMaterialName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterStandardName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterGradeName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterShapeName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterOD", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterWT", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterLT", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterMakerName", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterOrderBy", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
                ModelState.SetModelValue("FilterSortBy", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

                FilterModel.FilterItemCode = null;
                FilterModel.FilterDescription = null;
                FilterModel.FilterHeatNo = null;
                FilterModel.FilterUnitWeight = null;
                FilterModel.FilterMaterialName = null;
                FilterModel.FilterStandardName = null;
                FilterModel.FilterGradeName = null;
                FilterModel.FilterShapeName = null;
                FilterModel.FilterOD = null;
                FilterModel.FilterWT = null;
                FilterModel.FilterLT = null;
                FilterModel.FilterMakerName = null;
                FilterModel.FilterOrderBy = "OD";
                FilterModel.FilterSortBy = "ASC";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private List<SelectListItem> GetSelectListItemListIsBundeled()
        //{
        //    List<SelectListItem> result = new List<SelectListItem>();
        //    foreach (Constants.IsBundeledStatus enBundeled in (Constants.IsBundeledStatus[])Enum.GetValues(typeof(Constants.IsBundeledStatus)))
        //    {
        //        result.Add(new SelectListItem { Text = enBundeled.ToString(), Value = ((int)enBundeled).ToString() });
        //    }

        //    return result;
        //}

        private void IntialPSC8010()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            InitializeActionName = "PSC8010";
            QueryStringList = new Dictionary<string, string>();
            this.PipeModel.MaterialList = GetSelectListItemListMaterial();
            this.PipeModel.StandardList = GetSelectListItemListStandard();
            this.PipeModel.GradeList = GetSelectListItemListGrade();
            this.PipeModel.ShapeList = GetSelectListItemLShape();
            this.PipeModel.MakerList = GetSelectListItemListMaker();
            this.PipeModel.OrderByList = GetSelectListItemListOrderBy();
            this.PipeModel.SortByList = GetSelectListItemListSortBy();
            //this.PipeModel.IsBundedList = GetSelectListItemIsBundedList();

            this.PipeModel.FilterOrderBy = Common.Constants.OrderBy.OD.ToString();
            this.PipeModel.FilterSortBy = Common.Constants.SortBy.ASC.ToString();

            this.PipeModel.PipeList = GetPipeList("", "", "",
                                      null, "", "",
                                      "", "", null, null,
                                      null, "", this.PipeModel.FilterOrderBy, this.PipeModel.FilterSortBy);
        }

        private List<PipeItem> Search(string pItemCode,string pDescription, string pHeatNo,
                                      decimal? pWeight, string pMaterialName, string pStandardName,
                                      string pGradeName, string pShapeName, decimal? pOD, decimal? pWT,
                                      decimal? pLT, string pMakerName, string pOrderBy, string pSortBy)
        {
            List<PipeItem> result = null;
            //List<byte> bundeledStatus = null;

            try
            {
                //if (pIsBundled == null)
                //{
                //    bundeledStatus = GetIsBundeledStatus();
                //}
                //else
                //{
                //    bundeledStatus = new List<byte>();
                //    bundeledStatus.Add(Convert.ToByte(pIsBundled));
                //}

                result = GetPipeList(pItemCode, pDescription, pHeatNo,
                                      pWeight, pMaterialName, pStandardName,
                                      pGradeName, pShapeName, pOD, pWT,
                                      pLT, pMakerName, pOrderBy, pSortBy);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }

        // ddl  Material datasource
        private List<SelectListItem> GetSelectListItemListMaterial()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            MaterialService objMaterialService = new MaterialService(this.dbEntities);
            var objMaterialList = objMaterialService.GetMaterialList();

            foreach (Material objMaterial in objMaterialList)
            {
                result.Add(new SelectListItem { Text = objMaterial.MaterialName, Value = objMaterial.MaterialCode.ToString() });
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

        // ddl Grade datasource
        private List<SelectListItem> GetSelectListItemListGrade()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            GradeService objGradeService = new GradeService(this.dbEntities);
            var objGradeList = objGradeService.GetGradeList();

            foreach (Grade objGrade in objGradeList)
            {
                result.Add(new SelectListItem { Text = objGrade.GradeName, Value = objGrade.GradeCode.ToString() });
            }

            return result;
        }

        // ddl Shape datasource
        private List<SelectListItem> GetSelectListItemLShape()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            ShapeService objShapeService = new ShapeService(this.dbEntities);
            var objShapeList = objShapeService.GetShapeList();

            foreach (Shape objShape in objShapeList)
            {
                result.Add(new SelectListItem { Text = objShape.ShapeName, Value = objShape.ShapeCode.ToString() });
            }

            return result;
        }

        // ddl Maker datasource
        private List<SelectListItem> GetSelectListItemListMaker()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            MakerService objMakerService = new MakerService(this.dbEntities);
            var objMakerList = objMakerService.GetMakerList();

            foreach (Maker objMaker in objMakerList)
            {
                result.Add(new SelectListItem { Text = objMaker.MakerName, Value = objMaker.MakerCode.ToString() });
            }

            return result;
        }

        private List<SelectListItem> GetSelectListItemListSortBy()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.SortBy enStatus in (Constants.SortBy[])Enum.GetValues(typeof(Constants.SortBy)))
            {
                result.Add(new SelectListItem
                {
                    Text = (enStatus.ToString().Equals(Common.Constants.SortBy.ASC.ToString()) ? Resources.PSC2010_cshtml.ASC :
                            enStatus.ToString().Equals(Common.Constants.SortBy.DESC.ToString()) ? Resources.PSC2010_cshtml.DESC : ""),
                    //Text = (enStatus.ToString()),
                    Value = (enStatus).ToString()
                });

            }
            return result;
        }

        private List<SelectListItem> GetSelectListItemListOrderBy()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.OrderBy enStatus in (Constants.OrderBy[])Enum.GetValues(typeof(Constants.OrderBy)))
            {
                result.Add(new SelectListItem
                {
                    Text = (enStatus.ToString().Equals(Common.Constants.OrderBy.OD.ToString()) ? Resources.PSC2010_cshtml.OD :
                            enStatus.ToString().Equals(Common.Constants.OrderBy.WT.ToString()) ? Resources.PSC2010_cshtml.WT :
                            enStatus.ToString().Equals(Common.Constants.OrderBy.LT.ToString()) ? Resources.PSC2010_cshtml.LT : ""),
                    //Text = (enStatus.ToString()),
                    Value = (enStatus).ToString()
                });
            }

            return result;
        }
        //private List<SelectListItem> GetSelectListItemIsBundedList()
        //{
        //    List<SelectListItem> result = new List<SelectListItem>();
        //    foreach (Constants.IsBundeledStatus enStatus in (Constants.ActiveStatus[])Enum.GetValues(typeof(Constants.IsBundeledStatus)))
        //    {
        //        result.Add(new SelectListItem
        //        {
        //            Text = (enStatus.ToString().Equals("No") ? Resources.Common_cshtml.No :
        //                                                enStatus.ToString().Equals("Yes") ? Resources.Common_cshtml.Yes : "")
        //                                        ,
        //            Value = ((int)enStatus).ToString()
        //        });
        //    }
        //    return result;
        //}

        //private List<byte> GetIsBundeledStatus()
        //{
        //    List<byte> result = new List<byte>();
        //    foreach (Constants.IsBundeledStatus enStatus in (Constants.ActiveStatus[])Enum.GetValues(typeof(Constants.IsBundeledStatus)))
        //    {
        //        result.Add((byte)enStatus);
        //    }
        //    return result;
        //}

        private List<PipeItem> GetPipeList(string pItemCode,string pDescription, string pHeatNo,
                                      decimal? pWeight, string pMaterialName, string pStandardName,
                                      string pGradeName, string pShapeName, decimal? pOD, decimal? pWT,
                                      decimal? pLT, string pMakerName, string pOrderBy, string pSortBy)
        {
            string lang = string.Empty;
            List<PipeItem> result = new List<PipeItem>();
            PipeItemService objPipeItemService = new PipeItemService(this.dbEntities);
            HttpCookie langCookie = Request.Cookies["PSCS_culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                lang = "En";
            }
            result = objPipeItemService.GetPipeList(pItemCode,pDescription, pHeatNo,
                                      pWeight, pMaterialName, pStandardName,
                                      pGradeName, pShapeName, pOD, pWT,
                                      pLT, pMakerName, pOrderBy, pSortBy, lang);
            return result;
        }

        //public ViewResult paging(int page, string pItemCode,string pDescription, string pHeatNo,
        //                              decimal? pWeight, string pMaterialName, string pStandardName,
        //                              string pGradeName, string pShapeName, decimal? pOD, decimal? pWT,
        //                              decimal? pLT, string pMakerName, string pOrderBy, string pSortBy)
        //{
        //    ViewBag.pItemCode = pItemCode;
        //    ViewBag.pDescription = pDescription;
        //    ViewBag.pHeatNo = pHeatNo;
        //    ViewBag.pWeight = pWeight;
        //    ViewBag.pMaterialName = pMaterialName;
        //    ViewBag.pStandardName = pStandardName;
        //    ViewBag.pGradeName = pGradeName;
        //    ViewBag.pShapeName = pShapeName;
        //    ViewBag.pOD = pOD;
        //    ViewBag.pWT = pWT;
        //    ViewBag.pLT = pLT;
        //    ViewBag.pMakerName = pMakerName;
        //    ViewBag.pOrderBy = pOrderBy;
        //    ViewBag.pSortBy = pSortBy;
        //    this.PipeModel.PipeList = GetPipeList(pItemCode,pDescription, pHeatNo,
        //                              pWeight, pMaterialName, pStandardName,
        //                              pGradeName, pShapeName, pOD, pWT,
        //                              pLT, pMakerName, pOrderBy, pSortBy);

        //    int pageSize = 50;
        //    int pageNumber = page;
        //    this.PipeModel.Total = this.PipeModel.PipeList.Count.ToString();
        //    ViewBag.OnePageOfProducts = this.PipeModel.PipeList.ToPagedList(pageNumber, pageSize);
        //    var newlist = this.PipeModel.PipeList.ToPagedList(pageNumber, pageSize);
        //    this.PipeModel.PipeList = newlist.ToList();
        //    return View("PSC8010", this.PipeModel);
        //}

        public ViewResult paging(int page)
        {
            if(this.PipeModel.PipeList != null)
            {
                int pageSize = 50;
                int pageNumber = page;

                this.PipeModel.Total = this.PipeModel.PipeList.Count.ToString();
                ViewBag.OnePageOfProducts = this.PipeModel.PipeList.ToPagedList(pageNumber, pageSize);
                var newlist = this.PipeModel.PipeList.ToPagedList(pageNumber, pageSize);
                this.PipeModel.DisplayPipeList = newlist.ToList();
            }

            return View("PSC8010", this.PipeModel);
        }


    }
}