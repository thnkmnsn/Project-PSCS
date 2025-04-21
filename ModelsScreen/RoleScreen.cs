using PSCS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class RoleScreen
    {
        #region "View Model"
        public Common.Constants.AlertsType AlertsType { get; set; }
        public string Message { get; set; }

        public RoleScreen()
        {
            this.RoleList = new List<Role>();
        }

        public List<Role> RoleList { get; set; }

        public string FilterRoleID { get; set; }
        public string FilterNameTh { get; set; }
        public string FilterNameEn { get; set; }
        public string Total { get; set; }
        #endregion
    }

    public class RoleEdit
    {
        #region "View Model"
        public RoleEdit()
        {
            this.RoleList = new List<Role>();
        }

        public List<Role> RoleList { get; set; }

        [Display(Name = "Role ID")]
        public string InputRoleID { get; set; }

        [Display(Name = "Name TH")]
        [Required(ErrorMessageResourceType = typeof(PSC8040_Edit_cshtml), ErrorMessageResourceName = "NameTHRequired")]
        [RegularExpression(@"^[0-9ก-๙,\s]+$", ErrorMessageResourceType = typeof(PSC8040_Edit_cshtml), ErrorMessageResourceName = "NameTHOnly")]
        public string InputNameTh { get; set; }

        [Display(Name = "Name EN")]
        [Required(ErrorMessageResourceType = typeof(PSC8040_Edit_cshtml), ErrorMessageResourceName = "NameENRequired")]
        [RegularExpression(@"^[0-9A-Za-z,\s]+$", ErrorMessageResourceType = typeof(PSC8040_Edit_cshtml), ErrorMessageResourceName = "NameENOnly")]
        public string InputNameEn { get; set; }

        public string SubmitMode { get; set; }
        #endregion
    }
}