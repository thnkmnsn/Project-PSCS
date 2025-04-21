using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class Shape
    {
        public string ShapeCode { get; set; }
        public string ShapeName { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}