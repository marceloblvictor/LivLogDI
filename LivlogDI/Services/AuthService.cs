using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LivlogDI.Constants;
using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services.Interfaces;
using LivlogDI.Validators.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LivlogDI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IAuthValidator _validator;


        public AuthService(
            IUserRepository repo,
            IAuthValidator validator)
        {
            _repo = repo;
            _validator = validator;
        }

        public UserDTO Get(int userId)
        {
            var user = _repo.Get(userId);

            return CreateDTO(user);
        }

        public IEnumerable<UserDTO> GetAll()
        {
            var users = _repo.GetAll();

            return CreateDTOs(users);
        }

        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }

        public UserDTO SignUp(UserDTO dto)
        {
            _validator.ValidateNewUser(GetAll(), dto);

            var user = _repo.Add(CreateEntity(dto));

            return Get(user.Id);
        }

        public string Authenticate(UserDTO dto)
        {
            var users = GetAll();

            if (!IsUserValid(users, dto))
            {
                throw new Exception("Nome de usuário e/ou senha inválidos");
            }

            dto = GetUserByUsername(GetAll(), dto.Username);

            return GenerateToken(dto);
        }


        #region Helper Methods

        public UserDTO GetUserByUsername(IEnumerable<UserDTO> users, string username)
        {
            return users
                .Where(u => u.Username == username)
                .SingleOrDefault();
        }

        public bool IsUserValid(
            IEnumerable<UserDTO> users,
            UserDTO dto)
        {
            var user =
                FilterUserByUsername(
                    users,
                    dto.Username)
                .SingleOrDefault();

            if (user is null)
            {
                return false;
            }

            if (!ArePasswordsEqual(dto.Password, user.Password))
            {
                return false;
            };

            return true;
        }

        public bool ArePasswordsEqual(string inputPassword, string dbPassword)
        {
            return inputPassword.Equals(dbPassword);
        }

        public IList<UserDTO> FilterUserByUsername(
            IEnumerable<UserDTO> users,
            string username)
        {
            return users.Where(u => u.Username == username).ToList();
        }

        public string GenerateToken(UserDTO user)
        {
            _validator.ValidateUserClaimsData(user);

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JWT.JWT_KEY));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var token = new JwtSecurityToken(
                JWT.JWT_ISSUER,
                JWT.JWT_AUDIENCE,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public UserDTO CreateDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };
        }

        public IEnumerable<UserDTO> CreateDTOs(IEnumerable<User> users)
        {
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                userDtos.Add(CreateDTO(user));
            }

            return userDtos;
        }

        public User CreateEntity(UserDTO dto)
        {
            return new User
            {
                Id = dto.Id,
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email
            };
        }

        #endregion
    }
}
