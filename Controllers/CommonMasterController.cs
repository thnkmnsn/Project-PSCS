using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;
using PSCS.Services;
using PSCS.ModelsScreen;

namespace PSCS.Controllers
{
    public class CommonMasterController : BaseController
    {
        public CommonMasterScreen myModel
        {
            get
            {
                if (Session["CommonMasterScreen"] == null)
                {
                    Session["CommonMasterScreen"] = new CommonMasterScreen();
                    return (CommonMasterScreen)Session["CommonMasterScreen"];
                }
                else
                {
                    return (CommonMasterScreen)Session["CommonMasterScreen"];
                }
            }

            set { Session["CommonMasterScreen"] = value; }
        }

        public CommonMasterScreenEdit myModelEdit
        {
            get
            {
                if (Session["ModelCommonMasterScreenEdit"] == null)
                {
                    Session["ModelCommonMasterScreenEdit"] = new CommonMasterScreenEdit();
                    return (CommonMasterScreenEdit)Session["ModelCommonMasterScreenEdit"];
                }
                else
                {
                    return (CommonMasterScreenEdit)Session["ModelCommonMasterScreenEdit"];
                }
            }
            set { Session["ModelCommonMasterScreenEdit"] = value; }
        }

        [HttpGet]
        public ActionResult PSC8060()
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                InitializeActionName = "PSC8060";
                QueryStringList = new Dictionary<string, string>();

                // Initial service
                CommonMasterService objCommonMasterService = new CommonMasterService(this.dbEntities);

                this.myModel.CommonMasterList = objCommonMasterService.GetCommonMasterList();
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


        [HttpGet]
        public ActionResult PSC8060_Edit(string parentCode, string commonCode)
        {
            try
            {
                ViewBag.LoginUserName = this.LoginUser.UserId;

                CommonMasterService commonMasterService = new CommonMasterService(this.dbEntities);
                CommonMasterScreenEdit newModel = new CommonMasterScreenEdit();
                
                if (string.IsNullOrEmpty(parentCode))
                {
                    newModel.EditMode = "Add";
                    return View(newModel);
                }

                var result = commonMasterService.GetValueById(parentCode, commonCode);
                if (result != null)
                {

                    newModel.EditParentCode = result.EditParentCode;
                    newModel.EditCommonCode = result.EditCommonCode;
                    newModel.EditMode = "Edit";
                    newModel.EditValueEN = result.EditValueEN;
                    newModel.EditValueTH = result.EditValueTH;
                    return View(newModel);
                }
                else
                {
                    return HttpNotFound();
                }


            }
            catch (Exception ex)
            {
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);

                //Set for show Message Error if EditMode value is null or not edit or Add
                if (this.myModelEdit.EditMode == null || !(this.myModelEdit.EditMode.Equals("Edit") || this.myModelEdit.EditMode.Equals("Add")))
                {
                    this.myModelEdit.EditMode = "Edit";
                }
                return RedirectToAction("PSC8060", "CommonMaster");
            }
        }

        [HttpPost]
        public ActionResult PSC8060_Edit(string submitButton, CommonMasterScreenEdit editModel)
        {
            ViewBag.LoginUserName = this.LoginUser.UserId;
            
            Boolean result = false;

            this.myModel.AlertsType = Common.Constants.AlertsType.None;
            this.myModel.Message = string.Empty;

            if (!submitButton.Equals("Back"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        CommonMasterService commonMasterService = new CommonMasterService(this.dbEntities);
                        string userId = this.LoginUser.UserId;
                        switch (submitButton)
                        {

                            case "Update":
                                result = commonMasterService.Update(editModel, userId);
                                break;
                            case "Insert":
                                result = commonMasterService.Insert(editModel, userId);
                                break;
                            case "Delete":
                                result = commonMasterService.DeleteData(editModel);
                                break;
                            default:
                                return View(editModel);
                        }
                        if (result)
                        {
                            this.myModel.AlertsType = Common.Constants.AlertsType.Success;
                            
                            if (submitButton.Contains("Update"))
                            {
                                this.myModel.Message = PSCS.Resources.Common_cshtml.EditSuccessMsg;
                            }
                            else if (submitButton.Contains("Insert"))
                            {
                                this.myModel.Message = PSCS.Resources.Common_cshtml.AddSuccessMsg;
                            }
                            else if (submitButton.Contains("Delete"))
                            {
                                this.myModel.Message = PSCS.Resources.Common_cshtml.DeleteSuccessMsg;
                            }
                        }
                        else
                        {
                            this.myModel.AlertsType = Common.Constants.AlertsType.Danger;

                            if (submitButton.Contains("Update"))
                            {
                                this.myModel.Message = PSCS.Resources.Common_cshtml.EditFailMsg;
                            }
                            else if (submitButton.Contains("Insert"))
                            {
                                this.myModel.Message = PSCS.Resources.Common_cshtml.AddFailMsg;
                            }
                            else if (submitButton.Contains("Delete"))
                            {
                                this.myModel.Message = PSCS.Resources.Common_cshtml.DeleteFailMsg;
                            }
                        }
                        return RedirectToAction("PSC8060", "CommonMaster");
                    }

                    catch (Exception ex)
                    {
                        this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                        this.myModel.Message = ex.Message;
                        this.PrintError(ex.Message);
                        return RedirectToAction("PSC8060", "CommonMaster");
                    }

                }

                return View(editModel);
            }
            return RedirectToAction("PSC8060", "CommonMaster");
        }
    }
}