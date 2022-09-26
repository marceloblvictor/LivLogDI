using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface ILivlogDIContext
    {
        DbSet<Book> Books { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<CustomerBook> CustomerBooks { get; set; }
        DbSet<Fine> Fines { get; set; }
    }
}