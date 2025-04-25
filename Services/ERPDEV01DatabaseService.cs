using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.ModelERPDEV01;

namespace PSCS.Services
{
    public class ERPDEV01DatabaseService
    {
        public AMT_HistoryEntities Connect(string pConnectionString)
        {
            AMT_HistoryEntities result = null;

            result = new AMT_HistoryEntities();
            result.Database.Connection.ConnectionString = pConnectionString;

            return result;
        }
    }
}