using System;
using System.Collections.Generic;
using System.Web.Http;
using DDEvernote.Model;
using DDEvernote.DataLayer;
using DDEvernote.DataLayer.Sql;
using DDEvernote.Api.Filters;
using System.Web.Configuration;

namespace DDEvernote.Api.Controllers
{
    /// <summary>
    /// Управление заметками
    /// </summary>
    public class NotesController : ApiController
    {
        private readonly string _connectionString = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        private readonly INotesRepository _notesRepository;
        private readonly NLog.Logger _logger;

        /// <summary>
        /// Создание репозитория для управления категориями
        /// </summary>
        public NotesController()
        {
            _notesRepository = new NotesRepository(_connectionString);
            _logger = Logger.Log.Instance;
        }

        /// <summary>
        /// Создание заметки
        /// </summary>
        /// <param name="note">Заметка</param>
        /// <returns>Созданная заметка</returns>
        [HttpPost]
        [Route("api/notes")]
        [UsersExceptionFilter]
        public Note Post([FromBody] Note note)
        {
            _logger.Info("Запрос на создание заметки: \"{0}\" для пользователя с id: \"{1}\"", note.Title, note.Owner.Id);
            return _notesRepository.Create(note);
        }

        /// <summary>
        /// Получение заметки
        /// </summary>
        /// <param name="noteId">Идентификатор заметки</param>
        /// <returns>Существующая заметка</returns>
        [HttpGet]
        [Route("api/notes/{noteId}")]
        [NotesExceptionFilter]
        public Note Get(Guid noteId)
        {
            _logger.Info("Запрос на получение заметки с id: \"{0}\"", noteId);
            return _notesRepository.Get(noteId);
        }

        /// <summary>
        /// Обновление заметки
        /// </summary>
        /// <param name="note">Заметка</param>
        /// <returns>Обновленная заметка</returns>
        [HttpPut]
        [Route("api/notes")]
        [NotesExceptionFilter]
        public Note Put([FromBody] Note note)
        {
            _logger.Info("Запрос на обновление заметки с id: \"{0}\"", note.Id);
            return _notesRepository.Update(note);
        }

        /// <summary>
        /// Добавляет заметку в существующую категорию пользователя
        /// </summary>
        /// <param name="noteId">Идентификатор заметки</param>
        /// <param name="categoryId">Идентификатор категории</param>
        [HttpPost]
        [Route("api/notes/{noteId}/add_category")]
        [NotesExceptionFilter]
        [CategoriesExceptionFilter]
        public Guid AddCategoryToNote(Guid noteId, [FromBody]Guid categoryId)
        {
            _logger.Info("Запрос на добавление заметки с id: \"{0}\" в категорию с id: \"{1}\"", noteId, categoryId);
            _notesRepository.AddNoteInCategory(noteId, categoryId);
            return noteId;
        }

        /// <summary>
        /// Удаляет заметку из существующей категории пользователя
        /// </summary>
        /// <param name="noteId">Идентификатор заметки</param>
        /// <param name="categoryId">Идентификатор категории</param>
        [HttpDelete]
        [Route("api/notes/{noteId}/delete_category/{categoryId}")]
        [NotesExceptionFilter]
        [CategoriesExceptionFilter]
        public void DeleteCategoryFromNote(Guid noteId, Guid categoryId)
        {
            _logger.Info("Запрос на удаление заметки с id: \"{0}\" из категории с id: \"{1}\"", noteId, categoryId);
            _notesRepository.DeleteNoteFromCategory(categoryId, noteId);
        }

        /// <summary>
        /// Предоставляет существующему пользователю доступ к заметке
        /// </summary>
        /// <param name="noteId">Идентификатор заметки</param>
        /// <param name="userId">Идентификатор пользователя</param>
        [HttpPost]
        [Route("api/notes/{noteId}/shares")]
        [NotesExceptionFilter]
        [UsersExceptionFilter]
        public Guid ShareNote(Guid noteId, [FromBody]Guid userId)
        {
            _logger.Info("Запрос на предоставление общего доступа к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
            _notesRepository.Share(noteId, userId);
            return userId;
        }

        /// <summary>
        /// Запрещает доступ пользователю к заметке
        /// </summary>
        /// <param name="noteId">Идентификатор заметки</param>
        /// <param name="userId">Идентификатор пользователя</param>
        [HttpDelete]
        [Route("api/notes/{noteId}/shares/{userId}")]
        [NotesExceptionFilter]
        [UsersExceptionFilter]
        public void DenyShared(Guid noteId, Guid userId)
        {
            _logger.Info("Запрос на закрытие общего доступа к заметке с id: \"{0}\" пользователю с id: \"{1}\"", noteId, userId);
            _notesRepository.DenyShared(noteId, userId);
        }

        /// <summary>
        /// Удаление заметки
        /// </summary>
        /// <param name="noteId">Идентификтор заметки</param>
        [HttpDelete]
        [Route("api/notes/{noteId}")]
        public void Delete(Guid noteId)
        {
            _logger.Info("Запрос на удаление заметки с id: \"{0}\"", noteId);
            _notesRepository.Delete(noteId);
        }

        /// <summary>
        /// Получение всех заметок пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список заметок</returns>
        [HttpGet]
        [Route("api/users/{userId}/notes")]
        [UsersExceptionFilter]
        public IEnumerable<Note> GetNotes(Guid userId)
        {
            _logger.Info("Запрос на получение заметок пользователя с id: \"{0}\"", userId);
            return _notesRepository.GetUserNotes(userId);
        }

        /// <summary>
        /// Получение общих заметок пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список заметок</returns>
        [HttpGet]
        [Route("api/users/{userId}/shared")]
        [UsersExceptionFilter]
        public IEnumerable<Note> GetSharedNotesByUser(Guid userId)
        {
            _logger.Info("Запрос на получение общих заметок пользователя с id: \"{0}\"", userId);
            return _notesRepository.GetSharedNotesByUser(userId);
        }

        /// <summary>
        /// Получение общих заметок пользователя от определенного пользователя
        /// </summary>
        /// <param name="ownerUserId">Идентификатор хозяина заметок</param>
        /// <param name="sharedUserId">Идентификатор пользователя, с которым поделилиськ</param>
        /// <returns>Список заметок</returns>
        [HttpGet]
        [Route("api/users/{ownerUserId}/shared/{sharedUserId}")]
        [UsersExceptionFilter]
        public IEnumerable<Note> GetNotesBySharedUser(Guid ownerUserId, Guid sharedUserId)
        {
            _logger.Info("Запрос на получение общих заметок пользователя c id: \"{0}\" от пользователя с id: \"{1}\"", sharedUserId, ownerUserId);
            return _notesRepository.GetNotesBySharedUser(ownerUserId, sharedUserId);
        }

        /// <summary>
        /// Получает все заметки из определенной категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Список заметок</returns>
        [HttpGet]
        [Route("api/categories/{categoryId}/notes")]
        [CategoriesExceptionFilter]
        public IEnumerable<Note> GetNotesByCategory(Guid categoryId)
        {
            _logger.Info("Запрос на получение заметок из категории с id: \"{0}\"", categoryId);
            return _notesRepository.GetNotesByCategory(categoryId);
        }
    }
}
