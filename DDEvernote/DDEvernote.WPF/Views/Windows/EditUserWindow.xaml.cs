﻿using System;
using System.Collections.Generic;
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
using DDEvernote.Model;
using DDEvernote.WPF.ViewModels;
using System.Net.Http;

namespace DDEvernote.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        public EditUserWindow(ref User user)
        {
            InitializeComponent();
            this.DataContext = new ModelEditUserWindow(ref user);
        }
    }
}
