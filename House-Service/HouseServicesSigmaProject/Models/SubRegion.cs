using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HouseServicesSigmaProject.Models
{
    public class SubRegion
    {
        [Key]
        public int SubRegionId { get; set; }

        [Required]
        [Display(Name= "Название района")]
        public string Name { get; set; }
               
         public List<Street> Streets { get; set; }
        [Required]
        [Display(Name = "Пароль")]
         public string Password { get; set; }
        

     

    }
}