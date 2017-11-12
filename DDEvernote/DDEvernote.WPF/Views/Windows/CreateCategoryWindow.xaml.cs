using DDEvernote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DDEvernote.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for CreateCategoryWindow.xaml
    /// </summary>
    public partial class CreateCategoryWindow : Window
    {
        private ServiceClient _client;
        private readonly User _user;
        public CreateCategoryWindow(User user)
        {
            InitializeComponent();
            _client = new ServiceClient("http://localhost:52395/api/");
            _user = user;
        }

        private void createCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Categories.Where(c => c.Title == categoryName.Text).Count() == 0 && categoryName.Text != string.Empty)
            {
                var createdCategory = _client.CreateCategory(_user.Id, categoryName.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("Такая категория уже существует", "Ошибка создания категории", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
