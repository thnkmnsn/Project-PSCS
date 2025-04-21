using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class ReceivePlanManagementScreen : BaseScreen
    {
        public ReceivePlanManagementScreen()
        {
            StatusList = new List<SelectListItem>();
            ReceiveDataList = new List<ReceiveData>();
        }

        [Display(Name = "Delivery date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterDeliveryDate { get; set; }

        [Display(Name = "Container No.")]
        public string FilterContainerNo { get; set; }

        [Display(Name = "Status")]
        public string FilterStatus { get; set; }

        public List<SelectListItem> StatusList;

        // Table List
        public List<ReceiveData> ReceiveDataList { get; set; }
    }

    public class ReceivePlanManagementEdit : BaseScreen
    {
        public ReceivePlanManagementEdit()
        {
            //DeliveryDate = DateTime.Today;
            ReceivePlanDataList = new List<ReceivePlanData>();
        }

        public int ReceiveId { get; set; }

        [Display(Name = "Delivery date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Container No")]
        public string ContainerNo { get; set; }

        public string status { get; set; }

        // Table List
        public List<ReceivePlanData> ReceivePlanDataList { get; set; }

        public bool HasError { get; set; }
    }
}