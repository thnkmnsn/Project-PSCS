using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;

namespace PSCS.Services
{
    public class StockCheckMonitorService
    {
        private PSCSEntities db;

        public StockCheckMonitorService(PSCSEntities pDb)
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

        public List<StockCheckMonitor> GetStockCheckMonitorList()
        {
            return new List<StockCheckMonitor>()
            {
                new StockCheckMonitor(){ RowNo = 1, StockCheckDate = DateTime.Today, Yard = "2", YardName = "Yard 2", Location = "2A01", LocationName = "A1", Status = "Approve"},
                new StockCheckMonitor(){ RowNo = 2, StockCheckDate = DateTime.Today, Yard = "1", YardName = "Yard 1", Location = "1A01", LocationName = "A1", Status = "Approve"},
                new StockCheckMonitor(){ RowNo = 3, StockCheckDate = DateTime.Today, Yard = "4", YardName = "Yard 4", Location = "4E07", LocationName = "CNE7", Status = "New"},
                new StockCheckMonitor(){ RowNo = 4, StockCheckDate = DateTime.Today, Yard = "1", YardName = "Yard 1", Location = "1A10", LocationName = "A10", Status = "New"}

            };
        }

        public List<StockCheckMonitorDetail> GetStockCheckMonitorDetailList()
        {
            return new List<StockCheckMonitorDetail>()
            {
                new StockCheckMonitorDetail(){ RowNo = 1, StockCheckDate = DateTime.Today, Yard = "1", YardName = "Yard 1", Location = "1A01", LocationName = "A1",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400 , Qty = 10, ActualQty = 0, Remark ="", Status = "Approve"},
                new StockCheckMonitorDetail(){ RowNo = 2, StockCheckDate = DateTime.Today, Yard = "1", YardName = "Yard 1", Location = "1B02", LocationName = "B2",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 11, ActualQty = 0, Remark ="", Status = "New" },
                new StockCheckMonitorDetail(){ RowNo = 3, StockCheckDate = DateTime.Today, Yard = "2", YardName = "Yard 2", Location = "2A01", LocationName = "A2",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 12, ActualQty = 0, Remark ="", Status = "Approve" },
                new StockCheckMonitorDetail(){ RowNo = 4, StockCheckDate = DateTime.Today, Yard = "2", YardName = "Yard 2", Location = "2B03", LocationName = "B3",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 13, ActualQty = 0, Remark ="", Status = "New"},
                new StockCheckMonitorDetail(){ RowNo = 5, StockCheckDate = DateTime.Today, Yard = "2", YardName = "Yard 2", Location = "2C04", LocationName = "C4",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 14, ActualQty = 0, Remark ="", Status = "New"},
                new StockCheckMonitorDetail(){ RowNo = 6, StockCheckDate = DateTime.Today, Yard = "4", YardName = "Yard 4", Location = "4A03", LocationName = "CNA3",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 15, ActualQty = 0, Remark ="", Status = "New" },
                new StockCheckMonitorDetail(){ RowNo = 7, StockCheckDate = DateTime.Today, Yard = "4", YardName = "Yard 4", Location = "4B04", LocationName = "CNB4",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 16, ActualQty = 0, Remark ="", Status = "New" },
                new StockCheckMonitorDetail(){ RowNo = 8, StockCheckDate = DateTime.Today, Yard = "4", YardName = "Yard 4", Location = "4C05", LocationName = "CNC5",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 17, ActualQty = 0, Remark ="", Status = "New" },
                new StockCheckMonitorDetail(){ RowNo = 9, StockCheckDate = DateTime.Today, Yard = "4", YardName = "Yard 4", Location = "4D06", LocationName = "CND6",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 18, ActualQty = 0, Remark ="", Status = "New" },
                new StockCheckMonitorDetail(){ RowNo = 10, StockCheckDate = DateTime.Today, Yard = "4", YardName = "Yard 4", Location = "4E07", LocationName = "CNE7",ItemCode = "MC200E-2163007206400-80",HeatNo = "E6599",OD = Convert.ToDecimal(216.3),WT = Convert.ToDecimal(7.2),Length = 6400, Qty = 19, ActualQty = 0, Remark ="", Status = "New" }
            };
        }
    }
}