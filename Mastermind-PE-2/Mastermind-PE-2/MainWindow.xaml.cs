using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mastermind_PE_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _colors = new string[6]
        {
            "Red", // 0
            "Yellow", // 1
            "Orange", // 2
            "White", // 3
            "Green", // 4
            "Blue" // 5
        };

        private string[] _code = new string[4];
        private string[] _playerGuess = new string[4];
        private Label[] _labels = new Label[4];
        private int _attempts = 0;
        private int _maxAttempts = 10;
        private int _score = 100;
        private Border[,] _guessHistory = new Border[10, 4]; // _maxattempts moet hier in
        private string[] _highScores = new string[15];
        private string[] _playerName = new string[15]; // Vullen met spelernamen, ook max 15?

        //close message enkel mid-game met dit
        private bool _gameIsOngoing = true;

        // zogenaamde debug
        private bool _showSolution = false;

        // override > space > onClosING!! kiezen uit de dropdown
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (_gameIsOngoing)
            {
                MessageBoxResult exitMessage = MessageBox.Show(
                    $"Closing up? All unsolved codes will remain a mystery!", $"Attempt {_attempts}/{_maxAttempts}",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (exitMessage == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
            GenerateColorCode();
            UpdateAttempt();
            _labels[0] = colorLabel1;
            _labels[1] = colorLabel2;
            _labels[2] = colorLabel3;
            _labels[3] = colorLabel4;
        }

        private void StartGame()
        {
            // User naam vragen.
        }

        private void GenerateColorCode()
        {
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                int randomColorIndex = random.Next(6);
                _code[i] = _colors[randomColorIndex];
            }

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Title = $"Mastermind";
            if (_showSolution)
            {
                Title += $" ({string.Join(", ", _code)})";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedColor = selectedItem.Content.ToString();

                if (comboBox == comboBox1)
                {
                    _playerGuess[0] = selectedColor;
                    colorLabel1.BorderBrush = Brushes.LightGray;
                    colorLabel1.BorderThickness = new Thickness(3);
                    colorLabel1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedColor));
                }
                else if (comboBox == comboBox2)
                {
                    _playerGuess[1] = selectedColor;
                    colorLabel2.BorderBrush = Brushes.LightGray;
                    colorLabel2.BorderThickness = new Thickness(3);
                    colorLabel2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedColor));
                }
                else if (comboBox == comboBox3)
                {
                    _playerGuess[2] = selectedColor;
                    colorLabel3.BorderBrush = Brushes.LightGray;
                    colorLabel3.BorderThickness = new Thickness(3);
                    colorLabel3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedColor));
                }
                else if (comboBox == comboBox4)
                {
                    _playerGuess[3] = selectedColor;
                    colorLabel4.BorderBrush = Brushes.LightGray;
                    colorLabel4.BorderThickness = new Thickness(3);
                    colorLabel4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedColor));
                }
            }
        }

        private void ValidateAnswers_Click(object sender, RoutedEventArgs e)
        {
            bool answerIsGuessed = true;

            for (int i = 0; i < 4; i++)
            {
                if (_code[i] == _playerGuess[i])
                {
                    _labels[i].BorderBrush = Brushes.Black;
                    _labels[i].BorderThickness = new Thickness(3);
                }
                else if (_code.Contains(_playerGuess[i]))
                {
                    _labels[i].BorderBrush = Brushes.Wheat;
                    _labels[i].BorderThickness = new Thickness(3);
                    //kleur op de foute plaats = -1 punt
                    _score = _score - 1;
                    answerIsGuessed = false;
                }
                else // als m'n kleur totaal niet aanwezig is, -2 punten
                {
                    _score = _score - 2;
                    answerIsGuessed = false;
                }
            }

            for (int i = 0; i < _labels.Length; i++)
            {
                _guessHistory[_attempts, i] = new Border()
                {
                    Width = 40,
                    Height = 40,
                    Background = _labels[i].Background,
                    BorderBrush = _labels[i].BorderBrush.Clone(),
                    BorderThickness = _labels[i].BorderThickness,
                    CornerRadius = new CornerRadius(20),
                    Margin = new Thickness(5)
                };
            }

            _attempts++;
            UpdateAttempt();
            UpdateScore();
            UpdateHistory();

            // einde spel -> niet meer optie geven om te sluiten
            if (answerIsGuessed)
            {
                if (_attempts == 1)
                {
                    MessageBoxResult oneTryMessage = MessageBox.Show(
                   $"Wowza! You guessed the code on your first try!\r\nLet's see how long your luck will last in the next round!", "WINNER",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);
                    _gameIsOngoing = false;

                    NewGame();
                }
                else
                {
                    MessageBoxResult winMessage = MessageBox.Show(
                       $"You did it in {_attempts} tries! The code never stood a chance!\r\nLet's aim for the high-score!", "WINNER", //<-message titel moet blijkbaar, anders error
                       MessageBoxButton.OK,
                       MessageBoxImage.Information);
                    _gameIsOngoing = false;

                    NewGame();
                }
            }
            else if (_attempts == _maxAttempts)
            {
                MessageBoxResult loseMessage = MessageBox.Show(
                    $"Close, but no cigar! The answer was: {string.Join(", ", _code)}.\r\nLet's try again!", "FAILED",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _gameIsOngoing = false;

                NewGame();
            }
        }

        private void NewGame()
        {
            for (int i = 0; i < _labels.Length; i++)
            {
                _labels[i].Background = Brushes.Transparent;
                _labels[i].BorderBrush = Brushes.LightGray;
                _labels[i].BorderThickness = new Thickness(1);
            }

            _attempts = 0;
            _score = 100;
            _guessHistory = new Border[_maxAttempts, 4];
            userGuessHistory.Children.Clear();

            UpdateAttempt();
            UpdateScore();
            UpdateHistory();

            GenerateColorCode();
        }

        private void UpdateScore()
        {
            scoreLabel.Content = _score;
        }

        private void UpdateAttempt()
        {
            attemptLabel.Content = $"{_attempts}/{_maxAttempts}";
        }

        private void UpdateHistory()
        {
            for (int i = 0; i < _maxAttempts; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Border border = (Border)_guessHistory.GetValue(i, j);

                    if (border == null)
                    {
                        continue;
                    }

                    border.Height = 40;
                    border.Width = 40;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.Margin = new Thickness(6);

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    if (!userGuessHistory.Children.Contains(border))
                    {
                        userGuessHistory.Children.Add(border);
                    }
                }
            }
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void newGameMenu_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void HiScore_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult highScoreMssg = MessageBox.Show(
                $"Name - attempts - score", // player input hier in krijgen
                "Mastermind highscores",
                MessageBoxButton.OK,
                MessageBoxImage.None);

            for (int i = 0; i < 15; i++) // max 15 scores
            {
            }
        }
    }
}