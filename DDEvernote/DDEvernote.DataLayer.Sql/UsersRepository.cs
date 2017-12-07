using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using DDEvernote.Model;

namespace DDEvernote.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        private readonly NLog.Logger _logger;

        public UsersRepository(String connectionString)
        {
            _connectionString = connectionString;
            _logger = Logger.Log.Instance;
            _logger.Debug("Создание репозитория управления пользователями с помощью: \"{0}\"", connectionString);
        }

        public User Create(User user)
        {
            _logger.Debug("Начато создание пользователя с именем: \"{0}\"", user.Name);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                    _logger.Debug("Запрос к базе данных на создание пользователя с id: \"{0}\", name: \"{1}\"", user.Id, user.Name);
                    command.CommandText = "insert into users (id, name, password) " +
                        "values (@id, @name, @password);";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.ExecuteNonQuery();
                }
                user.Categories = new List<Category>();
                _logger.Info("Создан пользователь с id: \"{0}\", name: \"{1}\" (имеет категорий: \"{2}\")", user.Id, user.Name, user.Categories.Count());
                return user;
            }
        }

        public User Get(Guid userId)
        {
            _logger.Debug("Начато получение пользователя с id: \"{0}\"", userId);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение пользователся с id: \"{0}\"", userId);
                    command.CommandText = "select name, password  from users where id = @userId;";
                    command.Parameters.AddWithValue("@userId", userId);
                    var user = new User();
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new UserNotFoundException($"Пользователь с id: \"{userId}\" не существует", userId);
                        }
                        user.Id = userId;
                        user.Name = reader.GetString(reader.GetOrdinal("name"));
                        user.Password = reader.GetString(reader.GetOrdinal("password"));
                    }
                    _logger.Debug("Получение категорий пользователя с id: \"{0}\"", userId);
                    user.Categories = _categoriesRepository.GetUserCategories(userId);
                    _logger.Info("Получен пользователь с id: \"{0}\", name: \"{1}\" (имеет категорий: \"{2}\")", userId, user.Name, user.Categories.Count());
                    return user;
                }
            }
        }

        public User Update(User user)
        {
            _logger.Debug("Начато обновление пользователя с id: \"{0}\"", user.Id);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                if (!IsExist(user.Id))
                {
                    throw new UserNotFoundException($"Пользователя с id: \"{user.Id}\" не существует", user.Id);
                }
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на обновление пользователя с id: \"{0}\"", user.Id);
                    command.CommandText = "update users set name = @name, password = @password where id=@id";
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.ExecuteNonQuery();
                }
                _logger.Debug("Получение категорий пользователя с id: \"{0}\"", user.Id);
                user.Categories = _categoriesRepository.GetUserCategories(user.Id);
                _logger.Info("Обновлен пользователь с id: \"{0}\", name: \"{1}\" (имеет категорий: \"{2}\")", user.Id, user.Name, user.Categories.Count());
                return user;
            }
        }

        public void Delete(Guid userId)
        {
            _logger.Debug("Начано удаление пользователя с id: \"{0}\"", userId);
            if (!IsExist(userId))
            {
                _logger.Warn("Пользователь с id: \"{0}\" не может быть удален", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на удаление пользователя с id: \"{0}\"", userId);
                    command.CommandText = "delete from users where id=@id";
                    command.Parameters.AddWithValue("@id", userId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Пользователь с id: \"{0}\" удален", userId);
            }
        }

        public IEnumerable<User> GetUsersBySharedNote(Guid noteId)
        {
            _logger.Debug("Начато получение общих пользователей заметки с id: \"{0}\"", noteId);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            INotesRepository notesRepository = new NotesRepository(_connectionString);
            if (!notesRepository.IsExist(noteId))
            {
                throw new NoteNotFoundException($"Заметки с id: \"{noteId}\" не существует", noteId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение общих пользователей заметки с id: \"{0}\"", noteId);
                    command.CommandText = "select users.id, users.name, users.password " +
                        "from users inner join shared " +
                        "on shared.user_id=users.id " +
                        "where shared.note_id = @noteId";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfUsers = new List<User>();
                        while (reader.Read())
                        {
                            listOfUsers.Add(new User
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                Categories = _categoriesRepository.GetUserCategories(new Guid(reader.GetString(reader.GetOrdinal("id"))))
                            });
                        }
                        _logger.Info("Получено \"{0}\" пользователей для заметки с id: \"{1}\"", listOfUsers.Count(), noteId);
                        return listOfUsers;
                    }
                }
            }
        }

        public bool IsExist(Guid userId)
        {
            _logger.Debug("Провека существования пользователя с id: \"{0}\"", userId);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select name from users where id=@id";
                    command.Parameters.AddWithValue("@id", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _logger.Debug("Пользователь с id: \"{0}\" найден", userId);
                            return true;
                        }
                        _logger.Debug("Пользоветель с id: \"{0}\" не найден", userId);
                        return false;
                    }
                }
            }
        }

        public User Get(String userName)
        {
            _logger.Debug("Начато получение пользователя с именем: \"{0}\"", userName);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение пользователся с именем: \"{0}\"", userName);
                    command.CommandText = "select id, password  from users where name = @userName;";
                    command.Parameters.AddWithValue("@userName", userName);
                    var user = new User();
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Пользователь с именем: \"{userName}\" не существует");
                        }
                        user.Id = new Guid(reader.GetString(reader.GetOrdinal("id")));
                        user.Name = userName;
                        user.Password = reader.GetString(reader.GetOrdinal("password"));
                    }
                    _logger.Debug("Получение категорий пользователя с id: \"{0}\"", user.Id);
                    user.Categories = _categoriesRepository.GetUserCategories(user.Id);
                    _logger.Info("Получен пользователь с id: \"{0}\", name: \"{1}\" (имеет категорий: \"{2}\")", user.Id, user.Name, user.Categories.Count());
                    return user;
                }
            }
        }

        public IEnumerable<User> GetUsers()
        {
            _logger.Debug("Начато получение всех пользоватей с именем");
                ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение всех пользователей");
                    command.CommandText = "select * from users";
                    var users = new List<User>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(
                          new User
                          {
                              Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                              Name = reader.GetString(reader.GetOrdinal("name")),
                              Password = reader.GetString(reader.GetOrdinal("password")),
                              Categories = _categoriesRepository.GetUserCategories(new Guid(reader.GetString(reader.GetOrdinal("id"))))
                        });
                        }
                    }
                    _logger.Info("Полученo {0} пользователей",users.Count());
                    return users;
                }
            }
        }

        public IEnumerable<Guid> GetUsersForNotify(Guid userId)
        {
            _logger.Debug("Начато получение id пользователей для уведомления. Изменения от пользователя с id: \"{0}\"", userId);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            INotesRepository notesRepository = new NotesRepository(_connectionString);
            if (!notesRepository.IsExist(userId))
            {
                throw new UserNotFoundException($"Пользователя с id: \"{userId}\" не существует", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение id пользователей для их уведомления. Изменения от пользователя с id: \"{0}\"", userId);
                    command.CommandText = "select shared.user_id " +
                        "from notes inner join shared " +
                        "on notes.id=shared.note_id " +
                        "where notes.user_id = @userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfUsers = new List<Guid>();
                        while (reader.Read())
                        {
                            listOfUsers.Add(new Guid(reader.GetString(reader.GetOrdinal("user_id"))));
                        }
                        _logger.Info("Получено \"{0}\" id пользователей для уведомления. Изменения от пользователя с id: \"{1}\"", listOfUsers.Count(), userId);
                        return listOfUsers;
                    }
                }
            }
        }

        public IEnumerable<Guid> GetUsersForNotifyByNote(Guid noteId)
        {
            _logger.Debug("Начато получение id пользователей для уведомления. Изменения заметки с id: \"{0}\"", noteId);
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            INotesRepository notesRepository = new NotesRepository(_connectionString);
            if (!notesRepository.IsExist(noteId))
            {
                throw new NoteNotFoundException($"Заметки с id: \"{noteId}\" не существует", noteId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение id пользователей для уведомления. Изменена заметка с id: \"{0}\"", noteId);
                    command.CommandText = "select shared.user_id " +
                        "from notes inner join shared " +
                        "on notes.id=shared.note_id " +
                        "where notes.id = @noteId";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfUsers = new List<Guid>();
                        while (reader.Read())
                        {
                            listOfUsers.Add(new Guid(reader.GetString(reader.GetOrdinal("user_id"))));
                        }
                        _logger.Info("Получено \"{0}\" id пользователей для уведомления. Изменена заметка с id: \"{1}\"", listOfUsers.Count(), noteId);
                        return listOfUsers;
                    }
                }
            }
        }
    }
}
