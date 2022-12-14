using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
            return _dbContext.Books
                .AsNoTracking()
                .OrderByDescending(b => b.Id)
                .ToList();
        }

        public Book Get(int bookId)
        {
            return _dbContext.Books
                .AsNoTracking()
                .Where(b => b.Id == bookId)
                .FirstOrDefault()
                    ?? throw new ArgumentException("There is no record with the given id");
        }

        public Book Add(Book book)
        {
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            return book;
        }

        public Book Update(Book book)
        {
            _dbContext.Books.Update(book);
            _dbContext.SaveChanges();

            book = Get(book.Id);

            return book;
        }

        public bool Delete(int bookId)
        {
            var book = Get(bookId);            

            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
