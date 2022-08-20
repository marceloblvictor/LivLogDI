using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Book Get(int bookId);
        List<Book> GetAll();
    }
}