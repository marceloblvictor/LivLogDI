using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User Add(User user);
        bool Delete(int userId);
        User Get(int userId);
        List<User> GetAll();
        User Update(User user);
    }
}