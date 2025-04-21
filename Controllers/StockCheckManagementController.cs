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
    public class StockCheckManagementController : BaseController
    {
        public StockCheckManagementScreen myModel
        {
            get
            {
                if (Session["StockCheckManagementScreen"] == null)
                {
                    Session["StockCheckManagementScreen"] = new StockCheckManagementScreen();
                    return (StockCheckManagementScreen)Session["StockCheckManagementScreen"];
                }
                else
                {
                    return (StockCheckManagementScreen)Session["StockCheckManagementScreen"];
                }
            }

            set { Session["StockCheckManagementScreen"] = value; }
        }

        public StockCheckManagementScreenEdit myModelEdit
        {
            get
            {
                if (Session["StockCheckManagementScreenEdit"] == null)
                {
                    Session["StockCheckManagementScreenEdit"] = new StockCheckManagementScreenEdit();
                    return (StockCheckManagementScreenEdit)Session["StockCheckManagementScreenEdit"];
                }
                else
                {
                    return (StockCheckManagementScreenEdit)Session["StockCheckManagementScreenEdit"];
                }
            }

            set { Session["StockCheckManagementScreenEdit"] = value; }
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult PSC2310()
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                InitializeActionName = "PSC2310";
                QueryStringList = new Dictionary<string, string>();
                myModel.YardList = GetYard();
                myModel.StatusList = GetStatus();
                myModel.DataList = GetStockCheckManagement();
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

        [HttpGet]
        public ActionResult PSC2310_Edit(string row_no)
        {
            try
            {
                // Initial model
                ViewBag.LoginUserName = this.LoginUser.UserId;
                this.myModel.hasMessage = false;

                myModelEdit.YardList = GetYard();
                myModelEdit.StatusList = GetStatus();

                int rowNo = 0;
                if (int.TryParse(row_no, out rowNo))
                {
                    List<StockCheckManagement> lst = this.myModel.DataList;
                    StockCheckManagement target = lst.Where(x => x.RowNo == rowNo).FirstOrDefault();
                    if (target == null)
                    {
                        this.myModel.hasMessage = true;
                        this.myModel.AlertsType = Common.Constants.AlertsType.Infomation;
                        this.myModel.Message = "Don't have Description of Stock Check.";
                        return RedirectToAction("PSC2310", "StockCheckManagement");
                    }

                    myModelEdit.DataList = GetStockCheckManagementDetail(target.Yard);

                    myModelEdit.DetailYard = target.Yard;
                    myModelEdit.DetailStockCheckDate = target.StockCheckDate.ToString(Common.Constants.DATE_HYPHEN);
                    myModelEdit.DetailStatus = target.Status;
                    myModelEdit.EditMode = "Edit";
                }
                else
                {
                    myModelEdit.DataList = new List<StockCheckManagementDetail>();
                    myModelEdit.DetailYard = "1";
                    myModelEdit.DetailStockCheckDate = DateTime.Now.ToString(Common.Constants.DATE_HYPHEN);
                    myModelEdit.DetailStatus = "1";
                    myModelEdit.EditMode = "Create";
                }

            }
            catch (Exception ex)
            {
                this.myModel.hasMessage = true;
                this.myModel.AlertsType = Common.Constants.AlertsType.Danger;
                this.myModel.Message = ex.Message;
                this.PrintError(ex.Message);
                return RedirectToAction("PSC2310", "StockCheckManagement");
            }

            return View(myModelEdit);
        }
        [HttpPost]
        public ActionResult PSC2310_Edit(string submitButton, StockCheckManagementScreenEdit modelEdit)
        {
            return RedirectToAction("PSC2310", "StockCheckManagement");
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

        private List<SelectListItem> GetStatus()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (Constants.StockTakingStatus enStstus in (Constants.StockTakingStatus[])Enum.GetValues(typeof(Constants.StockTakingStatus)))
            {
                result.Add(new SelectListItem { Text = enStstus.ToString(), Value = ((int)enStstus).ToString() });
            }

            return result;
        }

        private List<StockCheckManagement> GetStockCheckManagement()
        {
            StockCheckManagementService service = new StockCheckManagementService(this.dbEntities);
            return service.GetStockCheckManagementList();
        }

        private List<StockCheckManagementDetail> GetStockCheckManagementDetail(string yard)
        {
            StockCheckManagementService service = new StockCheckManagementService(this.dbEntities);
            List<StockCheckManagementDetail> lst = service.GetStockCheckManagementDetailList();

            return lst.Where(x => x.Yard.Equals(yard)).ToList();


        }
    }
}