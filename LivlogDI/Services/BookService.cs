using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services.Interfaces;

namespace LivlogDI.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;

        public BookService(IBookRepository repo)
        {
            _repo = repo;
        }

        public BookDTO Get(int bookId)
        {
            var book = _repo.Get(bookId);

            return CreateDTO(book);
        }

        public IEnumerable<BookDTO> GetAll()
        {
            var books = _repo.GetAll();

            return books.Count > 0 ? 
                CreateDTOs(books)
                : new List<BookDTO>();
        }

        public BookDTO Add(BookDTO bookDTO)
        {
            var book = CreateEntity(bookDTO);

            book = _repo.Add(book);

            return CreateDTO(book);
        }

        private BookDTO CreateDTO(Book book)
        {
            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                ISSBN = book.ISSBN,
                PagesQuantity = book.PagesQuantity
            };
        }

        private IEnumerable<BookDTO> CreateDTOs(IEnumerable<Book> books)
        {
            var booksDtos = new List<BookDTO>();

            foreach (var book in books)
            {
                booksDtos.Add(CreateDTO(book));
            }

            return booksDtos;
        }

        private Book CreateEntity(BookDTO bookDTO)
        {
            return new Book
            {
                Id = bookDTO.Id,
                Title = bookDTO.Title,
                ISSBN = bookDTO.ISSBN,
                PagesQuantity = bookDTO.PagesQuantity
            };
        }
    }
}
