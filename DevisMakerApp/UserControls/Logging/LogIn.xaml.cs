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

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            MySqlManager manager = new();

            //var rows = manager.GetTable("nvl_table").SelectRows(["id", "col1"], [10, 42]);
            var row = manager.GetTable("users").SelectRow("mail", MailField.Text);

            if(row.Count == 0)
            {
                ErrorLabel.Content = "Utilisateur introuvable";
                return;
            }


            // Converts the password+salt to bytes
            byte[] saltedPassword = Encoding.ASCII.GetBytes(PasswordField.Password + row["password_salt"]);

            byte[] hashedPassword = SHA256.Create().ComputeHash(saltedPassword);

            //Encrypts it and converts it to string
            string password = Convert.ToBase64String(hashedPassword);

            if(password != (string)row["password"])
            {
                ErrorLabel.Content = "Mot de passe incorrect.";
                return;
            }

            ErrorLabel.Content = "Mot de passe correct !";

        }

        private void Clear()
        {
            MailField.Text = "";
            PasswordField.Password = "";
            ErrorLabel.Content = "";
        }

        public void Resize(double w, double h)
        {
            this.Width = w;
            this.Height = h;


        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((LoggingPageVM)this.DataContext).LogPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
