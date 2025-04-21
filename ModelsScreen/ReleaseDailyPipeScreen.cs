using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;

namespace PSCS.ModelsScreen
{
    public class ReleaseDailyPipeScreen : BaseScreen
    {
        public ReleaseDailyPipeScreen()
        {
            //FilterReleaseDate = DateTime.Today;
            this.YardList = new List<SelectListItem>();
            ReleaseYardDetailList = new List<ReleaseYardDetail>();
            ReleaseYardDetailListDisplay = new List<ReleaseYardDetail>();
        }
        public List<SelectListItem> YardList { get; set; }
        public List<ReleaseYardDetail> ReleaseYardDetailList { get; set; }
        public List<ReleaseYardDetail> ReleaseYardDetailListDisplay { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FilterRequestDate { get; set; }
        public string FilterYardID { get; set; }
    }
}