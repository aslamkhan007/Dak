using DakManSys.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.ViewModel
{
    public class DropDownViewModel
    {
        public DropDownViewModel()
        {
            Jct_Dak_Cities = new Jct_Dak_Cities();
            Jct_Dak_Country = new Jct_Dak_Country();
            Jct_Dak_State = new Jct_Dak_State();
            Jct_Dak_Itemtype = new Jct_Dak_Itemtype();
            Jct_Dak_BankMaster = new Jct_Dak_BankMaster();
        }
        public Jct_Dak_Cities Jct_Dak_Cities { set; get; }
        public Jct_Dak_Country Jct_Dak_Country { set; get; }
        public Jct_Dak_State Jct_Dak_State { set; get; }
        public Jct_Dak_Itemtype Jct_Dak_Itemtype { set; get; }

        public Jct_Dak_BankMaster Jct_Dak_BankMaster { set; get; }

        //SelectListItem propertises
        public List<SelectListItem> DakType { set; get; }
        public List<SelectListItem> MailMode { set; get; }
        public List<SelectListItem> MailService { set; get; }
        public List<SelectListItem> CountryList { set; get; }

        public List<SelectListItem> StateList { set; get; }
        public List<SelectListItem> CityList { set; get; }
        public List<SelectListItem> HodList { set; get; }
        public List<SelectListItem> DeptList { set; get; }
        public List<SelectListItem> BankList { set; get; }
        public List<SelectListItem> DakServiceModeList { set; get; }
    }
}