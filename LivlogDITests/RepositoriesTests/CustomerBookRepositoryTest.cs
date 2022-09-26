using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using static LivlogDI.Enums.BookRentalStatus;

namespace LivlogDITests.RepositoriesTests
{
    public class CustomerBookRepositoryTest
    {
        CustomerBook ValidCustomerBookActive { get; set; } = new()
        {
            Id = 4,
            BookId = 1,
            CustomerId = 1,
            StartDate = new DateTime(2023, 09, 01),
            DueDate = new DateTime(2023, 10, 01),
            Status = Active
        };

        Mock<LivlogDIContext> _mockedDbContext { get; set; }
        List<CustomerBook> _mockedCustomerBooks { get; set; } = new List<CustomerBook>
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

        CustomerBookRepository Repository { get; set; }

        public CustomerBookRepositoryTest()
        {            
            var query = _mockedCustomerBooks.AsQueryable();

            var mockCustBooks = new Mock<DbSet<CustomerBook>>();
            mockCustBooks
                .As<IQueryable<CustomerBook>>()
                .Setup(m => m.Provider)
                .Returns(query.Provider);
            mockCustBooks
                .As<IQueryable<CustomerBook>>()
                .Setup(m => m.Expression)
                .Returns(query.Expression);
            mockCustBooks
                .As<IQueryable<CustomerBook>>()
                .Setup(m => m.ElementType)
                .Returns(query.ElementType);
            mockCustBooks
                .As<IQueryable<CustomerBook>>()
                .Setup(m => m.GetEnumerator())
                .Returns(query.GetEnumerator());
            
            mockCustBooks
                .Setup(m => m.Add(It.IsAny<CustomerBook>()))
                .Callback<CustomerBook>((b) => _mockedCustomerBooks.Add(b));
            mockCustBooks
                .Setup(m => m.Remove(It.IsAny<CustomerBook>()))
                .Callback<CustomerBook>((b) => { _mockedCustomerBooks.Remove(b); });
            mockCustBooks
               .Setup(m => m.Update(It.IsAny<CustomerBook>()))
               .Callback<CustomerBook>((b) =>
               {
                   var custBookToBeUpdated = _mockedCustomerBooks
                    .AsQueryable()
                    .Where(custBook => custBook.Id == b.Id)
                    .Single();

                   custBookToBeUpdated.StartDate = b.StartDate;
                   custBookToBeUpdated.DueDate = b.DueDate;
                   custBookToBeUpdated.Status = b.Status;
               });

            _mockedDbContext = new Mock<LivlogDIContext>();
            _mockedDbContext
                .Setup(ctx => ctx.CustomerBooks)
                .Returns(mockCustBooks.Object);

            Repository = new CustomerBookRepository(_mockedDbContext.Object);
        }

        [Fact]
        public void GetAll_BooksAreOrderedDescendingById()
        {            
            // Act
            var books = Repository.GetAll();

            // Assert
            Assert.Equal(3, books.Count());
            Assert.Equal(3, books[0].Id);
            Assert.Equal(2, books[1].Id);
            Assert.Equal(1, books[2].Id);
        }

        [Theory]
        [InlineData(1)]
        public void GetABookWithAValidId_IsSuccess(int validID)
        {
            // Act
            var book = Repository.Get(validID);

            // Assert            
            Assert.NotNull(book);
            Assert.Equal(validID, book.Id);
        }

        [Theory]
        [InlineData(99)]
        public void GetABookWithAnInvalidId_IsFailure(int invalidId)
        {
            // Act
            var getBookOperation = 
                new Func<CustomerBook>(() => Repository.Get(invalidId));

            // Assert
            Assert.Throws<ArgumentException>(getBookOperation);
        }

        [Fact]
        public void CreateABook_PersistsNewBook()
        {
            var newCustBook = ValidCustomerBookActive;

            Repository.Add(newCustBook);

            Assert.True(_mockedDbContext.Object.CustomerBooks.Count() == 4);
            Assert.True(_mockedDbContext.Object.CustomerBooks
                            .Where(b => b.Id == newCustBook.Id)
                            .FirstOrDefault()?
                                .Id == newCustBook.Id);
        }

        [Fact]
        public void Update_GivenNewBookData_PersistsNewBook()
        {
            var updatedData = _mockedCustomerBooks[0];

            updatedData.StartDate = DateTime.Now;
            updatedData.DueDate = DateTime.Now;
            updatedData.Status = Returned;

            Repository.Update(updatedData);

            var updatedBook = Repository
                .GetAll()
                .FirstOrDefault(u => u.Id == updatedData.Id);

            Assert.True(updatedBook is not null);
            Assert.True(updatedBook.StartDate == updatedData.StartDate);
            Assert.True(updatedBook.DueDate == updatedData.DueDate);
            Assert.True(updatedBook.Status == updatedData.Status);
        }

        [Fact]
        public void Delete_GivenValidBookId_DeleteBook()
        {
            var bookToBeDeleted = _mockedCustomerBooks[0];

            Repository.Delete(bookToBeDeleted.Id);

            Assert.DoesNotContain(bookToBeDeleted, _mockedCustomerBooks);
        }

        [Fact]
        public void Delete_GivenInvalidBookId_ThrowsException()
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