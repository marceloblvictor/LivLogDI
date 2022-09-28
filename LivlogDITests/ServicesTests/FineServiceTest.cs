using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;
using LivlogDI.Validators;
using Moq;
using static LivlogDI.Enums.CustomerCategory;
using static LivlogDI.Enums.FineStatus;

namespace LivlogDITests.ServicesTests
{
    public class FineServiceTest
    {
        Mock<IFineRepository> _mockedRepo { get; set; }
        FineService _service { get; set; }

        Customer ValidCustomer { get; set; } = new()
        {
            Id = 1,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory)1
        };
        CustomerDTO ValidCustomerDTO { get; set; } = new()
        {
            Id = 1,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory)1
        };        
        Fine ValidFine { get; set; } = new()
        {
            Id = 1,
            Amount = 15m,
            Status = (FineStatus)1,
            CustomerId = 1            
        };
        List<Fine> ValidFines = new List<Fine>()
        {
            new ()
            {
                Id = 1,
                Amount = 15m,
                Status = (FineStatus) 1,
                CustomerId = 1
            },
            new ()
            {
                Id = 2,
                Amount = 13m,
                Status = (FineStatus) 1,
                CustomerId = 1
            },
            new ()
            {
                Id = 3,
                Amount = 12m,
                Status = (FineStatus) 2,
                CustomerId = 1
            },
        };
        FineDTO ValidFineDTO { get; set; } = new()
        {
            Id = 4,
            Amount = 15m,
            Status = (FineStatus)1,
            CustomerId = 1,
            CustomerName = "marceloblvictor"
        };
        List<FineDTO> ValidFinesDTOs = new List<FineDTO>()
        {
            new ()
            {
                Id = 1,
                Amount = 15m,
                Status = (FineStatus) 1,
                CustomerId = 1,
                CustomerName = "marceloblvictor"
            },
            new ()
            {
                Id = 2,
                Amount = 13m,
                Status = (FineStatus) 1,
                CustomerId = 1,
                CustomerName = "marceloblvictor"
            },
            new ()
            {
                Id = 3,
                Amount = 12m,
                Status = (FineStatus) 2,
                CustomerId = 1,
                CustomerName = "marceloblvictor"
            },
            new ()
            {
                Id = 4,
                Amount = 18m,
                Status = (FineStatus) 2,
                CustomerId = 1,
                CustomerName = "marceloblvictor"
            },
        };

        public FineServiceTest()
        {
            ValidFine.Customer = ValidCustomer;

            ValidFines.ForEach(f => { f.Customer = ValidCustomer; });

            _mockedRepo = new Mock<IFineRepository>();
            
            _mockedRepo
                .Setup(repo => repo.GetAll())
                .Returns(ValidFines);
            
            _mockedRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns<int>(id => ValidFines.Where(f => f.Id == id).Single());
            
            _mockedRepo
                .Setup(repo => repo.Add(It.IsAny<Fine>()))
                .Callback<Fine>(fine =>
                {
                    fine.Id = 4;
                    fine.Customer = ValidCustomer;
                    ValidFines.Add(fine);
                })
                .Returns<Fine>(fine => fine);
            
            _mockedRepo
                .Setup(repo => repo.Update(It.IsAny<Fine>()))
                .Callback<Fine>(f =>
                {
                    var fineToBeUpdated = ValidFines
                        .Where(bk => bk.Id == f.Id)
                        .Single();

                    fineToBeUpdated.Amount = f.Amount;
                    fineToBeUpdated.Status = f.Status;
                })
                .Returns<Fine>(fine => fine);

            _mockedRepo
                .Setup(repo => repo.Delete(It.IsAny<int>()))
                .Callback<int>(id =>
                {
                    ValidFines.Remove(ValidFines.Where(f => f.Id == id).Single());
                })
                .Returns(true);

            var _mockedCustomerRepo = new Mock<ICustomerRepository>();

            _mockedCustomerRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns(ValidCustomer);

            _service = new FineService(
                _mockedRepo.Object,
                new FineValidator(),
                new CustomerService(_mockedCustomerRepo.Object));
        }

        [Fact]
        public void GetAll_FinesAreOrderedDescendingById()
        {
            // Act
            var fines = _service.GetAll().ToList();

            // Assert
            Assert.Equal(3, fines.Count());
            Assert.True(fines.Any(b => b.Id == ValidFines[0].Id));
            Assert.True(fines.Any(b => b.Id == ValidFines[1].Id));
            Assert.True(fines.Any(b => b.Id == ValidFines[2].Id));
        }

        [Theory]
        [InlineData(1)]
        public void GetAFineWithAValidId_ReturnsCorrectFineDTO(int validID)
        {
            // Act
            var fine = _service.Get(validID);

            // Assert            
            Assert.NotNull(fine);
            Assert.Equal(validID, fine.Id);
        }

        [Fact]
        public void CreateAFine_PersistsNewFine()
        {
            var newFine = ValidFineDTO;

            var addedFineDto = _service.Create(newFine);

            Assert.True(ValidFinesDTOs.Count == 4);
            Assert.Equal(newFine.Amount, addedFineDto.Amount);
            Assert.Equal(newFine.Status, addedFineDto.Status);
        }

        [Fact]
        public void UpdateFineToPaid_SufficientAmountPaid_StatusChangedToPaid()
        {
            var fine = _service
                .GetAll()
                .Where(b => b.Id == 1)
                .Single();

            var updatedFineDTO = _service.UpdateFineToPaid(fine.Id, fine.Amount);

            Assert.True(fine.Id == updatedFineDTO.Id);
            Assert.True(updatedFineDTO.Status == Paid);
        }

        [Fact]
        public void GetCustomerDebts_GivenData_ReturnsCorrectAmount()
        {
            var customerInDebt = ValidCustomer;
            var correctAmount = ValidFines
                .Where(f => f.CustomerId == customerInDebt.Id &&
                            f.Status == Active)
                .Select(f => f.Amount)
                .Sum();

            var calculatedAmount = _service.GetCustomerDebts(customerInDebt.Id);

            Assert.Equal(correctAmount, calculatedAmount);
        }

        [Fact]
        public void FineCustomer_GivenData_CreateFine()
        {
            var daysOverdue = 100;

            var updatedFineDTO = _service.FineCustomer(ValidCustomer.Id, daysOverdue);

            Assert.True(ValidFines.Count == 4);
            Assert.True(ValidFines.Any(f => f.Id == updatedFineDTO.Id));
        }


        [Fact]
        public void Delete_ValidId_RemovesFromCollection()
        {
            var validFine = ValidFine;

            var result = _service.Delete(validFine.Id);

            Assert.True(result);
            Assert.DoesNotContain(validFine, ValidFines);
        }

        [InlineData(Top, 12.50)]
        [InlineData(Medium, 15.00)]
        [InlineData(Low, 20.00)]
        [Theory]
        public void CalculateFineAmount_GivenCategoryAndOverdueDays_ReturnsCorrectFineAmount(
            CustomerCategory category, 
            decimal correctAmount)
        {
            // Arrange
            var overdueDays = 10;

            // Act
            var amount = _service.CalculateFineAmount(category, overdueDays);

            // Assert
            Assert.Equal(correctAmount, amount);
        }

        [InlineData(Top, 1.25)]
        [InlineData(Medium, 1.50)]
        [InlineData(Low, 2)]
        [Theory]
        public void GetCategoryFineRate_GivenCategory_ReturnsCorrectFineRate(
            CustomerCategory category, 
            decimal correctRate)
        {
            // Act
            var result = _service.GetCategoryFineRate(category);

            // Assert
            Assert.Equal(correctRate, result);
        }

        [Fact]
        public void GetCategoryFineRate_GivenInvalidCategory_ThrowsArgumentException()
        {
            CustomerCategory invalidCategory = InvalidCategory;

            // Act
            var operation = () =>
            {
                _service.GetCategoryFineRate(invalidCategory);
            }; 

            // Assert
            Assert.ThrowsAny<ArgumentException>(operation);
        }

        [Fact]
        public void CreateDTO_GenerateDTOWithEntityData_Succes()
        {
            // Arrange
            var validFine = ValidFine;

            // Act
            var dto = _service.CreateDTO(validFine);

            // Assert
            Assert.True(dto.Id == validFine.Id);
            Assert.True(dto.Amount == validFine.Amount);
            Assert.True(dto.Status == validFine.Status);
            Assert.True(dto.CustomerName == validFine.Customer.Name);
            Assert.True(dto.CustomerId == validFine.CustomerId);
        }

        [Fact]
        public void CreateDTOS_GenerateDTOSWithEntityData_Succes()
        {
            // Arrange
            var validFines = ValidFines;

            // Act
            var dtos = _service.CreateDTOs(validFines);

            // Assert

            foreach (var dto in dtos)
            {
                var validFine =
                    validFines.First(u => u.Id == dto.Id);

                Assert.True(dto.Id == validFine.Id);
                Assert.True(dto.Amount == validFine.Amount);
                Assert.True(dto.Status == validFine.Status);
                Assert.True(dto.CustomerName == validFine.Customer.Name);
                Assert.True(dto.CustomerId == validFine.CustomerId);
            }
        }

        [Fact]
        public void CreateEntity_GenerateEntityWithDTOData_Succes()
        {
            // Arrange
            var dto = ValidFineDTO;

            // Act
            var entity = _service.CreateEntity(dto);

            // Assert
            Assert.True(entity.Id == dto.Id);
            Assert.True(entity.Amount == dto.Amount);
            Assert.True(entity.Status == dto.Status);
            Assert.True(entity.CustomerId == dto.CustomerId);
        }

        [Fact]
        public void FilterByIds_GivenValidIds_ReturnsCorrectFines()
        {
            // Arrange
            var validIds = new[] { 1, 2 };

            // Act
            var filteredFines = _service.FilterByIds(ValidFinesDTOs, validIds);

            // Assert
            Assert.Contains(ValidFinesDTOs.First(f => f.Id == validIds[0]), filteredFines);
            Assert.Contains(ValidFinesDTOs.First(f => f.Id == validIds[1]), filteredFines);
        }

        [Fact]
        public void FilterByCustomers_GivenValidCustomers_ReturnsCorrectFines()
        {
            // Arrange
            var customerId = ValidCustomerDTO.Id;

            // Act
            var filteredFines = _service.FilterByCustomer(ValidFinesDTOs, customerId);

            // Assert
            Assert.True(filteredFines.All(f => f.CustomerId == customerId));
        }

        [InlineData(Active)]
        [InlineData(Paid)]
        [Theory]
        public void FilterByStatus_GivenValidStatus_ReturnsCorrectFines(FineStatus status)
        {
            // Act
            var filteredFines = _service.FilterByStatus(ValidFinesDTOs, status);

            // Assert
            Assert.True(filteredFines.All(f => f.Status == status));
        }

        [InlineData(Active)]
        [InlineData(Paid)]
        [Theory]
        public void SetFineStatusToPaid_GivenFineDTO_ReturnsDTOWithPaidStatus(FineStatus status)
        {
            // Arrange
            var fineDto = ValidFineDTO;
            fineDto.Status = status;

            // Act
            fineDto = _service.SetFineStatusToPaid(fineDto);

            // Assert
            Assert.True(fineDto.Status == Paid);
        }
    }
}