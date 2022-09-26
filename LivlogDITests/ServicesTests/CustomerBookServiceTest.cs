using LivlogDI.Data.Repositories;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;
using LivlogDI.Validators;
using static LivlogDI.Enums.BookRentalStatus;
using static LivlogDI.Enums.CustomerCategory;

namespace LivlogNoDITests.ServicesTests
{
    public class CustomerBookServiceTest
    {
        CustomerBookService _service { get; set; }

        Book ValidBook { get; set; } = new()
        {
            Id = 1,
            Title = "LivroTeste1",
            ISBN = "teste1",
            Quantity = 5
        };

        IList<Book> ValidBooks = new List<Book>()
        {
            new()
            {
                Id = 1,
                Title = "LivroTeste1",
                ISBN = "teste1",
                Quantity = 5
            },
            new()
            {
                Id = 2,
                Title = "LivroTeste2",
                ISBN = "teste2",
                Quantity = 5
            },
            new()
            {
                Id = 3,
                Title = "LivroTeste3",
                ISBN = "teste3",
                Quantity = 5
            },
            new()
            {
                Id = 4,
                Title = "LivroTeste4",
                ISBN = "teste4",
                Quantity = 5
            },
        };

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

        CustomerBook ValidCustomerBookActive { get; set; } = new()
        {
            Id = 1,
            BookId = 1,
            CustomerId = 1,
            StartDate = new DateTime(2022, 09, 01),
            DueDate = new DateTime(2022, 10, 01),
            Status = Active
        };   

        IList<CustomerBook> ValidCustomerBooks { get; set; } = new List<CustomerBook>()
        {
            new()
            {
                Id = 1,
                BookId = 1,
                CustomerId = 1,
                StartDate = new DateTime(2022, 09, 01),
                DueDate = new DateTime(2022, 10, 01),
                Status = Active
            },
            new()
            {
                Id = 2,
                BookId = 2,
                CustomerId = 1,
                StartDate = null,
                DueDate = null,
                Status = WaitingQueue
            },
            new()
            {
                Id = 3,
                BookId = 3,
                CustomerId = 1,
                StartDate = null,
                DueDate = null,
                Status = Returned
            }
        };

        CustomerBookDTO ValidCustomerBookActiveDTO { get; set; } = new()
        {
            Id = 1,
            BookId = 1,
            CustomerId = 1,
            StartDate = new DateTime(2022, 09, 01),
            DueDate = new DateTime(2022, 10, 01),
            Status = Active
        };        

        IList<CustomerBookDTO> ValidCustomerBooksDTOs { get; set; } = new List<CustomerBookDTO>()
        {
            new()
            {
                Id = 1,
                BookId = 1,
                CustomerId = 1,
                StartDate = new DateTime(2022, 09, 01),
                DueDate = new DateTime(2022, 10, 01),
                Status = Active,
            },
            new()
            {
                Id = 2,
                BookId = 2,
                CustomerId = 1,
                StartDate = null,
                DueDate = null,
                Status = WaitingQueue
            },
            new()
            {
                Id = 3,
                BookId = 3,
                CustomerId = 1,
                StartDate = null,
                DueDate = null,
                Status = Returned
            }
        };        

        public CustomerBookServiceTest()
        {
            // Preparar Moq            

            var dbContext = new LivlogDI.Data.LivlogDIContext();
            var customerService = new CustomerService(new CustomerRepository(dbContext));

            _service = new CustomerBookService(
                new CustomerBookRepository(dbContext),
                customerService,
                new BookService(new BookRepository(dbContext)),
                new FineService(new FineRepository(dbContext), new FineValidator(), customerService),
                new BookRentalValidator(),
                new MessagerService());

            ValidCustomerBookActive.Book = ValidBook;
            ValidCustomerBookActive.Customer = ValidCustomer;

            ValidCustomerBookActiveDTO.BookTitle = ValidBook.Title;
            ValidCustomerBookActiveDTO.CustomerName = ValidCustomer.Name;

            foreach (var cb in ValidCustomerBooks)
            {
                cb.Customer = ValidCustomer;
                cb.Book = ValidBooks
                    .Where(b => b.Id == cb.BookId)
                    .Single();
            }
        }

        [Fact]
        public void SetCustomerBookStatusToReturned_GivenBooksDtos_ReturnReturnedBooks()
        {
            // Arrange
            var validDtos = ValidCustomerBooksDTOs
                .Where(cb => cb.Status == Active)
                .ToList();

            // Act
            var result = _service.SetCustomerBookStatusToReturned(validDtos);

            // Assert
            Assert.True(result.All(cb => cb.Status == Returned));
        }

        [Fact]
        public void GetOverdueDays_GivenTwoDates_ReturnCorrectDuration()
        {
            // Arrange
            (var startDate, var endDate) = 
                (ValidCustomerBookActive.StartDate, ValidCustomerBookActive.DueDate);

            // Act
            var result = _service.GetOverdueDays(startDate.Value, endDate.Value);

            // Assert
            Assert.True(result == 30);
        }

        [Fact]
        public void IsReturnedBookOverdue_GivenPastDueDate_ReturnsTrue()
        {
            // Arrange
            var returnDate = new DateTime(2022, 12, 31);
            var validCustomerBookDto = ValidCustomerBookActiveDTO;

            // Act
            var result = _service.IsReturnedBookOverdue(validCustomerBookDto, returnDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsReturnedBookOverdue_GivenFutureDueDate_ReturnsFalse()
        {
            // Arrange
            var returnDate = new DateTime(2022, 01, 01);
            var validCustomerBookDto = ValidCustomerBookActiveDTO;

            // Act
            var result = _service.IsReturnedBookOverdue(validCustomerBookDto, returnDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsReturnedBookOverdue_GivenSameDates_ReturnsFalse()
        {
            // Arrange
            var returnDate = new DateTime(2022, 12, 31);
            var dueDate = new DateTime(2022, 12, 31);

            var validCustomerBookDto = ValidCustomerBookActiveDTO;
            validCustomerBookDto.DueDate = dueDate;

            // Act
            var result = _service.IsReturnedBookOverdue(validCustomerBookDto, returnDate);

            // Assert
            Assert.True(result is false);
        }

        [Fact]
        public void CalculateDueDate_GivenStartDate_ReturnsCorrectDueDate()
        {
            // Arrange
            var validCustomerDto = ValidCustomerDTO;
            validCustomerDto.Category = Top;
            var startTime = new DateTime(2022, 09, 01);
            var correctDueDate = startTime.AddDays(15);


            // Act
            var result = _service.CalculateDueDate(validCustomerDto, startTime);

            // Assert
            Assert.Equal(correctDueDate, result);
        }

        [InlineData(Top, 15)]
        [InlineData(Medium, 10)]
        [InlineData(Low, 5)]
        [Theory]
        public void GetCustomerCategoryDaysDuration_GivenCategory_ReturnsCorrectDuration(
            CustomerCategory category, 
            int correctDuration)
        {
            // Act
            var result = _service.GetCustomerCategoryDaysDuration(category);

            // Assert
            Assert.Equal(correctDuration, result);
        }

        [Fact]
        public void GetCustomerCategoryDaysDuration_GivenInvalidCategory_ThrowsArgumentException()
        {
            // Arrange
            var invalidCategory = InvalidCategory;

            // Act
            var operation = () => 
            { 
                _service.GetCustomerCategoryDaysDuration(invalidCategory); 
            };

            // Assert
            Assert.Throws<ArgumentException>(operation);
        }

        [InlineData(Top, 5)]
        [InlineData(Medium, 3)]
        [InlineData(Low, 1)]
        [Theory]
        public void GetCustomerRentalsMaxLimit_GivenCategory_ReturnsCorrectMaxLimit(
            CustomerCategory category,
            int correctDuration)
        {
            // Act
            var result = _service.GetCustomerRentalsMaxLimit(category);

            // Assert
            Assert.Equal(correctDuration, result);
        }

        [Fact]
        public void GetCustomerRentalsMaxLimit_GivenInvalidCategory_ThrowsArgumentException()
        {
            // Arrange
            var invalidCategory = InvalidCategory;

            // Act
            var operation = () =>
            {
                _service.GetCustomerRentalsMaxLimit(invalidCategory);
            };

            // Assert
            Assert.Throws<ArgumentException>(operation);
        }

        [Fact]
        public void FilterByCustomer_GivenValidIds_ReturnCorrectData()
        {
            // Arrange
            var validCustomerId = 1;
            var validDtos = ValidCustomerBooksDTOs;

            // Act
            var filteredDtos = _service.FilterByCustomer(validDtos, validCustomerId);

            // Assert
            Assert.True(filteredDtos.All(dto => dto.CustomerId == validCustomerId));
        }

        [Fact]
        public void FilterByBooks_GivenValidIds_ReturnCorrectData()
        {
            // Arrange
            var validBookId= 1;
            var validDtos = ValidCustomerBooksDTOs;

            // Act
            var filteredDtos = _service.FilterByBooks(validDtos, new List<int> { validBookId });

            // Assert
            Assert.True(filteredDtos.All(dto => dto.BookId == validBookId));
        }

        [Fact]
        public void FilterByIds_GivenValidIds_ReturnCorrectData()
        {
            // Arrange
            var validBookCustomerId = 1;
            var validDtos = ValidCustomerBooksDTOs;

            // Act
            var filteredDtos = _service.FilterByIds(validDtos, new List<int> { validBookCustomerId });

            // Assert
            Assert.True(filteredDtos.All(dto => dto.Id == validBookCustomerId));
        }

        [Theory]
        [InlineData(Active)]
        [InlineData(WaitingQueue)]
        [InlineData(Returned)]
        public void FilterByStatus_GivenValidIds_ReturnCorrectData(BookRentalStatus status)
        {
            // Arrange
            var validStatus = status;
            var validDtos = ValidCustomerBooksDTOs;

            // Act
            var filteredDtos = _service.FilterByStatus(validDtos, validStatus);

            // Assert
            Assert.True(filteredDtos.All(dto => dto.Status == validStatus));
        }

        [Fact]
        public void FilterByCostumerAndBook_GivenValidIds_ReturnCorrectData()
        {
            // Arrange
            (var validBookId, var validCustomerId) = (1, 1);
            var validDtos = ValidCustomerBooksDTOs;

            // Act
            var filteredDtos = _service.FilterByCustomerAndBook(validDtos, validCustomerId, new[] { validBookId });

            // Assert
            Assert.True(filteredDtos.All(dto => dto.CustomerId == validCustomerId && dto.BookId == validBookId));
        }

        [Fact]
        public void FilterByCostumerAndBook_GivenInvalidIds_ReturnsEmptyList()
        {
            // Arrange
            (var invalidBookId, var invalidCustomerId) = (99, 99);
            var validDtos = ValidCustomerBooksDTOs;

            // Act
            var filteredDtos = _service.FilterByCustomerAndBook(validDtos, invalidCustomerId, new[] { invalidBookId });

            // Assert
            Assert.Empty(filteredDtos);
        }

        [Fact]
        public void CreateDTO_GenerateDTOWithEntityData_Succes()
        {
            // Arrange
            var validCustomerBook = ValidCustomerBookActive;

            // Act
            var dto = _service.CreateDTO(validCustomerBook);

            // Assert
            Assert.True(dto.Id == validCustomerBook.Id);
            Assert.True(dto.BookId == validCustomerBook.BookId);
            Assert.True(dto.BookTitle == validCustomerBook.Book.Title);
            Assert.True(dto.CustomerId == validCustomerBook.CustomerId);
            Assert.True(dto.CustomerName == validCustomerBook.Customer.Name);
            Assert.True(dto.StartDate == validCustomerBook.StartDate);
            Assert.True(dto.DueDate == validCustomerBook.DueDate);
            Assert.True(dto.Status == validCustomerBook.Status);
        }

        [Fact]
        public void CreateDTOS_GenerateDTOSWithEntityData_Succes()
        {
            // Arrange
            var validCustomerBooks = ValidCustomerBooks;

            // Act
            var dtos = _service.CreateDTOs(validCustomerBooks);

            // Assert

            foreach (var dto in dtos)
            {
                var validCustomerBook =
                    validCustomerBooks.First(u => u.Id == dto.Id);

                Assert.True(dto.Id == validCustomerBook.Id);
                Assert.True(dto.BookId == validCustomerBook.BookId);
                Assert.True(dto.BookTitle == validCustomerBook.Book.Title);
                Assert.True(dto.CustomerId == validCustomerBook.CustomerId);
                Assert.True(dto.CustomerName == validCustomerBook.Customer.Name);
                Assert.True(dto.StartDate == validCustomerBook.StartDate);
                Assert.True(dto.DueDate == validCustomerBook.DueDate);
                Assert.True(dto.Status == validCustomerBook.Status);
            }
        }

        [Fact]
        public void CreateEntity_GenerateEntityWithDTOData_Succes()
        {
            // Arrange
            var validCustomerBookDTO = ValidCustomerBookActiveDTO;

            // Act
            var dto = _service.CreateEntity(validCustomerBookDTO);

            // Assert
            Assert.True(dto.Id == validCustomerBookDTO.Id);
            Assert.True(dto.BookId == validCustomerBookDTO.BookId);
            Assert.True(dto.CustomerId == validCustomerBookDTO.CustomerId);
            Assert.True(dto.StartDate == validCustomerBookDTO.StartDate);
            Assert.True(dto.DueDate == validCustomerBookDTO.DueDate);
            Assert.True(dto.Status == validCustomerBookDTO.Status);
        }
    }
}