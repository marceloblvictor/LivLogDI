using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LivlogDIContext _dbContext;

        public UserRepository(
            LivlogDIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<User> GetAll()
        {
            return _dbContext.Users
                .OrderByDescending(b => b.Id)
                .ToList();
        }

        public User Get(int userId)
        {
            return _dbContext.Users
                .Where(u => u.Id == userId)
                .SingleOrDefault()
                    ?? throw new ArgumentException();
        }

        public User Add(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            user = Get(user.Id);

            return user;
        }

        public User Update(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            user = Get(user.Id);

            return user;
        }

        public bool Delete(int userId)
        {
            var user = Get(userId);

            if (user is null)
            {
                throw new ArgumentException();
            }

            _dbContext.Remove(user);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
