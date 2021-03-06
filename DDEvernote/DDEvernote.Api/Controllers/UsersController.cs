﻿using System;
using System.Web.Http;
using DDEvernote.Model;
using DDEvernote.DataLayer.Sql;
using DDEvernote.DataLayer;
using DDEvernote.Api.Filters;
using System.Collections.Generic;
using System.Web.Configuration;

namespace DDEvernote.Api.Controllers
{
    /// <summary>
    /// Управление пользователями
    /// </summary>
    public class UsersController : ApiController
    {
        private readonly string ConnectionString = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        private readonly IUsersRepository _usersRepository;
        private readonly NLog.Logger _logger;

        /// <summary>
        /// Создание репозитория для управления пользователями
        /// </summary>
        public UsersController()
        {
            _usersRepository = new UsersRepository(ConnectionString);
            _logger = Logger.Log.Instance;
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Созданный пользователь</returns>
        [HttpPost]
        [Route("api/users")]
        public User Post([FromBody] User user)
        {
            _logger.Info("Запрос на создание пользователя с именем: \"{0}\"", user.Name);
            return _usersRepository.Create(user);
        }

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Существующий пользователь</returns>
        [HttpGet]
        [Route("api/users/{userId}")]
        [UsersExceptionFilter]
        public User Get(Guid userId)
        {
            _logger.Info("Зарос на получение пользователя с id: \"{0}\"", userId);
            return _usersRepository.Get(userId);
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Обновленный пользователь</returns>
        [HttpPut]
        [Route("api/users")]
        [UsersExceptionFilter]
        public User Put([FromBody] User user)
        {
            _logger.Info("Запрос на обновление пользователя с id: \"{0}\"", user.Id);
            return _usersRepository.Update(user);
        }

        /// <summary>
        /// Удаления пользователя по идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        [HttpDelete]
        [Route("api/users/{userId}")]
        public void Delete(Guid userId)
        {
            _logger.Info("Запрос на удаление пользователя с id: \"{0}\"", userId);
            _usersRepository.Delete(userId);
        }

        /// <summary>
        /// Получение пользователя по имени
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>Пользователь</returns>
        [HttpGet]
        [Route("api/users/name/{userName}")]
        [UsersExceptionFilter]
        public User GetUserByName(String userName)
        {
            _logger.Info("Зарос на получение пользователя с именем: \"{0}\"", userName);
            return _usersRepository.Get(userName);
        }

        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        /// <returns>Пользователи</returns>
        [HttpGet]
        [Route("api/users")]
        [UsersExceptionFilter]
        public IEnumerable<User> GetUsers()
        {
            _logger.Info("Зарос на получение всех пользователей");
            return _usersRepository.GetUsers();
        }

        /// <summary>
        /// Получение общих пользователей для заметки
        /// </summary>
        /// <param name="noteId">Идентификатор заметки</param>
        /// <returns>Пользователи</returns>
        [HttpGet]
        [Route("api/notes/{noteId}/shared")]
        [NotesExceptionFilter]
        public IEnumerable<User> GetUsersBySharedNote(Guid noteId)
        {
            _logger.Info("Зарос на получение общих пользователей для заметки с id: \"{0}\"", noteId);
            return _usersRepository.GetUsersBySharedNote(noteId);
        }
    }

}
