using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDEvernote.DataLayer;
using DDEvernote.Model;
using System.Data.SqlClient;

namespace DDEvernote.DataLayer.Sql
{
    public class NotesRepository : INotesRepository
    {

        private readonly string _connectionString;

        public NotesRepository(String connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddNoteInCategory(Guid categoryId, Guid noteId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                if (_categoriesRepository.IsExist(categoryId) && IsExist(noteId))
                {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = "insert into category_notes values (@categoryId, @noteId)";
                        command.Parameters.AddWithValue("@categoryid", categoryId);
                        command.Parameters.AddWithValue("@noteId", noteId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public Note Create(Note note)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    note.Id = Guid.NewGuid();
                    note.Created = note.Changed = DateTime.Now;
                    if (note.Text == null)
                    {
                        note.Text = "";
                    }
                    command.CommandText = "insert into notes " +
                        "(id, user_id, title, text, created_time, changed_time) " +
                        "values  (@id, @user_id, @title, @text, @created_time, @changed_time);";
                    command.Parameters.AddWithValue("@id", note.Id);
                    command.Parameters.AddWithValue("@user_id", note.Owner.Id);
                    command.Parameters.AddWithValue("@title", note.Title);
                    command.Parameters.AddWithValue("@text", note.Text);
                    command.Parameters.AddWithValue("@created_time", note.Created);
                    command.Parameters.AddWithValue("@changed_time", note.Changed);
                    command.ExecuteNonQuery();
                    return note;
                }
            }
        }

        public void Delete(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from notes where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Note Get(Guid noteId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository _usersRepository = new UsersRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from notes where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Note с id {noteId} не найден");
                        }
                        return new Note
                        {
                            Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Owner = _usersRepository.Get(new Guid(reader.GetString(reader.GetOrdinal("user_id")))),
                            Text = reader.GetString(reader.GetOrdinal("text")),
                            Shared = _usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                            Categories = _categoriesRepository.GetCategoriesOfNote(noteId),
                            Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                        };
                    }
                }
            }
        }
        
        public IEnumerable<Note> GetUserNotes(Guid userId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository _usersRepository = new UsersRepository(_connectionString);
            if (_usersRepository.IsExist(userId))
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = "select * from notes where user_id=@userId";
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            var listOfNotes = new List<Note>();
                            var owner = _usersRepository.Get(userId);
                            while (reader.Read())
                            {
                                listOfNotes.Add(new Note
                                {
                                    Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                    Title = reader.GetString(reader.GetOrdinal("title")),
                                    Owner = owner,
                                    Text = reader.GetString(reader.GetOrdinal("text")),
                                    Shared = _usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                                    Categories = _categoriesRepository.GetUserCategories(userId),
                                    Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                                    Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                                });
                            }
                            return listOfNotes;
                        }
                    }
                }
            }
            throw new ArgumentException($"Пользователя с id {userId} не существует");
        }

        public IEnumerable<Note> GetNotesByCategory(Guid categoryId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository _usersRepository = new UsersRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select notes.id, notes.user_id, notes.title, notes.user_id, notes.text, notes.created_time, notes.changed_time " +
                        "from notes inner join category_notes " +
                        "on category_notes.note_id=notes.id " +
                        "where category_notes.category_id=@categoryId;";
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfNotes = new List<Note>();
                        while (reader.Read())
                        {
                            listOfNotes.Add(new Note
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Owner = _usersRepository.Get(new Guid(reader.GetString(reader.GetOrdinal("user_id")))),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Shared = _usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                                Categories = _categoriesRepository.GetUserCategories(new Guid(reader.GetString(reader.GetOrdinal("user_id")))),
                                Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                            });
                        }
                        return listOfNotes;
                    }
                }
            }
        }

        public bool IsExist(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select title from notes where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public void Share(Guid noteId, Guid userId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository _usersRepository = new UsersRepository(_connectionString);
            if (IsExist(noteId) && _usersRepository.IsExist(userId))
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = "insert into shared " +
                            "values (@noteId, @userId)";
                        command.Parameters.AddWithValue("@noteId", noteId);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                        return;
                    }
                }
            }
            throw new ArgumentException($"Заметка с id {noteId} не существует или пользователь с id {userId} не существует");
        }

        public void DenyShared(Guid noteId, Guid userId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository _usersRepository = new UsersRepository(_connectionString);
            if (IsExist(noteId) && _usersRepository.IsExist(userId))
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = "delete from shared " +
                            "where note_id = @noteId and user_id = @userId;";
                        command.Parameters.AddWithValue("@noteId", noteId);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                        return;
                    }
                }
            }
        }

        public Note Update(Note note)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                if (IsExist(note.Id))
                {
                    using (var command = sqlConnection.CreateCommand())
                    {
                        note.Changed = DateTime.Now;
                        command.CommandText = "update notes " +
                            "set title = @title, text = @text, changed_time = @changed_time where id=@id; ";
                        command.Parameters.AddWithValue("@id", note.Id);
                        command.Parameters.AddWithValue("@title", note.Title);
                        command.Parameters.AddWithValue("@text", note.Text??"");
                        command.Parameters.AddWithValue("@changed_time", note.Changed);
                        command.ExecuteNonQuery();
                        return note;
                    }
                }
                throw new ArgumentException($"Заметка с id {note.Id} не найдена.");
            }
        }
    }
}
