using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Enums;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using static LivlogDI.Enums.CustomerCategory;

namespace LivlogDITests.RepositoriesTests
{
    public class CustomerRepositoryTest
    {
        Customer ValidCustomer { get; set; } = new()
        {
            Id = 4,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory)1
        };

        Mock<LivlogDIContext> _mockedDbContext { get; set; }
        List<Customer> _mockedCustomers { get; set; } = new List<Customer>
        {
            new()
            {
                Id = 1,
                Name = "teste1",
                Email = "teste1@teste.com",
                Phone = "999999999",
                Category = Top,
            },
            new()
            {
                Id = 2,
                Name = "teste2",
                Email = "teste2@teste.com",
                Phone = "999999998",
                Category = Medium,
            },
            new()
            {
                Id = 3,
                Name = "teste3",
                Email = "teste3@teste.com",
                Phone = "999999997",
                Category = Low,
            }
        };

        CustomerRepository Repository { get; set; }

        public CustomerRepositoryTest()
        {            
            var query = _mockedCustomers.AsQueryable();

            var mockCustomers = new Mock<DbSet<Customer>>();

            mockCustomers
                .As<IQueryable<Customer>>()
                .Setup(m => m.Provider)
                .Returns(query.Provider);
            mockCustomers
                .As<IQueryable<Customer>>()
                .Setup(m => m.Expression)
                .Returns(query.Expression);
            mockCustomers
                .As<IQueryable<Customer>>()
                .Setup(m => m.ElementType)
                .Returns(query.ElementType);
            mockCustomers
                .As<IQueryable<Customer>>()
                .Setup(m => m.GetEnumerator())
                .Returns(query.GetEnumerator());
            
            mockCustomers
                .Setup(m => m.Add(It.IsAny<Customer>()))
                .Callback<Customer>((c) => _mockedCustomers.Add(c));
            mockCustomers
                .Setup(m => m.Remove(It.IsAny<Customer>()))
                .Callback<Customer>((c) => { _mockedCustomers.Remove(c); });
            mockCustomers
               .Setup(m => m.Update(It.IsAny<Customer>()))
               .Callback<Customer>((c) =>
               {
                   var customerToBeUpdated = _mockedCustomers
                    .AsQueryable()
                    .Where(cust => cust.Id == c.Id)
                    .Single();

                   customerToBeUpdated.Name = c.Name;
                   customerToBeUpdated.Email = c.Email;
                   customerToBeUpdated.Phone = c.Phone;
                   customerToBeUpdated.Category = c.Category;
               });

            _mockedDbContext = new Mock<LivlogDIContext>();
            _mockedDbContext
                .Setup(ctx => ctx.Customers)
                .Returns(mockCustomers.Object);

            Repository = new CustomerRepository(_mockedDbContext.Object);
        }

        [Fact]
        public void GetAll_CustomersAreOrderedDescendingById()
        {            
            // Act
            var customers = Repository.GetAll();

            // Assert
            Assert.Equal(3, customers.Count());
            Assert.Equal(3, customers[0].Id);
            Assert.Equal(2, customers[1].Id);
            Assert.Equal(1, customers[2].Id);
        }

        [Theory]
        [InlineData(1)]
        public void GetACustomerWithAValidId_IsSuccess(int validID)
        {
            // Act
            var customer = Repository.Get(validID);

            // Assert            
            Assert.NotNull(customer);
            Assert.Equal(validID, customer.Id);
        }

        [Theory]
        [InlineData(99)]
        public void GetACustomerkWithAnInvalidId_IsFailure(int invalidId)
        {
            // Act
            var getOperation = 
                new Func<Customer>(() => Repository.Get(invalidId));

            // Assert
            Assert.Throws<ArgumentException>(getOperation);
        }

        [Fact]
        public void CreateACustomer_PersistsNewCustomer()
        {
            var newCustomer = ValidCustomer;

            Repository.Add(newCustomer);

            Assert.True(_mockedDbContext.Object.Customers.Count() == 4);
            Assert.True(_mockedDbContext.Object.Customers
                            .Where(c => c.Id == newCustomer.Id)
                            .FirstOrDefault()?
                                .Id == newCustomer.Id);
        }

        [Fact]
        public void Update_GivenNewCustomerkData_PersistsNewCustomer()
        {
            var updatedData = _mockedCustomers[0];

            updatedData.Name = "nomeMODIFICADO";
            updatedData.Email = "emailmodificado@email.com";
            updatedData.Phone = "457487563";
            updatedData.Category = Medium;

            Repository.Update(updatedData);

            var updatedCustomer = Repository
                .GetAll()
                .FirstOrDefault(u => u.Id == updatedData.Id);

            Assert.True(updatedCustomer is not null);
            Assert.True(updatedCustomer.Name == updatedData.Name);
            Assert.True(updatedCustomer.Email == updatedData.Email);
            Assert.True(updatedCustomer.Phone == updatedData.Phone);
            Assert.True(updatedCustomer.Category == updatedData.Category);
        }

        [Fact]
        public void Delete_GivenValidCustomerId_DeleteCustomer()
        {
            var customerToBeDeleted = _mockedCustomers[0];

            Repository.Delete(customerToBeDeleted.Id);

            Assert.DoesNotContain(customerToBeDeleted, _mockedCustomers);
        }

        [Fact]
        public void Delete_GivenInvalidCustomerId_ThrowsException()
        {
            var invalidId = 99;

            var operation = () =>
            {
                Repository.Delete(invalidId);
            };

            Assert.Throws<ArgumentException>(operation);
        }
    }
}