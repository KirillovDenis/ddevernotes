using DDEvernote.Model;
using DDEvernote.WPF.ViewModels;
using System.Windows;

namespace DDEvernote.WPF.Views.Windows
{
    public partial class CreateCategoryWindow : Window
    {
        public CreateCategoryWindow(User user, Category category)
        {
            InitializeComponent();
            DataContext = new ModelCategoryWindow(user, category);
        }
    }
}
