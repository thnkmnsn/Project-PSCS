using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using PSCS.Resources;

namespace PSCS.ModelsScreen
{
    public class MonthlyCloseScreen : BaseScreen
    {
        public MonthlyCloseScreen()
        {
            this.MonthlyCloseList = new List<MonthlyClose>();
        }

        public List<MonthlyClose> MonthlyCloseList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterMonthlyDate { get; set; }

        [Display(Name = "Password", ResourceType = typeof(PSC3010_cshtml))]
        [Required(ErrorMessageResourceType = typeof(PSC3010_cshtml), ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string MonthlyClosePassword { get; set; }

        public Boolean IsMonthlyClose { get; set; }
        public Boolean IsRestore { get; set; }
    }
}