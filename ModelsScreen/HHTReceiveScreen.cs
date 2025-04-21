using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.ModelsScreen
{
    public class HHTReceiveScreen : BaseScreen
    {
        public string ContainerNo { get; set; }
        public string PiprBarcode { get; set; }
        public string QTY { get; set; }
        public string LocationBarcode { get; set; }
    }
}