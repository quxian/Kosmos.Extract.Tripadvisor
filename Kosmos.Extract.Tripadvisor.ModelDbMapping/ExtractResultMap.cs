using Kosmos.Extract.tripadvisor.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kosmos.Extract.Tripadvisor.ModelDbMapping
{
    public class ExtractResultMap : EntityTypeConfiguration<ExtractResult>
    {
        public ExtractResultMap()
        {
            this.HasKey(x => x.HashCode);
            this.Property(x => x.HashCode)
                .HasMaxLength(32);
        }
    }
}
