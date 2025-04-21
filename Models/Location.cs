using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
	public class Location
	{
        public int RowNo { get; set; }
        public string Place { get; set; }
        public string PlaceName { get; set; }
        public string Yard { get; set; }
        public string YardName { get; set; } 
        public string LocationID { get; set; }
        public string Name { get; set; }
        public string LocationCode { get; set; }
        public int DisplayOrder{ get; set; }
        public byte Active { get; set; }
        public string ActiveName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Boolean IsLogin { get; set; }
        public string LoginErrorMessage { get; set; }
    }
}