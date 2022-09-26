using LivlogDI.Data.Repositories;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;

namespace LivlogDITests.ServicesTests
{
    public class CustomerServiceTest
    {
        CustomerService _service { get; set; }

        Customer ValidCustomer { get; set; } = new()
        {
            Id = 1,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory) 1
        };

        IList<Customer> ValidCustomers = new List<Customer>()
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
            new()
            {
                Id = 4,
                Name = "marceloblvictor4",
                Phone = "98534542712",
                Email = "marceloblvictor4@gmail.com",
                Category = (CustomerCategory) 3
            },
        };

        CustomerDTO ValidCustomerDTO { get; set; } = new()
        {
            Id = 1,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory)1
        };

        IList<CustomerDTO> ValidCustomerDTOs = new List<CustomerDTO>()
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
            new()
            {
                Id = 4,
                Name = "marceloblvictor4",
                Phone = "98534542712",
                Email = "marceloblvictor4@gmail.com",
                Category = (CustomerCategory) 3
            },
        };

        public CustomerServiceTest()
        {
            var dbContext = new LivlogDI.Data.LivlogDIContext();
            _service = new CustomerService(new CustomerRepository(dbContext));
        }

        //[Fact]
        //public void GetCustomerCategory_ValidCustomerId_ReturnsCorrectCustomerCategory()
        //{
        //    // Arrange
        //    var validCustomer = ValidCustomer;

        //    // Act
        //    var result = _service.GetCustomerCategory(validCustomer.Id);

        //    // Assert
        //    Assert.Equal(validCustomer.Category, result);
        //}
       

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