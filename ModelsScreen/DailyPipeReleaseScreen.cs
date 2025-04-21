using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class DailyPipeReleaseScreen : BaseScreen
    {
        public DailyPipeReleaseScreen()
        {
            FilterDate = DateTime.Today;
            PipeYardList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            DailyPipeReleaseList = new List<DailyPipeRelease>();
        }

        public List<SelectListItem> PipeYardList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<DailyPipeRelease> DailyPipeReleaseList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterDate { get; set; }
        public string FilterPipeYard { get; set; }
        public string FilterStatus { get; set; }
    }
}