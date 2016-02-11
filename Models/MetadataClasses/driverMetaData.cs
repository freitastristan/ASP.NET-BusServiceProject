using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A4BusService.Models
{
    [MetadataType(typeof(driverMetaData))]
    public partial class driver
    {

    }
    public class driverMetaData
    {
        [Display(Name="Driver ID")]
        public int driverId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public byte[] lastName { get; set; }
        public string fullName { get; set; }
        public string homePhone { get; set; }
        public string workPhone { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        [Display(Name = "Province")]
        public string postalCode { get; set; }
        public string provinceCode { get; set; }
        [DisplayFormat(DataFormatString="{0:dd MMM yyyy}",
            ApplyFormatInEditMode=true)]
        public System.DateTime dateHired { get; set; }
    }
}