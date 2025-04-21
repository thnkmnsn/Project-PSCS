using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class ReleaseYardDetail
    {
        public int RowNo { get; set; }
        public decimal HHTReleaseId { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public decimal ReleaseId { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string RequestNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime RequestDate { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public Nullable<decimal> RequestQTY { get; set; }
        public Nullable<decimal> ActualQTY { get; set; }
        public Nullable<decimal> RemainQTY { get; set; }
        public Nullable<byte> Status { get; set; }
        public string LocationCodeList { get; set; }
        public Nullable<decimal> ChangeReleaseQty { get; set; }
        public string Status_Name { get; set; }
        public string Yard1Remark { get; set; }
        public string Yard2Remark { get; set; }
        public string CuttingRemark { get; set; }
        public string Description { get; set; }
        public string Maker { get; set; }
        public string Maker_Name { get; set; }
        public string Grade { get; set; }
        public string Grade_Name { get; set; }
        public string YardID { get; set; }
        public string YardName { get; set; }
        public string ProductName { get; set; }
        public string JobNo { get; set; }
        public string MfgNo { get; set; }

        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

    }
}