using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class InboundTallySheetScreen : BaseScreen
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        public SelectList ContainerList { get; set; }
        public string ContainerNo { get; set; }

        public List<ReceivingInstruction> ReceivingInstructionList { get; set; }

        public InboundTallySheetScreen()
        {
            IEnumerable<SelectListItem> defaultList = new List<SelectListItem>().AsEnumerable();

            DeliveryDate = DateTime.Today;
            ContainerList = new SelectList(defaultList);
            ReceivingInstructionList = new List<ReceivingInstruction>();
        }

    }

    public class InboundTallySheetEdit : BaseScreen
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        public int RecevedID { get; set; }
        public string TruckNumber { get; set; }
        public string ContainerNo { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan FinishedTime { get; set; }

        public List<ReceivingInstructionDetail> ReceivingInstructionDetailList { get; set; }

        public InboundTallySheetEdit()
        {
            ReceivingInstructionDetailList = new List<ReceivingInstructionDetail>();
        }

    }
}