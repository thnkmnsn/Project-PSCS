using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PSCS.ModelsScreen;


namespace PSCS.Services
{
    public class StockService
    {
        private PSCSEntities _db;
        private User _user;

        public StockService(PSCSEntities db, User user)
        {
            _db = db;
            _user = user;
        }

        #region Stocktaking
        public bool InsertUpdateDataByStocktaking(List<StockTakingScreen> list)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (_db)
                    {
                        _db.Configuration.LazyLoadingEnabled = false;

                        DateTime insertDate = DateTime.Now;
                        foreach (StockTakingScreen s in list)
                        {
                            if (!s.Check)
                            {
                                continue;
                            }

                            // Insert/update data based on Stocktaking Year-Month!!!!
                            // No use PSC3010_M_MONTHLY_CLOSE.CONTROL_DATE!!!!
                            // yyyy-MM-01 00:00:00
                            DateTime stockDate = new DateTime(s.StockTakingDate.Year, s.StockTakingDate.Month, 1);
                            string itemCode = s.ItemCode;
                            string heatNo = s.HeatNo;
                            string locationCode = s.LocationID;

                            // Get data from PSC2010_T_STOCK
                            PSC2010_T_STOCK psc2010 = _db.PSC2010_T_STOCK
                                        .Select(x => x)
                                        .Where(x => x.YEAR_MONTH == stockDate && x.ITEM_CODE.Equals(itemCode) && x.HEAT_NO.Equals(heatNo) && x.LOCATION_CODE.Equals(locationCode))
                                        .FirstOrDefault();

                            if (psc2010 == null)
                            {
                                // No data
                                InsertDataByStocktaking(s, stockDate, insertDate);
                            }
                            else
                            {
                                if (s.ActualQty == psc2010.STOCK_QTY)
                                {
                                    continue;
                                }

                                // Have data
                                UpdateDataByStocktaking(psc2010, s, stockDate, insertDate);
                            }
                        }

                        int result = _db.SaveChanges();

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
                catch
                {
                    tran.Dispose();
                    throw;
                }
            }
        }

        private void InsertDataByStocktaking(StockTakingScreen s, DateTime stockDate, DateTime insertDate)
        {
            // 1. Insert PSC2010_T_STOCK (Current Year-Month)
            PSC2010_T_STOCK stock = new PSC2010_T_STOCK();
            stock.YEAR_MONTH = stockDate;
            stock.ITEM_CODE = s.ItemCode;
            stock.HEAT_NO = s.HeatNo;
            stock.LOCATION_CODE = s.LocationID;
            stock.STOCK_START_QTY = 0;
            stock.STOCK_IN_QTY = 0;
            stock.STOCK_OUT_QTY = 0;
            stock.STOCK_ADJUST_IN_QTY = 0;
            stock.STOCK_ADJUST_OUT_QTY = 0;
            stock.STOCK_QTY = 0;
            stock.CREATE_DATE = insertDate;
            stock.CREATE_USER_ID = _user.UserId;
            stock.UPDATE_DATE = insertDate;
            stock.UPDATE_USER_ID = _user.UserId;

            // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = Actual Qty)
            PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
            detail.YEAR_MONTH = stockDate;
            detail.ITEM_CODE = s.ItemCode;
            detail.HEAT_NO = s.HeatNo;
            detail.LOCATION_CODE = s.LocationID;
            detail.STOCK_IN_DATE = s.StockTakingDate;
            //detail.UPDATE_TYPE = (int)Constants.STOCK_DETAIL_UPDATE_TYPE.NORMAL;
            detail.STOCK_IO_TYPE = Constants.STOCKTAKING_IN;
            detail.QTY = s.ActualQty;
            detail.CREATE_DATE = insertDate;
            detail.CREATE_USER_ID = _user.UserId;
            detail.UPDATE_DATE = insertDate;
            detail.UPDATE_USER_ID = _user.UserId;

            // 3. Update PSC2010_T_STOCK.STOCK_ADJUST_IN_QTY = PSC2011_T_STOCK_DETAIL.QTY
            stock.STOCK_ADJUST_IN_QTY = detail.QTY;

            // 4. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
            stock.STOCK_QTY = stock.STOCK_START_QTY + stock.STOCK_IN_QTY - stock.STOCK_OUT_QTY + stock.STOCK_ADJUST_IN_QTY - stock.STOCK_ADJUST_OUT_QTY;

            _db.PSC2010_T_STOCK.Add(stock);
            _db.PSC2011_T_STOCK_DETAIL.Add(detail);
        }


        private void UpdateDataByStocktaking(PSC2010_T_STOCK psc2010, StockTakingScreen s, DateTime stockDate, DateTime insertDate)
        {
            // 1. Get data from PSC2011_T_STOCK_DETAIL
            //PSC2011_T_STOCK_DETAIL psc2011 = _db.PSC2011_T_STOCK_DETAIL
            //            .Select(x => x)
            //            .Where(x => x.YEAR_MONTH == stockDate && x.ITEM_CODE.Equals(s.ItemCode) && x.HEAT_NO.Equals(s.HeatNo) && x.LOCATION_CODE.Equals(s.LocationID))
            //            .OrderBy(x => x.ID)
            //            .LastOrDefault();

            PSC2011_T_STOCK_DETAIL psc2011 = _db.PSC2011_T_STOCK_DETAIL
                        .Select(x => x)
                        .Where(x => x.YEAR_MONTH == stockDate && x.ITEM_CODE.Equals(s.ItemCode) && x.HEAT_NO.Equals(s.HeatNo) && x.LOCATION_CODE.Equals(s.LocationID))
                        .OrderByDescending(x => x.ID )
                        .FirstOrDefault();

            // 2. Insert PSC2011_T_STOCK_DETAIL (QTY = PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY - PSC2010_T_STOCK.STOCK_QTY || PSC2010_T_STOCK.STOCK_QTY - PSC2050.STOCKTAKING_INSTRUCTION.ACTUAL_QTY)
            PSC2011_T_STOCK_DETAIL detail = new PSC2011_T_STOCK_DETAIL();
            detail.YEAR_MONTH = stockDate;
            detail.ITEM_CODE = s.ItemCode;
            detail.HEAT_NO = s.HeatNo;
            detail.LOCATION_CODE = s.LocationID;

            if(psc2011 == null)
            {
                detail.STOCK_IN_DATE = s.StockTakingDate;
            }
            else
            {
                detail.STOCK_IN_DATE = psc2011.STOCK_IN_DATE;
            }

            //detail.UPDATE_TYPE = (int)Constants.STOCK_DETAIL_UPDATE_TYPE.NORMAL;
            if (s.ActualQty > psc2010.STOCK_QTY)
            {
                detail.QTY = s.ActualQty - psc2010.STOCK_QTY;
                detail.STOCK_IO_TYPE = Constants.STOCKTAKING_IN;

                // 2. Update PSC2010_T_STOCK.STOCK_ADJUST_IN_QTY = PSC2011_T_STOCK_DETAIL.QTY
                psc2010.STOCK_ADJUST_IN_QTY += detail.QTY;
            }
            else
            {
                detail.QTY = psc2010.STOCK_QTY - s.ActualQty;
                detail.STOCK_IO_TYPE = Constants.STOCKTAKING_OUT;

                // 2. Update PSC2010_T_STOCK.STOCK_ADJUST_OUT_QTY = PSC2011_T_STOCK_DETAIL.QTY
                psc2010.STOCK_ADJUST_OUT_QTY += detail.QTY;
            }

            detail.CREATE_DATE = insertDate;
            detail.CREATE_USER_ID = _user.UserId;
            detail.UPDATE_DATE = insertDate;
            detail.UPDATE_USER_ID = _user.UserId;

            // 3. Update PSC2010_T_STOCK.STOCK_QTY = STOCK_START_QTY + STOCK_IN_QTY - STOCK_OUT_QTY + STOCK_ADJUST_IN_QTY - STOCK_ADJUST_OUT_QTY (Current)
            psc2010.STOCK_QTY = psc2010.STOCK_START_QTY + psc2010.STOCK_IN_QTY - psc2010.STOCK_OUT_QTY + psc2010.STOCK_ADJUST_IN_QTY - psc2010.STOCK_ADJUST_OUT_QTY;

            _db.PSC2011_T_STOCK_DETAIL.Add(detail);
        }
        #endregion
    }
}