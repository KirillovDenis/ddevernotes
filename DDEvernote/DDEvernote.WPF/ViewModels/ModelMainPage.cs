using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DDEvernote.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DDEvernote.WPF.Views.Windows;
using DDEvernote.WPF.Views.Pages;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelMainPage : INotifyPropertyChanged
    {
        private ServiceClient _client;
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

        #region SelectedItemsOfLists

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

        #endregion

        #region ListsSourcesOfViews

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

        #endregion

        #region Commands

        private Command createNoteCommand;
        public Command CreateNoteCommand
        {
            get { return createNoteCommand; }
            set
            {
                createNoteCommand = value;
                OnPropertyChanged("CreateNoteCommand");
            }
        }
        private Command createCategoryCommand;
        public Command CreateCategoryCommand
        {
            get { return createCategoryCommand; }
            set
            {
                createCategoryCommand = value;
                OnPropertyChanged("CreateCategoryCommand");
            }
        }
        private Command editUserCommand;
        public Command EditUserCommand
        {
            get { return editUserCommand; }
            set
            {
                editUserCommand = value;
                OnPropertyChanged("EditUserCommand");
            }
        }
        private Command logOutCommand;
        public Command LogOutCommand
        {
            get { return logOutCommand; }
            set
            {
                logOutCommand = value;
                OnPropertyChanged("LogOutCommand");
            }
        }
        private Command selectNoteCommand;
        public Command SelectNoteCommand
        {
            get { return selectNoteCommand; }
            set
            {
                selectNoteCommand = value;
                OnPropertyChanged("SelectNoteCommand");
            }
        }
        private Command editNoteCommand;
        public Command EditNoteCommand
        {
            get { return editNoteCommand; }
            set
            {
                editNoteCommand = value;
                OnPropertyChanged("EditNoteCommand");
            }
        }
        private Command shareNoteCommand;
        public Command ShareNoteCommand
        {
            get { return shareNoteCommand; }
            set
            {
                shareNoteCommand = value;
                OnPropertyChanged("ShareNoteCommand");
            }
        }
        private Command selectCategoryCommand;
        public Command SelectCategoryCommand
        {
            get { return selectCategoryCommand; }
            set
            {
                selectCategoryCommand = value;
                OnPropertyChanged("SelectCategoryCommand");
            }
        }
        private Command selectUserCommand;
        public Command SelectUserCommand
        {
            get { return selectUserCommand; }
            set
            {
                selectUserCommand = value;
                OnPropertyChanged("SelectUserCommand");
            }
        }
        private Command editCategoryCommand;
        public Command EditCategoryCommand
        {
            get { return editCategoryCommand; }
            set
            {
                editCategoryCommand = value;
                OnPropertyChanged("EditCategoryCommand");
            }
        }
        private Command updNotesCommand;
        public Command UpdNotesCommand
        {
            get { return updNotesCommand; }
            set
            {
                updNotesCommand = value;
                OnPropertyChanged("UpdNotesCommand");
            }
        }
        private Command addCategoriesToNoteCommand;
        public Command AddCategoriesToNoteCommand
        {
            get { return addCategoriesToNoteCommand; }
            set
            {
                addCategoriesToNoteCommand = value;
                OnPropertyChanged("AddCategoryiesToNoteCommand");
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
        private Command addNotesToCategoryCommand;
        public Command AddNotesToCategoryCommand
        {
            get { return addNotesToCategoryCommand; }
            set
            {
                addNotesToCategoryCommand = value;
                OnPropertyChanged("AddNotesToCategoryCommand");
            }
        }
        private Command deleteCategoryCommand;
        public Command DeleteCategoryCommand
        {
            get { return deleteCategoryCommand; }
            set
            {
                deleteCategoryCommand = value;
                OnPropertyChanged("DeleteCategoryNote");
            }
        }
        private Command addSharedNotesToUserCommand;
        public Command AddSharedNotesToUserCommand
        {
            get { return addSharedNotesToUserCommand; }
            set
            {
                addSharedNotesToUserCommand = value;
                OnPropertyChanged("AddSharedNotesToUserCommand");
            }
        }
        private Command denySharedNotesToUserCommand;
        public Command DenySharedNotesToUserCommand
        {
            get { return denySharedNotesToUserCommand; }
            set
            {
                denySharedNotesToUserCommand = value;
                OnPropertyChanged("DenySharedNotesToUserCommand");
            }
        }

        #endregion

        public ModelMainPage(User user)
        {
            this.User = user;
            _client = new ServiceClient("http://localhost:52395/api/");
            UserNotes = new ObservableCollection<Note>(getNotes(user.Id));
            UserCategories = new ObservableCollection<Category>(user.Categories);
            Users = new ObservableCollection<User>(GetUsers().Where(u => u.Id != _user.Id));
            NotesOfSelectedCategory = new ObservableCollection<Note>();
            NotesOfSelectedUser = new ObservableCollection<Note>();
            VisibilityPropertyOfNote = Visibility.Hidden;

            CreateNoteCommand = new Command(() => { CreateNote(); });
            CreateCategoryCommand = new Command(() => { CreateCategory(); });
            EditUserCommand = new Command((page) => { EditUser((MainPage)page); });
            LogOutCommand = new Command((page) => { LogOut((MainPage)page); });
            SelectNoteCommand = new Command((note) => { SelectNote((Note)note); });
            EditNoteCommand = new Command((note) => { EditNote((Note)note); });
            ShareNoteCommand = new Command(() => { PickSharedUsers(SelectedNote); });
            SelectCategoryCommand = new Command((category) => { SelectCategory((Category)category); });
            SelectUserCommand = new Command((u) => { SelectUser((User)u); });
            UpdNotesCommand = new Command(() => { LoadNotes(); });
            AddCategoriesToNoteCommand = new Command(() => { PickCategoriesToAddNote(SelectedNote); });
            DeleteNoteCommand = new Command(() => { DeleteNote(SelectedNote.Id); });
            DeleteCategoryCommand = new Command(() => { DeleteCategory(_selectedCategory); });
            AddNotesToCategoryCommand = new Command(() => { PickNotesToAddInCategory(_selectedCategory); });
            AddSharedNotesToUserCommand = new Command(() => { PickNotesToShareUser(SelectedUser); });
            DenySharedNotesToUserCommand = new Command(() =>
            {
                if (SelectedNote.Owner.Id == User.Id)
                { DenyShareNote(SelectedNote.Id); }
            });
            EditCategoryCommand = new Command((category) => { EditCategory((Category)category); });
        }

        private void CreateNote()
        {
            var createNoteWindow = new NoteWindow(new Note { Owner = User });
            createNoteWindow.Closed += (object o, EventArgs args) =>
            {
                LoadNotes();
            };
            createNoteWindow.ShowDialog();
        }
        private void LoadNotes()
        {
            UserNotes = new ObservableCollection<Note>(getNotes(User.Id));
        }
        private void CreateCategory()
        {
            var categoryWin = new CreateCategoryWindow(_user, new Category { Id = new Guid(), Title = string.Empty });
            categoryWin.Closed += (object o, EventArgs arg) =>
              {
                  User.Categories = new List<Category>(_client.GetCategoriesByUser(_user.Id));
                  UserCategories = new ObservableCollection<Category>(_user.Categories);
              };
            categoryWin.ShowDialog();
        }
        private void EditUser(MainPage page)
        {
            var editUserWindow = new EditUserWindow(ref _user);
            editUserWindow.Closed += (object o, EventArgs arg) =>
             {
                 OnPropertyChanged("User");
                 if (((ModelEditUserWindow)editUserWindow.DataContext).IsDeleted)
                 {
                     LogOut(page);
                 }
             };
            editUserWindow.ShowDialog();
        }
        private void LogOut(MainPage page)
        {
            ((MainWindow)page.Parent).Content = new LoginPage();
        }
        private void SelectNote(Note note)
        {
            if (note != null)
            {
                SelectedNote = note;
                SelectedNote.Categories = new ObservableCollection<Category>(GetCategoriesByNote(note.Id));
                if (VisibilityPropertyOfNote == Visibility.Hidden)
                {
                    VisibilityPropertyOfNote = Visibility.Visible;
                }
            }
        }
        private void EditNote(Note note)
        {
            if (note != null)
            {
                var noteWin = new NoteWindow(note);
                noteWin.Closed += (object o, EventArgs args) =>
                  {
                      SelectedNote = _client.GetNote(SelectedNote.Id);
                      if (((ModelNoteWindow)noteWin.DataContext).IsDeleted)
                      {
                          UserNotes.Remove(UserNotes.Where(n => n.Id == note.Id).Single());
                      }
                  };
                noteWin.ShowDialog();
            }
        }
        private void SelectCategory(Category category)
        {
            if (category != null)
            {
                _selectedCategory = category;
                NotesOfSelectedCategory = new ObservableCollection<Note>(GetNotesByCategory(category.Id));
            }
        }
        private void SelectUser(User user)
        {
            _selectedUser = user;
            NotesOfSelectedUser = new ObservableCollection<Note>(GetSharedNotesByTwoUser(user.Id));
        }
        private void DeleteNote(Guid noteId)
        {
            _client.DeleteNote(noteId);
            UserNotes.Remove(UserNotes.Where(n => n.Id == noteId).Single());
            if (NotesOfSelectedCategory.Count > 0 && NotesOfSelectedCategory.Contains(NotesOfSelectedCategory.Where(n => n.Id == noteId).Single()))
            {
                NotesOfSelectedCategory.Remove(NotesOfSelectedCategory.Where(n => n.Id == noteId).Single());
            }
            if (NotesOfSelectedUser.Count > 0 && NotesOfSelectedUser.Contains(NotesOfSelectedUser.Where(n => n.Id == noteId).Single()))
            {
                NotesOfSelectedUser.Remove(NotesOfSelectedUser.Where(n => n.Id == noteId).Single());
            }
        }
        private void DeleteCategory(Category category)
        {
            _client.DeleteCategory(category.Id);
            UserCategories.Remove(category);
        }
        private void EditCategory(Category category)
        {
            var categoryWin = new CreateCategoryWindow(_user, category);
            categoryWin.Closed += (object o, EventArgs arg) =>
            {
                User.Categories = new List<Category>(_client.GetCategoriesByUser(_user.Id));
                UserCategories = new ObservableCollection<Category>(_user.Categories);
            };
            categoryWin.ShowDialog();
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
        private void DenyShareNote(Guid noteId)
        {
            _client.DenyShareNote(noteId, _selectedUser.Id);
            NotesOfSelectedUser.Remove(NotesOfSelectedUser.Where(n => n.Id == noteId).Single());
            UserNotes = new ObservableCollection<Note>(getNotes(_user.Id));
        }

        private void PickCategoriesToAddNote(Note note)
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
                if (((ModelPickItems)pickWindow.DataContext).SelectedItems != null)
                {
                    foreach (var category in ((ModelPickItems)pickWindow.DataContext).SelectedItems)
                    {
                        if (selectedCategories.Where(c => c.Id == category.Id).Count() == 0)
                        {
                            tmpAddCatgory.Add(category.Id);
                        }
                    }

                    foreach (var category in selectedCategories)
                    {
                        if (((ModelPickItems)pickWindow.DataContext).SelectedItems.Where(c => c.Id == category.Id).Count() == 0)
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
        private void PickNotesToAddInCategory(Category category)
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
                if (((ModelPickItems)pickWindow.DataContext).SelectedItems != null)
                {
                    foreach (var note in ((ModelPickItems)pickWindow.DataContext).SelectedItems)
                    {
                        if (NotesOfSelectedCategory.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpAddNotesInCategory.Add(note.Id);
                        }
                    }

                    foreach (var note in NotesOfSelectedCategory)
                    {
                        if (((ModelPickItems)pickWindow.DataContext).SelectedItems.Where(n => n.Id == note.Id).Count() == 0)
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
        private void PickNotesToShareUser(User user)
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
                if (((ModelPickItems)pickWindow.DataContext).SelectedItems != null)
                {
                    foreach (var note in ((ModelPickItems)pickWindow.DataContext).SelectedItems)
                    {
                        if (NotesOfSelectedUser.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpShareNoteToUser.Add(note.Id);
                        }
                    }

                    foreach (var note in NotesOfSelectedUser)
                    {
                        if (((ModelPickItems)pickWindow.DataContext).SelectedItems.Where(n => n.Id == note.Id).Count() == 0)
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
        private void PickSharedUsers(Note note)
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
                if (((ModelPickItems)pickWindow.DataContext).SelectedItems != null)
                {
                    foreach (var user in ((ModelPickItems)pickWindow.DataContext).SelectedItems)
                    {
                        if (selectedUsers.Where(u => u.Id == user.Id).Count() == 0)
                        {
                            tmpShare.Add(user.Id);
                        }
                    }

                    foreach (var user in selectedUsers)
                    {
                        if (((ModelPickItems)pickWindow.DataContext).SelectedItems.Where(u => u.Id == user.Id).Count() == 0)
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

        private IEnumerable<Note> getNotes(Guid userId)
        {
            return _client.GetNotes(userId).Concat(_client.GetSharedNotesByUser(userId));
        }
        private IEnumerable<Note> GetNotesByCategory(Guid categoryId)
        {
            return _client.GetNotesByCategory(categoryId);
        }
        private IEnumerable<Category> GetCategoriesByNote(Guid noteId)
        {
            return _client.GetCategoriesByNote(noteId);
        }
        private IEnumerable<User> GetUsers()
        {
            return _client.GetUsers();
        }
        private IEnumerable<User> GetUsersBySharedNote(Guid noteId)
        {
            return _client.GetUsersBySharedNote(noteId);
        }
        private IEnumerable<Note> GetSharedNotesByTwoUser(Guid userId)
        {
            return _client.GetNotesBySharedUser(_user.Id, userId).Concat(_client.GetNotesBySharedUser(userId, _user.Id));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
