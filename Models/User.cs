using PSCS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.Models
{
    public class User
    {
        [Display(Name = "UserId", ResourceType = typeof(PSC0010_cshtml))]
        [Required(ErrorMessageResourceType = typeof(PSC0010_cshtml), ErrorMessageResourceName = "UserIdRequired")]
        public string UserId { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Password", ResourceType = typeof(PSC0010_cshtml))]
        [Required(ErrorMessageResourceType = typeof(PSC0010_cshtml), ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Language { get; set; }
        public string LanguageName { get; set; }
        public Int32 RoleId { get; set; }
        public string RoleName { get; set; }
        public byte Active { get; set; }
        public string ActiveName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Boolean IsLogin { get; set; }
        public string LoginErrorMessage { get; set; }

        #region "View Model"
        public User()
        {
            this.UserList = new List<User>();
        }

        public List<User> UserList { get; set; }

        public string ErrorMessage { get; set; }

        public int RowNo { get; set; }
        #endregion
    }
}