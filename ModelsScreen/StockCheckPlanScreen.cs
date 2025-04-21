using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class StockCheckPlanScreen : BaseScreen
    {
        public StockCheckPlanScreen()
        {
            PipeYardList = new List<SelectListItem>();
            YardList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            LocationList = new List<SelectListItem>();

            ItemList = new List<StockList>();

            PrinterList = new List<SelectListItem>();
            FilterStockCheckList = new List<StockCheck>();
            StockCheckList = new List<StockCheck>();
            StockVisorList = new List<StockCheck>();
            StockAdjustList = new List<StockCheck>();
        }

        public List<SelectListItem> PipeYardList { get; set; }
        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> LocationList { get; set; }

        public List<StockList> ItemList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterStockDate { get; set; }
        public string FilterPipeYard { get; set; }
        public string FilterStatus { get; set; }
        public string FilterPrinter { get; set; }
        public string DeletedSelected { get; set; }

        public string FilterYard { get; set; }
        public string FilterLocation { get; set; }
        public string FilterHeatNo { get; set; }
        public string FilterDescription { get; set; }
        public bool HasAddSelected { get; set; }

        public List<SelectListItem> VisorStatusList { get; set; }
        public List<SelectListItem> PrinterList { get; set; }

        public List<StockCheck> FilterStockCheckList { get; set; }
        public List<StockCheck> StockCheckList { get; set; }
        public List<StockCheck> StockVisorList { get; set; }
        public List<StockCheck> StockAdjustList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public string itemcount { get; set; }
        public string isDisable { get; set; }
    }
}