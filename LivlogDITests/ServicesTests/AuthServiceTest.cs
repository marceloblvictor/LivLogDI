using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services;
using LivlogDI.Validators;
using Moq;

namespace LivlogDITests.ServicesTests
{
    public class AuthServiceTest
    {
        AuthService _service { get; set; }

        Mock<IUserRepository> _mockedRepo { get; set; }
        IList<User> _fakeUsers { get; set; }
        UserDTO _fakeUserDTOToBeAdded { get; set; } = new()
        {
            Id = 4,
            Username = "marceloblvictorQQQQ",
            Password = "abcdef1234Q",
            Email = "marceloblvictorQQQQ@gmail.com"
        };

        User ValidUser { get; set; } = new()
        {
            Id = 1,
            Username = "marceloblvictor",
            Password = "abcdef1234",
            Email = "marceloblvictor@gmail.com"
        };

        IList<User> ValidUsers = new List<User>()
        {
            new()
            {
                Id = 1,
                Username = "marceloblvictor",
                Password = "abcdef1234",
                Email = "marceloblvictor@gmail.com"
            },
            new()
            {
                Id = 2,
                Username = "marceloblvictorXXX",
                Password = "abcdef1234",
                Email = "marceloblvictorXXX@gmail.com"
            },
            new()
            {
                Id = 3,
                Username = "marceloblvictorYYY",
                Password = "abcdef1234",
                Email = "marceloblvictorYYY@gmail.com"
            }
        };

        UserDTO ValidUserDTO { get; set; } = new()
        {
            Id = 1,
            Username = "marceloblvictor",
            Password = "abcdef1234",
            Email = "marceloblvictor@gmail.com"
        };

        IList<UserDTO> ValidUsersDTOs = new List<UserDTO>()
        {
            new()
            {
                Id = 1,
                Username = "marceloblvictor",
                Password = "abcdef1234",
                Email = "marceloblvictor@gmail.com"
            },
            new()
            {
                Id = 2,
                Username = "marceloblvictorXXX",
                Password = "abcdef1234",
                Email = "marceloblvictorXXX@gmail.com"
            },
            new()
            {
                Id = 3,
                Username = "marceloblvictorYYY",
                Password = "abcdef1234",
                Email = "marceloblvictorYYY@gmail.com"
            },
            new()
            {
                Id = 4,
                Username = "marceloblvictorZZZ",
                Password = "abcdef1234",
                Email = "marceloblvictorZZZ@gmail.com"
            },
        };

        public AuthServiceTest()
        {
            _fakeUsers = ValidUsers;

            _mockedRepo = new Mock<IUserRepository>();
            _mockedRepo
                .Setup(repo => repo.GetAll())
                .Returns(_fakeUsers.ToList());
            _mockedRepo
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns<int>(id => _fakeUsers.Where(b => b.Id == id).Single());
            _mockedRepo
                .Setup(repo => repo.Add(It.IsAny<User>()))
                .Callback<User>(user =>
                {
                    user.Id = 4;
                    _fakeUsers.Add(user);
                })
                .Returns<User>(user => user);
            _mockedRepo
                .Setup(repo => repo.Delete(It.IsAny<int>()))
                .Callback<int>(id =>
                {
                    _fakeUsers.Remove(_fakeUsers.Where(u => u.Id == id).Single());
                })
                .Returns(true);

            _service = new AuthService(_mockedRepo.Object, new AuthValidator());
        }

        [Fact]
        public void GetAll_Invoked_ReturnedUsersOrderedDescendingById()
        {
            // Act
            var users = _service.GetAll().ToList();

            // Assert
            Assert.Equal(3, users.Count());
            Assert.True(users.Any(b => b.Id == _fakeUsers[0].Id));            
            Assert.True(users.Any(b => b.Id == _fakeUsers[1].Id));
            Assert.True(users.Any(b => b.Id == _fakeUsers[2].Id));
        }

        [Theory]
        [InlineData(1)]
        public void Get_ValidId_ReturnsCorrectUserDTO(int validID)
        {
            // Act
            var user = _service.Get(validID);

            // Assert            
            Assert.NotNull(user);
            Assert.Equal(validID, user.Id);
        }

        [Fact]
        public void SignUp_UserWithValidData_PersistsNewUser()
        {
            var newUser = _fakeUserDTOToBeAdded;

            var addedUserDto = _service.SignUp(newUser);

            Assert.True(_fakeUsers.Count == 4);
            Assert.Equal(newUser.Username, addedUserDto.Username);
        }

        [Fact]
        public void Authenticate_UserWithValidData_ReceivesAuthToken()
        {
            var validUserDTO = ValidUserDTO;

            var token = _service.Authenticate(validUserDTO);

            Assert.IsType<string>(token);
            Assert.True(token.Count() > 0);
        }

        [Fact]
        public void Authenticate_InvalidUsername_ThrowsException()
        {
            var invalidUserDTO = ValidUserDTO;
            invalidUserDTO.Username = "kdncdjksnc";

            var authentication =
                () => _service.Authenticate(invalidUserDTO);

            Assert.ThrowsAny<Exception>(authentication);
        }

        [Fact]
        public void Authenticate_InvalidPassword_ThrowsException()
        {
            var invalidUserDTO = ValidUserDTO;
            invalidUserDTO.Password = "kdncdjksnc";

            var authentication =
                () => _service.Authenticate(invalidUserDTO);

            Assert.ThrowsAny<Exception>(authentication);
        }

        [Fact]
        public void Delete_ValidId_RemovesFromCollection()
        {
            var validUser = ValidUser;

            var result = _service.Delete(validUser.Id);

            Assert.True(result);
            Assert.DoesNotContain(validUser, _fakeUsers);
        }

        #region No Mocked Objects unit tests

        [Fact]
        public void GetUserByUserName_ValidUsernameGiven_ReturnsUserWithGivenUsername()
        {
            // Arrange
            var validUsername = "marceloblvictor";

            // Act
            var user = _service.GetUserByUsername(ValidUsersDTOs, validUsername);

            // Assert
            Assert.True(user.Username == validUsername);
        }

        [Fact]
        public void GetUserByUserName_InvalidUsernameGiven_ReturnsNull()
        {
            // Arrange
            var invalidUsername = "marceloblvictorttttt";

            // Act
            var user = _service.GetUserByUsername(ValidUsersDTOs, invalidUsername);

            // Assert
            Assert.True(user is null);
        }

        [Fact]
        public void IsUserValid_GivenValidUser_ReturnsTrue()
        {
            // Act
            var result = _service.IsUserValid(ValidUsersDTOs, ValidUserDTO);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserValid_GivenInvalidUsername_ReturnsFalse()
        {
            // Arrange
            var invalidUser = ValidUserDTO;
            invalidUser.Username = "123456";

            // Act
            var result = _service.IsUserValid(ValidUsersDTOs, ValidUserDTO);

            // Assert
            Assert.True(result is false);
        }

        [Fact]
        public void IsUserValid_GivenInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var invalidUser = ValidUserDTO;
            invalidUser.Password = "123456";

            // Act
            var result = _service.IsUserValid(ValidUsersDTOs, ValidUserDTO);

            // Assert
            Assert.True(result is false);
        }

        [Fact]
        public void ArePasswordsEqual_GivenEqualPasswords_ReturnsTrue()
        {
            // Arrange
            var validPassword = "abcdef1234";

            // Act
            var result = _service.ArePasswordsEqual(validPassword, ValidUserDTO.Password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ArePasswordsEqual_GivenDifferentePasswords_ReturnsFalse()
        {
            // Arrange
            var invalidPassword = "abcdefdfsd4";

            // Act
            var result = _service.ArePasswordsEqual(invalidPassword, ValidUserDTO.Password);

            // Assert
            Assert.True(result is false);
        }

        [Fact]
        public void FilterUserByUsername_GivenValidUsername_ReturnsUsersWithGivenName()
        {
            // Arrange
            var validUsername = "marceloblvictor";

            // Act
            var filteredUsers = _service.FilterUserByUsername(ValidUsersDTOs, validUsername);

            // Assert
            Assert.True(filteredUsers.All(u => u.Username == validUsername));
        }

        [Fact]
        public void FilterUserByUsername_GivenInvalidUsername_ReturnsEmptyList()
        {
            // Arrange
            var invalidUsername = "marcelosdfsdictor";

            // Act
            var filteredUsers = _service.FilterUserByUsername(ValidUsersDTOs, invalidUsername);

            // Assert
            Assert.True(filteredUsers.Count == 0);
        }

        [Fact]
        public void GenerateToken_GivenValidUserClaimsData_ReturnsTokenString()
        {
            // Act
            var token = _service.GenerateToken(ValidUserDTO);

            // Assert
            Assert.True(!string.IsNullOrEmpty(token));
        }

        [Fact]
        public void GenerateToken_GivenInvalidEmail_ThrowsException()
        {
            // Arrange
            var invalidUser = ValidUserDTO;
            ValidUserDTO.Email = "";

            // Act
            var tokenGenerationFunc = () => _service.GenerateToken(invalidUser);

            // Assert
            Assert.ThrowsAny<Exception>(tokenGenerationFunc);
        }

        [Fact]
        public void GenerateToken_GivenEmptyUsername_ThrowsException()
        {
            // Arrange
            var invalidUser = ValidUserDTO;
            ValidUserDTO.Username = "";

            // Act
            var tokenGenerationFunc = () => _service.GenerateToken(invalidUser);

            // Assert
            Assert.ThrowsAny<Exception>(tokenGenerationFunc);
        }

        [Fact]
        public void CreateDTO_GenerateDTOWithEntityData_Succes()
        {
            // Arrange
            var validUser = ValidUser;

            // Act
            var dto = _service.CreateDTO(validUser);

            // Assert
            Assert.True(dto.Id == validUser.Id);
            Assert.True(dto.Username == validUser.Username);
            Assert.True(dto.Password == validUser.Password);
            Assert.True(dto.Email == validUser.Email);
        }

        [Fact]
        public void CreateDTOS_GenerateDTOSWithEntityData_Succes()
        {
            // Arrange
            var validUsers = ValidUsers;

            // Act
            var dtos = _service.CreateDTOs(validUsers);

            // Assert

            foreach (var dto in dtos)
            {
                var validUser = 
                    validUsers.First(u => u.Id == dto.Id);

                Assert.True(dto.Id == validUser.Id);
                Assert.True(dto.Username == validUser.Username);
                Assert.True(dto.Password == validUser.Password);
                Assert.True(dto.Email == validUser.Email);
            }
        }

        [Fact]
        public void CreateEntity_GenerateEntityWithDTOData_Succes()
        {
            // Arrange
            var validUserDto = ValidUserDTO;

            // Act
            var entity = _service.CreateEntity(validUserDto);

            // Assert
            Assert.True(entity.Id == validUserDto.Id);
            Assert.True(entity.Username == validUserDto.Username);
            Assert.True(entity.Password == validUserDto.Password);
            Assert.True(entity.Email == validUserDto.Email);
        }

        #endregion
    }
}