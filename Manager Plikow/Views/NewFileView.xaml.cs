﻿using System;
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

namespace Manager_Plikow
{
    /// <summary>
    /// Interaction logic for NewFileView.xaml
    /// </summary>
    public partial class NewFileView : Window
    {
        public NewFileView()
        {
            InitializeComponent();
            
        }
        public string itemNameTB;
        private void newFolderName_TextChanged(object sender, TextChangedEventArgs e)
        {
            itemNameTB = newFolderName.Text;
        }

        private void createFile_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
