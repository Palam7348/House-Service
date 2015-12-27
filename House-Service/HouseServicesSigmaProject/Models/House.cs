using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HouseServicesSigmaProject.Models
{
    public class House
    {
        [Key]
        public int HouseId { get; set; }
        [Required]
        [Display(Name = "Номер дома")]
        public int HouseNumber { get; set; }
        
        public List<Porch> Porches { get; set; }

        public Street street { get; set; }

        public int StreetId { get; set; }

    
    }
}