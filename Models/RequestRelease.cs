using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class RequestRelease
    {
        public decimal RequestId { get; set; }
        public decimal ReleaseId { get; set; }
        public Nullable<byte> Status { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public string JobNo { get; set; }
        public string MfgNo { get; set; }
    }
}