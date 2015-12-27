using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HouseServicesSigmaProject.Models
{
    public class HelpForHouse
    {
        public List<SelectListItem> RegionList { get; set; }

        public List<SelectListItem> StreetList { get; set; }
    }
}