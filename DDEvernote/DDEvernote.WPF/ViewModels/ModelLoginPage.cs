using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DDEvernote.Model;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelLoginPage : INotifyPropertyChanged
    {
        private ServiceClient _client;

        public ModelLoginPage()
        {
            _client = new ServiceClient("http://localhost:52395/api/");
        }

        public User LogIn(string name, string password)
        {
            var existedUser = _client.GetUserByName(name);
            if (password == existedUser.Password)
            {
                return existedUser;
            }
            MessageBox.Show("Неверное имя или пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }

        public User SingUp(string name, string password)
        {
            if (!_client.IsExistUserByName(name))
            {
                var createdUser = _client.CreateUser(new User { Name = name, Password = password });
                return createdUser;
            }
            MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
