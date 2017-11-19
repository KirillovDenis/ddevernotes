using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DDEvernote.Model;
using System.Windows.Input;
using System.Windows.Controls;
using DDEvernote.WPF.Views.Windows;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelLoginPage : INotifyPropertyChanged
    {
        private ServiceClient _client;
        public string UserName { get; set; }

        private Command _logInCommand;
        public Command LogInCommand
        {
            get { return _logInCommand; }
        }
        private Command _signUpCommand;
        public Command SignUpCommand
        {
            get { return _signUpCommand; }
        }

        public ModelLoginPage()
        {
            _client = new ServiceClient("http://localhost:52395/api/");
            _logInCommand = new Command(passwordBox => { LogIn((PasswordBox)passwordBox); });
            _signUpCommand = new Command(passwordBox => { SignUp((PasswordBox)passwordBox); });
        }
        public void LogIn(PasswordBox passwordBox)
        {
            var existedUser = _client.GetUserByName(UserName);
            if (passwordBox.Password == existedUser.Password)
            {
                ((MainWindow)((Page)((Grid)((StackPanel)passwordBox.Parent).Parent).Parent).Parent).Content = new MainPage(existedUser);
            }
            else
            {
                MessageBox.Show("Неверное имя или пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUp(PasswordBox passwordBox)
        {
            if (!_client.IsExistUserByName(UserName))
            {
                var createdUser = _client.CreateUser(new User { Name = UserName, Password = passwordBox.Password });
                ((MainWindow)((Page)((Grid)((StackPanel)passwordBox.Parent).Parent).Parent).Parent).Content = new MainPage(createdUser);
            }
            else
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
