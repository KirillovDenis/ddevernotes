using DDEvernote.Model;
using DDEvernote.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelNoteWindow : INotifyPropertyChanged
    {
        private ServiceClient _serviceclient;
        public Note Note { get; set; }
        public bool IsDeleted { get; set; }

        #region Commands

        private Command saveNoteCommand;
        public Command SaveNoteCommand
        {
            get { return saveNoteCommand; }
            set
            {
                saveNoteCommand = value;
                OnPropertyChanged("SaveNoteCommand");
            }
        }
        private Command deleteNoteCommand;
        public Command DeleteNoteCommand
        {
            get { return deleteNoteCommand; }
            set
            {
                deleteNoteCommand = value;
                OnPropertyChanged("DeleteNoteCommand");
            }
        }

        #endregion

        public ModelNoteWindow(Note note)
        {
            ResourceManager rm = new ResourceManager("DDEvernote.WPF.ConnectionResource",
                            Assembly.GetExecutingAssembly());
            string connString = rm.GetString("ConnectionString");
            _serviceclient = new ServiceClient(connString);
            this.Note = note;
            IsDeleted = false;

            SaveNoteCommand = new Command((noteWindow) => { SaveNote((Window)noteWindow); });
            DeleteNoteCommand = new Command((noteWindow) => { DeleteNote((Window)noteWindow); });
        }

        private void SaveNote(Window window)
        {
            if (this.Note.Title == string.Empty || this.Note.Title==null)
            {
                MessageBox.Show("Заметка должна иметь название", "Заметка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (this.Note.Id == Guid.Empty)
            {
                Note = _serviceclient.CreateNote(this.Note);
            }
            else
            {
                Note = _serviceclient.UpdateNote(this.Note);
            }
            window.Close();
        }
        private void DeleteNote(Window window)
        {
            if (this.Note.Id != Guid.Empty)
            {
                _serviceclient.DeleteNote(this.Note.Id);
                IsDeleted = true;
            }
            window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
    }
}
