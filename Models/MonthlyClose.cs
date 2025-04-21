using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
	public class MonthlyClose
	{
        public int RowNo { get; set; }
        public decimal Year { get; set; }
        public byte Monthly { get; set; }
        public System.DateTime ControlDate { get; set; }
        public byte Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
    }
}