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
        private readonly NLog.Logger _logger;

        public NotesRepository(String connectionString)
        {
            _connectionString = connectionString;
            _logger = Logger.Log.Instance;
            _logger.Debug("Создание репозитория управления заметками с помощью: \"{0}\"", connectionString);
        }

        public Note Create(Note note)
        {
            _logger.Debug("Начато создание заметки с названием: \"{0}\" для пользователя с id: \"{1}\"", note.Title, note.Owner.Id);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            if (!usersRepository.IsExist(note.Owner.Id))
            {
                throw new UserNotFoundException($"Пользователя с id: \"{note.Owner.Id}\" не существует", note.Owner.Id);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    note.Id = Guid.NewGuid();
                    note.Created = note.Changed = DateTime.Now;
                    _logger.Debug("Запрос к базе данных на создание заметки с названием: \"{0}\" для пользователя с id: \"{0}\"", note.Title, note.Owner.Id);
                    command.CommandText = "insert into notes " +
                                          "(id, user_id, title, text, created_time, changed_time) " +
                                          "values (@id, @user_id, @title, @text, @created_time, @changed_time);";
                    command.Parameters.AddWithValue("@id", note.Id);
                    command.Parameters.AddWithValue("@user_id", note.Owner.Id);
                    command.Parameters.AddWithValue("@title", note.Title);
                    command.Parameters.AddWithValue("@text", note.Text ?? "");
                    command.Parameters.AddWithValue("@created_time", note.Created);
                    command.Parameters.AddWithValue("@changed_time", note.Changed);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Создана заметка с id: \"{0}\", название: \"{1}\" для пользователя с id: \"{2}\"", note.Id, note.Title, note.Owner.Id);
                return note;
            }
        }

        public Note Get(Guid noteId)
        {
            _logger.Debug("Начато получение заметки с id: \"{0}\"", noteId);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository _usersRepository = new UsersRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение заметки с id: \"{0}\"", noteId);
                    command.CommandText = "select * " +
                                          "from notes " +
                                          "where id=@id;";
                    command.Parameters.AddWithValue("@id", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new NoteNotFoundException($"Заметка с id {noteId} не найдена", noteId);
                        }
                        _logger.Info("Получена заметка с id: \"{0}\", названием: \"{1}\" (id пользователя: \"{2}\")", noteId, reader.GetString(reader.GetOrdinal("title")), new Guid(reader.GetString(reader.GetOrdinal("user_id"))));
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

        public Note Update(Note note)
        {
            _logger.Debug("Начато обновление заметки с id: \"{0}\"", note.Id);
            if (!IsExist(note.Id))
            {
                throw new NoteNotFoundException($"Заметки с id: \"{note.Id}\" не существует", note.Id);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    note.Changed = DateTime.Now;
                    _logger.Debug("Запрос к базе данных на обновление заметки с id: \"{0}\"", note.Id);
                    command.CommandText = "update notes " +
                                          "set title=@title, text=@text, changed_time=@changed_time " +
                                          "where id=@id;";
                    command.Parameters.AddWithValue("@id", note.Id);
                    command.Parameters.AddWithValue("@title", note.Title);
                    command.Parameters.AddWithValue("@text", note.Text ?? "");
                    command.Parameters.AddWithValue("@changed_time", note.Changed);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Обновлена заметка с id: \"{0}\", названием: \"{1}\"", note.Id, note.Title);
                return note;

            }
        }

        public void AddNoteInCategory(Guid categoryId, Guid noteId)
        {
            _logger.Debug("Начато добавление заметки с id: \"{0}\" в категорию с id: \"{1}\"", noteId, categoryId);
            ICategoriesRepository categoriesRepository = new CategoriesRepository(_connectionString);
            if (!IsExist(noteId))
            {
                throw new NoteNotFoundException($"Заметки с id: \"{noteId}\" не существует", noteId);
            }
            else if (!categoriesRepository.IsExist(categoryId))
            {
                throw new CategoryNotFoundException($"Категория с id: \"{categoryId}\" не существует", categoryId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на добавление заметки с id: \"{0}\" в категорию с id: \"{1}\"", noteId, categoryId);
                    command.CommandText = "insert into category_notes " +
                                          "values (@categoryId, @noteId);";
                    command.Parameters.AddWithValue("@categoryid", categoryId);
                    command.Parameters.AddWithValue("@noteId", noteId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Добавлена заметка с id: \"{0}\" в категорию с id: \"{1}\"", noteId, categoryId);
            }
        }

        public void Share(Guid noteId, Guid userId)
        {
            _logger.Debug("Начато предоставление общего доступа к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            if (!IsExist(noteId))
            {
                throw new NoteNotFoundException($"Заметки с id: \"{noteId}\" не существует", noteId);
            }
            else if (!usersRepository.IsExist(userId))
            {
                throw new UserNotFoundException($"Пользователя с id: \"{userId}\" не существует", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на предоставление общего доступа к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
                    command.CommandText = "insert into shared " +
                                          "values (@noteId, @userId)";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Предоставлен общий доступ к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
            }
        }

        public void DenyShared(Guid noteId, Guid userId)
        {
            _logger.Debug("Начато закрытие общего доступа к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            if (!IsExist(noteId))
            {
                throw new NoteNotFoundException($"Заметки с id: \"{noteId}\" не существует", noteId);
            }
            else if (!usersRepository.IsExist(userId))
            {
                throw new UserNotFoundException($"Пользователя с id: \"{userId}\" не существует", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на закрытие общего доступа к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
                    command.CommandText = "delete " +
                                          "from shared " +
                                          "where note_id=@noteId and user_id=@userId;";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Закрыт общий доступ к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
            }
        }

        public void Delete(Guid noteId)
        {
            _logger.Debug("Начано удаление заметки с id: \"{0}\"", noteId);
            if (!IsExist(noteId))
            {
                _logger.Warn("Заметка с id: \"{0}\" не может быть удалена", noteId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на удаление заметки с id: \"{0}\"", noteId);
                    command.CommandText = "delete " +
                                          "from notes " +
                                          "where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Заметка с id: \"{0}\" удалена", noteId);
            }
        }

        public IEnumerable<Note> GetUserNotes(Guid userId)
        {
            _logger.Debug("Начато получение заметок пользователя с id: \"{0}\"", userId);
            ICategoriesRepository categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            if (!usersRepository.IsExist(userId))
            {
                throw new UserNotFoundException($"Пользователя с id: \"{userId}\" не существует", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение заметок пользователя с id: \"{0}\"", userId);
                    command.CommandText = "select * " +
                                          "from notes " +
                                          "where user_id=@userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfNotes = new List<Note>();
                        var owner = usersRepository.Get(userId);
                        while (reader.Read())
                        {
                            listOfNotes.Add(new Note
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Owner = owner,
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Shared = usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                                Categories = categoriesRepository.GetUserCategories(userId),
                                Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                            });
                        }
                        _logger.Info("Получено \"{0}\" заметок для пользователя с id: \"{1}\"", listOfNotes.Count(), userId);
                        return listOfNotes;
                    }
                }
            }
        }

        public IEnumerable<Note> GetNotesByCategory(Guid categoryId)
        {
            _logger.Debug("Начато получение заметок из категории с id: \"{0}\"", categoryId);
            ICategoriesRepository categoriesRepository = new CategoriesRepository(_connectionString);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение заметок из категории с id: \"{0}\"", categoryId);
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
                                Owner = usersRepository.Get(new Guid(reader.GetString(reader.GetOrdinal("user_id")))),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Shared = usersRepository.GetUsersBySharedNote(new Guid(reader.GetString(reader.GetOrdinal("id")))),
                                Categories = categoriesRepository.GetUserCategories(new Guid(reader.GetString(reader.GetOrdinal("user_id")))),
                                Changed = reader.GetDateTime(reader.GetOrdinal("changed_time")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created_time"))
                            });
                        }
                        _logger.Info("Получено \"{0}\" заметок из категории с id: \"{1}\"", listOfNotes.Count(), categoryId);
                        return listOfNotes;
                    }
                }
            }
        }

        public bool IsExist(Guid noteId)
        {
            _logger.Debug("Провека существования заметки с id: \"{0}\"", noteId);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select title " +
                                          "from notes " +
                                          "where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _logger.Debug("Заметка с id: \"{0}\" найдена", noteId);
                            return true;
                        }
                        _logger.Debug("Заметка с id: \"{0}\" не найдена", noteId);
                        return false;
                    }
                }
            }
        }
    }
}
