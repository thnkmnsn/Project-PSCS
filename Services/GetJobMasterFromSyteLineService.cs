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
    public class GetJobMasterFromSyteLineService
    {
        private ModelERPDEV01.AMT_HistoryEntities db;

        public GetJobMasterFromSyteLineService(ModelERPDEV01.AMT_HistoryEntities pDb)
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

        public List<JobMasterData> GetWIFromSyteLine(string pJob)
        {
            List<JobMasterData> result = new List<JobMasterData>();

            try
            {
                using (this.db)
                {

                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from jb in db.JobMasters
                               where jb.job == pJob
                               orderby jb.job
                               select new JobMasterData
                               {
                                   Job = jb.job,
                                   MFG = jb.MFG,
                                   ItemCode = jb.ItemCode,
                                   Heat = jb.Heat,
                                   Qty = jb.Qty,
                                   Description = jb.Description,
                                   Product = jb.Product,
                                   GroupMFGno = jb.GroupMFGno,
                                   MFGnum = jb.MFGnum,
                               }).AsQueryable();

                    result = obj.ToList();
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