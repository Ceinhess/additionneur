﻿using Additionneur.UserControls.Logging;
using Additionneur.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Additionneur.Views
{
    /// <summary>
    /// Logique d'interaction pour LoggingPage.xaml
    /// </summary>
    public partial class LoggingPage : Page
    {

        public LoggingPage()
        {
            DataContext = new LoggingPageVM();

            InitializeComponent();

        }

        private void LoginMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
