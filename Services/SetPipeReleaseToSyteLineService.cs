using System;
using System.Web;
using System.Linq;
using PSCS.ModelERPDEV01;
using System.Data.Objects;
using System.Transactions;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace PSCS.Services
{
    public class SetPipeReleaseToSyteLineService
    {
        private ModelERPDEV01.AMT_HistoryEntities db;

        public SetPipeReleaseToSyteLineService(ModelERPDEV01.AMT_HistoryEntities pDb)
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

        public bool InsertData(Pipe_Release pPipeRelease, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        if (pPipeRelease != null)
                        {
                            this.db.Pipe_Release.Add(pPipeRelease);
                        }

                        int result = this.db.SaveChanges();

                        if (result > 0)
                        {
                            tran.Complete();
                            return true;
                        }
                        else
                        {
                            tran.Dispose();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }

        public bool InsertData(List<Pipe_Release> pPipeReleaseList, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        
                        if(pPipeReleaseList != null)
                        {
                            foreach(Pipe_Release en in pPipeReleaseList)
                            {
                                this.db.Pipe_Release.Add(en);
                            }  
                        }
                        
                        int result = this.db.SaveChanges();

                        if (result > 0)
                        {
                            tran.Complete();
                            return true;
                        }
                        else
                        {
                            tran.Dispose();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }

    }
}