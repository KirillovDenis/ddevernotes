using DDEvernote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DDEvernote.WPF.Views.Pages;
using DDEvernote.WPF.Views.Windows;
using DDEvernote.WPF.ViewModels;

namespace DDEvernote.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(User user)
        {
            InitializeComponent();
            this.DataContext = new ModelMainPage(user);
        }
        
        private void editUserBtn_Click(object sender, RoutedEventArgs e)
        {
            ((ModelMainPage)this.DataContext).EditUser();
        }

        private void createNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            Window noteWin = new NoteWindow(new Note { Owner = ((ModelMainPage)this.DataContext).User });
            noteWin.Closed += ((ModelMainPage)this.DataContext).LoadNotes;
            noteWin.ShowDialog();
        }

        private void shareNoteCtxMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var note = (Note)menu.DataContext;
            ((ModelMainPage)this.DataContext).PickSharedUsers(note);
        }

        private void delNoteCtxMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var note = (Note)menu.DataContext;
            ((ModelMainPage)this.DataContext).DeleteNote(note.Id);
        }

        private void createCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            ((ModelMainPage)this.DataContext).CreateCategory();
        }

        private void delCategoryCtxMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var category = (Category)menu.DataContext;
            ((ModelMainPage)this.DataContext).DeleteCategory(category);
        }

        private void addNoteInCategory_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var category = (Category)menu.DataContext;
            ((ModelMainPage)this.DataContext).PickNotesToAddInCategory(category);
        }

        private void addCategoryToNoteCtxMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var note = (Note)menu.DataContext;
            ((ModelMainPage)this.DataContext).PickCategoriesToAddNote(note);
        }

        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Parent).Content = new LoginPage();
        }

        private void noteList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var note = ((ListView)sender).SelectedItem as Note;
            ((ModelMainPage)this.DataContext).EditNote(note);
        }

        private void noteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var note = ((ListView)sender).SelectedItem as Note;
            if (note != null)
            {
                ((ModelMainPage)this.DataContext).SelectNote(note);
            }
        }

        private void categoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var category = ((ListView)sender).SelectedItem as Category;
            if (category != null)
            {
                ((ModelMainPage)this.DataContext).SelectCategory(category);
            }
        }

        private void usersShareNoteCtxMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var user = (User)menu.DataContext;
            ((ModelMainPage)this.DataContext).PickNotesToShareUser(user);
        }

        private void users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var user = ((ListView)sender).SelectedItem as User;
            if (user != null)
            {
                ((ModelMainPage)this.DataContext).SelectUser(user);
            }
        }

        private void denyShare_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var menu = (ContextMenu)item.Parent;
            var note = (Note)menu.DataContext;
            if (note.Owner.Id == ((ModelMainPage)this.DataContext).User.Id)
            {
                ((ModelMainPage)this.DataContext).DenyShareNote(note.Id);
            }
        }
    }
}
