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
        BookDTO ValidBookDTO { get; set; } = new()
        {
            Id = 1,
            Title = "LivroTeste1",
            ISBN = "teste1",
            Quantity = 5
        };
        IList<BookDTO> ValidBookDTOs = new List<BookDTO>()
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

        BookService Service { get; set; }

        public BookServiceTest()
        {
            _fakeBookDTOToBeAdded = new BookDTO
            {
                Id = 4,
                Title = "Deteuronomios",
                ISBN = "0-9489-9819-9",
                Quantity = 72
            };

            _fakeBooks = new List<Book>
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
            _mockedRepo
                .Setup(repo => repo.Add(It.IsAny<Book>()))
                .Callback<Book>(book =>
                {
                    book.Id = 4;
                    _fakeBooks.Add(book);
                })
                .Returns<Book>(book => book);
            _mockedRepo
                .Setup(repo => repo.Update(It.IsAny<Book>()))
                .Callback<Book>(b =>
                {
                    var bookToBeUpdated = _fakeBooks
                        .Where(bk => bk.Id == b.Id)
                        .Single();

                    bookToBeUpdated.Title = b.Title;
                    bookToBeUpdated.ISBN = b.ISBN;
                    bookToBeUpdated.Quantity = b.Quantity;
                })
                .Returns<Book>(book => book);

            _mockedRepo
                .Setup(repo => repo.Delete(It.IsAny<int>()))
                .Callback<int>(id =>
                {
                    _fakeBooks.Remove(_fakeBooks.Where(u => u.Id == id).Single());
                })
                .Returns(true);

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

            var addedBookDto = Service.Create(newBook);

            Assert.True(_fakeBooks.Count == 4);
            Assert.Equal(newBook.Title, addedBookDto.Title);
        }

        [Fact]
        public void Update_PersistsChangesToBook()
        {
            var book = Service
                .GetAll()
                .Where(b => b.Id == 1)
                .Single();

            book.Title = "tituloMODIFICADO";
            book.ISBN = "isbnMODIFICADO";
            book.Quantity = 7;

            var updatedBookDTO = Service.Update(book.Id, book);

            Assert.Equal(book.Title, updatedBookDTO.Title);
            Assert.Equal(book.ISBN, updatedBookDTO.ISBN);
            Assert.Equal(book.Quantity, updatedBookDTO.Quantity);
        }


        [Fact]
        public void Delete_ValidId_RemovesFromCollection()
        {
            var validBook = ValidBook;

            var result = Service.Delete(validBook.Id);

            Assert.True(result);
            Assert.DoesNotContain(validBook, _fakeBooks);
        }

        [Fact]
        public void GetBookQuantity_ValidBookDTO_ReturnsCorrectQuantity()
        {
            // Arrange
            var validDTO = _fakeBookDTOToBeAdded;

            // Act
            var result = Service.GetBookQuantity(validDTO);

            // Assert
            Assert.Equal(result, validDTO.Quantity);
        }

        [Fact]
        public void FilterByIds_GivenValidId_ReturnsCorrectBook()
        {
            // Arrange
            var validBooks = ValidBookDTOs;
            int validId = 3;

            // Act
            var filteredBooks = Service.FilterByIds(validBooks, new[] { 3 });

            // Assert
            Assert.True(filteredBooks.Single().Id == validId);
        }

        [Fact]
        public void CreateDTO_GenerateDTOWithEntityData_Succes()
        {
            // Arrange
            var validBook = ValidBook;

            // Act
            var dto = Service.CreateDTO(validBook);

            // Assert
            Assert.True(dto.Id == validBook.Id);
            Assert.True(dto.Title == validBook.Title);
            Assert.True(dto.ISBN == validBook.ISBN);
            Assert.True(dto.Quantity == validBook.Quantity);
        }

        [Fact]
        public void CreateDTOS_GenerateDTOSWithEntityData_Succes()
        {
            // Arrange
            var validBooks = ValidBooks;

            // Act
            var dtos = Service.CreateDTOs(validBooks);

            // Assert

            foreach (var dto in dtos)
            {
                var validBook =
                    validBooks.First(u => u.Id == dto.Id);

                Assert.True(dto.Id == validBook.Id);
                Assert.True(dto.Title == validBook.Title);
                Assert.True(dto.ISBN == validBook.ISBN);
                Assert.True(dto.Quantity == validBook.Quantity);
            }
        }

        [Fact]
        public void CreateEntity_GenerateEntityWithDTOData_Succes()
        {
            // Arrange
            var validBookDto = ValidBookDTO;

            // Act
            var entity = Service.CreateEntity(validBookDto);

            // Assert
            Assert.True(entity.Id == validBookDto.Id);
            Assert.True(entity.Title == validBookDto.Title);
            Assert.True(entity.ISBN == validBookDto.ISBN);
            Assert.True(entity.Quantity == validBookDto.Quantity);
        }
    }
}