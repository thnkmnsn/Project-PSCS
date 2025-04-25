using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSCS.Services
{
    public class GradeService
    {
        private PSCSEntities db;

        public GradeService(PSCSEntities pDb)
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

        public List<Grade> GetGradeList()
        {
            List<Grade> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from pi in db.PSC8025_M_GRADE
                          orderby pi.GRADE_NAME
                          select new Grade
                          {
                              GradeCode = pi.GRADE,
                              GradeName = pi.GRADE_NAME

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