using PSCS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class CommonMasterScreen : BaseScreen
    {
        #region "View Model"

        public CommonMasterScreen()
        {
            this.CommonMasterList = new List<CommonMaster>();
        }

        public List<CommonMaster> CommonMasterList { get; set; }
        #endregion
    }

    public class CommonMasterScreenEdit
    {
        #region "View Model"

        [Display(Name = "ParentCode")]
        [Required(ErrorMessageResourceType = typeof(PSC8060_Edit_cshtml), ErrorMessageResourceName = "ParentCodeRequired")]
        public string EditParentCode { get; set; }

        [Display(Name = "CommonCode")]
        [Required(ErrorMessageResourceType = typeof(PSC8060_Edit_cshtml), ErrorMessageResourceName = "CommonCodeRequired")]
        public string EditCommonCode { get; set; }

        [Display(Name = "ValueEN")]
        [Required(ErrorMessageResourceType = typeof(PSC8060_Edit_cshtml), ErrorMessageResourceName = "ValueEnRequired")]
        [RegularExpression(@"^[0-9A-Za-z,\s]+$", ErrorMessageResourceType = typeof(PSC8060_Edit_cshtml), ErrorMessageResourceName = "ValueEnOnly")]
        public string EditValueEN { get; set; }

        [Display(Name = "ValueTH")]
        [Required(ErrorMessageResourceType = typeof(PSC8060_Edit_cshtml), ErrorMessageResourceName = "ValueThaiRequired")]
        [RegularExpression(@"^[0-9ก-๙,\s]+$", ErrorMessageResourceType = typeof(PSC8060_Edit_cshtml), ErrorMessageResourceName = "ValueThOnly")]
        public string EditValueTH { get; set; }

        public string EditMode { get; set; }
        #endregion
    }
}