using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace HouseServicesSigmaProject.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {
            
        }

        public DbSet<SubRegion> SubRegions { get; set; }

        public DbSet<Street> Streets { get; set; }

        public DbSet<House> Houses { get; set; }

        public DbSet<Porch> Porches { get; set; }

        public DbSet<Apartment> Apartments { get; set; }
    }
}