using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivlogDI.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly LivlogDIContext _dbContext;

        public CustomerRepository(
            LivlogDIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Customer> GetAll()
        {
            return _dbContext.Customers
                .Include(c => c.CustomerBooks)
                .OrderByDescending(b => b.Id)
                .ToList();
        }

        public Customer Get(int id)
        {
            return _dbContext.Customers
                .Include(c => c.CustomerBooks)
                .Where(b => b.Id == id)
                .SingleOrDefault()
                    ?? throw new ArgumentException();
        }

        public Customer Add(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();

            customer = Get(customer.Id);

            return customer;
        }

        public Customer Update(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            _dbContext.SaveChanges();

            customer = Get(customer.Id);

            return customer;
        }

        public bool Delete(int id)
        {
            var customer = Get(id);

            if (customer is null)
            {
                throw new ArgumentException();
            }

            _dbContext.Remove(customer);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
