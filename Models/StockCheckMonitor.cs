using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class StockCheckMonitor
    {
        public int RowNo { get; set; }
        public DateTime StockCheckDate { get; set; }
        public string Yard { get; set; }
        public string YardName { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string Status { get; set; }
    }
}