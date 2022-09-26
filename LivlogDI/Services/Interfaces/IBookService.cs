using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;

namespace LivlogDI.Services.Interfaces
{
    public interface IBookService
    {
        BookDTO Create(BookDTO bookDTO);
        BookDTO CreateDTO(Book book);
        IEnumerable<BookDTO> CreateDTOs(IEnumerable<Book> books);
        Book CreateEntity(BookDTO dto);
        bool Delete(int id);
        IList<BookDTO> FilterByIds(IEnumerable<BookDTO> books, IList<int> ids);
        BookDTO Get(int bookId);
        IEnumerable<BookDTO> GetAll();
        int GetBookQuantity(BookDTO bookDTO);
        BookDTO Update(int bookId, BookDTO bookDTO);
    }
}