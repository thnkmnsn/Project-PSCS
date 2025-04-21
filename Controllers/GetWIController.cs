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
    public class GetWIController : BaseController
    {
        public GetWIScreen myModel
        {
            get
            {
                if (Session["GetWIScreen"] == null)
                {
                    Session["GetWIScreen"] = new GetWIScreen();
                    return (GetWIScreen)Session["GetWIScreen"];
                }
                else
                {
                    return (GetWIScreen)Session["GetWIScreen"];
                }
            }
            set { Session["GetWIScreen"] = value; }
        }


        // GET: GetWI
        public ActionResult PSC3300()
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                InitializeActionName = "PSC3300";
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

        [HttpPost]
        public ActionResult PSC3300(string submitButton)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;
                switch (submitButton)
                {
                    case "Sync":
                        return (Sync_OnClick());
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
            Release objRelease = null;
            Boolean isJobNotFound = false;
            List<JobMasterData> objJobMasterDataList = null;

            try
            {
                ReleaseService objReleaseService = new ReleaseService(this.dbEntities);
                objRelease = objReleaseService.GetReleaseData("PSCS000001");

                if (objRelease != null)
                {
                    result = true;
                    this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                    this.myModel.Message = "Get Job master complete.";
                }
                else
                {
                    GetJobMasterFromSyteLineService objGetWIFromSyteLineService = new GetJobMasterFromSyteLineService(this.dbERPDEV01Entities);
                    objJobMasterDataList = objGetWIFromSyteLineService.GetWIFromSyteLine("PSCS000001");

                    if (objJobMasterDataList == null)
                    {
                        isJobNotFound = true;
                    }
                    else
                    {
                        if (objJobMasterDataList.Count == 0)
                        {
                            isJobNotFound = true;
                        }
                        else
                        {
                            isJobNotFound = false;
                            JobMasterService objJobMasterService = new JobMasterService(this.dbEntities, this.LoginUser.UserId);
                            Boolean IsUpdate = objJobMasterService.JobMaster(objJobMasterDataList);
                            if (IsUpdate)
                            {
                                ReleaseService objRelease1Service = new ReleaseService(this.dbEntities);
                                objRelease = objRelease1Service.GetReleaseData("PSCS000001");

                                if (objRelease != null)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                            }

                            if (result)
                            {
                                this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                                this.myModel.Message = "Get Job master complete.";
                            }
                            else
                            {
                                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                                this.myModel.Message = "Get Job master Fail.";
                            }
                        }
                    }
                    if (isJobNotFound)
                    {
                        this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                        this.myModel.Message = "Get new pipe not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return View(this.myModel);
            }

            return View("PSC3300", this.myModel);
        }

        private Boolean UpdateJobMasterData(List<JobMasterData> pJobMasterDataList)
        {
            Boolean result = false;

            try
            {
                JobMasterService objJobMasterService = new JobMasterService(this.dbEntities, this.LoginUser.UserId);
                result = objJobMasterService.JobMaster(pJobMasterDataList);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}