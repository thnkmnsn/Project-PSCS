using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace PSCS.Models
{
    public class ReceivingInstruction
    {
        public int RowNo { get; set; }
        public int RecevedID { get; set; }
        public string ContainerNo { get; set; }
        public string TruckNumber { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public Nullable<System.TimeSpan> StartTime { get; set; }
        public Nullable<System.TimeSpan> FinishedTime { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public string HeatNo { get; set; }
    }
}