using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HouseServicesSigmaProject.Models
{
    public class Apartment
    {
        [Key]
        public int ApartmentId { get; set; }
        [Required]
        [Display(Name = "Номер квартиры")]
        public int ApartmentNumber { get; set; }
        [Required]
        [Display(Name = "Электронная почта")]
        public string OwnerTelephoneNumber { get; set; }
        
        public Porch porch { get;set; }

        public int PorchId { get; set; }

      
    }
}