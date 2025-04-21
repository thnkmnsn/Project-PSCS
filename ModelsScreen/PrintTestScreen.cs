using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSCS.ModelsScreen
{
    public class PrintTestScreen : BaseScreen
    {
        #region "View Model"

        public PrintTestScreen()
        {
            this.PrinterList = new List<SelectListItem>();
        }

        public List<SelectListItem> PrinterList { get; set; }

        public string FilterPrinter { get; set; }
        public string InputPrinter { get; set; }
        #endregion
    }
}