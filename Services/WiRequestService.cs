using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PSCS.Services
{
    public class WiRequestService
    {
        private PSCSEntities db;

        public WiRequestService(PSCSEntities pDb)
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

        //public List<WiRequest> GetWiRequestData()
        //{
        //    List<WiRequest> result = new List<WiRequest>()
        //    {
        //        new WiRequest { ReqestId = 1, JobNo = "T000002331", MFGNo = "925M45", HeatNo = "Tesrlot",
        //                        Description = "48.6*4*6090 ERW 2 SGP L SUNSCO", Qty = (decimal) 1.0, Remain = (decimal) 16.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 2, JobNo = "T000002332", MFGNo = "925M46", HeatNo = "152231",
        //                        Description = "48.6*4*6090 ERW 2 SGP L SUNSCO", Qty = (decimal) 1.2, Remain = (decimal) 0.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 3, JobNo = "T000002333", MFGNo = "925M46", HeatNo = "152231",
        //                        Description = "48.6*4*6090 ERW 2 SGP L SUNSCO", Qty = (decimal) 0.8, Remain = (decimal) 0.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 4, JobNo = "T000002334", MFGNo = "925M47", HeatNo = "619231", 
        //                        Description = "48.6*4*6050 ERW 2 SGP L SUNSCO", Qty = (decimal) 10.0, Remain = (decimal) 21.7, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 5, JobNo = "T000002335", MFGNo = "925M49", HeatNo = "999999",
        //                        Description = "48.6*4*7090 ERW 2 SGP L SUNSCO", Qty = (decimal) 1.0, Remain = (decimal) 1.0, PipeReqNo = "" },
        //    };

        //    return result;
        //}

        //public List<WiRequest> GetWiRequestTest()
        //{
        //    List<WiRequest> result = new List<WiRequest>()
        //    {
        //        new WiRequest { ReqestId = 1, JobNo = "1", MFGNo = "925M45", HeatNo = "Tesrlot",
        //                        Description = "48.6*4*6090 ERW 2 SGP L SUNSCO", Qty = (decimal) 1.0, Remain = (decimal) 16.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 2, JobNo = "2", MFGNo = "925M46", HeatNo = "152231",
        //                        Description = "48.6*4*6090 ERW 2 SGP L SUNSCO", Qty = (decimal) 1.2, Remain = (decimal) 0.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 1, JobNo = "3", MFGNo = "925M46", HeatNo = "152231",
        //                        Description = "48.6*4*6090 ERW 2 SGP L SUNSCO", Qty = (decimal) 2.8, Remain = (decimal) 10.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 1, JobNo = "4", MFGNo = "925M47", HeatNo = "619231",
        //                        Description = "48.6*4*6050 ERW 2 SGP L SUNSCO", Qty = (decimal) 5.0, Remain = (decimal) 5.0, PipeReqNo = "" },
        //        new WiRequest { ReqestId = 1, JobNo = "5", MFGNo = "925M49", HeatNo = "999999",
        //                        Description = "48.6*4*7090 ERW 2 SGP L SUNSCO", Qty = (decimal) 10.0, Remain = (decimal) 25.0, PipeReqNo = "" },
        //    };

        //    return result;
        //}

        //public List<Release> SearchIntitial()
        //{

        //    List<Release> result = new List<Release>();
        //    var date = DateTime.Now;
        //    var query = (from rd in db.PSC2411_T_RELEASE_DETAIL
        //                 join r in db.PSC2410_T_RELEASE on rd.JOB_NO equals r.JOB_NO
        //                 join mp in db.PSC8010_M_PIPE_ITEM on new { r.ITEM_CODE, r.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
        //                 join m in db.PSC8027_M_MAKER on mp.MAKER equals m.MAKER
        //                 where rd.STATUS == 1
        //                 select new
        //                 {
        //                     rd.RELEASE_ID,
        //                     rd.RELEASE_DATE,
        //                     r.JOB_NO,
        //                     r.MFG_NO,
        //                     r.ITEM_CODE,
        //                     r.HEAT_NO,
        //                     mp.DESCRIPTION,
        //                     mp.MAKER,
        //                     m.MAKER_NAME,
        //                     rd.RELEASE_QTY,
        //                     rd.REQUEST_QTY,
        //                     rd.REMAIN_QTY,
        //                     rd.STATUS
        //                 }).ToList();

        //    for(int i = 0; i < query.Count; i++) {
        //        result.Add(
        //            new Release()
        //                {
        //                    RowNo =i+1,
        //                    ReleaseID = Convert.ToInt32(query[i].RELEASE_ID),
        //                    JobNo = query[i].JOB_NO,
        //                    ReleaseDate = query[i].RELEASE_DATE,
        //                    MfgNo = query[i].MFG_NO,
        //                    ItemCode = query[i].ITEM_CODE,
        //                    HeatNo = query[i].HEAT_NO,
        //                    Description = query[i].DESCRIPTION,
        //                    Maker = query[i].MAKER,
        //                    Maker_Name = query[i].MAKER_NAME,
        //                    RequestQTY = Math.Round(Convert.ToDecimal(query[i].REQUEST_QTY), 2),
        //                    QTY = Math.Round(Convert.ToDecimal(query[i].RELEASE_QTY), 2),
        //                    RemainQTY = Math.Round(Convert.ToDecimal(query[i].REMAIN_QTY), 2), 
        //                    Status = query[i].STATUS,
        //                });
        //    }
            

        //    return result;
        //}


        //public List<Release> SearchIntitialByCondition(DateTime? pDate)
        //{

        //    List<Release> result = new List<Release>();
        //    var date = DateTime.Now;
        //    var query = (from rd in db.PSC2411_T_RELEASE_DETAIL
        //                 join r in db.PSC2410_T_RELEASE on rd.JOB_NO equals r.JOB_NO
        //                 join mp in db.PSC8010_M_PIPE_ITEM on new { r.ITEM_CODE, r.HEAT_NO } equals new { mp.ITEM_CODE, mp.HEAT_NO }
        //                 join m in db.PSC8027_M_MAKER on mp.MAKER equals m.MAKER
        //                 //where date = today
        //                 select new
        //                 {
        //                     rd.RELEASE_ID,
        //                     rd.RELEASE_DATE,
        //                     r.JOB_NO,
        //                     r.MFG_NO,
        //                     r.ITEM_CODE,
        //                     r.HEAT_NO,
        //                     mp.DESCRIPTION,
        //                     mp.MAKER,
        //                     m.MAKER_NAME,
        //                     rd.RELEASE_QTY,
        //                     rd.REQUEST_QTY,
        //                     rd.REMAIN_QTY,
        //                     rd.STATUS
        //                 }).AsQueryable();

        //    //Move Date
        //    if (pDate != null)
        //    {
        //        DateTime releaseDate = Convert.ToDateTime(pDate).Date;
        //        //query = query.Where(x => x.RELEASE_DATE == releaseDate);
        //        query = query.Where(x => x.RELEASE_DATE.Value.Year == releaseDate.Year 
        //                                 && x.RELEASE_DATE.Value.Month == releaseDate.Month 
        //                                 && x.RELEASE_DATE.Value.Day == releaseDate.Day);
        //    }

        //    result = query.AsEnumerable()
        //                 .Select((x, index) => new Release
        //                 {
        //                     RowNo = index + 1,
        //                     ReleaseID = Convert.ToInt32(x.RELEASE_ID),
        //                     JobNo = x.JOB_NO,
        //                     ReleaseDate = x.RELEASE_DATE,
        //                     MfgNo = x.MFG_NO,
        //                     ItemCode = x.ITEM_CODE,
        //                     HeatNo = x.HEAT_NO,
        //                     Description = x.DESCRIPTION,
        //                     Maker = x.MAKER,
        //                     Maker_Name = x.MAKER_NAME,
        //                     RequestQTY = Convert.ToDecimal(x.REQUEST_QTY),
        //                     QTY = x.RELEASE_QTY,
        //                     RemainQTY = x.REMAIN_QTY,
        //                     Status = x.STATUS
        //                 }).ToList();

        //    //for (int i = 0; i < query.Count; i++)
        //    //{
        //    //    result.Add(
        //    //        new Release()
        //    //        {
        //    //            RowNo = i + 1,
        //    //            ReleaseID = Convert.ToInt32(query[i].RELEASE_ID),
        //    //            JobNo = query[i].JOB_NO,
        //    //            ReleaseDate = query[i].RELEASE_DATE,
        //    //            MfgNo = query[i].MFG_NO,
        //    //            ItemCode = query[i].ITEM_CODE,
        //    //            HeatNo = query[i].HEAT_NO,
        //    //            Description = query[i].DESCRIPTION,
        //    //            Maker = query[i].MAKER,
        //    //            Maker_Name = query[i].MAKER_NAME,
        //    //            RequestQTY = Convert.ToDecimal(query[i].REQUEST_QTY),
        //    //            QTY = query[i].RELEASE_QTY,
        //    //            RemainQTY = query[i].REMAIN_QTY,
        //    //            Status = query[i].STATUS,
        //    //        });
        //    //}


        //    return result;
        //}

        public List<Release> AddIntoGrid(List<Release> pGridList, Release pAddList)
        {
            List<Release> result = new List<Release>();
            try
            {
                List<Release> objAddListTemp = new List<Release>();
                objAddListTemp.Add(pAddList);
                pGridList.AddRange(objAddListTemp);

                result = pGridList.AsEnumerable()
                        .Select((x, index) => new Release
                        {
                            RowNo = index + 1,
                            ReleaseDate = DateTime.Now,
                            ItemCode = x.ItemCode,
                            JobNo = x.JobNo,
                            MfgNo = x.MfgNo,
                            HeatNo = x.HeatNo,
                            Description = x.Description,
                            Maker = x.Maker,
                            Maker_Name = x.Maker_Name,
                            QTY = x.QTY,
                            PresentQty = x.RequestQTY != null ? Math.Round(Convert.ToDecimal(x.RequestQTY), 2): Decimal.Parse(x.QTY.ToString("0.00")),
                            RequestQTY = x.RequestQTY!=null ? Math.Round(Convert.ToDecimal(x.RequestQTY), 2): Decimal.Parse(x.QTY.ToString("0.00")),
                            RemainQTY = x.RemainQTY !=null ? Math.Round(Convert.ToDecimal(x.RemainQTY), 2) : 0,
                            ReleaseID = x.ReleaseID
                        }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Release> RemoveFromGrid(List<Release> pGridList, string rowNo)
        {
            List<Release> result = new List<Release>();
            try
            {
                int row = Int32.Parse(rowNo);
                var item = pGridList.Where(x => x.RowNo == row).FirstOrDefault();
                pGridList.Remove(item);

                result = pGridList.AsEnumerable()
                       .Select((x, index) => new Release
                       {
                           RowNo = index + 1,
                           RequestDate = DateTime.Now,
                           ItemCode = x.ItemCode,
                           JobNo = x.JobNo,
                           MfgNo = x.MfgNo,
                           HeatNo = x.HeatNo,
                           Description = x.Description,
                           Maker = x.Maker,
                           ReleaseDate = x.ReleaseDate,
                           Maker_Name = x.Maker_Name,
                           RequestQTY = Decimal.Parse(x.QTY.ToString("0.00")),
                           RemainQTY = 0,
                       }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<Release> ChangeWiRequest(List<Release> pGridList, List<WiEdit> wiEditsList)
        {
          
            try
            {
                for (int i = 0; i < pGridList.Count; i++)
                {
                    if (pGridList[i].JobNo == wiEditsList[i].JobNo && pGridList[i].RequestQTY != wiEditsList[i].QTY) {
                        pGridList[i].RequestQTY = wiEditsList[i].QTY;
                        pGridList[i].ReleaseQTY = wiEditsList[i].ReleaseQTY;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pGridList;
        }

        public List<Release> WiSumQty(List<WiSum> wiSumList)
        {
            var result = new List<Release>();
            try
            {
                for (int i = 0; i < wiSumList.Count; i++)
                {

                    result.Add(
                        new Release()
                        {
                            RowNo = i + 1,
                            ReleaseDate = wiSumList[i].ReleaseDate,
                            HeatNo = wiSumList[i].HeatNo,
                            Description = wiSumList[i].Description,
                            Maker = wiSumList[i].Maker,
                            RequestQTY = wiSumList[i].QTY

                        });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Release> AddIntoConfirmGrid(List<Release> pGridList)
        {
            List<Release> result = new List<Release>();
            try
            {
                var obj = new List<Release>();
                foreach (var grid in pGridList)
                {
                    if (obj.FirstOrDefault(x => x.MfgNo == grid.MfgNo && x.HeatNo == grid.HeatNo) != null)
                    {
                        obj.FirstOrDefault(x => x.MfgNo == grid.MfgNo && x.HeatNo == grid.HeatNo).QTY += grid.QTY;
                    }
                    else
                    {
                        obj.Add(grid);
                    }
                }

                result = obj.AsEnumerable().Select((x, index) => new Release
                {
                    RowNo = index + 1,
                    HeatNo = x.HeatNo,
                    RequestQTY = (decimal) x.QTY,
                }).ToList();

                //result = obj.AsEnumerable().Select((x, index) => new WiRequest
                //{
                //    RowNo = index + 1,
                //    RequestDate = DateTime.Now,
                //    Description = x.Description,
                //    HeatNo = x.HeatNo,
                //    Qty = (decimal)x.Qty,
                //    PipeReqNo = x.PipeReqNo
                //}).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public bool CheckQtyIsInteger(List<Release> pGridList)
        {
            bool notInt = false;

            foreach (var item in pGridList)
            {
                string[] qty = ((double) item.QTY).ToString("0.00").Split('.');
                int qtyAfterPoint = int.Parse(qty[1]);

                if (qtyAfterPoint > 0)
                {
                    notInt = true;
                    break;
                }
            }


            return notInt;
        }




        //public bool SaveWi(List<Release> editModel,string LoginUser)
        //{
        //  //  using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
        //    {
        //        try
        //        {
        //            using (this.db)
        //            {
        //                this.db.Configuration.LazyLoadingEnabled = false;
        //                DateTime insertDate = DateTime.Now;
        //                int? intTransNo = this.db.PSC2411_T_RELEASE_DETAIL.Max(pi => (int?)pi.RELEASE_ID);
        //                int intNewTransNo = 0;

        //                for (int i=0;i<editModel.Count;i++)
        //                {
        //                    if (editModel[i].ReleaseID == 0)
        //                    {
        //                        intNewTransNo = intNewTransNo == 0 ? (Convert.ToInt32(intTransNo == null ? 1 : intTransNo + 1)) : intNewTransNo + 1;

        //                        PSC2411_T_RELEASE_DETAIL insert = new PSC2411_T_RELEASE_DETAIL();
        //                        insert.RELEASE_ID = intNewTransNo;
        //                        insert.JOB_NO = editModel[i].JobNo;
        //                        insert.RELEASE_DATE = insertDate;
        //                        insert.RELEASE_QTY = editModel[i].QTY;
        //                        insert.REQUEST_QTY = editModel[i].RequestQTY;
        //                        insert.REMAIN_QTY = editModel[i].PresentQty- editModel[i].RequestQTY;
        //                        insert.ACTUAL_QTY = 0;
        //                        insert.STATUS = 1;
        //                        insert.CREATE_USER_ID = LoginUser;
        //                        insert.CREATE_DATE = insertDate;
        //                        insert.UPDATE_USER_ID = LoginUser;
        //                        insert.UPDATE_DATE = insertDate;
        //                        this.db.PSC2411_T_RELEASE_DETAIL.Add(insert);
        //                    }
        //                }

        //                int result = this.db.SaveChanges();

        //                if (result > 0)
        //                {
        //                  //  tran.Complete();
        //                    return true;
        //                }
        //                else
        //                {
        //                  // tran.Dispose();
        //                    return false;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //           // tran.Dispose();
        //            throw ex;
        //        }
        //    }
        //}

    }
}