//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PSCS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PSC3300_T_SYTELINE_JOB_MASTER
    {
        public int TRANS_NO { get; set; }
        public string JOB_NO { get; set; }
        public string MFG_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string HEAT_NO { get; set; }
        public Nullable<decimal> QTY { get; set; }
        public string DESCRIPTION { get; set; }
        public string Product {  get; set; }
        public string GroupMFGno {  get; set; }
        public string MFGnum { get; set; }
        public string CREATE_USER_ID { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
    }
}
