using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DDEvernote.Model;
using DDEvernote.DataLayer;
using DDEvernote.DataLayer.Sql;

namespace DDEvernote.Api.Controllers
{
    /// <summary>
    /// Управления категориями
    /// </summary>
    public class CategoriesController : ApiController
    {
        private const string _connectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly ICategoriesRepository _categoriesRepository;
        
        /// <summary>
        /// Создание репозитория для управления категориями
        /// </summary>
        public CategoriesController()
        {
            _categoriesRepository = new CategoriesRepository(_connectionString);
        }

        /// <summary>
        /// Создание категории у пользователя
        /// </summary>
        /// <param name="user_id">Идентификатор пользователя, которому будет принадлежать категория</param>
        /// <param name="category">Название категории</param>
        /// <returns>Созданная категория</returns>
        [HttpPost]
        [Route("api/user/{user_id}/category")]
        public Category Post(Guid user_id, [FromBody] Category category)
        {
            return _categoriesRepository.Create(user_id, category.Title);
        }

        /// <summary>
        /// Получает категорию по идентификатору
        /// </summary>
        /// <param name="category_id">Идентификатор</param>
        /// <returns>Существующая категория</returns>
        [HttpGet]
        [Route("api/category/{category_id}")]
        public Category Get(Guid category_id)
        {
            return _categoriesRepository.Get(category_id);
        }

        /// <summary>
        /// Обновляет название категории
        /// </summary>
        /// <param name="category">Категория</param>
        /// <returns>Обновленная категория</returns>
        [HttpPut]
        [Route("api/category")]
        public Category Put([FromBody] Category category)
        {
            return _categoriesRepository.UpdateTitle(category.Id, category.Title);
        }

        /// <summary>
        /// Получение всех категорий пользователя
        /// </summary>
        /// <param name="user_id">Идентификтор пользователя</param>
        /// <returns>Список категорий</returns>
        [HttpGet]
        [Route("api/user/{user_id}/categories")]
        public IEnumerable<Category> GetCateories(Guid user_id)
        {
            return _categoriesRepository.GetUserCategories(user_id);
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="category_id">Идентификатор категории</param>
        [HttpDelete]
        [Route("api/category/{category_id}")]
        public void Delete(Guid category_id)
        {
            _categoriesRepository.Delete(category_id);
        }
    }
}
