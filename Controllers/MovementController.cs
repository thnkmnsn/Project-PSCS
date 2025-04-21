using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.ModelsScreen;
using System.Net;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class MovementController : BaseController
    {
        public MovementScreen model
        {
            get
            {
                if (Session["MovementScreen"] == null)
                {
                    Session["MovementScreen"] = new MovementScreen();
                    return (MovementScreen)Session["MovementScreen"];
                }
                else
                {
                    return (MovementScreen)Session["MovementScreen"];
                }
            }

            set { Session["MovementScreen"] = value; }
        }

        public MovementScreenEdit modelEdit
        {
            get
            {
                if (Session["MovementScreenEdit"] == null)
                {
                    Session["MovementScreenEdit"] = new MovementScreenEdit();
                    return (MovementScreenEdit)Session["MovementScreenEdit"];
                }
                else
                {
                    return (MovementScreenEdit)Session["MovementScreenEdit"];
                }
            }

            set { Session["MovementScreenEdit"] = value; }
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2210()
        {
            try
            {
                // Initial View
                IntialPSC2210();
                if (modelEdit.HasError)
                {
                    modelEdit.HasError = false;
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.None;
                    this.model.Message = string.Empty;
                }

                if (this.LoginUser.RoleId == Constants.ROLE_YARDSUPERVISOR)
                {
                    this.model.IsYardSupervisorRole = true;
                    this.model.IsControllerRole = false;
                }
                else if (this.LoginUser.RoleId == Constants.ROLE_CONTROLLER)
                {
                    this.model.IsYardSupervisorRole = false;
                    this.model.IsControllerRole = true;
                }
                else
                {
                    this.model.IsYardSupervisorRole = false;
                    this.model.IsControllerRole = false;
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
            }

            return View(this.model);
        }

        private void IntialPSC2210()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
            InitializeActionName = "PSC2210";
            QueryStringList = new Dictionary<string, string>();
            this.model.YardList = GetYard();
            this.model.FilterInternalMoveDate = DateTime.Now;

            LoadData(this.model.FilterInternalMoveDate);

            this.model.MoveLocationList = GetMoveLocationList(this.model.FilterInternalMoveDate, "");
            this.model.Total = this.model.MoveLocationList.Count.ToString();
        }

        [HttpPost]
        public ActionResult PSC2210(MovementScreen FilterModel, string submitButton)
        {
            try
            {
                // Initial View
                ViewBag.LoginUserName = this.LoginUser.UserId;
                ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                switch (submitButton)
                {
                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");

                    case "Filter":
                        ViewBag.IsFilter = true;
                        LoadData(FilterModel.FilterInternalMoveDate);
                        return Filter_OnClick(FilterModel);

                    case "LoadData":
                        LoadData(FilterModel.FilterInternalMoveDate);
                        break;

                    case "Save":
                        return Save_OnClick(FilterModel);

                    case "Delete":
                        return Filter_OnClick(FilterModel);

                    case "Approve":
                        return Filter_OnClick(FilterModel);
                }

                return View(this.model);
                //return Filter_OnClick(FilterModel);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Common.Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.model);
            }
        }

        // Filter Onclick
        private ActionResult Filter_OnClick(MovementScreen FilterModel)
        {
            try
            {
                ModelState.Clear();
                this.model.FilterInternalMoveDate = FilterModel.FilterInternalMoveDate;
                this.model.FilterYardID = FilterModel.FilterYardID;
                SearchDataToGrid(FilterModel.FilterInternalMoveDate, FilterModel.FilterYardID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC2210", this.model);
        }

        private Boolean SearchDataToGrid(DateTime pMovementDate, string pYardId)
        {
            Boolean result = false;

            try
            {
                this.model.MoveLocationList = GetMoveLocationList(pMovementDate, pYardId);

                this.model.FilterInternalMoveDate = pMovementDate;
                this.model.FilterYardID = pYardId;
                this.model.Total = this.model.MoveLocationList.Count.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean ParsingMoveOut(ref List<MoveLocation> pList, decimal pMoveId, Common.Constants.HHTMoveOutStatus pStatus)
        {
            Boolean result = false;

            MoveOutService objMoveOutService = null;
            List<MoveOut> moveOutList = null;
            MoveLocation objMoveLocationChild = null;

            try
            {
                objMoveOutService = new MoveOutService(this.dbEntities);
                moveOutList = objMoveOutService.GetMoveOutListByMoveId(pMoveId, pStatus);
                if (moveOutList != null)
                {
                    foreach (MoveOut enMoveOut in moveOutList)
                    {
                        objMoveLocationChild = new MoveLocation();
                        objMoveLocationChild.RowNoDisplay = "";
                        objMoveLocationChild.MoveId = Convert.ToDecimal(0);
                        objMoveLocationChild.HHTTransId = enMoveOut.ID;
                        objMoveLocationChild.MoveDate = null;
                        objMoveLocationChild.ItemCode = enMoveOut.ITEM_CODE;
                        objMoveLocationChild.HeatNo = enMoveOut.HEAT_NO;
                        objMoveLocationChild.HeatNoDisplay = "";
                        objMoveLocationChild.Description = "";
                        objMoveLocationChild.Time = enMoveOut.TRAN_DATE;
                        objMoveLocationChild.TimeDisplay = enMoveOut.TRAN_DATE.ToString("HH:mm:ss");
                        objMoveLocationChild.Operation = "Out";
                        objMoveLocationChild.LocationCodeFrom = enMoveOut.LOCATION_CODE;
                        objMoveLocationChild.LocationCodeFromName = enMoveOut.LOCATION_CODE;
                        objMoveLocationChild.LocationCodeFromNameDisplay = GetDisplayLocation(enMoveOut.LOCATION_CODE);
                        objMoveLocationChild.QTY = null;
                        objMoveLocationChild.HHTTransQTY = enMoveOut.ACTUAL_QTY;
                        objMoveLocationChild.LocationCodeTo = "";
                        objMoveLocationChild.LocationCodeToName = "";
                        objMoveLocationChild.LocationCodeToNameDisplay = "";
                        objMoveLocationChild.Status = enMoveOut.STATUS;
                        objMoveLocationChild.Remark = "";
                        objMoveLocationChild.Is_Release = 0;
                        objMoveLocationChild.DeliveryDate = Common.Common.GetDeliveryDateFromBarcode(enMoveOut.BARCODE);

                        //SET ReceiveDate by DeliveryDate
                        if (objMoveLocationChild.DeliveryDate != null)
                        {
                            ReceivingInstructionService objReceivingInstructionService = new ReceivingInstructionService(this.dbEntities);
                            List<ReceivingInstruction> objList = objReceivingInstructionService.GetReceivingInstructionList(objMoveLocationChild.DeliveryDate.Value, objMoveLocationChild.ItemCode, objMoveLocationChild.HeatNo);
                            if (objList != null)
                            {
                                if (objList.Count > 0)
                                {
                                    objMoveLocationChild.ReceiveDate = objList[0].ReceiveDate;
                                }
                                else
                                {
                                    objMoveLocationChild.ReceiveDate = objMoveLocationChild.DeliveryDate;
                                }
                            }
                            else
                            {
                                objMoveLocationChild.ReceiveDate = objMoveLocationChild.DeliveryDate;
                            }
                        }

                        objMoveLocationChild.Type = Constants.MovementType.Out;

                        if (pList == null)
                        {
                            pList = new List<MoveLocation>();
                        }
                        pList.Add(objMoveLocationChild);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean ParsingMoveIn(ref List<MoveLocation> pList, decimal pMoveId, Common.Constants.HHTMoveInStatus pStatus)
        {
            Boolean result = false;

            MoveINService objMoveINService = null;
            List<MoveIN> moveINList = null;
            MoveLocation objMoveLocationChild = null;

            try
            {
                objMoveINService = new MoveINService(this.dbEntities);
                moveINList = objMoveINService.GetMoveINByMoveId(pMoveId, pStatus);
                if (moveINList != null)
                {
                    foreach (MoveIN enMoveIN in moveINList)
                    {
                        objMoveLocationChild = new MoveLocation();
                        objMoveLocationChild.RowNoDisplay = "";
                        objMoveLocationChild.MoveId = Convert.ToDecimal(0);
                        objMoveLocationChild.HHTTransId = enMoveIN.ID;
                        objMoveLocationChild.MoveDate = null;
                        objMoveLocationChild.ItemCode = enMoveIN.ITEM_CODE;
                        objMoveLocationChild.HeatNo = enMoveIN.HEAT_NO;
                        objMoveLocationChild.HeatNoDisplay = "";
                        objMoveLocationChild.Description = "";
                        objMoveLocationChild.Time = enMoveIN.TRAN_DATE;
                        objMoveLocationChild.TimeDisplay = enMoveIN.TRAN_DATE.ToString("HH:mm:ss");
                        objMoveLocationChild.Operation = "In";
                        objMoveLocationChild.LocationCodeFrom = "";
                        objMoveLocationChild.LocationCodeFromName = "";
                        objMoveLocationChild.LocationCodeFromNameDisplay = "";
                        objMoveLocationChild.QTY = null;
                        objMoveLocationChild.HHTTransQTY = enMoveIN.ACTUAL_QTY;
                        objMoveLocationChild.LocationCodeTo = enMoveIN.LOCATION_CODE;
                        objMoveLocationChild.LocationCodeToName = enMoveIN.LOCATION_CODE;
                        objMoveLocationChild.LocationCodeToNameDisplay = GetDisplayLocation(enMoveIN.LOCATION_CODE);
                        objMoveLocationChild.Status = enMoveIN.STATUS;
                        objMoveLocationChild.Remark = "";
                        objMoveLocationChild.Is_Release = 0;
                        objMoveLocationChild.DeliveryDate = Common.Common.GetDeliveryDateFromBarcode(enMoveIN.BARCODE);
                        //SET ReceiveDate by DeliveryDate
                        if (objMoveLocationChild.DeliveryDate != null)
                        {
                            ReceivingInstructionService objReceivingInstructionService = new ReceivingInstructionService(this.dbEntities);
                            List<ReceivingInstruction> objList = objReceivingInstructionService.GetReceivingInstructionList(objMoveLocationChild.DeliveryDate.Value, objMoveLocationChild.ItemCode, objMoveLocationChild.HeatNo);
                            if (objList != null)
                            {
                                if (objList.Count > 0)
                                {
                                    objMoveLocationChild.ReceiveDate = objList[0].ReceiveDate;
                                }
                                else
                                {
                                    objMoveLocationChild.ReceiveDate = objMoveLocationChild.DeliveryDate;
                                }
                            }
                            else
                            {
                                objMoveLocationChild.ReceiveDate = objMoveLocationChild.DeliveryDate;
                            }
                        }
                        objMoveLocationChild.Type = Constants.MovementType.In;

                        if (pList == null)
                        {
                            pList = new List<MoveLocation>();
                        }
                        pList.Add(objMoveLocationChild);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private Boolean ParsingRelease(ref List<MoveLocation> pList, decimal pMoveId, Common.Constants.HHTReleaseStatus pStatus)
        {
            Boolean result = false;

            ReleaseService objReleaseService = null;
            List<Release> ReleaseList = null;
            MoveLocation objMoveLocationChild = null;

            try
            {
                objReleaseService = new ReleaseService(this.dbEntities);
                ReleaseList = objReleaseService.GetReleaseListByMoveId(pMoveId, pStatus);
                if (ReleaseList != null && ReleaseList.Count > 0)
                {
                    foreach (Release enRelease in ReleaseList)
                    {
                        objMoveLocationChild = new MoveLocation();
                        objMoveLocationChild.RowNoDisplay = "";
                        objMoveLocationChild.MoveId = Convert.ToDecimal(0);
                        objMoveLocationChild.HHTTransId = enRelease.ID;
                        objMoveLocationChild.HHTJobNo = enRelease.JobNo;
                        objMoveLocationChild.MoveDate = null;
                        objMoveLocationChild.ItemCode = enRelease.ItemCode;
                        objMoveLocationChild.HeatNo = enRelease.HeatNo;
                        objMoveLocationChild.HeatNoDisplay = "";
                        objMoveLocationChild.Description = "";
                        objMoveLocationChild.Time = enRelease.TRAN_DATE;
                        objMoveLocationChild.TimeDisplay = enRelease.TRAN_DATE.ToString("HH:mm:ss");
                        objMoveLocationChild.Operation = "In";
                        objMoveLocationChild.LocationCodeFrom = "";
                        objMoveLocationChild.LocationCodeFromName = "";
                        objMoveLocationChild.LocationCodeFromNameDisplay = "";
                        objMoveLocationChild.QTY = null;
                        objMoveLocationChild.HHTTransQTY = enRelease.QTY;
                        objMoveLocationChild.LocationCodeTo = enRelease.LocationCode;
                        objMoveLocationChild.LocationCodeToName = enRelease.LocationCode;
                        objMoveLocationChild.LocationCodeToNameDisplay = GetDisplayLocation(enRelease.LocationCode);
                        objMoveLocationChild.ProductName = enRelease.Product;
                        objMoveLocationChild.MfgNo = enRelease.MfgNo;
                        objMoveLocationChild.Operator = enRelease.Operator;
                        objMoveLocationChild.Status = enRelease.Status;
                        objMoveLocationChild.Remark = "";
                        objMoveLocationChild.Is_Release = 0;
                        objMoveLocationChild.DeliveryDate = Common.Common.GetDeliveryDateFromBarcode(enRelease.Barcode);
                        //SET ReceiveDate by DeliveryDate
                        if (objMoveLocationChild.DeliveryDate != null)
                        {
                            ReceivingInstructionService objReceivingInstructionService = new ReceivingInstructionService(this.dbEntities);
                            List<ReceivingInstruction> objList = objReceivingInstructionService.GetReceivingInstructionList(objMoveLocationChild.DeliveryDate.Value, objMoveLocationChild.ItemCode, objMoveLocationChild.HeatNo);
                            if (objList != null)
                            {
                                if (objList.Count > 0)
                                {
                                    objMoveLocationChild.ReceiveDate = objList[0].ReceiveDate;
                                }
                                else
                                {
                                    objMoveLocationChild.ReceiveDate = objMoveLocationChild.DeliveryDate;
                                }
                            }
                            else
                            {
                                objMoveLocationChild.ReceiveDate = objMoveLocationChild.DeliveryDate;
                            }
                        }
                        objMoveLocationChild.Type = Constants.MovementType.Release;

                        if (pList == null)
                        {
                            pList = new List<MoveLocation>();
                        }
                        pList.Add(objMoveLocationChild);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private decimal GetSumQty(List<MoveLocation> Source, Constants.MovementType pType)
        {
            decimal result = 0;
            List<MoveLocation> objMoveLocationChildListGetQty = null;

            try
            {
                objMoveLocationChildListGetQty = Source.Where(ml => ml.Type == pType).ToList();
                if (objMoveLocationChildListGetQty != null)
                {
                    foreach(MoveLocation en in objMoveLocationChildListGetQty)
                    {
                        result = result +  (en.HHTTransQTY == null? 0 : Convert.ToDecimal(en.HHTTransQTY));
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<MoveLocation> GetMoveLocationList(DateTime pMovementDate, string pYardId)
        {
            List<MoveLocation> result = new List<MoveLocation>();
            List<MoveLocation> objMoveLocationList = null;
            List<MoveLocation> objMoveLocationChildList = null;
            
            List<MoveLocation> objMoveLocationChildListTemp = null;
            decimal decQtyOut = 0;
            decimal decQtyIn = 0;
            decimal decQtyRelease = 0;
            Boolean IsChildSave = false;

            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                HttpCookie langCookie = Request.Cookies["PSCS_culture"];
                objMoveLocationList = objMoveLocationService.GetMoveLocationList(pMovementDate, pYardId);

                if (objMoveLocationList != null)
                {
                    foreach (MoveLocation en in objMoveLocationList)
                    {
                        objMoveLocationChildList = new List<MoveLocation>();
                        
                        //1. PSC2210_T_HHT_MOVE_OUT
                        ParsingMoveOut(ref objMoveLocationChildList, en.MoveId, Constants.HHTMoveOutStatus.SubmitTrans);
                        ParsingMoveOut(ref objMoveLocationChildList, en.MoveId, Constants.HHTMoveOutStatus.ApproveTrans);

                        //2. PSC2210_T_HHT_MOVE_IN
                        ParsingMoveIn(ref objMoveLocationChildList, en.MoveId, Constants.HHTMoveInStatus.SubmitTrans);
                        ParsingMoveIn(ref objMoveLocationChildList, en.MoveId, Constants.HHTMoveInStatus.ApproveTrans);

                        //3. PSC2410_T_HHT_RELEASE
                        ParsingRelease(ref objMoveLocationChildList, en.MoveId, Constants.HHTReleaseStatus.Submit);
                        ParsingRelease(ref objMoveLocationChildList, en.MoveId, Constants.HHTReleaseStatus.Approve);
                        ParsingRelease(ref objMoveLocationChildList, en.MoveId, Constants.HHTReleaseStatus.Sync);

                        decQtyOut = GetSumQty(objMoveLocationChildList, Constants.MovementType.Out);
                        decQtyIn = GetSumQty(objMoveLocationChildList, Constants.MovementType.In);
                        decQtyRelease = GetSumQty(objMoveLocationChildList, Constants.MovementType.Release);

                        IsChildSave = false;
                        objMoveLocationChildListTemp = objMoveLocationChildList.OrderBy(ml => ml.Time).ToList();
                        if(objMoveLocationChildListTemp != null)
                        {
                            foreach (MoveLocation enTemp in objMoveLocationChildListTemp)
                            {
                                if (enTemp.Status == 3) //ApproveTrans
                                {
                                    IsChildSave = true;
                                    break;
                                }
                            }
                        }

                        MoveLocation objMoveLocation = new MoveLocation();
                        objMoveLocation.RowNoDisplay = en.RowNo.ToString();
                        objMoveLocation.MoveId = en.MoveId;
                        objMoveLocation.MoveDate = en.MoveDate;
                        objMoveLocation.ItemCode = en.ItemCode;
                        objMoveLocation.HeatNo = en.HeatNo;
                        objMoveLocation.HeatNoDisplay = en.HeatNo;
                        objMoveLocation.Description = en.Description;
                        objMoveLocation.Time = en.Time;
                        objMoveLocation.TimeDisplay = "";
                        objMoveLocation.Operation = "";
                        objMoveLocation.LocationCodeFrom = en.LocationCodeFrom;
                        objMoveLocation.LocationCodeFromName = en.LocationCodeFromName;
                        objMoveLocation.LocationCodeFromNameDisplay = "";
                        objMoveLocation.QTY = null;
                        objMoveLocation.LocationCodeTo = en.LocationCodeTo;
                        objMoveLocation.LocationCodeToName = en.LocationCodeToName;
                        objMoveLocation.LocationCodeToNameDisplay = "";
                        objMoveLocation.Status = en.Status;
                        objMoveLocation.QTYDifferent = decQtyOut - (decQtyIn + decQtyRelease);
                        objMoveLocation.Remark = en.Remark;
                        objMoveLocation.Is_Release = 0;
                        if(objMoveLocation.QTYDifferent == 0)
                        {
                            if(IsChildSave)
                            {
                                objMoveLocation.CellCss = "rowSaved";
                            }
                            else
                            {
                                objMoveLocation.CellCss = "";
                            }
                        }
                        else
                        {
                            objMoveLocation.CellCss = "rowDiff";
                        }
                        result.Add(objMoveLocation);

                        
                        foreach (MoveLocation enTemp in objMoveLocationChildListTemp)
                        {
                            if(enTemp.Status == 2)      //SubmitTrans
                            {
                                enTemp.CellCss = "";
                            }
                            else if(enTemp.Status == 3) //ApproveTrans
                            {
                                enTemp.CellCss = "rowSaved";
                            }
                            else if (enTemp.Status == 4) //Sync
                            {
                                enTemp.CellCss = "rowSaved";
                            }

                            result.Add(enTemp);
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

        // Save Onclick 
        [HttpPost]
        public ActionResult Save_OnClick(MovementScreen FilterModel)
        {
            try
            {
                Boolean result = false;
                Boolean checkRemark = false;
                List<MoveLocation> objMoveLocationList = null;
                List<MoveLocation> objMoveLocationChildListTemp = null;
                string strMsg = string.Empty;

                if (FilterModel.MoveLocationList != null && this.model.MoveLocationList != null)
                {
                    if (FilterModel.MoveLocationList.Count == this.model.MoveLocationList.Count)
                    {
                        for (int index = 0; index < FilterModel.MoveLocationList.Count; index++)
                        {
                            this.model.MoveLocationList[index].Remark = FilterModel.MoveLocationList[index].Remark;
                        }
                    }
                }

                if (this.model.MoveLocationList != null)
                {
                    objMoveLocationList = this.model.MoveLocationList.Where(ml => ml.MoveId != 0).ToList();

                    if (objMoveLocationList != null)
                    {
                        //foreach (MoveLocation en in objMoveLocationList)
                        //{
                        //    if (en.Remark != null)
                        //    {
                        //        checkRemark = true;
                        //    }
                        //    else
                        //    {
                        //        checkRemark = false;
                        //        break;
                        //    }
                        //}

                        //if (checkRemark)
                        //{
                        //    Save all list
                        //}
                        //else
                        //{
                        //    this.model.AlertsType = Constants.AlertsType.Danger;
                        //    this.model.Message = Resources.Common_cshtml.CheckSaveAllRemarksMsg;
                        //}

                        //Update Remark on Move Location
                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        result = objMoveLocationService.UpdateMoveLocationData(objMoveLocationList, this.LoginUser.UserId);
                        strMsg = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;

                        //Get Monthly Close
                        string MonthlyClose = string.Empty;
                        MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                        MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                        if (objMonthlyClose != null)
                        {
                            DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                            string strFMonth = dFMonth.ToString("yyyy-MM");

                            MonthlyClose = strFMonth + "-01";
                        }

                        //Set Date
                        DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);

                        if (result)
                        {
                            List<MoveLocation> objMoveLocationChildList = new List<MoveLocation>();

                            foreach (MoveLocation enMoveLocation in objMoveLocationList)
                            {
                                if(enMoveLocation.Remark != null)
                                {
                                    objMoveLocationChildList.Clear();

                                    //1. PSC2210_T_HHT_MOVE_OUT
                                    ParsingMoveOut(ref objMoveLocationChildList, enMoveLocation.MoveId, Constants.HHTMoveOutStatus.SubmitTrans);

                                    //2. PSC2210_T_HHT_MOVE_IN
                                    ParsingMoveIn(ref objMoveLocationChildList, enMoveLocation.MoveId, Constants.HHTMoveInStatus.SubmitTrans);

                                    //3. PSC2410_T_HHT_MOVE_IN
                                    ParsingRelease(ref objMoveLocationChildList, enMoveLocation.MoveId, Constants.HHTReleaseStatus.Submit);

                                    if (objMoveLocationChildList != null)
                                    {
                                        objMoveLocationChildListTemp = objMoveLocationChildList.OrderBy(ml => ml.Time).ToList();
                                        if (objMoveLocationChildListTemp != null)
                                        {
                                            foreach (MoveLocation enTemp in objMoveLocationChildListTemp)
                                            {
                                                //SET stockInDate FROM ReceiveDate
                                                //DateTime stockInDate = enTemp.ReceiveDate == null ? DateTime.Now : enTemp.ReceiveDate.Value;
                                                DateTime stockInDate = DateTime.Now;
                                                switch (enTemp.Type)
                                                {
                                                    case Constants.MovementType.Out:
                                                        StockListService objStockListService = new StockListService(this.dbEntities);
                                                        result = objStockListService.UpdateOutData(dateYearMonth, stockInDate, enTemp.ItemCode, enTemp.HeatNo, enTemp.LocationCodeFrom, Convert.ToDecimal(enTemp.HHTTransQTY), this.LoginUser.UserId);
                                                        if (result)
                                                        {
                                                            MoveOutService objMoveOutUpdateService = new MoveOutService(this.dbEntities);
                                                            result = objMoveOutUpdateService.UpdateStatus(Convert.ToDecimal(enTemp.HHTTransId), Constants.HHTMoveOutStatus.ApproveTrans, this.LoginUser.UserId);
                                                            strMsg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                                        }
                                                        else
                                                        {
                                                            strMsg = strMsg + "  " + Resources.Common_cshtml.HeatNo + " = " + enTemp.HeatNo + " (" + Resources.Common_cshtml.FromLocation + " : " + GetDisplayLocation(enTemp.LocationCodeFrom) + ")";
                                                            break;
                                                        }
                                                        break;

                                                    case Constants.MovementType.In:
                                                        StockListService objStockListService1 = new StockListService(this.dbEntities);
                                                        result = objStockListService1.UpdateInData(dateYearMonth, stockInDate, enTemp.ItemCode, enTemp.HeatNo, enTemp.LocationCodeTo, Convert.ToDecimal(enTemp.HHTTransQTY), this.LoginUser.UserId);

                                                        if (result)
                                                        {
                                                            MoveINService objMoveINServiceUpdateService = new MoveINService(this.dbEntities);
                                                            result = objMoveINServiceUpdateService.UpdateStatus(Convert.ToDecimal(enTemp.HHTTransId), Constants.HHTMoveInStatus.ApproveTrans, this.LoginUser.UserId);
                                                            strMsg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                                        }
                                                        else
                                                        {
                                                            strMsg = strMsg + "  " + Resources.Common_cshtml.HeatNo + " = " + enTemp.HeatNo + " (" + Resources.Common_cshtml.DestinationLocation + " : " + GetDisplayLocation(enTemp.LocationCodeTo) + ")";
                                                            break;
                                                        }

                                                        break;

                                                    case Constants.MovementType.Release:
                                                        StockListService objStockListService2 = new StockListService(this.dbEntities);
                                                        result = objStockListService2.UpdateInData(dateYearMonth, stockInDate, enTemp.ItemCode, enTemp.HeatNo, enTemp.LocationCodeTo, Convert.ToDecimal(enTemp.HHTTransQTY), this.LoginUser.UserId);

                                                        if (result)
                                                        {
                                                            ReleaseService objReleaseUpdateService = new ReleaseService(this.dbEntities);
                                                            result = objReleaseUpdateService.UpdateStatus(Convert.ToDecimal(enTemp.HHTTransId), Constants.HHTReleaseStatus.Approve, this.LoginUser.UserId);
                                                            strMsg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                                        }
                                                        else
                                                        {
                                                            strMsg = strMsg + "  " + Resources.Common_cshtml.HeatNo + " = " + enTemp.HeatNo + " (" + Resources.Common_cshtml.DestinationLocation + " : " + GetDisplayLocation(enTemp.LocationCodeTo) + ")";
                                                            break;
                                                        }

                                                        break;
                                                }
                                            }
                                        }

                                        if (objMoveLocationChildListTemp != null)
                                        {
                                            objMoveLocationChildListTemp.Clear();
                                        }
                                        objMoveLocationChildListTemp = objMoveLocationChildList.Where(ml => ml.Type == Constants.MovementType.Release).ToList();
                                        if (objMoveLocationChildListTemp != null)
                                        {
                                            foreach (MoveLocation enMoveReleaseHHT in objMoveLocationChildListTemp)
                                            {
                                                UpdateReleaseDetail(enMoveLocation.MoveId, enMoveReleaseHHT.HHTJobNo, enMoveReleaseHHT.HHTTransQTY);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Alert Message
                        this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                        this.model.Message = strMsg;

                        SearchDataToGrid(FilterModel.FilterInternalMoveDate, FilterModel.FilterYardID);
                    }
                }

                return View("PSC2210", this.model);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, PSCS.Resources.PSC2210_cshtml.ErrorNoUser);
            }
        }

        public class UpdateModel
        {
            public string Remark { get; set; }
            public string Destination { get; set; }
        }

        // Save Onclick 
        [HttpPost]
        public ActionResult Save_AjexOnClick(List<UpdateModel> pUpdateModel)
        {
            try
            {
                Boolean result = false;
                Boolean checkRemark = false;
                List<MoveLocation> objMoveLocationList = null;
                List<MoveLocation> objMoveLocationChildListTemp = null;
                string strMsg = string.Empty;

                if (pUpdateModel != null && this.model.MoveLocationList != null)
                {
                    if(pUpdateModel.Count == this.model.MoveLocationList.Count)
                    {
                        for (int index = 0; index < pUpdateModel.Count; index++)
                        {
                            this.model.MoveLocationList[index].Remark = pUpdateModel[0].Remark;
                        }
                    }
                }

                if (this.model.MoveLocationList != null)
                {
                    objMoveLocationList = this.model.MoveLocationList.Where(ml => ml.MoveId != 0).ToList();

                    if(objMoveLocationList != null)
                    {
                        foreach (MoveLocation en in objMoveLocationList)
                        {
                            if (en.Remark != null)
                            {
                                checkRemark = true;
                            }
                            else
                            {
                                checkRemark = false;
                                break;
                            }
                        }

                        if (checkRemark)
                        {
                            //Update Remark on Move Location
                            MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                            result = objMoveLocationService.UpdateMoveLocationData(objMoveLocationList, this.LoginUser.UserId);
                            strMsg = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;

                            //Get Monthly Close
                            string MonthlyClose = string.Empty;
                            MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                            MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                            if (objMonthlyClose != null)
                            {
                                DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                                string strFMonth = dFMonth.ToString("yyyy-MM");

                                MonthlyClose = strFMonth + "-01";
                            }

                            //Set Date
                            DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);
                            DateTime stockDate = DateTime.Now;

                            
                            if (result)
                            {
                                List<MoveLocation> objMoveLocationChildList = new List<MoveLocation>();

                                foreach (MoveLocation enMoveLocation in objMoveLocationList)
                                {
                                    objMoveLocationChildList.Clear();

                                    //1. PSC2210_T_HHT_MOVE_OUT
                                    ParsingMoveOut(ref objMoveLocationChildList, enMoveLocation.MoveId, Constants.HHTMoveOutStatus.SubmitTrans);

                                    //2. PSC2210_T_HHT_MOVE_IN
                                    ParsingMoveIn(ref objMoveLocationChildList, enMoveLocation.MoveId, Constants.HHTMoveInStatus.SubmitTrans);

                                    //3. PSC2410_T_HHT_MOVE_IN
                                    ParsingRelease(ref objMoveLocationChildList, enMoveLocation.MoveId, Constants.HHTReleaseStatus.Submit);

                                    if (objMoveLocationChildList != null)
                                    {
                                        objMoveLocationChildListTemp = objMoveLocationChildList.OrderBy(ml => ml.Time).ToList();
                                        if (objMoveLocationChildListTemp != null)
                                        {
                                            foreach (MoveLocation enTemp in objMoveLocationChildListTemp)
                                            {
                                                switch (enTemp.Type)
                                                {
                                                    case Constants.MovementType.Out:
                                                        StockListService objStockListService = new StockListService(this.dbEntities);
                                                        result = objStockListService.UpdateOutData(dateYearMonth, stockDate, enTemp.ItemCode, enTemp.HeatNo, enTemp.LocationCodeFrom, Convert.ToDecimal(enTemp.HHTTransQTY), this.LoginUser.UserId);
                                                        if (result)
                                                        {
                                                            MoveOutService objMoveOutUpdateService = new MoveOutService(this.dbEntities);
                                                            result = objMoveOutUpdateService.UpdateStatus(Convert.ToDecimal(enTemp.HHTTransId), Constants.HHTMoveOutStatus.ApproveTrans, this.LoginUser.UserId);
                                                            strMsg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                                        }
                                                        else
                                                        {
                                                            strMsg = strMsg + "  " + Resources.Common_cshtml.HeatNo + " = " + enTemp.HeatNo + " (" + Resources.Common_cshtml.FromLocation + " : " + GetDisplayLocation(enTemp.LocationCodeFrom) + ")";
                                                            break;
                                                        }
                                                        break;

                                                    case Constants.MovementType.In:
                                                        StockListService objStockListService1 = new StockListService(this.dbEntities);
                                                        result = objStockListService1.UpdateInData(dateYearMonth, stockDate, enTemp.ItemCode, enTemp.HeatNo, enTemp.LocationCodeTo, Convert.ToDecimal(enTemp.HHTTransQTY), this.LoginUser.UserId);

                                                        if (result)
                                                        {
                                                            MoveINService objMoveINServiceUpdateService = new MoveINService(this.dbEntities);
                                                            result = objMoveINServiceUpdateService.UpdateStatus(Convert.ToDecimal(enTemp.HHTTransId), Constants.HHTMoveInStatus.ApproveTrans, this.LoginUser.UserId);
                                                            strMsg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                                        }
                                                        else
                                                        {
                                                            strMsg = strMsg + "  " + Resources.Common_cshtml.HeatNo + " = " + enTemp.HeatNo + " (" + Resources.Common_cshtml.DestinationLocation + " : " + GetDisplayLocation(enTemp.LocationCodeTo) + ")";
                                                            break;
                                                        }

                                                        break;

                                                    case Constants.MovementType.Release:
                                                        StockListService objStockListService2 = new StockListService(this.dbEntities);
                                                        result = objStockListService2.UpdateInData(dateYearMonth, stockDate, enTemp.ItemCode, enTemp.HeatNo, enTemp.LocationCodeTo, Convert.ToDecimal(enTemp.HHTTransQTY), this.LoginUser.UserId);

                                                        if (result)
                                                        {
                                                            ReleaseService objReleaseUpdateService = new ReleaseService(this.dbEntities);
                                                            result = objReleaseUpdateService.UpdateStatus(Convert.ToDecimal(enTemp.HHTTransId), Constants.HHTReleaseStatus.Approve, this.LoginUser.UserId);
                                                            strMsg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                                        }
                                                        else
                                                        {
                                                            strMsg = strMsg + "  " + Resources.Common_cshtml.HeatNo + " = " + enTemp.HeatNo + " (" + Resources.Common_cshtml.DestinationLocation + " : " + GetDisplayLocation(enTemp.LocationCodeTo) + ")";
                                                            break;
                                                        }

                                                        break;
                                                }
                                            }
                                        }

                                        if (objMoveLocationChildListTemp != null)
                                        {
                                            objMoveLocationChildListTemp.Clear();
                                        }
                                        objMoveLocationChildListTemp = objMoveLocationChildList.Where(ml => ml.Type == Constants.MovementType.Release).ToList();
                                        if (objMoveLocationChildListTemp != null)
                                        {
                                            foreach(MoveLocation enMoveReleaseHHT in objMoveLocationChildListTemp)
                                            {
                                                UpdateReleaseDetail(enMoveLocation.MoveId, enMoveReleaseHHT.HHTJobNo, enMoveReleaseHHT.HHTTransQTY);
                                            }
                                        }
                                    }
                                }
                            }

                            // Alert Message
                            this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                            this.model.Message = strMsg;
                        }
                        else
                        {
                            this.model.AlertsType = Constants.AlertsType.Danger;
                            this.model.Message = Resources.Common_cshtml.CheckSaveAllRemarksMsg;
                        }
                    }
                }
                
                return View("PSC2210", this.model);
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, PSCS.Resources.PSC2210_cshtml.ErrorNoUser);
            }
        }

        private Boolean UpdateReleaseDetail(decimal moveId, string jobNo, decimal? actualQty)
        {
            Boolean result = false;
            //1. Find JOB_NO at PSC2410_T_HHT_RELEASE by MOVE_ID 
            List<Release> objReleaseJOBNo = new List<Release>();
            MoveLocationService objMoveLocationServiceReleaseJOBNO = new MoveLocationService(this.dbEntities);
            objReleaseJOBNo = objMoveLocationServiceReleaseJOBNO.Get2410ReleaseByMoveID(moveId);

            var jobNos = new List<string>();
            var ReqsQTYbyJobNos = new List<decimal>();
            var requestIDs = new List<decimal>();
            var releaseIDs = new List<decimal>();
            string jobNoTxt = "";


            if (objReleaseJOBNo != null && objReleaseJOBNo.Count > 0)
            {
                foreach (Release enJobNo in objReleaseJOBNo)
                {
                    jobNoTxt = enJobNo.JobNo;
                    if (jobNoTxt != "")
                    {
                        string[] selectedjobNoTxt = jobNoTxt.Split(',');
                        if (selectedjobNoTxt != null && selectedjobNoTxt.Length > 0)
                        {
                            for (int index = 0; index < selectedjobNoTxt.Length; ++index)
                            {
                                selectedjobNoTxt[index] = selectedjobNoTxt[index].Trim();
                            }
                            decimal? reqQTY = 0;
                            reqQTY = 0;
                            for (int index = 0; index < selectedjobNoTxt.Length; ++index)
                            {
                                if (!jobNos.Contains(selectedjobNoTxt[index]))
                                {
                                    jobNos.Add(selectedjobNoTxt[index].Trim());

                                    //2.Find Request_ID at PSC2420_T_REQUEST by JOB_NO
                                    List<Request> ListReleaseReqID = new List<Request>();
                                    MoveLocationService objMoveLocationServiceReqID = new MoveLocationService(this.dbEntities);
                                    ListReleaseReqID = objMoveLocationServiceReqID.Get2420RequestByJobNo(selectedjobNoTxt[index]);
                                    if (ListReleaseReqID != null && ListReleaseReqID.Count > 0)
                                    {
                                        foreach (Request enReqID in ListReleaseReqID)
                                        {
                                            if (!requestIDs.Contains(enReqID.RequestId))
                                            {
                                                requestIDs.Add(enReqID.RequestId);
                                            }

                                            //Collect Request_Qty 
                                            reqQTY += enReqID.RequestQTY.GetValueOrDefault(0);

                                            //3.Find Release_ID at PSC2421_T_REQUEST_RELEASE by REQUEST_ID
                                            List<RequestRelease> objReleaseReleaseID = new List<RequestRelease>();
                                            MoveLocationService objMoveLocationServiceREleaseID = new MoveLocationService(this.dbEntities);
                                            objReleaseReleaseID = objMoveLocationServiceREleaseID.Get2421RequestReleaseByRequestID(enReqID.RequestId);

                                            if (objReleaseReleaseID != null && objReleaseReleaseID.Count > 0)
                                            {
                                                foreach (RequestRelease enReleaseID in objReleaseReleaseID)
                                                {
                                                    if(enReleaseID.JobNo.Equals(jobNo))
                                                    {
                                                        if (!releaseIDs.Contains(enReleaseID.ReleaseId))
                                                        {
                                                            releaseIDs.Add(enReleaseID.ReleaseId);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (jobNo.Contains(enReleaseID.JobNo))
                                                        {
                                                            if (!releaseIDs.Contains(enReleaseID.ReleaseId))
                                                            {
                                                                releaseIDs.Add(enReleaseID.ReleaseId);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        ReqsQTYbyJobNos.Add(reqQTY.GetValueOrDefault(0));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //4. UPDATE QTY at PSC2411_T_RELEASE_DETAIL by RELEASE_ID
            // GET PSC2411_T_RELEASE_DETAIL BY RELEASE_ID 
            decimal? oldReleaseDetail_ActualQty = 0;
            decimal? oldReleaseDetail_RequestQty = 0;
            decimal? newReleaseDetail_ActualQty = 0;
            Boolean updateReleaseFlg = false;
            string userId = this.LoginUser.UserId;
            decimal pReleaseID = 0;

            if (releaseIDs != null && releaseIDs.Count > 0)
            {
                pReleaseID = releaseIDs[0];
                ReleaseDetail objReleaseDetail = null;
                ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                objReleaseDetail = objReleaseDetailService.GetReleaseDetail(pReleaseID);
                if (objReleaseDetail != null)
                {
                    oldReleaseDetail_ActualQty = objReleaseDetail.ActualQTY.HasValue ? objReleaseDetail.ActualQTY : 0;
                    oldReleaseDetail_RequestQty = objReleaseDetail.RequestQTY.HasValue ? objReleaseDetail.RequestQTY : 0;
                }

                newReleaseDetail_ActualQty = oldReleaseDetail_ActualQty + actualQty;
                if (newReleaseDetail_ActualQty == oldReleaseDetail_RequestQty)
                {
                    //Update status
                    updateReleaseFlg = true;
                }

                //Update PSC2411_T_RELEASE_DETAIL               
                ReleaseDetailService objUpReleaseDetail = new ReleaseDetailService(this.dbEntities);
                result = objUpReleaseDetail.UpdateReleaseDetailData(releaseIDs[0], newReleaseDetail_ActualQty, updateReleaseFlg, userId);
            }

            //5. UPDATE PSC2420_T_REQUEST by JOB_NO when releaseflg = true
            if (result == true && jobNos != null && jobNos.Count > 0 && updateReleaseFlg == true)
            {
                foreach (string strJobNo in jobNos)
                {   //UpdateRequestStatus
                    RequestService objUpRequest = new RequestService(this.dbEntities);
                    result = objUpRequest.UpdateRequestStatus(strJobNo, userId);
                    if (result == false)
                    {
                        break;
                    }
                }
            }

            ////6. UPDATE PSC2410_T_RELEASE by JobNo
            //decimal relQTY = 0;
            //decimal oldrelReleaseQTY = 0;
            //decimal newrelReleaseQTY = 0;
            //Boolean chkUpdateRelease = false;
            //if (result == true && jobNos != null && jobNos.Count > 0 && updateReleaseFlg == true)
            //{
            //    for (int i = 0; i < jobNos.Count; ++i)
            //    {
            //        chkUpdateRelease = false;
            //        Release getRelease = null;
            //        //PSCS RELEASE TABLE
            //        ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
            //        getRelease = objReleaseService.GetReleaseData(jobNos[i].ToString());

            //        if (getRelease != null)
            //        {
            //            relQTY = getRelease.QTY;
            //            oldrelReleaseQTY = getRelease.ReleaseQTY.GetValueOrDefault(0);
            //            newrelReleaseQTY = ReqsQTYbyJobNos[i];

            //            if (newrelReleaseQTY == relQTY)
            //            {
            //                chkUpdateRelease = true;
            //            }

            //            //UPDATE
            //            //Update PSC2411_T_RELEASE_DETAIL               
            //            ReleaseService objUpRelease = new ReleaseService(this.dbEntities);
            //            result = objUpRelease.Update2410Release(jobNos[i].ToString(), newrelReleaseQTY, chkUpdateRelease, userId);
            //            if (result == false)
            //            {
            //                break;
            //            }
            //        }
            //    }
            //}

            return result;
        }

        public bool LoadData(DateTime pMovementDate)
        {
            bool result = true;

            try
            {
                //1. PSC2210_T_HHT_MOVE_OUT  =>   PSC2210_T_MOVE_LOCATION
                result = SaveOutToMoveLocation(pMovementDate);

                //2. PSC2210_T_HHT_MOVE_IN  =>   PSC2210_T_MOVE_LOCATION
                result = SaveInToMoveLocation(pMovementDate);

                //3. PSC2410_T_HHT_MOVE_IN  =>   PSC2210_T_MOVE_LOCATION
                result = SaveReleaseToMoveLocation(pMovementDate);

                return result;
            }
            catch (Exception ex)
            {
                //this.model.AlertsType = Common.Constants.AlertsType.Danger;
                //this.model.Message = ex.Message;
                //this.PrintError(ex.Message);

                return false;
            }
        }

        private Boolean SaveOutToMoveLocation(DateTime pMovementDate)
        {
            Boolean result = true;
            Boolean IsUpdate = false;

            try
            {
                //1. PSC2210_T_HHT_MOVE_OUT  =>   PSC2210_T_MOVE_LOCATION
                List<MoveLocation> checkMoveLocation = new List<MoveLocation>();
                

                //1.1 Select PSC2210_T_HHT_MOVE_OUT where submit,moveid = null 
                List<MoveOut> moveOutList = new List<MoveOut>();
                MoveOutService objMoveOutService = new MoveOutService(this.dbEntities);
                moveOutList = objMoveOutService.GetMoveOutListByNullMoveIdandSubmitStatus(pMovementDate);

                //1.2 Check data with PSC2210_T_MOVE_LOCATION
                if (moveOutList != null)
                {
                    foreach (MoveOut enMoveOut in moveOutList)
                    {
                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        checkMoveLocation = objMoveLocationService.GetMoveLocationListByItemCodeAndHeatNo(pMovementDate, enMoveOut.ITEM_CODE, enMoveOut.HEAT_NO);

                        if (checkMoveLocation == null)
                        {
                            MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                            result = objMoveLocationService1.InsertOutByItemcodeAndHeatNo(pMovementDate, enMoveOut, this.LoginUser);
                        }
                        else
                        {
                            if(checkMoveLocation.Count == 0)
                            {
                                MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                result = objMoveLocationService1.InsertOutByItemcodeAndHeatNo(pMovementDate, enMoveOut, this.LoginUser);
                            }
                            else
                            {
                                for (int index = 0; index < checkMoveLocation.Count; index++)
                                {
                                    if (checkMoveLocation[index].Status == (int)Common.Constants.MoveLocationStatus.Submit)
                                    {
                                        MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                        result = objMoveLocationService1.UpdateQtyFromMoveOutItemcodeAndHeatNo(pMovementDate, enMoveOut, this.LoginUser);
                                        IsUpdate = true;
                                    }
                                }
                                if (IsUpdate == false)
                                {
                                    MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                    result = objMoveLocationService1.InsertOutByItemcodeAndHeatNo(pMovementDate, enMoveOut, this.LoginUser);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return result;
        }

        private Boolean SaveInToMoveLocation(DateTime pMovementDate)
        {
            //2. PSC2210_T_HHT_MOVE_IN  =>   PSC2210_T_MOVE_LOCATION
            Boolean result = true;
            Boolean IsUpdateMoveIn = false;

            try
            {
                List<MoveLocation> checkMoveLocation = new List<MoveLocation>();

                List<MoveIN> moveINList = new List<MoveIN>();
                MoveINService objMoveINService = new MoveINService(this.dbEntities);
                moveINList = objMoveINService.GetMoveINListByNullMoveIdandSubmitStatus(pMovementDate);

                if (moveINList != null)
                {
                    foreach (MoveIN enMoveIN in moveINList)
                    {
                        IsUpdateMoveIn = false;

                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        checkMoveLocation = objMoveLocationService.GetMoveLocationListByItemHeadLocationTo(pMovementDate, enMoveIN.ITEM_CODE, enMoveIN.HEAT_NO);

                        if (checkMoveLocation == null)
                        {
                            MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                            result = objMoveLocationService1.InsertInByItemcodeAndHeatNo(pMovementDate, enMoveIN, this.LoginUser);
                        }
                        else
                        {
                            if (checkMoveLocation.Count == 0)
                            {
                                MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                result = objMoveLocationService1.InsertInByItemcodeAndHeatNo(pMovementDate, enMoveIN, this.LoginUser);
                            }
                            else if (checkMoveLocation.Count == 1)
                            {
                                decimal decMoveId = checkMoveLocation[0].MoveId;
                                MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                IsUpdateMoveIn = objMoveLocationService1.UpdateQtyFromMoveIN(decMoveId, enMoveIN, this.LoginUser);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return result;
        }

        private Boolean SaveReleaseToMoveLocation(DateTime pMovementDate)
        {
            Boolean result = true;
            Boolean IsUpdateRelease = false;

            try
            {
                List<MoveLocation> checkMoveLocation = new List<MoveLocation>();

                List<Release> ReleaseList = new List<Release>();
                ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                ReleaseList = objReleaseService.GetReleaseListByNullMoveIdandSubmitStatus(pMovementDate);

                if (ReleaseList != null && ReleaseList.Count > 0)
                {
                    foreach (Release enRelease in ReleaseList)
                    {
                        MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                        checkMoveLocation = objMoveLocationService.GetMoveLocationListByItemHeadLocationTo(pMovementDate, enRelease.ItemCode, enRelease.HeatNo);

                        if (checkMoveLocation == null)
                        {
                            MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                            result = objMoveLocationService1.InsertReleaseByItemcodeAndHeatNo(pMovementDate, enRelease, this.LoginUser);
                        }
                        else
                        {
                            if (checkMoveLocation.Count == 0)
                            {
                                MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                result = objMoveLocationService1.InsertReleaseByItemcodeAndHeatNo(pMovementDate, enRelease, this.LoginUser);
                            }
                            else if (checkMoveLocation.Count == 1)
                            {
                                decimal decMoveId = checkMoveLocation[0].MoveId;
                                MoveLocationService objMoveLocationService1 = new MoveLocationService(this.dbEntities);
                                IsUpdateRelease = objMoveLocationService1.UpdateQtyFromRelease(decMoveId, enRelease, this.LoginUser);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return result;
        }

        private string GetDisplayLocation(string pLocationCode)
        {
            string result = string.Empty;

            try
            {
                LocationService objLocationService = new LocationService(this.dbEntities);
                Location objLocation = objLocationService.GetLocationByLocationCode(pLocationCode);
                if (objLocation != null)
                {
                    result = objLocation.YardName + "-" + objLocation.Name;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        [HttpPost]
        public string GetMoveLocationdata(decimal pMoveId)
        {
            string result = string.Empty;

            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                result = objMoveLocationService.GetMoveLocationDataByMoveId(pMoveId);

                return result;
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return string.Empty;
            }
        }

        [HttpPost]
        public string GetMoveLocationdataNotMoveId(MovementScreen FilterModel, string pAllMoveId, string pMovedate, string pYardId)
        {
            string result = string.Empty;
            List<decimal> MoveIdList = new List<decimal>();

            try
            {
                if (pAllMoveId != null && pAllMoveId != "")
                {
                    string[] AllMoveIdList = pAllMoveId.Split(',');

                    if (AllMoveIdList != null)
                    {
                        for (int i = 0; i < AllMoveIdList.Length; i++)
                        {
                            if (AllMoveIdList[i] != "")
                            {
                                MoveIdList.Add(Convert.ToDecimal(AllMoveIdList[i]));
                            }

                        }
                    }
                }

                DateTime deliveryDate = Convert.ToDateTime(pMovedate).Date;
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                result = objMoveLocationService.GetReceivePlanListNotMoveId(deliveryDate, pYardId, MoveIdList);

                if (result != "")
                {
                    // Add new record to model 
                    string[] selectedList = result.Split('*');
                    if (selectedList != null && selectedList.Length > 0)
                    {
                        var tempMoveLocationList = this.model.MoveLocationList;
                        MoveLocation newMoveLocation = new MoveLocation();
                        newMoveLocation.MoveId = decimal.Parse(selectedList[0]);
                        tempMoveLocationList.Add(newMoveLocation);
                        this.model.MoveLocationList = tempMoveLocationList;
                    }
                }

                return result;
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return string.Empty;
            }
        }

        public ActionResult DeleteMoveLocation(string pTransId, string pType)
        {
            try
            {
                Boolean result = false;
                decimal decTransId = 0;

                pTransId = pTransId.Trim();
                pType = pType.Trim();

                decTransId = pTransId == string.Empty ? 0 : Convert.ToDecimal(pTransId);
                Common.Constants.MovementType MyStatus = (Common.Constants.MovementType)Enum.Parse(typeof(Common.Constants.MovementType), pType, true);

                switch(MyStatus)
                {
                    case Constants.MovementType.Out:
                        //1. PSC2210_T_HHT_MOVE_OUT
                        MoveOutService objMoveOutService = null;
                        objMoveOutService = new MoveOutService(this.dbEntities);
                        result = objMoveOutService.UpdateDeleteStatus(decTransId, this.LoginUser.UpdateUserID);
                        break;

                    case Constants.MovementType.In: 
                        //2. PSC2210_T_HHT_MOVE_IN
                        MoveINService objMoveINService = null;
                        objMoveINService = new MoveINService(this.dbEntities);
                        result = objMoveINService.UpdateDeleteStatus(decTransId, this.LoginUser.UpdateUserID);
                        break;

                    case Constants.MovementType.Release:
                        //3. PSC2410_T_HHT_MOVE_IN
                        ReleaseService objReleaseService = null;

                        objReleaseService = new ReleaseService(this.dbEntities);
                        result = objReleaseService.UpdateDeleteStatus(decTransId, this.LoginUser.UpdateUserID);
                        break;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }

            return View("", null);
        }

        //private Boolean ClearGrid()
        //{
        //    Boolean result = false;

        //    List<MoveLocation> obj = new List<MoveLocation>();
        //    this.model.MoveLocationList = obj;
        //    this.model.Total = "0";

        //    return result;
        //}
        
        

        
        // Parent view: Approve Onclick 
        private bool Approve_MoveLocationList()
        {
            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var pDataList = this.model.MoveLocationList;
                string message = string.Empty;
                Boolean result = false;

                if (pDataList != null && LoginUser != null)
                {
                    result = objMoveLocationService.ApproveMoveLocation(pDataList, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Parent view: Delete Onclick 
        private bool Delete_MoveLocationList(int rowId)
        {
            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var pDataList = this.model.MoveLocationList;
                string message = string.Empty;
                Boolean result = false;
                //decimal MoveId = 0;
                if (pDataList != null && LoginUser != null)
                {
                    //MoveId = pDataList[rowId - 1].MoveId;
                    result = objMoveLocationService.DeleteMoveLocation(rowId);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<SelectListItem> GetYard()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            YardService objYardService = new YardService(this.dbEntities);
            var objYardList = objYardService.GetYardList();

            foreach (Yard objYard in objYardList)
            {
                result.Add(new SelectListItem { Text = objYard.Name, Value = objYard.YardID.ToString() });
            }

            return result;
        }

        private List<SelectListItem> GetHours()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = 0; i < 24; i++)
            {
                result.Add(new SelectListItem { Text = i.ToString("00"), Value = i.ToString("00") });
            }

            return result;
        }

        private List<SelectListItem> GetMinute()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = 0; i < 60; i++)
            {
                result.Add(new SelectListItem { Text = i.ToString("00"), Value = i.ToString() });
            }

            return result;
        }

        #region "Un Use"

        // Save Onclick 
        [HttpPost]
        public ActionResult Update_MoveLocationList(List<UpdateModel> pUpdateList)
        {
            try
            {
                var result = true;
                var msg = string.Empty;
                Boolean checkRemark = true;

                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var tempMoveLocationList = this.model.MoveLocationList;

                for (int i = 0; i < this.model.MoveLocationList.Count; i++)
                {
                    if (pUpdateList[i].Remark != null)
                    {
                        tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
                        checkRemark = true;
                    }
                    else
                    {
                        checkRemark = false;
                        break;
                    }
                }
                if (checkRemark == true)
                {
                    for (int i = 0; i < this.model.MoveLocationList.Count; i++)
                    {
                        tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
                    }

                    result = objMoveLocationService.UpdateMoveLocationData(tempMoveLocationList, userId);
                    msg = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                    this.model.MoveLocationList = tempMoveLocationList;

                    // Alert Message
                    this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                    this.model.Message = msg;
                    ModelState.Clear();
                }
                else
                {
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = Resources.Common_cshtml.CheckSaveAllRemarksMsg;
                    result = false;
                    msg = Resources.Common_cshtml.CheckAllRemarksMsg;
                }

                return Json(new { success = result, message = msg });
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, PSCS.Resources.PSC2210_cshtml.ErrorNoUser);
            }
        }

        // Approve Onclick 
        [HttpPost]
        public ActionResult Approve_MoveLocationList(List<UpdateModel> pUpdateList)
        {
            try
            {
                MoveLocationService objMoveLocationService = new MoveLocationService(this.dbEntities);
                string userId = this.LoginUser.UserId;
                var tempMoveLocationList = this.model.MoveLocationList;

                Boolean checkRemark = true;
                Boolean checkDestination = true;

                string MonthlyClose = string.Empty;
                MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
                MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
                if (objMonthlyClose != null)
                {
                    DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
                    string strFMonth = dFMonth.ToString("yyyy-MM");

                    MonthlyClose = strFMonth + "-01";
                }


                for (int i = 0; i < this.model.MoveLocationList.Count; i++)
                {
                    if (pUpdateList[i].Remark != null)
                    {
                        tempMoveLocationList[i].Remark = pUpdateList[i].Remark;
                        checkRemark = true;
                    }
                    else
                    {
                        checkRemark = false;
                        break;
                    }

                    if (pUpdateList[i].Destination != null)
                    {
                        tempMoveLocationList[i].LocationCodeTo = pUpdateList[i].Destination;
                        checkDestination = true;
                    }
                    else
                    {
                        checkDestination = false;
                        break;
                    }

                }

                if (checkRemark == true && checkDestination == true)
                {
                    //Save
                    var result = objMoveLocationService.UpdateMoveLocationData(tempMoveLocationList, userId);
                    var msg = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;
                    this.model.MoveLocationList = tempMoveLocationList;

                    if (result == true)
                    {
                        List<MoveLocation> pDataList = new List<MoveLocation>();
                        pDataList = this.model.MoveLocationList;
                        //Approve
                        //Approve (Update Status)
                        foreach (MoveLocation en in pDataList)
                        {
                            DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);
                            DateTime stockDate = DateTime.Now;//DateTime.ParseExact(iString, "yyyy-MM-dd", null);

                            StockListService objStockListService = new StockListService(this.dbEntities);

                            List<MoveLocation> objMLoOut = new List<MoveLocation>();
                            MoveLocationService objMoveLocationServiceOut = new MoveLocationService(this.dbEntities);
                            objMLoOut = objMoveLocationServiceOut.GetMoveLocationByMoveId(en.MoveId);

                            if (objMLoOut != null && objMLoOut.Count > 0)
                            {
                                result = objStockListService.UpdateOutData(dateYearMonth, stockDate, objMLoOut[0].ItemCode, objMLoOut[0].HeatNo, objMLoOut[0].LocationCodeFrom, Convert.ToDecimal(objMLoOut[0].QTY), userId);
                                msg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsgNotInStock;
                                if (!result)
                                {
                                    msg = msg + "  " + Resources.Common_cshtml.HeatNo + " = " + objMLoOut[0].HeatNo + " (" + Resources.Common_cshtml.FromLocation + " : " + GetDisplayLocation(objMLoOut[0].LocationCodeFrom) + ", " + Resources.Common_cshtml.DestinationLocation + " : " + GetDisplayLocation(objMLoOut[0].LocationCodeTo) + ")";
                                    break;
                                }
                            }
                        }

                        if (result == true)
                        {
                            foreach (MoveLocation en in pDataList)
                            {
                                //string strYearMonth = "2019-04-01"; //PSC3010_M_MONTHLY_CLOSE
                                DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);
                                DateTime dateStock = DateTime.Now;

                                StockListService objStockListInService = new StockListService(this.dbEntities);

                                List<MoveLocation> objMLoIn = new List<MoveLocation>();
                                MoveLocationService objMoveLocationServiceIn = new MoveLocationService(this.dbEntities);
                                objMLoIn = objMoveLocationServiceIn.GetMoveLocationByMoveId(en.MoveId);
                                if (objMLoIn != null && objMLoIn.Count > 0)
                                {
                                    result = objStockListInService.UpdateInData(dateYearMonth, dateStock, objMLoIn[0].ItemCode, objMLoIn[0].HeatNo, objMLoIn[0].LocationCodeTo, Convert.ToDecimal(objMLoIn[0].QTY), userId);
                                }
                            }

                            MoveLocationService objMoveLocation = new MoveLocationService(this.dbEntities);
                            result = objMoveLocation.ApproveMoveLocation(pDataList, userId);
                            msg = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;
                        }

                        // if IN FROM Release (IS_RELEAE = 1) Update  PSC2411_T_RELEASE_DETAIL
                        decimal? MoveLoQTY = 0;
                        if (result == true)
                        {
                            foreach (MoveLocation en in pDataList)
                            {
                                List<MoveLocation> objMLoRelease = new List<MoveLocation>();
                                MoveLocationService objMoveLocationServiceRelease = new MoveLocationService(this.dbEntities);
                                objMLoRelease = objMoveLocationServiceRelease.GetMoveLocationByMoveId(en.MoveId);

                                if (objMLoRelease != null && objMLoRelease.Count > 0)
                                {
                                    MoveLoQTY = objMLoRelease[0].QTY.HasValue ? objMLoRelease[0].QTY : 0;
                                    if (objMLoRelease[0].Is_Release == (int)Common.Constants.MoveLocationIn.Release)
                                    {
                                        result = UpdateReleaseDetail(en.MoveId, en.HHTJobNo, MoveLoQTY);
                                    }
                                }
                            }
                        }
                    }

                    //Alert Message
                    this.model.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
                    this.model.Message = msg;
                    ModelState.Clear();
                    return Json(new { success = result, message = msg });
                }
                else
                {
                    string msg = string.Empty;

                    if (checkDestination == false)
                    {
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.Common_cshtml.CheckAllDestinationsMsg;
                        msg = Resources.Common_cshtml.CheckAllDestinationsMsg;
                        //this.PrintError(Resources.Common_cshtml.CheckAllDestinationsMsg);
                    }
                    else if (checkRemark == false)
                    {
                        this.model.AlertsType = Constants.AlertsType.Danger;
                        this.model.Message = Resources.Common_cshtml.CheckAllRemarksMsg;
                        msg = Resources.Common_cshtml.CheckAllRemarksMsg;
                    }

                    return Json(new { success = false, message = msg });
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        #endregion
    }
}