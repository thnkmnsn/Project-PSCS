using PSCS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class LocationScreen : BaseScreen
    {
        #region "ViewModel"

        public LocationScreen()
        {
            this.PlaceList = new List<SelectListItem>();
            this.YardList = new List<SelectListItem>();
            this.ActiveList = new List<SelectListItem>();
            this.LocationList = new List<Location>();
        }

        public List<Location> LocationList { get; set; }
        public List<SelectListItem> PlaceList { get; set; }
        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> ActiveList { get; set; }

        public string FilterPlace { get; set; }
        public string FilterYard { get; set; }
        public string FilterLocationID { get; set; }
        public string FilterName { get; set; }
        public string FilterActive { get; set; }
        public int DisplayOrder { get; set; }
        #endregion
    }


    public class LocationEditScreen
    {
        public LocationEditScreen()
        {
            this.PlaceList = new List<SelectListItem>();
            this.YardList = new List<SelectListItem>();
            this.ActiveList = new List<SelectListItem>();
            this.LocationList = new List<Location>();
        }

        public List<Location> LocationList { get; set; }
        public List<SelectListItem> PlaceList { get; set; }
        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> ActiveList { get; set; }

        public string InputPlace { get; set; }
        public string InputYard { get; set; }
        [Required(ErrorMessageResourceType = typeof(PSC8020_Edit_cshtml), ErrorMessageResourceName = "LocationRequired")]
        public string InputLocation { get; set; }
        [Required(ErrorMessageResourceType = typeof(PSC8020_Edit_cshtml), ErrorMessageResourceName = "NameRequired")]
        public string InputName { get; set; }
        public int DisplayOrder { get; set; }
        public string InputActive { get; set; }
        public string SubmitMode { get; set; }
        public bool HasError { get; set; }
    }
}