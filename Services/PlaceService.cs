using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;

namespace PSCS.Services
{
    public class PlaceService
    {
        private PSCSEntities db;

        public PlaceService(PSCSEntities pDb)
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

        public SelectList GetPlace()
        {
            SelectList result = null;

            try
            {
                using (this.db)
                {
                    List<PSC8021_M_PLACE> getYard = db.PSC8021_M_PLACE.ToList();
                    result = new SelectList(getYard, "PLACE", "NAME");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //public List<Place> GetPlaceList()
        //{
        //    List<Place> result = null;

        //    try
        //    {
        //        using (this.db)
        //        {
        //            result = db.PSC8021_M_PLACE.Select(p => new Place
        //            {
        //                PlaceID = p.PLACE,
        //                Name = p.NAME,
        //                CreateDate = p.CREATE_DATE,
        //                CreateUserID = p.CREATE_USER_ID,
        //                UpdateDate = p.UPDATE_DATE,
        //                UpdateUserID = p.UPDATE_USER_ID,
        //            }).ToList();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}


        public List<Place> GetPlaceList()
        {
            List<Place> result = null;

            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                result = (from pa in db.PSC8021_M_PLACE
                          orderby pa.NAME
                          select new Place
                          {
                              PlaceID = pa.PLACE,
                              Name = pa.NAME,
                              CreateDate = pa.CREATE_DATE,
                              CreateUserID = pa.CREATE_USER_ID,
                              UpdateDate = pa.UPDATE_DATE,
                              UpdateUserID = pa.UPDATE_USER_ID
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