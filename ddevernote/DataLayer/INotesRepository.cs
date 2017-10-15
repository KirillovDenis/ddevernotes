using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ddevernote.Model;

namespace ddevernote.DataLayer
{
    public interface INotesRepository
    {
        Note Create(Note note);
        Note Get(Guid noteId);
        Note Update(Note note);
        void Delete(Guid noteId);
        Note AddNote(Guid categoryId, Guid noteId);
        IEnumerable<Note> GetUserNotes(Guid userId);
        IEnumerable<Note> GetUserNotesByCategory(Guid userId, string categoryName);
    }
}
