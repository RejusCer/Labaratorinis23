//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Labaratorinis23.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ModuleUser
    {
        public int ID { get; set; }
        public int userID { get; set; }
        public int moduleID { get; set; }
        public Nullable<int> grade { get; set; }
    
        public virtual Module Module { get; set; }
        public virtual User User { get; set; }
    }
}