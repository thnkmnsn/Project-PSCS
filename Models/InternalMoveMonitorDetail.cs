using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class InternalMoveMonitorDetail
    {
        public int RowNo { get; set; }
        public int MoveId { get; set; }
        public int TranNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public string FromLocationName { get; set; }
        public Nullable<decimal> QTY { get; set; }
        public string DestinationName { get; set; }
        public string Remark { get; set; }
    }
}