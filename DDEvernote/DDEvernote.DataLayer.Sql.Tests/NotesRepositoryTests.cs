using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DDEvernote.Model;

namespace DDEvernote.DataLayer.Sql.Tests
{
    [TestClass]
    public class NotesRepositoryTests
    {
        private const string ConnectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShoudCreateNote()
        {
            //arrange
            var user = new User
            {
                Name = "test user",
                Password = "test password",
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user
            };

            //act
            var categoriesRepo = new CategoriesRepository(ConnectionString);
            var usersRepo = new UsersRepository(ConnectionString);
            var notesRepo = new NotesRepository(ConnectionString);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            notesRepo.Create(note);
            var usersNote = notesRepo.GetUserNotes(user.Id);

            //assert
            Assert.AreEqual(usersNote.Single().Title, note.Title);
        }

        [TestMethod]
        public void ShouldGetAllNotesOfUser()
        {
            //arrange
            var user = new User
            {
                Name = "test user",
                Password = "test password",
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user
            };
            var note2 = new Note
            {
                Title = "test note2",
                Owner = user
            };
            var category = new Category
            {
                Title = "test title category",
            };

            //act
            var categoriesRepo = new CategoriesRepository(ConnectionString);
            var usersRepo = new UsersRepository(ConnectionString);
            var notesRepo = new NotesRepository(ConnectionString);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            note = notesRepo.Create(note);
            note2 = notesRepo.Create(note2);
            category = categoriesRepo.Create(user.Id, category.Title);
            notesRepo.AddNoteInCategory(category.Id, note.Id);
            IEnumerable<Note> createdNotes = notesRepo.GetUserNotes(user.Id);

            //assert
            Assert.AreEqual(createdNotes.Where(n => n.Id == note.Id).Single().Categories.Single().Id, category.Id);
            Assert.AreEqual(createdNotes.Where(n => n.Id == note2.Id).Single().Title, note2.Title);
        }

        [TestMethod]
        public void ShoudGetNoteByCategory()
        {
            //arrange
            var user = new User
            {
                Name = "test user",
                Password = "test password",
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user
            };
            const string categoryName = "test title category";

            //act
            var categoriesRepo = new CategoriesRepository(ConnectionString);
            var usersRepo = new UsersRepository(ConnectionString);
            var notesRepo = new NotesRepository(ConnectionString);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            var createdCategory = categoriesRepo.Create(user.Id, categoryName);
            var createdNote = notesRepo.Create(note);
            notesRepo.AddNoteInCategory(createdCategory.Id, createdNote.Id);

            var noteOfUser = notesRepo.GetNotesByCategory(createdCategory.Id);

            //assert
            Assert.AreEqual(noteOfUser.Single().Id, createdNote.Id);
        }

        [TestMethod]
        public void ShoudUpdateNote()
        {
            //arrange
            var user = new User
            {
                Name = "test user",
                Password = "test password",
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user
            };
            const string updatedText = "some test updated text for note";
            //act
            var categoriesRepo = new CategoriesRepository(ConnectionString);
            var usersRepo = new UsersRepository(ConnectionString);
            var notesRepo = new NotesRepository(ConnectionString);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            var createdNote = notesRepo.Create(note);
            createdNote.Text = updatedText;
            notesRepo.Update(createdNote);
            var updatedNoteFromDB = notesRepo.Get(createdNote.Id);

            //assert
            Assert.AreEqual(updatedText, updatedNoteFromDB.Text);
        }

        [TestMethod]
        public void ShoudAddCategoryToNote()
        {
            //arrange
            var user = new User
            {
                Name = "test user",
                Password = "test password",
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user
            };
            var category = new Category
            {
                Title = "test title category",
            };

            //act
            var categoriesRepo = new CategoriesRepository(ConnectionString);
            var usersRepo = new UsersRepository(ConnectionString);
            var notesRepo = new NotesRepository(ConnectionString);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            note = notesRepo.Create(note);
            category = categoriesRepo.Create(user.Id, category.Title);
            notesRepo.AddNoteInCategory(category.Id, note.Id);
            var createdNotes = notesRepo.Get(note.Id);

            //assert
            Assert.AreEqual(createdNotes.Categories.Single().Id, category.Id);
        }

        [TestMethod]
        public void ShouldShareNote()
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

            var sharedNote = notesRepository.Get(note.Id);
            var su = sharedNote.Shared.Single();

            //asserts
            Assert.AreEqual(user2.Id, su.Id);
        }

        [TestMethod]
        public void ShouldDenySharedNote()
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
            notesRepository.DenyShared(note.Id, user2.Id);
            var sharedNote = notesRepository.Get(note.Id);

            //asserts
            Assert.AreEqual(0, sharedNote.Shared.Count());
        }

        [TestMethod]
        public void ShoudDeleteNote()
        {
            //arrange
            var user = new User
            {
                Name = "test user",
                Password = "test password",
            };
            var note = new Note
            {
                Title = "test note",
                Owner = user
            };

            //act
            var categoriesRepo = new CategoriesRepository(ConnectionString);
            var usersRepo = new UsersRepository(ConnectionString);
            var notesRepo = new NotesRepository(ConnectionString);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            var createdNote = notesRepo.Create(note);
            notesRepo.Delete(createdNote.Id);

            //assert
            Assert.IsFalse(notesRepo.IsExist(createdNote.Id));
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
