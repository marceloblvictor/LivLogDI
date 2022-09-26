using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivlogDI.Data.Repositories
{
    public class FineRepository : IFineRepository
    {
        private readonly LivlogDIContext _dbContext;

        public FineRepository(
            LivlogDIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Fine> GetAll()
        {
            return _dbContext.Fines
                .Include(f => f.Customer)
                .OrderByDescending(b => b.Id)
                .ToList();
        }

        public Fine Get(int fineId)
        {
            return _dbContext.Fines
                .Include(f => f.Customer)
                .Where(f => f.Id == fineId)
                .SingleOrDefault()
                    ?? throw new ArgumentException();
        }

        public Fine Add(Fine fine)
        {
            _dbContext.Fines.Add(fine);
            _dbContext.SaveChanges();

            fine = Get(fine.Id);

            return fine;
        }

        public Fine Update(Fine fine)
        {
            _dbContext.Fines.Update(fine);
            _dbContext.SaveChanges();

            fine = Get(fine.Id);

            return fine;
        }

        public bool Delete(int fineId)
        {
            var fine = Get(fineId);

            if (fine is null)
            {
                throw new ArgumentException();
            }

            _dbContext.Remove(fine);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
