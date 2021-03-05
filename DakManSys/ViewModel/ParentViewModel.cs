using DakManSys.Entity;
using DakManSys.Models.MasterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.ViewModel
{
    public class ParentViewModel
    {
        public ParentViewModel()
        {
            Jct_Dak_Party_Header = new Jct_Dak_Party_Header();
            Jct_Dak_Party_Detail_Address = new Jct_Dak_Party_Detail_Address();
            Jct_Dak_Register = new Jct_Dak_Register();
            Jct_Dak_Register_Cheque = new Jct_Dak_Register_Cheque();
            Jct_Dak_Service_Details = new Jct_Dak_Service_Details();
            Jct_Dak_Service_Mode = new Jct_Dak_Service_Mode();
            Jct_Dak_Type_Sr_No = new Jct_Dak_Type_Sr_No();
            JCT_EMP_HOD = new JCT_EMP_HOD();
            Jct_Dak_Party_Sr_No = new Jct_Dak_Party_Sr_No();
            DropDownViewModel = new DropDownViewModel();
            DakServiceModel = new DakServiceModel();
        }

        public Jct_Dak_Party_Header Jct_Dak_Party_Header { set; get; }
        public Jct_Dak_Party_Detail_Address Jct_Dak_Party_Detail_Address { set; get; }
        public Jct_Dak_Register Jct_Dak_Register { set; get; }
        public Jct_Dak_Register_Cheque Jct_Dak_Register_Cheque { set; get; }
        public Jct_Dak_Service_Details Jct_Dak_Service_Details { set; get; }
        public Jct_Dak_Service_Mode Jct_Dak_Service_Mode { set; get; }
        public Jct_Dak_Type_Sr_No Jct_Dak_Type_Sr_No { set; get; }
        public JCT_EMP_HOD JCT_EMP_HOD { set; get; }
        public DropDownViewModel DropDownViewModel { set; get; }
        public Jct_Dak_Party_Sr_No Jct_Dak_Party_Sr_No { set; get; }
        public Jct_Dak_Itemtype Jct_Dak_Itemtype { set; get; }




        //Master Model//
        public DakServiceModel DakServiceModel { set; get; }
        //----------//
        //Extra Entries
        public DateTime EntryDate { set; get; }
        public string RadioValue { set; get; }
        public string Sr_No { set; get; }
        public Char dakType { set; get; }
        public string displayDate { set; get; }
        public string AddressID { get; set; }

    }
}