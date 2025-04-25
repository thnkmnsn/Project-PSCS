using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Services
{
    public class ShapeService
    {
        private PSCSEntities db;

        public ShapeService(PSCSEntities pDb)
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

        public List<Shape> GetShapeList()
        {
            List<Shape> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from pi in db.PSC8026_M_SHAPE
                          orderby pi.SHAPE_NAME
                          select new Shape
                          {
                              ShapeCode = pi.SHAPE,
                              ShapeName = pi.SHAPE_NAME

                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}