using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class InternalMoveOnYardScreen
    {
        #region "ViewModel"
        public Common.Constants.AlertsType AlertsType { get; set; }
        public string Message { get; set; }

        public InternalMoveOnYardScreen()
        {
            this.YardList = new List<SelectListItem>();
            this.DataList = new List<InternalMoveOnYard>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<InternalMoveOnYard> DataList { get; set; }

        public string FilterInternalMoveDate { get; set; }
        public string FilterYardID { get; set; }

        #endregion
    }

    public class InternalMoveOnYardScreenEdit
    {
        #region "ViewModel"
        public Common.Constants.AlertsType AlertsType { get; set; }
        public string Message { get; set; }

        public InternalMoveOnYardScreenEdit()
        {
            this.YardList = new List<SelectListItem>();
            this.HoursList = new List<SelectListItem>();
            this.MinuteList = new List<SelectListItem>();
            this.DataList = new List<InternalMoveOnYard>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> HoursList { get; set; }
        public List<SelectListItem> MinuteList { get; set; }
        public List<InternalMoveOnYard> DataList { get; set; }

        public string DetailInternalMoveDate { get; set; }
        public string DetailYardID { get; set; }
        public string DetailStartHours { get; set; }
        public string DetailStartMinute { get; set; }
        public string DetailFinishHours { get; set; }
        public string DetailFinishMinute { get; set; }

        #endregion
    }

}