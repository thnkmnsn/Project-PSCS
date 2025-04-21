using PSCS.Common;
using PSCS.Models;
using PSCS.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;
using PSCS.ModelsScreen;
using System.Net;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace PSCS.Controllers
{
	[SessionExpire]
	public class ReceivePlanAdjustController : BaseController
	{
		public ReceivePlanScreen model
		{
			get
			{
				if (Session["ModelReceivePlanScreen"] == null)
				{
					Session["ModelReceivePlanScreen"] = new ReceivePlanScreen();
					return (ReceivePlanScreen)Session["ModelReceivePlanScreen"];
				}
				else
				{
					return (ReceivePlanScreen)Session["ModelReceivePlanScreen"];
				}
			}
			set { Session["ModelReceivePlanScreen"] = value; }
		}

		public ReceivePlanDetailScreen modelEdit
		{
			get
			{
				if (Session["ReceivePlanDetailScreen"] == null)
				{
					Session["ReceivePlanDetailScreen"] = new ReceivePlanDetailScreen();
					return (ReceivePlanDetailScreen)Session["ReceivePlanDetailScreen"];
				}
				else
				{
					return (ReceivePlanDetailScreen)Session["ReceivePlanDetailScreen"];
				}
			}

			set { Session["ReceivePlanDetailScreen"] = value; }
		}

		[NoDirectAccess]
		[HttpGet]
		public ActionResult PSC2110()
		{
			try
			{
				// Initial View
				IntialPSC2110();
				if (modelEdit.HasError)
				{
					modelEdit.HasError = false;
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

			return View(this.model);
		}

		[HttpPost]
		public ActionResult PSC2110(ReceivePlanScreen FilterModel, string submitButton)
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
						return RedirectToAction("PSC2110", "ReceivePlan");

					case "Filter":
						return Filter_OnClick(FilterModel);

					case "ClearFilter":
						ModelState.SetModelValue("FilterDeliveryDate", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
						ModelState.SetModelValue("FilterContainerNo", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
						ModelState.SetModelValue("FilterHeatNo", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
						ModelState.SetModelValue("FilterOD", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
						ModelState.SetModelValue("FilterWT", new ValueProviderResult("", "", CultureInfo.InvariantCulture));
						ModelState.SetModelValue("FilterLength", new ValueProviderResult("", "", CultureInfo.InvariantCulture));

						FilterModel.FilterDeliveryDate = null;
						FilterModel.FilterContainerNo = "";
						FilterModel.FilterHeatNo = "";
						FilterModel.FilterOD = "";
						FilterModel.FilterWT = "";
						FilterModel.FilterLength = "";
						break;
				}

				return Filter_OnClick(FilterModel);
			}
			catch (Exception ex)
			{
				this.modelEdit.AlertsType = Constants.AlertsType.Danger;
				this.modelEdit.Message = ex.Message;
				this.PrintError(ex.Message);

				return View(this.modelEdit);
			}
		}

		[NoDirectAccess]
		[HttpGet]
		public ActionResult PSC2111(string _id)
		{
            ReceivingInstruction objReceivingInstruction = null;

			try
			{
				// Initial View
				ViewBag.LoginUserName = this.LoginUser.UserId;
				ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
				this.modelEdit.AlertsType = Constants.AlertsType.None;
				this.modelEdit.Message = "";
				
                if (this.LoginUser.RoleId == Constants.ROLE_YARDSUPERVISOR)
				{
					this.modelEdit.IsYardSupervisorRole = true;
					this.modelEdit.IsControllerRole = false;
				}
				else if (this.LoginUser.RoleId == Constants.ROLE_CONTROLLER)
				{
					this.modelEdit.IsYardSupervisorRole = false;
					this.modelEdit.IsControllerRole = true;
				}
				else
				{
					this.modelEdit.IsYardSupervisorRole = false;
					this.modelEdit.IsControllerRole = false;
				}
				this.modelEdit = new ReceivePlanDetailScreen
				{
					ReceivingInstructionDetailList = new List<ReceivingInstructionDetail>
					{
						new ReceivingInstructionDetail { LocationChange = "Value1" }
					},
					DropdownHtml = GetLocationList()
				};
				this.modelEdit.ReceiveId = Convert.ToInt32(_id);
				this.modelEdit.HoursList = GetHours();
				this.modelEdit.MinuteList = GetMinute();
                // Initial service
                ReceivingInstructionService objReceivingInstructionService = new ReceivingInstructionService(this.dbEntities);
				objReceivingInstruction = objReceivingInstructionService.GetReceivingInstruction(this.modelEdit.ReceiveId);
                if (objReceivingInstruction != null)
				{
					this.modelEdit.ReceiveId = objReceivingInstruction.RecevedID;
					this.modelEdit.DeliveryDate = Convert.ToDateTime(objReceivingInstruction.DeliveryDate);
					this.modelEdit.ReceiveDate = Convert.ToDateTime(objReceivingInstruction.ReceiveDate);
					this.modelEdit.ContainerNo = objReceivingInstruction.ContainerNo;
					this.modelEdit.TruckNo = objReceivingInstruction.TruckNumber;
					

					if (objReceivingInstruction.StartTime != null)
					{
						this.modelEdit.StartHours = Convert.ToString(objReceivingInstruction.StartTime.Value.Hours);
						this.modelEdit.StartMinute = Convert.ToString(objReceivingInstruction.StartTime.Value.Minutes);
						this.modelEdit.startHourAndMinute = Convert.ToString(objReceivingInstruction.StartTime.Value.Hours) + ":" + Convert.ToString(objReceivingInstruction.StartTime.Value.Minutes);
					}
					else
					{
						this.modelEdit.StartHours = "00";
						this.modelEdit.StartMinute = "00";
						this.modelEdit.startHourAndMinute = "00:00";
					}
					if (objReceivingInstruction.FinishedTime != null)
					{
						this.modelEdit.FinishHours = Convert.ToString(objReceivingInstruction.FinishedTime.Value.Hours);
						this.modelEdit.FinishMinute = Convert.ToString(objReceivingInstruction.FinishedTime.Value.Minutes);
						this.modelEdit.FinishHourAndMinute = Convert.ToString(objReceivingInstruction.FinishedTime.Value.Hours) + ":" + Convert.ToString(objReceivingInstruction.FinishedTime.Value.Minutes);
					}
					else
					{
						this.modelEdit.FinishHours = "00";
						this.modelEdit.FinishMinute = "00";
						this.modelEdit.FinishHourAndMinute = "00:00";
					}
				}
				ReceivingInstructionDetailService objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
				var result = objReceivingInstructionDetailService.GetReceivingInstructionDetailList2(Convert.ToInt32(this.modelEdit.ReceiveId));
                if (result != null)
				{
					//Get Time of start 
					HHTReceiveService objHHTReceiveService = null;
                    List<HHTReceive> objHHTResult = null;
					objHHTReceiveService = new HHTReceiveService(this.dbEntities);
					//objHHTResult = objHHTReceiveService.GetHHTReceiveListByItemCodeHeadNo(result[0].ItemCode, result[0].HeatNo);
					objHHTResult = objHHTReceiveService.GetHHTReceiveListByReceiveID(this.modelEdit.ReceiveId);
                    
                    if (objHHTResult != null && objHHTResult.Count > 0)
					{
						if (objHHTResult[0].CreateDate != null)
						{
							this.modelEdit.StartHours = Convert.ToString(objHHTResult[0].CreateDate.Value.Hour);
							this.modelEdit.StartMinute = Convert.ToString(objHHTResult[0].CreateDate.Value.Minute);
							this.modelEdit.startHourAndMinute = Convert.ToString(objHHTResult[0].CreateDate.Value.Hour) + ":" + Convert.ToString(objHHTResult[0].CreateDate.Value.Minute);
						}
						else
						{
							this.modelEdit.StartHours = "00";
							this.modelEdit.StartMinute = "00";
							this.modelEdit.startHourAndMinute = "00:00";
						}
					}
					else
					{
						this.modelEdit.StartHours = "00";
						this.modelEdit.StartMinute = "00";
						this.modelEdit.startHourAndMinute = "00:00";
					}

					if (result.Count > 0)
					{
						foreach (ReceivingInstructionDetail enDetail in result)
						{
							objHHTReceiveService = new HHTReceiveService(this.dbEntities);
							objHHTResult = objHHTReceiveService.GetHHTReceiveList(Convert.ToInt32(enDetail.RecevedID), enDetail.ItemCode, enDetail.HeatNo, Constants.HHTReceiveStatus.ReceivedTrans);
                            enDetail.LocationText = null;
							if (objHHTResult != null)
							{
								if (objHHTResult.Count > 0)
								{
									List<HHTReceive> objHHTReceiveDistinctList = (objHHTResult.GroupBy(hrc => new { hrc.LocationCode })
																		  .Select(m => new HHTReceive
																		  {
																			  LocationCode = m.Key.LocationCode
																		  })).ToList();

									if (objHHTReceiveDistinctList != null)
									{
										if (objHHTReceiveDistinctList.Count > 0)
										{
											foreach (HHTReceive en in objHHTReceiveDistinctList)
											{
												if (enDetail.LocationText == null)
												{
													enDetail.LocationText = GetDisplayLocation(en.LocationCode);
                                                }
												else
												{
													enDetail.LocationText = enDetail.LocationText + ",\n" + GetDisplayLocation(en.LocationCode);
												}

											}
										}
									}
								}
							}
						}
					}
                    ConfigureViewModel(this.modelEdit);
                    
                    this.modelEdit.ReceivingInstructionDetailList = result;
					//this.modelEdit.StatusList = GetSelectListItemListStatus();
					this.modelEdit.StatusList = GetReceiveDetailStatusList();
					this.modelEdit.Total = this.modelEdit.ReceivingInstructionDetailList.Count.ToString();
					
                    return View(this.modelEdit);
				}
				else
				{
					this.modelEdit.AlertsType = Constants.AlertsType.Danger;
					this.modelEdit.Message = Resources.Common_cshtml.NoDataFound;
					this.modelEdit.HasError = true;

					return View(this.modelEdit);
				}
			}
			catch (Exception ex)
			{
				
				this.modelEdit.AlertsType = Constants.AlertsType.Danger;
				this.modelEdit.Message = ex.Message;
				this.PrintError(ex.Message);

				return View(this.model);
			}
		}

		[HttpPost]
		public ActionResult PSC2111(string submitButton, ReceivePlanDetailScreen modelEdit, string pHeatNo)
		{
			HHTReceiveService objHHTReceiveService = null;
			ReceivingInstruction objReceivingInstruction = null;
			string userId = this.LoginUser.UserId;
			string message = string.Empty;
			Boolean result = false;
			ReceivingInstructionDetailService objReceivingInstructionDetailService = null;
			List<ReceivingInstructionDetail> objReceivingInstructionDetailList = null;

			try
			{
				// Initial View
				ViewBag.LoginUserName = this.LoginUser.UserId;
				ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
				this.model.AlertsType = Constants.AlertsType.None;
				this.model.Message = string.Empty;

				switch (submitButton)
				{
					case "Save":
						if (ModelState.IsValid)
						{
							if (modelEdit != null)
							{
								Boolean IsAllReceived = false;

								objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
								objReceivingInstructionDetailList = objReceivingInstructionDetailService.GetReceivingInstructionDetailList2(Convert.ToInt32(this.modelEdit.ReceiveId));

								if (objReceivingInstructionDetailList != null)
								{
									var e = from s in objReceivingInstructionDetailList
											where s.Status == (byte)Constants.ReceiveDetailStatus.Receive
											select s;

									if (objReceivingInstructionDetailList.Count == e.Count())
									{
										IsAllReceived = true;
									}
									else
									{
										IsAllReceived = false;
									}

									objReceivingInstruction = new ReceivingInstruction();
									objReceivingInstruction.RecevedID = this.modelEdit.ReceiveId;
									objReceivingInstruction.ReceiveDate = modelEdit.ReceiveDate;
									objReceivingInstruction.TruckNumber = modelEdit.TruckNo;
                                    objReceivingInstruction.Status = Convert.ToByte(Constants.ReceiveStatus.New); // Edit by Art 21-04-25 Version 1.0.0.23

                                    //foreach (ReceivingInstructionDetail en in modelEdit.ReceivingInstructionDetailList)
                                    //{
                                    //    ReceivingInstructionDetailService objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
                                    //    result = objReceivingInstructionDetailService.Update(en.RecevedID, en.ItemCode, en.HeatNo, this.LoginUser.UserId, en.Remark, modelEdit.TruckNo, en.Status);
                                    //}

                                    ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);
									result = objReceivePlanService.SaveData1(objReceivingInstruction, modelEdit.ReceivingInstructionDetailList, this.LoginUser.UpdateUserID);

								}
							}
							this.modelEdit.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
							message = result ? Resources.Common_cshtml.SaveSuccessMsg : Resources.Common_cshtml.SaveFailMsg;

                        }
						else
						{
							message = Resources.Common_cshtml.SaveFailMsg;
						}
						
                        break;
                    case "Approve":
						if (ModelState.IsValid)
						{
							if (modelEdit != null)
							{
								Boolean IsAllSubmit = false;

								objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
								objReceivingInstructionDetailList = objReceivingInstructionDetailService.GetReceivingInstructionDetailList2(Convert.ToInt32(this.modelEdit.ReceiveId));
								if (objReceivingInstructionDetailList != null)
								{
									var e = from s in objReceivingInstructionDetailList
											where s.Status == (byte)Constants.ReceiveDetailStatus.Receive
											select s;

									if (objReceivingInstructionDetailList.Count == e.Count())
									{
										IsAllSubmit = true;
									}
									else
									{
										IsAllSubmit = false;
									}

									if (IsAllSubmit)
									{
										objReceivingInstruction = new ReceivingInstruction();
										objReceivingInstruction.RecevedID = this.modelEdit.ReceiveId;
										objReceivingInstruction.ReceiveDate = modelEdit.ReceiveDate;
										objReceivingInstruction.TruckNumber = modelEdit.TruckNo;
										objReceivingInstruction.Status = (byte)Constants.ReceiveStatus.Approve;

										foreach (ReceivingInstructionDetail en in modelEdit.ReceivingInstructionDetailList)
										{
											objHHTReceiveService = new HHTReceiveService(this.dbEntities);
											en.Status = (byte)Constants.ReceiveDetailStatus.Approve;
											en.HHTReceiveList = objHHTReceiveService.GetHHTReceiveList(en.RecevedID, en.ItemCode, en.HeatNo, Constants.HHTReceiveStatus.ReceivedTrans);
										}

										string MonthlyClose = string.Empty;
										MonthlyCloseService objMonthlyCloseService = new MonthlyCloseService(this.dbEntities);
										MonthlyClose objMonthlyClose = objMonthlyCloseService.GetOpenMonthlyClose();
										if (objMonthlyClose != null)
										{
											DateTime dFMonth = new DateTime(Convert.ToInt32(objMonthlyClose.Year), objMonthlyClose.Monthly, 1);
											string strFMonth = dFMonth.ToString("yyyy-MM");

											MonthlyClose = strFMonth + "-01";
										}
										DateTime dateYearMonth = DateTime.ParseExact(MonthlyClose, "yyyy-MM-dd", null);

										//DateTime dateStock = DateTime.Now;
										DateTime dateStock = objReceivingInstruction.ReceiveDate == null ? DateTime.Now : Convert.ToDateTime(objReceivingInstruction.ReceiveDate);

										ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);
										result = objReceivePlanService.ApproveData(dateYearMonth, dateStock, objReceivingInstruction, modelEdit.ReceivingInstructionDetailList, this.LoginUser.UpdateUserID);
									}
									else
									{
										result = false;
									}
								}
							}
							message = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;
						}
						else
						{
							message = result ? Resources.Common_cshtml.ApproveSuccessMsg : Resources.Common_cshtml.ApproveFailMsg;
						}

						break;
					case "Back":
						//this.model.AlertsType = Constants.AlertsType.None;
						//this.model.Message = "";
						return View("PSC2110", this.model);

					default:
						result = false;
						break;
				}

				// Alert Message
				this.modelEdit.AlertsType = result ? Constants.AlertsType.Success : Constants.AlertsType.Danger;
				this.modelEdit.Message = message;

				return View("PSC2111", this.modelEdit);
			}
			catch (Exception ex)
			{
				this.model.AlertsType = Constants.AlertsType.Danger;
				this.model.Message = ex.Message;
				this.PrintError(ex.Message);
				//modelEdit.DropdownHtml = GetLocationList();
				return View("PSC2110", this.model);
			}
		}

		public ActionResult GetActualQty(string pReceiveId, string pItemCode, string pHeatNo)
		{
			string result = string.Empty;

			HHTReceiveService objHHTReceiveService = null;
			List<HHTReceive> objHHTResult = null;
			decimal decActualQTY = 0;
			ReceivingInstructionDetailService objReceivingInstructionDetailService = null;
			ReceivePlanService objReceivePlanService = null;
			ReceivingInstructionDetail objReceivingInstructionDetail = null;

			try
			{
				objHHTReceiveService = new HHTReceiveService(this.dbEntities);
				objHHTResult = objHHTReceiveService.GetHHTReceiveList(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo, Constants.HHTReceiveStatus.NewTrans);

				if (objHHTResult != null)
				{
					if (objHHTResult.Count > 0)
					{
						foreach (HHTReceive en in objHHTResult)
						{
							objHHTReceiveService = new HHTReceiveService(this.dbEntities);
							objHHTReceiveService.Update(en.Id, Constants.HHTReceiveStatus.ReceivedTrans, this.LoginUser.UserId);
							decActualQTY = decActualQTY + en.ActualQTY;
						}
						objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
						ReceivingInstructionDetail objDetail = objReceivingInstructionDetailService.GetReceivingInstructionDetailList(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo);
						if (objDetail != null)
						{
							if (objDetail.Status == (byte)Common.Constants.ReceiveDetailStatus.New)
							{
								objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
								objReceivingInstructionDetailService.UpdateActualQtyAndStatus(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo, decActualQTY, Constants.ReceiveDetailStatus.Receive, this.LoginUser.UserId);
							}
							else
							{
								objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
								objReceivingInstructionDetailService.UpdateActualQty(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo, decActualQTY, this.LoginUser.UserId);
							}
						}
					}
				}

				objReceivePlanService = new ReceivePlanService(this.dbEntities);
				objReceivingInstructionDetail = objReceivePlanService.GetReceive(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo);

				if (objReceivingInstructionDetail != null)
				{
					////result = String.Format("{0:0.00}", objReceivingInstructionDetail.ActualQty) + "*" + objReceivingInstructionDetail.Status;
					//result = String.Format("{0:0.00}", objReceivingInstructionDetail.ActualQty) + "*" + GetStatus(Convert.ToByte(objReceivingInstructionDetail.Status));

					objHHTReceiveService = new HHTReceiveService(this.dbEntities);
					objHHTResult = objHHTReceiveService.GetHHTReceiveList(Convert.ToInt32(pReceiveId), pItemCode, pHeatNo, Constants.HHTReceiveStatus.ReceivedTrans);
					objReceivingInstructionDetail.LocationText = null;
					if (objHHTResult != null)
					{
						if (objHHTResult.Count > 0)
						{
							List<HHTReceive> objHHTReceiveDistinctList = (objHHTResult.GroupBy(hrc => new { hrc.LocationCode })
																  .Select(m => new HHTReceive
																  {
																	  LocationCode = m.Key.LocationCode
																  })).ToList();

							if (objHHTReceiveDistinctList != null)
							{
								if (objHHTReceiveDistinctList.Count > 0)
								{
									foreach (HHTReceive en in objHHTReceiveDistinctList)
									{
										if (objReceivingInstructionDetail.LocationText == null)
										{
											objReceivingInstructionDetail.LocationText = GetDisplayLocation(en.LocationCode);
										}
										/*else if (objReceivingInstructionDetail.LocationChange == null)
										{
											objReceivingInstructionDetail.LocationChange = GetDisplayLocation(en.LocationCode);
										}*/
										else
										{
											objReceivingInstructionDetail.LocationText = objReceivingInstructionDetail.LocationText + ",\n" + GetDisplayLocation(en.LocationCode);
											//objReceivingInstructionDetail.LocationChange = objReceivingInstructionDetail.LocationChange + ",\n" + GetDisplayLocation(en.LocationCode);
										}
									}
								}
							}

						}
					}
				}

				//Get Time of start 
				HHTReceiveService objHHTReceiveServiceTime = null;
				List<HHTReceive> objHHTResultTime = null;
				objHHTReceiveServiceTime = new HHTReceiveService(this.dbEntities);
				objHHTResultTime = objHHTReceiveServiceTime.GetHHTReceiveListByItemCodeHeadNo(objReceivingInstructionDetail.ItemCode, objReceivingInstructionDetail.HeatNo);

				if (objHHTResultTime != null && objHHTResultTime.Count > 0)
				{
					if (objHHTResultTime[0].CreateDate != null)
					{
						objReceivingInstructionDetail.StartHour = Convert.ToString(objHHTResultTime[0].CreateDate.Value.Hour);
						objReceivingInstructionDetail.StartMinute = Convert.ToString(objHHTResultTime[0].CreateDate.Value.Minute);
						objReceivingInstructionDetail.StartHourAndMinute = Convert.ToString(objHHTResultTime[0].CreateDate.Value.Hour) + ":" + Convert.ToString(objHHTResultTime[0].CreateDate.Value.Minute);
					}
					else
					{
						objReceivingInstructionDetail.StartHour = "00";
						objReceivingInstructionDetail.StartMinute = "00";
						objReceivingInstructionDetail.StartHourAndMinute = "00:00";
					}
				}
				else
				{
					objReceivingInstructionDetail.StartHour = "00";
					objReceivingInstructionDetail.StartMinute = "00";
					objReceivingInstructionDetail.StartHourAndMinute = "00:00";
				}

				return Json(objReceivingInstructionDetail, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				//throw ex;
				Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
			}
		}

		private string GetStatus(byte pStatus)
		{
			string result = string.Empty;
			if (pStatus == (byte)Constants.ReceiveStatus.New)
			{
				result = Resources.Common_cshtml.New;
			}
			else if (pStatus == (byte)Constants.ReceiveStatus.Receive)
			{
				result = Resources.Common_cshtml.Receive;
			}
			else if (pStatus == (byte)Constants.ReceiveStatus.Approve)
			{
				result = Resources.Common_cshtml.Approve;
			}
			else
			{
				result = "";
			}

			return result;
		}
		/*private string GetDropDownLocation(string pLocation)
        {
            string result = string.Empty;
            try
            {
                ReceivingInstructionDetailService objSelectListItemService = new ReceivingInstructionDetailService(this.dbEntities);
                List<SelectListItem> locations = objSelectListItemService.GetLocationDropdown(pLocation);

                if (locations != null && locations.Count > 0)
                {
                    // สร้าง HTML สำหรับ dropdown list
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<select class='form-control edit edit-mode'>");
                    foreach (var location in locations)
                    {
                        sb.AppendFormat("<option value='{0}'>{1}</option>", location.Value, location.Text);
                    }
                    sb.Append("</select>");
                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                // ควรใช้การจัดการข้อผิดพลาดที่ดีกว่านี้ใน production code
                throw ex;
            }

            return result;
        }*/
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

		private void IntialPSC2110()
		{
			ViewBag.LoginUserName = this.LoginUser.UserId;
			ViewBag.Lang = Request.Cookies["PSCS_culture"].Value;
			InitializeActionName = "PSC2110";
			QueryStringList = new Dictionary<string, string>();
			this.model.FilterDeliveryDate = this.model.FilterDeliveryDate != null ? this.model.FilterDeliveryDate : null;
			this.model.FilterContainerNo = this.model.FilterContainerNo != string.Empty ? this.model.FilterContainerNo : "";
			this.model.FilterHeatNo = this.model.FilterHeatNo != string.Empty ? this.model.FilterHeatNo : "";
			this.model.FilterOD = this.model.FilterOD != string.Empty ? this.model.FilterOD : "";
			this.model.FilterWT = this.model.FilterWT != string.Empty ? this.model.FilterWT : "";
			this.model.FilterLength = this.model.FilterLength != string.Empty ? this.model.FilterLength : "";
			this.model.ReceivePlanList = Search(this.model);
			this.model.Total = this.model.ReceivePlanList.Count.ToString();
		}

		private List<ReceivePlan> Search(ReceivePlanScreen FilterModel)
		{
			List<ReceivePlan> result = null;

			try
			{
				result = GetReceiveList(FilterModel.FilterDeliveryDate,
										FilterModel.FilterContainerNo);

				if (FilterModel.FilterHeatNo != string.Empty || FilterModel.FilterOD != string.Empty || FilterModel.FilterWT != string.Empty || FilterModel.FilterLength != string.Empty)
				{
					List<ReceiveItemsPlan> objReceiveItemsPlanList = GetReceivePlanAndDetailList(FilterModel.FilterDeliveryDate,
																								 FilterModel.FilterContainerNo);

					if (FilterModel.FilterHeatNo != null && FilterModel.FilterHeatNo != string.Empty)
					{
						if (objReceiveItemsPlanList != null)
						{
							objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.HeatNo == FilterModel.FilterHeatNo).ToList();
						}
					}
					if (FilterModel.FilterOD != null && FilterModel.FilterOD != string.Empty)
					{
						if (objReceiveItemsPlanList != null)
						{
							objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.OD == Convert.ToDecimal(FilterModel.FilterOD)).ToList();
						}
					}
					if (FilterModel.FilterWT != null && FilterModel.FilterWT != string.Empty)
					{

						if (objReceiveItemsPlanList != null)
						{
							objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.WT == Convert.ToDecimal(FilterModel.FilterWT)).ToList();
						}
					}
					if (FilterModel.FilterLength != null && FilterModel.FilterLength != string.Empty)
					{
						if (objReceiveItemsPlanList != null)
						{
							objReceiveItemsPlanList = objReceiveItemsPlanList.Where(x => x.Length == Convert.ToDecimal(FilterModel.FilterLength)).ToList();
						}
					}

					List<ReceiveItemsPlan> objReceiveItemsPlanListDistinct = (objReceiveItemsPlanList.GroupBy(r => new { r.ReceiveId })
											.Select((m, index) => new ReceiveItemsPlan
											{
												ReceiveId = m.Key.ReceiveId,
											})).ToList();

					if (objReceiveItemsPlanListDistinct == null)
					{
						result = new List<ReceivePlan>();
					}
					else
					{
						List<ReceivePlan> objReceivePlanList = new List<ReceivePlan>();

						foreach (ReceiveItemsPlan en in objReceiveItemsPlanListDistinct)
						{
							foreach (ReceivePlan enReceivePlan in result)
							{
								if (en.ReceiveId == enReceivePlan.ReceiveId)
								{
									objReceivePlanList.Add(enReceivePlan);
								}
							}
						}

						result = objReceivePlanList;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return result;
		}

		// Filter ReceivePlan view
		private ActionResult Filter_OnClick(ReceivePlanScreen FilterModel)
		{
			try
			{
				this.model.ReceivePlanList = Search(FilterModel);

				if (FilterModel.FilterHeatNo != null && FilterModel.FilterHeatNo != string.Empty)
				{
					this.model.FilterHeatNo = FilterModel.FilterHeatNo;
				}
				if (FilterModel.FilterOD != null && FilterModel.FilterOD != string.Empty)
				{
					this.model.FilterOD = FilterModel.FilterOD;
				}
				if (FilterModel.FilterWT != null && FilterModel.FilterWT != string.Empty)
				{
					this.model.FilterWT = FilterModel.FilterWT;
				}
				if (FilterModel.FilterLength != null && FilterModel.FilterLength != string.Empty)
				{
					this.model.FilterLength = FilterModel.FilterLength;
				}

				this.model.FilterDeliveryDate = FilterModel.FilterDeliveryDate;
				this.model.FilterContainerNo = FilterModel.FilterContainerNo;
				this.model.FilterHeatNo = FilterModel.FilterHeatNo;
				this.model.FilterOD = FilterModel.FilterOD;
				this.model.FilterWT = FilterModel.FilterWT;
				this.model.FilterLength = FilterModel.FilterLength;
				this.model.Total = this.model.ReceivePlanList.Count.ToString();
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return View("PSC2110", this.model);
		}

		private List<ReceivePlan> GetReceiveList(DateTime? pDeliveryDate, string pContainerNo)
		{
			List<ReceivePlan> result = new List<ReceivePlan>();
			try
			{
				ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);
				HttpCookie langCookie = Request.Cookies["PSCS_culture"];
				string pLanguage = langCookie != null ? langCookie.Value : "En";

				result = objReceivePlanService.GetReceivePlanList(pDeliveryDate, pContainerNo, pLanguage);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return result;
		}

		private List<ReceiveItemsPlan> GetReceivePlanAndDetailList(DateTime? pDeliveryDate, string pContainerNo)
		{
			List<ReceiveItemsPlan> result = new List<ReceiveItemsPlan>();
			try
			{
				ReceivePlanService objReceivePlanService = new ReceivePlanService(this.dbEntities);
				HttpCookie langCookie = Request.Cookies["PSCS_culture"];
				string pLanguage = langCookie != null ? langCookie.Value : "En";

				result = objReceivePlanService.GetReceivePlanAndDetailList(pDeliveryDate, pContainerNo, pLanguage);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return result;
		}

		private List<SelectListItem> GetHours()
		{
			List<SelectListItem> result = new List<SelectListItem>();
			for (int i = 0; i < 24; i++)
			{
				result.Add(new SelectListItem { Text = i.ToString("00"), Value = i.ToString() });
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

		private List<SelectListItem> GetSelectListItemListStatus()
		{
			List<SelectListItem> result = new List<SelectListItem>();
			foreach (Constants.ReceiveStatus enStatus in (Constants.ReceiveStatus[])Enum.GetValues(typeof(Constants.ReceiveStatus)))
			{
				//result.Add(new SelectListItem { Text = enStatus.ToString(), Value = ((int)enStatus).ToString() });
				result.Add(new SelectListItem
				{
					Text = (enStatus.ToString().Equals(Common.Constants.ReceiveStatus.New.ToString()) ? Resources.Common_cshtml.New :
							//enStatus.ToString().Equals(Common.Constants.ReceiveStatus.Plan.ToString()) ? Resources.Common_cshtml.Plan :
							//enStatus.ToString().Equals(Common.Constants.ReceiveStatus.Receive.ToString()) ? Resources.Common_cshtml.Receive :
							//enStatus.ToString().Equals(Common.Constants.ReceiveStatus.Submit.ToString()) ? Resources.Common_cshtml.Submit :
							enStatus.ToString().Equals(Common.Constants.ReceiveStatus.Approve.ToString()) ? Resources.Common_cshtml.Approve : ""),
					//Text = (enStatus.ToString()),
					Value = ((Byte)enStatus).ToString()
				});
			}
			return result;
		}

		private List<SelectListItem> GetReceiveDetailStatusList()
		{
			List<SelectListItem> result = new List<SelectListItem>();
			foreach (Constants.ReceiveDetailForSupervisorStatus enStatus in (Constants.ReceiveDetailForSupervisorStatus[])Enum.GetValues(typeof(Constants.ReceiveDetailForSupervisorStatus)))
			{
				result.Add(new SelectListItem
				{
					Text = (enStatus.ToString().Equals(Common.Constants.ReceiveDetailStatus.New.ToString()) ? Resources.Common_cshtml.New :
							enStatus.ToString().Equals(Common.Constants.ReceiveDetailStatus.Receive.ToString()) ? Resources.Common_cshtml.Receive :
							enStatus.ToString().Equals(Common.Constants.ReceiveDetailStatus.Approve.ToString()) ? Resources.Common_cshtml.Approve : ""),
					Value = ((Byte)enStatus).ToString()
				});
			}
			return result;
		}

		private void ConfigureViewModel(ReceivePlanDetailScreen model)
		{
			IEnumerable<Product> products = Repository.FetchProducts();
			model.ProductList = new SelectList(products, "ID", "Name");
		}

		public ActionResult RenderBarcodeSmall(string containerNo)
		{
			if (containerNo != null)
			{
				Image img = null;
				using (var ms = new MemoryStream())
				{
					var writer = new ZXing.BarcodeWriter() { Format = BarcodeFormat.CODE_39 };
					writer.Options.Height = 80;
					writer.Options.Width = 100;
					writer.Options.PureBarcode = false;
					img = writer.Write(containerNo);
					//var cropped = Common.Common.CropWhiteSpace(img);
					img.Save(ms, ImageFormat.Bmp);

					return File(ms.ToArray(), "image/jpeg");
				}
			}
			else
			{
				return null;
			}

		}

		public ActionResult RenderBarcode(string containerNo)
		{
			if (containerNo != null)
			{
				Image img = null;
				using (var ms = new MemoryStream())
				{
					var writer = new ZXing.BarcodeWriter() { Format = BarcodeFormat.CODE_39 };
					writer.Options.Height = 80;
					writer.Options.Width = 715;
					writer.Options.PureBarcode = false;
					img = writer.Write(containerNo);
					//var cropped = Common.Common.CropWhiteSpace(img);
					img.Save(ms, ImageFormat.Bmp);

					return File(ms.ToArray(), "image/jpeg");
				}
			}
			else
			{
				return null;
			}
		}
        private List<SelectListItem> GetLocationList()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            ReceivingInstructionDetailService objLocationService = new ReceivingInstructionDetailService(this.dbEntities);
			
            foreach (Location objLocation in objLocationService.GetLocationDropdown())
            {
                result.Add(new SelectListItem
                {
                    Text = objLocation.YardName+"-"+objLocation.Name,
                    Value = objLocation.LocationID,
                    //Selected = objLocation.Contains(objLocation.LocationID.ToString())
                });
            }

            return result;
        }
        /*private List<SelectListItem> GetLocationList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            ReceivingInstructionDetailService objlocationService = new ReceivingInstructionDetailService(this.dbEntities);
            var objLocationlist = objlocationService.GetLocationDropdown();
            ReceivingInstructionDetailService objReceivingInstructionDetailService = new ReceivingInstructionDetailService(this.dbEntities);
            var objReceivingInstructionDetailList = objReceivingInstructionDetailService.GetReceivingInstructionDetailList3(Convert.ToInt32(this.modelEdit.ReceiveId));

            // Get all LocationChange values from objReceivingInstructionDetailList
            var locationChanges = objReceivingInstructionDetailList
                .Where(detail => detail.LocationChange != null)
                .Select(detail => detail.LocationChange)
                .ToList();

            // Add all LocationChange values to the dropdown list first
            foreach (var locationChange in locationChanges)
            {
                var location = objLocationlist.FirstOrDefault(l => l.LocationID == locationChange);
                if (location != null)
                {
                    result.Add(new SelectListItem
                    {
                        Text = $"{location.YardName}-{location.Name}",
                        Value = location.LocationID.ToString()
                    });

                    // Optionally, remove the LocationChange from the list to avoid duplication
                    objLocationlist.Remove(location);
                }
            }

            // Add the rest of the locations to the dropdown list
            foreach (Location objLocation in objLocationlist)
            {
                result.Add(new SelectListItem
                {
                    Text = $"{objLocation.YardName}-{objLocation.Name}",
                    Value = objLocation.LocationID.ToString(),
                });
            }

            return result;
        }*/
    }
}