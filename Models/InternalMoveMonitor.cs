using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class InternalMoveMonitor
    {
        public int RowNo { get; set; }
        public int MoveId { get; set; }
        public Nullable<DateTime> InternalMovementDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string YardID { get; set; }
        public string YardName { get; set; }
    }
}