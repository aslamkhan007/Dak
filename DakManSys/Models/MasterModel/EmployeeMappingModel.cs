using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.Models.MasterModel
{
    public class EmployeeMappingModel
    {
        public string EmployeeCode { set; get; }
        public string EmployeeName { set; get; }
        public string Department { set; get; }
        public string MobileNo {set;get;}
        public string Email { set; get; }


        public List<SelectListItem> Deptlist { set; get; }
    }
}