//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DakManSys.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Jct_Dak_Itemtype
    {
        public int TransNo { get; set; }
        public string Dak_Code { get; set; }
        public string DakType { get; set; }
        public string Description { get; set; }
        public string Empcode { get; set; }
        public Nullable<System.DateTime> Created_On { get; set; }
        public string Created_Hostname { get; set; }
        public string Ip_Address { get; set; }
        public Nullable<System.DateTime> EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
    }
}
