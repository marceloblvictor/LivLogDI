using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;
using Moq;

namespace LivlogDITests.ServicesTests
{
    public class CustomerServiceTest
    {
        Mock<ICustomerRepository> _mockedRepo { get; set; }
        CustomerService _service { get; set; }

        Customer ValidCustomer { get; set; } = new()
        {
            Id = 1,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory) 1
        };
        List<Customer> ValidCustomers = new List<Customer>()
        {
            new()
            {
                Id = 1,
                Name = "marceloblvictor",
                Phone = "98534542767",
                Email = "marceloblvictor@gmail.com",
                Category = (CustomerCategory) 1
            },
            new ()
            {
                Id = 2,
                Name = "marceloblvictor2",
                Phone = "98534542753",
                Email = "marceloblvictor2@gmail.com",
                Category = (CustomerCategory) 2
            },
            new()
            {
                Id = 3,
                Name = "marceloblvictor3",
                Phone = "98534542732",
                Email = "marceloblvictor3@gmail.com",
                Category = (CustomerCategory) 3
            },
        };
        CustomerDTO ValidCustomerDTO { get; set; } = new()
        {
            Id = 4,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory)1
        };

        public CustomerServiceTest()
        {
            _mockedRepo = new Mock<ICustomerRepository>();

            _mockedRepo
                .Setup(repo => repo.GetAll())
                .Returns(ValidCustomers);

            _mockedRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns<int>(id => ValidCustomers.Where(c => c.Id == id).Single());

            _mockedRepo
                .Setup(repo => repo.Add(It.IsAny<Customer>()))
                .Callback<Customer>(customer =>
                {
                    customer.Id = 4;
                    ValidCustomers.Add(customer);
                })
                .Returns<Customer>(customer => customer);

            _mockedRepo
                .Setup(repo => repo.Update(It.IsAny<Customer>()))
                .Callback<Customer>(c =>
                {
                    var customerToBeUpdated = ValidCustomers
                        .Where(cust => cust.Id == c.Id)
                        .Single();

                    customerToBeUpdated.Name = c.Name;
                    customerToBeUpdated.Email = c.Email;
                    customerToBeUpdated.Phone = c.Phone;
                    customerToBeUpdated.Category = c.Category;
                })
                .Returns<Customer>(customer => customer);

            _mockedRepo
                .Setup(repo => repo.Delete(It.IsAny<int>()))
                .Callback<int>(id =>
                {
                    ValidCustomers.Remove(ValidCustomers.Where(f => f.Id == id).Single());
                })
                .Returns(true);

            _service = new CustomerService(
                _mockedRepo.Object);
        }

        [Fact]
        public void GetAll_CustomersAreOrderedDescendingById()
        {
            // Act
            var customers = _service.GetAll().ToList();

            // Assert
            Assert.Equal(3, customers.Count());
            Assert.True(customers.Any(c => c.Id == ValidCustomers[0].Id));
            Assert.True(customers.Any(c => c.Id == ValidCustomers[1].Id));
            Assert.True(customers.Any(c => c.Id == ValidCustomers[2].Id));
        }

        [Theory]
        [InlineData(1)]
        public void GetACustomerWithAValidId_ReturnsCorrectCustomerDTO(int validID)
        {
            // Act
            var customer = _service.Get(validID);

            // Assert            
            Assert.NotNull(customer);
            Assert.Equal(validID, customer.Id);
        }

        [Fact]
        public void CreateACustomer_PersistsNewCustomer()
        {
            var newCustomer = ValidCustomerDTO;

            var addedCustomerDto = _service.Create(newCustomer);

            Assert.True(ValidCustomers.Count == 4);
            Assert.Equal(newCustomer.Name, addedCustomerDto.Name);
            Assert.Equal(newCustomer.Email, addedCustomerDto.Email);
            Assert.Equal(newCustomer.Phone, addedCustomerDto.Phone);
            Assert.Equal(newCustomer.Category, addedCustomerDto.Category);
        }

        [Fact]
        public void Update_CustomerIsUpdated()
        {
            var customer = _service
                .GetAll()
                .Where(b => b.Id == 1)
                .Single();

            customer.Name = "nomeMODIFICADO";
            customer.Email = "emailMODIFICADO@email.com";
            customer.Phone = "67567567";
            customer.Category = CustomerCategory.Low;

            var updatedCustomerDTO = _service.Update(customer.Id, customer);

            Assert.True(customer.Id == updatedCustomerDTO.Id);
            Assert.True(customer.Name == updatedCustomerDTO.Name);
            Assert.True(customer.Email == updatedCustomerDTO.Email);
            Assert.True(customer.Phone == updatedCustomerDTO.Phone);
            Assert.True(customer.Category == updatedCustomerDTO.Category);
        }

        [Fact]
        public void CreateCustomer_NewCustomerIsAdded()
        {
            var newCustomer = ValidCustomerDTO;

            var updatedCustomerDTO = _service.Create(newCustomer);

            Assert.True(ValidCustomers.Count == 4);
            Assert.True(ValidCustomers.Any(c => c.Id == updatedCustomerDTO.Id));
        }


        [Fact]
        public void Delete_ValidId_RemovesFromCollection()
        {
            var validCustomer = ValidCustomer;

            var result = _service.Delete(validCustomer.Id);

            Assert.True(result);
            Assert.DoesNotContain(validCustomer, ValidCustomers);
        }

        [Fact]
        public void GetCustomerCategory_ValidCustomerId_ReturnsCorrectCustomerCategory()
        {
            // Arrange
            var validCustomer = ValidCustomer;

            // Act
            var result = _service.GetCustomerCategory(validCustomer.Id);

            // Assert
            Assert.Equal(validCustomer.Category, result);
        }


        [Fact]
        public void CreateDTO_GenerateDTOWithEntityData_Succes()
        {
            // Arrange
            var validCustomer = ValidCustomer;

            // Act
            var dto = _service.CreateDTO(validCustomer);

            // Assert
            Assert.True(dto.Id == validCustomer.Id);
            Assert.True(dto.Name == validCustomer.Name);
            Assert.True(dto.Phone == validCustomer.Phone);
            Assert.True(dto.Email == validCustomer.Email);
            Assert.True(dto.Category == validCustomer.Category);
        }

        [Fact]
        public void CreateDTOS_GenerateDTOSWithEntityData_Succes()
        {
            // Arrange
            var validCustomers = ValidCustomers;

            // Act
            var dtos = _service.CreateDTOs(validCustomers);

            // Assert

            foreach (var dto in dtos)
            {
                var validCustomer =
                    validCustomers.First(u => u.Id == dto.Id);

                Assert.True(dto.Id == validCustomer.Id);
                Assert.True(dto.Name == validCustomer.Name);
                Assert.True(dto.Phone == validCustomer.Phone);
                Assert.True(dto.Email == validCustomer.Email);
                Assert.True(dto.Category == validCustomer.Category);
            }
        }

        [Fact]
        public void CreateEntity_GenerateEntityWithDTOData_Succes()
        {
            // Arrange
            var dto = ValidCustomerDTO;

            // Act
            var entity = _service.CreateEntity(dto);

            // Assert
            Assert.True(entity.Id == dto.Id);
            Assert.True(entity.Name == dto.Name);
            Assert.True(entity.Phone == dto.Phone);
            Assert.True(entity.Email == dto.Email);
            Assert.True(entity.Category == dto.Category);
        }
    }
}