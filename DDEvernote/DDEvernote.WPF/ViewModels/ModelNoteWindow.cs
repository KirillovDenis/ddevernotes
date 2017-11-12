using DDEvernote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelNoteWindow
    {
        private ServiceClient _serviceclient;
        public Note Note { get; set; }

        public ModelNoteWindow(Note note)
        {
            _serviceclient = new ServiceClient("http://localhost:52395/api/");
            this.Note = note;
        }

        public void saveNote()
        {
            if (this.Note.Id == Guid.Empty)
            {
                Note = _serviceclient.CreateNote(this.Note);
            }
            else
            {
                Note = _serviceclient.UpdateNote(this.Note);
            }
        }

        public void deleteNote()
        {
            if (this.Note.Id != Guid.Empty)
            {
                _serviceclient.DeleteNote(this.Note.Id);
            }
        }
    }
}
