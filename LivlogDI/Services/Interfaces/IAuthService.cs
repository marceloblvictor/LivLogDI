using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;

namespace LivlogDI.Services.Interfaces
{
    public interface IAuthService
    {
        bool ArePasswordsEqual(string inputPassword, string dbPassword);
        string Authenticate(UserDTO dto);
        UserDTO CreateDTO(User user);
        IEnumerable<UserDTO> CreateDTOs(IEnumerable<User> users);
        User CreateEntity(UserDTO dto);
        bool Delete(int id);
        IList<UserDTO> FilterUserByUsername(IEnumerable<UserDTO> users, string username);
        string GenerateToken(UserDTO user);
        UserDTO Get(int userId);
        IEnumerable<UserDTO> GetAll();
        UserDTO GetUserByUsername(IEnumerable<UserDTO> users, string username);
        bool IsUserValid(IEnumerable<UserDTO> users, UserDTO dto);
        UserDTO SignUp(UserDTO dto);
    }
}