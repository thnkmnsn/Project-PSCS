using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSCS.Models
{
    public class CascadingModel
    {
        public CascadingModel()
        {
            this.Countries = new List<SelectListItem>();
            this.States = new List<SelectListItem>();
            this.Data = new List<StockList>();
        }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> States { get; set; }
        public List<StockList> Data { get; set; }

        public int CountryId { get; set; }
        public int StateId { get; set; }
        //public int Data { get; set; }
    }
}