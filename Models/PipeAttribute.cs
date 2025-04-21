using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class PipeAttribute
    {
        public Nullable<int> RowNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string Label { get; set; }
        public string Attribute { get; set; }
    }
}