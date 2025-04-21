using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReleaseLocationDetail
    {
        public int RowNo { get; set; }
        public decimal ReleaseId { get; set; }
        public string LocationCode { get; set; }

        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}