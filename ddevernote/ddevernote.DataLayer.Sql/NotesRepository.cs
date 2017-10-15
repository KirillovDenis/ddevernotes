using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ddevernote.DataLayer;
using ddevernote.Model;
using System.Data.SqlClient;

namespace ddevernote.DataLayer.Sql
{
    public class NotesRepository : INotesRepository
    {

        private readonly string _connectionString;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IUsersRepository _usersRepository;

        public NotesRepository(String connectionString, ICategoriesRepository categoriesRepository, IUsersRepository usersRepository)
        {
            _connectionString = connectionString;
            _categoriesRepository = categoriesRepository;
            _usersRepository = usersRepository;
        }

        public Note AddNote(Guid categoryId, Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                Note note;
                try
                {
                    Category category = _categoriesRepository.Get(categoryId);
                    note = Get(noteId);
                    foreach (var cat in note.Categories)
                    {
                        if (cat.Id == categoryId)
                        {
                            return note;
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    throw ex;
                }
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "insert into category_notes values (@categoryId, @noteId)";
                    command.Parameters.AddWithValue("@categoryid", categoryId);
                    command.Parameters.AddWithValue("@noteId", noteId);
                    command.ExecuteNonQuery();
                    return note;
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
                    command.CommandText = "insert into notes (id, user_id, title, text, created_time, changed_time) values  (@id, @user_id, @title, @text, @created_time, @changed_time);";
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
                    command.CommandText = "delete from category_notes where note_id=@id; delete from shared where note_id=@id; delete from notes where id=@id";
                    command.Parameters.AddWithValue("@id",noteId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public Note Get(Guid noteId)
        {
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
                        var note = new Note
                        {
                            Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Owner = _usersRepository.Get(new Guid(reader.GetString(reader.GetOrdinal("user_id")))),
                            Text = reader.GetString(reader.GetOrdinal("text")),
                            Shared = _usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                            Categories = _categoriesRepository.GetUserCategories(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                            Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                        };
                        return note;
                    }
                }
            }
        }
        public IEnumerable<Note> GetUserNotes(Guid userId)
        {
            User user;
            try
            {
                user = _usersRepository.Get(userId);
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from notes where user_id=@userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {


                            yield return new Note
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Owner = user,
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Shared = _usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                                Categories = _categoriesRepository.GetUserCategories(user.Id),
                                Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                            };
                        }
                    }
                }
            }
        }
        public IEnumerable<Note> GetUserNotesByCategory(Guid userId, string categoryName)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select notes.id, notes.user_id, notes.title, notes.text, notes.created_time, notes.changed_time from notes inner join category_notes on category_notes.note_id=notes.id inner join category on category_notes.category_id=category.id where category.user_id=@userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Note
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Owner = _usersRepository.Get(userId),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Shared = _usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                                Categories = _categoriesRepository.GetUserCategories(userId),
                                Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                            };
                        }
                    }
                }
            }
        }
        public Note Update(Note note)
        {
            //TODO maybe redo this method for updating shared and category_note
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select title from notes where id=@id";
                    command.Parameters.AddWithValue("@id", note.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Заметка с id {note.Id} не найденa");
                        }
                    }
                }


                using (var command = sqlConnection.CreateCommand())
                {note.Changed = DateTime.Now;
                    if (note.Text == null)
                    {
                        note.Text = "";
                    }
                    command.CommandText = "update notes set title = @title, text = @text, changed_time = @changed_time where id=@id; ";
                    command.Parameters.AddWithValue("@id", note.Id);
                    command.Parameters.AddWithValue("@title", note.Title);
                    command.Parameters.AddWithValue("@text", note.Text);
                    command.Parameters.AddWithValue("@changed_time", note.Changed);
                    command.ExecuteNonQuery();
                    return note;
                }
            }
        }
    }
}
