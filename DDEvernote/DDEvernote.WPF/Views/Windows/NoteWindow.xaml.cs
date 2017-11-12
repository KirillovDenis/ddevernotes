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
using DDEvernote.WPF.ViewModels;

namespace DDEvernote.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        public bool IsDeleted { get; set; }
        public NoteWindow(Note note)
        {
            InitializeComponent();
            DataContext = new ModelNoteWindow(note);
            IsDeleted = false;
        }

        private void saveNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            ((ModelNoteWindow)this.DataContext).saveNote();
            this.Close();
        }

        private void deleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            ((ModelNoteWindow)this.DataContext).deleteNote();
            IsDeleted = true;
            this.Close();
        }
    }
}
