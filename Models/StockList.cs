using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class StockList
    {
        public int RowNo { get; set; }
        public DateTime YearMonth { get; set; }
        public string YardID { get; set; }
        public string YardName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string HeatNo { get; set; }
        public string Description { get; set; }
        public string ReceiveDateText { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public string Grade { get; set; }
        public string Maker { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public Nullable<int> Gerab_PO { get; set; }
        public Nullable<int> Singapore { get; set; }
        public Nullable<int> C21_SHL1 { get; set; }
        public Nullable<decimal> MN { get; set; }
        public Nullable<decimal> C { get; set; }
        public Nullable<decimal> MNDivC { get; set; }
    }
}