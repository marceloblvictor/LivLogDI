using LivlogDI.Models.DTO;
using LivlogDI.Validators;

namespace LivlogDITests.ValidatorsTests
{
    public class AuthValidatorTest
    {
        AuthValidator _validator { get; set; }

        UserDTO ValidNewUserDTO { get; set; } = new()
        {
            Id = 0,
            Username = "marceloblvictorNOVO",
            Password = "abcdef1234",
            Email = "marceloblvictorNOVO@gmail.com"
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

        public AuthValidatorTest()
        {
            _validator = new AuthValidator();
        }

        [Fact]
        public void ValidateNewUser_ValidUserData_DontThrowException()
        {
            // Arrange
            var validUserDTO = ValidNewUserDTO;

            // Act
            var validation = () => _validator.ValidateNewUser(ValidUsersDTOs, validUserDTO);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateNewUser_InvalidUsername_ThrowsException()
        {
            // Arrange
            var invalidUserDTO = ValidNewUserDTO;
            invalidUserDTO.Username = "marceloblvictor";

            // Act
            var validation = () => _validator.ValidateNewUser(ValidUsersDTOs, invalidUserDTO);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateNewUser_InvalidPassword_ThrowsException()
        {
            // Arrange
            var invalidUserDTO = ValidNewUserDTO;
            invalidUserDTO.Password = "123";

            // Act
            var validation = () => _validator.ValidateNewUser(ValidUsersDTOs, invalidUserDTO);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateNewUser_InvalidEmail_ThrowsException()
        {
            // Arrange
            var invalidUserDTO = ValidNewUserDTO;
            invalidUserDTO.Email = "marceloblvictor@gmail.com";

            // Act
            var validation = () => _validator.ValidateNewUser(ValidUsersDTOs, invalidUserDTO);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }
    }
}