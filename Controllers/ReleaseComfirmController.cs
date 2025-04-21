using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Common;
using PSCS.ModelsScreen;
using PSCS.Models;

namespace PSCS.Controllers
{
    [SessionExpire]
    public class ReleaseComfirmController : BaseController
    {
        public ReleaseComfirmScreen model
        {
            get
            {
                if (Session["ReleaseComfirmScreen"] == null)
                {
                    Session["ReleaseComfirmScreen"] = new ReleaseComfirmScreen();
                    return (ReleaseComfirmScreen)Session["ReleaseComfirmScreen"];
                }
                else
                {
                    return (ReleaseComfirmScreen)Session["ReleaseComfirmScreen"];
                }
            }
            set { Session["ReleaseComfirmScreen"] = value; }
        }

        #region "Sub&Function"
        private void IntialPSC2420()
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            InitializeActionName = "PSC2420";
            QueryStringList = new Dictionary<string, string>();
        }
        
       private List<ReleaseDetail> CreateReleaseDetailList(List<Request> pRequest)
        {
            List<ReleaseDetail> result = null;
            //List<ReleaseDetail> objReleaseDetailList = null;
            //List<RequestRelease> objRequestReleaseList = null;
            //RequestRelease objRequestRelease = null;

            try
            {
                if(pRequest != null)
                {
                    pRequest.Distinct().ToList();
                    result = (pRequest.GroupBy(r => new { r.ItemCode, r.HeatNo })
                                                                    .Select((m , index) => new ReleaseDetail
                                                                    {
                                                                        RowNo = index + 1,
                                                                        ItemCode = m.Key.ItemCode,
                                                                        HeatNo = m.Key.HeatNo,
                                                                        Description = m.FirstOrDefault().Description,
                                                                        Maker = m.FirstOrDefault().Maker,
                                                                        Maker_Name = m.FirstOrDefault().Maker_Name,
                                                                        RequestDate = DateTime.Now,
                                                                        RequestQTY = m.Select(r => r.RequestQTY).Sum(),                                                                       
                                                                    })).ToList();

                    
                    
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private void GenerateRequestNo()
        {
            if (this.model.ReleaseDetailList != null && this.model.ReleaseDetailList.Count > 0)
            {
                //YYMMDDRRR, YY ปีคริสศักราช/MMเดือน/DDวันที่/RRRรันนิ่งนัมเบอร์ของแต่ละวัน

                int NextNo = 0;
                string fullDate = string.Empty;
                string format = "yyMMdd";
                fullDate = DateTime.Today.ToString(format);

                //get Last running number in day on DB 
                ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                NextNo = objReleaseDetailService.GetLastRequestNo(fullDate);

                for (int i = 0; i < this.model.ReleaseDetailList.Count; i++)
                {
                    NextNo = NextNo + 1;
                    this.model.ReleaseDetailList[i].RequestNo = fullDate.ToString() + NextNo.ToString().PadLeft(3, '0');
                }  
            }

        }

      private ActionResult Confirm_OnClick(string pUserId)
        {
            Boolean result = false;
            try
            {

                foreach (ReleaseDetail en in this.model.ReleaseDetailList)
                {
                    if (en.RequestNo == null)
                    {
                        GenerateRequestNo();
                        ReleaseDetailService objReleaseDetailService = new ReleaseDetailService(this.dbEntities);
                        result = objReleaseDetailService.SaveData(this.model.ReleaseDetailList, this.model.WIRequestList, pUserId);
                        break;
                    }
                }
                    
                if (result)
                {
                    this.model.AlertsType = Constants.AlertsType.Success;
                    ViewBag.DisbleBtn = true;
                    this.model.Message = PSCS.Resources.PSC2420_cshtml.SaveSuccessMsg;
                }
                else
                {
                    ViewBag.DisbleBtn = false;
                    this.model.AlertsType = Constants.AlertsType.Danger;
                    this.model.Message = PSCS.Resources.PSC2420_cshtml.SaveFailMsg;
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }

            return View(this.model);
        }

        #endregion
        
        #region "PSC2420 WI Confirm"
        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2420()
        {
            try
            {
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                // Initial View
                IntialPSC2420();
                
                RequestService objRequestService = new RequestService(this.dbEntities);
                this.model.WIRequestList = objRequestService.GetRequestsList(Constants.RequestStatus.Draft);
                this.model.ReleaseDetailList = CreateReleaseDetailList(this.model.WIRequestList);
                this.model.Total = this.model.ReleaseDetailList.Count.ToString();
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);
            }
            
            return View(this.model);
        }

      

        [HttpPost]
        public ActionResult PSC2420(string submitButton)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.model.AlertsType = Constants.AlertsType.None;
                this.model.Message = string.Empty;

                try
                {
                    ViewBag.LoginUserName = this.LoginUser.UserId;
                    switch (submitButton)
                    {
                        case "Back":
                            return RedirectToAction("PSC2410", "Release");
                        case "Confirm":
                            return Confirm_OnClick(this.LoginUser.UserId);
                        default:
                            return View(this.model);
                    }
                }
                catch (Exception ex)
                {
                    this.model.AlertsType = Common.Constants.AlertsType.Danger;
                    this.model.Message = ex.Message;
                    this.PrintError(ex.Message);
                    return View(this.model);
                }
            }
            catch (Exception ex)
            {
                this.model.AlertsType = Constants.AlertsType.Danger;
                this.model.Message = ex.Message;
                this.PrintError(ex.Message);

                return View(this.model);
            }
        }
        #endregion

    }
}