using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LivlogDITests.Services
{
    public class BookRepositoryTest
    {
        List<Book> Books = new List<Book>
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

        public BookRepositoryTest()
        {
        }

        [Fact]
        public void GetAllBooksOrderedDescendingById()
        {
            // Arrange
            var query = Books.AsQueryable();
            
            var mockBooks = new Mock<DbSet<Book>>();
            mockBooks.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(query.Provider);
            mockBooks.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(query.Expression);
            mockBooks.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(query.ElementType);
            mockBooks.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(query.GetEnumerator());

            var mockDbContext = new Mock<LivlogDIContext>();
            mockDbContext.Setup(ctx => ctx.Books).Returns(mockBooks.Object);

            var booksRepository = new BookRepository(mockDbContext.Object);

            // Act
            var books = booksRepository.GetAll();

            // Assert
            Assert.Equal(3, books.Count());
            Assert.Equal(3, books[0].Id);
            Assert.Equal(2, books[1].Id);
            Assert.Equal(1, books[2].Id);
        }
    }
}