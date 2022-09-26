using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Validators;

namespace LivlogNoDITests.ValidatorsTests
{
    public class FineValidatorTest
    {
        FineValidator _validator { get; set; }

        FineDTO ValidFineDTO { get; set; } = new()
        {
            Id = 1,
            Amount = 15m,
            Status = (FineStatus)1,
            CustomerId = 1,
            CustomerName = "marceloblvictor"
        };

        public FineValidatorTest()
        {
            _validator = new FineValidator();
        }

        [Fact]
        public void ValidateFineToBePaid_ValidData_DontThrowException()
        {
            // Arrange
            var validFineDTO = ValidFineDTO;
            var validAmount = 20m;

            // Act
            var validation = () => _validator.ValidateFineToBePaid(validFineDTO, validAmount);
            validation();

            // Assert
            // Just make sure that no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void ValidateFineToBePaid_InsuficientPayedAmount_ThrowException()
        {
            // Arrange
            var invalidUserDTO = ValidFineDTO;
            var invalidAmount = 5m;

            // Act
            var validation = () => _validator.ValidateFineToBePaid(invalidUserDTO, invalidAmount);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }

        [Fact]
        public void ValidateFineToBePaid_InvalidFineStatus_ThrowException()
        {
            // Arrange
            var invalidUserDTO = ValidFineDTO;
            invalidUserDTO.Status = FineStatus.Paid;
            var validAmount = 20m;

            // Act
            var validation = () => _validator.ValidateFineToBePaid(invalidUserDTO, validAmount);

            // Assert
            Assert.ThrowsAny<Exception>(validation);
        }
    }
}