using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using System.Transactions;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class RoleService
    {
        private PSCSEntities db;

        public RoleService(PSCSEntities pDb)
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

        public SelectList GetRole()
        {
            SelectList result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    List<PSC8040_M_ROLE> getYard = db.PSC8040_M_ROLE.ToList();
                    result = new SelectList(getYard, "ROLE", "NAME");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        #region Method
        public List<Role> GetRoleList()
        {
            List<Role> result = new List<Role>();

            try
            {
                using (this.db)
                {
                    result = db.PSC8040_M_ROLE.Select(p => new Role
                    {
                        RoleID = p.ROLE_ID,
                        NameTh = p.NAME_TH,
                        NameEn = p.NAME_EN,
                        CreateDate = p.CREATE_DATE,
                        CreateUserID = p.CREATE_USER_ID,
                        UpdateDate = p.UPDATE_DATE,
                        UpdateUserID = p.UPDATE_USER_ID
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public RoleEdit GetRoleById(string role_id)
        {
            RoleEdit result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    byte roleId = Convert.ToByte(role_id);

                    var obj = this.db.PSC8040_M_ROLE.SingleOrDefault(x => x.ROLE_ID == roleId);

                    result = new RoleEdit()
                    {
                        InputRoleID = obj.ROLE_ID.ToString(),
                        InputNameTh = obj.NAME_TH,
                        InputNameEn = obj.NAME_EN
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Role> GetRoleList(string pRoleId, string pNameTh, string pNameEn)
        {
            List<Role> result = new List<Role>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = db.PSC8040_M_ROLE.AsQueryable();

                    // Role
                    if (!string.IsNullOrEmpty(pRoleId))
                    {
                        byte roleId = Convert.ToByte(pRoleId);
                        obj = obj.Where(x => x.ROLE_ID == roleId);
                    }

                    // Name TH
                    if (!string.IsNullOrEmpty(pNameTh))
                    {
                        obj = obj.Where(x => x.NAME_TH.Contains(pNameTh));
                    }

                    // Name EN
                    if (!string.IsNullOrEmpty(pNameEn))
                    {
                        obj = obj.Where(x => x.NAME_EN.Contains(pNameEn));
                    }


                    result = obj.AsEnumerable().Select((x, index) => new Role
                    {
                        RowNo = index + 1,
                        RoleID = x.ROLE_ID,
                        NameTh = x.NAME_TH,
                        NameEn = x.NAME_EN
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public bool InsertData(RoleEdit editModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime insertDate = DateTime.Now;
                
                        PSC8040_M_ROLE insert = new PSC8040_M_ROLE();
                        insert.NAME_TH = editModel.InputNameTh.Trim();
                        insert.NAME_EN = editModel.InputNameEn.Trim();
                        insert.CREATE_DATE = insertDate;
                        insert.CREATE_USER_ID = userId;
                        insert.UPDATE_DATE = insertDate;
                        insert.UPDATE_USER_ID = userId;

                        this.db.PSC8040_M_ROLE.Add(insert);

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


        public bool UpdateData(RoleEdit editModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        byte roleId = Convert.ToByte(editModel.InputRoleID);

                        var update = this.db.PSC8040_M_ROLE.SingleOrDefault(x => x.ROLE_ID == roleId);

                        if (update != null)
                        {
                            update.NAME_TH = editModel.InputNameTh.Trim();
                            update.NAME_EN = editModel.InputNameEn.Trim();
                            update.UPDATE_USER_ID = userId;
                            update.UPDATE_DATE = DateTime.Now;
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
                catch(Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }


        public bool DeleteData(string pRoleId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    int result = 0;

                    byte roleId = Convert.ToByte(pRoleId);

                    var objRole = this.db.PSC8040_M_ROLE.SingleOrDefault(x => x.ROLE_ID == roleId);

                    if (objRole != null)
                    {
                        var objUser = this.db.PSC8030_M_USER.Where(x => x.ROLE_ID == roleId);

                        if (objUser.Count() != 0 )
                        {
                            result = 0;
                        }
                        else
                        {
                            db.PSC8040_M_ROLE.Remove(objRole);
                            result = db.SaveChanges();
                        }
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
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }
        #endregion
    }
}