using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class PipeItemScreen : BaseScreen
    {
        #region "View Model"

        public PipeItemScreen()
        {
            this.PipeList = new List<PipeItem>();
            this.DisplayPipeList = new List<PipeItem>();
            this.MaterialList = new List<SelectListItem>();
            this.StandardList = new List<SelectListItem>();
            this.GradeList = new List<SelectListItem>();
            this.ShapeList = new List<SelectListItem>();
            this.MakerList = new List<SelectListItem>();
            this.OrderByList = new List<SelectListItem>();
            this.SortByList = new List<SelectListItem>();
        }

        public List<PipeItem> PipeList { get; set; }
        public List<PipeItem> DisplayPipeList { get; set; }
        public List<SelectListItem> MaterialList { get; set; }
        public List<SelectListItem> StandardList { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public List<SelectListItem> ShapeList { get; set; }
        public List<SelectListItem> MakerList { get; set; }
        public List<SelectListItem> OrderByList { get; set; }
        public List<SelectListItem> SortByList { get; set; }

        public string FilterItemCode { get; set; }
        public string FilterDescription { get; set; }

        public string FilterHeatNo { get; set; }

        //public string FilterContainerNo { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public Nullable<DateTime> FilterReceiveDate { get; set; }

        //public Nullable<decimal> FilterQty { get; set; }

        public decimal? FilterOD { get; set; }

        public decimal? FilterWT { get; set; }  
        
        public decimal? FilterLT { get; set; }

        public decimal? FilterUnitWeight { get; set; }

        public string FilterMaterialName { get; set; }

        public string FilterStandardName { get; set; }

        public string FilterGradeName { get; set; }

        public string FilterShapeName { get; set; }

        public string FilterMakerName { get; set; }

        public string FilterOrderBy { get; set; }

        public string FilterSortBy { get; set; }

        //public Nullable<byte> FilterIsBunded { get; set; }

        public bool HasError { get; set; }
        #endregion
    }


    public class PipeItemDetail
    {
        #region "View Model"

        public PipeItemDetail()
        {
            this.PipeAttributeList = new List<PipeAttribute>();
        }
        //For Attribute
        public List<PipeAttribute> PipeAttributeList { get; set; }

        [Display(Name = "Item Code")]
        public string DetailItemCode { get; set; }

        [Display(Name = "Description")]
        public string DetailDescription { get; set; }

        [Display(Name = "Heat No")]
        public string DetailHeatNo { get; set; }

        [Display(Name = "O.D")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? DetailOD { get; set; }

        [Display(Name = "W.T")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? DetailWT { get; set; }

        [Display(Name = "LT")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? DetailLT { get; set; }

        [Display(Name = "Size")]
        public string DetailSize { get; set; }

        [Display(Name = "Unit Weight")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal? DetailUnitWeight { get; set; }

        [Display(Name = "Material")]
        public string DetailMaterial { get; set; }

        [Display(Name = "Material Name")]
        public string DetailMaterialName { get; set; }

        [Display(Name = "Standard")]
        public string DetailStandard { get; set; }

        [Display(Name = "Standard Name")]
        public string DetailStandardName { get; set; }

        [Display(Name = "Grade")]
        public string DetailGrade { get; set; }

        [Display(Name = "Grade Name")]
        public string DetailGradeName { get; set; }

        [Display(Name = "Shape")]
        public string DetailShape { get; set; }

        [Display(Name = "Shape Name")]
        public string DetailShapeName { get; set; }

        [Display(Name = "Maker")]
        public string DetailMaker { get; set; }

        [Display(Name = "Maker Name")]
        public string DetailMakerName { get; set; }

        public Nullable<int> PT370 { get; set; }
        public Nullable<int> PG370 { get; set; }
        public Nullable<int> EN_Spec { get; set; }
        public Nullable<int> Aramco { get; set; }
        public Nullable<int> Gerab_PO { get; set; }
        public Nullable<int> Singapore { get; set; }
        public Nullable<int> C21_SHL1 { get; set; }
        public Nullable<decimal> MN { get; set; }
        public Nullable<decimal> C { get; set; }
        #endregion
    }
}