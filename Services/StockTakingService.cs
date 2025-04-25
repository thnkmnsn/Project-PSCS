using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Transactions;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class StockTakingService
    {
        private PSCSEntities db;

        public StockTakingService(PSCSEntities pDb)
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

        public DateTime GetStockTakingMaxDate()
        {
            try
            {
                DateTime StockTakingMaxDate = DateTime.Now;
                using (PSCSEntities db = new PSCSEntities())
                {
                    StockTakingMaxDate = (from d in db.PSC2050_T_STOCKTAKING_INSTRUCTION select d.STOCKTAKING_DATE).DefaultIfEmpty(DateTime.Today).Max();
                }
                return StockTakingMaxDate;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void CreateExpression(string pDate,
                             string pYardID,
                             string pLocationID,
                             string pItemCode,
                             string pHeatNo,
                             string pOD,
                             string pWT,
                             string pLength,
                             string pGrade,
                             string pMaker,
                             string pStatus,
                            ref List<Expression<Func<StockTakingScreen, bool>>> andEx)
        {
            try
            {
                // Add "and" condition
                if (!string.IsNullOrEmpty(pDate))
                {
                    DateTime dtStockTaking = Convert.ToDateTime(pDate).Date;
                    andEx.Add(x => x.StockTakingDate == dtStockTaking);
                }

                if (!string.IsNullOrEmpty(pYardID))
                {
                    andEx.Add(x => x.FilterYardID == pYardID);
                }

                if (!string.IsNullOrEmpty(pLocationID))
                {
                    andEx.Add(x => x.FilterLocationID == pLocationID);
                }

                if (!string.IsNullOrEmpty(pItemCode))
                {
                    andEx.Add(x => x.ItemCode.Contains(pItemCode));
                }

                if (!string.IsNullOrEmpty(pHeatNo))
                {
                    andEx.Add(x => x.HeatNo.Contains(pHeatNo));
                }

                if (!string.IsNullOrEmpty(pOD))
                {
                    decimal od = Convert.ToDecimal(pOD);
                    andEx.Add(x => x.OD == od);
                }

                if (!string.IsNullOrEmpty(pWT))
                {
                    decimal wt = Convert.ToDecimal(pWT);
                    andEx.Add(x => x.WT == wt);
                }

                if (!string.IsNullOrEmpty(pLength))
                {
                    decimal lt = Convert.ToDecimal(pLength);
                    andEx.Add(x => x.Lenght == lt);
                }

                if (!string.IsNullOrEmpty(pGrade))
                {
                    andEx.Add(x => x.Grade.Contains(pGrade));
                }

                if (!string.IsNullOrEmpty(pMaker))
                {
                    andEx.Add(x => x.Maker.Contains(pMaker));
                }

                if (!string.IsNullOrEmpty(pStatus))
                {
                    byte status = Convert.ToByte(pStatus);
                    andEx.Add(x => x.FilterStatus == status);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //sample query from https://forums.asp.net/t/2068158.aspx?Joining+tables+through+lambda+expression
        /*public List<StockTakingScreen> GetStockTakingsList(List<Expression<Func<StockTakingScreen, bool>>> andExList)
        {
            List<StockTakingScreen> result = new List<StockTakingScreen>();

            try
            {
                using (this.db)
                {

                    db.Configuration.LazyLoadingEnabled = false;

                    var data = db.PSC2050_T_STOCKTAKING_INSTRUCTION
                                .Join(db.PSC8020_M_LOCATION, ST => ST.LOCATION_CODE, LO => LO.LOCATION_CODE, (ST, LO) => new { ST, LO })
                                .Join(db.PSC8022_M_YARD, STLO => STLO.LO.YARD, YA => YA.YARD, (STLO, YA) => new { STLO, YA })
                                .Select(m => new StockTakingScreen
                                {
                                    StockTakingDate = m.STLO.ST.STOCKTAKING_DATE,
                                    FilterYardID = m.YA.YARD,
                                    PipeYard = m.YA.NAME,
                                    FilterLocationID = m.STLO.ST.LOCATION_CODE,
                                    Location = m.STLO.LO.NAME,
                                    ItemCode = m.STLO.ST.ITEM_CODE,
                                    HeatNo = m.STLO.ST.HEAT_NO,
                                    OD = m.STLO.ST.OD,
                                    WT = m.STLO.ST.WT,
                                    Lenght = m.STLO.ST.LT,
                                    unit_weight = m.STLO.ST.WEIGHT,
                                    Grade = m.STLO.ST.GRADE_NAME,
                                    Maker = m.STLO.ST.MAKER_NAME,
                                    Remark = m.STLO.ST.REMARK,
                                    CurrentQty = m.STLO.ST.STOCK_QTY,
                                    ActualQty = m.STLO.ST.ACTUAL_QTY,
                                    FilterStatus = m.STLO.ST.STATUS
                                }).AsQueryable();

                    foreach (var andEx in andExList)
                    {
                        data = data.Where(andEx);
                    }

                    var obj = data.AsEnumerable().Select((x, index) => new StockTakingScreen
                    {
                        RowNo = index + 1,
                        PipeYard = x.PipeYard,
                        Location = x.Location,
                        ItemCode = x.ItemCode,
                        HeatNo = x.HeatNo,
                        OD = x.OD,
                        WT = x.WT,
                        Lenght = x.Lenght,
                        Grade = x.Grade,
                        Maker = x.Maker,
                        Remark = x.Remark,
                        CurrentQty = x.CurrentQty,
                        ActualQty = x.ActualQty,
                        unit_weight = x.unit_weight,
                    });

                    if (obj.Count() > 0)
                    {
                        result = obj.ToList();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }*/

        public List<StockTakingScreen> GetStockTakingList(DateTime pYearMonth, string pYardID, string pLocationCode, string pItemCode,
                            string pHeatNo, decimal pOD, decimal pWT, decimal pLength, string pGrade, string pMaker, List<byte> pStatus)
        {
            List<StockTakingScreen> result = new List<StockTakingScreen>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    List<byte> uids = new List<byte>() { 1, 2 };

                    //List<byte> uids = new List<byte>();// { 1,2 };
                    //uids.Add(Convert.ToByte("1"));
                    //uids.Add(Convert.ToByte("2"));
                    var objStockList = (from st in this.db.PSC2050_T_STOCKTAKING_INSTRUCTION
                                            //join pi in db.PSC8010_M_PIPE on sl.ITEM_CODE equals pi.ITEM_CODE
                                        join lo in db.PSC8020_M_LOCATION on st.LOCATION_CODE equals lo.LOCATION_CODE
                                        join yd in db.PSC8022_M_YARD on lo.YARD equals yd.YARD
                                        where (pYardID == string.Empty || yd.YARD == pYardID)
                                        && (pLocationCode == string.Empty || lo.LOCATION_CODE == pLocationCode)
                                        && (pItemCode == string.Empty || st.ITEM_CODE.Contains(pItemCode))
                                        && (pHeatNo == string.Empty || st.HEAT_NO.Contains(pHeatNo))
                                        && (pOD == 0 || st.OD == pOD)
                                        && (pWT == 0 || st.WT == pWT)
                                        && (pLength == 0 || st.LT == pLength)
                                        && (pGrade == string.Empty || st.GRADE_NAME.Contains(pGrade))
                                        && (pMaker == string.Empty || st.MAKER_NAME.Contains(pMaker))
                                        && (pYearMonth == DateTime.MinValue || st.STOCKTAKING_DATE == pYearMonth)
                                        //&& (uids == null || uids.Contains(st.STATUS))

                                        select new
                                        {
                                            st.STOCKTAKING_DATE,
                                            st.ITEM_CODE,
                                            st.HEAT_NO,
                                            st.LOCATION_CODE,
                                            LOCATION_NAME = lo.NAME,
                                            yd.YARD,
                                            YARD_NAME = yd.NAME,
                                            st.STATUS,
                                            st.OD,
                                            st.WT,
                                            st.LT,
                                            st.GRADE_NAME,
                                            st.MAKER_NAME,
                                            st.REMARK,
                                            st.WEIGHT,
                                            st.CREATE_DATE,
                                            st.CREATE_USER_ID,
                                            st.UPDATE_DATE,
                                            st.UPDATE_USER_ID,
                                            st.STOCK_QTY,
                                            st.ACTUAL_QTY,
                                        }).ToList();

                    result = objStockList.AsEnumerable()
                        .Where(xx => pStatus.Contains(xx.STATUS))
                        .Select((x, index) => new StockTakingScreen
                        {
                            RowNo = index + 1,
                            StockTakingDate = x.STOCKTAKING_DATE,
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            LocationID = x.LOCATION_CODE,
                            Location = x.LOCATION_NAME,
                            PipeYard = x.YARD_NAME,
                            OD = x.OD,
                            WT = x.WT,
                            Lenght = x.LT,
                            Grade = x.GRADE_NAME,
                            Maker = x.MAKER_NAME,
                            Remark = x.REMARK,
                            CurrentQty = x.STOCK_QTY,
                            ActualQty = x.ACTUAL_QTY,
                            Status = x.STATUS == (int)Constants.StockTakingStatus.New ? Constants.StockTakingStatus.New.ToString() :
                                  x.STATUS == (int)Constants.StockTakingStatus.Submit ? Constants.StockTakingStatus.Submit.ToString() :
                                  x.STATUS == (int)Constants.StockTakingStatus.Adjust ? Constants.StockTakingStatus.Adjust.ToString() :
                                  x.STATUS == (int)Constants.StockTakingStatus.Approve ? Constants.StockTakingStatus.Approve.ToString() : "",
                        }).ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean Save(List<StockTakingScreen> pStockTakings, User LoginUser)
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
                        foreach (StockTakingScreen enStockTaking in pStockTakings)
                        {
                            var objStockTaking = this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.SingleOrDefault(ur => ur.STOCKTAKING_DATE == enStockTaking.StockTakingDate && 
                                                                                                    ur.LOCATION_CODE == enStockTaking.LocationID &&
                                                                                                    ur.ITEM_CODE == enStockTaking.ItemCode &&
                                                                                                    ur.HEAT_NO == enStockTaking.HeatNo);
                            if (objStockTaking != null)
                            {
                                objStockTaking.ACTUAL_QTY = enStockTaking.ActualQty;
                                objStockTaking.UPDATE_DATE = DateTime.Now;
                                objStockTaking.UPDATE_USER_ID = LoginUser.UserId;
                            }
                        }

                        flag = this.db.SaveChanges();
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

        public Boolean SubmitUpdateStatus(List<StockTakingScreen> pStockTakings, User LoginUser)
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
                        foreach (StockTakingScreen enStockTaking in pStockTakings)
                        {
                            var objStockTaking = this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.SingleOrDefault(ur => ur.STOCKTAKING_DATE == enStockTaking.StockTakingDate &&
                                                                                                    ur.LOCATION_CODE == enStockTaking.LocationID &&
                                                                                                    ur.ITEM_CODE == enStockTaking.ItemCode &&
                                                                                                    ur.HEAT_NO == enStockTaking.HeatNo);
                            if (objStockTaking != null && LoginUser != null)
                            {
                                objStockTaking.ACTUAL_QTY = enStockTaking.ActualQty;
                                objStockTaking.STATUS = (Byte)Constants.StockTakingStatus.Submit;
                                objStockTaking.UPDATE_DATE = DateTime.Now;
                                objStockTaking.UPDATE_USER_ID = LoginUser.UserId;
                            }
                        }

                        flag = this.db.SaveChanges();
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

        public Boolean UpdateStatus(List<StockTakingScreen> pStockTakings, Constants.StockTakingStatus pStatus, User LoginUser)
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
                        foreach (StockTakingScreen enStockTaking in pStockTakings)
                        {
                            var objStockTaking = this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.SingleOrDefault(ur => ur.STOCKTAKING_DATE == enStockTaking.StockTakingDate && 
                                                                                                    ur.LOCATION_CODE == enStockTaking.LocationID &&
                                                                                                    ur.ITEM_CODE == enStockTaking.ItemCode &&
                                                                                                    ur.HEAT_NO == enStockTaking.HeatNo);
                            if (objStockTaking != null && LoginUser != null)
                            {
                                objStockTaking.STATUS  = (Byte)pStatus;
                                objStockTaking.UPDATE_DATE = DateTime.Now;
                                objStockTaking.UPDATE_USER_ID = LoginUser.UserId;
                            }
                        }

                        flag = this.db.SaveChanges();
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

        public Boolean DeleteData(List<StockTakingScreen> pStockTakings)
        {
            Boolean result = false;
            Boolean isUsed = false;
            TransactionScope tran = null;
            int flag = 0;

            try
            {
                using (tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
                {
                    using (this.db)
                    {
                         foreach (StockTakingScreen enStockTaking in pStockTakings)
                        {
                            // Delete in PSC2050_T_STOCKTAKING_INSTRUCTION
                            var objStockTaking = this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.SingleOrDefault(ur => ur.STOCKTAKING_DATE == enStockTaking.StockTakingDate &&
                                                                                                    ur.LOCATION_CODE == enStockTaking.LocationID &&
                                                                                                    ur.ITEM_CODE == enStockTaking.ItemCode &&
                                                                                                    ur.HEAT_NO == enStockTaking.HeatNo);
                            if (objStockTaking != null)
                            {
                                this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.Remove(objStockTaking);
                            }

                            objStockTaking = this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.FirstOrDefault(ur => ur.STOCKTAKING_DATE != enStockTaking.StockTakingDate &&
                                                                                                    ur.ITEM_CODE == enStockTaking.ItemCode &&
                                                                                                    ur.HEAT_NO == enStockTaking.HeatNo);
                            if (objStockTaking != null)
                            {
                                isUsed = true;
                            }

                            if (!isUsed)
                            {
                                //Delete in PSC8010_M_PIPE
                                var objPipeMaster = this.db.PSC8010_M_PIPE.SingleOrDefault(ur => ur.ITEM_CODE == enStockTaking.ItemCode &&
                                                                            ur.HEAT_NO == enStockTaking.HeatNo);
                                if (objPipeMaster != null)
                                {
                                    this.db.PSC8010_M_PIPE.Remove(objPipeMaster);
                                }
                            }
                        }

                        flag = this.db.SaveChanges();
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
    }
}