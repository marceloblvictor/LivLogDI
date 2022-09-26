using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LivlogDITests.RepositoriesTests
{
    public class UserRepositoryTest
    {
        Mock<LivlogDIContext> _mockedDbContext { get; set; }
        List<User> _mockedUsers { get; set; }

        UserRepository Repository { get; set; }

        public UserRepositoryTest()
        {
            _mockedUsers = new List<User>
            {
                new User()
                {
                    Id = 1,
                    Username = "user1",
                    Password = "12345678",
                    Email = "user1@email.com"
                },
                new User()
                {
                    Id = 2,
                    Username = "user2",
                    Password = "12345678",
                    Email = "user2@email.com"
                },
                new User()
                {
                    Id = 3,
                    Username = "user3",
                    Password = "12345678",
                    Email = "user3@email.com"
                },
            };

            // Mocking the DbSet
            var query = _mockedUsers.AsQueryable();

            var mockUsers = new Mock<DbSet<User>>();

            mockUsers
                .As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(query.Provider);
            mockUsers
                .As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(query.Expression);
            mockUsers
                .As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(query.ElementType);
            mockUsers
                .As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(query.GetEnumerator());

            mockUsers
                .Setup(m => m.Add(It.IsAny<User>()))
                .Callback<User>((u) => _mockedUsers.Add(u));

            mockUsers
                .Setup(m => m.Remove(It.IsAny<User>()))
                .Callback<User>((u) => { var result = _mockedUsers.Remove(u); }) ;

            mockUsers
               .Setup(m => m.Update(It.IsAny<User>()))
               .Callback<User>((u) => 
               {
                   var userToBeUpdated = _mockedUsers
                    .AsQueryable()
                    .Where(user => user.Id == u.Id)
                    .Single();

                   userToBeUpdated.Username = u.Username;
                   userToBeUpdated.Password = u.Password;
                   userToBeUpdated.Email = u.Email;
               });            

            // Mocking the DbContext
            _mockedDbContext = new Mock<LivlogDIContext>();
            _mockedDbContext
                .Setup(ctx => ctx.Users)
                .Returns(mockUsers.Object);

            Repository = new UserRepository(_mockedDbContext.Object);
        }

        [Fact]
        public void GetAll_UsersAreOrderedDescendingById()
        {            
            // Act
            var users = Repository.GetAll();

            // Assert
            Assert.Equal(3, users.Count());
            Assert.Equal(3, users[0].Id);
            Assert.Equal(2, users[1].Id);
            Assert.Equal(1, users[2].Id);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Get_GivenValidId_IsSuccess(int validID)
        {
            // Act
            var user = Repository.Get(validID);

            // Assert            
            Assert.NotNull(user);
            Assert.Equal(validID, user.Id);
        }

        [Theory]
        [InlineData(99)]
        public void Get_GivenInvalidId_IsFailure(int invalidId)
        {
            // Act
            var getUserOperation =
                new Func<User>(() => Repository.Get(invalidId));

            // Assert
            Assert.Throws<ArgumentException>(getUserOperation);
        }

        [Fact]
        public void Create_GivenNewUserData_PersistsNewUser()
        {
            var newUser = new User
            {
                Id = 4,
                Username = "Deteuronomios",
                Password = "0-9489-9819-9",
                Email = "deuteoronomios@gmail.com"
            };            

            Repository.Add(newUser);

            Assert.True(_mockedDbContext.Object.Users.Count() == 4);
            Assert.True(_mockedDbContext.Object.Users
                            .Where(b => b.Id == newUser.Id)
                            .FirstOrDefault()?
                                .Username == newUser.Username);
            Assert.True(_mockedDbContext.Object.Users.Any(b => b.Id == newUser.Id));
        }

        [Fact]
        public void Update_GivenNewUserData_PersistsNewUser()
        {
            var updatedData = new User
            {
                Id = 1,
                Username = "DeteuronomiosMODIFICADO",
                Password = "0-9489-9819-9MODIFICADO",
                Email = "deuteoronomiosMODIFICADO@gmail.com"
            };

            Repository.Update(updatedData);

            var updatedUser = Repository
                .GetAll()
                .FirstOrDefault(u => u.Id == updatedData.Id);

            Assert.True(updatedUser is not null);
            Assert.True(updatedUser.Username == updatedData.Username);
            Assert.True(updatedUser.Password == updatedData.Password);
            Assert.True(updatedUser.Email == updatedData.Email);
        }

        [Fact]
        public void Delete_GivenValidUserId_DeleteUser()
        {
            var userToBeDeleted = _mockedUsers[0];
            
            Repository.Delete(userToBeDeleted.Id);

            Assert.DoesNotContain(userToBeDeleted, _mockedUsers);            
        }

        [Fact]
        public void Delete_GivenInvalidUserId_ThrowsException()
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