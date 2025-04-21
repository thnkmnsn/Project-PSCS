using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class PipeScreen : BaseScreen
    {
        #region "View Model"

        public PipeScreen()
        {
            this.IsBundedList = new List<SelectListItem>();
            this.PipeList = new List<Pipe>();
        }

        public List<SelectListItem> IsBundedList;

        public List<Pipe> PipeList { get; set; }

        public string FilterItemCode { get; set; }

        public string FilterHeatNo { get; set; }

        public string FilterContainerNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterReceiveDate { get; set; }

        public Nullable<decimal> FilterQty { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public Nullable<decimal> FilterUnitWeight { get; set; }

        public string FilterMaterialName { get; set; }

        public string FilterStandardName { get; set; }

        public string FilterGradeName { get; set; }

        public string FilterShapeName { get; set; }

        [RegularExpression(@"^\d+\.\d{0,1}$")]
        public Nullable<decimal> FilterOD { get; set; }

        [RegularExpression(@"^\d+\.\d{0,1}$")]
        public Nullable<decimal> FilterWT { get; set; }

        public Nullable<decimal> FilterLT { get; set; }

        public string FilterMakerName { get; set; }

        public Nullable<byte> FilterIsBunded { get; set; }

        public bool HasError { get; set; }
        #endregion
    }


    public class PipeDetail
    {
        #region "View Model"

        public PipeDetail()
        {
            this.IsBundedList = new List<SelectListItem>();
        }

        public List<SelectListItem> IsBundedList;

        public int RowNo { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is required")]
        public string InputItemCode { get; set; }

        [Display(Name = "Heat No")]
        [Required(ErrorMessage = "Heat No is required")]
        public string InputHeatNo { get; set; }

        [Display(Name = "Container No")]
        public string InputContainerNo { get; set; }

        [Display(Name = "Receive Date")]
        [Required(ErrorMessage = "Item Code is required")]
        public DateTime ReceiveDate { get; set; }

        public string InputReceiveDate { get; set; }

        [Display(Name = "QTY")]
        public string InputQTY { get; set; }

        [Display(Name = "Unit Weight")]
        //[RegularExpression(@"^\d+\.\d{0,3}$", ErrorMessage = "This number is maximum 3 digit")]
        public string InputUnitWeight { get; set; }

        [Display(Name = "Bundles")]
        public string InputBundles { get; set; }

        [Display(Name = "Material")]
        public string InputMaterial { get; set; }

        [Display(Name = "Material Name")]
        public string InputMaterialName { get; set; }

        [Display(Name = "Standard")]
        public string InputStandard { get; set; }

        [Display(Name = "Standard Name")]
        public string InputStandardName { get; set; }

        [Display(Name = "Grade")]
        public string InputGrade { get; set; }

        [Display(Name = "Grade Name")]
        public string InputGradeName { get; set; }

        [Display(Name = "Shape")]
        public string InputShape { get; set; }

        [Display(Name = "Shape Name")]
        public string InputShapeName { get; set; }

        [Display(Name = "OD")]
        //[RegularExpression(@"^\d+\.\d{0,1}$", ErrorMessage = "This number is maximum 1 digit")]
        public string InputOD { get; set; }

        [Display(Name = "WT")]
        //[RegularExpression(@"^\d+\.\d{0,1}$", ErrorMessage = "This number is maximum 1 digit")]
        public string InputWT { get; set; }

        [Display(Name = "LT")]
        public string InputLT { get; set; }

        [Display(Name = "Maker")]
        public string InputMaker { get; set; }

        [Display(Name = "Maker Name")]
        public string InputMakerName { get; set; }

        [Display(Name = "Is Bundeled")]
        public Nullable<byte> InputIsBundled { get; set; }

        [Display(Name = "Is Bundeled Name")]
        public string InputBundedName { get; set; }

        [Display(Name = "Display Order")]
        // not null
        public int InputDisplayOrder { get; set; }

        //public Nullable<System.DateTime> CREATE_DATE { get; set; }

        //public string CREATE_USER_ID { get; set; }

        //public Nullable<DateTime> UPDATE_DATE { get; set; }

        //public string UPDATE_USER_ID { get; set; }
        #endregion
    }
}