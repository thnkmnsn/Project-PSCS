using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class InternalMoveMonitorScreen : BaseScreen
    {
        #region "ViewModel"
        public InternalMoveMonitorScreen()
        {
            //this.MonthlyCloseList = new List<MonthlyClose>();
            this.YardList = new List<SelectListItem>();
            this.HoursList = new List<SelectListItem>();
            this.MinuteList = new List<SelectListItem>();
            this.DataList = new List<InternalMoveMonitor>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> HoursList { get; set; }
        public List<SelectListItem> MinuteList { get; set; }
        public List<InternalMoveMonitor> DataList { get; set; }


        [Display(Name = "Internalmove date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterInternalMoveDate { get; set; }

        public string FilterStartHours { get; set; }
        public string FilterStartMinute { get; set; }
        public string FilterFinishHours { get; set; }
        public string FilterFinishMinute { get; set; }

        [Display(Name = "Yard")]
        public string FilterYardID { get; set; }

        #endregion
    }

    public class InternalMoveMonitorScreenDetail : BaseScreen
    {
        #region "ViewModel"
        public InternalMoveMonitorScreenDetail()
        {
            this.YardList = new List<SelectListItem>();
            this.HoursList = new List<SelectListItem>();
            this.MinuteList = new List<SelectListItem>();
            this.DataList = new List<InternalMoveMonitorDetail>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> HoursList { get; set; }
        public List<SelectListItem> MinuteList { get; set; }
        public List<InternalMoveMonitorDetail> DataList { get; set; }

        public int MoveId { get; set; }

        [Display(Name = "Internalmove date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string DetailInternalMoveDate { get; set; }

        public string DetailStartHours { get; set; }
        public string DetailStartMinute { get; set; }
        public string DetailFinishHours { get; set; }
        public string DetailFinishMinute { get; set; }

        [Display(Name = "Yard")]
        public string DetailYardID { get; set; }

        public bool HasError { get; set; }

        #endregion
    }
}