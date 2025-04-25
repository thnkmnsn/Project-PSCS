using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;
using System.Web.Mvc;
using PSCS.Common;
using System.Transactions;
using System.Text;
using PSCS.ModelsScreen;

namespace PSCS.Services
{
    public class LocationService
    {
        // Attribute 
        private PSCSEntities db;

        // Constructor 
        public LocationService(PSCSEntities pDb)
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
        public SelectList GetLocation(string pYardID)
        {
            SelectList result = null;
            List<PSC8020_M_LOCATION> objLocationAll = null;
            List<PSC8020_M_LOCATION> objLocation = null;

            try
            {
                using (this.db)
                {
                    objLocationAll = db.PSC8020_M_LOCATION.ToList();

                    objLocation = objLocationAll.Where(m => m.YARD == pYardID).ToList();
                    result = new SelectList(objLocation, "LOCATION", "LOCATION_CODE", 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public LocationEditScreen GetLocationById(string location_code)
        {
            LocationEditScreen result = new LocationEditScreen();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = this.db.PSC8020_M_LOCATION.SingleOrDefault(x => x.LOCATION_CODE == location_code);

                    result = new LocationEditScreen()
                    {
                        InputPlace = obj.PLACE,
                        InputYard = obj.YARD,
                        InputLocation = obj.LOCATION_CODE,
                        InputName = obj.NAME,
                        InputActive = obj.ACTIVE.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Location> GetLocationList(string pYardID)
        {
            List<Location> result = new List<Location>();

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    result = db.PSC8020_M_LOCATION
                        .Where(m => m.YARD == pYardID && m.LOCATION_CODE != Constants.LocationCodeRelease)
                        .AsEnumerable()
                        .Select((x, index) => new Location
                        {
                            RowNo = index + 1,
                            Place = x.PLACE,
                            Yard = x.YARD,
                            LocationID = x.LOCATION,
                            Name = x.NAME,
                            LocationCode = x.LOCATION_CODE,
                            Active = x.ACTIVE,
                            ActiveName = GetActive(x.ACTIVE)
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Location GetLocationByLocationCode(string pLocationCode)
        {
            Location result = null;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from lo in db.PSC8020_M_LOCATION
                               join yd in db.PSC8022_M_YARD on lo.YARD equals yd.YARD
                               where lo.LOCATION_CODE == pLocationCode 
                               select new
                               {
                                   lo.LOCATION_CODE,
                                   lo.PLACE,
                                   lo.YARD,
                                   lo.LOCATION,
                                   lo.NAME,
                                   YARD_NAME = yd.NAME,
                               }).SingleOrDefault();

                    if(obj != null)
                    {
                        result = new Location()
                        {
                            Yard = obj.YARD,
                            YardName = obj.YARD_NAME,
                            LocationID = obj.LOCATION,
                            LocationCode = obj.LOCATION_CODE,
                            Name = obj.NAME,
                        };
                    }
                    else
                    {
                        result = new Location();
                        result.YardName = "NG";
                        result.Name = "NG";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public List<Location> GetLocationList(string pPlaceID, string pYardID, string pName, List<byte> pActive)
        {
            List<Location> result = new List<Location>();
            int cnt = 0;

            try
            {
                using (this.db)
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var obj = (from lo in db.PSC8020_M_LOCATION
                               join pa in db.PSC8021_M_PLACE
                                 on lo.PLACE equals pa.PLACE
                               join yd in db.PSC8022_M_YARD
                                 on new { yard = lo.YARD, place = pa.PLACE } equals
                                    new { yard = yd.YARD, place = yd.PLACE }
                               orderby lo.PLACE,lo.YARD,lo.LOCATION_CODE
                               select new
                               {
                                   lo.PLACE,
                                   PLACE_NAME = pa.NAME,
                                   lo.YARD,
                                   YARD_NAME = yd.NAME,
                                   lo.LOCATION,
                                   LOCATION_NAME = lo.NAME,
                                   lo.ACTIVE
                               }).AsQueryable();

                    cnt = obj.Count();

                    // Place
                    if (!string.IsNullOrEmpty(pPlaceID))
                    {
                        obj = obj.Where(x => x.PLACE == pPlaceID);
                    }

                    cnt = obj.Count();

                    // Yard
                    if (!string.IsNullOrEmpty(pYardID))
                    {
                        obj = obj.Where(x => x.YARD == pYardID);
                    }

                    cnt = obj.Count();

                    // Location
                    if (!string.IsNullOrEmpty(pName))
                    {
                        obj = obj.Where(x => x.LOCATION_NAME.Contains(pName));
                    }

                    cnt = obj.Count();

                    // Active
                    obj = obj.Where(x => pActive.Contains(x.ACTIVE));

                    cnt = obj.Count();

                    // Convert into location viewmodel list
                    result = obj.AsEnumerable().Select((x, index) => new Location
                    {
                        RowNo = index + 1,
                        Place = x.PLACE,
                        PlaceName = x.PLACE_NAME,
                        Yard = x.YARD,
                        YardName = x.YARD_NAME,
                        LocationID = x.LOCATION,
                        Name = x.LOCATION_NAME,
                        LocationCode = x.LOCATION,
                        Active = x.ACTIVE,
                        ActiveName = GetActive(x.ACTIVE)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
     

        public bool InsertData(LocationEditScreen editModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        this.db.Configuration.LazyLoadingEnabled = false;
                        DateTime insertDate = DateTime.Now;

                        var obj = this.db.PSC8020_M_LOCATION.SingleOrDefault(x => x.LOCATION_CODE == editModel.InputLocation);

                        // Check duplicate data before insert
                        if (obj == null)
                        {
                            PSC8020_M_LOCATION insert = new PSC8020_M_LOCATION();
                            insert.PLACE = editModel.InputPlace;
                            insert.YARD = editModel.InputYard;
                            insert.LOCATION = editModel.InputLocation;
                            insert.LOCATION_CODE = editModel.InputLocation;
                            insert.NAME = editModel.InputName;
                            insert.ACTIVE = Convert.ToByte(editModel.InputActive);
                            insert.DISPLAY_ORDER = 1;
                            insert.CREATE_DATE = insertDate;
                            insert.CREATE_USER_ID = userId;
                            insert.UPDATE_DATE = insertDate;
                            insert.UPDATE_USER_ID = userId;

                            this.db.PSC8020_M_LOCATION.Add(insert);
                        }
                   
                        int result = this.db.SaveChanges();

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
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }


        public bool UpdateData(LocationEditScreen editModel, string userId)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        DateTime updateDate = DateTime.Now;

                        var update = this.db.PSC8020_M_LOCATION.SingleOrDefault(x => x.LOCATION_CODE == editModel.InputLocation);

                        if (update != null)
                        {
                            update.PLACE = editModel.InputPlace;
                            update.YARD = editModel.InputYard;
                            update.NAME = editModel.InputName;
                            update.ACTIVE = Convert.ToByte(editModel.InputActive);
                            update.UPDATE_USER_ID = userId;
                            update.UPDATE_DATE = updateDate;
                        }

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
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }


        public bool DeleteData(string pLocation)
        {
            int result = 0;
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(Common.Common.GetTransactionTimeout())))
            {
                try
                {
                    using (this.db)
                    {
                        db.Configuration.LazyLoadingEnabled = false;

                        // Get data from PSC8020_M_LOCATION
                        var objLocation = this.db.PSC8020_M_LOCATION.SingleOrDefault(x => x.LOCATION_CODE == pLocation);


                        if (objLocation != null)
                        {
                            var objStockTaking = this.db.PSC2050_T_STOCKTAKING_INSTRUCTION.Where(x => x.LOCATION_CODE == pLocation);
                            var objStockDetail = this.db.PSC2011_T_STOCK_DETAIL.Where(x => x.LOCATION_CODE == pLocation);
                            var objStock = this.db.PSC2010_T_STOCK.Where(x => x.LOCATION_CODE == pLocation);

                            if (objStockTaking.Count() != 0 || objStockDetail.Count() != 0 || objStock.Count() != 0)
                            {
                                result = 0;
                            }
                            else
                            {
                                db.PSC8020_M_LOCATION.Remove(objLocation);
                                result = db.SaveChanges();
                            }
                        }

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
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }
        #endregion

        #region Private
        private string GetActive(byte pActive)
        {
            string result = string.Empty;
            if (pActive == 1)
            {
                result = Resources.Common_cshtml.Active;
            }
            else if (pActive == 2)
            {
                result = Resources.Common_cshtml.InActive;
            }
            else
            {
                result = "";
            }
            return result;
        }
        #endregion
    }
}