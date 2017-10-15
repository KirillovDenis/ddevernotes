using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ddevernote.Model;

namespace ddevernote.DataLayer.Sql.Tests
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
            var usersRepo = new UsersRepository(ConnectionString, categoriesRepo);
            var notesRepo = new NotesRepository(ConnectionString, categoriesRepo, usersRepo);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            notesRepo.Create(note);

            var usersNote = notesRepo.GetUserNotes(user.Id);

            //assert
            Assert.AreEqual(usersNote.Single().Title, note.Title);
        }

        [TestMethod]
        public void ShoudGetNoteByCategoryName()
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
            var usersRepo = new UsersRepository(ConnectionString, categoriesRepo);
            var notesRepo = new NotesRepository(ConnectionString, categoriesRepo, usersRepo);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);

            var createdCategory = categoriesRepo.Create(user.Id, categoryName);
            var createdNote = notesRepo.Create(note);
            notesRepo.AddNote(createdCategory.Id, createdNote.Id);

            var usersNote = notesRepo.GetUserNotesByCategory(user.Id, categoryName);

            //assert
            Assert.AreEqual(usersNote.Single().Id, createdNote.Id);
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
            var usersRepo = new UsersRepository(ConnectionString, categoriesRepo);
            var notesRepo = new NotesRepository(ConnectionString, categoriesRepo, usersRepo);

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
        [ExpectedException(typeof(ArgumentException))]
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
            var usersRepo = new UsersRepository(ConnectionString, categoriesRepo);
            var notesRepo = new NotesRepository(ConnectionString, categoriesRepo, usersRepo);

            user = usersRepo.Create(user);
            _tempUsers.Add(user.Id);
            var createdNote = notesRepo.Create(note);
            notesRepo.Delete(createdNote.Id);

            //assert
            notesRepo.Get(createdNote.Id);
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
