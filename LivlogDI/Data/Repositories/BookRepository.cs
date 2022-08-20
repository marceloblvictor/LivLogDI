using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LivlogDIContext _dbContext;

        public BookRepository(LivlogDIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Book> GetAll()
        {
            return new List<Book>();
        }

        public Book Get(int bookId)
        {
            return new Book();
        }
    }
}
