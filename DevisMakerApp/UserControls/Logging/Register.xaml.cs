using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
using Additionneur.ViewModels;

namespace Additionneur.UserControls.Logging
{
    /// <summary>
    /// Logique d'interaction pour Register.xaml
    /// </summary>
    public partial class Register : UserControl
    {
        public Register()
        {
            InitializeComponent();
        }

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(this.DataContext != null)
            {
                ((LoggingPageVM)this.DataContext).RegPassword = ((PasswordBox)sender).Password;
            }
        }

        private void RegPasswordVerifyBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((LoggingPageVM)this.DataContext).RegPasswordVerify = ((PasswordBox)sender).Password;
            }
        }

        private void UsernameField_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Keyboard.Focus(UsernameField);
            }
        }
    }
}
