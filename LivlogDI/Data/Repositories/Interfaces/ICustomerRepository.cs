using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Customer Add(Customer customer);
        bool Delete(int id);
        Customer Get(int id);
        List<Customer> GetAll();
        Customer Update(Customer customer);
    }
}