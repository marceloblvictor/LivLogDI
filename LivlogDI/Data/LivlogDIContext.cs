using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivlogDI.Data
{
    public class LivlogDIContext : DbContext, ILivlogDIContext
    {
        public LivlogDIContext()
        {
        }

        public LivlogDIContext(DbContextOptions<LivlogDIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; } = default!;
        public virtual DbSet<User> Users { get; set; } = default!;
    }
}
