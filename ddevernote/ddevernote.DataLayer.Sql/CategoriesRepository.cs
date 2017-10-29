using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using DDEvernote.Model;

namespace DDEvernote.DataLayer.Sql
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly string _connectionString;
        private readonly NLog.Logger _logger;

        public CategoriesRepository(string connectionString)
        {
            _connectionString = connectionString;
            _logger = Logger.Log.Instance;
            _logger.Debug("Создание репозитория управления категориями с помощью: \"{0}\"", connectionString);
        }

        public Category Create(Guid userId, string categoryTitle)
        {
            _logger.Debug("Начато создание категории с названием: \"{0}\" для пользователя с id: \"{1}\"", categoryTitle, userId);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            if (!usersRepository.IsExist(userId))
            {
                throw new UserNotFoundException($"Пользователя с id: \"{userId}\" не существует", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Title = categoryTitle
                };
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на создание категории с названием: \"{0}\" для пользователя с id: \"{0}\"", categoryTitle, userId);
                    command.CommandText = "insert into category(id, title, user_id) values(@id, @title, @userId)";
                    command.Parameters.AddWithValue("@id", category.Id);
                    command.Parameters.AddWithValue("@title", category.Title);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Создана категория с id: \"{0}\", название: \"{1}\" для пользователя с id: \"{2}\"", category.Id, category.Title, userId);
                return category;
            }
        }

        public Category Get(Guid categoryId)
        {
            _logger.Debug("Начато получение категории с id: \"{0}\"", categoryId);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение категории с id: \"{0}\"", categoryId);
                    command.CommandText = "select title from category where id=@id";
                    command.Parameters.AddWithValue("@id", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new CategoryNotFoundException($"Категория с id: \"{categoryId}\" не найдена", categoryId);
                        }
                        _logger.Info("Получена категория с id: \"{0}\", названием: \"{1}\"", categoryId, reader.GetString(reader.GetOrdinal("title")));
                        return new Category
                        {
                            Id = categoryId,
                            Title = reader.GetString(reader.GetOrdinal("title"))
                        };
                    }
                }
            }
        }

        public Category Update(Category category)
        {
            _logger.Debug("Начато обновление категории с id: \"{0}\"", category.Id);
            if (!IsExist(category.Id))
            {
                throw new CategoryNotFoundException($"Категория с id: \"{category.Id}\" не существует", category.Id);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на обновление категории с id: \"{0}\"", category.Id);
                    command.CommandText = "update category set title = @title  where id=@id";
                    command.Parameters.AddWithValue("@id", category.Id);
                    command.Parameters.AddWithValue("@title", category.Title);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Обновлена категория с id: \"{0}\", названием: \"{1}\"", category.Id, category.Title);
                return category;
            }
        }

        public void Delete(Guid categoryId)
        {
            _logger.Debug("Начано удаление категории с id: \"{0}\"", categoryId);
            if (!IsExist(categoryId))
            {
                _logger.Warn("Категория с id: \"{0}\" не может быть удалена", categoryId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на удаление категории с id: \"{0}\"", categoryId);
                    command.CommandText = "delete from category where id=@id";
                    command.Parameters.AddWithValue("@id", categoryId);
                    command.ExecuteNonQuery();
                }
                _logger.Info("Категория с id: \"{0}\" удалена", categoryId);
            }
        }

        public IEnumerable<Category> GetUserCategories(Guid userId)
        {
            _logger.Debug("Начато получение категорий пользователя с id: \"{0}\"", userId);
            IUsersRepository usersRepository = new UsersRepository(_connectionString);
            if (!usersRepository.IsExist(userId))
            {
                throw new UserNotFoundException($"Пользователь с id: \"{userId}\" не существует", userId);
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    _logger.Debug("Запрос к базе данных на получение категорий пользователя с id: \"{0}\"", userId);
                    command.CommandText = "select id, title from category where user_id=@userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfCategories = new List<Category>();
                        while (reader.Read())
                        {
                            listOfCategories.Add(new Category
                            {
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id")))
                            });
                        }
                        _logger.Info("Получено \"{0}\" категорий для пользователя с id: \"{1}\"", listOfCategories.Count(), userId);
                        return listOfCategories;
                    }
                }
            }
        }

        public IEnumerable<Category> GetCategoriesOfNote(Guid noteId)
        {
            _logger.Debug("Начато получение категорий заметки с id: \"{0}\"", noteId);
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
                    _logger.Debug("Запрос к базе данных на получение категорий заметки с id: \"{0}\"", noteId);
                    command.CommandText = "select category.id, category.title " +
                        "from category_notes inner join category " +
                        "on category_notes.category_id = category.id " +
                        "where category_notes.note_id = @noteId;";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        List<Category> listOfCategories = new List<Category>();
                        while (reader.Read())
                        {
                            listOfCategories.Add(new Category
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Title = reader.GetString(reader.GetOrdinal("title"))
                            });
                        }
                        _logger.Info("Получено \"{0}\" категорий для заметки с id: \"{1}\"", listOfCategories.Count(), noteId);
                        return listOfCategories;
                    }
                }
            }
        }

        public bool IsExist(Guid categoryId)
        {
            _logger.Debug("Провека существования категории с id: \"{0}\"", categoryId);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select title from category where id=@id";
                    command.Parameters.AddWithValue("@id", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _logger.Debug("Категория с id: \"{0}\" найдена", categoryId);
                            return true;
                        }
                        _logger.Debug("Категория с id: \"{0}\" не найдена", categoryId);
                        return false;
                    }
                }
            }
        }
    }
}
