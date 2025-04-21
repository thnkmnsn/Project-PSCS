using PSCS.Common;
using PSCS.Excels;
using PSCS.Models;
using PSCS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using PSCS.ModelsScreen;

namespace PSCS.Controllers
{
    public class StockTakingController : BaseController
    {
       public StockTakingScreen myModel
        {
            get
            {
                if (Session["StockTakingScreen"] == null)
                {
                    Session["StockTakingScreen"] = new StockTakingScreen();
                    return (StockTakingScreen)Session["StockTakingScreen"];
                }
                else
                {
                    return (StockTakingScreen)Session["StockTakingScreen"];
                }
            }
            set { Session["StockTakingScreen"] = value; }
        }

        [HttpGet]
        public ActionResult PSC2510()
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                InitializeActionName = "PSC2510";
                QueryStringList = new Dictionary<string, string>();
                this.myModel.AlertsType = Constants.AlertsType.None;
                this.myModel.Message = string.Empty;

                SetRoleFlag();

                YardService objYardService = new YardService(this.dbEntities);
                StockTakingService objStockTakingService = new StockTakingService(this.dbEntities);

                foreach (Yard objYard in objYardService.GetYardList())
                {
                    this.myModel.YardList.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
                }

                this.myModel.StatusList = GetSelectListItemListStatus();

                //GetMaxStockTakingDate
                this.myModel.StockTakingDate = objStockTakingService.GetStockTakingMaxDate();
                //GetStockTakingList
                this.myModel.DataList = GetStockTaking(this.myModel.StockTakingDate, "", "", "", "", "", "", "", "", "", GetStatusList());

                SetDisableButton("");
            }
            catch(Exception ex)
            {
                this.myModel.AlertsType = Constants.AlertsType.Danger ;
                this.myModel.Message = ex.Message;
                return View(this.myModel);
            }

            return View(this.myModel);
        }
               

        [HttpPost]
        public ActionResult PSC2510(string submitButton,
                                    DateTime StockTakingDate, string FilterYardID, string FilterLocationID, string FilterStatus,
                                    string ItemCode, string HeatNo, string OD, string WT, string Lenght, string Grade, string Maker,
                                    string hiddenRowNo, string actualQTY, FormCollection collection)
        {
            //Boolean result = false;
            //ViewBag.StartupScript = "alert('hello world!');";
            

            try
            {
                this.myModel.AlertsType = Constants.AlertsType.None;
                this.myModel.Message = string.Empty;
                ViewBag.LoginUserName = this.LoginUser.UserId;

                SetRoleFlag();

                switch (submitButton)
                {
                    case "Filter":
                        return Filter_OnClick(StockTakingDate, FilterYardID, FilterLocationID, ItemCode, HeatNo, OD, WT, Lenght, Grade, Maker, FilterStatus);

                    case "SaveModal":
                        return SaveModal_OnClick(hiddenRowNo, actualQTY);

                    case "Save":
                        Save_OnClick(myModel.DataList);
                        return Filter_OnClick(StockTakingDate, FilterYardID, FilterLocationID, ItemCode, HeatNo, OD, WT, Lenght, Grade, Maker, FilterStatus);

                    case "Submit":
                        Submit_OnClick(myModel.DataList, collection);
                        return Filter_OnClick(StockTakingDate, FilterYardID, FilterLocationID, ItemCode, HeatNo, OD, WT, Lenght, Grade, Maker, FilterStatus);

                    case "Adjust":
                        Adjust_OnClick(myModel.DataList, collection);
                        return Filter_OnClick(StockTakingDate, FilterYardID, FilterLocationID, ItemCode, HeatNo, OD, WT, Lenght, Grade, Maker, FilterStatus);

                    case "Approve":
                        Approve_OnClick(myModel.DataList, collection);
                        return Filter_OnClick(StockTakingDate, FilterYardID, FilterLocationID, ItemCode, HeatNo, OD, WT, Lenght, Grade, Maker, FilterStatus);

                    case "Delete":
                        Delete_OnClick(myModel.DataList, collection);
                        return Filter_OnClick(StockTakingDate, FilterYardID, FilterLocationID, ItemCode, HeatNo, OD, WT, Lenght, Grade, Maker, FilterStatus);

                    default:
                        return ddlYard_Change(FilterYardID);

                }
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                return View(this.myModel);
            }
        }

        private void SetDisableButton(string pFilterStatus)
        {
            if (pFilterStatus != string.Empty)
            {
                switch ((Constants.StockTakingStatus)Convert.ToInt32(pFilterStatus))
                {
                    case Constants.StockTakingStatus.New:
                        myModel.SaveDisable = false;
                        myModel.SubmitDisable = false;
                        myModel.AdjustDisable = true;
                        myModel.ApproveDisable = true;
                        break;
                    case Constants.StockTakingStatus.Submit:
                        myModel.SaveDisable = true;
                        myModel.SubmitDisable = true;
                        myModel.AdjustDisable = false;
                        myModel.ApproveDisable = false;
                        break;
                    case Constants.StockTakingStatus.Adjust:
                        myModel.SaveDisable = false;
                        myModel.SubmitDisable = false;
                        myModel.AdjustDisable = true;
                        myModel.ApproveDisable = true;
                        break;
                    case Constants.StockTakingStatus.Approve:
                        myModel.SaveDisable = true;
                        myModel.SubmitDisable = true;
                        myModel.AdjustDisable = true;
                        myModel.ApproveDisable = true;
                        break;
                }
            }
            else
            {
                var lstStatus = this.myModel.DataList.Select(c => c.Status).Distinct().ToList();
                if (lstStatus != null)
                {
                    if (lstStatus.Count == 1)
                    {
                        Enum.TryParse(lstStatus[0], out Constants.StockTakingStatus myStatus);
                        switch (myStatus)
                        {
                            case Constants.StockTakingStatus.New:
                                myModel.SaveDisable = false;
                                myModel.SubmitDisable = false;
                                myModel.AdjustDisable = true;
                                myModel.ApproveDisable = true;
                                break;
                            case Constants.StockTakingStatus.Submit:
                                myModel.SaveDisable = true;
                                myModel.SubmitDisable = true;
                                myModel.AdjustDisable = false;
                                myModel.ApproveDisable = false;
                                break;
                            case Constants.StockTakingStatus.Adjust:
                                myModel.SaveDisable = false;
                                myModel.SubmitDisable = false;
                                myModel.AdjustDisable = true;
                                myModel.ApproveDisable = true;
                                break;
                            case Constants.StockTakingStatus.Approve:
                                myModel.SaveDisable = true;
                                myModel.SubmitDisable = true;
                                myModel.AdjustDisable = true;
                                myModel.ApproveDisable = true;
                                break;
                            default:
                                myModel.SaveDisable = true;
                                myModel.SubmitDisable = true;
                                myModel.AdjustDisable = true;
                                myModel.ApproveDisable = true;
                                break;
                        }
                    }
                }
            }
        }

        private List<byte> GetStatusList()
        {
            List<byte> result = null;
            
            switch (this.LoginUser.RoleId)
            {
                case Constants.ROLE_SYSTEMADMIN:
                    result = new  List<byte>();
                    foreach (Constants.StockTakingStatus enStstus in (Constants.StockTakingStatus[])Enum.GetValues(typeof(Constants.StockTakingStatus)))
                    {
                        result.Add((byte)enStstus);
                    }
                    break;

                case Constants.ROLE_ADMIN:
                    result = new List<byte>();
                    foreach (Constants.StockTakingStatus enStstus in (Constants.StockTakingStatus[])Enum.GetValues(typeof(Constants.StockTakingStatus)))
                    {
                        result.Add((byte)enStstus);
                    }
                    break;

                case Constants.ROLE_MANAGER:
                    result  = new List<byte>();
                    foreach (Constants.Manager_StockTakingStatus enStstus in (Constants.Manager_StockTakingStatus[])Enum.GetValues(typeof(Constants.Manager_StockTakingStatus)))
                    {
                        result.Add((byte)enStstus);
                    }
                    break;

                case Constants.ROLE_CONTROLLER:
                    result = new List<byte>();
                    foreach (Constants.Editor_StockTakingStatus enStstus in (Constants.Editor_StockTakingStatus[])Enum.GetValues(typeof(Constants.Editor_StockTakingStatus)))
                    {
                        result.Add((byte)enStstus);
                    }
                    break;

                default:
                    break;
            }

            return result;
        }

        private List<SelectListItem> GetSelectListItemListStatus()
        {
            List<SelectListItem> result = null;

            switch (this.LoginUser.RoleId)
            {
                case Constants.ROLE_SYSTEMADMIN:
                    result = new List<SelectListItem>();
                    foreach (Constants.StockTakingStatus enStstus in (Constants.StockTakingStatus[])Enum.GetValues(typeof(Constants.StockTakingStatus)))
                    {
                        result.Add(new SelectListItem { Text = enStstus.ToString(), Value = ((int)enStstus).ToString() });
                    }
                    break;

                case Constants.ROLE_ADMIN:
                    result = new List<SelectListItem>();
                    foreach (Constants.StockTakingStatus enStstus in (Constants.StockTakingStatus[])Enum.GetValues(typeof(Constants.StockTakingStatus)))
                    {
                        result.Add(new SelectListItem { Text = enStstus.ToString(), Value = ((int)enStstus).ToString() });
                    }
                    break;

                case Constants.ROLE_MANAGER:
                    result = new List<SelectListItem>();
                    foreach (Constants.Manager_StockTakingStatus enStstus in (Constants.Manager_StockTakingStatus[])Enum.GetValues(typeof(Constants.Manager_StockTakingStatus)))
                    {
                        result.Add(new SelectListItem { Text = enStstus.ToString(), Value = ((int)enStstus).ToString() });
                    }
                    break;

                case Constants.ROLE_CONTROLLER:
                    result = new List<SelectListItem>();
                    foreach (Constants.Editor_StockTakingStatus enStstus in (Constants.Editor_StockTakingStatus[])Enum.GetValues(typeof(Constants.Editor_StockTakingStatus)))
                    {
                        result.Add(new SelectListItem { Text = enStstus.ToString(), Value = ((int)enStstus).ToString() });
                    }
                    break;

                default:
                    break;
            }

            return result;
        }

        private List<StockTakingScreen> Search(DateTime pDate, string pYardID, string pLocationID, string pItemCode,
                                         string pHeatNo, string pOD, string pWT, string pLength, string pGrade, string pMaker,string pStatus)
        {
            List<StockTakingScreen> result = null;
            List<byte> status = null;

            try
            {
                StockListService objStockListService = new StockListService(this.dbEntities);
                if(pStatus == string.Empty)
                {
                    status = GetStatusList();
                }
                else
                {
                    status = new List<byte>();
                    status.Add(Convert.ToByte(pStatus));
                }
                
                result = GetStockTaking(pDate, pYardID, pLocationID, pItemCode, pHeatNo, pOD, pWT, pLength, pGrade, pMaker, status);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<StockTakingScreen> GetCheckedOnDataList(List<StockTakingScreen> pDataList, FormCollection collection)
        {
            List<StockTakingScreen> result = null;

            try
            {
                if (pDataList != null)
                {
                    foreach (StockTakingScreen enStockTakingScreen in pDataList)
                    {
                        if (!string.IsNullOrEmpty(collection["Check_" + enStockTakingScreen.RowNo]))
                        {
                            string strCheck = collection["Check_" + enStockTakingScreen.RowNo];
                            if (strCheck.IndexOf("true") >= 0)
                            {
                                if (result == null)
                                {
                                    result = new List<StockTakingScreen>();
                                }
                                enStockTakingScreen.Check = true;
                                result.Add(enStockTakingScreen);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        private ActionResult Filter_OnClick(DateTime pDate, string pYardID, string pLocationID, string pItemCode,
                                    string pHeatNo, string pOD, string pWT, string pLength, string pGrade, string pMaker,string pStatus)
        {
            try
            {
                this.myModel.DataList = Search(pDate, pYardID, pLocationID, pItemCode, pHeatNo, pOD, pWT, pLength, pGrade, pMaker, pStatus);

                SetDisableButton(pStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2510", this.myModel);
        }

        private ActionResult ddlYard_Change(string pYardList)
        {
            List<SelectListItem> lstLocation = null;

            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);

                if (pYardList != string.Empty)
                {
                    lstLocation = new List<SelectListItem>();
                    foreach (Location objLocation in objLocationService.GetLocationList(pYardList))
                    {
                        lstLocation.Add(new SelectListItem { Text = objLocation.Name, Value = objLocation.LocationCode.ToString() });
                    }

                    this.myModel.LocationList = lstLocation;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2510", this.myModel);
        }

        private ActionResult SaveModal_OnClick(string pRowNo, string pActualQTY)
        {
            try
            {
                if (this.myModel.DataList != null)
                {
                    SetRoleFlag();

                    int index = Convert.ToInt32(pRowNo) - 1;
                    if (index >= 0)
                    {
                        if(index <= this.myModel.DataList.Count )
                        {
                            this.myModel.DataList[index].ActualQty = Convert.ToInt32(pActualQTY);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2510", this.myModel);
        }

        private Boolean Save_OnClick(List<StockTakingScreen> pDataList)
        {
            Boolean result = false;
            try
            {
                StockTakingService objStockTakingService = new StockTakingService(this.dbEntities);
                if (pDataList != null && LoginUser != null)
                {
                    result = objStockTakingService.Save(pDataList, this.LoginUser);
                    if (result)
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Success ;
                        this.myModel.Message = "Save complete.";
                    }
                    else
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Danger ;
                        this.myModel.Message = "Save fail.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
                
        private Boolean Submit_OnClick(List<StockTakingScreen> pDataList, FormCollection collection)
        {
            Boolean result = false;
            List<StockTakingScreen> objStockTaking = null;

            try
            {
                objStockTaking = GetCheckedOnDataList(pDataList, collection);
                if(objStockTaking == null)
                {
                    this.myModel.AlertsType = Constants.AlertsType.Danger ;
                    this.myModel.Message = "Please Choose Stock Taking.";
                }
                else
                {
                    StockTakingService objStockTakingService = new StockTakingService(this.dbEntities);
                    
                    result = objStockTakingService.SubmitUpdateStatus(objStockTaking, this.LoginUser);
                    if (result)
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Success ;
                        this.myModel.Message = "Submit complete.";
                    }
                    else
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Danger ;
                        this.myModel.Message = "Submit fail.";
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean Adjust_OnClick(List<StockTakingScreen> pDataList, FormCollection collection)
        {
            Boolean result = false;
            List<StockTakingScreen> objStockTaking = null;

            try
            {
                objStockTaking = GetCheckedOnDataList(pDataList, collection);
                if (objStockTaking == null)
                {
                    this.myModel.AlertsType = Constants.AlertsType.Danger ;
                    this.myModel.Message = "Please Choose Stock Taking.";
                }
                else
                {
                    StockTakingService objStockTakingService = new StockTakingService(this.dbEntities);

                    result = objStockTakingService.UpdateStatus(objStockTaking, Constants.StockTakingStatus.Adjust,this.LoginUser);
                    if (result)
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Success ;
                        this.myModel.Message = "Adjust complete.";
                    }
                    else
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Danger ;
                        this.myModel.Message = "Adjust fail.";
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean Approve_OnClick(List<StockTakingScreen> pDataList, FormCollection collection)
        {
            Boolean result = false;
            List<StockTakingScreen> objStockTaking = null;
            StockTakingService objStockTakingService = null;
            StockService objStockService = null;
            PipeService objPipeService = null;

            try
            {
                objStockTaking = GetCheckedOnDataList(pDataList, collection);
                if (objStockTaking == null)
                {
                    this.myModel.AlertsType = Constants.AlertsType.Danger;
                    this.myModel.Message = "Please Choose Stock Taking.";
                }
                else
                {
                    objPipeService = new PipeService(this.dbEntities);
                    result = objPipeService.InsertData(objStockTaking);
                    if (!result)
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Danger ;
                        this.myModel.Message = "Approve fail : can not update Pipe master.";
                    }
                    else
                    {
                        objStockService = new StockService(this.dbEntities, this.LoginUser);
                        result = objStockService.InsertUpdateDataByStocktaking(objStockTaking);
                        if (!result)
                        {
                            this.myModel.AlertsType = Constants.AlertsType.Danger ;
                            this.myModel.Message = "Approve fail : can not update Stock.";
                        }
                        else
                        {
                            objStockTakingService = new StockTakingService(this.dbEntities);
                            result = objStockTakingService.UpdateStatus(objStockTaking, Constants.StockTakingStatus.Approve, this.LoginUser);
                            if (result)
                            {
                                this.myModel.AlertsType = Constants.AlertsType.Success ;
                                this.myModel.Message = "Approve complete.";
                            }
                            else
                            {
                                this.myModel.AlertsType = Constants.AlertsType.Danger ;
                                this.myModel.Message = "Approve fail.";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean Delete_OnClick(List<StockTakingScreen> pDataList, FormCollection collection)
        {
            Boolean result = false;
            Boolean isApprove = false;
            List<StockTakingScreen> objStockTaking = null;
            StockTakingService objStockTakingService = null;

            try
            {
                objStockTaking = GetCheckedOnDataList(pDataList, collection);
                if (objStockTaking == null)
                {
                    this.myModel.AlertsType = Constants.AlertsType.Danger;
                    this.myModel.Message = "Please Choose Stock Taking.";
                }
                else
                {
                    //Check data is approve
                    foreach (StockTakingScreen enStockTaking in objStockTaking)
                    {
                        string tmp = Constants.StockTakingStatus.Approve.ToString();
                        if (enStockTaking.Status.Equals(Constants.StockTakingStatus.Approve.ToString()))
                        {
                            isApprove = true;
                            break;
                        }
                    }

                    if(isApprove)
                    {
                        this.myModel.AlertsType = Constants.AlertsType.Danger;
                        this.myModel.Message = "Can't delete data in status Approved.";
                    }
                    else
                    {
                        objStockTakingService = new StockTakingService(this.dbEntities);
                        result = objStockTakingService.DeleteData(objStockTaking);

                        if (result)
                        {
                            this.myModel.AlertsType = Constants.AlertsType.Success;
                            this.myModel.Message = "Delete complete.";
                        }
                        else
                        {
                            this.myModel.AlertsType = Constants.AlertsType.Danger;
                            this.myModel.Message = "Delete fail.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        [HttpPost]
        public ActionResult PSC2511()
        {
            //ImportExcelModel model = new ImportExcelModel();
            ViewBag.LoginUserName = this.LoginUser.UserId;

            ImportExcelScreen thisModel = new ImportExcelScreen();
            return View( thisModel);
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileWrapper importFile)
        {
            string strType = string.Empty;
            string strMessage = string.Empty;

            try
            {
                if (importFile == null)
                {
                    strType = "Error";
                    strMessage = "Please select file.";
                }

                if(strType == string.Empty)
                {
                    if (!importFile.ContentType.Equals(Constants.EXCEL_CONTENTTYPE))
                    {
                        strType = "Error";
                        strMessage = "Please select excel file.";
                    }
                }

                if (strType == string.Empty)
                {
                    StocktakingExcel excel = new StocktakingExcel(this.dbEntities, this.LoginUser);
                    // Import & save to db
                    using (MemoryStream ms = new MemoryStream())
                    {
                        importFile.InputStream.CopyTo(ms);
                        if (!excel.Import(ms))
                        {
                            strType = "Error";
                            strMessage = excel.GetErrorMesssage();
                        }
                        else
                        {
                            strType = "Success";
                            strMessage = "Import successfully.";
                        }
                    }
                    Session["ImportStocktakingDate"] = excel.StocktakingDate;
                }  
            }
            catch (Exception ex)
            {
                strType = "Error";
                strMessage = ex.Message;
            }

            if (strType == "Error")
            {
                this.PrintError(strMessage);
            }

            return Json(new { Type = strType, Message = strMessage });
        }

        [HttpPost]
        public ActionResult Back()
        {
            try
            {
                SetRoleFlag();
                ViewBag.IsError = false;

                YardService objYardService = new YardService(this.dbEntities);
                StockTakingService objStockTakingService = new StockTakingService(this.dbEntities);

                foreach (Yard objYard in objYardService.GetYardList())
                {
                    this.myModel.YardList.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
                }

                if(Session["ImportStocktakingDate"] != null)
                {
                    this.myModel.StockTakingDate = (DateTime)Session["ImportStocktakingDate"];
                    Session["ImportStocktakingDate"] = null;
                }

                this.myModel.DataList = GetStockTaking(this.myModel.StockTakingDate, "", "", "", "", "", "", "", "", "", GetStatusList());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2510", this.myModel);
        }

        [HttpPost]
        public ActionResult Export(DateTime StockTakingDate)
        {
            this.myModel.AlertsType = Constants.AlertsType.None;
            this.myModel.Message = string.Empty;

            StocktakingExcel excel = new StocktakingExcel(this.dbEntities, this.LoginUser);
            excel.YearMonthDate = StockTakingDate;

            MemoryStream ms = new MemoryStream();
            if (!excel.Export(ms))
            {
                Response.Clear();
                this.myModel.AlertsType = Constants.AlertsType.Danger;
                this.myModel.Message = "Unable to export stocktaking list. " + excel.GetErrorMesssage();
                PrintError(this.myModel.Message);
                return View("PSC2510", this.myModel);
            }

            DateTime dt = DateTime.Today;
            Response.AddHeader("Content-Disposition", "attachment; filename=Stocktaking_" + dt.ToString(Constants.DATE_FILENAME) + Constants.EXCEL_EXTENSION);

            return new FileStreamResult(ms, Constants.EXCEL_CONTENTTYPE);
        }

        private List<StockTakingScreen> GetStockTaking(DateTime pDate,string pYardID, string pLocationID, string pItemCode, string pHeatNo, string pOD, string pWT, string pLength, string pGrade, string pMaker, List<byte> pStatus)
        {
            StockTakingService objStockTakingService = new StockTakingService(this.dbEntities);
            var andExList = new List<Expression<Func<StockTakingScreen, bool>>>();

            List<StockTakingScreen> StockTakingList = new List<StockTakingScreen>();
            StockTakingList = objStockTakingService.GetStockTakingList(pDate, pYardID, pLocationID, pItemCode, pHeatNo, 
                                                                          pOD == string.Empty ? 0 : Convert.ToDecimal(pOD),
                                                                          pWT == string.Empty ? 0 : Convert.ToDecimal(pWT),
                                                                          pLength == string.Empty ? 0 : Convert.ToDecimal(pLength),
                                                                          pGrade, pMaker, pStatus);

            return StockTakingList;
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
                    ViewBag.IsYardSupervisor = true;
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