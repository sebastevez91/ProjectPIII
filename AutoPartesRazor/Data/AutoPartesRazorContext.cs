using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Data
{
    public class AutoPartesRazorContext : DbContext
    {
        public AutoPartesRazorContext (DbContextOptions<AutoPartesRazorContext> options)
            : base(options)
        {
        }

        public DbSet<AutoPartesRazor.Models.Product> Product { get; set; } = default!;

        public DbSet<AutoPartesRazor.Models.Brand>? Brand { get; set; }

        public DbSet<AutoPartesRazor.Models.Category>? Category { get; set; }

        public DbSet<AutoPartesRazor.Models.Client>? Client { get; set; }
    }
}
