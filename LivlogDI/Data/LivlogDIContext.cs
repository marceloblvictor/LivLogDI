using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LivlogDI.Models.Entities;

namespace LivlogDI.Data
{
    public class LivlogDIContext : DbContext
    {
        public LivlogDIContext (DbContextOptions<LivlogDIContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = default!;
    }
}
