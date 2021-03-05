using DakManSys.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DakManSys.ViewModel
{
    public class GridViewModel
    {
        public GridViewModel()
        {
            Jct_Dak_Register = new Jct_Dak_Register();
            Jct_Dak_Register_Recieved = new Jct_Dak_Register_Recieved();
            Jct_Dak_Party_Header = new Jct_Dak_Party_Header();
            Jct_Dak_Party_Detail_Address = new Jct_Dak_Party_Detail_Address();
            Jct_Dak_Register_Cheque = new Jct_Dak_Register_Cheque();
            JCT_EMP_HOD = new JCT_EMP_HOD();
        }
        public Jct_Dak_Register Jct_Dak_Register { set; get; }
        public Jct_Dak_Register_Recieved Jct_Dak_Register_Recieved { set; get; }
        public Jct_Dak_Party_Header Jct_Dak_Party_Header { set; get; }
        public Jct_Dak_Party_Detail_Address Jct_Dak_Party_Detail_Address { set; get; }
        public JCT_EMP_HOD JCT_EMP_HOD { set; get; }
        public Jct_Dak_Register_Cheque Jct_Dak_Register_Cheque { set; get; }

        //Extra entries
        public string partyCode { set; get; }
        public string partyName { set; get; }

        public string partyCity { get; set; }

    }
}