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

namespace oprot.plot.wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RenameGroupWindow : Window
    {
        private string _name;
        public RenameGroupWindow(string name)
        {
            InitializeComponent();
            _name = txtName.Text = name;
        }

        public string Go()
        {
            if (this.ShowDialog() ?? false)
            {
                return txtName.Text;

            }
            else
            {
                return _name;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
