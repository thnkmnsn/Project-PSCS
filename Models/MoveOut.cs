using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class MoveOut
    {
        public decimal ID { get; set; }
        public DateTime TRAN_DATE { get; set; }
        public decimal? MOVE_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string HEAT_NO { get; set; }
        public string BARCODE { get; set; }
        public string LOCATION_CODE { get; set; }
        public Nullable<decimal> ACTUAL_QTY { get; set; }
        public int STATUS { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }

    }
}