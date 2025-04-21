using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReceiveItemsPlan
    {
        public int RowNo { get; set; }
        public int ReceiveId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DeliveryDate { get; set; }
        public string ContainerNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<int> Status { get; set; }
    }
}