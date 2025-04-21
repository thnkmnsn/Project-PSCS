using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class ReleaseComfirmScreen : BaseScreen
    {
        public ReleaseComfirmScreen()
        {
            WIRequestList = new List<Request>();
            ReleaseDetailList = new List<ReleaseDetail>();
        }
        
        public List<Request> WIRequestList { get; set; }
        public List<ReleaseDetail> ReleaseDetailList { get; set; }
    }
}