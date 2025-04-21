using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;
using PSCS.ModelsScreen;

namespace PSCS.ModelsScreen
{
    public class StockTakingScreen : BaseScreen
    {

        public StockTakingScreen()
        {
            this.YardList = new List<SelectListItem>();
            this.LocationList = new List<SelectListItem>();
            this.StatusList = new List<SelectListItem>();
            this.DataList = new List<StockTakingScreen>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<StockTakingScreen> DataList { get; set; }

        public Boolean SaveDisable { get; set; }
        public Boolean SubmitDisable { get; set; }
        public Boolean AdjustDisable { get; set; }
        public Boolean ApproveDisable { get; set; }

        public string FilterYardID { get; set; }
        public string FilterLocationID { get; set; }
        public byte FilterStatus { get; set; }

        public int RowNo { get; set; }
        public string PipeYard { get; set; }
        public string Location { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Lenght { get; set; }
        public string Grade { get; set; }
        public string Maker { get; set; }
        public string Remark { get; set; }
        public decimal CurrentQty { get; set; }
        public decimal ActualQty { get; set; }
        public Nullable<decimal> unit_weight { get; set; }
        public string Status { get; set; }
        public bool Check { get; set; }
        public string UserId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StockTakingDate { get; set; }
        public string LocationID { get; set; }
    }
}