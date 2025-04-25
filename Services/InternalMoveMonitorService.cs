using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class InternalMoveMonitorService
    {
        private PSCSEntities db;

        public InternalMoveMonitorService(PSCSEntities pDb)
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

        #region Method
        public List<InternalMoveMonitor> GetInternalMovementData()
        {
            List<InternalMoveMonitor> result = new List<InternalMoveMonitor>()
            {
                new InternalMoveMonitor
                {
                    RowNo = 1,
                    MoveId = 1,
                    InternalMovementDate = DateTime.Parse("2019/02/21"),
                    StartTime = DateTime.Parse("2019/02/21 08:00"),
                    EndTime = DateTime.Parse("2019/02/21 17:00"),
                    YardID = "1",
                    YardName = "Yard1"
                },
                new InternalMoveMonitor
                {
                    RowNo = 2,
                    MoveId = 2,
                    InternalMovementDate = DateTime.Parse("2019/03/01"),
                    StartTime = DateTime.Parse("2019/03/01 09:00"),
                    EndTime = DateTime.Parse("2019/03/01 12:00"),
                    YardID = "1",
                    YardName = "Yard1"
                }
            };

            return result;
        }

        public List<InternalMoveMonitor> GetInternalMovementData(DateTime? pInternalMoveDate, string pStartHours,
                                    string pStartMinute, string pFinishHours, string pFinishMinute, string pYardID)
        {
            List<InternalMoveMonitor> result = new List<InternalMoveMonitor>();
            int cnt = 0;

            try
            {
                List<InternalMoveMonitor> data = GetInternalMovementData();

                var obj = (from mo in data
                          select new
                          {
                              mo.MoveId,
                              mo.InternalMovementDate,
                              mo.StartTime,
                              mo.EndTime,
                              mo.YardID,
                              mo.YardName
                          }).AsQueryable();

                cnt = obj.Count();

                // Delivery Date
                if (pInternalMoveDate != null)
                {
                    DateTime internalMoveDate = Convert.ToDateTime(pInternalMoveDate).Date;
                    obj = obj.Where(x => x.InternalMovementDate == internalMoveDate);
                }
                cnt = obj.Count();

                // Start
                if (!string.IsNullOrEmpty(pStartHours))
                {
                    int startHours = Int32.Parse(pStartHours);
                    obj = obj.Where(x => x.StartTime.Hour == startHours);

                    if (!string.IsNullOrEmpty(pStartMinute))
                    {
                        int startMinute = Int32.Parse(pStartMinute);
                        obj = obj.Where(x => x.StartTime.Minute == startMinute);
                    }
                }
                cnt = obj.Count();

                // End
                if (!string.IsNullOrEmpty(pFinishHours))
                {
                    int finishHours = Int32.Parse(pFinishHours);
                    obj = obj.Where(x => x.EndTime.Hour == finishHours);

                    if (!string.IsNullOrEmpty(pFinishMinute))
                    {
                        int finishMinute = Int32.Parse(pFinishMinute);
                        obj = obj.Where(x => x.EndTime.Minute == finishMinute);
                    }
                }
                cnt = obj.Count();

                // Yard
                if (!string.IsNullOrEmpty(pYardID))
                {
                    obj = obj.Where(x => x.YardID == pYardID);
                }
                cnt = obj.Count();

                // Convert into location viewmodel list
                result = obj.AsEnumerable().Select((x, index) => new InternalMoveMonitor
                {
                    RowNo = index + 1,
                    InternalMovementDate = x.InternalMovementDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    YardID = x.YardID,
                    YardName = x.YardName
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return result;
        }

        public List<InternalMoveMonitorDetail> GetInternalMovementDataDetail()
        {
            List<InternalMoveMonitorDetail> result = new List<InternalMoveMonitorDetail>()
            {
                new InternalMoveMonitorDetail
                {
                    RowNo = 1,
                    MoveId = 1,
                    TranNo = 1,
                    ItemCode = "MC311E-0603011106000-59",
                    HeatNo = "O85011",
                    OD = Convert.ToDecimal(267.4) ,
                    WT = Convert.ToDecimal(9.8),
                    Length = Convert.ToDecimal(6570),
                    FromLocationName = "A14",
                    QTY = 6,
                    DestinationName = "A13",
                    Remark = "TPCO C2 WPB"
                },
                new InternalMoveMonitorDetail
                {
                    RowNo = 2,
                    MoveId = 1,
                    TranNo = 2,
                    ItemCode = "MC311E-0603011106000-59",
                    HeatNo = "O85011",
                    OD = Convert.ToDecimal(267.4),
                    WT = Convert.ToDecimal(9.8),
                    Length = Convert.ToDecimal(6280),
                    FromLocationName = "A14",
                    QTY = 1,
                    DestinationName = "A13",
                    Remark = "TPCO C2 WPB"
                },
                new InternalMoveMonitorDetail
                {
                    RowNo = 3,
                    MoveId = 2,
                    TranNo = 3,
                    ItemCode = "MC311E-0603011106000-59",
                    HeatNo = "1834769V",
                    OD = Convert.ToDecimal(219.1),
                    WT = Convert.ToDecimal(13.0),
                    Length = Convert.ToDecimal(6340),
                    FromLocationName = "A14",
                    QTY = 1,
                    DestinationName = "A13",
                    Remark = "WMST C2 WPB ย้ายไปแค่ 1"
                },
            };

            return result;
        }


        public List<InternalMoveMonitorDetail> GetInternalMovementDataDetail(string pId)
        {
            List<InternalMoveMonitorDetail> result = new List<InternalMoveMonitorDetail>();

            try
            {
                List<InternalMoveMonitorDetail> data = GetInternalMovementDataDetail();
                int _id = Int32.Parse(pId);

                var obj = (from de in data
                           where de.MoveId == _id
                           orderby de.TranNo
                           select new
                           {
                               move_id = de.MoveId,
                               tran_no = de.TranNo,
                               item_code = de.ItemCode,
                               heat_no = de.HeatNo,
                               od = de.OD,
                               wt = de.WT,
                               lt = de.Length,
                               from_location = de.FromLocationName,
                               qty = de.QTY,
                               destination_location = de.DestinationName,
                               remark = de.Remark
                           }).AsQueryable();

                result = obj.AsEnumerable().Select((x, index) => new InternalMoveMonitorDetail
                {
                    RowNo = index + 1,
                    ItemCode = x.item_code,
                    HeatNo = x.heat_no,
                    OD = x.od,
                    WT = x.wt,
                    Length = x.lt,
                    FromLocationName = x.from_location,
                    QTY = x.qty,
                    DestinationName = x.destination_location,
                    Remark = x.remark
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<InternalMoveMonitor> Search(InternalMoveMonitorScreen FilterModel)
        {
            List<InternalMoveMonitor> result = new List<InternalMoveMonitor>();

            try
            {
                result = GetInternalMovementData(FilterModel.FilterInternalMoveDate, 
                                                 FilterModel.FilterStartHours, 
                                                 FilterModel.FilterStartMinute,
                                                 FilterModel.FilterFinishHours, 
                                                 FilterModel.FilterFinishMinute,
                                                 FilterModel.FilterYardID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        #endregion
    }
}