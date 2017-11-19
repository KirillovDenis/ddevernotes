using DDEvernote.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DDEvernote.WPF.Views.Windows;
using System.Windows.Controls;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelEditUserWindow : INotifyPropertyChanged
    {
        private readonly ServiceClient _client;
        private User _user;
        public User User { get; set; }
        public bool IsDeleted { get; set; }

        #region Commands

        private Command saveUserCommand;
        public Command SaveUserCommand
        {
            get { return saveUserCommand; }
            set
            {
                saveUserCommand = value;
                OnPropertyChanged("SaveUserCommand");
            }
        }
        private Command deleteUserCommand;
        public Command DeleteUserCommand
        {
            get { return deleteUserCommand; }
            set
            {
                deleteUserCommand = value;
                OnPropertyChanged("DeleteUserCommand");
            }
        }

        #endregion

        public ModelEditUserWindow(ref User user)
        {
            _client = new ServiceClient("http://localhost:52395/api/");
            _user = user;
            User = new User();
            User.Name = _user.Name;
            IsDeleted = false;

            SaveUserCommand = new Command((passBox) => { SaveEdit((PasswordBox)passBox); });
            DeleteUserCommand = new Command((editUserWindow) => { DeleteUser((EditUserWindow)editUserWindow); });
        }

        private void SaveEdit(PasswordBox passBox)
        {
            string password = passBox.Password;
            bool IsNameExist = false;
            if (User.Name != _user.Name && User.Name != string.Empty)
            {
                if (_client.IsExistUserByName(User.Name))
                {
                    IsNameExist = true;
                }
                _user.Name = User.Name;
            }
            if (password != string.Empty && password != _user.Password)
            {
                _user.Password = password;
            }
            if (!IsNameExist)
            {
                _client.UpdateUser(_user);
                ((EditUserWindow)((StackPanel)passBox.Parent).Parent).Close();
            }
            else
            {
                MessageBox.Show("Такое имя уже занято", "Ошибка редактирования", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteUser(Window window)
        {
            if (MessageBox.Show("Вы действительно хотите удалить пользователя?", 
                                "Подтвержение удаления", 
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Question, 
                                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                _client.DeleteUser(_user.Id);
                IsDeleted = true;
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
