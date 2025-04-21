using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReceivingInstructionDetail
    {
        public int RowNo { get; set; }
        public int RecevedID { get; set; }
        public int TranNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string Description { get; set; }

        public decimal Id { get; set; }
        public decimal OD { get; set; }
        public decimal WT { get; set; }
        public decimal LT { get; set; }

        public decimal Qty { get; set; }
        public Nullable<decimal> ActualQty { get; set; }
        public Nullable<decimal> TotalActualQty { get; set; }
        public Nullable<decimal> ActualHistory { get; set; }
        public Nullable<decimal> QtyChange { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationChange { get; set; }
        public Nullable<decimal> Bundles { get; set; }
        public string PoNo { get; set; }

        public int Status { get; set; }
        public string StatusText { get; set; }
        public string LocationText { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }

        public List<HHTReceive> HHTReceiveList { get; set; }

        public string StartHour { get; set; }
        public string StartMinute { get; set; }

        public string StartHourAndMinute { get; set; }
    }
}