using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class MoveLocation
    {
        public int RowNo { get; set; }
        public string RowNoDisplay { get; set; }
        public decimal MoveId { get; set; }
        public Nullable<decimal> HHTTransId { get; set; }
        public string HHTJobNo { get; set; }
        public Nullable<System.DateTime> MoveDate { get; set; }     
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string HeatNoDisplay { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public DateTime Time { get; set; }
        public string TimeDisplay { get; set; }
        public string Operation { get; set; }
        public string LocationCodeFrom { get; set; }
        public string LocationCodeFromName { get; set; }
        public string LocationCodeFromNameDisplay { get; set; }
        public decimal QTYFrom { get; set; }
        public Nullable<decimal> QTY { get; set; }
        public Nullable<decimal> HHTTransQTY { get; set; }
        public string LocationCodeTo { get; set; }
        public string LocationCodeToName { get; set; }
        public string LocationCodeToNameDisplay { get; set; }
        public Nullable<decimal> QTYDifferent { get; set; }
        public string ProductName { get; set; }
        public string MfgNo { get; set; }
        public string Operator { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }

        public int Is_Release { get; set; }
        public Common.Constants.MovementType Type { get; set; }
        public int IsSelected { get; set; }
        public string CellCss { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }

        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
    }
}