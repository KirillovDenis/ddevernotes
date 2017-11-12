using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DDEvernote.DataLayer.Sql;
using System.Linq;
using DDEvernote.Model;
using System.Collections.Generic;

namespace DDEvernote.DataLayer.Sql.Tests
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
            var userRepository = new UsersRepository(ConnectionString);
        
            user = userRepository.Create(user);
            _tempUsers.Add(user.Id);
            var createdUser = userRepository.Get(user.Id);

            //asserts
            Assert.AreEqual(user.Name, createdUser.Name);
            Assert.AreEqual(user.Password, createdUser.Password);
        }


        [TestMethod]
        public void ShouldGetUserByName()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Password = "123"
            };

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var userRepository = new UsersRepository(ConnectionString);

            user = userRepository.Create(user);
            _tempUsers.Add(user.Id);
            var createdUser = userRepository.Get(user.Name);

            //asserts
            Assert.AreEqual(user.Id, createdUser.Id);
        }

        [TestMethod]
        public void ShouldGetUsersByShraredNoteId()
        {
            //arrange
            var user1 = new User
            {
                Name = "testName1",
                Password = "123"
            };
            var user2 = new User
            {
                Name = "testName2",
                Password = "123",
                Categories = new List<Category>()
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user1,
                Text = "some test text"
            };

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString);
            var notesRepository = new NotesRepository(ConnectionString);

            user1 = usersRepository.Create(user1);
            user2 = usersRepository.Create(user2);
            _tempUsers.Add(user1.Id);
            _tempUsers.Add(user2.Id);
            note = notesRepository.Create(note);
            notesRepository.Share(note.Id, user2.Id);

            var receivedUser = usersRepository.GetUsersBySharedNote(note.Id).Single();

            //asserts
            Assert.AreEqual(user2.Id, receivedUser.Id);
        }

        [TestMethod]
        public void ShouldDeleteUser()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Password = "password"
            };

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            var createdUser = usersRepository.Get(user.Id);
            usersRepository.Delete(createdUser.Id);

            //asserts
            Assert.IsFalse(usersRepository.IsExist(createdUser.Id));
        }

        [TestMethod]
        public void ShouldUpdateUser()
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
            var userRepo = new UsersRepository(ConnectionString);

            userRepo.Create(user);
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
            {
                new UsersRepository(ConnectionString).Delete(id);
            }
            _tempUsers.Clear();
        }
    }
}
