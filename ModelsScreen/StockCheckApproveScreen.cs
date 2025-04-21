using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.ModelsScreen
{
    public class StockCheckApproveScreen : BaseScreen
    {
        public List<PipeItem> PipeItemList { get; set; }

        public StockCheckApproveScreen()
        {
            this.PipeItemList = new List<PipeItem>();
        }
    }
}