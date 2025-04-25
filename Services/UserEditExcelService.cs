using PSCS.Common;
using PSCS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;

namespace PSCS.Services
{
    public class UserEditExcelService
    {
        private PSCSEntities _db;
        private User _user;
        public List<string> ErrorMessages = new List<string>();

        public UserEditExcelService(PSCSEntities db, User user)
        {
            _db = db;
            _user = user;
        }

        public List<PSC2050_T_STOCKTAKING_INSTRUCTION> GetData()
        {
            using (_db)
            {
                _db.Configuration.LazyLoadingEnabled = false;

                var psc2510 = _db.PSC2050_T_STOCKTAKING_INSTRUCTION
                            .Select(x => x)
                            //.Where(x => x.STOCKTAKING_DATE == date)
                            .OrderBy(x => x.LOCATION_CODE).ThenBy(x => x.ITEM_CODE);

                if (psc2510 == null)
                {
                    return new List<PSC2050_T_STOCKTAKING_INSTRUCTION>();
                }
                else
                {
                    return psc2510.ToList();
                }
            }
        }

    }
}