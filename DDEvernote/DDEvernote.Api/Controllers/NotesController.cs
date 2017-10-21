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
    /// Управление заметками
    /// </summary>
    public class NotesController : ApiController
    {
        private const string _connectionString = @"Server=DENIS-2;Database=ddevernotes;Trusted_Connection=true;";
        private readonly INotesRepository _notesRepository;

        /// <summary>
        /// Создание репозитория для управления категориями
        /// </summary>
        public NotesController()
        {
            _notesRepository = new NotesRepository(_connectionString);
        }

        /// <summary>
        /// Создание заметки
        /// </summary>
        /// <param name="note">Заметка</param>
        /// <returns>Созданная заметка</returns>
        [HttpPost]
        [Route("api/note")]
        public Note Post([FromBody] Note note)
        {
            return _notesRepository.Create(note);
        }

        /// <summary>
        /// Получение заметки
        /// </summary>
        /// <param name="note_id">Идентификатор заметки</param>
        /// <returns>Существующая заметка</returns>
        [HttpGet]
        [Route("api/note/{note_id}")]
        public Note Get(Guid note_id)
        {
            return _notesRepository.Get(note_id);
        }

        /// <summary>
        /// Обновление заметки
        /// </summary>
        /// <param name="note">Заметка</param>
        /// <returns>Обновленная заметка</returns>
        [HttpPut]
        [Route("api/note")]
        public Note Put([FromBody] Note note)
        {
            return _notesRepository.Update(note);
        }

        /// <summary>
        /// Удаление замтки
        /// </summary>
        /// <param name="note_id">Идентификтор заметки</param>
        [HttpDelete]
        [Route("api/note/{note_id}")]
        public void Delete(Guid note_id)
        {
            _notesRepository.Delete(note_id);
        }

        /// <summary>
        /// Получение всех заметок пользователя
        /// </summary>
        /// <param name="user_id">Идентификатор пользователя</param>
        /// <returns>Список заметок</returns>
        [HttpGet]
        [Route("api/user/{user_id}/notes")]
        public IEnumerable<Note> GetNotes(Guid user_id)
        {
            return _notesRepository.GetUserNotes(user_id);
        }

        /// <summary>
        /// Добавляет заметку в существующую категорию пользователя
        /// </summary>
        /// <param name="note_id">Идентификатор заметки</param>
        /// <param name="category_id">Идентификатор категории</param>
        [HttpPost]
        [Route("api/note/{note_id}/add_category/{category_id}")]
        public void AddCategoryToNote(Guid note_id, Guid category_id)
        {
            _notesRepository.AddNoteInCategory(category_id, note_id);
        }

        /// <summary>
        /// Получает все заметки из определнной категории
        /// </summary>
        /// <param name="category_id">Идентификатор категории</param>
        /// <returns>Список заметок</returns>
        [HttpGet]
        [Route("api/category/{category_id}/notes")]
        public IEnumerable<Note> GetNotesByCategory(Guid category_id)
        {
            return _notesRepository.GetNotesByCategory(category_id);
        }

        /// <summary>
        /// Предоставляет существующему пользователю доступ к заметке
        /// </summary>
        /// <param name="note_id">Идентификатор заметки</param>
        /// <param name="user_id">Идентификатор пользователя</param>
        [HttpPost]
        [Route("api/note/{note_id}/share/{user_id}")]
        public void ShareNote(Guid note_id, Guid user_id)
        {
            _notesRepository.Share(note_id, user_id);
        }

        /// <summary>
        /// Запрещает доступ пользователю к заметке
        /// </summary>
        /// <param name="note_id">Идентификатор заметки</param>
        /// <param name="user_id">Идентификатор пользователя</param>
        [HttpDelete]
        [Route("api/note/{note_id}/deny_shared/{user_id}")]
        public void DenyShared(Guid note_id, Guid user_id)
        {
            _notesRepository.DenyShared(note_id, user_id);
        }
    }
}
