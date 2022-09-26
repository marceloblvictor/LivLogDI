using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Book Add(Book book);
        bool Delete(int bookId);
        Book Get(int bookId);
        List<Book> GetAll();
        Book Update(Book book);
    }
}