//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace A4BusService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tripStop
    {
        public int tripStopId { get; set; }
        public int tripId { get; set; }
        public int busStopNumber { get; set; }
        public System.TimeSpan tripStopTime { get; set; }
        public string comments { get; set; }
    
        public virtual busStop busStop { get; set; }
        public virtual trip trip { get; set; }
    }
}
