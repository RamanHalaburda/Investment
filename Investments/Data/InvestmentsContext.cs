using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Investments.Models
{
    public class InvestmentsContext : DbContext
    {
        public InvestmentsContext (DbContextOptions<InvestmentsContext> options)
            : base(options)
        {
        }

        public DbSet<Investments.Models.Optimazation> Optimazation { get; set; }
    }
}
