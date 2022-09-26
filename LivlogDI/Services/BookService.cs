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

        public BookDTO Create(BookDTO bookDTO)
        {
            var book = CreateEntity(bookDTO);

            book = _repo.Add(book);

            return CreateDTO(book);
        }

        public BookDTO Update(int bookId, BookDTO bookDTO)
        {
            var book = _repo.Get(bookId);

            book.Title = bookDTO.Title;
            book.ISBN = bookDTO.ISBN;
            book.Quantity = bookDTO.Quantity;

            var updatedBook =
                _repo.Update(book);

            return CreateDTO(updatedBook);
        }

        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }

        #region Helper Methods

        public BookDTO CreateDTO(Book book)
        {
            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Quantity = book.Quantity
            };
        }

        public IEnumerable<BookDTO> CreateDTOs(IEnumerable<Book> books)
        {
            var booksDtos = new List<BookDTO>();

            foreach (var book in books)
            {
                booksDtos.Add(CreateDTO(book));
            }

            return booksDtos;
        }

        public Book CreateEntity(BookDTO dto)
        {
            return new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                ISBN = dto.ISBN,
                Quantity = dto.Quantity
            };
        }

        public IList<BookDTO> FilterByIds(IEnumerable<BookDTO> books, IList<int> ids)
        {
            return books
                .Where(b => ids.Contains(b.Id))
                .ToList();
        }

        public int GetBookQuantity(BookDTO bookDTO)
        {
            return bookDTO.Quantity;
        }

        #endregion
    }
}
