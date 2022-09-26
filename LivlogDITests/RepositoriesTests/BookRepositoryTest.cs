using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LivlogDITests.RepositoriesTests
{
    public class BookRepositoryTest
    {
        Mock<LivlogDIContext> _mockedDbContext { get; set; }
        List<Book> _mockedBooks { get; set; }

        BookRepository Repository { get; set; }

        public BookRepositoryTest()
        {
            _mockedBooks = new List<Book>
            {
                new Book()
                {
                    Id = 1,
                    Title = "As tranças do rei careca",
                    ISBN = "0-2918-9948-X",
                    Quantity = 99
                },
                new Book()
                {
                    Id = 2,
                    Title = "Batleby, o escrivão",
                    ISBN = "0-6392-2838-0",
                    Quantity = 132
                },
                new Book()
                {
                    Id = 3,
                    Title = "Coraline",
                    ISBN = "0-3543-4529-X",
                    Quantity = 272
                },
            };

            var query = _mockedBooks.AsQueryable();

            var mockBooks = new Mock<DbSet<Book>>();
            mockBooks
                .As<IQueryable<Book>>()
                .Setup(m => m.Provider)
                .Returns(query.Provider);
            mockBooks
                .As<IQueryable<Book>>()
                .Setup(m => m.Expression)
                .Returns(query.Expression);
            mockBooks
                .As<IQueryable<Book>>()
                .Setup(m => m.ElementType)
                .Returns(query.ElementType);
            mockBooks
                .As<IQueryable<Book>>()
                .Setup(m => m.GetEnumerator())
                .Returns(query.GetEnumerator());
            
            mockBooks
                .Setup(m => m.Add(It.IsAny<Book>()))
                .Callback<Book>((b) => _mockedBooks.Add(b));
            mockBooks
                .Setup(m => m.Remove(It.IsAny<Book>()))
                .Callback<Book>((b) => { _mockedBooks.Remove(b); });
            mockBooks
               .Setup(m => m.Update(It.IsAny<Book>()))
               .Callback<Book>((b) =>
               {
                   var bookToBeUpdated = _mockedBooks
                    .AsQueryable()
                    .Where(user => user.Id == b.Id)
                    .Single();

                   bookToBeUpdated.Title = b.Title;
                   bookToBeUpdated.ISBN = b.ISBN;
                   bookToBeUpdated.Quantity = b.Quantity;
               });

            _mockedDbContext = new Mock<LivlogDIContext>();
            _mockedDbContext
                .Setup(ctx => ctx.Books)
                .Returns(mockBooks.Object);

            Repository = new BookRepository(_mockedDbContext.Object);
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
                new Func<Book>(() => Repository.Get(invalidId));

            // Assert
            Assert.Throws<ArgumentException>(getBookOperation);
        }

        [Fact]
        public void CreateABook_PersistsNewBook()
        {
            var newBook = new Book
            {
                Id = 4,
                Title = "Deteuronomios",
                ISBN = "0-9489-9819-9",
                Quantity = 72
            };

            Repository.Add(newBook);
            

            Assert.True(_mockedDbContext.Object.Books.Count() == 4);
            Assert.True(_mockedDbContext.Object.Books
                            .Where(b => b.Id == newBook.Id)
                            .FirstOrDefault()?
                                .Title == newBook.Title);
            Assert.True(_mockedDbContext.Object.Books.Any(b => b.Id == newBook.Id));
        }

        [Fact]
        public void Update_GivenNewBookData_PersistsNewUser()
        {
            var updatedData = new Book
            {
                Id = 1,
                Title = "DeteuronomiosMODIFICADO",
                ISBN = "0-9489-9819-9MODIFICADO",
                Quantity = 4
            };

            Repository.Update(updatedData);

            var updatedBook = Repository
                .GetAll()
                .FirstOrDefault(u => u.Id == updatedData.Id);

            Assert.True(updatedBook is not null);
            Assert.True(updatedBook.Title == updatedData.Title);
            Assert.True(updatedBook.ISBN == updatedData.ISBN);
            Assert.True(updatedBook.Quantity == updatedData.Quantity);
        }

        [Fact]
        public void Delete_GivenValidBookId_DeleteBook()
        {
            var bookToBeDeleted = _mockedBooks[0];

            Repository.Delete(bookToBeDeleted.Id);

            Assert.DoesNotContain(bookToBeDeleted, _mockedBooks);
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