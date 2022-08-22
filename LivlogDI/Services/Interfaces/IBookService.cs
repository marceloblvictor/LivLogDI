using LivlogDI.Models.DTO;

namespace LivlogDI.Services.Interfaces
{
    public interface IBookService
    {
        BookDTO Add(BookDTO bookDTO);
        BookDTO Get(int bookId);
        IEnumerable<BookDTO> GetAll();
    }
}