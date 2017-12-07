using DDEvernote.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Linq;
using System.Resources;
using System.Reflection;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelCategoryWindow : INotifyPropertyChanged
    {
        private readonly ServiceClient _client;
        private readonly User _user;
        public Category Category { get; set; }
        public bool IsDeleted { get; set; }

        #region Commands

        private Command saveCategoryCommand;
        public Command SaveCategoryCommand
        {
            get { return saveCategoryCommand; }
            set
            {
                saveCategoryCommand = value;
                OnPropertyChanged("SaveCategoryCommand");
            }
        }
        private Command deleteCategoryCommand;
        public Command DeleteCategoryCommand
        {
            get { return deleteCategoryCommand; }
            set
            {
                deleteCategoryCommand = value;
                OnPropertyChanged("DeleteCategoryCommand");
            }
        }

        #endregion

        public ModelCategoryWindow(User user, Category category)
        {
            ResourceManager rm = new ResourceManager("DDEvernote.WPF.ConnectionResource",
                            Assembly.GetExecutingAssembly());
            string connectionString = rm.GetString("ConnectionString");
            _client = new ServiceClient(connectionString);
            this.Category = category;
            _user = user;
            IsDeleted = false;

            SaveCategoryCommand = new Command((window) => { SaveCategory((Window)window); });
            DeleteCategoryCommand = new Command((window) => { DeleteCategory((Window)window); });
        }

        private void SaveCategory(Window window)
        {
            if (this.Category.Title == string.Empty || this.Category.Title == null)
            {
                MessageBox.Show("Категория должна иметь название", "Категория", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_user.Categories.Where(c => c.Title == this.Category.Title).Count() == 0 && this.Category.Id == Guid.Empty)
            {
                Category = _client.CreateCategory(_user.Id, this.Category.Title);
            }
            else if(this.Category.Id != Guid.Empty)
            {
                Category = _client.UpdateCategory(this.Category);
            }
            else
            {
                MessageBox.Show("Такая категория уже существует", "Категория", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            window.Close();
        }
        private void DeleteCategory(Window window)
        {
            if (this.Category.Id != Guid.Empty)
            {
                _client.DeleteCategory(this.Category.Id);
                IsDeleted = true;
            }
            window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
