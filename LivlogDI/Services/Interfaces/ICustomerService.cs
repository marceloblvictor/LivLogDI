using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;

namespace LivlogDI.Services.Interfaces
{
    public interface ICustomerService
    {
        CustomerDTO Create(CustomerDTO customerDTO);
        CustomerDTO CreateDTO(Customer customer);
        IEnumerable<CustomerDTO> CreateDTOs(IEnumerable<Customer> customers);
        Customer CreateEntity(CustomerDTO dto);
        bool Delete(int id);
        CustomerDTO Get(int id);
        IEnumerable<CustomerDTO> GetAll();
        CustomerCategory GetCustomerCategory(int id);
        CustomerDTO Update(int id, CustomerDTO customerDTO);
    }
}