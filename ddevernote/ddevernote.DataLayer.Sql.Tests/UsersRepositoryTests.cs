using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ddevernote.DataLayer.Sql;
using System.Linq;
using ddevernote.Model;
using System.Collections.Generic;

namespace ddevernote.DataLayer.Sql.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private const string ConnectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateUser()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Password = "123"
            };

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var userRepository = new UsersRepository(ConnectionString, categoriesRepository);
            var result = userRepository.Create(user);
        
            var userFromDb = userRepository.Get(result.Id);
            _tempUsers.Add(userFromDb.Id);


            //asserts
            Assert.AreEqual(user.Name, userFromDb.Name);
            Assert.AreEqual(user.Password, userFromDb.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldCreateAndDeleteUser()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Password = "password"
            };
            const string category = "testCategory";

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString, categoriesRepository);
            user = usersRepository.Create(user);

            _tempUsers.Add(user.Id);
            var userDB = usersRepository.Get(user.Id);
            usersRepository.Delete(userDB.Id);


            //asserts
           var deltedUser = usersRepository.Get(userDB.Id);
        }

        [TestMethod]
        public void ShouldCreateAndUpdateUser()
        {
            //arrange
            var user = new User
            {
                Name = "testName",
                Password = "testPassword"
            };
            const string updatedName = "updated test Name";
            const string updatedPassword = "updated test Password";

            //act
            var categoryRepo = new CategoriesRepository(ConnectionString);
            var userRepo = new UsersRepository(ConnectionString, categoryRepo);

            user = userRepo.Create(user);
            _tempUsers.Add(user.Id);

            user.Name = updatedName;
            user.Password = updatedPassword;
            userRepo.Update(user);
            var updatedUser = userRepo.Get(user.Id);

            //assert
            Assert.AreEqual(updatedName, updatedUser.Name);
            Assert.AreEqual(updatedPassword, updatedUser.Password);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var id in _tempUsers)
                new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)).Delete(id);
            _tempUsers.Clear();
        }
    }
}
