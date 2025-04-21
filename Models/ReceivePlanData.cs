using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReceivePlanData
    {
        public int RowNo { get; set; }    
        public DateTime ReceivedDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string ContainerNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public decimal? OD { get; set; }
        public decimal? WT { get; set; }
        public decimal? Length { get; set; }
        public decimal? PlanQty { get; set; }
        public decimal? PlanBundles { get; set; }
        public decimal? ActualQty { get; set; }
        public decimal? ActualBundles { get; set; }
        public string YardName { get; set; }
        public string LocationName { get; set; }
        public string PONo { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
    }
}