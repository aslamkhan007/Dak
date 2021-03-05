using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DakManSys.Models.MasterModel
{
    public class DakServiceModel
    {
        public string DakService_Name { set; get; }
        public string ServiceMode_Code {set;get;}
        public string Description { set; get; }
        public string Address_Line_1 { set; get; }
        public string Address_Line_2 { set; get; }
        public string Address_line_3 { set; get; }
        public string city { set; get;}
        public string ZipCode { set; get; }
        public string Country { set; get; }
        public string State { set; get; }
        public string PhoneNo { set; get; }
        public string Email_Address { set; get; }
        public string Office_Number { set; get; }
        public string Fax_Number { set; get; }
        public string Website { set; get; }

    }
}