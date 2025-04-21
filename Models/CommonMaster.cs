using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class CommonMaster
    {
        public int RowNo { get; set; }
        public string ParentCode { get; set; }
        public string CommonCode { get; set; }
        public string ValueEn { get; set; }
        public string ValueTh { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
    }
}