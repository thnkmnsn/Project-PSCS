using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Services
{
    public class InternalMoveOnYardService
    {
        private PSCSEntities db;

        public InternalMoveOnYardService(PSCSEntities pDb)
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

        public List<InternalMoveOnYard> GetInternalMovementYardData()
        {
            List<InternalMoveOnYard> result = new List<InternalMoveOnYard>()
            {
                new InternalMoveOnYard { RowNo = 1, Key = 1, InternalMovementDate = DateTime.Parse("2019/02/21"), StartTime = DateTime.Parse("2019/02/21 08:30"), EndTime = DateTime.Parse("2019/02/21 17:30"),
                                             YardID = "2", YardName = "Yard2" , ItemCode = null, HeatNo = null, OD = null , WT = null, Length = null,
                                             FromLocationName = null, QTY = null, DestinationName = null, Remark = null },
            };

            return result;
        }

        public List<InternalMoveOnYard> GetInternalMovementYardDataEdit()
        {
            List<InternalMoveOnYard> result = new List<InternalMoveOnYard>()
            {
                new InternalMoveOnYard { RowNo = 1, Key = 1, InternalMovementDate = DateTime.Parse("2019/02/21"), StartTime = DateTime.Parse("2019/02/21 08:30"), EndTime = DateTime.Parse("2019/02/21 17:30"),
                                             YardID = "2", YardName = "Yard2" , ItemCode = "MC311E-0603011106000-59", HeatNo = "O85011", OD = Convert.ToDecimal(267.4) , WT = Convert.ToDecimal(9.8), Length = Convert.ToDecimal(6570),
                                             FromLocationName = "A14", QTY = 6, DestinationName = "A13", Remark = null },
                new InternalMoveOnYard { RowNo = 2, Key = 1, InternalMovementDate = DateTime.Parse("2019/02/21"), StartTime = DateTime.Parse("2019/02/21 08:30"), EndTime = DateTime.Parse("2019/02/21 17:30"),
                                             YardID = "2", YardName = "Yard2" , ItemCode = "MC311E-0603011106000-59", HeatNo = "O85011", OD = Convert.ToDecimal(267.4) , WT = Convert.ToDecimal(9.8), Length = Convert.ToDecimal(6280),
                                             FromLocationName = "A14", QTY = 1, DestinationName = "A13", Remark = null },
                new InternalMoveOnYard { RowNo = 3, Key = 1, InternalMovementDate = DateTime.Parse("2019/02/21"), StartTime = DateTime.Parse("2019/02/21 08:30"), EndTime = DateTime.Parse("2019/02/21 17:30"),
                                             YardID = "2", YardName = "Yard2" , ItemCode = "MC311E-0603011106000-59", HeatNo = "1834769V", OD = Convert.ToDecimal(219.1) , WT = Convert.ToDecimal(13.0), Length = Convert.ToDecimal(6340),
                                             FromLocationName = "A14", QTY = 1, DestinationName = "A13", Remark = null },
            };

            return result;
        }

        public List<InternalMoveOnYard> GetInternalMovementYardDataCreate()
        {
            List<InternalMoveOnYard> result = new List<InternalMoveOnYard>()
            {
                new InternalMoveOnYard { RowNo = 1, Key = 1, InternalMovementDate = null, StartTime = null, EndTime = null,
                                             YardID = null, YardName = null , ItemCode = "MC311E-0603011106000-59", HeatNo = "O85011", OD = Convert.ToDecimal(267.4) , WT = Convert.ToDecimal(9.8), Length = Convert.ToDecimal(6570),
                                             FromLocationName = "A14", QTY = 6, DestinationName = "A13", Remark = null },
                new InternalMoveOnYard { RowNo = 2, Key = 1, InternalMovementDate = null, StartTime = null, EndTime = null,
                                             YardID = null, YardName = null , ItemCode = "MC311E-0603011106000-59", HeatNo = "O85011", OD = Convert.ToDecimal(267.4) , WT = Convert.ToDecimal(9.8), Length = Convert.ToDecimal(6280),
                                             FromLocationName = "A14", QTY = 1, DestinationName = "A13", Remark = null },
                new InternalMoveOnYard { RowNo = 3, Key = 1, InternalMovementDate = null, StartTime = null, EndTime = null,
                                             YardID = null, YardName = null , ItemCode = "MC311E-0603011106000-59", HeatNo = "1834769V", OD = Convert.ToDecimal(219.1) , WT = Convert.ToDecimal(13.0), Length = Convert.ToDecimal(6340),
                                             FromLocationName = "A14", QTY = 1, DestinationName = "A13", Remark = null },
            };

            return result;
        }
    }
}