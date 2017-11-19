using DDEvernote.Model;
using System.Windows.Controls;
using DDEvernote.WPF.ViewModels;

namespace DDEvernote.WPF
{
    public partial class MainPage : Page
    {
        public MainPage(User user)
        {
            InitializeComponent();
            this.DataContext = new ModelMainPage(user);
        }
    }
}
