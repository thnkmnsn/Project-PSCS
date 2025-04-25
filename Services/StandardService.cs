using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Services
{
    public class StandardService
    {
        private PSCSEntities db;

        public StandardService(PSCSEntities pDb)
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

        public List<Standard> GetStandardList()
        {
            List<Standard> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from pi in db.PSC8024_M_STANDARD
                          orderby pi.STANDARD_NAME 
                          select new Standard
                          {
                              StandardCode = pi.STANDARD,
                              StandardName = pi.STANDARD_NAME

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