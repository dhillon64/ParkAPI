using Microsoft.EntityFrameworkCore;
using ParkiAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ParkyAPI.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options): base(options)
        {
          
        }

        public DbSet<NationalPark> NationalParks { get; set; } 
    }
}
