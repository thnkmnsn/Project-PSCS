using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;

namespace PSCS.ModelsScreen
{
    public class ReleaseSelectLocationScreen : BaseScreen
    {
        public ReleaseSelectLocationScreen()
        {
            this.YardList = new List<SelectListItem>();
            ReleaseYardDetailListDisplay = new List<ReleaseYardDetail>();
            this.SelectStatus = new List<SelectListItem>();
            WIRequestList = new List<Request>();
        }
        public List<SelectListItem> YardList { get; set; }
        public List<ReleaseYardDetail> ReleaseYardDetailListDisplay { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FilterRequestDate { get; set; }
        public string FilterYardID { get; set; }
        public Boolean FilterQtyRemaining { get; set; }
        public string FilterJobs {  get; set; }
        public List<SelectListItem> SelectStatus { get; set; }
        public List<Request> WIRequestList { get; set; }
        public bool HasRequest { get; set; }
        public string SelectedRequestId { get; set; }
    }

    public class ReleaseSelectLocationPatialScreen : BaseScreen
    {
        public ReleaseSelectLocationPatialScreen()
        {
            this.DataList = new List<LocationPatial>();
        }

        public List<LocationPatial> DataList { get; set; }
    }

    public class LocationPatial
    {
        public int RowNo { get; set; }
        public string YardName { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string ReceiveDateText { get; set; }
    }
}