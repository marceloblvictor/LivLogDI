using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface ICustomerBookRepository
    {
        CustomerBook Add(CustomerBook customerBook);
        bool Delete(int id);
        CustomerBook Get(int id);
        List<CustomerBook> GetAll();
        CustomerBook Update(CustomerBook customerBook);
    }
}