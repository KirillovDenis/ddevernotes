using System;
using System.Collections.Generic;
using System.Web.Http;
using DDEvernote.Model;
using DDEvernote.DataLayer;
using DDEvernote.DataLayer.Sql;
using DDEvernote.Api.Filters;

namespace DDEvernote.Api.Controllers
{
    /// <summary>
    /// Управления категориями
    /// </summary>
    public class CategoriesController : ApiController
    {
        private const string _connectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly NLog.Logger _logger;
        
        /// <summary>
        /// Создание репозитория для управления категориями
        /// </summary>
        public CategoriesController()
        {
            _categoriesRepository = new CategoriesRepository(_connectionString);
            _logger = Logger.Log.Instance;
        }

        /// <summary>
        /// Создание категории у пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, которому будет принадлежать категория</param>
        /// <param name="category">Название категории</param>
        /// <returns>Созданная категория</returns>
        [HttpPost]
        [Route("api/users/{userId}/categories")]
        [UsersExceptionFilter]
        public Category Post(Guid userId, [FromBody] Category category)
        {
            _logger.Info("Запрос на создание категории: \"{0}\" для пользователя с id: \"{1}\"", category.Title, userId);
            return _categoriesRepository.Create(userId, category.Title);
        }

        /// <summary>
        /// Получает категорию по идентификатору
        /// </summary>
        /// <param name="categoryId">Идентификатор</param>
        /// <returns>Существующая категория</returns>
        [HttpGet]
        [Route("api/categories/{categoryId}")]
        [CategoriesExceptionFilter]
        public Category Get(Guid categoryId)
        {
            _logger.Info("Зарос на получение категории с id: \"{0}\"", categoryId);
            return _categoriesRepository.Get(categoryId);
        }

        /// <summary>
        /// Обновляет название категории
        /// </summary>
        /// <param name="category">Категория</param>
        /// <returns>Обновленная категория</returns>
        [HttpPut]
        [Route("api/categories")]
        [CategoriesExceptionFilter]
        public Category Put([FromBody] Category category)
        {
            _logger.Info("Запрос на обновление категории с id: \"{0}\"", category.Id);
            return _categoriesRepository.Update(category);
        }

        /// <summary>
        /// Получение всех категорий пользователя
        /// </summary>
        /// <param name="userId">Идентификтор пользователя</param>
        /// <returns>Список категорий</returns>
        [HttpGet]
        [Route("api/users/{userId}/categories")]
        [UsersExceptionFilter]
        public IEnumerable<Category> GetCategories(Guid userId)
        {
            _logger.Info("Запрос на получение категорий пользователся с id: \"{0}\"", userId);
            return _categoriesRepository.GetUserCategories(userId);
        }
        

        /// <summary>
        /// Получение всех категорий заметки
        /// </summary>
        /// <param name="noteId">Идентификтор заметки</param>
        /// <returns>Список категорий</returns>
        [HttpGet]
        [Route("api/notes/{noteId}/categories")]
        [NotesExceptionFilter]
        public IEnumerable<Category> GetCategoriesByNote(Guid noteId)
        {
            _logger.Info("Запрос на получение категорий заметки с id: \"{0}\"", noteId);
            return _categoriesRepository.GetCategoriesOfNote(noteId);
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        [HttpDelete]
        [Route("api/categories/{categoryId}")]
        public void Delete(Guid categoryId)
        {
            _logger.Info("Запрос на удаление категории с id: \"{0}\"", categoryId);
            _categoriesRepository.Delete(categoryId);
        }
    }
}
