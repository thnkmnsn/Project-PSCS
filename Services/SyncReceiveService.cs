using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.ModelERPDEV01;
using System.Transactions;
using System.Data.SqlClient;
using System.Data.Objects;

namespace PSCS.Services
{
    public class SyncReceiveService
    {
        private ModelERPDEV01.AMT_HistoryEntities db;

        public SyncReceiveService(ModelERPDEV01.AMT_HistoryEntities pDb)
        {
            try
            {
                this.db = pDb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean CallProcedure(decimal pTransaction)
        {
            Boolean result = false;

            try
            {
                using (this.db)
                {

                    //var getdetailprojectlist = db.Database.SqlQuery("exec dbo.[SearchPipe] @heatNo", new SqlParameter("@heatNo", "E65998"));
                    //return getdetailprojectlist;
                    //object result1  = db.UpdatePipesp(pTransaction);

                    //ObjectParameter returnMessage = new ObjectParameter("Returned", typeof(string));
                    AMT_HistoryEntities objDb = new AMT_HistoryEntities();
                    //int intReturn = objDb.UpdatePipesp(1);
                    string str = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

    }
}