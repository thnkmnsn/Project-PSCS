using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSCS.ModelsScreen
{
    public class SyncReceiveDataScreen : BaseScreen
    {
        #region "View Model"

        public SyncReceiveDataScreen()
        {
            this.DataList = new List<SyncReceiveDataScreen>();
        }

        public List<SyncReceiveDataScreen> DataList { get; set; }

        //public int RowNo { get; set; }
        public int TransactionNo { get; set; }
        public string ItemCode { get; set; }
        public string HeatNo { get; set; }
        public string ContainerNo { get; set; }
        public Nullable<DateTime> DeliveryDate { get; set; }
        public Nullable<DateTime> ReceivedDate { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Bundles { get; set; }
        public string PONo { get; set; }
        public string Remark { get; set; }
        #endregion
    }
}