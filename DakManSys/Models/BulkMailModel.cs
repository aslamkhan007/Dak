using DakManSys.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DakManSys.Models
{
    public class BulkMailModel
    {
        public BulkMailModel()
        {
            Jct_Dak_Register = new Jct_Dak_Register();
        }
        public string Hod_Name { get; set; }
        public string MailCC { get; set; }
        public string Hod_Code { get; set; }
        public string MailTo { get; set; }
        public int? Count { get; set; }
        public Jct_Dak_Register Jct_Dak_Register { set; get; }
        public string InwardNo { set; get; }

        public string deptCode { set; get; }
    }
}