using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PSCS.Models;
using PSCS.ModelsScreen;
using PSCS.Common;

namespace PSCS.Services
{
    public class MonthlyCloseService
    {
        private PSCSEntities db;

        public MonthlyCloseService(PSCSEntities pDb)
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

        public MonthlyClose GetOpenMonthlyClose()
        {
            MonthlyClose result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                //var obj = this.db.PSC3010_M_MONTHLY_CLOSE.Where(mc => mc.STATUS.Equals((int)Common.Constants.MonthlyCloseStatus.Open))
                //    .OrderByDescending(mc => mc.YEAR).ThenByDescending(mc => mc.MONTHLY);

                var obj = this.db.PSC3010_M_MONTHLY_CLOSE.Where(mc => mc.STATUS.Equals((int)Common.Constants.MonthlyCloseStatus.Open))
                    .OrderBy(mc => mc.YEAR).ThenBy(mc => mc.MONTHLY);

                if (obj != null)
                {
                    var objData = obj.AsEnumerable()
                          .Select((mc, index) => new MonthlyClose
                          {
                              Year = mc.YEAR,
                              Monthly = mc.MONTHLY,
                              ControlDate = Convert.ToDateTime(mc.CONTROL_DATE),
                              Status = mc.STATUS,
                              CreateDate = mc.CREATE_DATE,
                              CreateUserID = mc.CREATE_USER_ID,
                              UpdateDate = mc.UPDATE_DATE,
                              UpdateUserID = mc.UPDATE_USER_ID

                          }).ToList();
                    result = objData[0];
                    //result = new MonthlyClose()
                    //{
                    //    //Year = obj[0].YEAR,
                    //    //Monthly = obj.MONTHLY,
                    //    //ControlDate = Convert.ToDateTime(obj.CONTROL_DATE),
                    //    //Status = obj.STATUS,
                    //    //CreateDate = obj.CREATE_DATE,
                    //    //CreateUserID = obj.CREATE_USER_ID,
                    //    //UpdateDate = obj.UPDATE_DATE,
                    //    //UpdateUserID = obj.UPDATE_USER_ID
                    //};
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<MonthlyClose> GetMonthlyCloseList()
        {
            List<MonthlyClose> result = new List<MonthlyClose>();

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                var obj = (from mc in db.PSC3010_M_MONTHLY_CLOSE
                           orderby mc.YEAR, mc.MONTHLY
                           select new MonthlyClose
                           {
                               Year = mc.YEAR,
                               Monthly = mc.MONTHLY,
                               ControlDate = Convert.ToDateTime(mc.CONTROL_DATE),
                               Status = mc.STATUS,
                               CreateDate = mc.CREATE_DATE,
                               CreateUserID = mc.CREATE_USER_ID,
                               UpdateDate = mc.UPDATE_DATE,
                               UpdateUserID = mc.UPDATE_USER_ID
                           }).AsQueryable();

                result = obj.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public int UpdateMonthlyClose(DateTime pFilterMonthlyDate, string pUserId)
        {
            int result = 5;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        int filterMonth = pFilterMonthlyDate.Month;
                        int filterYear = pFilterMonthlyDate.Year;

                        //DateTime filterDateTime = new DateTime(filterYear, filterMonth, DateTime.DaysInMonth(filterYear, filterMonth));

                        var checkMonthUsed = this.db.PSC3010_M_MONTHLY_CLOSE.Where(mc => mc.STATUS.Equals((int)Common.Constants.MonthlyCloseStatus.Open)).OrderByDescending(mc => mc.YEAR).ThenByDescending(mc => mc.MONTHLY);
                        if (checkMonthUsed == null || checkMonthUsed.Count() == 0 || checkMonthUsed.Count() > 2)
                        {
                            //Check data in Database 
                            result = 2;
                            return result;
                        }

                        int checkIsDateFilterisUsed = checkMonthUsed.Count();
                        foreach (PSC3010_M_MONTHLY_CLOSE en in checkMonthUsed)
                        {
                            if(en.YEAR != filterYear || en.MONTHLY != filterMonth)
                            {
                                checkIsDateFilterisUsed--;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (checkIsDateFilterisUsed == 0)
                        {
                            //Filter not in this Month
                            result = 3;
                            return result;
                        }
                        else if (checkIsDateFilterisUsed == 2)
                        {
                            //Please close previous month
                            result = 4;
                            return result;
                        }
                        else
                        {
                            var monthClose = checkMonthUsed.OrderBy(mc => mc.YEAR).ThenBy(mc => mc.MONTHLY).FirstOrDefault();

                            //Update
                            monthClose.CONTROL_DATE = DateTime.Now;
                            monthClose.STATUS = (int)Common.Constants.MonthlyCloseStatus.Closed;
                            monthClose.UPDATE_USER_ID = pUserId;
                            monthClose.UPDATE_DATE = DateTime.Now;

                            DateTime nextYearMonth = pFilterMonthlyDate.AddMonths(1);
                            int nextMonth = nextYearMonth.Month;
                            int nextYear = nextYearMonth.Year;

                            var nextMonthData = this.db.PSC3010_M_MONTHLY_CLOSE.Where(mc => mc.YEAR == nextYear && mc.MONTHLY == nextMonth).SingleOrDefault();

                            if (nextMonthData == null)
                            {
                                //Insert new month
                                PSC3010_M_MONTHLY_CLOSE insert = new PSC3010_M_MONTHLY_CLOSE();
                                insert.YEAR = nextYear;
                                insert.MONTHLY = Convert.ToByte(nextMonth);
                                insert.CONTROL_DATE = new DateTime(nextYear, nextMonth, 1);
                                insert.STATUS = (int)Common.Constants.MonthlyCloseStatus.Open;
                                insert.CREATE_USER_ID = pUserId;
                                insert.UPDATE_USER_ID = pUserId;
                                insert.CREATE_DATE = DateTime.Now;
                                insert.UPDATE_DATE = DateTime.Now;

                                this.db.PSC3010_M_MONTHLY_CLOSE.Add(insert);
                            }
                            else
                            {
                                //Update
                                nextMonthData.CONTROL_DATE = DateTime.Now;
                                nextMonthData.STATUS = (int)Common.Constants.MonthlyCloseStatus.Open;
                                nextMonthData.UPDATE_USER_ID = pUserId;
                                nextMonthData.UPDATE_DATE = DateTime.Now;
                            }
                            
                        }

                        int Saveresult = this.db.SaveChanges();
                        if (Saveresult == 2)
                        {
                            //Save success
                            tran.Complete();
                            result = 1;
                        }
                        else
                        {
                            //Can't savechange
                            tran.Dispose();
                            result = 5;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }

            return result;
        }

        public int RestoreMonthlyClose(DateTime pFilterMonthlyDate, string pUserId)
        {
            int result = 4;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        int filterMonth = pFilterMonthlyDate.Month;
                        int filterYear = pFilterMonthlyDate.Year;

                        DateTime filterDateTime = new DateTime(filterYear, filterMonth, 1);

                        var checkMonthUsed = this.db.PSC3010_M_MONTHLY_CLOSE.Where(mc => mc.STATUS.Equals((int)Common.Constants.MonthlyCloseStatus.Open));
                        if (checkMonthUsed == null || checkMonthUsed.Count() == 0 || checkMonthUsed.Count() > 2)
                        {
                            //Check data in Database 
                            result = 2;
                            return result;
                        }


                        var ThisMonth = checkMonthUsed.OrderByDescending(mc => mc.YEAR).ThenByDescending(mc => mc.MONTHLY).FirstOrDefault();

                        if(ThisMonth.YEAR == filterYear && ThisMonth.MONTHLY == filterMonth)
                        {
                            if (checkMonthUsed.Count() == 2)
                            {
                                //Alreay Restore
                                result = 4;
                                return result;
                            }

                            //Get Yesterday
                            DateTime filterRestoreDateTime = filterDateTime.AddDays(-1);
                            int filterRestoreMonth = filterRestoreDateTime.Month;
                            int filterRestoreYear = filterRestoreDateTime.Year;

                            var updateRestoreMonth = this.db.PSC3010_M_MONTHLY_CLOSE.Where(mc => mc.YEAR == filterRestoreYear && mc.MONTHLY == filterRestoreMonth).SingleOrDefault();
                            if (updateRestoreMonth != null)
                            {
                                //Update
                                updateRestoreMonth.CONTROL_DATE = new DateTime(filterRestoreYear, filterRestoreMonth, 1);
                                updateRestoreMonth.STATUS = (int)Common.Constants.MonthlyCloseStatus.Open;
                                updateRestoreMonth.UPDATE_USER_ID = pUserId;
                                updateRestoreMonth.UPDATE_DATE = DateTime.Now;
                            }

                            int Saveresult = this.db.SaveChanges();

                            if (Saveresult == 1)
                            {
                                //Save success
                                tran.Complete();
                                result = 1;
                            }
                            else
                            {
                                //Can't savechange
                                tran.Dispose();
                                result = 6;
                            }
                        } 
                        else
                        {
                            if (checkMonthUsed.Count() == 2)
                            {
                                //Can restore data only 1 month 
                                result = 3;
                                return result;
                            }
                            else
                            {
                                ///MonthCloseFilter not in used 
                                result = 5;
                                return result;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }

            return result;
        }

    }
}