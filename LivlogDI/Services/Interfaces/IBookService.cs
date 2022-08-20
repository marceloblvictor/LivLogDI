using LivlogDI.Models.DTO;

namespace LivlogDI.Services.Interfaces
{
    public interface IBookService
    {
        BookDTO Get(int bookId);
        IEnumerable<BookDTO> GetAll();
    }
}