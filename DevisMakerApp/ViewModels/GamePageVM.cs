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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace Additionneur.ViewModels
{
    class GamePageVM : BaseViewModel
    {

        // Menu / game settings
        private int difficulty;

        private int maxRounds;

        public ICommand StartGame { get; }

        public ICommand GoToLoggingMenu { get; }

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
            get { return (maxRounds - 10) / 5; }
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

        private Color backgroundGradientColor = Colors.White;

        public ICommand NewRound { get; }
        public ICommand RePlay { get; }

        private int toGuess;

        public Color BackgroundGradientColor
        {
            get { return backgroundGradientColor; }
            set
            {
                backgroundGradientColor = value;
                OnPropertyChanged("BackgroundGradientColor");
            }
        }

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

        private string username;

        private int statsTypeIndex = 0;
        private int statsDifficultyIndex = 0;

        private ObservableCollection<Grid> statsList = [];

        private int averageScore;
        private int totalGames;



        public ObservableCollection<Grid> StatsList
        {
            get { return statsList; }
            set
            {
                statsList = value;
                OnPropertyChanged("StatsList");
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged("Username");
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

        public int AverageScore
        {
            get { return averageScore; }
            set
            {
                averageScore = value;
                OnPropertyChanged("AverageScore");
            }
        }
        public int TotalGames
        {
            get { return totalGames; }
            set
            {
                totalGames = value;
                OnPropertyChanged("TotalGames");
            }
        }

        public GamePageVM()
        {
            StartGame = new RelayCommand(o => startGame());
            NewRound = new RelayCommand(o => newRound());
            RePlay = new RelayCommand(o => rePlay());
            GoToStatsMenu = new RelayCommand(o => goToStatsMenu());
            GoToGameMenu = new RelayCommand(o => goToGameMenu());
            GoToLoggingMenu = new RelayCommand(o => goToLoggingMenu());

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
            // Shows the game screen
            GameVisibility = Visibility.Visible;
            MenuVisibility = Visibility.Collapsed;

            // Resets the score and round counters
            CurrentRound = 0;
            Score = 0;

            // Changes the sign of the operation according to the game type
            // 0 = sum, 1 = difference, 2 = mult, 3 = division
            OperationText = (new[] { "+", "-", "x", "//" })[GameTypeIndex];

            // Generate the first round
            newRound();
        }

        private void endGame()
        {
            // Shows the end screen
            GameVisibility = Visibility.Collapsed;
            EndVisibility = Visibility.Visible;

            // updates the score text
            ScoreText = $"Final score: {Score}/{currentRound-1}";
            
            // upload the round stats to the DB
            MySqlManager manager = new();

            manager.GetTable("games").InsertRow(new Dictionary<string, object>
            {
                {"user_id", MainVM.User.Id},
                {"difficulty", Difficulty },
                {"game_type", GameTypeIndex },
                {"correct_answers", Score },
                {"max_answers", maxRounds }
            });

        }

        private void rePlay()
        {
            // simply go back to the game menu
            EndVisibility = Visibility.Collapsed;
            MenuVisibility = Visibility.Visible;
        }

        private void newRound()
        {
            // Updates current round
            currentRound++;

            //If not the first round, checks the answer of the last one depending on the game type
            if (currentRound != 1)
            {
                switch(GameTypeIndex)
                {
                    case 0: // sum
                        if (ValueOne + ValueTwo == Result) 
                        {   // If good answer, increment score and set green background effect (red if wrong answer). Same for the 3 next cases.
                            Score++;
                            BackgroundGradientColor = new Color() { ScG = 0.7f, ScR = 0.2f, ScB = 0.4f, ScA = 0.5f };
                        }
                        else BackgroundGradientColor = new Color() { ScG = 0.22f, ScR = 0.7f, ScB = 0.2f, ScA = 0.5f };
                        break;
                    case 1: // difference
                        if (ValueOne - ValueTwo == Result)
                        {
                            Score++;
                            BackgroundGradientColor = new Color() { ScG = 0.7f, ScR = 0.2f, ScB = 0.4f, ScA = 0.5f };
                        }
                        else BackgroundGradientColor = new Color() { ScG = 0.22f, ScR = 0.7f, ScB = 0.2f, ScA = 0.5f };
                        break;
                    case 2: // multiplication
                        if (ValueOne * ValueTwo == Result)
                        {
                            Score++;
                            BackgroundGradientColor = new Color() { ScG = 0.7f, ScR = 0.2f, ScB = 0.4f, ScA = 0.5f};
                            
                        }
                        else BackgroundGradientColor = new Color() { ScG = 0.22f, ScR = 0.7f, ScB = 0.2f, ScA = 0.5f };
                        break;
                    case 3: // division
                        if (ValueOne / ValueTwo == Result)
                        {
                            Score++;
                            BackgroundGradientColor = new Color() { ScG = 0.7f, ScR = 0.2f, ScB = 0.4f, ScA = 1f };
                        }
                        else BackgroundGradientColor = new Color() { ScG = 0.22f, ScR = 0.7f, ScB = 0.2f, ScA = 0.5f };
                        break;
                }

                fadeBackground(currentRound);
            }

            // If last round was THE last round, do not generate a new one and end the game
            if(currentRound > maxRounds)
            {
                endGame();
                return;
            }
            
            // Updates current text + resets fields values
            ScoreText = $"Score: {Score}/{currentRound-1}";
            RoundsText = $"Round: {currentRound}/{maxRounds}";

            ValueOne = 0;
            ValueTwo = 0;
            Result = 0;

            IsValueOneLocked = true;
            IsValueTwoLocked = true;
            IsResultLocked = true;

            // Chooses which field the player will guess
            // 0 = first value, 1 = second one, 2 = result
            // will always be 2 for divisions
            toGuess = random.Next(3);

            FocusedField = (new[] { "ValueOneField", "ValueTwoField", "ResultField" })[toGuess];

            // generates the round depending on the type of game
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

        private async void fadeBackground(int round)
        {
            await Task.Delay(50);

            // Check if the fade is still on the correct round (avoid multiple stacking calls to this function if next button is spammed)
            if (round != currentRound)
                return;

            // Ligthens the background
            BackgroundGradientColor = BackgroundGradientColor * 1.2f;

            // If fully white, set color to white. 
            if (BackgroundGradientColor.ScR > 1f && BackgroundGradientColor.ScG > 1f && BackgroundGradientColor.ScB > 1f)
                BackgroundGradientColor = Colors.White;
            // If not, call the function again.
            else
                fadeBackground(round);
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
            // Updates User Name in case it's the first menu opening or if it changed
            Username = MainVM.User.Username;
            
            // Shows the stats menu and hide the game one
            MenuVisibility = Visibility.Collapsed;
            StatsVisibility = Visibility.Visible;

            // Generate the stats
            generateStats();


        }

        private void generateStats()
        {
            // Clears the list of games
            StatsList.Clear();

            // Colors corresponding to the background of eahc difficulty
            Color[] backgrounds = new[]
            {
                new Color() { R= 240, G= 255, B= 240, A= 255}, // easy
                new Color() { R= 250, G= 250, B= 240, A= 255}, // normal
                new Color() { R= 255, G= 240, B= 240, A= 255}  // hard
            };

            // Dict that'll store all the games rows
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            // Lists that correspond to the filters
            List<string> columnsNames = [];
            List<object> columnsValues = [];

            // Total score that will be used for average score calculation
            int avgScore = 0;

            // User will always be a filter
            columnsNames.Add("user_id");
            columnsValues.Add(MainVM.User.Id);

            if (StatsTypeIndex != 4)
            {   // If filtered game type (not "all"), adds it to the lists
                columnsNames.Add("game_type");
                columnsValues.Add(StatsTypeIndex);
            }

            if (StatsDifficultyIndex != 3)
            {   // If filtered difficulty (not "all"), adds it to the lists
                columnsNames.Add("difficulty");
                columnsValues.Add(StatsDifficultyIndex);
            }

            // Creates the manager for the MySQL request
            MySqlManager manager = new();

            if (columnsNames.Count > 1) // If more than one argument, pass array
                rows = manager.GetTable("games").SelectRows(columnsNames.ToArray(), columnsValues.ToArray());
            else // else pass just the user id column name and value
                rows = manager.GetTable("games").SelectRows(columnsNames[0], columnsValues[0]);

            // For each row (= game played)
            for(int j = rows.Count - 1; j >= 0; j--)
            { 
                // row = current row
                var row = rows[j];
                
                // Create a grid that'll contain the informations about this game
                Grid container = new();
                container.Background = new SolidColorBrush(backgrounds[(int)row["difficulty"]]);

                // Creates 6 columns of equal width in this grid
                for(int i = 0; i<6; i++)
                {
                    ColumnDefinition col = new();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    container.ColumnDefinitions.Add(col);
                }

                // Stretch the grid to take all the available width
                container.HorizontalAlignment = HorizontalAlignment.Stretch;

                // Textbox corresponding to the difficulty, in first column
                TextBox diff = new();
                diff.Text = (new[] { "Easy", "Normal", "Hard" })[(int)row["difficulty"]];
                diff.SetValue(Grid.ColumnProperty, 0);

                // Textbox corresponding to the game type, in first column
                TextBox type = new();
                type.Text = (new[] { "Sums", "Differences", "Multiplications", "Divisions" })[(int)row["game_type"]];
                type.SetValue(Grid.ColumnProperty, 1);

                // Textbox corresponding to the max amount of rounds in this game, in second column
                TextBox totRounds = new();
                totRounds.Text = row["max_answers"].ToString();
                totRounds.SetValue(Grid.ColumnProperty, 2);

                // Textbox corresponding to the score, third column
                TextBox score = new();
                score.Text = row["correct_answers"].ToString();
                score.SetValue(Grid.ColumnProperty, 3);

                // Textbox corresponding to average good answers in %, fourth column
                TextBox avg = new();
                int avgGame = (((int)row["correct_answers"] * 100) / ((int)row["max_answers"]));
                avg.Text = avgGame.ToString() + "%";
                avg.SetValue(Grid.ColumnProperty, 4);

                avgScore += avgGame;

                // Adds the texteboxes to the grid
                container.Children.Add(diff);
                container.Children.Add(type);
                container.Children.Add(totRounds);
                container.Children.Add(score);
                container.Children.Add(avg);

                // Give all of the textboxes their common properties
                foreach (TextBox t in container.Children)
                {
                    t.FontFamily = new FontFamily("Cascadia Code");
                    t.FontSize = 16;
                    t.VerticalAlignment = VerticalAlignment.Center;
                    t.Margin = new Thickness(25, 0, 0, 0);
                    t.HorizontalAlignment = HorizontalAlignment.Right;
                    t.BorderThickness = new Thickness(0);
                    t.Background = Brushes.Transparent;
                }

                // Adds the container to the List of played games
                StatsList.Add(container);
            }

            // Update total game and average score for the active filters
            TotalGames = rows.Count;

            AverageScore = rows.Count > 0 ? avgScore / rows.Count : 0;

        }

        private void goToGameMenu()
        {
            StatsVisibility = Visibility.Collapsed;
            MenuVisibility = Visibility.Visible;
        }

        private void goToLoggingMenu()
        {
            MainVM.GoToLoggingView();
        }
    }
}
