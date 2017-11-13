using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DDEvernote.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DDEvernote.WPF.Views.Windows;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelMainPage : INotifyPropertyChanged
    {
        private ServiceClient _client;
        private bool _isEnableMainWindow;
        public bool IsEnableMainWindow
        {
            get { return _isEnableMainWindow; }
            set
            {
                _isEnableMainWindow = value;
                OnPropertyChanged("IsEnableMainWindow");
            }
        }
        private Category _selectedCategory;
        private Note _selectedNote;
        public Note SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                _selectedNote = value;
                OnPropertyChanged("SelectedNote");
            }
        }
        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }
        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }
        private Visibility _visibilityPropertyOfNote;
        public Visibility VisibilityPropertyOfNote
        {
            get { return _visibilityPropertyOfNote; }
            set
            {
                _visibilityPropertyOfNote = value;
                OnPropertyChanged("VisibilityPropertyOfNote");
            }
        }
        private ObservableCollection<Note> _userNotes;
        public ObservableCollection<Note> UserNotes
        {
            get { return _userNotes; }
            set
            {
                _userNotes = value;
                OnPropertyChanged("UserNotes");
            }
        }
        private ObservableCollection<Category> _userCategories;
        public ObservableCollection<Category> UserCategories
        {
            get { return _userCategories; }
            set
            {
                _userCategories = value;
                OnPropertyChanged("UserCategories");
            }
        }
        private ObservableCollection<Note> _notesOfSelectedCategory;
        public ObservableCollection<Note> NotesOfSelectedCategory
        {
            get { return _notesOfSelectedCategory; }
            set
            {
                _notesOfSelectedCategory = value;
                OnPropertyChanged("NotesOfSelectedCategory");
            }
        }
        private ObservableCollection<Note> _notesOfSelectedUser;
        public ObservableCollection<Note> NotesOfSelectedUser
        {
            get { return _notesOfSelectedUser; }
            set
            {
                _notesOfSelectedUser = value;
                OnPropertyChanged("NotesOfSelectedUser");
            }
        }
        private ObservableCollection<Category> _categoriesOfSelectedNote;
        public ObservableCollection<Category> CategoriesOfSelectedNote
        {
            get { return _categoriesOfSelectedNote; }
            set
            {
                _categoriesOfSelectedNote = value;
                OnPropertyChanged("CategoriesOfSelectedNote");
            }
        }

        public ModelMainPage(User user)
        {
            this.User = user;
            _client = new ServiceClient("http://localhost:52395/api/");
            UserNotes = new ObservableCollection<Note>(getNotes(user.Id));
            UserCategories = new ObservableCollection<Category>(user.Categories);
            Users = new ObservableCollection<User>(GetUsers().Where(u => u.Id != _user.Id));
            NotesOfSelectedCategory = new ObservableCollection<Note>();
            NotesOfSelectedUser = new ObservableCollection<Note>();
            IsEnableMainWindow = true;
            VisibilityPropertyOfNote = Visibility.Hidden;
        }

        private IEnumerable<Note> getNotes(Guid userId)
        {
            return _client.GetNotes(userId).Concat(_client.GetSharedNotesByUser(userId));
        }

        public void DeleteNote(Guid noteId)
        {
            _client.DeleteNote(noteId);
            UserNotes.Remove(UserNotes.Where(n => n.Id == noteId).Single());
            NotesOfSelectedCategory.Remove(NotesOfSelectedCategory.Where(n => n.Id == noteId).Single());
            NotesOfSelectedUser.Remove(NotesOfSelectedUser.Where(n => n.Id == noteId).Single());
        }
        public void LoadNotes(object sender, EventArgs arg)
        {
            UserNotes = new ObservableCollection<Note>(getNotes(User.Id));
        }

        public void CreateCategory()
        {
            var categoryWin = new CreateCategoryWindow(_user);
            categoryWin.Closed += (object o, EventArgs arg) =>
              {
                  User.Categories = new List<Category>(_client.GetCategoriesByUser(_user.Id));
                  UserCategories = new ObservableCollection<Category>(_user.Categories);
              };
            categoryWin.ShowDialog();
        }

        public void DeleteCategory(Category category)
        {
            _client.DeleteCategory(category.Id);
            UserCategories.Remove(category);
        }

        private void AddNotesInCategory(IEnumerable<Guid> notesId, Guid categoryId)
        {
            if (notesId.Count() > 0)
            {
                foreach (var noteId in notesId)
                {
                    _client.AddNoteInCategory(noteId, categoryId);
                }
            }
        }

        private void DeleteNotesFromCategory(IEnumerable<Guid> notesId, Guid categoryId)
        {
            if (notesId.Count() > 0)
            {
                foreach (var noteId in notesId)
                {
                    _client.DeleteNoteFromCategory(noteId, categoryId);
                }
            }
        }

        private void ShareNotesToUser(IEnumerable<Guid> notesId, Guid userId)
        {
            if (notesId.Count() > 0)
            {
                foreach (var noteId in notesId)
                {
                    _client.ShareNote(noteId, userId);
                }
            }
        }
        private void DenyShareNotesToUser(IEnumerable<Guid> notesId, Guid userId)
        {
            if (notesId.Count() > 0)
            {
                foreach (var noteId in notesId)
                {
                    _client.DenyShareNote(noteId, userId);
                }
            }
        }
        private void AddCategoriesToNote(IEnumerable<Guid> categoriesId, Guid noteId)
        {
            if (categoriesId.Count() > 0)
            {
                foreach (var categoryId in categoriesId)
                {
                    _client.AddNoteInCategory(noteId, categoryId);
                }
            }
        }

        private void DeleteCategoriesFromNote(IEnumerable<Guid> categoriesId, Guid noteId)
        {
            if (categoriesId.Count() > 0)
            {
                foreach (var categoryId in categoriesId)
                {
                    _client.DeleteNoteFromCategory(noteId, categoryId);
                }
            }
        }

        public void SelectCategory(Category category)
        {
            _selectedCategory = category;
            NotesOfSelectedCategory = new ObservableCollection<Note>(GetNotesByCategory(category.Id));
        }

        public void SelectUser(User user)
        {
            _selectedUser = user;
            NotesOfSelectedUser = new ObservableCollection<Note>(GetSharedNotesByTwoUser(user.Id));
        }

        public void SelectNote(Note note)
        {
            SelectedNote = note;
            SelectedNote.Categories = new ObservableCollection<Category>(GetCategoriesByNote(note.Id));
            if (VisibilityPropertyOfNote == Visibility.Hidden)
            {
                VisibilityPropertyOfNote = Visibility.Visible;
            }
        }

        private IEnumerable<Note> GetNotesByCategory(Guid categoryId)
        {
            return _client.GetNotesByCategory(categoryId);
        }

        private IEnumerable<Category> GetCategoriesByNote(Guid noteId)
        {
            return _client.GetCategoriesByNote(noteId);
        }

        private void ShareNote(IEnumerable<Guid> usersId, Guid noteId)
        {
            foreach (var userId in usersId)
            {
                _client.ShareNote(noteId, userId);
            }
        }

        private void DenyShareNote(IEnumerable<Guid> usersId, Guid noteId)
        {
            foreach (var userId in usersId)
            {
                _client.DenyShareNote(noteId, userId);
            }
        }

        public void DenyShareNote(Guid noteId)
        {
            _client.DenyShareNote(noteId, _selectedUser.Id);
            NotesOfSelectedUser.Remove(NotesOfSelectedUser.Where(n => n.Id == noteId).Single());
            UserNotes = new ObservableCollection<Note>(getNotes(_user.Id));
        }

        private IEnumerable<User> GetUsers()
        {
            return _client.GetUsers();
        }

        private IEnumerable<User> GetUsersBySharedNote(Guid noteId)
        {
            return _client.GetUsersBySharedNote(noteId);
        }

        public void PickSharedUsers(Note note)
        {
            var users = GetUsers();
            var selectedUsers = GetUsersBySharedNote(note.Id);
            var ansList = new List<GeneralType>();
            foreach (var user in users.Where(u => u.Id != _user.Id && u.Id != note.Owner.Id))
            {
                if (selectedUsers.Where(u => u.Id == user.Id).Count() == 1)
                {
                    ansList.Add(new GeneralType { Id = user.Id, Title = user.Name, IsSelected = true });
                }
                else
                {
                    ansList.Add(new GeneralType { Id = user.Id, Title = user.Name, IsSelected = false });
                }
            }
            PickItemsDialogWindow pickWindow = new PickItemsDialogWindow(ansList);
            pickWindow.Closed += (object o, EventArgs arg) =>
            {
                var tmpShare = new List<Guid>();
                var tmpDenyShare = new List<Guid>();
                if (pickWindow.SelectedItems != null)
                {
                    foreach (var user in pickWindow.SelectedItems)
                    {
                        if (selectedUsers.Where(u => u.Id == user.Id).Count() == 0)
                        {
                            tmpShare.Add(user.Id);
                        }
                    }

                    foreach (var user in selectedUsers)
                    {
                        if (pickWindow.SelectedItems.Where(u => u.Id == user.Id).Count() == 0)
                        {
                            tmpDenyShare.Add(user.Id);
                        }
                    }
                }
                ShareNote(tmpShare, note.Id);
                DenyShareNote(tmpDenyShare, note.Id);
                foreach (var userNote in UserNotes)
                {
                    if (userNote.Id == note.Id)
                    {
                        userNote.Shared = _client.GetUsersBySharedNote(note.Id);
                        break;
                    }
                }
                SelectedNote = _client.GetNote(SelectedNote.Id);
            };
            pickWindow.ShowDialog();
        }

        public void PickCategoriesToAddNote(Note note)
        {
            var selectedCategories = GetCategoriesByNote(note.Id);
            var ansList = new List<GeneralType>();
            foreach (var category in _userCategories.Where(u => u.Id != _user.Id))
            {
                if (selectedCategories.Where(u => u.Id == category.Id).Count() == 1)
                {
                    ansList.Add(new GeneralType { Id = category.Id, Title = category.Title, IsSelected = true });
                }
                else
                {
                    ansList.Add(new GeneralType { Id = category.Id, Title = category.Title, IsSelected = false });
                }
            }
            PickItemsDialogWindow pickWindow = new PickItemsDialogWindow(ansList);
            pickWindow.Closed += (object o, EventArgs arg) =>
            {
                var tmpAddCatgory = new List<Guid>();
                var tmpDeleteCategory = new List<Guid>();
                if (pickWindow.SelectedItems != null)
                {
                    foreach (var category in pickWindow.SelectedItems)
                    {
                        if (selectedCategories.Where(c => c.Id == category.Id).Count() == 0)
                        {
                            tmpAddCatgory.Add(category.Id);
                        }
                    }

                    foreach (var category in selectedCategories)
                    {
                        if (pickWindow.SelectedItems.Where(c => c.Id == category.Id).Count() == 0)
                        {
                            tmpDeleteCategory.Add(category.Id);
                        }
                    }
                }
                AddCategoriesToNote(tmpAddCatgory, note.Id);
                DeleteCategoriesFromNote(tmpDeleteCategory, note.Id);
                foreach (var userNote in UserNotes)
                {
                    if (userNote.Id == note.Id)
                    {
                        userNote.Categories = _client.GetCategoriesByNote(note.Id);
                        break;
                    }
                }
                SelectedNote = _client.GetNote(SelectedNote.Id);
            };
            pickWindow.ShowDialog();
        }

        public void PickNotesToAddInCategory(Category category)
        {
            var ansList = new List<GeneralType>();
            foreach (var note in UserNotes)
            {
                if (NotesOfSelectedCategory.Where(n => n.Id == note.Id).Count() == 1)
                {
                    ansList.Add(new GeneralType { Id = note.Id, Title = note.Title, IsSelected = true });
                }
                else
                {
                    ansList.Add(new GeneralType { Id = note.Id, Title = note.Title, IsSelected = false });
                }
            }
            PickItemsDialogWindow pickWindow = new PickItemsDialogWindow(ansList);
            pickWindow.Closed += (object o, EventArgs arg) =>
            {
                var tmpAddNotesInCategory = new List<Guid>();
                var tmpDeleteNotesFromCategory = new List<Guid>();
                if (pickWindow.SelectedItems != null)
                {
                    foreach (var note in pickWindow.SelectedItems)
                    {
                        if (NotesOfSelectedCategory.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpAddNotesInCategory.Add(note.Id);
                        }
                    }

                    foreach (var note in NotesOfSelectedCategory)
                    {
                        if (pickWindow.SelectedItems.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpDeleteNotesFromCategory.Add(note.Id);
                        }
                    }
                }
                AddNotesInCategory(tmpAddNotesInCategory, category.Id);
                DeleteNotesFromCategory(tmpDeleteNotesFromCategory, category.Id);
                NotesOfSelectedCategory = new ObservableCollection<Note>(GetNotesByCategory(category.Id));
            };
            pickWindow.ShowDialog();
        }

        public void PickNotesToShareUser(User user)
        {
            var ansList = new List<GeneralType>();
            foreach (var note in UserNotes)
            {
                if (note.Owner.Id != user.Id)
                {
                    if (NotesOfSelectedUser.Where(n => n.Id == note.Id).Count() == 1)
                    {
                        ansList.Add(new GeneralType { Id = note.Id, Title = note.Title, IsSelected = true });
                    }
                    else
                    {
                        ansList.Add(new GeneralType { Id = note.Id, Title = note.Title, IsSelected = false });
                    }
                }
            }
            PickItemsDialogWindow pickWindow = new PickItemsDialogWindow(ansList);
            pickWindow.Closed += (object o, EventArgs arg) =>
            {
                var tmpShareNoteToUser = new List<Guid>();
                var tmpDenyShareNoteToUser = new List<Guid>();
                if (pickWindow.SelectedItems != null)
                {
                    foreach (var note in pickWindow.SelectedItems)
                    {
                        if (NotesOfSelectedUser.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpShareNoteToUser.Add(note.Id);
                        }
                    }

                    foreach (var note in NotesOfSelectedUser)
                    {
                        if (pickWindow.SelectedItems.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpDenyShareNoteToUser.Add(note.Id);
                        }
                    }
                }
                ShareNotesToUser(tmpShareNoteToUser, user.Id);
                DenyShareNotesToUser(tmpDenyShareNoteToUser, user.Id);

                NotesOfSelectedUser = new ObservableCollection<Note>(GetSharedNotesByTwoUser(user.Id));
                UserNotes = new ObservableCollection<Note>(getNotes(_user.Id));
            };
            pickWindow.ShowDialog();
        }

        private IEnumerable<Note> GetSharedNotesByTwoUser(Guid userId)
        {
            return _client.GetNotesBySharedUser(_user.Id, userId).Concat(_client.GetNotesBySharedUser(userId, _user.Id));
        }

        public void EditNote(Note note)
        {
            var noteWin = new NoteWindow(note);
            noteWin.Closed += (object o, EventArgs args) =>
              {
                  SelectedNote = _client.GetNote(SelectedNote.Id);
                  if (noteWin.IsDeleted)
                  {
                      UserNotes.Remove(UserNotes.Where(n => n.Id == note.Id).Single());
                  }
              };
            noteWin.ShowDialog();
        }

        public void EditUser()
        {
            var editUserWindow = new EditUserWindow(ref _user);
            editUserWindow.Closed += (object o, EventArgs arg) =>
             {
                 OnPropertyChanged("User");
             };
            editUserWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
