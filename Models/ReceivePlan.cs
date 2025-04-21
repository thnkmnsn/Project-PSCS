using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReceivePlan
    {
        public int RowNo { get; set; }
        public int ReceiveId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DeliveryDate { get; set; }
        public string ContainerNo { get; set; }
        public string TruckNo { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> FinishTime { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusText { get; set; }
    }
}