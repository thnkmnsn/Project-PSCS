using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Services
{
    public class MaterialService
    {
        private PSCSEntities db;

        public MaterialService(PSCSEntities pDb)
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

        public List<Material> GetMaterialList()
        {
            List<Material> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from pi in db.PSC8023_M_MATERIAL
                          orderby pi.MATERIAL_NAME
                          select new Material
                          {
                              MaterialCode = pi.MATERIAL,
                              MaterialName = pi.MATERIAL_NAME

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