using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Common;
using System.Transactions;
using PSCS.Models;
using PSCS.ModelERPDEV01;

namespace PSCS.Services
{
    public class ReceivingNewPipeService
    {
        // Attribute 
        private PSCSEntities db;
        private string UserId { get; set; }

        // Constructor 
        public ReceivingNewPipeService(PSCSEntities pDb, string pUserId)
        {
            try
            {
                this.db = pDb;
                this.UserId = pUserId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean ReceivingNewPipe(List<PipeItemData> pPipeItemDataList, List<PipeReceivedData> pPipeReceivedDataList)
        {
            Boolean result = false;

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime insertDate = DateTime.Now;

                        if (pPipeItemDataList != null)
                        {
                            List<Material> MateialList = new List<Material>();
                            List<Standard> StandardList = new List<Standard>();
                            List<Grade> GradeList = new List<Grade>();
                            List<Shape> ShapeList = new List<Shape>();
                            List<Maker> MakerList = new List<Maker>();

                            MateialList = this.db.PSC8023_M_MATERIAL.Select(p => new Material
                            {
                                MaterialCode = p.MATERIAL,
                                MaterialName = p.MATERIAL_NAME,
                                CreateUserID = p.CREATE_USER_ID,
                                UpdateUserID = p.UPDATE_USER_ID,
                                CreateDate = p.CREATE_DATE,
                                UpdateDate = p.UPDATE_DATE,
                            }).ToList();
                            StandardList = this.db.PSC8024_M_STANDARD.Select(p => new Standard
                            {
                                StandardCode = p.STANDARD,
                                StandardName = p.STANDARD_NAME,
                                CreateUserID = p.CREATE_USER_ID,
                                UpdateUserID = p.UPDATE_USER_ID,
                                CreateDate = p.CREATE_DATE,
                                UpdateDate = p.UPDATE_DATE,
                            }).ToList();
                            GradeList = this.db.PSC8025_M_GRADE.Select(p => new Grade
                            {
                                GradeCode = p.GRADE,
                                GradeName = p.GRADE_NAME,
                                CreateUserID = p.CREATE_USER_ID,
                                UpdateUserID = p.UPDATE_USER_ID,
                                CreateDate = p.CREATE_DATE,
                                UpdateDate = p.UPDATE_DATE,
                            }).ToList();
                            ShapeList = this.db.PSC8026_M_SHAPE.Select(p => new Shape
                            {
                                ShapeCode = p.SHAPE,
                                ShapeName = p.SHAPE_NAME,
                                CreateUserID = p.CREATE_USER_ID,
                                UpdateUserID = p.UPDATE_USER_ID,
                                CreateDate = p.CREATE_DATE,
                                UpdateDate = p.UPDATE_DATE,
                            }).ToList();
                            MakerList = this.db.PSC8027_M_MAKER.Select(p => new Maker
                            {
                                MakerCode = p.MAKER,
                                MakerName = p.MAKER_NAME,
                                CreateUserID = p.CREATE_USER_ID,
                                UpdateUserID = p.UPDATE_USER_ID,
                                CreateDate = p.CREATE_DATE,
                                UpdateDate = p.UPDATE_DATE,
                            }).ToList();

                            int? intTransNo = db.PSC3200_T_SYTELINE_PIPE_ITEM.Max(pi => (int?)pi.TRANS_NO);
                            int intNewTransNo = 0;
                            foreach (PipeItemData en in pPipeItemDataList)
                            {
                                intNewTransNo = intNewTransNo == 0 ? (Convert.ToInt32(intTransNo == null ? 1 : intTransNo + 1)) : intNewTransNo + 1;
                                PSC3200_T_SYTELINE_PIPE_ITEM insert = new PSC3200_T_SYTELINE_PIPE_ITEM();

                                insert.TRANS_NO = intNewTransNo;
                                insert.ITEM_CODE = en.ItemCode;
                                insert.HEAT_NO = en.HeatNo;
                                insert.DESCRIPTION = en.Description;
                                insert.OD = en.OD;
                                insert.WT = en.WT;
                                insert.LT = en.LT;
                                insert.UNIT_WEIGHT = en.UnitWeight;
                                insert.MATERIAL = en.Material;
                                insert.MATERIAL_NAME = en.MaterialName;
                                insert.STANDARD = en.standard;
                                insert.STANDARD_NAME = en.standardName;
                                insert.GRADE = en.Grade;
                                insert.GRADE_NAME = en.GradeName;
                                insert.SHAPE = en.Shape;
                                insert.SHAPE_NAME = en.ShapeName;
                                insert.MAKER = en.Maker;
                                insert.MAKER_NAME = en.MakerName;
                                insert.LABEL1 = en.Label1;
                                insert.ATTRIBUTE1 = en.Attribute1;
                                insert.LABEL2 = en.Label2;
                                insert.ATTRIBUTE2 = en.Attribute2;
                                insert.LABEL3 = en.Label3;
                                insert.ATTRIBUTE3 = en.Attribute3;
                                insert.LABEL4 = en.Label4;
                                insert.ATTRIBUTE4 = en.Attribute4;
                                insert.LABEL5 = en.Label5;
                                insert.ATTRIBUTE5 = en.Attribute5;
                                insert.LABEL6 = en.Label6;
                                insert.ATTRIBUTE6 = en.Attribute6;
                                insert.LABEL7 = en.Label7;
                                insert.ATTRIBUTE7 = en.Attribute7;
                                insert.LABEL8 = en.Label8;
                                insert.ATTRIBUTE8 = en.Attribute8;
                                insert.LABEL9 = en.Label9;
                                insert.ATTRIBUTE9 = en.Attribute9;
                                insert.LABEL10 = en.Label10;
                                insert.ATTRIBUTE10 = en.Attribute10;
                                insert.CREATE_USER_ID = this.UserId;
                                insert.CREATE_DATE = DateTime.Now;
                                insert.UPDATE_USER_ID = this.UserId;
                                insert.UPDATE_DATE = DateTime.Now;

                                this.db.PSC3200_T_SYTELINE_PIPE_ITEM.Add(insert);

                                var objUpdate = this.db.PSC8010_M_PIPE_ITEM.SingleOrDefault(pi => pi.ITEM_CODE == en.ItemCode && pi.HEAT_NO == en.HeatNo);

                                // Check duplicate data before insert
                                if (objUpdate == null)
                                {
                                    PSC8010_M_PIPE_ITEM insertItem = new PSC8010_M_PIPE_ITEM();

                                    insertItem.ITEM_CODE = en.ItemCode;
                                    insertItem.HEAT_NO = en.HeatNo;
                                    insertItem.DESCRIPTION = en.Description;
                                    insertItem.OD = Convert.ToDecimal(en.OD);
                                    insertItem.WT = Convert.ToDecimal(en.WT);
                                    insertItem.LT = Convert.ToDecimal(en.LT);
                                    insertItem.UNIT_WEIGHT = en.UnitWeight;
                                    insertItem.MATERIAL = en.Material;
                                    insertItem.MATERIAL_NAME = en.MaterialName;
                                    insertItem.STANDARD = en.standard;
                                    insertItem.STANDARD_NAME = en.standardName;
                                    insertItem.GRADE = en.Grade;
                                    insertItem.GRADE_NAME = en.GradeName;
                                    insertItem.SHAPE = en.Shape;
                                    insertItem.SHAPE_NAME = en.ShapeName;
                                    insertItem.MAKER = en.Maker;
                                    insertItem.MAKER_NAME = en.MakerName;
                                    insertItem.LABEL1 = en.Label1;
                                    insertItem.ATTRIBUTE1 = en.Attribute1;
                                    insertItem.LABEL2 = en.Label2;
                                    insertItem.ATTRIBUTE2 = en.Attribute2;
                                    insertItem.LABEL3 = en.Label3;
                                    insertItem.ATTRIBUTE3 = en.Attribute3;
                                    insertItem.LABEL4 = en.Label4;
                                    insertItem.ATTRIBUTE4 = en.Attribute4;
                                    insertItem.LABEL5 = en.Label5;
                                    insertItem.ATTRIBUTE5 = en.Attribute5;
                                    insertItem.LABEL6 = en.Label6;
                                    insertItem.ATTRIBUTE6 = en.Attribute6;
                                    insertItem.LABEL7 = en.Label7;
                                    insertItem.ATTRIBUTE7 = en.Attribute7;
                                    insertItem.LABEL8 = en.Label8;
                                    insertItem.ATTRIBUTE8 = en.Attribute8;
                                    insertItem.LABEL9 = en.Label9;
                                    insertItem.ATTRIBUTE9 = en.Attribute9;
                                    insertItem.LABEL10 = en.Label10;
                                    insertItem.ATTRIBUTE10 = en.Attribute10;
                                    insertItem.CREATE_USER_ID = this.UserId;
                                    insertItem.CREATE_DATE = DateTime.Now;
                                    insertItem.UPDATE_USER_ID = this.UserId;
                                    insertItem.UPDATE_DATE = DateTime.Now;

                                    Boolean IsFound = false;

                                    if(MateialList != null)
                                    {
                                        IsFound = false;
                                        foreach (Material MaterialEn in MateialList)
                                        {
                                            if(MaterialEn.MaterialCode == en.Material)
                                            {
                                                IsFound = true;
                                                break;
                                            }
                                        }
                                        if(IsFound == false)
                                        {
                                            Material objMat = new Material();
                                            objMat.MaterialCode = en.Material;
                                            objMat.MaterialName = en.MaterialName;
                                            MateialList.Add(objMat);

                                            PSC8023_M_MATERIAL insertMat = new PSC8023_M_MATERIAL();
                                            insertMat.MATERIAL = objMat.MaterialCode;
                                            insertMat.MATERIAL_NAME = objMat.MaterialName;
                                            insertMat.CREATE_USER_ID = this.UserId;
                                            insertMat.CREATE_DATE = DateTime.Now;
                                            insertMat.UPDATE_USER_ID = this.UserId;
                                            insertMat.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8023_M_MATERIAL.Add(insertMat);
                                        }
                                    }
                                    if (StandardList != null)
                                    {
                                        IsFound = false;
                                        foreach (Standard StandardEn in StandardList)
                                        {
                                            if (StandardEn.StandardCode == en.standard)
                                            {
                                                IsFound = true;
                                                break;
                                            }
                                        }
                                        if (IsFound == false)
                                        {
                                            Standard objStd = new Standard();
                                            objStd.StandardCode = en.standard;
                                            objStd.StandardName = en.standardName;
                                            StandardList.Add(objStd);

                                            PSC8024_M_STANDARD insertStd = new PSC8024_M_STANDARD();
                                            insertStd.STANDARD = objStd.StandardCode;
                                            insertStd.STANDARD_NAME = objStd.StandardName;
                                            insertStd.CREATE_USER_ID = this.UserId;
                                            insertStd.CREATE_DATE = DateTime.Now;
                                            insertStd.UPDATE_USER_ID = this.UserId;
                                            insertStd.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8024_M_STANDARD.Add(insertStd);
                                        }
                                    }
                                    if (GradeList != null)
                                    {
                                        IsFound = false;
                                        foreach (Grade GradeEn in GradeList)
                                        {
                                            if (GradeEn.GradeCode == en.Grade)
                                            {
                                                IsFound = true;
                                                break;
                                            }
                                        }
                                        if (IsFound == false)
                                        {
                                            Grade objGrade = new Grade();
                                            objGrade.GradeCode = en.Grade;
                                            objGrade.GradeName = en.GradeName;
                                            GradeList.Add(objGrade);

                                            PSC8025_M_GRADE insertGrade = new PSC8025_M_GRADE();
                                            insertGrade.GRADE = objGrade.GradeCode;
                                            insertGrade.GRADE_NAME = objGrade.GradeName;
                                            insertGrade.CREATE_USER_ID = this.UserId;
                                            insertGrade.CREATE_DATE = DateTime.Now;
                                            insertGrade.UPDATE_USER_ID = this.UserId;
                                            insertGrade.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8025_M_GRADE.Add(insertGrade);
                                        }
                                    }
                                    if (ShapeList != null)
                                    {
                                        IsFound = false;
                                        foreach (Shape ShapeEn in ShapeList)
                                        {
                                            if (ShapeEn.ShapeCode == en.Shape)
                                            {
                                                IsFound = true;
                                                break;
                                            }
                                        }
                                        if (IsFound == false)
                                        {
                                            Shape objShape = new Shape();
                                            objShape.ShapeCode = en.Shape;
                                            objShape.ShapeName = en.ShapeName;
                                            ShapeList.Add(objShape);

                                            PSC8026_M_SHAPE insertShape = new PSC8026_M_SHAPE();
                                            insertShape.SHAPE = objShape.ShapeCode;
                                            insertShape.SHAPE_NAME = objShape.ShapeName;
                                            insertShape.CREATE_USER_ID = this.UserId;
                                            insertShape.CREATE_DATE = DateTime.Now;
                                            insertShape.UPDATE_USER_ID = this.UserId;
                                            insertShape.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8026_M_SHAPE.Add(insertShape);
                                        }
                                    }
                                    if (MakerList != null)
                                    {
                                        IsFound = false;
                                        foreach (Maker MakerEn in MakerList)
                                        {
                                            if (MakerEn.MakerCode == en.Maker)
                                            {
                                                IsFound = true;
                                                break;
                                            }
                                        }
                                        if (IsFound == false)
                                        {
                                            Maker objMaker = new Maker();
                                            objMaker.MakerCode = en.Maker;
                                            objMaker.MakerName = en.MakerName;
                                            MakerList.Add(objMaker);

                                            PSC8027_M_MAKER insertMaker = new PSC8027_M_MAKER();
                                            insertMaker.MAKER = objMaker.MakerCode;
                                            insertMaker.MAKER_NAME = objMaker.MakerName;
                                            insertMaker.CREATE_USER_ID = this.UserId;
                                            insertMaker.CREATE_DATE = DateTime.Now;
                                            insertMaker.UPDATE_USER_ID = this.UserId;
                                            insertMaker.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8027_M_MAKER.Add(insertMaker);
                                        }
                                    }

                                    this.db.PSC8010_M_PIPE_ITEM.Add(insertItem);
                                }
                                else
                                {
                                    objUpdate.DESCRIPTION = en.Description;
                                    objUpdate.OD = Convert.ToDecimal(en.OD);
                                    objUpdate.WT = Convert.ToDecimal(en.WT);
                                    objUpdate.LT = Convert.ToDecimal(en.LT);
                                    objUpdate.UNIT_WEIGHT = en.UnitWeight;
                                    objUpdate.MATERIAL = en.Material;
                                    objUpdate.MATERIAL_NAME = en.MaterialName;
                                    objUpdate.STANDARD = en.standard;
                                    objUpdate.STANDARD_NAME = en.standardName;
                                    objUpdate.GRADE = en.Grade;
                                    objUpdate.GRADE_NAME = en.GradeName;
                                    objUpdate.SHAPE = en.Shape;
                                    objUpdate.SHAPE_NAME = en.ShapeName;
                                    objUpdate.MAKER = en.Maker;
                                    objUpdate.MAKER_NAME = en.MakerName;
                                    objUpdate.LABEL1 = en.Label1;
                                    objUpdate.ATTRIBUTE1 = en.Attribute1;
                                    objUpdate.LABEL2 = en.Label2;
                                    objUpdate.ATTRIBUTE2 = en.Attribute2;
                                    objUpdate.LABEL3 = en.Label3;
                                    objUpdate.ATTRIBUTE3 = en.Attribute3;
                                    objUpdate.LABEL4 = en.Label4;
                                    objUpdate.ATTRIBUTE4 = en.Attribute4;
                                    objUpdate.LABEL5 = en.Label5;
                                    objUpdate.ATTRIBUTE5 = en.Attribute5;
                                    objUpdate.LABEL6 = en.Label6;
                                    objUpdate.ATTRIBUTE6 = en.Attribute6;
                                    objUpdate.LABEL7 = en.Label7;
                                    objUpdate.ATTRIBUTE7 = en.Attribute7;
                                    objUpdate.LABEL8 = en.Label8;
                                    objUpdate.ATTRIBUTE8 = en.Attribute8;
                                    objUpdate.LABEL9 = en.Label9;
                                    objUpdate.ATTRIBUTE9 = en.Attribute9;
                                    objUpdate.LABEL10 = en.Label10;
                                    objUpdate.ATTRIBUTE10 = en.Attribute10;
                                    objUpdate.UPDATE_USER_ID = this.UserId;
                                    objUpdate.UPDATE_DATE = DateTime.Now;
                                }
                            }
                        }

                        if (pPipeReceivedDataList != null)
                        {
                            decimal? dec3100TransNo = db.PSC3100_T_SYTELINE_NEW_PIPE.Max(pi => (decimal?)pi.TRANS_NO);
                            decimal dec3100NewTransNo = 0;
                            foreach (PipeReceivedData en in pPipeReceivedDataList)
                            {
                                var objUpdate = this.db.PSC3100_T_SYTELINE_NEW_PIPE.SingleOrDefault(rd => rd.CONTAINER_NO == en.ContainerNo && rd.ITEM_CODE == en.ItemCode && rd.HEAT_NO == en.HeatNo);

                                // Check duplicate data before insert
                                if (objUpdate == null)
                                {
                                    dec3100NewTransNo = dec3100NewTransNo == 0 ? (Convert.ToInt32(dec3100TransNo == null ? 1 : dec3100TransNo + 1)) : dec3100NewTransNo + 1;
                                    PSC3100_T_SYTELINE_NEW_PIPE insert = new PSC3100_T_SYTELINE_NEW_PIPE();

                                    insert.TRANS_NO = (int)dec3100NewTransNo;
                                    insert.DELIVERY_DATE = en.DeliveryDate;
                                    insert.RECEIVED_DATE = en.ReceivedDate;
                                    insert.CONTAINER_NO = en.ContainerNo;
                                    insert.ITEM_CODE = en.ItemCode;
                                    insert.HEAT_NO = en.HeatNo;
                                    insert.QTY = en.QTY;
                                    insert.BUNDLES = en.Bundles;
                                    insert.PO_NO = "";
                                    //insert.STATUS = Convert.ToByte(Constants.ReceivingStatus.New);
                                    //insert.REMARK = string.Empty;
                                    insert.CREATE_USER_ID = this.UserId;
                                    insert.CREATE_DATE = DateTime.Now;
                                    insert.UPDATE_USER_ID = this.UserId;
                                    insert.UPDATE_DATE = DateTime.Now;

                                    this.db.PSC3100_T_SYTELINE_NEW_PIPE.Add(insert);
                                }
                                else
                                {
                                    objUpdate.DELIVERY_DATE = en.DeliveryDate;
                                    objUpdate.RECEIVED_DATE = en.ReceivedDate;
                                    objUpdate.CONTAINER_NO = en.ContainerNo;
                                    objUpdate.ITEM_CODE = en.ItemCode;
                                    objUpdate.HEAT_NO = en.HeatNo;
                                    objUpdate.QTY = en.QTY;
                                    objUpdate.BUNDLES = en.Bundles;
                                    objUpdate.PO_NO = "";
                                    //objUpdate.STATUS = Convert.ToByte(Constants.ReceivingStatus.New);
                                    //objUpdate.REMARK = string.Empty;
                                    objUpdate.UPDATE_USER_ID = this.UserId;
                                    objUpdate.UPDATE_DATE = DateTime.Now;
                                }
                            }

                            //List<PipeReceivedData> Alllist = pPipeReceivedDataList.Select(r => new PipeReceivedData { DeliveryDate = r.DeliveryDate, ReceivedDate = r.ReceivedDate, ContainerNo = r.ContainerNo }).ToList();
                            //List<PipeReceivedData> Distinctlist = Alllist.Distinct().ToList();
                            List<PipeReceivedData> Distinctlist = (pPipeReceivedDataList.GroupBy(pi => new { pi.DeliveryDate, pi.ReceivedDate, pi.ContainerNo })
                                                                    .Select(m => new PipeReceivedData
                                                                    {
                                                                        DeliveryDate = m.Key.DeliveryDate,
                                                                        ReceivedDate = m.Key.ReceivedDate,
                                                                        ContainerNo = m.Key.ContainerNo
                                                                    })).ToList();

                            int? intReceiveId = db.PSC2110_T_RECEIVING_INSTRUCTION.Max(u => (int?)u.RECEIVE_ID);
                            int intNewReceiveId = 0;
                            foreach (PipeReceivedData en in Distinctlist)
                            {
                                var obj = this.db.PSC2110_T_RECEIVING_INSTRUCTION.SingleOrDefault(x => x.DELIVERY_DATE == en.DeliveryDate && x.CONTAINER_NO == en.ContainerNo);

                                // Check duplicate data before insert
                                if (obj == null)
                                {
                                    intNewReceiveId = intNewReceiveId == 0 ? (Convert.ToInt32(intReceiveId == null ? 1 : intReceiveId + 1)) : intNewReceiveId + 1;

                                    PSC2110_T_RECEIVING_INSTRUCTION insert = new PSC2110_T_RECEIVING_INSTRUCTION();

                                    insert.RECEIVE_ID = intNewReceiveId;
                                    insert.DELIVERY_DATE = en.DeliveryDate;
                                    insert.RECEIVED_DATE = en.ReceivedDate;
                                    insert.CONTAINER_NO = en.ContainerNo;
                                    insert.STATUS = Convert.ToByte(Constants.ReceivingStatus.New);
                                    insert.CREATE_USER_ID = this.UserId;
                                    insert.CREATE_DATE = DateTime.Now;
                                    insert.UPDATE_USER_ID = this.UserId;
                                    insert.UPDATE_DATE = DateTime.Now;

                                    this.db.PSC2110_T_RECEIVING_INSTRUCTION.Add(insert);

                                    List<PipeReceivedData> objPipeReceivedDataItems = (pPipeReceivedDataList.Where(
                                        l => l.DeliveryDate == en.DeliveryDate && l.ReceivedDate == en.ReceivedDate && l.ContainerNo == en.ContainerNo)).ToList();
                                    if (objPipeReceivedDataItems != null)
                                    {
                                        foreach (PipeReceivedData enItem in objPipeReceivedDataItems)
                                        {
                                            var objItem = this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.SingleOrDefault(x => x.RECEIVE_ID == intNewReceiveId && x.ITEM_CODE == enItem.ItemCode  && x.HEAT_NO == enItem.HeatNo);

                                            // Check duplicate data before insert
                                            if (objItem == null)
                                            {
                                                PSC2111_T_RECEIVING_INSTRUCTION_DETAIL insertItem = new PSC2111_T_RECEIVING_INSTRUCTION_DETAIL();

                                                insertItem.RECEIVE_ID = intNewReceiveId;
                                                //insertItem.TRAN_NO = Convert.ToInt32(0);
                                                insertItem.ITEM_CODE = enItem.ItemCode;
                                                insertItem.HEAT_NO = enItem.HeatNo;
                                                insertItem.QTY = enItem.QTY;
                                                insertItem.BUNDLES = enItem.Bundles;
                                                insertItem.PO_NO = "";
                                                insertItem.STATUS = Convert.ToByte(Constants.ReceivingStatus.New);
                                                insertItem.REMARK = string.Empty;
                                                insertItem.CREATE_USER_ID = this.UserId;
                                                insertItem.CREATE_DATE = DateTime.Now;
                                                insertItem.UPDATE_USER_ID = this.UserId;
                                                insertItem.UPDATE_DATE = DateTime.Now;

                                                this.db.PSC2111_T_RECEIVING_INSTRUCTION_DETAIL.Add(insertItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        int SaveChangeResult = this.db.SaveChanges();

                        if (SaveChangeResult > 0)
                        {
                            tran.Complete();
                            result = true;
                        }
                        else
                        {
                            tran.Dispose();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }

                return result;
            }
        }
    }
}