using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrgDAL
{
    public class OrganizationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-LKQLTDG;Database=OrgAPIDb;Trusted_Connection=True;MultipleActiveResultSets=True");
        }

        public DbSet<Department> Departments { get; set; }
    }
}
