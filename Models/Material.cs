using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class Material
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}