using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSCS.Models
{
    public class LandedGoodsImport
    {
        public LandedGoodsImport()
        {
            this.DataList = new List<LandedGoodsImport>();
        }

        public List<LandedGoodsImport> DataList { get; set; }

        public int RowNo { get; set; }
        public string ReceivedDate { get; set; }
        public string PONumber { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string ContainerNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> Length { get; set; }
        public string Grade { get; set; }
        public string Maker { get; set; }
        public Nullable<decimal> ReceivedQty { get; set; }
    }
}