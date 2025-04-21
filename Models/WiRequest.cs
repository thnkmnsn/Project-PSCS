using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class WiRequest
    {
        public int RowNo { get; set; }
        public int ReqestId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> RequestDate { get; set; }
        public string JobNo { get; set; }
        public string MFGNo { get; set; }
        public string HeatNo { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Remain { get; set; }
        public string PipeReqNo { get; set; }
    }
}