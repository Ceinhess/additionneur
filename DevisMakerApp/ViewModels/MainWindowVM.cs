using DevisMakerApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DevisMakerApp.Models;

namespace DevisMakerApp.ViewModels
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

        public override void GoToGameView()
        {
            ActiveView = new GamePage();
        }

        private void ChangeActiveView(object sender)
        {
            
        }
        
    }
}
