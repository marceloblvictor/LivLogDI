using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;
using Moq;

namespace LivlogDITests.ServicesTests
{
    public class BookServiceTest
    {
        Mock<IBookRepository> _mockedRepo { get; set; }
        List<Book> _fakeBooks { get; set; }
        BookDTO _fakeBookDTOToBeAdded { get; set; }

        BookService Service { get; set; }

        public BookServiceTest()
        {
            _fakeBookDTOToBeAdded = new BookDTO
            {
                Id = 4,
                Title = "Deteuronomios",
                ISSBN = "0-9489-9819-9",
                PagesQuantity = 72
            };

            _fakeBooks = new List<Book>
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

            _mockedRepo = new Mock<IBookRepository>();
            _mockedRepo
                .Setup(repo => repo.GetAll())
                .Returns(_fakeBooks);
            _mockedRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns<int>(id => _fakeBooks.Where(b => b.Id == id).Single());
            _mockedRepo
                .Setup(repo => repo.Add(It.IsAny<Book>()))
                .Callback<Book>(book =>
                {
                    book.Id = 4;
                    _fakeBooks.Add(book);
                })
                .Returns<Book>(book => book);

            Service = new BookService(_mockedRepo.Object);
        }

        [Fact]
        public void GetAll_BooksAreOrderedDescendingById()
        {
            // Act
            var books = Service.GetAll().ToList();

            // Assert
            Assert.Equal(3, books.Count());
            Assert.True(books.Any(b => b.Id == _fakeBooks[0].Id));
            Assert.True(books.Any(b => b.Id == _fakeBooks[1].Id));
            Assert.True(books.Any(b => b.Id == _fakeBooks[2].Id));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetABookWithAValidId_ReturnsCorrectBookDTO(int validID)
        {
            // Act
            var book = Service.Get(validID);

            // Assert            
            Assert.NotNull(book);
            Assert.Equal(validID, book.Id);
        }

        [Fact]
        public void CreateABook_PersistsNewBook()
        {
            var newBook = _fakeBookDTOToBeAdded;

            var addedBookDto = Service.Add(newBook);

            Assert.True(_fakeBooks.Count == 4);
            Assert.Equal(newBook.Title, addedBookDto.Title);
        }
    }
}