using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;

namespace PSCS.Services
{
    public class StocktakingExcelService
    {
        private PSCSEntities _db;
        private User _user;
        public List<string> ErrorMessages = new List<string>();

        public StocktakingExcelService(PSCSEntities db, User user)
        {
            _db = db;
            _user = user;
        }

        public List<PSC2050_T_STOCKTAKING_INSTRUCTION> GetData(DateTime date)
        {
            using (_db)
            {
                _db.Configuration.LazyLoadingEnabled = false;

                var psc2510 = _db.PSC2050_T_STOCKTAKING_INSTRUCTION
                            .Select(x => x)
                            .Where(x => x.STOCKTAKING_DATE == date)
                            .OrderBy(x => x.LOCATION_CODE).ThenBy(x => x.ITEM_CODE);

                if(psc2510 == null)
                {
                    return new List<PSC2050_T_STOCKTAKING_INSTRUCTION>();
                }
                else
                {
                    return psc2510.ToList();
                }
            }
        }

        public bool InsertUpdateData(DataTable dt, DateTime stocktakingDate)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                List<string> lstInfo = new List<string>();
                try
                {
                    using (_db)
                    {
                        
                        ErrorMessages.Clear();

                        _db.Configuration.LazyLoadingEnabled = false;

                        DateTime insertDate = DateTime.Now;
                        foreach (DataRow dr in dt.Rows)
                        {
                           
                            string location = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.LOCATION_CODE].ToString().Trim();
                            string itemCode = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.ITEM_CODE].ToString();
                            string heatNo = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.HEAT_NO].ToString();

                            lstInfo.Add("Location = " + location + " Item code = " + itemCode + ",Heat no = " + heatNo);

                            PSC8010_M_PIPE psc8010 = _db.PSC8010_M_PIPE
                                    .Select(x => x)
                                    .Where(x =>  x.ITEM_CODE.Equals(itemCode) && x.HEAT_NO.Equals(heatNo))
                                    .FirstOrDefault();

                            if (psc8010 == null)
                            {
                                // Insert
                                PSC8010_M_PIPE insert = new PSC8010_M_PIPE();
                                insert.ITEM_CODE = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.ITEM_CODE].ToString();
                                insert.HEAT_NO = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.HEAT_NO].ToString();
                                //insert.RECEIVED_DATE = Convert.ToDateTime(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.STOCKTAKING_DATE]);
                                insert.UNIT_WEIGHT = (decimal)dr[(int)Constants.STOCKLIST_INSTRUCTION_COL_NAME.WEIGHT];
                                insert.OD = (decimal)dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.OD];
                                insert.WT = (decimal)dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.WT];
                                insert.LT = (decimal)dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.LT];
                                insert.GRADE_NAME = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.GRADE_NAME].ToString();
                                insert.MAKER_NAME = dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.MAKER_NAME].ToString();
                                insert.CREATE_USER_ID = _user.UserId;
                                insert.CREATE_DATE = insertDate;
                                insert.UPDATE_USER_ID = _user.UserId;
                                insert.UPDATE_DATE = insertDate;
                                
                                _db.PSC8010_M_PIPE.Add(insert);
                            }

                            PSC2050_T_STOCKTAKING_INSTRUCTION psc2510 = _db.PSC2050_T_STOCKTAKING_INSTRUCTION
                                    .Select(x => x)
                                    .Where(x => x.STOCKTAKING_DATE == stocktakingDate && x.LOCATION_CODE.Equals(location) && x.ITEM_CODE.Equals(itemCode) && x.HEAT_NO.Equals(heatNo))
                                    .FirstOrDefault();

                            if (psc2510 == null)
                            {
                                // Insert
                                PSC2050_T_STOCKTAKING_INSTRUCTION insert = new PSC2050_T_STOCKTAKING_INSTRUCTION();
                                insert.STOCKTAKING_DATE = Convert.ToDateTime(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.STOCKTAKING_DATE]);
                                insert.LOCATION_CODE = Convert.ToString(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.LOCATION_CODE]);
                                insert.ITEM_CODE = Convert.ToString(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.ITEM_CODE]);
                                insert.HEAT_NO = Convert.ToString(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.HEAT_NO]);
                                insert.OD = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.OD]);
                                insert.WT = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.WT]);
                                insert.LT = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.LT]);
                                insert.GRADE_NAME = Convert.ToString(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.GRADE_NAME]);
                                insert.MAKER_NAME = Convert.ToString(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.MAKER_NAME]);
                                insert.REMARK = Convert.ToString(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.REMARK]);
                                insert.STOCK_QTY = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.STOCK_QTY]);
                                insert.ACTUAL_QTY = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.ACTUAL_QTY]);
                                insert.WEIGHT = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.WEIGHT]);
                                insert.STATUS = 1;
                                insert.CREATE_USER_ID = _user.UserId;
                                insert.CREATE_DATE = insertDate;
                                insert.UPDATE_USER_ID = _user.UserId;
                                insert.UPDATE_DATE = insertDate;

                                _db.PSC2050_T_STOCKTAKING_INSTRUCTION.Add(insert);
                            }
                            else
                            {
                                if(psc2510.STATUS == (int)Constants.StockTakingStatus.New)
                                {
                                    // Update
                                    psc2510.ACTUAL_QTY = Convert.ToDecimal(dr[(int)Constants.STOCKTAKING_INSTRUCTION_COL_NAME.ACTUAL_QTY]);
                                    psc2510.UPDATE_USER_ID = _user.UserId;
                                    psc2510.UPDATE_DATE = insertDate;
                                }
                                else
                                {
                                    Constants.StockTakingStatus myStatus = (Constants.StockTakingStatus)Convert.ToInt32(psc2510.STATUS);
                                    ErrorMessages.Add("can not update Item code = " + psc2510.ITEM_CODE +",Heat no = " + psc2510.HEAT_NO + " is status " + myStatus.ToString());
                                }
                                
                            }

                            int result = _db.SaveChanges();

                            if (result > 0)
                            {
                                
                            }
                            else
                            {
                                ErrorMessages.Add("can not Insert/update Item code = " + psc2510.ITEM_CODE + ",Heat no = " + psc2510.HEAT_NO );
                            }
                        }

                        if (ErrorMessages.Count == 0)
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