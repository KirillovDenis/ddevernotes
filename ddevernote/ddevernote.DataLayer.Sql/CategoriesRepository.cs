using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DDEvernote.Model;

namespace DDEvernote.DataLayer.Sql
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly string _connectionString;
        public CategoriesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Category UpdateTitle(Guid categoryId, string newTitle)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "update category set title = @title  where id=@id";
                    command.Parameters.AddWithValue("@id", categoryId);
                    command.Parameters.AddWithValue("@title", newTitle);
                    command.ExecuteNonQuery();
                }
                return new Category
                {
                    Id = categoryId,
                    Title = newTitle
                };
            }
        }

        public Category Create(Guid userId, string categoryTitle)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    var category = new Category
                    {
                        Id = Guid.NewGuid(),
                        Title = categoryTitle
                    };
                    command.CommandText = "insert into category(id, title, user_id) values(@id, @title, @userId)";
                    command.Parameters.AddWithValue("@id", category.Id);
                    command.Parameters.AddWithValue("@title", category.Title);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                    return category;
                }
            }
        }

        public void Delete(Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from category where id=@id";
                    command.Parameters.AddWithValue("@id", categoryId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Category> GetUserCategories(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select id, title from category where user_id=@userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        var listOfCategory = new List<Category>();
                        while (reader.Read())
                        {
                            listOfCategory.Add(new Category
                            {
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id")))
                            });
                        }
                        return listOfCategory;
                    }
                }
            }
        }

        public Category Get(Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select title from category where id=@id";
                    command.Parameters.AddWithValue("@id", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Категория с id {categoryId} не найдена");
                        }
                        return new Category
                        {
                            Id = categoryId,
                            Title = reader.GetString(reader.GetOrdinal("title"))
                        };
                    }
                }
            }
        }

        public bool IsExist(Guid categoryId)
        {
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
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public IEnumerable<Category> GetCategoriesOfNote(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select category.id, category.title " +
                        "from category_notes inner join category " +
                        "on category_notes.category_id = category.id " +
                        "where category_notes.note_id = @noteId;";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        List<Category> listOfCategory = new List<Category>();
                        while (reader.Read())
                        {
                            listOfCategory.Add(new Category
                            {
                                Id = new Guid(reader.GetString(reader.GetOrdinal("id"))),
                                Title = reader.GetString(reader.GetOrdinal("title"))
                            });
                        }
                        return listOfCategory;
                    }
                }
            }
        }
    }
}
