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
        public virtual DbSet<Customer> Customers { get; set; } = default!;
        public virtual DbSet<CustomerBook> CustomerBooks { get; set; } = default!;
        public virtual DbSet<Fine> Fines { get; set; } = default!;
        public virtual DbSet<User> Users { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerBook>()
                .HasKey(cb => cb.Id);

            modelBuilder.Entity<CustomerBook>()
                .HasOne(cb => cb.Customer)
                    .WithMany(c => c.CustomerBooks)
                .HasForeignKey(cb => cb.CustomerId);

            modelBuilder.Entity<CustomerBook>()
                .HasOne(cb => cb.Book)
                    .WithMany(b => b.CustomerBooks)
                .HasForeignKey(cb => cb.BookId);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Customer)
                    .WithMany(c => c.Fines)
                .HasForeignKey(f => f.CustomerId);
        }
    }
}
