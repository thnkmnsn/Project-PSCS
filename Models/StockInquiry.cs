using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class StockInquiry
    {
        public int RowNo { get; set; }
        public string PipeYard { get; set; }
        public string Location { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string HeatNo { get; set; }
        public string ReceivedDate { get; set; }
        public string OD { get; set; }
        public string WT { get; set; }
        public string Length { get; set; }
        public string Grade { get; set; }
        public string Maker { get; set; }
        public int Qty { get; set; }
    }
}