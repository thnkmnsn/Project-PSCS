using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Web;
using PSCS.Models;
using PSCS.Common;

namespace PSCS.Services
{
    public class ReceivingInstructionService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public ReceivingInstructionService(PSCSEntities pDb)
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

        public ReceivingInstruction GetReceivingInstruction(int pReceiveId)
        {
            ReceivingInstruction result =null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = db.PSC2110_T_RECEIVING_INSTRUCTION
                        .Where(m => m.RECEIVE_ID == pReceiveId)
                        .AsEnumerable().Select(r => new ReceivingInstruction
                        {
                            RecevedID = r.RECEIVE_ID,
                            ContainerNo = r.CONTAINER_NO,
                            TruckNumber = r.TRUCK_NUMBER,
                            DeliveryDate = r.DELIVERY_DATE,
                            ReceiveDate = r.RECEIVED_DATE,
                            StartTime = r.START_TIME,
                            FinishedTime = r.FINISHED_TIME,
                            Status = r.STATUS,
                        }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReceivingInstruction> GetReceivingInstructionList(DateTime deliveryDate)
        {
            List<ReceivingInstruction> result = new List<ReceivingInstruction>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = db.PSC2110_T_RECEIVING_INSTRUCTION
                        .Where(m => m.DELIVERY_DATE == deliveryDate)
                        .AsEnumerable()
                        .Select((r, index) => new ReceivingInstruction
                        {
                            RowNo = index + 1,
                            RecevedID = r.RECEIVE_ID,
                            ContainerNo =  r.CONTAINER_NO,
                            TruckNumber = r.TRUCK_NUMBER,
                            DeliveryDate = r.DELIVERY_DATE,
                            ReceiveDate = r.RECEIVED_DATE,
                            StartTime = r.START_TIME,
                            FinishedTime = r.FINISHED_TIME,
                            Status = r.STATUS,
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<ReceivingInstruction> GetReceivingInstructionList(DateTime deliveryDate, string ItemCode, string HeatNo)
        {
            List<ReceivingInstruction> result = new List<ReceivingInstruction>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    
                    result = (from r in db.PSC2110_T_RECEIVING_INSTRUCTION
                               join rd in db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL on r.RECEIVE_ID equals rd.RECEIVE_ID
                              where r.DELIVERY_DATE.Value.Day == deliveryDate.Day 
                              && r.DELIVERY_DATE.Value.Month == deliveryDate.Month
                              && r.DELIVERY_DATE.Value.Year == deliveryDate.Year
                              && rd.ITEM_CODE == ItemCode && rd.HEAT_NO == HeatNo
                              select new ReceivingInstruction
                               {
                                   RowNo = 0,
                                   RecevedID = r.RECEIVE_ID,
                                   ContainerNo = r.CONTAINER_NO,
                                   TruckNumber = r.TRUCK_NUMBER,
                                   DeliveryDate = r.DELIVERY_DATE,
                                   ReceiveDate = r.RECEIVED_DATE,
                                   StartTime = r.START_TIME,
                                   FinishedTime = r.FINISHED_TIME,
                                   Status = r.STATUS,
                               }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }



        public List<ReceivingInstruction> Search(DateTime deliveryDate)
        {
            List<ReceivingInstruction> result = new List<ReceivingInstruction>();

            try
            {
                result = GetReceivingInstructionList(deliveryDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        
        public bool Insert(ReceivingInstruction pData)
        {
            try
            {
                using (this.db)
                {
                    this.db.Configuration.LazyLoadingEnabled = false;
                    DateTime insertDate = DateTime.Now;

                    var obj = this.db.PSC2110_T_RECEIVING_INSTRUCTION.SingleOrDefault(x => x.DELIVERY_DATE == pData.DeliveryDate && x.CONTAINER_NO == pData.ContainerNo);

                    // Check duplicate data before insert
                    if (obj == null)
                    {
                        PSC2110_T_RECEIVING_INSTRUCTION insert = new PSC2110_T_RECEIVING_INSTRUCTION();

                        insert.DELIVERY_DATE = pData.DeliveryDate;
                        insert.RECEIVED_DATE = pData.ReceiveDate;
                        insert.CONTAINER_NO = pData.ContainerNo;
                        insert.STATUS = pData.Status;
                        insert.CREATE_DATE = pData.CreateDate;
                        insert.CREATE_USER_ID = pData.CreateUserID;
                        insert.UPDATE_DATE = pData.UpdateDate;
                        insert.UPDATE_USER_ID = pData.UpdateUserID;

                        this.db.PSC2110_T_RECEIVING_INSTRUCTION.Add(insert);
                    }

                    int result = this.db.SaveChanges();

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            //{
            //    try
            //    {
            //        using (this.db)
            //        {
            //            this.db.Configuration.LazyLoadingEnabled = false;
            //            DateTime insertDate = DateTime.Now;

            //            var obj = this.db.PSC2110_T_RECEIVING_INSTRUCTION.SingleOrDefault(x => x.DELIVERY_DATE == pData.DeliveryDate && x.CONTAINER_NO == pData.ContainerNo);

            //            // Check duplicate data before insert
            //            if (obj == null)
            //            {
            //                PSC2110_T_RECEIVING_INSTRUCTION insert = new PSC2110_T_RECEIVING_INSTRUCTION();

            //                insert.DELIVERY_DATE = pData.DeliveryDate;
            //                insert.RECEIVED_DATE = pData.ReceiveDate;
            //                insert.CONTAINER_NO = pData.ContainerNo;
            //                insert.STATUS = pData.Status;
            //                insert.CREATE_DATE = pData.CreateDate;
            //                insert.CREATE_USER_ID = pData.CreateUserID;
            //                insert.UPDATE_DATE = pData.UpdateDate;
            //                insert.UPDATE_USER_ID = pData.UpdateUserID;

            //                this.db.PSC2110_T_RECEIVING_INSTRUCTION.Add(insert);
            //            }

            //            int result = this.db.SaveChanges();

            //            if (result > 0)
            //            {
            //                tran.Complete();
            //                return true;
            //            }
            //            else
            //            {
            //                tran.Dispose();
            //                return false;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        tran.Dispose();
            //        throw ex;
            //    }
            //}
        }

        public Boolean UpdateReceiveData(ReceivingInstruction pReceive, string pUserId)
        {
            Boolean result = false;

            try
            {
                if(pReceive != null)
                {
                    var obj = this.db.PSC2110_T_RECEIVING_INSTRUCTION.SingleOrDefault(x => x.RECEIVE_ID == pReceive.RecevedID);

                    if (obj != null)
                    {
                        obj.RECEIVED_DATE = pReceive.ReceiveDate;
                        obj.TRUCK_NUMBER = pReceive.TruckNumber;
                        obj.STATUS = pReceive.Status;
                        obj.UPDATE_USER_ID = pUserId;
                        obj.UPDATE_DATE = DateTime.Now;
                    }

                    int intSaveChanges = this.db.SaveChanges();

                    if (intSaveChanges > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result =  false;
                    }
                }
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