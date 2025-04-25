using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace PSCS.Services
{
    public class YardService
    {
        private PSCSEntities db;

        public YardService(PSCSEntities pDb)
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

        public SelectList GetYard()
        {
            SelectList result = null;

            try
            {
                using (this.db)
                {
                    List<PSC8022_M_YARD> getYard = db.PSC8022_M_YARD.ToList();
                    result = new SelectList(getYard, "YARD", "NAME");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Yard> GetYardList(string pPlaceID = "")
        {
            List<Yard> result = new List<Yard>();

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                var obj = (from yd in db.PSC8022_M_YARD
                            orderby yd.YARD
                            select new Yard
                            {
                                Place = yd.PLACE,
                                YardID = yd.YARD,
                                Name = yd.NAME,
                                CreateDate = yd.CREATE_DATE,
                                CreateUserID = yd.CREATE_USER_ID,
                                UpdateDate = yd.UPDATE_DATE,
                                UpdateUserID = yd.UPDATE_USER_ID
                            }).AsQueryable();
       
                if (!string.IsNullOrEmpty(pPlaceID))
                {
                    //var pa = db.PSC8021_M_PLACE.Where(m => m.NAME.Equals(pPlaceID)).FirstOrDefault().PLACE;
                    obj = obj.Where(x => x.Place == pPlaceID);
                }

                result = obj.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}