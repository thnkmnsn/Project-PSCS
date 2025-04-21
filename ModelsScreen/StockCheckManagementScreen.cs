using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class StockCheckManagementScreen : BaseScreen
    {
        #region "ViewModel"
        [Display(Name = "Stock Check Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FilterStockCheckDate { get; set; }

        [Display(Name = "Yard")]
        public string FilterYard { get; set; }

        [Display(Name = "Status")]
        public string FilterStatus { get; set; }

        public Boolean hasMessage { get; set; }

        public List<SelectListItem> YardList { get; set; }
        public List<StockCheckManagement> DataList { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public StockCheckManagementScreen()
        {
            FilterStockCheckDate = DateTime.Today;
            YardList = new List<SelectListItem>();
            DataList = new List<StockCheckManagement>();
            StatusList = new List<SelectListItem>();
        }
        #endregion
    }

    public class StockCheckManagementScreenEdit : BaseScreen
    {
        public List<StockCheckManagementDetail> DataList { get; set; }
        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Display(Name = "Yard")]
        public string DetailYard { get; set; }
        [Display(Name = "Stock Check Date")]
        public string DetailStockCheckDate { get; set; }
        [Display(Name = "Status")]
        public string DetailStatus { get; set; }
        public string EditMode { get; set; }

        #region "ViewModel"
        public StockCheckManagementScreenEdit()
        {
            YardList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            DataList = new List<StockCheckManagementDetail>();
        }
        #endregion
    }
}