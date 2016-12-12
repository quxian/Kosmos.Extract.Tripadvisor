using Kosmos.Extract.tripadvisor.Model;
using Kosmos.Extract.Tripadvisor.ModelDbMapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kosmos.Extract.Tripadvisor.DbContext
{
    public class AppDbContext : System.Data.Entity.DbContext
    {
        public AppDbContext() : base("TripadvisorDbConnection")
        {

        }

        public DbSet<ExtractResult> ExtractResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ExtractResultMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
