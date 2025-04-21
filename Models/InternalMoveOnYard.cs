using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class InternalMoveOnYard
    {
        public int RowNo { get; set; }
        public int Key { get; set; }
        public Nullable<System.DateTime> InternalMovementDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string YardID { get; set; }
        public string YardName { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public string FromLocationName { get; set; }
        public Nullable<decimal> QTY { get; set; }
        public string DestinationName { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
    }
}
