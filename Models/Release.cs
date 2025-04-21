using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class Release
    {
        public int RowNo { get; set; }
        public decimal ID { get; set; }
        public DateTime TRAN_DATE { get; set; }
        public decimal? MoveId { get; set; }
        public string JobNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string Barcode { get; set; }
        public string LocationCode { get; set; }
        public decimal QTY { get; set; }
        public string Product {  get; set; }
        public string MFGnum {  get; set; }
        public string Operator {  get; set; }
        public Nullable<System.DateTime> ReleaseDate { get; set; }
        public string MfgNo { get; set; }
        public Nullable<decimal> ReleaseQTY { get; set; }
        //public Nullable<decimal> ReceiveQTY { get; set; }
        public Nullable<decimal> RemainQTY { get; set; }
        public int Status { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public string Description { get; set; }
        public string Maker { get; set; }
        public string Maker_Name { get; set; }
        public decimal ReleaseID { get; set; }
        public Nullable<decimal> RequestQTY { get; set; }

        public Nullable<decimal> PresentQty { get; set; }



    }
}