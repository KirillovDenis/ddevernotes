using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ddevernote.Model;
using System.Collections.Generic;

namespace ddevernote.DataLayer.Sql.Tests
{
    [TestClass]
    public class CategoriesRepositoryTests
    {
        private const string ConnectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly List<Guid> _tempUsers = new List<Guid>();



        [TestMethod]
        public void ShouldAddCategory()
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
            var usersRepository = new UsersRepository(ConnectionString, categoriesRepository);

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
            var usersRepository = new UsersRepository(ConnectionString, categoriesRepository);

            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);

            Category createdCategory = categoriesRepository.Create(user.Id, categoryTitle);
            var updatedCategory = categoriesRepository.UpdateTitle(createdCategory.Id, updatedCategoryTitle);
            var usersCategory = categoriesRepository.GetUserCategories(user.Id);

            //assert 
            Assert.AreEqual(usersCategory.Single().Title, updatedCategoryTitle);
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
