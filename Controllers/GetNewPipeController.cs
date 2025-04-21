using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.Services;
using PSCS.ModelERPDEV01;
using PSCS.Models;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class GetNewPipeController : BaseController
    {
        public GetNewPipeScreen myModel
        {
            get
            {
                if (Session["GetNewPipeScreen"] == null)
                {
                    Session["GetNewPipeScreen"] = new GetNewPipeScreen();
                    return (GetNewPipeScreen)Session["GetNewPipeScreen"];
                }
                else
                {
                    return (GetNewPipeScreen)Session["GetNewPipeScreen"];
                }
            }
            set { Session["GetNewPipeScreen"] = value; }
        }

        // GET: GetNewPipe
        [NoDirectAccess]
        public ActionResult PSC3100()
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                InitializeActionName = "PSC3100";
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

        [NoDirectAccess]
        [HttpPost]
        public ActionResult PSC3100(string submitButton)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                switch (submitButton)
                {
                    case "Sync":
                        return (Sync_OnClick());
                    case "Back":
                        return RedirectToAction("PSC0100", "Menu");
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

        private ActionResult Sync_OnClick()
        {
            Boolean result = false;
            Boolean isPipeNotFound = false;
            Boolean isGetNewPipeFail = false;
            List<PipeReceivedData> objPipeReceivedDataList = null;
            List<PipeItemData> objPipeItemDataList = null;
            string strPipeError = string.Empty;

            try
            {
                GetNewPipeFromSyteLineService objGetNewPipeFromSyteLineService = new GetNewPipeFromSyteLineService(this.dbERPDEV01Entities);
                objPipeReceivedDataList = objGetNewPipeFromSyteLineService.GetNewPipeFromSyteLine();

                if (objPipeReceivedDataList == null)
                {
                    isPipeNotFound = true;
                }
                else
                {
                    if (objPipeReceivedDataList.Count == 0)
                    {
                        isPipeNotFound = true;
                    }
                    else
                    {
                        isPipeNotFound = false;

                        List<PipeReceivedData> objItemCodeHeatNoContainerNoDistinctList = (objPipeReceivedDataList.GroupBy(pi => new { pi.ContainerNo, pi.ItemCode, pi.HeatNo })
                                                                   .Select(m => new PipeReceivedData
                                                                   {
                                                                       ContainerNo = m.Key.ContainerNo,
                                                                       ItemCode = m.Key.ItemCode,
                                                                       HeatNo = m.Key.HeatNo
                                                                   })).ToList();

                        //List<PipeReceivedData> objAllList = objPipeReceivedDataList.Select(r => new PipeReceivedData { ItemCode = r.ItemCode, HeatNo = r.HeatNo }).ToList();
                        //List<PipeReceivedData> objDistinctList = objAllList.Distinct().ToList();
                        List<PipeReceivedData> objDistinctList = (objPipeReceivedDataList.GroupBy(pi => new { pi.ItemCode, pi.HeatNo})
                                                                    .Select(m => new PipeReceivedData
                                                                    {
                                                                        ItemCode = m.Key.ItemCode,
                                                                        HeatNo = m.Key.HeatNo
                                                                    })).ToList();
                        if (objDistinctList != null)
                        {
                            foreach (PipeReceivedData en in objDistinctList)
                            {
                                PipeItemService objPipeItemService = new PipeItemService(this.dbEntities);
                                PipeItem objPipeItem = objPipeItemService.GetPipeItem(en.ItemCode, en.HeatNo);
                                if (objPipeItem == null)
                                {
                                    objGetNewPipeFromSyteLineService = new GetNewPipeFromSyteLineService(this.dbERPDEV01Entities);
                                    PipeItemData objPipeItemData = objGetNewPipeFromSyteLineService.GetNewPipeItemFromSyteLine(en.ItemCode, en.HeatNo);
                                    if (objPipeItemData != null)
                                    {
                                        if (objPipeItemDataList == null)
                                        {
                                            objPipeItemDataList = new List<PipeItemData>();
                                        }
                                        objPipeItemDataList.Add(objPipeItemData);
                                    }
                                    else
                                    {
                                        strPipeError = " Item code : " + en.ItemCode + " Heat no :" + en.HeatNo;
                                        isGetNewPipeFail = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if(strPipeError == string.Empty)
                        {
                            result = UpdatePscsReceiveData(objPipeItemDataList, objPipeReceivedDataList);
                            if (result)
                            {
                                objGetNewPipeFromSyteLineService = new GetNewPipeFromSyteLineService(this.dbERPDEV01Entities);
                                result = objGetNewPipeFromSyteLineService.CallUpdatePipeProcedure();

                                if (result)
                                {
                                    isGetNewPipeFail = false;
                                }
                                else
                                {
                                    isGetNewPipeFail = true;
                                }
                            }
                            else
                            {
                                isGetNewPipeFail = true;
                            }
                        }
                        
                        if (isGetNewPipeFail)
                        {
                            this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                            this.myModel.Message = "Get new pipe fail, please contract administrator.";
                            if(strPipeError != string.Empty)
                            {
                                this.myModel.Message = this.myModel.Message + "pipe item not found (" + strPipeError +")";
                            }
                        }
                        else
                        {
                            this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                            this.myModel.Message = "Get new pipe QTY record(s) Complete, ".Replace("QTY", objPipeReceivedDataList.Count.ToString());
                        }
                    }
                }

                if (isPipeNotFound)
                {
                    this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                    this.myModel.Message = Resources.PSC3100_cshtml.NewPipesNotFound;
                }
                //var names = new int[] { 64695, 64696 };
                //List<PipeReceivedData> Templist = (from pi in this.dbEntities.PSC3100_T_SYTELINE_NEW_PIPE
                //                                 where names.Contains(pi.TRANS_NO)
                //                                 select new PipeReceivedData {
                //                                     TransNo = pi.TRANS_NO,
                //                                     ItemCode = pi.ITEM_CODE,
                //                                     HeatNo = pi.HEAT_NO,
                //                                     ContainerNo = pi.CONTAINER_NO,
                //                                     DeliveryDate = DateTime.Now,
                //                                     ReceivedDate = DateTime.Now,
                //                                 }).AsQueryable().ToList();

                //List<PipeReceivedData> Alllist = Templist.Select(r => new PipeReceivedData { DeliveryDate = r.DeliveryDate, ReceivedDate = r.ReceivedDate, ContainerNo = r.ContainerNo }).ToList();

                //List<PipeReceivedData> Distinctlist = (Alllist.GroupBy(d => new { d.DeliveryDate, d.ReceivedDate, d.ContainerNo })
                //.Select(m => new PipeReceivedData {
                //    DeliveryDate = m.Key.DeliveryDate,
                //    ReceivedDate = m.Key.ReceivedDate,
                //    ContainerNo = m.Key.ContainerNo })).ToList();

                //int? intReceiveId = this.dbEntities.PSC2110_T_RECEIVING_INSTRUCTION.Max(u => (int?)u.RECEIVE_ID);
                //int intNewReceiveId = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("PSC3100", this.myModel);
        }

        private Boolean UpdatePscsReceiveData(List<PipeItemData> pPipeItemDataList, List<PipeReceivedData> pPipeReceivedDataList)
        {
            Boolean result = false;

            try
            {
                ReceivingNewPipeService objReceivingNewPipeService = new ReceivingNewPipeService(this.dbEntities, this.LoginUser.UserId);
                result = objReceivingNewPipeService.ReceivingNewPipe(pPipeItemDataList,pPipeReceivedDataList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }
        #region CallService
        //private List<SyncReceiveDataScreen> GetReceiveingData()
        //{
        //    try
        //    {
        //        SyncReceiveDataService objSyncReceiveDataService = new SyncReceiveDataService(this.dbEntities);
        //        List<SyncReceiveDataScreen> RecieveDataList = objSyncReceiveDataService.GetActiveSyncReceiveData();
        //        return RecieveDataList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        
        #endregion

    }
}