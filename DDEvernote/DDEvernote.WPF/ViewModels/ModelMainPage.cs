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
using System.Resources;
using System.Reflection;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNet.SignalR.Client;
using System.Threading;
using System.Net.Http;
using Microsoft.AspNet.SignalR.Json;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelMainPage : INotifyPropertyChanged
    {

        private IHubProxy HubProxy { get; set; }
        private HubConnection Connection { get; set; }
        private ClientWebSocket _socket;
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
        private Command unloadPageCommand;
        public Command UnloadPageCommand
        {
            get { return unloadPageCommand; }
            set
            {
                unloadPageCommand = value;
                OnPropertyChanged("UnloadPageCommand");
            }
        }

        #endregion

        public ModelMainPage(User user)
        {
            this.User = user;
            ResourceManager rm = new ResourceManager("DDEvernote.WPF.ConnectionResource",
                            Assembly.GetExecutingAssembly());
            string connString = rm.GetString("ConnectionString");
            _socket = new ClientWebSocket();
            ConnectToHubAsync();
            _client = new ServiceClient(connString);
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
            UnloadPageCommand = new Command(() => { Connection_Closed(); });
        }

        private async void ConnectToHubAsync()
        {
            ResourceManager rm = new ResourceManager("DDEvernote.WPF.ConnectionResource",
                            Assembly.GetExecutingAssembly());
            string ServerURI = rm.GetString("ServerURI");
            Connection = new HubConnection(ServerURI);
            Connection.Closed += Connection_Closed;
            HubProxy = Connection.CreateHubProxy("ServerHub");
            HubProxy.On<Guid>("UpdateNotes", (noteId) => { OnUpdateNotes(noteId); });
            HubProxy.On<Guid>("DeleteNotes", (noteId) => { OnDeleteNotes(noteId); });
            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {

            }
            await HubProxy.Invoke("Connect", User.Id);
        }

        private void OnDeleteNotes(Guid noteId)
        {
            var dispatcher = Application.Current.Dispatcher;

            var note = UserNotes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                dispatcher.Invoke(() =>
                {
                    UserNotes.Remove(note);
                    if (SelectedNote != null && SelectedNote.Id == noteId)
                    {
                        VisibilityPropertyOfNote = Visibility.Hidden;
                    }
                });
            }
        }
        private void OnUpdateNotes(Guid noteId)
        {
            var dispatcher = Application.Current.Dispatcher;
            var UpdatedNote = GetNote(noteId);

            var note = UserNotes.FirstOrDefault(n => n.Id == noteId);
            List<Note> tmpList;
            if (note != null)
            {
                note.Title = UpdatedNote.Title;
                note.Text = UpdatedNote.Text;
                note.Changed = UpdatedNote.Changed;
                note.Categories = UpdatedNote.Categories;
                note.Shared = UpdatedNote.Shared;
                tmpList = UserNotes.ToList();
                UserNotes = new ObservableCollection<Note>(tmpList);
                if (SelectedNote != null && SelectedNote.Id == note.Id)
                {
                    SelectedNote = note;
                }
                OnPropertyChanged("SelectedNote");
            }
            else
            {
                dispatcher.Invoke(() => UserNotes.Add(UpdatedNote));
            }
            OnPropertyChanged("UserNotes");
        }
        private void SendUpdate(Note note)
        {
            var noteToSend = new Note { Id = note.Id, Owner = new User { Id = note.Owner.Id } };
            var listNoteToSendShared = new List<User>();
            foreach (var user in note.Shared)
            {
                listNoteToSendShared.Add(new User { Id = user.Id });
            }
            noteToSend.Shared = listNoteToSendShared;
            bool sendToOwner = this._user.Id != note.Owner.Id;
            if (Connection.State != ConnectionState.Connected)
            {
                Connection.Start();
            }
            HubProxy.Invoke("SendUpdate", noteToSend, sendToOwner);
        }
        private void SendDelete(Note note)
        {
            var noteToSend = new Note { Id = note.Id, Owner = new User { Id = note.Owner.Id } };
            var listNoteToSendShared = new List<User>();
            foreach (var user in note.Shared)
            {
                listNoteToSendShared.Add(new User { Id = user.Id });
            }
            noteToSend.Shared = listNoteToSendShared;
            bool sendToOwner = this._user.Id != note.Owner.Id;
            if (Connection.State != ConnectionState.Connected)
            {
                Connection.Start();
            }
            HubProxy.Invoke("SendDelete", noteToSend, sendToOwner);
        }
        private void SendUpdateIdNote(Guid noteId)
        {
            var note = UserNotes.Where(n => n.Id == noteId).Single();
            SendUpdate(note);
        }
        private void SendDeleteIdNote(Guid noteId)
        {
            var note = UserNotes.Where(n => n.Id == noteId).Single();
            SendDelete(note);
        }
        private void Connection_Closed()
        {
            if (Connection != null)
            {
                Connection.Stop();
                Connection.Dispose();
            }
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
            Connection_Closed();
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
                          SendDelete(note);
                      }
                      else
                      {
                          SendUpdate(note);
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
            var tmpNote = UserNotes.Where(n => n.Id == noteId).FirstOrDefault();
            UserNotes.Remove(UserNotes.Where(n => n.Id == noteId).Single());
            var noteToSend = new Note { Id = noteId, Owner = new User { Id = tmpNote.Owner.Id } };
            var listNoteToSendShared = new List<User>();
            foreach (var user in tmpNote.Shared)
            {
                listNoteToSendShared.Add(new User { Id = user.Id });
            }
            noteToSend.Shared = listNoteToSendShared;
            bool sendToOwner = false;
            if (tmpNote != null)
            {
                sendToOwner = this._user.Id != tmpNote.Owner.Id;
            }
            HubProxy.Invoke("SendDelete", noteToSend, sendToOwner);
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
            foreach (var note in UserNotes)
            {
                if (note.Categories.Where(c => c.Id == category.Id).Count() == 1)
                {
                    SendUpdate(note);
                }
            }
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

            var tmpListUsersGuid = new List<Guid>();
            tmpListUsersGuid.Add(_selectedUser.Id);
            HubProxy.Invoke("RemoveNote", tmpListUsersGuid, new Note { Id = noteId }, false);
        }

        private void PickCategoriesToAddNote(Note note)
        {
            var selectedCategories = GetCategoriesByNote(note.Id);
            var ansList = new List<GeneralType>();
            foreach (var category in selectedCategories)
            {
                ansList.Add(new GeneralType { Id = category.Id, Title = category.Title, IsSelected = true });
            }
            foreach (var category in _userCategories)
            {
                if (selectedCategories.Where(c => c.Id == category.Id).Count() == 0)
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
                SendUpdate(note);
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
                            SendUpdateIdNote(note.Id);
                        }
                    }

                    foreach (var note in NotesOfSelectedCategory)
                    {
                        if (((ModelPickItems)pickWindow.DataContext).SelectedItems.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpDeleteNotesFromCategory.Add(note.Id);
                            SendUpdateIdNote(note.Id);
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
                var selectedItems = ((ModelPickItems)pickWindow.DataContext).SelectedItems;
                if (selectedItems != null)
                {
                    foreach (var note in selectedItems)
                    {
                        if (NotesOfSelectedUser.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpShareNoteToUser.Add(note.Id);
                            //SendUpdateIdNote(note.Id);
                        }
                    }

                    foreach (var note in NotesOfSelectedUser)
                    {
                        if (selectedItems.Where(n => n.Id == note.Id).Count() == 0)
                        {
                            tmpDenyShareNoteToUser.Add(note.Id);
                            //SendDeleteIdNote(note.Id);
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

                bool sendToOwner = this._user.Id != note.Owner.Id;
                var tmpNote = new Note { Id = note.Id, Owner = new User { Id = note.Owner.Id } };
                HubProxy.Invoke("AddNote", tmpShare, tmpNote, sendToOwner);
                HubProxy.Invoke("RemoveNote", tmpDenyShare, tmpNote, sendToOwner);

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

        private Note GetNote(Guid noteId)
        {
            return _client.GetNote(noteId);
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
