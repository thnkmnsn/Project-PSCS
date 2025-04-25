using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PSCS.Models;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class AccountService
    {
        private PSCSEntities db;

        public AccountService(PSCSEntities pDb)
        {
            this.db = pDb;
        }

        public User GetUserLogin(String pUsername, String pPassword)
        {
            User result =  null;

            using (this.db)
            {
                var data = db.PSC8030_M_USER.Where(x => x.USER_ID == pUsername && x.PASSWORD == pPassword)
                           .Select(m => new User
                           {
                               UserId = m.USER_ID,
                               UserName = m.USER_NAME,
                               Password = m.PASSWORD,
                               Language = m.LANGUAGE,
                               RoleId = m.ROLE_ID,
                               Active = m.ACTIVE,
                               CreateDate = m.CREATE_DATE,
                               CreateUserID = m.CREATE_USER_ID,
                               UpdateDate = m.UPDATE_DATE,
                               UpdateUserID = m.UPDATE_USER_ID,
                           }).AsQueryable();

                if (data.Count() == 1)
                {
                    result = data.FirstOrDefault();
                    result.IsLogin = true;
                }
                else
                {
                    result = new User();
                    result.IsLogin = false;
                }
            }

            return result;
        }


        public User GetUserById(String pUserId)
        {
            User result = new User();

            try
            {
                using (this.db)
                {
                    var data = this.db.PSC8030_M_USER.SingleOrDefault(x => x.USER_ID == pUserId);

                    result = new User()
                    {
                        UserId = data.USER_ID,
                        UserName = data.USER_NAME,
                        Password = data.PASSWORD,
                        Language = data.LANGUAGE,
                        RoleId = data.ROLE_ID,
                        Active = data.ACTIVE,
                        CreateDate = data.CREATE_DATE,
                        CreateUserID = data.CREATE_USER_ID,
                        UpdateDate = data.UPDATE_DATE,
                        UpdateUserID = data.UPDATE_USER_ID,
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public int ChangePassword(ChangePasswordScreen pwModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        var update = this.db.PSC8030_M_USER.SingleOrDefault(x => x.USER_ID == userId && x.PASSWORD == pwModel.CurrentPassword);

                        if (update != null)
                        {
                            update.PASSWORD = pwModel.NewPassword;
                            update.UPDATE_USER_ID = userId;
                            update.UPDATE_DATE = DateTime.Now;
                        }
                        else
                        {
                            // if current not match
                            tran.Dispose();
                            return 0;
                        }

                        int result = db.SaveChanges();

                        if (result > 0)
                        {
                            // if change sucess
                            tran.Complete();
                            return 1;
                        }
                        else
                        {
                            // if change fail
                            tran.Dispose();
                            return -1;
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