using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class ReceiveMonitorScreen : BaseScreen
    {
        #region "View Model"

        public ReceiveMonitorScreen()
        {
            this.DataList = new List<ReceiveMonitorScreen>();
            this.StatusList = new List<SelectListItem>();
        }

        public List<ReceiveMonitorScreen> DataList { get; set; }

        public List<SelectListItem> StatusList;

        [Display(Name = "Delivery date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterDeliveryDate { get; set; }

        [Display(Name = "Container No")]
        public string FilterContainerNo { get; set; }

        [Display(Name = "Status")]
        public string FilterStatus { get; set; }
        #endregion
    }
}