using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DDEvernote.Model;

namespace DDEvernote.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        

        public UsersRepository(String connectionString)
        {
            _connectionString = connectionString;
        }

        public User Create(User user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                    command.CommandText = "insert into users (id, name, password) " +
                        "values (@id, @name, @password);";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.ExecuteNonQuery();
                    user.Categories = new List<Category>();
                    return user;
                }
            }
        }

        public void Delete(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from users where id=@id";
                    command.Parameters.AddWithValue("@id", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public User Get(Guid Id)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select name, password  from users where id = @id;";
                    command.Parameters.AddWithValue("@id", Id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Пользователь с id {Id} не найден");
                        }
                        var user = new User()
                        {
                            Id = Id,
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Password = reader.GetString(reader.GetOrdinal("password"))
                        };
                        user.Categories = _categoriesRepository.GetUserCategories(user.Id);
                        return user;
                    }
                }
            }
        }

        public IEnumerable<User> GetUsersBySharedNote(Guid noteId)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
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
                        return listOfUsers;
                    }
                }
            }
        }

        public bool IsExist(Guid userId)
        {
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
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public User Update(User user)
        {
            ICategoriesRepository _categoriesRepository = new CategoriesRepository(_connectionString);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                if (IsExist(user.Id))
                {
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = "update users set name = @name, password = @password where id=@id";
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@id", user.Id);
                        command.ExecuteNonQuery();
                        user.Categories = _categoriesRepository.GetUserCategories(user.Id);
                        return user;
                    }
                }
                throw new ArgumentException($"Пользователя с id { user.Id } не существует");
            }
        }
    }
}
