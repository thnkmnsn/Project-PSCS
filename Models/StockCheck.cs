using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSCS.Models
{
    public class StockCheck
    {
        public int RowNo { get; set; }
        public decimal StockCheckId { get; set; }
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}")]
        public DateTime StockCheckDate { get; set; }
        public string Yard { get; set; }
        public string YardName { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public decimal Qty { get; set; }
        public Nullable<decimal> ActualQty { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusText { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public bool IsEdit { get; set; }
    }
}