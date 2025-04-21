using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSCS.ModelsScreen
{
    public class BaseScreen
    {
        #region "View Model"
        public Common.Constants.AlertsType AlertsType { get; set; }
        public string Message { get; set; }
        [DataType(DataType.Currency)]
        public string Total { get; set; }
        public string TotalWeigth { get; set; }
        #endregion
    }
}