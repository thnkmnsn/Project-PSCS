using PSCS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class ChangePasswordScreen : BaseScreen
    {
        #region "View Model"
        public ChangePasswordScreen()
        {

        }
        [Display(Name = "CurrentPassword", ResourceType = typeof(PSC0011_cshtml))]
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(PSC0011_cshtml), ErrorMessageResourceName = "CurrentPasswordRequiredMsg")]
        public string CurrentPassword { get; set; }

        [Display(Name = "NewPassword", ResourceType = typeof(PSC0011_cshtml))]
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(PSC0011_cshtml), ErrorMessageResourceName = "NewPasswordRequiredMsg")]
        [RegularExpression(@"^[0-9A-Za-z!@#$%^&*()_+|~\-=\\`{}[\]:"";'<>?,.\/]+$", ErrorMessageResourceType = typeof(PSC0011_cshtml), ErrorMessageResourceName = "EnglishOnlyMsg")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(PSC0011_cshtml), ErrorMessageResourceName = "ComparenotMatchMsg")]
        [Required(ErrorMessageResourceType = typeof(PSC0011_cshtml), ErrorMessageResourceName = "ConfirmPasswordRequiredMsg")]
        [RegularExpression(@"^[0-9A-Za-z!@#$%^&*()_+|~\-=\\`{}[\]:"";'<>?,.\/]+$", ErrorMessageResourceType = typeof(PSC0011_cshtml), ErrorMessageResourceName = "EnglishOnlyMsg")]
        public string ConfirmPassword { get; set; }
        #endregion
    }
}