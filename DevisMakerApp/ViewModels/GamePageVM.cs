using DevisMakerApp.Classes.MySqlCrud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DevisMakerApp.ViewModels
{
    class GamePageVM : BaseViewModel
    {

        // Menu / game settings
        private int difficulty;

        private int maxRounds;
        
        public ICommand StartGame { get; }

        private bool isMenuVisible;
        private bool isGameVisible;
        private bool isEndVisible;
        private bool isStatsVisible;


        public int Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                OnPropertyChanged("Difficulty");
            }
        }
        public int MaxRoundsIndex
        {
            get { return (maxRounds-10)/5; }
            set
            {
                maxRounds = (value + 2) * 5;
                OnPropertyChanged("MaxRoundsIndex");
            }
        }
        

        public Visibility MenuVisibility
        {
            get { return isMenuVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isMenuVisible = (value == Visibility.Visible);
                OnPropertyChanged("MenuVisibility");
            }
        }
        public Visibility GameVisibility
        {
            get { return isGameVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isGameVisible = (value == Visibility.Visible);
                OnPropertyChanged("GameVisibility");
            }
        }
        public Visibility EndVisibility
        {
            get { return isEndVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isEndVisible = (value == Visibility.Visible);
                OnPropertyChanged("EndVisibility");
            }
        }
        public Visibility StatsVisibility
        {
            get { return isStatsVisible ? Visibility.Visible : Visibility.Collapsed; }
            set
            {
                isStatsVisible = (value == Visibility.Visible);
                OnPropertyChanged("StatsVisibility");
            }
        }

        // Game
        private Random random = new();

        private int currentRound;

        private int score;
        private string scoreText;
        private string roundsText;

        private int valueOne;
        private int valueTwo;
        private int result;

        private bool isValueOneLocked;
        private bool isValueTwoLocked;
        private bool isResultLocked;

        public ICommand NewRound { get; }
        public ICommand RePlay { get; }

        

        private int toGuess;

        public bool IsValueOneLocked
        {
            get { return isValueOneLocked; }
            set
            {
                isValueOneLocked = value;
                OnPropertyChanged("IsValueOneLocked");
            }
        }
        public bool IsValueTwoLocked
        {
            get { return isValueTwoLocked; }
            set
            {
                isValueTwoLocked = value;
                OnPropertyChanged("IsValueTwoLocked");
            }
        }
        public bool IsResultLocked
        {
            get { return isResultLocked; }
            set
            {
                isResultLocked = value;
                OnPropertyChanged("IsResultLocked");
            }
        }

        public int CurrentRound
        {
            get { return currentRound; }
            set
            {
                currentRound = value;
                OnPropertyChanged("CurrentRound");
            }
        }
        public int Score
        {
            get { return score; }
            set
            {
                score = value;
                OnPropertyChanged("Score");
            }
        }

        public string ScoreText
        {
            get { return scoreText; }
            set
            {
                scoreText = value;
                OnPropertyChanged("ScoreText");
            }
        }
        public string RoundsText
        {
            get { return roundsText; }
            set
            {
                roundsText = value;
                OnPropertyChanged("RoundsText");
            }
        }

        public int ValueOne
        {
            get { return valueOne; }
            set
            {
                valueOne = value;
                OnPropertyChanged("ValueOne");
            }
        }
        public int ValueTwo
        {
            get { return valueTwo; }
            set
            {
                valueTwo = value;
                OnPropertyChanged("ValueTwo");
            }
        }
        public int Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }

        // STATS

        public ICommand GoToGameMenu { get; }
        public ICommand GoToStatsMenu { get; }

        private string userName;
        private string userSurname;

        private float userAvgEasy;
        private float userAvgNormal;
        private float userAvgHard;

        private float userAvgTotal;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }
        public string UserSurname
        {
            get { return userSurname; }
            set
            {
                userSurname = value;
                OnPropertyChanged("UserSurname");
            }
        }

        public float UserAvgEasy
        {
            get { return userAvgEasy; }
            set
            {
                userAvgEasy = value;
                OnPropertyChanged("UserAvgEasy");
            }
        }
        public float UserAvgNormal
        {
            get { return userAvgNormal; }
            set
            {
                userAvgNormal = value;
                OnPropertyChanged("UserAvgNormal");
            }
        }
        public float UserAvgHard
        {
            get { return userAvgHard; }
            set
            {
                userAvgHard = value;
                OnPropertyChanged("UserAvgHard");
            }
        }
        public float UserAvgTotal
        {
            get { return userAvgTotal; }
            set
            {
                userAvgTotal = value;
                OnPropertyChanged("UserAvgTotal");
            }
        }


        public GamePageVM()
        {
            StartGame = new RelayCommand(o => startGame());
            NewRound = new RelayCommand(o => newRound());
            RePlay = new RelayCommand(o => rePlay());
            GoToStatsMenu = new RelayCommand(o => goToStatsMenu());

            Difficulty = 1;
            MaxRoundsIndex = 1;
            CurrentRound = 0;
            Score = 0;

            

            StatsVisibility = Visibility.Collapsed;
            EndVisibility = Visibility.Collapsed;
            GameVisibility = Visibility.Collapsed;
            MenuVisibility = Visibility.Visible;

        }

        // GAME
        private void startGame()
        {
            GameVisibility = Visibility.Visible;
            MenuVisibility = Visibility.Collapsed;

            CurrentRound = 0;
            Score = 0;

            newRound();
        }

        private void endGame()
        {
            ScoreText = $"Score final: {Score}/{currentRound}";
            GameVisibility = Visibility.Collapsed;
            EndVisibility= Visibility.Visible;

            MySqlManager manager = new();

            manager.GetTable("rounds").InsertRow(new Dictionary<string, object>
            {
                {"user_id", MainVM.User.Id},
                {"difficulty", Difficulty },
                {"correct_answers", Score },
                {"max_answers", maxRounds }
            });

        }

        private void rePlay()
        {
            EndVisibility = Visibility.Visible;
            MenuVisibility = Visibility.Visible;
        }

        private void newRound()
        {
            //If not the first round, checks the answer of the last one
            if (currentRound != 0 && ValueOne + ValueTwo == Result)
            {
                Score++;
            }

            // If last round was THE last round, do not generate a new one
            if(currentRound >= maxRounds)
            {

                endGame();
                return;
            }
            
            // Generate round
            currentRound++;

            ScoreText = $"Score: {Score}/{currentRound-1}";
            RoundsText = $"Round: {currentRound}/{maxRounds}";

            ValueOne = 0;
            ValueTwo = 0;
            Result = 0;

            IsValueOneLocked = true;
            IsValueTwoLocked = true;
            isResultLocked = true;

            int maxValue = (int) Math.Pow(10, 2 + Difficulty);

            toGuess = random.Next(3);

            switch (toGuess)
            {
                case 0: // First Value
                    IsValueOneLocked = false;
                    ValueTwo = random.Next(maxValue);
                    Result = ValueTwo + random.Next(maxValue);

                    break;

                case 1: // Second Value
                    ValueOne = random.Next(maxValue);
                    IsValueTwoLocked = false;
                    Result = ValueOne + random.Next(maxValue);

                    break;

                case 2: // Result Value
                    ValueOne = random.Next(maxValue);
                    ValueTwo = random.Next(maxValue);
                    IsResultLocked = false;

                    break;

            }
        }

        // STATS
        private void goToStatsMenu()
        {
            Trace.WriteLine("TEST");

            MySqlManager manager = new();

            var rows = manager.GetTable("rounds").SelectRows("user_id", MainVM.User.Id);

            float avgEasy = 0f;
            float avgNormal = 0f;
            float avgHard = 0f;

            int nbEasy = 0;
            int nbNormal = 0;
            int nbHard = 0;

            Trace.WriteLine(rows);

            foreach (var row in rows)
            {
                Trace.WriteLine(row["correct_answers"].GetType());
                Trace.WriteLine(row["max_answers"].GetType());

                //Trace.WriteLine("cor1"+row["correct_answers"]);
                switch (Convert.ToInt32(row["difficulty"]))
                {
                    case 0:
                        nbEasy++;
                        avgEasy +=   (float)(int)row["correct_answers"] / (int) row["max_answers"];
                        Trace.WriteLine(row["correct_answers"]);
                        break;

                    case 1:
                        nbNormal++;
                        avgNormal += (float)(int)row["correct_answers"] / (int)row["max_answers"];
                        Trace.WriteLine("avgnormal: " + avgEasy);
                        break;

                    case 2:
                        nbHard++;
                        avgHard += (float)(int)row["correct_answers"] / (int)row["max_answers"];
                        break;
                }
            }

            UserAvgEasy = nbEasy != 0 ? avgEasy / nbEasy : 0f;
            UserAvgNormal = nbNormal != 0 ? avgNormal / nbNormal : 0f;
            UserAvgHard = nbHard != 0 ? avgHard / nbHard : 0f;

            UserAvgTotal = (UserAvgEasy * nbEasy + UserAvgNormal * nbNormal + UserAvgHard * nbHard) / ( nbEasy + nbNormal + nbHard);

            UserName = MainVM.User.Name;
            UserSurname = UserSurname != "." ? MainVM.User.Surname : "";
            
            MenuVisibility = Visibility.Collapsed;
            StatsVisibility = Visibility.Visible;
            
        }

        private void goToGameMenu()
        {
            StatsVisibility = Visibility.Collapsed;
            MenuVisibility = Visibility.Visible;
        }

    }
}
