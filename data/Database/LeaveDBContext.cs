using data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace data.Database
{   
    public class LeaveDBContext:DbContext
    {
        public LeaveDBContext(DbContextOptions<LeaveDBContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
                        
        }



        public DbSet<Employee> Employees { get; set; }
        public DbSet<Leave> Leaves { get; set; }
    }
}