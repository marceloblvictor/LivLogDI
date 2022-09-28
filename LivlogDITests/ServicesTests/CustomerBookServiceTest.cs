using LivlogDI.Constants;
using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;
using LivlogDI.Services.Interfaces;
using LivlogDI.Validators;
using Microsoft.VisualStudio.TestPlatform.Common;
using Moq;
using static LivlogDI.Enums.BookRentalStatus;
using static LivlogDI.Enums.CustomerCategory;

namespace LivlogDITests.ServicesTests
{
    public class CustomerBookServiceTest
    {
        Mock<ICustomerBookRepository> _mockedRepo { get; set; }
        
        CustomerBookService _service { get; set; }

        Book ValidBook { get; set; } = new()
        {
            Id = 1,
            Title = "LivroTeste1",
            ISBN = "teste1",
            Quantity = 5
        };
        List<Book> ValidBooks { get; set; } = new List<Book>()
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
        List<Fine> ValidFines { get; set; } = new List<Fine>()
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
        CustomerBook ValidCustomerBook { get; set; } = new()
        {
            Id = 1,
            BookId = 1,
            CustomerId = 1,
            StartDate = new DateTime(2022, 09, 01),
            DueDate = new DateTime(2022, 10, 01),
            Status = Active
        };   
        List<CustomerBook> ValidCustomerBooks { get; set; } = new List<CustomerBook>()
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
        CustomerBookDTO ValidCustomerBookDTO { get; set; } = new()
        {
            Id = 1,
            BookId = 1,
            CustomerId = 1,
            StartDate = new DateTime(2022, 09, 01),
            DueDate = new DateTime(2022, 10, 01),
            Status = Active
        };        
        List<CustomerBookDTO> ValidCustomerBooksDTOs { get; set; } = new List<CustomerBookDTO>()
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

        CustomerBooksRequestDTO RentBooksRequest { get; set; } = new()
        {
            CustomerId = 1,
            BookIds = new[] { 1 }
        };

        public CustomerBookServiceTest()
        {
            // Preparar Moq            
            _mockedRepo = new Mock<ICustomerBookRepository>();
            _mockedRepo
                .Setup(repo => repo.GetAll())
                .Returns(ValidCustomerBooks);
            _mockedRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns<int>(id => ValidCustomerBooks.Where(b => b.Id == id).Single());
            _mockedRepo
                .Setup(repo => repo.Add(It.IsAny<CustomerBook>()))
                .Callback<CustomerBook>(book =>
                {
                    book.Id = 4;
                    ValidCustomerBooks.Add(book);
                })
                .Returns<CustomerBook>(book => 
                {
                    book.Book = ValidBooks.Where(b => b.Id == book.BookId).Single();
                    book.Customer = ValidCustomer;
                    return book;
                });
            _mockedRepo
                .Setup(repo => repo.Update(It.IsAny<CustomerBook>()))
                .Callback<CustomerBook>(b =>
                {
                    var bookToBeUpdated = ValidCustomerBooks
                        .Where(bk => bk.Id == b.Id)
                        .Single();

                    bookToBeUpdated.StartDate = b.StartDate;
                    bookToBeUpdated.DueDate = b.DueDate;
                    bookToBeUpdated.Status = b.Status;
                })
                .Returns<CustomerBook>(book => book);

            _mockedRepo
                .Setup(repo => repo.Delete(It.IsAny<int>()))
                .Callback<int>(id =>
                {
                    ValidCustomerBooks.Remove(ValidCustomerBooks.Where(u => u.Id == id).Single());
                })
                .Returns(true);

            var _mockedCustomerRepo = new Mock<ICustomerRepository>();
            _mockedCustomerRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns(ValidCustomer);
            var customerService = new CustomerService(_mockedCustomerRepo.Object);
            
            var _mockedBookRepo = new Mock<IBookRepository>();
            _mockedBookRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns(ValidBook);
            _mockedBookRepo
                .Setup(repo => repo.GetAll())
                .Returns(ValidBooks);
            var bookService = new BookService(_mockedBookRepo.Object);

            var fineValidator = new FineValidator();

            var _mockedFineRepo = new Mock<IFineRepository>();
            _mockedFineRepo
                .Setup(repo => repo.GetAll())
                .Returns(ValidFines);
            _mockedFineRepo
                .Setup(repo => repo.Add(It.IsAny<Fine>()))
                .Callback<Fine>(fine =>
                {
                    fine.Id = 4;
                    fine.Customer = ValidCustomer;
                    ValidFines.Add(fine);
                })
                .Returns<Fine>(fine => fine);
            var fineService = new FineService(
                _mockedFineRepo.Object, 
                fineValidator, 
                customerService);

            var _mockedMessenger = new Mock<IMessagerService>();
            _mockedMessenger
                .Setup(repo => repo.SendEmail("x", "y", "w", "z"))
                .Returns(true);

            _service = new CustomerBookService(
                _mockedRepo.Object,
                customerService,
                bookService,
                fineService,
                new BookRentalValidator(),
                _mockedMessenger.Object);

            ValidCustomerBook.Book = ValidBook;
            ValidCustomerBook.Customer = ValidCustomer;

            ValidCustomerBookDTO.BookTitle = ValidBook.Title;
            ValidCustomerBookDTO.CustomerName = ValidCustomer.Name;

            foreach (var cb in ValidCustomerBooks)
            {
                cb.Customer = ValidCustomer;
                cb.Book = ValidBooks
                    .Where(b => b.Id == cb.BookId)
                    .Single();
            }
            
            ValidFines.ForEach(f => { f.Customer = ValidCustomer; });
        }

        [Fact]
        public void GetAll_CustomersBooksAreOrderedDescendingById()
        {
            // Act
            var books = _service.GetAll().ToList();

            // Assert
            Assert.Equal(3, books.Count());
            Assert.True(books.Any(b => b.Id == ValidCustomerBooks[0].Id));
            Assert.True(books.Any(b => b.Id == ValidCustomerBooks[1].Id));
            Assert.True(books.Any(b => b.Id == ValidCustomerBooks[2].Id));
        }

        [Theory]
        [InlineData(1)]
        public void GetACustomerBookWithAValidId_ReturnsCorrectCustomerBookDTO(int validID)
        {
            // Act
            var book = _service.Get(validID);

            // Assert            
            Assert.NotNull(book);
            Assert.Equal(validID, book.Id);
        }

        [Theory]
        [InlineData(1)]
        public void GetByCustomer_ValidId_ReturnsCorrectCustomerBookDTO(int validID)
        {
            // Act
            var books = _service.GetByCustomer(validID);

            // Assert            
            Assert.True(books.All(b => b.CustomerId == validID));
        }

        [Theory]
        [InlineData(99, 98)]
        public void GetByCustomer_InvalidId_ReturnsEmptyList(int invalidID, int invalidId2)
        {
            // Act
            var books = _service.GetByCostumerAndBook(invalidID, invalidId2);

            // Assert            
            Assert.Empty(books);
        }

        [Theory]
        [InlineData(1, 1)]
        public void GetByCustomerAndBook_ReturnsCorrectCustomerBookDTO(int customerId, int bookId)
        {
            // Act
            var books = _service.GetByCostumerAndBook(customerId, bookId);

            // Assert            
            Assert.True(
                books.All(b => b.CustomerId == customerId && b.BookId == bookId));
        }

        [Fact]
        public void Update_PersistsChangesToBook()
        {
            var book = _service
                .GetAll()
                .Where(b => b.Id == 1)
                .Single();

            book.StartDate = new DateTime(2022, 05, 01);
            book.DueDate = new DateTime(2022, 10, 01);
            book.Status = (BookRentalStatus) 3;

            var updatedBookDTO = _service.Update(book.Id, book);

            Assert.Equal(book.StartDate, updatedBookDTO.StartDate);
            Assert.Equal(book.DueDate, updatedBookDTO.DueDate);
            Assert.Equal(book.Status, updatedBookDTO.Status);
        }

        [Fact]
        public void Delete_ValidId_RemovesFromCollection()
        {
            var validBook = ValidBook;

            var result = _service.Delete(validBook.Id);

            Assert.True(result);
            Assert.DoesNotContain(validBook, ValidBooks);
        }

        [Fact]
        public void RentBooks_ValidCustomer_Successfull()
        {
            // Arrange
            var requestDTO = RentBooksRequest;

            ValidFines = ValidFines
                .Where(f => f.CustomerId == requestDTO.CustomerId)
                .Select(f => { f.Status = FineStatus.Paid; return f; })
                .ToList();

            // Act
            var resultDto = _service.RentBooks(requestDTO);

            // Assert
            Assert.True(ValidCustomerBooks.Count == 4);
            Assert.True(ValidCustomerBooks.Any(cb => cb.BookId == requestDTO.BookIds[0] && 
                                                     cb.CustomerId == requestDTO.CustomerId &&
                                                     cb.Status == Active));
        }

        [Fact]
        public void ReturnBooks_ValidDate_Successfull()
        {
            // Arrange
            var custBookIds = new[] { 1 };            

            // Act
            var resultDto = _service.ReturnBooks(custBookIds);

            // Assert
            Assert.True(ValidCustomerBooks
                .Where(cb => cb.Id == custBookIds[0])
                .Single()
                    .Status == Returned);
        }

        [Fact]
        public void ReturnBooks_OverdueDate_Successfull()
        {
            // Arrange
            var custBook = ValidCustomerBooks[0];
            custBook.DueDate = new DateTime(2022, 09, 01);
            var custBookIds = new[] { custBook.Id };

            // Act
            var resultDto = _service.ReturnBooks(custBookIds);

            // Assert
            Assert.True(ValidFines.Count == 4);
            Assert.True(ValidFines.Any(f => f.CustomerId == custBook.CustomerId));
        }

        [Fact]
        public void RenewnBookRental_ValidDate_Successfull()
        {
            // Arrange
            var custBook = ValidCustomerBooks[0];
            var today = DateTime.Now;
            custBook.DueDate = today;
            var custBookIds = new[] { custBook.Id };
            
            ValidFines = ValidFines
                .Where(f => f.CustomerId == custBook.CustomerId)
                .Select(f => { f.Status = FineStatus.Paid; return f; })
                .ToList();

            // Act
            var resultDto = _service.RenewBookRental(custBookIds);

            // Assert
            Assert.True(ValidCustomerBooks
                .Where(cb => cb.Id == custBookIds[0])
                .Single()
                    .DueDate > today);
        }

        [Theory]
        [InlineData(1)]
        public void GetWaitingList_GivenValidId_ReturnCorrectData(int bookId)
        {
            // Act
            var books = _service.GetWaitingList(bookId);

            // Assert            
            Assert.True(
                books.All(b => b.Status == WaitingQueue));
        }

        [Fact]
        public void AddToWaitingList_ValidCustomer_Successfull()
        {
            // Arrange
            var requestDTO = RentBooksRequest;
            ValidCustomerBooks = ValidCustomerBooks
                .Where(cb => cb.CustomerId == requestDTO.CustomerId)
                .Select(cb => { cb.Status = Returned; return cb; })
                .ToList();
            ValidBooks.ForEach(b => { b.Quantity = 0; });
            ValidFines = ValidFines
                .Where(f => f.CustomerId == requestDTO.CustomerId)
                .Select(f => { f.Status = FineStatus.Paid; return f; })
                .ToList();

            // Act
            var resultDto = _service.AddToWaitingList(requestDTO);

            // Assert
            Assert.True(ValidCustomerBooks.Count == 4);
            Assert.True(ValidCustomerBooks.Any(cb => cb.BookId == requestDTO.BookIds[0] && 
                                                     cb.CustomerId == requestDTO.CustomerId &&
                                                     cb.Status == WaitingQueue));
        }

        [Fact]
        public void RemoveFromWaitingList_ValidId_RemovesFromCollection()
        {
            var waitedBook = ValidCustomerBooks[1];

            var result = _service.RemoveFromWaitingList(waitedBook.Id);

            Assert.True(result);
            Assert.DoesNotContain(waitedBook, ValidCustomerBooks);
        }

        [Fact]
        public void RemoveFromWaitingQueueByCustomerAndBook_ValidId_RemovesFromCollection()
        {
            var waitedCustomerBook = ValidCustomerBooks[1];

            var result = _service.RemoveFromWaitingQueueByCustomerAndBook(
                waitedCustomerBook.CustomerId,
                waitedCustomerBook.BookId);

            Assert.True(result);
            Assert.DoesNotContain(waitedCustomerBook, ValidCustomerBooks);
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
                (ValidCustomerBook.StartDate, ValidCustomerBook.DueDate);

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
            var validCustomerBookDto = ValidCustomerBookDTO;

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
            var validCustomerBookDto = ValidCustomerBookDTO;

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

            var validCustomerBookDto = ValidCustomerBookDTO;
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
            var validCustomerBook = ValidCustomerBook;

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
            var validCustomerBookDTO = ValidCustomerBookDTO;

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