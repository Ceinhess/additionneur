using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
using Additionneur;
using Additionneur.Classes.MySqlCrud;
using System.Security.Cryptography;
using Additionneur.ViewModels;


namespace Additionneur.UserControls.Logging
{
    /// <summary>
    /// Logique d'interaction pour LogIn.xaml
    /// </summary>
    public partial class LogIn : UserControl
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((LoggingPageVM)this.DataContext).LogPassword = ((PasswordBox)sender).Password;
            }
        }

        private void MailField_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(MailField);
        }

        private void MailField_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Keyboard.Focus(MailField);
            }
        }
    }
}
