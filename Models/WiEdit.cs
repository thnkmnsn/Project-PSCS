using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class WiEdit
    {
        public int RowNo { get; set; }
        public string JobNo { get; set; }
        public Nullable<System.DateTime> ReleaseDate { get; set; }
        public string MfgNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public decimal QTY { get; set; }
        public Nullable<decimal> ReleaseQTY { get; set; }
        //public Nullable<decimal> ReceiveQTY { get; set; }
        public Nullable<decimal> RequestQTY { get; set; }
        public Nullable<decimal> RemainQTY { get; set; }
        public Nullable<byte> Status { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public string Description { get; set; }
        public string Maker { get; set; }



    }
}