using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class HHTStockCheck
    {
        public decimal Id { get; set; }
        public Nullable<decimal> StockCheckID { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string Barcode { get; set; }
        public string LocationCode { get; set; }
        public decimal ActualQTY { get; set; }
        public int Status { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}