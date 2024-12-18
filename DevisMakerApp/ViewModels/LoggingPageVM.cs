﻿using Additionneur.Classes.MySqlCrud;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Additionneur.Models;

namespace Additionneur.ViewModels
{
    internal class LoggingPageVM : BaseViewModel
    {
        private string logMail;
        private string logPassword;

        private string regUsername;
        private string regMail;
        private string regPassword;
        private string regPasswordVerify;

        private bool isLoggingMenuVisible;
        private bool isLoginMenuVisible;
        private bool isRegisterMenuVisible;

        private bool isRegBtnEnabled;

        private string regMessageText;
        private SolidColorBrush regMessageColor;

        private string logMessageText;
        private SolidColorBrush logMessageColor;

        public ICommand OpenLog { get; }
        public ICommand OpenReg { get; }
        public ICommand ReturnToChoice { get; }

        public ICommand SubmitLog { get; }

        public ICommand SubmitReg { get; }

        public string LogMail
        {
            get { return logMail; }
            set
            {
                logMail = value;
                OnPropertyChanged("LogMail");
            }
        }
        public string LogPassword
        {
            get { return logPassword; }
            set
            {
                logPassword = value;
                OnPropertyChanged("LogPassword");
            }
        }

        public string RegUsername
        {
            get { return regUsername; }
            set
            {
                regUsername = value;
                OnPropertyChanged("RegUsername");
            }
        }
        public string RegMail
        {
            get { return regMail; }
            set
            {
                regMail = value;
                OnPropertyChanged("RegMail");
            }
        }
        public string RegPassword
        {
            get { return regPassword; }
            set
            {
                regPassword = value;
                OnPropertyChanged("RegPassword");
            }
        }
        public string RegPasswordVerify
        {
            get { return regPasswordVerify; }
            set
            {
                regPasswordVerify = value;
                OnPropertyChanged("RegPasswordVerify");
            }
        }

        public bool IsRegBtnEnabled
        {
            get { return isRegBtnEnabled; }
            set 
            {
                isRegBtnEnabled = value;
                OnPropertyChanged("IsRegBtnEnabled");
            }
        }

        public string RegMessageText
        {
            get { return regMessageText; }
            set
            {
                regMessageText = value;
                OnPropertyChanged("RegMessageText");
            }
        }
        public SolidColorBrush RegMessageColor
        {
            get { return regMessageColor; }
            set
            {
                regMessageColor = value;
                OnPropertyChanged("RegMessageColor");
            }
        }

        public string LogMessageText
        {
            get { return logMessageText; }
            set
            {
                logMessageText = value;
                OnPropertyChanged("LogMessageText");
            }
        }
        public SolidColorBrush LogMessageColor
        {
            get { return logMessageColor; }
            set
            {
                logMessageColor = value;
                OnPropertyChanged("LogMessageColor");
            }
        }

        public Visibility LoggingMenuVisibility
        {
            get { return isLoggingMenuVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isLoggingMenuVisible = (value == Visibility.Visible);
                OnPropertyChanged("LoggingMenuVisibility");
            }
        }

        public Visibility LoginMenuVisibility
        {
            get { return isLoginMenuVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isLoginMenuVisible = (value == Visibility.Visible);
                OnPropertyChanged("LoginMenuVisibility");
            }
        }

        public Visibility RegisterMenuVisibility
        {
            get { return isRegisterMenuVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isRegisterMenuVisible = (value == Visibility.Visible);
                OnPropertyChanged("RegisterMenuVisibility");
            }
        }


        public LoggingPageVM()
        {
            OpenLog = new RelayCommand(o => openLog("LoginBTN"));
            SubmitLog = new RelayCommand(o => submitLog("SubmitLoginBTN"));

            OpenReg = new RelayCommand(o => openReg("RegisterBTN"));
            SubmitReg = new RelayCommand(o => submitReg("SubmitRegisterBTN"));

            ReturnToChoice = new RelayCommand(o => returnToChoice("BackBTN"));

            LoggingMenuVisibility = Visibility.Visible;
            LoginMenuVisibility = Visibility.Collapsed;
            RegisterMenuVisibility = Visibility.Collapsed;

            

            Clear();
        }

        // ==========[ NAVIGATION
        private void openLog(object sender)
        {
            LoginMenuVisibility = Visibility.Visible;
            LoggingMenuVisibility = Visibility.Collapsed;
        }

        private void openReg(object sender)
        {
            RegisterMenuVisibility = Visibility.Visible;
            LoggingMenuVisibility = Visibility.Collapsed;
        }

        private void returnToChoice(object sender)
        {
            LoginMenuVisibility = Visibility.Collapsed;
            RegisterMenuVisibility = Visibility.Collapsed;
            LoggingMenuVisibility = Visibility.Visible;

            Clear();
        }

        // ==========[ MISC
        private void Clear()
        {
            RegUsername = "";
            RegMail = "";
            RegPassword = "";
            RegPasswordVerify = "";

            LogMail = "";
            LogPassword = "";

            IsRegBtnEnabled = true;

            RegMessageText = "";
            RegMessageColor = Brushes.DarkRed;

            LogMessageText = "";
            LogMessageColor = Brushes.DarkRed;
        }

        // ==========[ FORM HANDLING
        private void submitLog(object sender)
        {
            // Creates MySQL manager and requests a row with the given email to verify if it exists.
            MySqlManager manager = new();

            var row = manager.GetTable("users").SelectRow("mail", LogMail);

            if (row.Count == 0)
            {
                LogMessageText = "Unknow user.";
                return;
            }
            

            // Converts the password+salt to bytes
            byte[] saltedPassword = Encoding.ASCII.GetBytes(LogPassword + row["password_salt"]);

            byte[] hashedPassword = SHA256.Create().ComputeHash(saltedPassword);

            //Encrypts it and converts it to string
            string password = Convert.ToBase64String(hashedPassword);

            // Compares encrypted given password to the one stored on the DB
            if (password != (string)row["password"])
            {   // Stops if incorrect
                LogMessageText = "Incorrect passwords.";
                return;
            }

            // Continues if correct, show "success" text then proceed
            LogMessageText = "Success !";
            LogMessageColor = Brushes.Green;

            SuccessfullyLogIn(row);
        }
        private async void SuccessfullyLogIn(Dictionary<string, object> UserData)
        {
            // Wait 0.4 seconds
            await Task.Delay(400);

            // Clear all fields and hide the menu
            Clear();
            LoginMenuVisibility = Visibility.Collapsed;

            // Updates the current user in the Main View Model
            MainVM.User = new User((int)UserData["user_id"], (string)UserData["username"], (string)UserData["mail"]);

            // Tells the main view model to go to the game view
            MainVM.GoToGameView();
        }


        private void submitReg(object sender)
        {
            // Verifies that the form is correctly filled
            string test = VerifyRegisterForm();
            if (test != "AllGood")
            {

                return;
            }

            MySqlManager manager = new();

            // Creates the salt (24 random characters chosen from the "chars" string)
            const string chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789#@%&";
            var random = new Random();

            string salt = "";
            for (int i = 0; i < random.Next(16, 32); i++)
            {
                salt += chars[random.Next(chars.Length)];
            }

            // Converts the password+salt to bytes
            byte[] saltedPassword = Encoding.ASCII.GetBytes(RegPassword + salt);

            byte[] hashedPassword = SHA256.Create().ComputeHash(saltedPassword);

            //Encrypts it and converts it to string
            string password = Convert.ToBase64String(hashedPassword);

            Dictionary<string, object> UserData = new() {
                {"username", RegUsername},
                {"mail", RegMail},
                { "password", password },
                { "password_salt", salt }
            };

            manager.GetTable("users").InsertRow(UserData);

            RegMessageText = "Succès !";
            RegMessageColor = Brushes.Green;
            IsRegBtnEnabled = false;

            ReturnToMenuDelayed();
        }

        private string VerifyRegisterForm()
        {
            RegUsername = RegUsername.Trim(' ');
            RegMail = RegMail.Trim(' ');
            RegPassword = RegPassword.Trim(' ');
            RegPasswordVerify = RegPasswordVerify.Trim(' ');

            // Tests de longueurs min
            if (RegUsername.Length < 0 ||
                RegMail.Length < 6 ||
                RegPassword.Length < 6)
            {
                RegMessageText = "Erreur dans le formulaire. \n(longueur minimum)";
                return "LongueurMin";
            }
            // Tests de longueurs max
            if (RegUsername.Length > 128 ||
                RegMail.Length > 256 ||
                RegPassword.Length > 256)
            {
                RegMessageText = "Erreur dans le formulaire. \n(longueur maximale)";
                return "LongueurMax";
            }

            // Teste la correspondance du mot de passe
            if (RegPassword != RegPasswordVerify)
            {
                RegMessageText = "Le mot de passe ne correspond pas.";
                return "MDPVerif";
            }

            // Teste la validite du mail
            var email = new EmailAddressAttribute();
            if (!email.IsValid(RegMail))
            {
                RegMessageText = "Email non valide.";
                return "MailValide";
            }

            // Teste si l'email n'est pas déjà dans la base de donnee
            MySqlManager manager = new();

            var testDB = manager.GetTable("users").SelectRow("mail", RegMail);

            if (testDB.Count != 0)
            {
                RegMessageText = "Email déjà inscrit.";
                return "MailPresent"; ;
            }

            RegMessageText = "";
            return "AllGood";
        }

        private async void ReturnToMenuDelayed()
        {
            await Task.Delay(2000);

            Clear();
            RegisterMenuVisibility = Visibility.Collapsed;
            LoginMenuVisibility = Visibility.Visible;
        }

        

    }
}
