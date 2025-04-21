using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class PipeItem
    {
        public Nullable<int> RowNo { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string HeatNo { get; set; }
        public Nullable<decimal> OD { get; set; }
        public Nullable<decimal> WT { get; set; }
        public Nullable<decimal> LT { get; set; }
        public string Size { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public string Material { get; set; }
        public string MaterialName { get; set; }
        public string standard { get; set; }
        public string standardName { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public string Shape { get; set; }
        public string ShapeName { get; set; }
        public string Maker { get; set; }
        public string MakerName { get; set; }
        public string Label1 { get; set; }
        public string Attribute1 { get; set; }
        public string Label2 { get; set; }
        public string Attribute2 { get; set; }
        public string Label3 { get; set; }
        public string Attribute3 { get; set; }
        public string Label4 { get; set; }
        public string Attribute4 { get; set; }
        public string Label5 { get; set; }
        public string Attribute5 { get; set; }
        public string Label6 { get; set; }
        public string Attribute6 { get; set; }
        public string Label7 { get; set; }
        public string Attribute7 { get; set; }
        public string Label8 { get; set; }
        public string Attribute8 { get; set; }
        public string Label9 { get; set; }
        public string Attribute9 { get; set; }
        public string Label10 { get; set; }
        public string Attribute10 { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}