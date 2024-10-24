using Additionneur.Classes.MySqlCrud;
using Additionneur.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Additionneur.ViewModels
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

        private int gameTypeIndex;

        /// <summary>
        /// 0 = Sums, 1 = Differences, 2 = Multiplications, 3 = Divisions.
        /// </summary>
        public int GameTypeIndex
        {
            get { return gameTypeIndex; }
            set
            {
                gameTypeIndex = value;
                OnPropertyChanged("GameTypeIndex");
            }
        }
        /// <summary>
        /// 0 = easy, 1 = normal, 2 = hard
        /// </summary>
        public int Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                OnPropertyChanged("Difficulty");
            }
        }
        /// <summary>
        /// (x + 1) * 5. <br/>
        /// ex: 1 = 10, 2 = 15, 4 = 25 ...
        /// </summary>
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
        private string operationText;

        private int valueOne;
        private int valueTwo;
        private int result;

        private bool isValueOneLocked;
        private bool isValueTwoLocked;
        private bool isResultLocked;

        private string focusedField;

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

        public string OperationText
        {
            get { return operationText; }
            set
            {
                operationText = value;
                OnPropertyChanged("OperationText");
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

        public string FocusedField
        {
            get { return focusedField; }
            set
            {
                focusedField = value;
                OnPropertyChanged("FocusedField");
            }
        }

        // STATS

        public ICommand GoToGameMenu { get; }
        public ICommand GoToStatsMenu { get; }

        private string userName;
        private string userSurname;

        private int statsTypeIndex = 0;
        private int statsDifficultyIndex = 0;

        private ObservableCollection<Grid> statsList = [];

        private float userAvgEasy;
        private float userAvgNormal;
        private float userAvgHard;

        private float userAvgTotal;

        public ObservableCollection<Grid> StatsList
        {
            get { return statsList; }
            set
            {
                statsList = value;
                OnPropertyChanged("StatsList");
            }
        }

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

        public int StatsTypeIndex
        {
            get { return statsTypeIndex; }
            set
            {
                statsTypeIndex = value;
                generateStats();
                OnPropertyChanged("StatsTypeIndex");
                
            }
        }

        public int StatsDifficultyIndex
        {
            get { return statsDifficultyIndex; }
            set
            {
                statsDifficultyIndex = value;
                generateStats();
                OnPropertyChanged("StatsDifficultyIndex");
                
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
            GoToGameMenu = new RelayCommand(o=> goToGameMenu());

            GameTypeIndex = 0;
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

            OperationText = (new[] { "+", "-", "x", "//" })[GameTypeIndex];

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
            EndVisibility = Visibility.Collapsed;
            MenuVisibility = Visibility.Visible;
        }

        private void newRound()
        {
            //If not the first round, checks the answer of the last one depending on the game type
            if (currentRound != 0)
            {
                switch(GameTypeIndex)
                {
                    case 0: // sum
                        if (ValueOne + ValueTwo == Result) Score++;
                        break;
                    case 1: // difference
                        if (ValueOne - ValueTwo == Result) Score++;
                        break;
                    case 2: // multiplication
                        if (ValueOne * ValueTwo == Result) Score++;
                        break;
                    case 3: // division
                        if (ValueOne / ValueTwo == Result) Score++;
                        break;
                }
            }

            // If last round was THE last round, do not generate a new one
            if(currentRound >= maxRounds)
            {

                endGame();
                return;
            }
            
            // Updates current round and text + resets fields values
            currentRound++;

            ScoreText = $"Score: {Score}/{currentRound-1}";
            RoundsText = $"Round: {currentRound}/{maxRounds}";

            ValueOne = 0;
            ValueTwo = 0;
            Result = 0;

            IsValueOneLocked = true;
            IsValueTwoLocked = true;
            IsResultLocked = true;

            toGuess = random.Next(3);

            FocusedField = (new[] { "ValueOneField", "ValueTwoField", "ResultField" })[toGuess];

            // generates the round
            switch (GameTypeIndex)
            {
                case 0:
                    makeSumRound();
                    break;
                case 1:
                    makeDifferenceRound();
                    break;
                case 2:
                    makeMultiplicationRound();
                    break;
                case 3:
                    makeDivisionRound();
                    break;
            }
        }

        private void makeSumRound()
        {
            int maxValue = (int)Math.Pow(10, 2 + Difficulty);

            //If difficulty is hard, allow negative values
            int minValue = Difficulty == 2 ? -maxValue : 0;

            switch (toGuess)
            {
                case 0: // First Value
                    IsValueOneLocked = false;
                    ValueTwo = random.Next(minValue, maxValue);
                    Result = random.Next(minValue, maxValue) + valueTwo;

                    break;

                case 1: // Second Value
                    ValueOne = random.Next(minValue, maxValue);
                    IsValueTwoLocked = false;
                    Result = ValueOne + random.Next(minValue, maxValue);

                    break;

                case 2: // Result Value
                    ValueOne = random.Next(minValue, maxValue);
                    ValueTwo = random.Next(minValue, maxValue);
                    IsResultLocked = false;

                    break;

            }
        }

        

        private void makeDifferenceRound()
        {
            int maxValue = (int)Math.Pow(10, 2 + Difficulty) / 2;

            //If difficulty is hard, allow negative values
            int minValue = Difficulty == 2 ? -maxValue : 0;

            switch (toGuess)
            {
                case 0: // First Value
                    IsValueOneLocked = false;
                    ValueTwo = random.Next(minValue, maxValue);
                    Result = random.Next(minValue, maxValue) - valueTwo;

                    break;

                case 1: // Second Value
                    ValueOne = random.Next(minValue, maxValue);
                    IsValueTwoLocked = false;
                    Result = ValueOne - random.Next(minValue, maxValue);

                    break;

                case 2: // Result Value
                    ValueOne = random.Next(minValue, maxValue);
                    ValueTwo = random.Next(minValue, maxValue);
                    IsResultLocked = false;

                    break;

            }
        }

        private void makeMultiplicationRound()
        {
            // 10 for easy, 25 for normal, 100 for hard
            int maxValue = (new[] { 10, 25, 100 })[Difficulty];

            //If difficulty isn't easy, allow negative values
            int minValue = Difficulty == 0 ? 2 : -maxValue;

            switch (toGuess)
            {
                case 0: // First Value
                    IsValueOneLocked = false;
                    ValueTwo = random.Next(minValue, maxValue);
                    Result = random.Next(minValue, maxValue) * valueTwo;

                    break;

                case 1: // Second Value
                    ValueOne = random.Next(minValue, maxValue);
                    IsValueTwoLocked = false;
                    Result = ValueOne * random.Next(minValue, maxValue);

                    break;

                case 2: // Result Value
                    ValueOne = random.Next(minValue, maxValue);
                    ValueTwo = random.Next(minValue, maxValue);
                    IsResultLocked = false;

                    break;

            }
        }

        private void makeDivisionRound()
        {

            int maxFirstValue = (int)Math.Pow(10, 2 + Difficulty);

            //If difficulty isn't easy, allow negative values
            int minFirstValue = Difficulty == 0 ? 10 : -maxFirstValue;

            FocusedField = "ValueField";

            ValueOne = random.Next(minFirstValue, maxFirstValue);

            ValueTwo = ValueOne > 0 ? random.Next(-(ValueOne / 2), ValueOne / 2) : random.Next((ValueOne / 2), -ValueOne / 2);

            IsResultLocked = false;

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

            generateStats();


        }

        private void generateStats()
        {
            StatsList.Clear();

            MySqlManager manager = new();

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            List<string> columnsNames = [];
            List<object> columnsValues = [];

            columnsNames.Add("user_id");
            columnsValues.Add(MainVM.User.Id);

            if (StatsTypeIndex != 4)
            {
                columnsNames.Add("game_type");
                columnsValues.Add(StatsTypeIndex);
            }

            if (StatsDifficultyIndex != 3)
            {
                columnsNames.Add("difficulty");
                columnsValues.Add(StatsDifficultyIndex);
            }


            if(columnsNames.Count > 1)
                rows = manager.GetTable("rounds").SelectRows(columnsNames.ToArray(), columnsValues.ToArray());
            else
                rows = manager.GetTable("rounds").SelectRows(columnsNames[0], columnsValues[0]);


            for(int j = rows.Count - 1; j >= 0; j--)
            { 
                var row = rows[j];
            
                Grid container = new();

                for(int i = 0; i<5; i++)
                {
                    ColumnDefinition col = new();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    container.ColumnDefinitions.Add(col);
                }

                container.HorizontalAlignment = HorizontalAlignment.Stretch;
                    

                TextBox diff = new();
                diff.Text = (new[] { "Easy", "Normal", "Hard" })[(int) row["difficulty"]];
                diff.SetValue(Grid.ColumnProperty, 0);

                TextBox totRounds = new();
                totRounds.Text = row["max_answers"].ToString();
                totRounds.SetValue(Grid.ColumnProperty, 1);

                TextBox score = new();
                score.Text = row["correct_answers"].ToString();
                score.SetValue(Grid.ColumnProperty, 2);

                TextBox avg = new();
                avg.Text = (((int) row["correct_answers"] * 100) / ((int) row["max_answers"]) ).ToString() + "%";
                avg.SetValue(Grid.ColumnProperty, 3);

                container.Children.Add(diff);
                container.Children.Add(totRounds);
                container.Children.Add(score);
                container.Children.Add(avg);

                foreach (TextBox t in container.Children)
                {
                    t.FontFamily = new System.Windows.Media.FontFamily("Cascadia Code");
                    t.FontSize = 16;
                    t.VerticalAlignment = VerticalAlignment.Center;
                    t.Margin = new Thickness(25, 0, 0, 0);
                    t.HorizontalAlignment = HorizontalAlignment.Right;
                    t.BorderThickness = new Thickness(0);
                    t.Background = Brushes.Transparent;
                }

                StatsList.Add(container);
            }

            




        }

        private void goToGameMenu()
        {
            StatsVisibility = Visibility.Collapsed;
            MenuVisibility = Visibility.Visible;
        }


    }
}
