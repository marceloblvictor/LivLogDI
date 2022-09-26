using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Enums;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using static LivlogDI.Enums.CustomerCategory;

namespace LivlogDITests.RepositoriesTests
{
    public class FineRepositoryTest
    {
        Fine ValidFine { get; set; } = new()
        {
            Id = 4,
            Amount = 15m,
            Status = (FineStatus)1,
            CustomerId = 1
        };

        Mock<LivlogDIContext> _mockedDbContext { get; set; }
        List<Fine> _mockedFines { get; set; } = new List<Fine>
        {
            new ()
            {
                Id = 1,
                Amount = 15m,
                Status = (FineStatus) 1,
                CustomerId = 1
            },
            new ()
            {
                Id = 2,
                Amount = 13m,
                Status = (FineStatus) 1,
                CustomerId = 1
            },
            new ()
            {
                Id = 3,
                Amount = 12m,
                Status = (FineStatus) 2,
                CustomerId = 1
            },
        };

        FineRepository Repository { get; set; }

        public FineRepositoryTest()
        {            
            var query = _mockedFines.AsQueryable();

            var mockFines = new Mock<DbSet<Fine>>();

            mockFines
                .As<IQueryable<Fine>>()
                .Setup(m => m.Provider)
                .Returns(query.Provider);
            mockFines
                .As<IQueryable<Fine>>()
                .Setup(m => m.Expression)
                .Returns(query.Expression);
            mockFines
                .As<IQueryable<Fine>>()
                .Setup(m => m.ElementType)
                .Returns(query.ElementType);
            mockFines
                .As<IQueryable<Fine>>()
                .Setup(m => m.GetEnumerator())
                .Returns(query.GetEnumerator());
            
            mockFines
                .Setup(m => m.Add(It.IsAny<Fine>()))
                .Callback<Fine>((f) => _mockedFines.Add(f));
            mockFines
                .Setup(m => m.Remove(It.IsAny<Fine>()))
                .Callback<Fine>((f) => { _mockedFines.Remove(f); });
            mockFines
               .Setup(m => m.Update(It.IsAny<Fine>()))
               .Callback<Fine>((f) =>
               {
                   var fineToBeUpdated = _mockedFines
                    .AsQueryable()
                    .Where(fine => fine.Id == f.Id)
                    .Single();

                   fineToBeUpdated.Amount = f.Amount;
                   fineToBeUpdated.Status = f.Status;
               });

            _mockedDbContext = new Mock<LivlogDIContext>();
            _mockedDbContext
                .Setup(ctx => ctx.Fines)
                .Returns(mockFines.Object);

            Repository = new FineRepository(_mockedDbContext.Object);
        }

        [Fact]
        public void GetAll_FinesAreOrderedDescendingById()
        {            
            // Act
            var fines = Repository.GetAll();

            // Assert
            Assert.Equal(3, fines.Count());
            Assert.Equal(3, fines[0].Id);
            Assert.Equal(2, fines[1].Id);
            Assert.Equal(1, fines[2].Id);
        }

        [Theory]
        [InlineData(1)]        
        public void GetAFineWithAValidId_IsSuccess(int validID)
        {
            // Act
            var fine = Repository.Get(validID);

            // Assert            
            Assert.NotNull(fine);
            Assert.Equal(validID, fine.Id);
        }

        [Theory]
        [InlineData(99)]       
        public void GetAFineWithAnInvalidId_IsFailure(int invalidId)
        {
            // Act
            var getOperation = 
                new Func<Fine>(() => Repository.Get(invalidId));

            // Assert
            Assert.Throws<ArgumentException>(getOperation);
        }

        [Fact]
        public void CreateAFine_PersistsNewFine()
        {
            var newFine = ValidFine;

            Repository.Add(newFine);

            Assert.True(_mockedDbContext.Object.Fines.Count() == 4);
            Assert.True(_mockedDbContext.Object.Fines
                            .Where(c => c.Id == newFine.Id)
                            .FirstOrDefault()?
                                .Id == newFine.Id);
        }

        [Fact]
        public void Update_GivenNewFineData_PersistsNewFine()
        {
            var updatedData = _mockedFines[0];

            updatedData.Amount = 342;
            updatedData.Status = FineStatus.Paid;

            Repository.Update(updatedData);

            var updatedFine = Repository
                .GetAll()
                .FirstOrDefault(u => u.Id == updatedData.Id);

            Assert.True(updatedFine is not null);
            Assert.True(updatedFine.Amount == updatedData.Amount);
            Assert.True(updatedFine.Status == updatedData.Status);
        }

        [Fact]
        public void Delete_GivenValidFineId_DeleteFine()
        {
            var fineToBeDeleted = _mockedFines[0];

            Repository.Delete(fineToBeDeleted.Id);

            Assert.DoesNotContain(fineToBeDeleted, _mockedFines);
        }

        [Fact]
        public void Delete_GivenInvalidFineId_ThrowsException()
        {
            var invalidId = 99;

            var operation = () =>
            {
                Repository.Delete(invalidId);
            };

            Assert.Throws<ArgumentException>(operation);
        }
    }
}