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
        public DbSet<Procedure> Procedure { get; set; }
        public DbSet<Laboratories> Laboratories { get; set;}
        public DbSet<TypeAnalysisTests> TypeAnalysisTests { get; set; }
        public DbSet<Vacancies> Vacancies { get; set; }
        public DbSet<Schedules> Schedules { get; set; }
        public DbSet<Checklist> Checklist { get; set; }
    }
}
