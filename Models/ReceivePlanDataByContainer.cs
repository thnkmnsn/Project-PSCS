using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReceivePlanDataByContainer
    {
        public int RowNo { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string ContainerNo { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}