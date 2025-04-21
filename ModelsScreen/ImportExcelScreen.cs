using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSCS.ModelsScreen
{
    public class ImportExcelScreen : BaseScreen
    {
        [DisplayName("ImportFile")]
        public HttpPostedFileBase ImportFile { get; set; }
    }
}