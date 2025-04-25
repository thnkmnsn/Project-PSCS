using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Transactions;
using PSCS.Common;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class UserService
    {
        private PSCSEntities db;

        public UserService(PSCSEntities pDb)
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

        public List<User> GetUserList(string pFilterUserID, string pFilterUserName, string pFilterLanguage, string pFilterRole, List<byte> pFilterActive, string lang)
        {
            List<User> result = new List<User>();
            byte FilterRole = 0;

            try
            {
                if (!string.IsNullOrEmpty(pFilterRole))
                {
                    FilterRole = Convert.ToByte(pFilterRole);
                }
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var objUserList = (from ur in this.db.PSC8030_M_USER
                                       join ro in this.db.PSC8040_M_ROLE on ur.ROLE_ID equals ro.ROLE_ID
                                       where (pFilterUserID == string.Empty || ur.USER_ID.Contains(pFilterUserID))
                                       && (pFilterUserName == string.Empty || ur.USER_NAME.Contains(pFilterUserName))
                                       && (pFilterLanguage == string.Empty || ur.LANGUAGE.Trim() == pFilterLanguage)
                                       && (FilterRole == 0 || ur.ROLE_ID == FilterRole)
                                       select new
                                       {
                                           ur.USER_ID,
                                           ur.USER_NAME,
                                           ur.PASSWORD,
                                           ur.LANGUAGE,
                                           ur.ROLE_ID,
                                           ROLE_NAME = (lang.Equals("Th") ? ro.NAME_TH : ro.NAME_EN),
                                           ur.ACTIVE,
                                           ur.CREATE_DATE,
                                           ur.CREATE_USER_ID,
                                           ur.UPDATE_DATE,
                                           ur.UPDATE_USER_ID,
                                       }).ToList();

                    result = objUserList.AsEnumerable().Where(xx => pFilterActive.Contains(xx.ACTIVE)).Select((x, index) => new User
                    {
                        RowNo = index + 1,
                        UserId = x.USER_ID,
                        UserName = x.USER_NAME,
                        Password = x.PASSWORD,
                        Language = x.LANGUAGE,
                        LanguageName = GetLanguage(x.LANGUAGE),
                        RoleId = x.ROLE_ID,
                        RoleName = x.ROLE_NAME,
                        Active = x.ACTIVE,
                        ActiveName = GetActive(x.ACTIVE),
                        CreateDate = x.CREATE_DATE,
                        CreateUserID = x.CREATE_USER_ID,
                        UpdateDate = x.UPDATE_DATE,
                        UpdateUserID = x.UPDATE_USER_ID,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //public PSC8040_M_USER GetUser(User pUser)
        //{
        //    PSC8040_M_USER result = null;

        //    try
        //    {

        //        using (this.db)
        //        {
        //            result = this.db.PSC8040_M_USER.SingleOrDefault(ur => ur.USER_ID == pUser.UserId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        public UserScreenEdit GetUserById(string pUserId)
        {
            UserScreenEdit result = new UserScreenEdit();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = this.db.PSC8030_M_USER.SingleOrDefault(x => x.USER_ID == pUserId);

                    result = new UserScreenEdit()
                    {
                        EditUserID = obj.USER_ID,
                        EditUserName = obj.USER_NAME,
                        EditPassword = obj.PASSWORD,
                        EditLanguageSelected = obj.LANGUAGE.Trim(),
                        EditActiveSelected = obj.ACTIVE == (int)Constants.ActiveStatus.Active ? ((int)Constants.ActiveStatus.Active).ToString() :
                                                obj.ACTIVE == (int)Constants.ActiveStatus.InActive ? ((int)Constants.ActiveStatus.InActive).ToString() : "",
                        EditRoleIdSelected = obj.ROLE_ID
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean Insert(string pModalUserId, string pModalUserName, string pModalPassword, string pModalLanguageSelected, Int32 pModalRoleIdSelected, string pModalActiveSelected, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        var objUser = this.db.PSC8030_M_USER.SingleOrDefault(ur => ur.USER_ID == pModalUserId);
                        if (objUser == null)
                        {
                            PSC8030_M_USER objNewUser = new PSC8030_M_USER();

                            objNewUser.USER_ID = pModalUserId;
                            objNewUser.USER_NAME = pModalUserName;
                            objNewUser.PASSWORD = pModalPassword;
                            objNewUser.LANGUAGE = pModalLanguageSelected.Trim();
                            objNewUser.ROLE_ID = Convert.ToByte(pModalRoleIdSelected);
                            objNewUser.ACTIVE = Convert.ToByte(pModalActiveSelected);
                            objNewUser.CREATE_USER_ID = LoginUser.UserId;
                            objNewUser.CREATE_DATE = DateTime.Now;
                            objNewUser.UPDATE_USER_ID = LoginUser.UserId;
                            objNewUser.UPDATE_DATE = DateTime.Now;

                            this.db.PSC8030_M_USER.Add(objNewUser);
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
            }

            return result;
        }

        public Boolean Update(string pModalUserId, string pModalUserName, string pModalPassword, string pModalLanguageSelected, Int32 pModalRoleIdSelected, string pModalActiveSelected, User LoginUser)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        var objUser = this.db.PSC8030_M_USER.SingleOrDefault(ur => ur.USER_ID == pModalUserId);
                        if (objUser != null)
                        {
                            objUser.USER_NAME = pModalUserName;
                            objUser.PASSWORD = pModalPassword;
                            objUser.LANGUAGE = pModalLanguageSelected.Trim();
                            objUser.ROLE_ID = Convert.ToByte(pModalRoleIdSelected);
                            objUser.ACTIVE = Convert.ToByte(pModalActiveSelected);
                            objUser.UPDATE_USER_ID = LoginUser.UserId;
                            objUser.UPDATE_DATE = DateTime.Now;
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
            }

            return result;
        }

        public Boolean Delete(string pModalUserId)
        {
            Boolean result = false;
            TransactionScope tran = null;
            int flag = 0;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                        var objUser = this.db.PSC8030_M_USER.SingleOrDefault(ur => ur.USER_ID == pModalUserId);
                        if (objUser != null)
                        {
                            this.db.PSC8030_M_USER.Remove(objUser);
                            flag = this.db.SaveChanges();
                        }

                        if (flag >= 1)
                        {
                            result = true;
                            tran.Complete();
                        }
                        else
                        {
                            result = false;
                            tran.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
            }

            return result;
        }

        #region Private
        private string GetActive(byte pActive)
        {
            string result = string.Empty;
            if (pActive == 1)
            {
                result = PSCS.Resources.Common_cshtml.Active;
            }
            else if (pActive == 2)
            {
                result = PSCS.Resources.Common_cshtml.InActive;
            }
            else
            {
                result = "";
            }
            return result;
        }

        private string GetLanguage(string pLanguage)
        {
            string result = string.Empty;
            if (pLanguage.Trim().Equals("1"))
            {
                result = PSCS.Resources.Common_cshtml.English;
            }
            else if (pLanguage.Trim().Equals("2"))
            {
                result = PSCS.Resources.Common_cshtml.Thai;
            }
            else
            {
                result = "";
            }
            return result;
        }
        #endregion
    }
}