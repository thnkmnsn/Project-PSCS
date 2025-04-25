using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSCS.Models;
using PSCS.ModelERPDEV01;
using PSCS.ModelsScreen;
using System.Transactions;
using System.Data;

namespace PSCS.Services
{
    public class PipeItemService
    {
        private PSCSEntities db;

        public PipeItemService(PSCSEntities pDb)
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
        public PipeItem GetPipeItem(string pItemCode, string pHeatNo)
        {
            PipeItem result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var objPipeItem = this.db.PSC8010_M_PIPE_ITEM.SingleOrDefault(x => x.ITEM_CODE == pItemCode && x.HEAT_NO == pHeatNo);

                    if (objPipeItem != null)
                    {
                        result = new PipeItem()
                        {
                            ItemCode = objPipeItem.ITEM_CODE,
                            Description = objPipeItem.DESCRIPTION,
                            HeatNo = objPipeItem.HEAT_NO,
                            OD = objPipeItem.OD,
                            WT = objPipeItem.WT,
                            LT = objPipeItem.LT,
                            UnitWeight = objPipeItem.UNIT_WEIGHT,
                            Material = objPipeItem.MATERIAL,
                            MaterialName = objPipeItem.MATERIAL_NAME,
                            standard = objPipeItem.STANDARD,
                            standardName = objPipeItem.STANDARD_NAME,
                            Grade = objPipeItem.GRADE,
                            GradeName = objPipeItem.GRADE_NAME,
                            Shape = objPipeItem.SHAPE,
                            ShapeName = objPipeItem.SHAPE_NAME,
                            Maker = objPipeItem.MAKER,
                            MakerName = objPipeItem.MAKER_NAME,
                            Label1 = objPipeItem.LABEL1,
                            Attribute1 = objPipeItem.ATTRIBUTE1,
                            Label2 = objPipeItem.LABEL2,
                            Attribute2 = objPipeItem.ATTRIBUTE2,
                            Label3 = objPipeItem.LABEL3,
                            Attribute3 = objPipeItem.ATTRIBUTE3,
                            Label4 = objPipeItem.LABEL4,
                            Attribute4 = objPipeItem.ATTRIBUTE4,
                            Label5 = objPipeItem.LABEL5,
                            Attribute5 = objPipeItem.ATTRIBUTE5,
                            Label6 = objPipeItem.LABEL6,
                            Attribute6 = objPipeItem.ATTRIBUTE6,
                            Label7 = objPipeItem.LABEL7,
                            Attribute7 = objPipeItem.ATTRIBUTE7,
                            Label8 = objPipeItem.LABEL8,
                            Attribute8 = objPipeItem.ATTRIBUTE8,
                            Label9 = objPipeItem.LABEL9,
                            Attribute9 = objPipeItem.ATTRIBUTE9,
                            Label10 = objPipeItem.LABEL10,
                            Attribute10 = objPipeItem.ATTRIBUTE10,
                            CreateUserID = objPipeItem.CREATE_USER_ID,
                            CreateDate = objPipeItem.CREATE_DATE,
                            UpdateUserID = objPipeItem.UPDATE_USER_ID,
                            UpdateDate = objPipeItem.UPDATE_DATE,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public PipeItemDetail GetPipeListById(string item_code, string heat_no)
        {
            PipeItemDetail result = new PipeItemDetail();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    //DateTime receiveDate = Convert.ToDateTime(receive_date).Date;

                    var obj = (from pi in db.PSC8010_M_PIPE_ITEM
                               join mt in db.PSC8023_M_MATERIAL on pi.MATERIAL equals mt.MATERIAL
                               join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                               join mg in db.PSC8025_M_GRADE on pi.GRADE equals mg.GRADE
                               join ms in db.PSC8024_M_STANDARD on pi.STANDARD equals ms.STANDARD
                               join mh in db.PSC8026_M_SHAPE on pi.SHAPE equals mh.SHAPE
                               where pi.ITEM_CODE == item_code && pi.HEAT_NO == heat_no
                               select new
                               {
                                   pi.ITEM_CODE,
                                   pi.DESCRIPTION,
                                   pi.HEAT_NO,
                                   pi.UNIT_WEIGHT,
                                   pi.MATERIAL,
                                   mt.MATERIAL_NAME,
                                   pi.STANDARD,
                                   ms.STANDARD_NAME,
                                   pi.GRADE,
                                   mg.GRADE_NAME,
                                   pi.SHAPE,
                                   mh.SHAPE_NAME,
                                   pi.OD,
                                   pi.WT,
                                   pi.LT,
                                   pi.MAKER,
                                   mm.MAKER_NAME,
                                   pi.PT370,
                                   pi.PG3701,
                                   pi.EN_Spec,
                                   pi.Aramco,
                                   pi.Gerab_PO,
                                   pi.Singapore,
                                   pi.C21_SHL1,
                                   pi.MN,
                                   pi.C,
                               }).SingleOrDefault();

                    result = new PipeItemDetail()
                    {
                        DetailItemCode = obj.ITEM_CODE,
                        DetailDescription = obj.DESCRIPTION,
                        DetailHeatNo = obj.HEAT_NO,
                        DetailUnitWeight = obj.UNIT_WEIGHT,
                        DetailMaterial = obj.MATERIAL,
                        DetailMaterialName = obj.MATERIAL_NAME,
                        DetailStandard = obj.STANDARD,
                        DetailStandardName = obj.STANDARD_NAME,
                        DetailGrade = obj.GRADE,
                        DetailGradeName = obj.GRADE_NAME,
                        DetailShape = obj.SHAPE,
                        DetailShapeName = obj.SHAPE_NAME,
                        DetailOD = obj.OD,
                        DetailWT = obj.WT,
                        DetailLT = obj.LT,
                        DetailSize = (obj.OD != null ? obj.OD.Value.ToString("#,##0.0") : "0.0") + " * " + (obj.WT != null ? obj.WT.Value.ToString("#,##0.0") : "0.0") + " * " + (obj.LT != null ? obj.LT.Value.ToString("#,##0") : "0"),
                        DetailMaker = obj.MAKER,
                        DetailMakerName = obj.MAKER_NAME,
                        PT370 = obj.PT370,
                        PG370 = obj.PG3701,
                        EN_Spec = obj.EN_Spec,
                        Aramco = obj.Aramco,
                        Gerab_PO = obj.Gerab_PO,
                        Singapore = obj.Singapore,
                        C21_SHL1 = obj.C21_SHL1,
                        MN = obj.MN,
                        C = obj.C,
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<PipeItem> GetPipeList(string pItemCode, string pDescription, string pHeatNo,
                                      decimal? pWeight, string pMaterial, string pStandard,
                                      string pGrade, string pShape, decimal? pOD, decimal? pWT,
                                      decimal? pLT, string pMaker, string pOrderBy, string pSortBy, string lang)
        {
            List<PipeItem> result = new List<PipeItem>();
            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var objPipeList = (from pi in db.PSC8010_M_PIPE_ITEM
                                       join mt in db.PSC8023_M_MATERIAL on pi.MATERIAL equals mt.MATERIAL
                                       join mm in db.PSC8027_M_MAKER on pi.MAKER equals mm.MAKER
                                       join mg in db.PSC8025_M_GRADE on pi.GRADE equals mg.GRADE
                                       join ms in db.PSC8024_M_STANDARD on pi.STANDARD equals ms.STANDARD
                                       join mh in db.PSC8026_M_SHAPE on pi.SHAPE equals mh.SHAPE
                                       orderby pi.ITEM_CODE
                                       select new
                                       {
                                           pi.ITEM_CODE,
                                           pi.DESCRIPTION,
                                           pi.HEAT_NO,
                                           pi.UNIT_WEIGHT,
                                           pi.MATERIAL,
                                           mt.MATERIAL_NAME,
                                           pi.STANDARD,
                                           ms.STANDARD_NAME,
                                           pi.GRADE,
                                           mg.GRADE_NAME,
                                           pi.SHAPE,
                                           mh.SHAPE_NAME,
                                           pi.OD,
                                           pi.WT,
                                           pi.LT,
                                           pi.MAKER,
                                           mm.MAKER_NAME,
                                           pi.CREATE_DATE,
                                           pi.CREATE_USER_ID,
                                           pi.UPDATE_DATE,
                                           pi.UPDATE_USER_ID
                                       }).AsQueryable();

                    // Item Code
                    if (!string.IsNullOrEmpty(pItemCode))
                    {
                        objPipeList = objPipeList.Where(x => x.ITEM_CODE.Contains(pItemCode));
                    }
                    // DESCRIPTION
                    if (!string.IsNullOrEmpty(pDescription))
                    {
                        objPipeList = objPipeList.Where(x => x.DESCRIPTION.Contains(pDescription));
                    }
                    // Heat No
                    if (!string.IsNullOrEmpty(pHeatNo))
                    {
                        objPipeList = objPipeList.Where(x => x.HEAT_NO.Contains(pHeatNo));
                    }
                    // OD
                    if (pOD != null)
                    {
                        objPipeList = objPipeList.Where(x => x.OD == pOD);
                    }
                    // WT
                    if (pWT != null)
                    {
                        objPipeList = objPipeList.Where(x => x.WT == pWT);
                    }
                    // LT
                    if (pLT != null)
                    {
                        objPipeList = objPipeList.Where(x => x.LT == pLT);
                    }
                    // Weight
                    if (pWeight != null)
                    {
                        objPipeList = objPipeList.Where(x => x.UNIT_WEIGHT == pWeight);
                    }
                    // Material ID
                    if (!string.IsNullOrEmpty(pMaterial))
                    {
                        objPipeList = objPipeList.Where(x => x.MATERIAL == pMaterial);
                    }
                    // Standard ID
                    if (!string.IsNullOrEmpty(pStandard))
                    {
                        objPipeList = objPipeList.Where(x => x.STANDARD == pStandard);
                    }
                    // Grade ID
                    if (!string.IsNullOrEmpty(pGrade))
                    {
                        objPipeList = objPipeList.Where(x => x.GRADE == pGrade);
                    }
                    // Shape ID
                    if (!string.IsNullOrEmpty(pShape))
                    {
                        objPipeList = objPipeList.Where(x => x.SHAPE == pShape);
                    }
                    // Maker ID
                    if (!string.IsNullOrEmpty(pMaker))
                    {
                        objPipeList = objPipeList.Where(x => x.MAKER == pMaker);
                    }


                    //OrderBySortBy
                    if (!string.IsNullOrEmpty(pOrderBy) && !string.IsNullOrEmpty(pSortBy))
                    {
                        if (pSortBy.Equals(Common.Constants.SortBy.ASC.ToString()))
                        {
                            if (pOrderBy.Equals(Common.Constants.OrderBy.OD.ToString()))
                            {
                                objPipeList = objPipeList.OrderBy(x => x.OD);
                            }
                            else if (pOrderBy.Equals(Common.Constants.OrderBy.WT.ToString()))
                            {
                                objPipeList = objPipeList.OrderBy(x => x.WT);
                            }
                            else if (pOrderBy.Equals(Common.Constants.OrderBy.LT.ToString()))
                            {
                                objPipeList = objPipeList.OrderBy(x => x.LT);
                            }
                        }
                        else
                        {
                            if (pOrderBy.Equals(Common.Constants.OrderBy.OD.ToString()))
                            {
                                objPipeList = objPipeList.OrderByDescending(x => x.OD);
                            }
                            else if (pOrderBy.Equals(Common.Constants.OrderBy.WT.ToString()))
                            {
                                objPipeList = objPipeList.OrderByDescending(x => x.WT);
                            }
                            else if (pOrderBy.Equals(Common.Constants.OrderBy.LT.ToString()))
                            {
                                objPipeList = objPipeList.OrderByDescending(x => x.LT);
                            }
                        }
                    }

                    result = objPipeList.AsEnumerable().Select((pi, index) => new PipeItem
                    {
                        RowNo = index + 1,
                        ItemCode = pi.ITEM_CODE,
                        Description = pi.DESCRIPTION,
                        HeatNo = pi.HEAT_NO,
                        UnitWeight = pi.UNIT_WEIGHT,
                        Material = pi.MATERIAL,
                        MaterialName = pi.MATERIAL_NAME,
                        standard = pi.STANDARD,
                        standardName = pi.STANDARD_NAME,
                        Grade = pi.GRADE,
                        GradeName = pi.GRADE_NAME,
                        Shape = pi.SHAPE,
                        ShapeName = pi.SHAPE_NAME,
                        OD = pi.OD,
                        WT = pi.WT,
                        LT = pi.LT,
                        Size = (pi.OD != null ? pi.OD.Value.ToString("#,##0.0") : "0.0") + " * " + (pi.WT != null ? pi.WT.Value.ToString("#,##0.0") : "0.0") + " * " + (pi.LT != null ? pi.LT.Value.ToString("#,##0") : "0"),
                        Maker = pi.MAKER,
                        MakerName = pi.MAKER_NAME
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<PipeAttribute> GetAttributeList(string item_code, string heat_no)
        {
            List<PipeAttribute> result = new List<PipeAttribute>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    //DateTime receiveDate = Convert.ToDateTime(receive_date).Date;
                    var obj = (from pi in db.PSC8010_M_PIPE_ITEM
                               where pi.ITEM_CODE == item_code && pi.HEAT_NO == heat_no
                               select pi).SingleOrDefault();

                    if (obj != null)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            PipeAttribute attr = new PipeAttribute();
                            switch (i)
                            {
                                case 1:
                                    {
                                        if (obj.LABEL1 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL1;
                                            attr.Attribute = obj.ATTRIBUTE1;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        if (obj.LABEL2 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL2;
                                            attr.Attribute = obj.ATTRIBUTE2;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        if (obj.LABEL3 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL3;
                                            attr.Attribute = obj.ATTRIBUTE3;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        if (obj.LABEL4 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL4;
                                            attr.Attribute = obj.ATTRIBUTE4;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 5:
                                    {
                                        if (obj.LABEL5 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL5;
                                            attr.Attribute = obj.ATTRIBUTE5;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 6:
                                    {
                                        if (obj.LABEL6 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL6;
                                            attr.Attribute = obj.ATTRIBUTE6;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 7:
                                    {
                                        if (obj.LABEL7 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL7;
                                            attr.Attribute = obj.ATTRIBUTE7;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 8:
                                    {
                                        if (obj.LABEL8 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL8;
                                            attr.Attribute = obj.ATTRIBUTE8;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 9:
                                    {
                                        if (obj.LABEL9 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL9;
                                            attr.Attribute = obj.ATTRIBUTE9;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                case 10:
                                    {
                                        if (obj.LABEL10 != null)
                                        {
                                            attr.RowNo = i;
                                            attr.Label = obj.LABEL10;
                                            attr.Attribute = obj.ATTRIBUTE10;
                                            result.Add(attr);
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
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

        //public List<PipeItem> GetGradeList()
        //{
        //    List<PipeItem> result = null;

        //    try
        //    {
        //        db.Configuration.LazyLoadingEnabled = false;

        //        result = (from pi in db.PSC8010_M_PIPE_ITEM
        //                  orderby pi.GRADE
        //                  select new PipeItem
        //                  {
        //                      Grade = pi.GRADE,
        //                      GradeName = pi.GRADE_NAME

        //                  }).Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        //public List<PipeItem> GetMaterialList()
        //{
        //    List<PipeItem> result = null;

        //    try
        //    {
        //        db.Configuration.LazyLoadingEnabled = false;

        //        result = (from pi in db.PSC8010_M_PIPE_ITEM
        //                  orderby pi.MATERIAL
        //                  select new PipeItem
        //                  {
        //                      Material = pi.MATERIAL,
        //                      MaterialName = pi.MATERIAL_NAME

        //                  }).Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        //public List<PipeItem> GetStandardList()
        //{
        //    List<PipeItem> result = null;

        //    try
        //    {
        //        db.Configuration.LazyLoadingEnabled = false;

        //        result = (from pi in db.PSC8010_M_PIPE_ITEM
        //                  orderby pi.STANDARD
        //                  select new PipeItem
        //                  {
        //                      standard = pi.STANDARD,
        //                      standardName = pi.STANDARD_NAME

        //                  }).Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        //public List<PipeItem> GetShapeList()
        //{
        //    List<PipeItem> result = null;

        //    try
        //    {
        //        db.Configuration.LazyLoadingEnabled = false;

        //        result = (from pi in db.PSC8010_M_PIPE_ITEM
        //                  orderby pi.SHAPE
        //                  select new PipeItem
        //                  {
        //                      Shape = pi.SHAPE,
        //                      ShapeName = pi.SHAPE_NAME

        //                  }).Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        //public List<PipeItem> GetMakerList()
        //{
        //    List<PipeItem> result = null;

        //    try
        //    {
        //        db.Configuration.LazyLoadingEnabled = false;

        //        result = (from pi in db.PSC8010_M_PIPE_ITEM
        //                  orderby pi.MAKER
        //                  select new PipeItem
        //                  {
        //                      Maker = pi.MAKER,
        //                      MakerName = pi.MAKER_NAME

        //                  }).Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        public Boolean UpdatePipeItem_Old(PipeItemData pPipeItemData, string pUserId)
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
                        PipeItemData en = pPipeItemData;

                        if (en != null)
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

                            decimal? decTransNo = db.PSC3200_T_SYTELINE_PIPE_ITEM.Max(pi => (int?)pi.TRANS_NO);
                            decimal decNewTransNo = 0;


                            decNewTransNo = decNewTransNo == 0 ? (Convert.ToDecimal(decTransNo == null ? 1 : decTransNo + 1)) : decNewTransNo + 1;
                            PSC3200_T_SYTELINE_PIPE_ITEM insert = new PSC3200_T_SYTELINE_PIPE_ITEM();

                            insert.TRANS_NO = (float)decNewTransNo;
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
                            insert.CREATE_USER_ID = pUserId;
                            insert.CREATE_DATE = DateTime.Now;
                            insert.UPDATE_USER_ID = pUserId;
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
                                //insertItem.MATERIAL_NAME = en.MaterialName;
                                insertItem.STANDARD = en.standard;
                                //insertItem.STANDARD_NAME = en.standardName;
                                insertItem.GRADE = en.Grade;
                                //insertItem.GRADE_NAME = en.GradeName;
                                insertItem.SHAPE = en.Shape;
                                //insertItem.SHAPE_NAME = en.ShapeName;
                                insertItem.MAKER = en.Maker;
                                //insertItem.MAKER_NAME = en.MakerName;
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
                                insertItem.CREATE_USER_ID = pUserId;
                                insertItem.CREATE_DATE = DateTime.Now;
                                insertItem.UPDATE_USER_ID = pUserId;
                                insertItem.UPDATE_DATE = DateTime.Now;

                                Boolean IsFound = false;

                                if (MateialList != null)
                                {
                                    IsFound = false;
                                    foreach (Material MaterialEn in MateialList)
                                    {
                                        if (MaterialEn.MaterialCode == en.Material)
                                        {
                                            IsFound = true;
                                            break;
                                        }
                                    }
                                    if (IsFound == false)
                                    {
                                        Material objMat = new Material();
                                        objMat.MaterialCode = en.Material;
                                        objMat.MaterialName = en.MaterialName;
                                        MateialList.Add(objMat);

                                        PSC8023_M_MATERIAL insertMat = new PSC8023_M_MATERIAL();
                                        insertMat.MATERIAL = objMat.MaterialCode;
                                        insertMat.MATERIAL_NAME = objMat.MaterialName;
                                        insertMat.CREATE_USER_ID = pUserId;
                                        insertMat.CREATE_DATE = DateTime.Now;
                                        insertMat.UPDATE_USER_ID = pUserId;
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
                                        insertStd.CREATE_USER_ID = pUserId;
                                        insertStd.CREATE_DATE = DateTime.Now;
                                        insertStd.UPDATE_USER_ID = pUserId;
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
                                        insertGrade.CREATE_USER_ID = pUserId;
                                        insertGrade.CREATE_DATE = DateTime.Now;
                                        insertGrade.UPDATE_USER_ID = pUserId;
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
                                        insertShape.CREATE_USER_ID = pUserId;
                                        insertShape.CREATE_DATE = DateTime.Now;
                                        insertShape.UPDATE_USER_ID = pUserId;
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
                                        insertMaker.CREATE_USER_ID = pUserId;
                                        insertMaker.CREATE_DATE = DateTime.Now;
                                        insertMaker.UPDATE_USER_ID = pUserId;
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
                                //objUpdate.MATERIAL_NAME = en.MaterialName;
                                objUpdate.STANDARD = en.standard;
                                //objUpdate.STANDARD_NAME = en.standardName;
                                objUpdate.GRADE = en.Grade;
                                //objUpdate.GRADE_NAME = en.GradeName;
                                objUpdate.SHAPE = en.Shape;
                                //objUpdate.SHAPE_NAME = en.ShapeName;
                                objUpdate.MAKER = en.Maker;
                                //objUpdate.MAKER_NAME = en.MakerName;
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
                                objUpdate.UPDATE_USER_ID = pUserId;
                                objUpdate.UPDATE_DATE = DateTime.Now;
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

        public Boolean UpdatePipeItem(List<PipeItemData> pPipeItemList, string pUserId)
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
                        List<PipeItemData> InsertPipeItemList = null;

                        if (pPipeItemList != null)
                        {
                            
                            List<PSC8010_M_PIPE_ITEM> objPipeItemsList = this.db.PSC8010_M_PIPE_ITEM.ToList();

                            //foreach (PSC8010_M_PIPE_ITEM en in objPipeItemsList)
                            //{
                            //    this.db.PSC8010_M_PIPE_ITEM.Remove(en);
                            //}
                            //int SaveDeleteChangeResult = this.db.SaveChanges();

                            if(objPipeItemsList != null)
                            {
                                foreach (PipeItemData enSyteLine in pPipeItemList)
                                {
                                    var objUpdate = objPipeItemsList.SingleOrDefault(pi => pi.ITEM_CODE == enSyteLine.ItemCode && pi.HEAT_NO == enSyteLine.HeatNo);
                                    if (objUpdate == null)
                                    {
                                        if(InsertPipeItemList == null)
                                        {
                                            InsertPipeItemList = new List<PipeItemData>();
                                        }
                                        InsertPipeItemList.Add(enSyteLine);
                                    }
                                }
                            }

                            if (InsertPipeItemList != null)
                            {
                                //[PSC8023_M_MATERIAL]
                                List<Material> objMateialDistinctList = (InsertPipeItemList.GroupBy(pi => new { pi.Material, pi.MaterialName })
                                                                   .Select(m => new Material
                                                                   {
                                                                       MaterialCode = m.Key.Material,
                                                                       MaterialName = m.Key.MaterialName
                                                                   })).ToList();
                                if (objMateialDistinctList != null)
                                {
                                    foreach (Material en in objMateialDistinctList)
                                    {
                                        var objUpdate = this.db.PSC8023_M_MATERIAL.SingleOrDefault(me => me.MATERIAL == en.MaterialCode);

                                        // Check duplicate data before insert
                                        if (objUpdate == null)
                                        {
                                            PSC8023_M_MATERIAL insertMat = new PSC8023_M_MATERIAL();
                                            insertMat.MATERIAL = en.MaterialCode;
                                            insertMat.MATERIAL_NAME = en.MaterialName;
                                            insertMat.CREATE_USER_ID = pUserId;
                                            insertMat.CREATE_DATE = DateTime.Now;
                                            insertMat.UPDATE_USER_ID = pUserId;
                                            insertMat.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8023_M_MATERIAL.Add(insertMat);
                                        }
                                    }
                                }

                                //[PSC8024_M_STANDARD]
                                List<Standard> objStandardDistinctList = (InsertPipeItemList.GroupBy(pi => new { pi.standard, pi.standardName })
                                                                       .Select(m => new Standard
                                                                       {
                                                                           StandardCode = m.Key.standard,
                                                                           StandardName = m.Key.standardName
                                                                       })).ToList();
                                if (objStandardDistinctList != null)
                                {
                                    foreach (Standard en in objStandardDistinctList)
                                    {
                                        var objUpdate = this.db.PSC8024_M_STANDARD.SingleOrDefault(st => st.STANDARD == en.StandardCode);

                                        // Check duplicate data before insert
                                        if (objUpdate == null)
                                        {
                                            PSC8024_M_STANDARD insertStd = new PSC8024_M_STANDARD();
                                            insertStd.STANDARD = en.StandardCode;
                                            insertStd.STANDARD_NAME = en.StandardName;
                                            insertStd.CREATE_USER_ID = pUserId;
                                            insertStd.CREATE_DATE = DateTime.Now;
                                            insertStd.UPDATE_USER_ID = pUserId;
                                            insertStd.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8024_M_STANDARD.Add(insertStd);
                                        }
                                    }
                                }
                                //[PSC8025_M_GRADE]
                                List<Grade> objGradeDistinctList = (InsertPipeItemList.GroupBy(pi => new { pi.Grade, pi.GradeName })
                                                                       .Select(m => new Grade
                                                                       {
                                                                           GradeCode = m.Key.Grade,
                                                                           GradeName = m.Key.GradeName
                                                                       })).ToList();
                                if (objGradeDistinctList != null)
                                {
                                    foreach (Grade en in objGradeDistinctList)
                                    {
                                        var objUpdate = this.db.PSC8025_M_GRADE.SingleOrDefault(gr => gr.GRADE == en.GradeCode);

                                        // Check duplicate data before insert
                                        if (objUpdate == null)
                                        {
                                            PSC8025_M_GRADE insertGrd = new PSC8025_M_GRADE();
                                            insertGrd.GRADE = en.GradeCode;
                                            insertGrd.GRADE_NAME = en.GradeName;
                                            insertGrd.CREATE_USER_ID = pUserId;
                                            insertGrd.CREATE_DATE = DateTime.Now;
                                            insertGrd.UPDATE_USER_ID = pUserId;
                                            insertGrd.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8025_M_GRADE.Add(insertGrd);
                                        }
                                    }
                                }
                                //[PSC8026_M_SHAPE]
                                List<Shape> objShapeDistinctList = (InsertPipeItemList.GroupBy(pi => new { pi.Shape, pi.ShapeName })
                                                                       .Select(m => new Shape
                                                                       {
                                                                           ShapeCode = m.Key.Shape,
                                                                           ShapeName = m.Key.ShapeName
                                                                       })).ToList();
                                if (objShapeDistinctList != null)
                                {
                                    foreach (Shape en in objShapeDistinctList)
                                    {
                                        var objUpdate = this.db.PSC8026_M_SHAPE.SingleOrDefault(sh => sh.SHAPE == en.ShapeCode);

                                        // Check duplicate data before insert
                                        if (objUpdate == null)
                                        {
                                            PSC8026_M_SHAPE insertSha = new PSC8026_M_SHAPE();
                                            insertSha.SHAPE = en.ShapeCode;
                                            insertSha.SHAPE_NAME = en.ShapeName;
                                            insertSha.CREATE_USER_ID = pUserId;
                                            insertSha.CREATE_DATE = DateTime.Now;
                                            insertSha.UPDATE_USER_ID = pUserId;
                                            insertSha.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8026_M_SHAPE.Add(insertSha);
                                        }
                                    }
                                }
                                //[PSC8027_M_MAKER]
                                List<Maker> objMakerDistinctList = (InsertPipeItemList.GroupBy(pi => new { pi.Maker, pi.MakerName })
                                                                       .Select(m => new Maker
                                                                       {
                                                                           MakerCode = m.Key.Maker,
                                                                           MakerName = m.Key.MakerName
                                                                       })).ToList();
                                if (objMakerDistinctList != null)
                                {
                                    foreach (Maker en in objMakerDistinctList)
                                    {
                                        var objUpdate = this.db.PSC8027_M_MAKER.SingleOrDefault(mk => mk.MAKER == en.MakerCode);

                                        // Check duplicate data before insert
                                        if (objUpdate == null)
                                        {
                                            PSC8027_M_MAKER insertSha = new PSC8027_M_MAKER();
                                            insertSha.MAKER = en.MakerCode;
                                            insertSha.MAKER_NAME = en.MakerName;
                                            insertSha.CREATE_USER_ID = pUserId;
                                            insertSha.CREATE_DATE = DateTime.Now;
                                            insertSha.UPDATE_USER_ID = pUserId;
                                            insertSha.UPDATE_DATE = DateTime.Now;
                                            this.db.PSC8027_M_MAKER.Add(insertSha);
                                        }
                                    }
                                }
                                foreach (PipeItemData en in InsertPipeItemList)
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
                                    insertItem.STANDARD = en.standard;
                                    insertItem.GRADE = en.Grade;
                                    insertItem.SHAPE = en.Shape;
                                    insertItem.MAKER = en.Maker;

                                    //insertItem.TS = en.Attribute1;
                                    //insertItem.Chemical = en.Attribute2;

                                    if (!string.IsNullOrEmpty(en.Attribute3))
                                    {
                                        if (en.Attribute3.IndexOf("PT_N") > -1)
                                        {
                                            insertItem.PT370 = 0;
                                        }
                                        if (en.Attribute3.IndexOf("PT_Y") > -1)
                                        {
                                            insertItem.PT370 = 1;
                                        }
                                        if (en.Attribute3.IndexOf("PG_N") > -1)
                                        {
                                            insertItem.PG3701 = 0;
                                        }
                                        if (en.Attribute3.IndexOf("PG_Y") > -1)
                                        {
                                            insertItem.PG3701 = 1;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(en.Attribute4))
                                    {
                                        if (en.Attribute4.IndexOf("EN_N") > -1)
                                        {
                                            insertItem.EN_Spec = 0;
                                        }
                                        if (en.Attribute4.IndexOf("EN_Y") > -1)
                                        {
                                            insertItem.EN_Spec = 1;
                                        }
                                        if (en.Attribute4.IndexOf("AR_N") > -1)
                                        {
                                            insertItem.Aramco = 0;
                                        }
                                        if (en.Attribute4.IndexOf("AR_Y") > -1)
                                        {
                                            insertItem.Aramco = 1;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(en.Attribute5))
                                    {
                                        if (en.Attribute5.IndexOf("SG_N") > -1)
                                        {
                                            insertItem.Singapore = 0;
                                        }
                                        if (en.Attribute5.IndexOf("SG_Y") > -1)
                                        {
                                            insertItem.Singapore = 1;
                                        }
                                        if (en.Attribute5.IndexOf("GR_N") > -1)
                                        {
                                            insertItem.Gerab_PO = 0;
                                        }
                                        if (en.Attribute5.IndexOf("GR_Y") > -1)
                                        {
                                            insertItem.Gerab_PO = 1;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(en.Attribute7))
                                    {
                                        if (en.Attribute7.IndexOf("N") > -1)
                                        {
                                            insertItem.C21_SHL1 = 0;
                                        }
                                        if (en.Attribute7.IndexOf("Y") > -1)
                                        {
                                            insertItem.C21_SHL1 = 1;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(en.Attribute8))
                                    {
                                        insertItem.MN = Convert.ToDecimal(en.Attribute8);
                                    }

                                    if (!string.IsNullOrEmpty(en.Attribute9))
                                    {
                                        insertItem.C = Convert.ToDecimal(en.Attribute9);
                                    }

                                    insertItem.CREATE_USER_ID = pUserId;
                                    insertItem.CREATE_DATE = DateTime.Now;
                                    insertItem.UPDATE_USER_ID = pUserId;
                                    insertItem.UPDATE_DATE = DateTime.Now;

                                    this.db.PSC8010_M_PIPE_ITEM.Add(insertItem);
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
                            //tran.Dispose();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //tran.Dispose();
                    throw ex;
                }

                return result;
            }
        }
        public Boolean UpdatePipeItem1(List<PipeItemData> pPipeItemList, string pUserId)
        {
            Boolean result = false;

            var transactionOptions = new TransactionOptions();
            transactionOptions.Timeout = TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout());
            List<Material> objMaterial = new List<Material>();
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;

                        if (pPipeItemList != null && pPipeItemList.Count > 0)
                        {
                            // ดึงข้อมูลจาก db
                            List<PSC8010_M_PIPE_ITEM> objPipeItemsList = this.db.PSC8010_M_PIPE_ITEM
                                .OrderByDescending(pi => pi.CREATE_DATE)
                                .Take(3000)
                                .ToList();

                            foreach (PipeItemData en in pPipeItemList)
                            {
                                // หา PipeItem ที่ต้องการอัปเดต
                                var objUpdate = objPipeItemsList.SingleOrDefault(pi => pi.ITEM_CODE == en.ItemCode && pi.HEAT_NO == en.HeatNo);

                                if (objUpdate != null)
                                {
                                    // อัปเดตข้อมูลในฐานข้อมูล
                                    objUpdate.DESCRIPTION = en.Description;
                                    objUpdate.OD = Convert.ToDecimal(en.OD);
                                    objUpdate.WT = Convert.ToDecimal(en.WT);
                                    objUpdate.LT = Convert.ToDecimal(en.LT);
                                    objUpdate.UNIT_WEIGHT = en.UnitWeight;
                                    objUpdate.MATERIAL = en.Material;
                                    objUpdate.STANDARD = en.standard;
                                    objUpdate.GRADE = en.Grade;
                                    objUpdate.SHAPE = en.Shape;
                                    objUpdate.MAKER = en.Maker;
                                    //objUpdate.TS = en.Attribute1;
                                    //objUpdate.Chemical = en.Attribute2;
                                    objUpdate.LABEL1 = en.Label1;
                                    //objUpdate.ATTRIBUTE1 = en.Attribute1;
                                    objUpdate.LABEL2 = en.Label2; 
                                    //objUpdate.ATTRIBUTE2 = en.Attribute2;
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
                                    // อัปเดตวันที่และผู้ใช้งาน
                                    objUpdate.UPDATE_USER_ID = pUserId;
                                    objUpdate.UPDATE_DATE = DateTime.Now;

                                    this.db.Entry(objUpdate).State = EntityState.Modified;
                                }
                            }
                            List<PSC8010_M_PIPE> objPipeItem = this.db.PSC8010_M_PIPE
                                .OrderByDescending(pi => pi.CREATE_DATE)
                                .Take(100).ToList();
                            foreach (PipeItemData en in pPipeItemList)
                            {
                                var objUpdate = objPipeItem.SingleOrDefault(pi => pi.ITEM_CODE == en.ItemCode && pi.HEAT_NO == en.HeatNo);
                                if(objUpdate != null)
                                {
                                    objUpdate.UNIT_WEIGHT = en.UnitWeight;
                                    this.db.Entry(objUpdate).State = EntityState.Modified;
                                }
                            }

                            // [PSC8023_M_MATERIAL]
                            List<Material> objMateialDistinctList = (pPipeItemList.GroupBy(pi => new { pi.Material, pi.MaterialName })
                                       .Select(m => new Material
                                       {
                                           MaterialCode = m.Key.Material,
                                           MaterialName = m.Key.MaterialName
                                       })).ToList();
                            if (objMateialDistinctList != null)
                            {
                                foreach (Material en in objMateialDistinctList)
                                {
                                    // หา record ที่ต้องการอัปเดต
                                    var objUpdate = this.db.PSC8023_M_MATERIAL.SingleOrDefault(me => me.MATERIAL == en.MaterialCode);

                                    // ถ้าเจอ record ในตาราง ให้ทำการอัปเดต
                                    if (objUpdate != null)
                                    {
                                        // ตรวจสอบว่าข้อมูลมีการเปลี่ยนแปลงหรือไม่
                                        if (objUpdate.MATERIAL_NAME != en.MaterialName)
                                        {
                                            objUpdate.MATERIAL_NAME = en.MaterialName;
                                            objUpdate.UPDATE_USER_ID = pUserId;
                                            objUpdate.UPDATE_DATE = DateTime.Now;
                                            this.db.Entry(objUpdate).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            // [PSC8024_M_STANDARD]
                            List<Standard> objStandardDistinctList = (pPipeItemList.GroupBy(pi => new { pi.standard, pi.standardName })
                                                   .Select(m => new Standard
                                                   {
                                                       StandardCode = m.Key.standard,
                                                       StandardName = m.Key.standardName
                                                   })).ToList();

                            if (objStandardDistinctList != null)
                            {
                                foreach (Standard en in objStandardDistinctList)
                                {
                                    // หา record ที่ต้องการอัปเดต
                                    var objUpdate = this.db.PSC8024_M_STANDARD.SingleOrDefault(st => st.STANDARD == en.StandardCode);

                                    // ถ้าเจอ record ในตาราง ให้ทำการอัปเดต
                                    if (objUpdate != null)
                                    {
                                        // ตรวจสอบว่าข้อมูลมีการเปลี่ยนแปลงหรือไม่
                                        if (objUpdate.STANDARD_NAME != en.StandardName)
                                        {
                                            objUpdate.STANDARD_NAME = en.StandardName;
                                            objUpdate.UPDATE_USER_ID = pUserId;
                                            objUpdate.UPDATE_DATE = DateTime.Now;
                                            this.db.Entry(objUpdate).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            // [PSC8025_M_GRADE]
                            List<Grade> objGradeDistinctList = (pPipeItemList.GroupBy(pi => new { pi.Grade, pi.GradeName })
                                       .Select(m => new Grade
                                       {
                                           GradeCode = m.Key.Grade,
                                           GradeName = m.Key.GradeName
                                       })).ToList();

                            if (objGradeDistinctList != null)
                            {
                                foreach (Grade en in objGradeDistinctList)
                                {
                                    // หา record ที่ต้องการอัปเดต
                                    var objUpdate = this.db.PSC8025_M_GRADE.SingleOrDefault(gr => gr.GRADE == en.GradeCode);

                                    // ถ้าเจอ record ให้ทำการอัปเดต
                                    if (objUpdate != null)
                                    {
                                        // ตรวจสอบว่าค่ามีการเปลี่ยนแปลงหรือไม่
                                        if (objUpdate.GRADE_NAME != en.GradeName)
                                        {
                                            objUpdate.GRADE_NAME = en.GradeName;
                                            objUpdate.UPDATE_USER_ID = pUserId;
                                            objUpdate.UPDATE_DATE = DateTime.Now;
                                            this.db.Entry(objUpdate).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            // [PSC8026_M_SHAPE]
                            List<Shape> objShapeDistinctList = (pPipeItemList.GroupBy(pi => new { pi.Shape, pi.ShapeName })
                                       .Select(m => new Shape
                                       {
                                           ShapeCode = m.Key.Shape,
                                           ShapeName = m.Key.ShapeName
                                       })).ToList();

                            if (objShapeDistinctList != null)
                            {
                                foreach (Shape en in objShapeDistinctList)
                                {
                                    // หา record ที่ต้องการอัปเดต
                                    var objUpdate = this.db.PSC8026_M_SHAPE.SingleOrDefault(sh => sh.SHAPE == en.ShapeCode);

                                    // ถ้าเจอ record ให้ทำการอัปเดต
                                    if (objUpdate != null)
                                    {
                                        // ตรวจสอบว่าข้อมูลมีการเปลี่ยนแปลงหรือไม่
                                        if (objUpdate.SHAPE_NAME != en.ShapeName)
                                        {
                                            objUpdate.SHAPE_NAME = en.ShapeName;
                                            objUpdate.UPDATE_USER_ID = pUserId;
                                            objUpdate.UPDATE_DATE = DateTime.Now;
                                            this.db.Entry(objUpdate).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            // [PSC8027_M_MAKER]
                            List<Maker> objMakerDistinctList = (pPipeItemList.GroupBy(pi => new { pi.Maker, pi.MakerName })
                                       .Select(m => new Maker
                                       {
                                           MakerCode = m.Key.Maker,
                                           MakerName = m.Key.MakerName
                                       })).ToList();

                            if (objMakerDistinctList != null)
                            {
                                foreach (Maker en in objMakerDistinctList)
                                {
                                    // หา record ที่ต้องการอัปเดต
                                    var objUpdate = this.db.PSC8027_M_MAKER.SingleOrDefault(mk => mk.MAKER == en.MakerCode);

                                    // ถ้าเจอ record ให้ทำการอัปเดต
                                    if (objUpdate != null)
                                    {
                                        // ตรวจสอบว่าข้อมูลมีการเปลี่ยนแปลงหรือไม่
                                        if (objUpdate.MAKER_NAME != en.MakerName)
                                        {
                                            objUpdate.MAKER_NAME = en.MakerName;
                                            objUpdate.UPDATE_USER_ID = pUserId;
                                            objUpdate.UPDATE_DATE = DateTime.Now;
                                            this.db.Entry(objUpdate).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            /*List<StockList> objStockDistincList = (pPipeItemList.GroupBy(pi => new { pi.ItemCode, pi.HeatNo })
                                                                .Select(m => new StockList
                                                                {
                                                                    ItemCode = m.Key.ItemCode,
                                                                    HeatNo = m.Key.HeatNo,
                                                                })).ToList();
                            if (objStockDistincList.Any())
                            {
                                foreach (StockList en in objStockDistincList)
                                {
                                    var objUpdate = this.db.PSC2010_T_STOCK.FirstOrDefault(st => st.ITEM_CODE == en.ItemCode && st.HEAT_NO == en.HeatNo);
                                    if (objUpdate == null)
                                    {
                                        PSC2010_T_STOCK insertStock = new PSC2010_T_STOCK();
                                        insertStock.ITEM_CODE = en.ItemCode;
                                        insertStock.HEAT_NO = en.HeatNo;
                                        insertStock.YEAR_MONTH = new DateTime(2019, 4, 1, 0, 0, 0);
                                        this.db.PSC2010_T_STOCK.Add(insertStock);
                                    }
                                }
                            }*/
                            // Save Changes
                            this.db.SaveChanges();
                        }

                        tran.Complete();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    // จับข้อผิดพลาดใน SaveChanges หรืออื่น ๆ
                    Console.WriteLine("Error: " + ex.Message);
                    throw new ApplicationException("Error updating pipe items", ex);
                }
            }

            return result;
        }
        #endregion
    }
}