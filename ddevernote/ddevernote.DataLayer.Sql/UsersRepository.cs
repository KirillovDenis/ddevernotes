using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ddevernote.Model;

namespace ddevernote.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        private readonly ICategoriesRepository _categoriesRepository;

        public UsersRepository(String connectionString, ICategoriesRepository categoriesRepository)
        {
            _connectionString = connectionString;
            _categoriesRepository = categoriesRepository;
        }

        public User Create(User user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                    command.CommandText = "insert into users (id, name, password) values (@id, @name, @password)";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.ExecuteNonQuery();
                    return user;
                }
            }
        }
        public void Delete(Guid userId)
        {//TODO cascade
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select name, password from users where id = @id";
                    command.Parameters.AddWithValue("@id", Id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Пользователь с id {Id} не найден");
                        }
                        var user = new User
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select users.id, users.name, users.password from users inner join shared on shared.user_id=users.id where shared.note_id = @noteId";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        yield return new User
                        {
                            Id = new Guid(reader.GetString(reader.GetOrdinal("users.id"))),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Password = reader.GetString(reader.GetOrdinal("password")),
                            Categories = _categoriesRepository.GetUserCategories(new Guid(reader.GetString(reader.GetOrdinal("users.id"))))
                        };
                    }
                }
            }
        }
        public User Update(User user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    var existedUser = Get(user.Id);
                }
                catch (ArgumentException ex)
                {
                    throw ex;
                }
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "update users set name = @name, password = @password where id=@id";
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.ExecuteNonQuery();
                    return user;
                }
            }
        }
    }
}
