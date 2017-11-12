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
    public class ModelEditUserWindow : INotifyPropertyChanged
    {
        private readonly ServiceClient _client;
        private User _user;
        public User EditableUser
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("EditableUser");
            }
        }

        public ModelEditUserWindow(ref User user)
        {
            _user = user;
            _client = new ServiceClient("http://localhost:52395/api/");
        }

        public bool SaveEdit(String newName, String newPassword)
        {
            bool IsNameExist = false;
            if (newName != _user.Name && newName!=string.Empty)
            {
                if (_client.IsExistUserByName(newName))
                {
                    IsNameExist = true;
                }
                _user.Name = newName;
            }
            if (newPassword != string.Empty && newPassword != _user.Password)
            {
                _user.Password = newPassword;
            }
            if (!IsNameExist)
            {
                _client.UpdateUser(_user);
                return true;
            }
            return false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
