using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDEvernote.Model;

namespace DDEvernote.DataLayer
{
    public interface INotesRepository
    {
        Note Create(Note note);
        Note Get(Guid noteId);
        Note Update(Note note);
        void AddNoteInCategory(Guid categoryId, Guid noteId);
        void DeleteNoteFromCategory(Guid categoryId, Guid noteId);
        void Share(Guid noteId, Guid userId);
        void DenyShared(Guid noteId, Guid userId);
        void Delete(Guid noteId);
        IEnumerable<Note> GetUserNotes(Guid userId);
        IEnumerable<Note> GetNotesByCategory(Guid categoryId);
        IEnumerable<Note> GetSharedNotesByUser(Guid userId);
        IEnumerable<Note> GetNotesBySharedUser(Guid ownerUserId, Guid sharedUserId);
        bool IsExist(Guid noteId);
        IEnumerable<Guid> GetUserNotify(Guid noteId);
    }
}
