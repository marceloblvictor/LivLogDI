using LivlogDI.Models.DTO;

namespace LivlogDI.Validators.Interfaces
{
    public interface IAuthValidator
    {
        void ValidateNewUser(IEnumerable<UserDTO> users, UserDTO dto);
        void ValidateUserClaimsData(UserDTO user);
    }
}