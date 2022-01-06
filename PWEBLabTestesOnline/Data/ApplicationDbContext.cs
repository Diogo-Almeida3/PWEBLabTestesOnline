using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using PWEBLabTestesOnline.Models;

namespace PWEBLabTestesOnline.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AnalysisTests> AnalysisTests { get; set; }
        public DbSet<Procedure> Procedure { get; set; }
        public DbSet<TypeAnalysisTests> TypeAnalysisTests { get; set; }
        public DbSet<Laboratories> Laboratories { get; set;}
    }
}
