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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DDEvernote.Model;
using DDEvernote.WPF.Views.Windows;
using DDEvernote.WPF.ViewModels;

namespace DDEvernote.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new ModelLoginPage();
        }


        private void signInBtn_Click(object sender, RoutedEventArgs e)
        {
            User existedUser = ((ModelLoginPage)this.DataContext).LogIn(loginName.Text, loginPassword.Password);
            if (existedUser!=null)
            {
                ((MainWindow)this.Parent).Content = new MainPage(existedUser);
            }
        }

        private void signUpBtn_Click(object sender, RoutedEventArgs e)
        {
            User createdUser = ((ModelLoginPage)this.DataContext).SingUp(loginName.Text, loginPassword.Password);
            if (createdUser != null)
            {
                ((MainWindow)this.Parent).Content = new MainPage(createdUser);
            }
        }
    }
}
