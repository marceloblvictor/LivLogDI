using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Validators;
using static LivlogDI.Enums.BookRentalStatus;


namespace LivlogDITests.ValidatorsTests
{
    public class BookRentalValidatorTest
    {
        BookRentalValidator _validator { get; set; }
        
        IList<UserDTO> ValidUsersDTOs = new List<UserDTO>()
        {
            new()
            {
                Id = 1,
                Username = "marceloblvictor",
                Password = "abcdef1234",
                Email = "marceloblvictor@gmail.com"
            },
            new()
            {
                Id = 2,
                Username = "marceloblvictorXXX",
                Password = "abcdef1234",
                Email = "marceloblvictorXXX@gmail.com"
            },
            new()
            {
                Id = 3,
                Username = "marceloblvictorYYY",
                Password = "abcdef1234",
                Email = "marceloblvictorYYY@gmail.com"
            },
            new()
            {
                Id = 4,
                Username = "marceloblvictorZZZ",
                Password = "abcdef1234",
                Email = "marceloblvictorZZZ@gmail.com"
            },
        };

        IList<FineDTO> ValidFinesDTOs = new List<FineDTO>()
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

        CustomerDTO ValidCustomerDTO { get; set; } = new()
        {
            Id = 1,
            Name = "marceloblvictor",
            Phone = "98534542767",
            Email = "marceloblvictor@gmail.com",
            Category = (CustomerCategory)1
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

        IList<BookDTO> ValidBooksDTOs = new List<BookDTO>()
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

        public BookRentalValidatorTest()
        {
            _validator = new BookRentalValidator();
        }

        [Fact]
        public void ValidateCustomerNotInDebt_NoFinesFromCustomer_ThrowsNoException()
        {
            // Arrange
            var validCustomer = ValidCustomerDTO;
            validCustomer.Id = 2;
            var allFines = ValidFinesDTOs;

            // Act
            var validation = () => _validator.ValidateCustomerNotInDebt(allFines, validCustomer);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateCustomerNotInDebt_CustomerHasFines_ThrowsException()
        {
            // Arrange
            var validCustomer = ValidCustomerDTO;
            var allFines = ValidFinesDTOs;

            // Act
            var validation = () => _validator.ValidateCustomerNotInDebt(allFines, validCustomer);
            
            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateCustomerRentalsLimit_BooksAmountLessThanLimit_ThrowsNoException()
        {
            // Arrange
            var books = ValidBooksDTOs;
            var activeCustomerBooks = ValidCustomerBooksDTOs;
            int customerLimit = 7;

            // Act
            var validation = () => _validator.ValidateCustomerRentalsLimit(
                activeCustomerBooks, books, customerLimit);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateCustomerRentalsLimit_BooksAmountBiggerThanLimit_ThrowsException()
        {
            // Arrange
            var books = ValidBooksDTOs;
            var activeCustomerBooks = ValidCustomerBooksDTOs;
            int customerLimit = 1;

            // Act
            var validation = () => _validator.ValidateCustomerRentalsLimit(
                activeCustomerBooks, books, customerLimit);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateBookAvailability_GivenLessAmountThanQuantity_ThrowsNoException()
        {
            // Arrange
            var books = ValidBooksDTOs;
            var rentedBooks = ValidCustomerBooksDTOs;

            foreach (var book in books)
            {
                book.Quantity = 100;
            }

            // Act
            var validation = () => _validator.ValidateBookAvailability(
                books, rentedBooks);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateBookAvailability_GivenHigherAmountThanQuantity_ThrowsException()
        {
            // Arrange
            var books = ValidBooksDTOs;
            var rentedBooks = ValidCustomerBooksDTOs;

            foreach (var book in books)
            {
                book.Quantity = 0;
            }

            // Act
            var validation = () => _validator.ValidateBookAvailability(
                books, rentedBooks);

            // Assert            
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateCustomerBooksSameCustomer_IfSameCustomer_ThrowsNoException()
        {
            // Arrange
            var rentedBooks = ValidCustomerBooksDTOs;                        

            // Act
            var validation = () => _validator.ValidateCustomerBooksSameCustomer(rentedBooks);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateCustomerBooksSameCustomer_IfNotSameCustomer_ThrowsException()
        {
            // Arrange
            var rentedBooks = ValidCustomerBooksDTOs;
            rentedBooks[0].CustomerId = 99;

            // Act
            var validation = () => _validator.ValidateCustomerBooksSameCustomer(rentedBooks);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateReturnedBookStatus_IfStatusIsActive_ThrowsNoException()
        {
            // Arrange
            var returnedBook = ValidCustomerBooksDTOs[0];
            returnedBook.Status = Active;

            // Act
            var validation = () => _validator.ValidateReturnedBookStatus(returnedBook);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateReturnedBookStatus_IfStatusIsWaitingQueue_ThrowsException()
        {
            // Arrange
            var returnedBook = ValidCustomerBooksDTOs[0];
            returnedBook.Status = WaitingQueue;

            // Act
            var validation = () => _validator.ValidateReturnedBookStatus(returnedBook);            

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateReturnedBookStatus_IfStatusIsReturned_ThrowsException()
        {
            // Arrange
            var returnedBook = ValidCustomerBooksDTOs[0];
            returnedBook.Status = Returned;

            // Act
            var validation = () => _validator.ValidateReturnedBookStatus(returnedBook);            

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateBookWaitingQueue_IfNoCustomerInWaitingQueue_ThrowsNoException()
        {
            // Arrange
            var bookWaitingList = ValidCustomerBooksDTOs;

            foreach (var book in bookWaitingList)
            {
                book.Status = WaitingQueue;
            }

            var fines = ValidFinesDTOs;
            int customerId = 1;

            // Act
            var validation = () => _validator.ValidateBookWaitingQueue(
                bookWaitingList, fines, customerId);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateBookWaitingQueue_IfHasCustomerInWaitingQueue_ThrowsException()
        {
            // Arrange
            var bookWaitingList = ValidCustomerBooksDTOs;

            foreach (var book in bookWaitingList)
            {
                book.Status = WaitingQueue;
            }

            var fines = ValidFinesDTOs;
            int customerId = 99;

            // Act
            var validation = () => _validator.ValidateBookWaitingQueue(
                bookWaitingList, fines, customerId);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateAnyFreeBook_HasNotEnoughBooks_ThrowsNoException()
        {
            // Arrange
            var requestedBooks = ValidCustomerBooksDTOs;
            int freeQuantity = 0;

            // Act
            var validation = () => _validator.ValidateAnyFreeBook(requestedBooks, freeQuantity);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateAnyFreeBook_HasEnoughBooks_ThrowsException()
        {
            // Arrange
            var requestedBooks = ValidCustomerBooksDTOs;
            int freeQuantity = 10;

            // Act
            var validation = () => _validator.ValidateAnyFreeBook(requestedBooks, freeQuantity);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateRenewalOnlyInDueDate_InDueDate_ThrowsNoException()
        {
            // Arrange
            var renewedBooks = ValidCustomerBooksDTOs;
            
            foreach (var book in renewedBooks)
            {
                book.DueDate = DateTime.Now;
            }

            // Act
            var validation = () => _validator.ValidateRenewalOnlyInDueDate(renewedBooks);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateRenewalOnlyInDueDate_NotInDueDate_ThrowsException()
        {
            // Arrange
            var renewedBooks = ValidCustomerBooksDTOs;
            renewedBooks[0].DueDate = new DateTime(2023, 01, 01);

            // Act
            var validation = () => _validator.ValidateRenewalOnlyInDueDate(renewedBooks);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateIfCustomerIsAlreadyInQueueOrHasTheBook_RegularCustomer_ThrowsNoException()
        {
            // Arrange
            var customer = ValidCustomerDTO;
            var allCustomerBooks = ValidCustomerBooksDTOs;
            int bookId = 10;

            // Act
            var validation = () => _validator.ValidateIfCustomerIsAlreadyInQueueOrHasTheBook(
                customer, bookId, allCustomerBooks);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateIfCustomerIsAlreadyInQueueOrHasTheBook_CustomerWaiting_ThrowsException()
        {
            // Arrange
            var customer = ValidCustomerDTO;
            var allCustomerBooks = ValidCustomerBooksDTOs;
            allCustomerBooks[0].Status = WaitingQueue;
            int bookId = 1;

            // Act
            var validation = () => _validator.ValidateIfCustomerIsAlreadyInQueueOrHasTheBook(
                customer, bookId, allCustomerBooks);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateIfCustomerIsAlreadyInQueueOrHasTheBook_CustomerInPossession_ThrowsException()
        {
            // Arrange
            var customer = ValidCustomerDTO;
            var allCustomerBooks = ValidCustomerBooksDTOs;
            allCustomerBooks[0].Status = Active;
            int bookId = 1;

            // Act
            var validation = () => _validator.ValidateIfCustomerIsAlreadyInQueueOrHasTheBook(
                customer, bookId, allCustomerBooks);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateWaitedBookStatus_GivenWaitedBook_ThrowsNoException()
        {
            // Arrange
            var returnedBook = ValidCustomerBooksDTOs[0];
            returnedBook.Status = WaitingQueue;

            // Act
            var validation = () => _validator.ValidateWaitedBookStatus(returnedBook);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [InlineData(Active)]
        [InlineData(Returned)]
        [Theory]
        public void ValidateWaitedBookStatus_GivenNotWaitedBook_ThrowsNoException(BookRentalStatus status)
        {
            // Arrange
            var returnedBook = ValidCustomerBooksDTOs[0];
            returnedBook.Status = status;

            // Act
            var validation = () => _validator.ValidateWaitedBookStatus(returnedBook);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }
    }
}