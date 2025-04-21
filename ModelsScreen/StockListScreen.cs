using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class StockListScreen : BaseScreen
    {
        public StockListScreen()
        {
            //FilterYearMonth = DateTime.Today;
            this.YardList = new List<SelectListItem>();
            this.LocationList = new List<SelectListItem>();
            this.GradeList = new List<SelectListItem>();
            this.MakerList = new List<SelectListItem>();
            this.StandardList = new List<SelectListItem>();
            this.OrderByList = new List<SelectListItem>();
            this.SortByList = new List<SelectListItem>();
            this.DataList = new List<StockList>();
            this.DisplayDataList = new List<StockList>();
        }

        public List<SelectListItem> YardList { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public List<SelectListItem> MakerList { get; set; }
        public List<SelectListItem> StandardList { get; set; }
        public List<SelectListItem> OrderByList { get; set; }
        public List<SelectListItem> SortByList { get; set; }
        public List<StockList> DataList { get; set; }
        public List<StockList> DisplayDataList { get; set; }
        public List<SelectListItem> GerabPOList { get; set; }
        public List<SelectListItem> SingaporeList { get; set; }
        public List<SelectListItem> C21SHL1List { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public System.DateTime FilterYearMonth { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterReceiveDate { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public System.DateTime FilterReceiveDateTo { get; set; }
        public string FilterItemCode { get; set; }
        public string FilterYardID { get; set; }
        public string FilterLocationID { get; set; }
        public string FilterDescription { get; set; }
        public string FilterHeatNo { get; set; }
        public string FilterOD { get; set; }
        public string FilterWT { get; set; }
        public string FilterLength { get; set; }
        public string FilterGrade { get; set; }
        public string FilterMaker { get; set; }
        public string FilterStandardName { get; set; }
        public string FilterOrderBy { get; set; }
        public string FilterSortBy { get; set; }
        public Boolean FilterIsShowZero { get; set; }

        public string FilterGerabPO { get; set; }
        public string FilterSingapore { get; set; }
        public string FilterC21SHL1 { get; set; }
    }

    public class StockListPatialScreen
    {
        public StockListPatialScreen()
        {
            this.DataList = new List<StockListPatial>();
        }

        public List<StockListPatial> DataList { get; set; }
    }

    public class StockListPatial
    {
        public int RowNo { get; set; }
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> Qty { get; set; }
    }
}
