using PSCS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class UserScreen
    {
        #region "View Model"
        public Common.Constants.AlertsType AlertsType { get; set; }
        public string Message { get; set; }

        public UserScreen()
        {
            this.LanguageList = new List<SelectListItem>();
            this.RoleList = new List<SelectListItem>();
            this.ActiveList = new List<SelectListItem>();

            this.UserList = new List<User>();
        }

        public List<SelectListItem> LanguageList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
        public List<SelectListItem> ActiveList { get; set; }
        public List<User> UserList { get; set; }

        public string FilterUserID { get; set; }
        public string FilterUserName { get; set; }
        public string FilterLanguage { get; set; }
        public string FilterRole { get; set; }
        public string FilterActive { get; set; }
        public string Total { get; set; }
        #endregion
    }

    public class UserScreenEdit
    {
        public UserScreenEdit()
        {
            this.LanguageList = new List<SelectListItem>();
            this.RoleList = new List<SelectListItem>();
            this.ActiveList = new List<SelectListItem>();
        }

        public List<SelectListItem> LanguageList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
        public List<SelectListItem> ActiveList { get; set; }

        [Display(Name = "UserID", ResourceType = typeof(PSC8030_Edit_cshtml))]
        [Required(ErrorMessageResourceType = typeof(PSC8030_Edit_cshtml), ErrorMessageResourceName = "UserIdRequired")]
        public string EditUserID { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(PSC8030_Edit_cshtml))]
        [Required(ErrorMessageResourceType = typeof(PSC8030_Edit_cshtml), ErrorMessageResourceName = "UserNameRequired")]
        public string EditUserName { get; set; }

        [Display(Name = "Password", ResourceType = typeof(PSC8030_Edit_cshtml))]
        //[Required(ErrorMessageResourceType = typeof(PSC8030_Edit_cshtml), ErrorMessageResourceName = "PasswordRequired")]
        public string EditPassword { get; set; }

        [Display(Name = "Terms and Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(PSC8030_Edit_cshtml), ErrorMessageResourceName = "PasswordRequired")]
        public bool EditPasswordAllow { get; set; }

        public string EditLanguageSelected { get; set; }

        public Int32 EditRoleIdSelected { get; set; }

        public string EditActiveSelected { get; set; }

        public string EditMode { get; set; }
    }
}