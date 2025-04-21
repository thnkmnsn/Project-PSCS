using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class ReleaseScreen : BaseScreen
    {
        public ReleaseScreen()
        {
            WIRequestList = new List<Request>();
            WiEdit = new List<WiEdit>();
            WiSum = new List<WiSum>();
        }

        public List<Request> WIRequestList { get; set; }
        //public int SelectedRowNo { get; set; }
        public string SelectedRequestId { get; set; }
        public string SelectedJobNo { get; set; }
        public List<WiEdit> WiEdit { get; set; }
        public List<WiSum> WiSum { get; set; }
        public string WiBarcode { get; set; }
        public bool HasRequest { get; set; }
        //public List<Request> WIRequestDelete { get; set; }
    }
}