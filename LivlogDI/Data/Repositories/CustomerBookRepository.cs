using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivlogDI.Data.Repositories
{
    public class CustomerBookRepository : ICustomerBookRepository
    {
        private readonly LivlogDIContext _dbContext;

        public CustomerBookRepository(
            LivlogDIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CustomerBook> GetAll()
        {
            return _dbContext.CustomerBooks
                .Include(cb => cb.Book)
                .Include(cb => cb.Customer)
                .OrderByDescending(b => b.Id)
                .ToList();
        }

        public CustomerBook Get(int id)
        {
            return _dbContext.CustomerBooks
                .Include(cb => cb.Book)
                .Include(cb => cb.Customer)
                .Where(b => b.Id == id)
                .SingleOrDefault()
                    ?? throw new ArgumentException();
        }

        public CustomerBook Add(CustomerBook customerBook)
        {
            _dbContext.CustomerBooks.Add(customerBook);
            _dbContext.SaveChanges();

            customerBook = Get(customerBook.Id);

            return customerBook;
        }

        public CustomerBook Update(CustomerBook customerBook)
        {
            _dbContext.CustomerBooks.Update(customerBook);
            _dbContext.SaveChanges();

            customerBook = Get(customerBook.Id);

            return customerBook;
        }

        public bool Delete(int id)
        {
            var customerBook = Get(id);            

            _dbContext.CustomerBooks.Remove(customerBook);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
