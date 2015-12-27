using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HouseServicesSigmaProject.Models
{
    public class Street
    {
        [Key]
        public int StreetId { get; set; }

        
        [Display(Name = "Название улицы")]
        public string Name { get; set; }
        
        public List<House> Houses { get; set; }
        
        public SubRegion subRegion { get; set; }

        public int SubRegionId { get; set; }


    }
}