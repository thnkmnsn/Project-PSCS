using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class MoveLocationScreen : BaseScreen
    {
        public MoveLocationScreen()
        {
            FilterInternalMoveDate = DateTime.Today;
            this.YardList = new List<SelectListItem>();
            this.HoursList = new List<SelectListItem>();
            this.MinuteList = new List<SelectListItem>();
            this.MoveLocationList = new List<MoveLocation>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> HoursList { get; set; }
        public List<SelectListItem> MinuteList { get; set; }
        public List<MoveLocation> MoveLocationList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FilterInternalMoveDate { get; set; }

        public string FilterYardID { get; set; }
        //public string Total { get; set; }

        public Boolean IsYardSupervisorRole { get; set; }
        public Boolean IsControllerRole { get; set; }

    }

    public class MoveLocationScreenEdit
    {
        #region "ViewModel"
        public Common.Constants.AlertsType AlertsType { get; set; }
        public string Message { get; set; }

        public MoveLocationScreenEdit()
        {
            this.YardList = new List<SelectListItem>();
            this.HoursList = new List<SelectListItem>();
            this.MinuteList = new List<SelectListItem>();
            this.MoveLocationList = new List<MoveLocation>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> HoursList { get; set; }
        public List<SelectListItem> MinuteList { get; set; }
        public List<MoveLocation> MoveLocationList { get; set; }

        public string DetailInternalMoveDate { get; set; }
        public string DetailYardID { get; set; }
        public string DetailStartHours { get; set; }
        public string DetailStartMinute { get; set; }
        public string DetailFinishHours { get; set; }
        public string DetailFinishMinute { get; set; }
        public bool HasError { get; set; }

        #endregion
    }

}