using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class StockCheckMonitorDetail
    {
        public int RowNo { get; set; }
        public DateTime StockCheckDate { get; set; }
        public string Yard { get; set; }
        public string YardName { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public int Qty { get; set; }
        public int ActualQty { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }
}