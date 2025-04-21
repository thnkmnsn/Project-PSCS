using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class Pipe
    {
        public int RowNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string ContainerNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }

        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public Nullable<decimal> Bundles { get; set; }
        public string Material { get; set; }
        public string MaterialName { get; set; }
        public string Standard { get; set; }
        public string StandardName { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public string Shape { get; set; }
        public string ShapeName { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> LT { get; set; }
        public string Maker { get; set; }
        public string MakerName { get; set; }
        public Nullable<byte> IsBunded { get; set; }
        public string IsBundedName { get; set; }
        public int DisplayOrder { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
    }
}