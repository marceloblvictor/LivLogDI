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
                    ISSBN = "0-2918-9948-X",
                    PagesQuantity = 99
                },
                new Book()
                {
                    Id = 2,
                    Title = "Batleby, o escrivão",
                    ISSBN = "0-6392-2838-0",
                    PagesQuantity = 132
                },
                new Book()
                {
                    Id = 3,
                    Title = "Coraline",
                    ISSBN = "0-3543-4529-X",
                    PagesQuantity = 272
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
        [InlineData(2)]
        [InlineData(3)]
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
        [InlineData(999)]
        [InlineData(9999)]
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
                ISSBN = "0-9489-9819-9",
                PagesQuantity = 72
            };

            Repository.Add(newBook);
            

            Assert.True(_mockedDbContext.Object.Books.Count() == 4);
            Assert.True(_mockedDbContext.Object.Books
                            .Where(b => b.Id == newBook.Id)
                            .FirstOrDefault()?
                                .Title == newBook.Title);
            Assert.True(_mockedDbContext.Object.Books.Any(b => b.Id == newBook.Id));
        }
    }
}