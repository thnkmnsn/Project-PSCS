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
    public class PipeService
    {
        private PSCSEntities db;

        public PipeService(PSCSEntities pDb)
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
        

        public PipeDetail GetPipeListById(string item_code, string heat_no, string receive_date)
        {
            PipeDetail result = new PipeDetail();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    DateTime receiveDate = Convert.ToDateTime(receive_date).Date;

                    var obj =  this.db.PSC8010_M_PIPE
                              .SingleOrDefault(x => x.ITEM_CODE == item_code 
                                            && x.HEAT_NO == heat_no
                                           /* && x.RECEIVED_DATE == receiveDate*/);

                    result = new PipeDetail()
                    {
                        InputItemCode = obj.ITEM_CODE,
                        InputHeatNo = obj.HEAT_NO,
                        //InputContainerNo = obj.CONTAINER_NO,
                        //ReceiveDate = obj.RECEIVED_DATE,
                        //InputReceiveDate = string.Format("{0:yyyy-MM-dd}", obj.RECEIVED_DATE),
                        //InputQTY = obj.QTY.ToString(),
                        InputUnitWeight = obj.UNIT_WEIGHT.ToString(),
                        //InputBundles = obj.BUNDLES.ToString(),
                        InputMaterial = obj.MATERIAL,
                        InputMaterialName = obj.MATERIAL_NAME,
                        InputStandard = obj.STANDARD,
                        InputStandardName = obj.STANDARD_NAME,
                        InputGrade = obj.GRADE,
                        InputGradeName = obj.GRADE_NAME,
                        InputShape = obj.SHAPE,
                        InputShapeName = obj.SHAPE_NAME,
                        InputOD = obj.OD.ToString(),
                        InputWT = obj.WT.ToString(),
                        InputLT = obj.LT.ToString(),
                        InputMaker = obj.MAKER,
                        InputMakerName = obj.MAKER_NAME,
                        //InputDisplayOrder = obj.DISPLAY_ORDER,
                        //InputIsBundled = obj.IS_BUNDLED,
                        //InputBundedName = obj.IS_BUNDLED.ToString()
                    };    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Pipe> GetPipeList(string pItemCode, string pHeatNo, string pContainNo, DateTime? pReceiveDate, 
                                      decimal? pQty, decimal? pWeight, string pMaterialName, string pStandardName,
                                      string pGradeName, string pShapeName, decimal? pOD, decimal? pWT, 
                                      decimal? pLT, string pMakerName, List<byte> pIsBundled, string lang)
        {
            List<Pipe> result = new List<Pipe>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;


                    var obj = (from pi in db.PSC8010_M_PIPE
                               orderby pi.ITEM_CODE
                               select new
                               {
                                   pi.ITEM_CODE,
                                   pi.HEAT_NO,
                                   //pi.CONTAINER_NO,
                                   //pi.RECEIVED_DATE,
                                   //pi.QTY,
                                   pi.UNIT_WEIGHT,
                                   //pi.BUNDLES,
                                   pi.MATERIAL,
                                   pi.MATERIAL_NAME,
                                   pi.STANDARD,
                                   pi.STANDARD_NAME,
                                   pi.GRADE,
                                   pi.GRADE_NAME,
                                   pi.SHAPE,
                                   pi.SHAPE_NAME,
                                   pi.OD,
                                   pi.WT,
                                   pi.LT,
                                   pi.MAKER,
                                   pi.MAKER_NAME,
                                   //pi.IS_BUNDLED,
                                   //pi.DISPLAY_ORDER,
                                   pi.CREATE_DATE,
                                   pi.CREATE_USER_ID,
                                   pi.UPDATE_DATE,
                                   pi.UPDATE_USER_ID
                               }).AsQueryable();

                    // Item Code
                    if (!string.IsNullOrEmpty(pItemCode))
                    {
                        obj = obj.Where(x => x.ITEM_CODE.Contains(pItemCode));
                    }

                    // Heat No
                    if (!string.IsNullOrEmpty(pHeatNo))
                    {
                        obj = obj.Where(x => x.HEAT_NO.Contains(pHeatNo));
                    }

                    // Contain No
                    //if (!string.IsNullOrEmpty(pContainNo))
                    //{
                    //    obj = obj.Where(x => x.CONTAINER_NO.Contains(pContainNo));
                    //}

                    // Receive Date
                    //if (pReceiveDate != null)
                    //{
                    //    DateTime receiveDate = Convert.ToDateTime(pReceiveDate).Date;
                    //    obj = obj.Where(x => x.RECEIVED_DATE == receiveDate);
                    //}

                    // Qty
                    //if (pQty != null)
                    //{
                    //    obj = obj.Where(x => x.QTY == pQty);
                    //}

                    // Weight
                    if (pWeight != null)
                    {
                        obj = obj.Where(x => x.UNIT_WEIGHT == pWeight);
                    }

                    // Material Name
                    if (!string.IsNullOrEmpty(pMaterialName))
                    {
                        obj = obj.Where(x => x.MATERIAL_NAME.Contains(pMaterialName));
                    }

                    // Standard Name
                    if (!string.IsNullOrEmpty(pStandardName))
                    {
                        obj = obj.Where(x => x.STANDARD_NAME.Contains(pStandardName));
                    }

                    // Grade Name
                    if (!string.IsNullOrEmpty(pGradeName))
                    {
                        obj = obj.Where(x => x.GRADE_NAME.Contains(pGradeName));
                    }

                    // Shape Name
                    if (!string.IsNullOrEmpty(pShapeName))
                    {
                        obj = obj.Where(x => x.SHAPE_NAME.Contains(pShapeName));
                    }

                    // OD
                    if (pOD != null)
                    {
                        obj = obj.Where(x => x.OD == pOD);
                    }

                    // WT
                    if (pWT != null)
                    {
                        obj = obj.Where(x => x.WT == pWT);
                    }

                    // LT
                    if (pLT != null)
                    {
                        obj = obj.Where(x => x.LT == pLT);
                    }

                    // Maker Name
                    if (!string.IsNullOrEmpty(pMakerName))
                    {
                        obj = obj.Where(x => x.MAKER_NAME.Contains(pMakerName));
                    }

                    //Is Bundled
                    //if (pIsBundled != null)
                    //{
                    //    obj = obj.Where(x => pIsBundled.Contains(x.IS_BUNDLED.Value));
                    //}

                    result = obj.AsEnumerable().Select((pi, index) => new Pipe
                    {
                        RowNo = index + 1,
                        ItemCode = pi.ITEM_CODE,
                        HeatNo = pi.HEAT_NO,
                        //ContainerNo = pi.CONTAINER_NO,
                        //ReceiveDate = pi.RECEIVED_DATE,
                        //Qty = pi.QTY,
                        UnitWeight = pi.UNIT_WEIGHT,
                        //Bundles = pi.BUNDLES,
                        Material = pi.MATERIAL,
                        MaterialName = pi.MATERIAL_NAME,
                        Standard = pi.STANDARD,
                        StandardName = pi.STANDARD_NAME,
                        Grade = pi.GRADE,
                        GradeName = pi.GRADE_NAME,
                        Shape = pi.SHAPE,
                        ShapeName = pi.SHAPE_NAME,
                        OD = pi.OD,
                        WT = pi.WT,
                        LT = pi.LT,
                        Maker = pi.MAKER,
                        MakerName = pi.MAKER_NAME,
                        //DisplayOrder = pi.DISPLAY_ORDER,
                        //IsBundedName = GetIsBundled(pi.IS_BUNDLED.Value),


                    }).ToList();

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }



        public bool InsertData(List<StockTakingScreen> list)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (db)
                    {
                        bool isInsert = false;
                        db.Configuration.LazyLoadingEnabled = false;

                        DateTime insertDate = DateTime.Now;
                        foreach (StockTakingScreen s in list)
                        {
                            if (!s.Check)
                            {
                                continue;
                            }

                            string itemCode = s.ItemCode;
                            string heatNo = s.HeatNo;

                            // Get data from PSC2010_T_STOCK
                            PSC8010_M_PIPE psc8010 = db.PSC8010_M_PIPE
                                        .Select(x => x)
                                        .Where(x => x.ITEM_CODE.Equals(itemCode) && x.HEAT_NO.Equals(heatNo))
                                        .FirstOrDefault();

                            if (psc8010 != null)
                            {
                                continue;
                            }
                            else
                            {
                                isInsert = true;
                                PSC8010_M_PIPE insert = new PSC8010_M_PIPE();
                                insert.ITEM_CODE = s.ItemCode;
                                insert.HEAT_NO = s.HeatNo;
                                //insert.RECEIVED_DATE = s.StockTakingDate;
                                //insert.QTY = 0;
                                insert.UNIT_WEIGHT = s.unit_weight;
                                //insert.BUNDLES = 0;
                                insert.MATERIAL = string.Empty;
                                insert.MATERIAL_NAME = string.Empty;
                                insert.STANDARD = string.Empty;
                                insert.STANDARD_NAME = string.Empty;
                                insert.GRADE = string.Empty;
                                insert.GRADE_NAME = s.Grade;
                                insert.SHAPE = string.Empty;
                                insert.SHAPE_NAME = string.Empty;
                                insert.OD = s.OD;
                                insert.WT = s.WT;
                                insert.LT = s.Lenght;
                                insert.MAKER = string.Empty;
                                insert.MAKER_NAME = s.Maker;
                                //insert.IS_BUNDLED = 0;
                                insert.CREATE_DATE = insertDate;
                                insert.CREATE_USER_ID = s.UserId;
                                insert.UPDATE_DATE = insertDate;
                                insert.UPDATE_USER_ID = s.UserId;
                                db.PSC8010_M_PIPE.Add(insert);
                            }
                        }

                        if (isInsert)
                        {
                            int result = db.SaveChanges();

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
                        else
                        {
                            return true;
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

        private string GetIsBundled(byte pIS_BUNDLED)
        {
            string result = string.Empty;
            if (pIS_BUNDLED == 1)
            {
                result = PSCS.Resources.Common_cshtml.Yes;
            }
            else if (pIS_BUNDLED == 0)
            {
                result = PSCS.Resources.Common_cshtml.No;
            }
            else
            {
                result = "";
            }
            return result;
        }
    }

    

    #endregion
}