using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PSCS.Models;

namespace PSCS.Services
{
    public class DatabaseService
    {
        public PSCSEntities Connect(string pConnectionString)
        {
            PSCSEntities result = null;

            result = new PSCSEntities();
            result.Database.Connection.ConnectionString = pConnectionString;

            return result;
        }
    }
}