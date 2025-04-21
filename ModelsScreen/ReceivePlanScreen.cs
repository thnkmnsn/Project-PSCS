using System;
using PSCS.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class ReceivePlanScreen : BaseScreen
    {
        public ReceivePlanScreen()
        {
            //FilterDeliveryDate = DateTime.Now;
            ReceivePlanList = new List<ReceivePlan>();
        }

        public List<ReceivePlan> ReceivePlanList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterDeliveryDate { get; set; }
        public string FilterContainerNo { get; set; }
        public string FilterHeatNo { get; set; }
        public string FilterOD { get; set; }
        public string FilterWT { get; set; }
        public string FilterLength { get; set; }
    }

    public class ReceivePlanDetailScreen : BaseScreen
    {
        public ReceivePlanDetailScreen()
        {
            this.HoursList = new List<SelectListItem>();
            this.MinuteList = new List<SelectListItem>();
            this.ReceivingInstructionDetailList = new List<ReceivingInstructionDetail>();
            this.StatusList = new List<SelectListItem>();
            this.RemarkList = new List<SelectListItem>();
            DropdownHtml = new List<SelectListItem>();
        }
        public string SelectedLocation { get; set; }
        public IEnumerable<SelectListItem> RemarkList { get; set; }
        public List<SelectListItem> HoursList { get; set; }
        public List<SelectListItem> MinuteList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<ReceivingInstructionDetail> ReceivingInstructionDetailList { get; set; }
        public List<SelectListItem> DropdownHtml { get; set; }
        public decimal Id { get; set; }
        public int ReceiveId { get; set; }
        public string HeatNo {  get; set; }
        public int TranNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        public string ContainerNo { get; set; }
        public string TruckNo { get; set; }

        //[Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(PSC2111_cshtml), ErrorMessageResourceName = "TruckNoRequired")]
        //public bool EditTruckNoAllow { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(PSC2111_cshtml), ErrorMessageResourceName = "ReceiveDateRequired")]
        public bool EditReceiveDateAllow { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(PSC2111_cshtml), ErrorMessageResourceName = "RemarkRequired")]
        public bool EditRemarkAllow { get; set; }

        public string StartHours { get; set; }
        public string StartMinute { get; set; }
        public string startHourAndMinute { get; set; }
        public string FinishHours { get; set; }
        public string FinishMinute { get; set; }
        public string FinishHourAndMinute { get; set; }
        public string status { get; set; }
        public bool HasError { get; set; }

        public int SelectedProductId { get { return 3; } }
        public SelectList ProductList { get; set; }

        public Boolean IsYardSupervisorRole { get; set; }
        public Boolean IsControllerRole { get; set; }
    }

    public class Product
    {
        public int ID { set; get; }
        public string Name { set; get; }
    }

    public static class Repository
    {
        public static IEnumerable<Product> FetchProducts()
        {
            return new List<Product>()
            {
                new Product(){ ID = 1, Name = Resources.Common_cshtml.New },
                new Product(){ ID = 2, Name = Resources.Common_cshtml.Submit },
                new Product(){ ID = 3, Name = Resources.Common_cshtml.Approve }
            };
        }
    }

    public class ReceivePlanPatial
    {
        public int RowNo { get; set; }
        public int ReceiveId { get; set; }
        public int TranNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        public string ContainerNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public decimal Qty { get; set; }
        public string status { get; set; }
        public bool HasError { get; set; }
    }

    public class ReceivePlanPatialScreen : BaseScreen
    {
        public ReceivePlanPatialScreen()
        {
            ReceivePlanPatialList = new List<ReceivePlanPatial>();
        }

        public List<ReceivePlanPatial> ReceivePlanPatialList { get; set; }

        public int ReceiveId { get; set; }
        public int TranNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        public string ContainerNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public decimal Qty { get; set; }
        public string status { get; set; }
        public bool HasError { get; set; }
    }
}