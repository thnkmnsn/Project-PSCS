using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using PSCS.Models;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class CommonMasterService
    {
        private PSCSEntities db;

        public CommonMasterService(PSCSEntities pDb)
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

        public List<CommonMaster> GetCommonMasterList()
        {
            List<CommonMaster> result = new List<CommonMaster>();

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                var obj = (from cm in db.PSC8050_M_COMMON
                           orderby cm.PARENT_CODE,cm.COMMON_CODE
                           select new CommonMaster
                           {
                               ParentCode = cm.PARENT_CODE,
                               CommonCode = cm.COMMON_CODE,
                               ValueEn = cm.VALUE_EN,
                               ValueTh = cm.VALUE_TH,
                               CreateDate = cm.CREATE_DATE,
                               CreateUserID = cm.CREATE_USER_ID,
                               UpdateDate = cm.UPDATE_DATE,
                               UpdateUserID = cm.UPDATE_USER_ID
                           }).AsQueryable();

                //result = obj.ToList();
                result = obj.AsEnumerable().Select((x, index) => new CommonMaster
                {
                    RowNo = index + 1,
                    ParentCode = x.ParentCode,
                    CommonCode = x.CommonCode,
                    ValueEn = x.ValueEn,
                    ValueTh = x.ValueTh,
                    CreateDate = x.CreateDate,
                    CreateUserID = x.CreateUserID,
                    UpdateDate = x.UpdateDate,
                    UpdateUserID = x.UpdateUserID,
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public CommonMasterScreenEdit GetValueById(string parentCode,string commonCode)
        {
            CommonMasterScreenEdit result = new CommonMasterScreenEdit();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = this.db.PSC8050_M_COMMON.SingleOrDefault(x => x.PARENT_CODE == parentCode && x.COMMON_CODE == commonCode);

                    result = new CommonMasterScreenEdit()
                    {
                        EditParentCode = obj.PARENT_CODE,
                        EditCommonCode = obj.COMMON_CODE,
                        EditValueEN = obj.VALUE_EN,
                        EditValueTH = obj.VALUE_TH,
                        
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }


        public bool Insert(CommonMasterScreenEdit editModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime insertDate = DateTime.Now;

                        var obj = this.db.PSC8050_M_COMMON.SingleOrDefault(x => x.PARENT_CODE == editModel.EditParentCode && x.COMMON_CODE == editModel.EditCommonCode);

                        // Check duplicate data before insert
                        if (obj == null)
                        {
                            PSC8050_M_COMMON insert = new PSC8050_M_COMMON();

                            insert.PARENT_CODE = editModel.EditParentCode;
                            insert.COMMON_CODE = editModel.EditCommonCode;
                            insert.VALUE_EN = editModel.EditValueEN;
                            insert.VALUE_TH = editModel.EditValueTH;
                            insert.CREATE_DATE = insertDate;
                            insert.CREATE_USER_ID = userId;
                            insert.UPDATE_DATE = insertDate;
                            insert.UPDATE_USER_ID = userId;

                            this.db.PSC8050_M_COMMON.Add(insert);
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

        public bool Update(CommonMasterScreenEdit editModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC8050_M_COMMON.SingleOrDefault(x => x.PARENT_CODE == editModel.EditParentCode && x.COMMON_CODE == editModel.EditCommonCode);

                        // Check duplicate data before insert
                        if (update != null)
                        {
                            
                            //update.PARENT_CODE = editModel.EditParentCode;
                            //update.COMMON_CODE = editModel.EditCommonCode;
                            update.VALUE_EN = editModel.EditValueEN;
                            update.VALUE_TH = editModel.EditValueTH;
                            update.UPDATE_DATE = updateDate;
                            update.UPDATE_USER_ID = userId;
                            
                        }

                        int result = db.SaveChanges();
                        

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


        public bool DeleteData(CommonMasterScreenEdit editModel)
        {
            int result = 0;
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        // Get data from PSC8020_M_LOCATION
                        var obj = this.db.PSC8050_M_COMMON.SingleOrDefault(x => x.PARENT_CODE == editModel.EditParentCode && x.COMMON_CODE == editModel.EditCommonCode);


                        if (obj != null)
                        {
                          
                                db.PSC8050_M_COMMON.Remove(obj);
                                result = db.SaveChanges();
                          
                        }

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