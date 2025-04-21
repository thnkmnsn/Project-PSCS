using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class StockCheckManagement
    {
        public int RowNo { get; set; }
        public DateTime StockCheckDate { get; set; }
        public string Yard { get; set; }
        public string YardName { get; set; }
        public string Status { get; set; }
    }
}