using System;
using PSCS.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class PrintScreen : BaseScreen
    {
        #region "View Model"

        public PrintScreen()
        {
            ReceiveItemsPlanList = new List<ReceiveItemsPlan>();
        }

        public List<ReceiveItemsPlan> ReceiveItemsPlanList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FilterDeliveryDate { get; set; }
        public string FilterContainerNo { get; set; }

        public string FilterHeatNo { get; set; }
        public string FilterOD { get; set; }
        public string FilterWT { get; set; }
        public string FilterLength { get; set; }

        #endregion
    }
}