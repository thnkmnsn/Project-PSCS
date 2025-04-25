using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Services
{
    public class MakerService
    {
        private PSCSEntities db;

        public MakerService(PSCSEntities pDb)
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

        public List<Maker> GetMakerList()
        {
            List<Maker> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from pi in db.PSC8027_M_MAKER
                          orderby pi.MAKER_NAME
                          select new Maker
                          {
                              MakerCode = pi.MAKER,
                              MakerName = pi.MAKER_NAME

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