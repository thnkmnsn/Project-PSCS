using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.ModelERPDEV01;
using System.Transactions;
using System.Data.SqlClient;
using System.Data.Objects;

namespace PSCS.Services
{
    public class GetNewPipeFromSyteLineService
    {
        private ModelERPDEV01.AMT_HistoryEntities db;

        public GetNewPipeFromSyteLineService(ModelERPDEV01.AMT_HistoryEntities pDb)
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

        public List<PipeReceivedData> GetNewPipeFromSyteLine()
        {
            List<PipeReceivedData> result = new List<PipeReceivedData>();

            try
            {
                using (this.db)
                {

                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from rc in db.PipeReceiveds
                                select new {
                                    rc.ITEM_CODE,
                                    rc.HEAT_NO,
                                    rc.CONTAINER_NO,
                                    rc.DELIVERY_DATE,
                                    rc.RECEIVED_DATE,
                                    rc.QTY,
                                    rc.BUNDLES
                                }
                                ).AsQueryable();

                    if (obj != null)
                    {
                        result = obj.AsEnumerable().Select((x, index) => new PipeReceivedData
                        {
                            ItemCode = x.ITEM_CODE,
                            HeatNo = x.HEAT_NO,
                            ContainerNo = x.CONTAINER_NO,
                            DeliveryDate = x.DELIVERY_DATE,
                            ReceivedDate = x.RECEIVED_DATE,
                            QTY = Convert.ToDecimal(x.QTY),
                            Bundles = Convert.ToDecimal(x.BUNDLES),
                        }).ToList();
                    }
                    //var obj = (from rc in db.PipeReceiveds
                    //           //where rc.trans_num == null ? Convert.ToDecimal("0") : (decimal?)
                    //           //orderby rc.trans_num
                    //           select new PipeReceivedData
                    //           {
                    //               //TransNo = rc.trans_num,
                    //               //TransNo = 0,
                    //               ItemCode = rc.ITEM_CODE,
                    //               HeatNo = rc.HEAT_NO,
                    //               ContainerNo = rc.CONTAINER_NO,
                    //               DeliveryDate = rc.DELIVERY_DATE,
                    //               ReceivedDate = rc.RECEIVED_DATE,
                    //               QTY = rc.QTY == null ?0:Convert.ToDecimal(rc.QTY),
                    //               Bundles = rc.BUNDLES == null ? 0 : Convert.ToDecimal(rc.BUNDLES),
                    //               //PoNo = rc.PO_NO
                    //               //PoNo = ""
                    //           }).AsQueryable();

                    //result = obj.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public PipeItemData GetNewPipeItemFromSyteLine(string pItemCode, string pHeatNo)
        {
            PipeItemData result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    List<PipeItemData> objPipeItemDataList = (from rc in db.PipePSCSMs
                                                              where rc.item == pItemCode && rc.Heat_no == pHeatNo
                                                              select new PipeItemData
                                                              {
                                                                  ItemCode = rc.item,
                                                                  HeatNo = rc.Heat_no,
                                                                  Description = rc.Description,
                                                                  OD = rc.OD,
                                                                  WT = rc.WT,
                                                                  LT = rc.LT,
                                                                  UnitWeight = rc.unit_weight,
                                                                  Material = rc.Material,
                                                                  MaterialName = rc.Material_name,
                                                                  standard = rc.standard,
                                                                  standardName = rc.standard_name,
                                                                  Grade = rc.Grade,
                                                                  GradeName = rc.Grade_name,
                                                                  Shape = rc.Shape,
                                                                  ShapeName = rc.Shap_name,
                                                                  Maker = rc.Maker,
                                                                  MakerName = rc.Maker_name,
                                                                  Label1 = rc.label_1,
                                                                  Attribute1 = rc.attribute1,
                                                                  Label2 = rc.label_2,
                                                                  Attribute2 = rc.attribute2,
                                                                  Label3 = rc.label_3,
                                                                  Attribute3 = rc.attribute3,
                                                                  Label4 = rc.label_4,
                                                                  Attribute4 = rc.attribute4,
                                                                  Label5 = rc.label_5,
                                                                  Attribute5 = rc.attribute5,
                                                                  Label6 = rc.label_6,
                                                                  Attribute6 = rc.attribute6,
                                                                  Label7 = rc.label_7,
                                                                  Attribute7 = rc.attribute7,
                                                                  Label8 = rc.label_8,
                                                                  Attribute8 = rc.attribute8,
                                                                  Label9 = rc.label_9,
                                                                  Attribute9 = rc.attribute9,
                                                                  Label10 = rc.label_10,
                                                                  Attribute10 = rc.attribute10,
                                                              }).AsQueryable().ToList();
                    if(objPipeItemDataList != null)
                    {
                        if(objPipeItemDataList.Count > 0)
                        {
                            result = objPipeItemDataList[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<PipeItemData> GetPipeItemAndHeatNoFromSyteLineList()
        {
            List<PipeItemData> result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    //List<PipeItemData> objPipeItemDataList = (from rc in db.PipePSCSMs
                    //                                          select new PipeItemData
                    //                                          {
                    //                                              ItemCode = rc.item,
                    //                                              HeatNo = rc.Heat_no
                    //                                          }).AsQueryable().ToList();

                    List<PipeItemData> objPipeItemDataList = (from rc in db.PipePSCSMs /*where rc.Heat_no == "123456"*/
                                                              select new PipeItemData
                                                              {
                                                                  ItemCode = rc.item,
                                                                  HeatNo = rc.Heat_no,
                                                                  Description = rc.Description,
                                                                  OD = rc.OD,
                                                                  WT = rc.WT,
                                                                  LT = rc.LT,
                                                                  UnitWeight = rc.unit_weight,
                                                                  Material = rc.Material,
                                                                  MaterialName = rc.Material_name,
                                                                  standard = rc.standard,
                                                                  standardName = rc.standard_name,
                                                                  Grade = rc.Grade,
                                                                  GradeName = rc.Grade_name,
                                                                  Shape = rc.Shape,
                                                                  ShapeName = rc.Shap_name,
                                                                  Maker = rc.Maker,
                                                                  MakerName = rc.Maker_name,
                                                                  Label1 = rc.label_1,
                                                                  Attribute1 = rc.attribute1,
                                                                  Label2 = rc.label_2,
                                                                  Attribute2 = rc.attribute2,
                                                                  Label3 = rc.label_3,
                                                                  Attribute3 = rc.attribute3,
                                                                  Label4 = rc.label_4,
                                                                  Attribute4 = rc.attribute4,
                                                                  Label5 = rc.label_5,
                                                                  Attribute5 = rc.attribute5,
                                                                  Label6 = rc.label_6,
                                                                  Attribute6 = rc.attribute6,
                                                                  Label7 = rc.label_7,
                                                                  Attribute7 = rc.attribute7,
                                                                  Label8 = rc.label_8,
                                                                  Attribute8 = rc.attribute8,
                                                                  Label9 = rc.label_9,
                                                                  Attribute9 = rc.attribute9,
                                                                  Label10 = rc.label_10,
                                                                  Attribute10 = rc.attribute10,
                                                              }).AsQueryable().ToList();

                    if (objPipeItemDataList != null)
                    {
                        if (objPipeItemDataList.Count > 0)
                        {
                            result = objPipeItemDataList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean CallUpdatePipeProcedure()
        {
            Boolean result = false;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    int intResult = db.UpdatePipesp();
                    
                    if(intResult > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


    }
}