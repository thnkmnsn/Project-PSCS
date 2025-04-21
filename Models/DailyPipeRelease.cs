using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class DailyPipeRelease
    {
        public int RowNo { get; set; }
        public int DailyId { get; set; }
        public string PipeYard { get; set; }
        public string PipeYardName { get; set; }
        public DateTime IssuedDate { get; set; }
        public string CardNo { get; set; }
        public string MFGNo { get; set; }
        public string HeatNo { get; set; }
        public string Size { get; set; }
        public string Grade { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusText { get; set; }
    }
}