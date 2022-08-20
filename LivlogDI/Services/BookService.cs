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

            return CreateDTOs(books);
        }

        private BookDTO CreateDTO(Book book)
        {
            return new BookDTO();
        }

        private IEnumerable<BookDTO> CreateDTOs(IEnumerable<Book> books)
        {
            var booksDtos = new List<BookDTO>();

            //foreach (var book in books)
            //{
            //    booksDtos.Add(new BookDTO
            //    {
            //        Id = book.Id,
            //        Title = book.Title,
            //        ISSBN = book.ISSBN,
            //        PagesQuantity = book.PagesQuantity
            //    });
            //}

            return booksDtos;
        }
    }
}
