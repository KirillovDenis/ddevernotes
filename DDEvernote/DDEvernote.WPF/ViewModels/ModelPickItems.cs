using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DDEvernote.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DDEvernote.WPF.Views.Windows;
using DDEvernote.WPF.Views.Pages;

namespace DDEvernote.WPF.ViewModels
{
    public class ModelPickItems : INotifyPropertyChanged
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

        #region Commands

        private Command saveCommand;
        public Command SaveCommand
        {
            get { return saveCommand; }
            set
            {
                saveCommand = value;
                OnPropertyChanged("SaveCommand");
            }
        }

        #endregion

        public ModelPickItems(IEnumerable<GeneralType> inputList)
        {
            InputList = new ObservableCollection<GeneralType>(inputList);

            SaveCommand = new Command((window) => { Save((Window)window); });
        }

        private void Save(Window window)
        {
            var tmpList = new List<GeneralType>();
            foreach (var item in InputList)
            {
                if (item.IsSelected == true)
                {
                    tmpList.Add(item);
                }
            }
            _selectedItems = tmpList;
            window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
