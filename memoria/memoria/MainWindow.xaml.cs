using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace memoria
{
    public partial class MainWindow : Window
    {
        private List<Card> cards;
        private Button firstCardButton, secondCardButton;
        private DispatcherTimer timer;
        private int elapsedTime, matchedPairs;
        private int GridSize = 4; // Default size (4x4)

        public MainWindow()
        {
            InitializeComponent();  // This should be the first line in the constructor

            // Set the initial state for the StartGameButton
            if (StartGameButton != null)
            {
                StartGameButton.IsEnabled = true;  // Disable until difficulty is selected
            }
        }

        private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DifficultyComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                GridSize = int.Parse(selectedItem.Tag.ToString());

                // Enable the StartGameButton if it is not null
                if (StartGameButton != null)
                {
                    StartGameButton.IsEnabled = true;
                }
            }
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            matchedPairs = 0;
            GameGrid.Children.Clear();
            GameGrid.Rows = GridSize;
            GameGrid.Columns = GridSize;

            // Ha van régi timer, állítsuk le
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            SetupGame();
            SetupTimer();
        }

        private void SetupGame()
        {
            int numberOfPairs = (GridSize * GridSize) / 2;
            cards = GenerateCards(numberOfPairs);

            foreach (var card in cards)
            {
                var button = new Button
                {
                    Tag = card,
                    Content = "?",
                    FontSize = 24,
                    Margin = new Thickness(5)
                };
                button.Click += Card_Click;
                GameGrid.Children.Add(button);
            }
        }

        private List<Card> GenerateCards(int pairCount)
        {
            var numbers = Enumerable.Range(1, pairCount).ToList();
            var cardSet = numbers.Concat(numbers).ToList();
            var random = new Random();
            cardSet = cardSet.OrderBy(x => random.Next()).ToList();

            return cardSet.Select(number => new Card(number)).ToList();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (firstCardButton != null && secondCardButton != null) return;

            var button = sender as Button;
            if (button == null || button == firstCardButton || button.Content.ToString() != "?") return;

            var card = (Card)button.Tag;
            button.Content = card.Number.ToString();

            if (firstCardButton == null)
            {
                firstCardButton = button;
            }
            else
            {
                secondCardButton = button;
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            var card1 = (Card)firstCardButton.Tag;
            var card2 = (Card)secondCardButton.Tag;

            if (card1.Number == card2.Number)
            {
                card1.IsMatched = card2.IsMatched = true;
                matchedPairs++;
                firstCardButton.IsEnabled = secondCardButton.IsEnabled = false;
                ResetSelectedCards();
                if (matchedPairs == (GridSize * GridSize) / 2) EndGame();
            }
            else
            {
                var delay = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                delay.Tick += (s, args) =>
                {
                    delay.Stop();
                    firstCardButton.Content = secondCardButton.Content = "?";
                    ResetSelectedCards();
                };
                delay.Start();
            }
        }

        private void ResetSelectedCards()
        {
            firstCardButton = null;
            secondCardButton = null;
        }

        private void SetupTimer()
        {
            elapsedTime = 0;
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            TimerText.Text = $"Idő: {elapsedTime} mp";
        }

        private void EndGame()
        {
            timer.Stop();
            MessageBox.Show($"Gratulálok! Befejezted a játékot {elapsedTime} másodperc alatt.");
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Leállítjuk és nullázzuk a timer-t, mielőtt új játékot indítunk
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            StartGame();
        }
    }
}
