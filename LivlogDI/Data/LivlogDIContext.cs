using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivlogDI.Data
{
    public class LivlogDIContext : DbContext
    {
        public LivlogDIContext()
        {
        }

        public LivlogDIContext (DbContextOptions<LivlogDIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; } = default!;
    }
}
