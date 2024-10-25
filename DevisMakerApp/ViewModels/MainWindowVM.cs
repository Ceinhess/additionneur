using Additionneur.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Additionneur.Models;

namespace Additionneur.ViewModels
{
    class MainWindowVM : BaseViewModel
    {

        ICommand ChangeView;

        private Page activeView;

        public User User;
        public Page ActiveView
        { 
            get { return activeView; }
            set
            {
                activeView = value;
                OnPropertyChanged("ActiveView");
            }
        }


        public MainWindowVM() 
        {
            ChangeView = new RelayCommand(o => ChangeActiveView(o));

            ActiveView = new LoggingPage();
            
        }

        public void GoToGameView()
        {
            ActiveView = new GamePage();
        }

        public void GoToLoggingView()
        {
            ActiveView = new LoggingPage();
        }

        private void ChangeActiveView(object sender)
        {
            
        }
        
    }
}
