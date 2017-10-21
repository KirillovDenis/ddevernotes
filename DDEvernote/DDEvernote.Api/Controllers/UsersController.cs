using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DDEvernote.Model;
using DDEvernote.DataLayer.Sql;
using DDEvernote.DataLayer;


namespace DDEvernote.Api.Controllers
{
    /// <summary>
    /// Управление пользователями
    /// </summary>
    public class UsersController : ApiController
    {
        private const string ConnectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Создание репозитория для управления пользователями
        /// </summary>
        public UsersController()
        {
            _usersRepository = new UsersRepository(ConnectionString);
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Созданный пользователь</returns>
        [HttpPost]
        [Route("api/user")]
        public User Post([FromBody] User user)
        {
            return _usersRepository.Create(user);
        }

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Существующий пользователь</returns>
        [HttpGet]
        [Route("api/user/{id}")]
        public User Get(Guid id)
        {
            return _usersRepository.Get(id);
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Обновленный пользователь</returns>
        [HttpPut]
        [Route("api/user")]
        public User Put([FromBody] User user)
        {
            return _usersRepository.Update(user);
        }

        /// <summary>
        /// Удаления пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        [HttpDelete]
        [Route("api/user/{id}")]
        public void Delete(Guid id)
        {
            _usersRepository.Delete(id);
        }
    }

}
