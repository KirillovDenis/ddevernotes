using DDEvernote.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for PickItemsDialogWindow.xaml
    /// </summary>
    public partial class PickItemsDialogWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<GeneralType> _inputList;
        public ObservableCollection<GeneralType> InputList
        {
            get { return _inputList; }
            set
            {
                _inputList = value;
                OnPropertyChanged("InputList");
            }
        }
        private IEnumerable<GeneralType> _selectedItems;
        public IEnumerable<GeneralType> SelectedItems
        {
            get { return _selectedItems; }
        }
        

        public PickItemsDialogWindow(IEnumerable<GeneralType> inList)
        {
            InitializeComponent();
            DataContext = this;
            InputList = new ObservableCollection<GeneralType>(inList);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            var tmpList = new List<GeneralType>();
            foreach (var item in list.SelectedItems)
            {
                tmpList.Add((GeneralType)item);
            }
            _selectedItems = tmpList;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
