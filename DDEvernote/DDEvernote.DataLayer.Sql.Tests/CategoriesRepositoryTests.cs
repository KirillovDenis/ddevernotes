using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DDEvernote.Model;
using System.Collections.Generic;

namespace DDEvernote.DataLayer.Sql.Tests
{
    [TestClass]
    public class CategoriesRepositoryTests
    {
        private const string ConnectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateCategory()
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
            var usersRepository = new UsersRepository(ConnectionString);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);

            categoriesRepository.Create(user.Id, category);
            var userDB = usersRepository.Get(user.Id);

            //asserts
            Assert.AreEqual(category, userDB.Categories.Single().Title);
        }

        [TestMethod]
        public void ShoudDeleteCategory()
        {
            //arrange
            var user = new User
            {
                Name = "test name",
                Password = "test password"
            };
            const string categoryTitle = "test category title";

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);

            Category createdCategory = categoriesRepository.Create(user.Id, categoryTitle);
            categoriesRepository.Delete(createdCategory.Id);
            var usersCategory = categoriesRepository.GetUserCategories(user.Id);

            //assert 
            Assert.IsTrue(usersCategory.Count() == 0);
        }

        [TestMethod]
        public void ShoudUpdateCategory()
        {
            //arrange
            var user = new User
            {
                Name = "test name",
                Password = "test password"
            };
            const string categoryTitle = "test category title";
            const string updatedCategoryTitle = "updated category title";

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);

            Category createdCategory = categoriesRepository.Create(user.Id, categoryTitle);
            createdCategory.Title = updatedCategoryTitle;
            var updatedCategory = categoriesRepository.Update(createdCategory);
            var usersCategory = categoriesRepository.GetUserCategories(user.Id);

            //assert 
            Assert.AreEqual(usersCategory.Single().Title, updatedCategoryTitle);
        }

        [TestMethod]
        public void ShouldGetCategoriesOfUser()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Password = "password"
            };
            var category = new Category
            {
                Title = "testCategory"
            };
            var category2 = new Category
            {
                Title = "testCategory2"
            };

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            category = categoriesRepository.Create(user.Id, category.Title);
            category2 = categoriesRepository.Create(user.Id, category2.Title);
            IEnumerable<Category> createdCategories = categoriesRepository.GetUserCategories(user.Id);

            //asserts
            Assert.AreEqual(category.Title, createdCategories.Where(c => c.Id == category.Id).Single().Title);
            Assert.AreEqual(category2.Title, createdCategories.Where(c => c.Id == category2.Id).Single().Title);
        }

        [TestMethod]
        public void ShouldGetCategoriesOfNote()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Password = "password"
            };
            var note = new Note
            {
                Title = "test title note",
                Owner = user

            };
            var category = new Category
            {
                Title = "testCategory"
            };
            var category2 = new Category
            {
                Title = "testCategory2"
            };

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString);
            var notesRepository = new NotesRepository(ConnectionString);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            category = categoriesRepository.Create(user.Id, category.Title);
            category2 = categoriesRepository.Create(user.Id, category2.Title);
            note = notesRepository.Create(note);
            notesRepository.AddNoteInCategory(note.Id, category.Id);
            notesRepository.AddNoteInCategory(note.Id, category2.Id);

            IEnumerable<Category> createdCategories = categoriesRepository.GetCategoriesOfNote(note.Id);

            //asserts
            Assert.AreEqual(category.Title, createdCategories.Where(c => c.Id == category.Id).Single().Title);
            Assert.AreEqual(category2.Title, createdCategories.Where(c => c.Id == category2.Id).Single().Title);
        }

        [TestCleanup]
        public void CleanData()
        {
            IUsersRepository tmpUsersRepository = new UsersRepository(ConnectionString);
            foreach (var id in _tempUsers)
            {
                tmpUsersRepository.Delete(id);
            }
            _tempUsers.Clear();
        }
    }
}
