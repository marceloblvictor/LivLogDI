using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services.Interfaces;

namespace LivlogDI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(
            ICustomerRepository repo)
        {
            _repo = repo;
        }

        public CustomerDTO Get(int id)
        {
            var customer = _repo.Get(id);

            return CreateDTO(customer);
        }

        public IEnumerable<CustomerDTO> GetAll()
        {
            var customers = _repo.GetAll();

            return CreateDTOs(customers);
        }

        public CustomerDTO Create(CustomerDTO customerDTO)
        {
            var customer = CreateEntity(customerDTO);

            customer = _repo.Add(customer);

            return CreateDTO(customer);
        }

        public CustomerDTO Update(int id, CustomerDTO customerDTO)
        {
            var customer = _repo.Get(id);

            customer.Name = customerDTO.Name;
            customer.Email = customerDTO.Email;
            customer.Phone = customerDTO.Phone;
            customer.Category = customerDTO.Category;

            var updatedCustomer =
                _repo.Update(customer);

            return CreateDTO(updatedCustomer);
        }

        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }

        #region Helper methods

        public CustomerCategory GetCustomerCategory(int id)
        {
            var customer = Get(id);

            return customer.Category;
        }

        public CustomerDTO CreateDTO(Customer customer)
        {
            return new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Category = customer.Category
            };
        }

        public IEnumerable<CustomerDTO> CreateDTOs(IEnumerable<Customer> customers)
        {
            var customerDtos = new List<CustomerDTO>();

            foreach (var customer in customers)
            {
                customerDtos.Add(CreateDTO(customer));
            }

            return customerDtos;
        }

        public Customer CreateEntity(CustomerDTO dto)
        {
            return new Customer
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Category = dto.Category
            };
        }

        #endregion
    }
}
