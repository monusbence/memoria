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

        public MainWindow()
        {
            InitializeComponent();
            SetupGame();
            SetupTimer();
        }

        private void SetupGame()
        {
            matchedPairs = 0;
            GameGrid.Children.Clear();
            cards = GenerateCards();
            foreach (var card in cards)
            {
                var button = new Button
                {
                    Tag = card,
                    Content = "?",  // Alapértelmezett kérdőjel
                    FontSize = 24,
                    Margin = new Thickness(5)
                };
                button.Click += Card_Click;
                GameGrid.Children.Add(button);
            }
        }

        private List<Card> GenerateCards()
        {
            // Generáljuk a kártyák listáját párokkal
            var numbers = Enumerable.Range(1, 8).ToList();
            var cardSet = numbers.Concat(numbers).ToList(); // Minden szám kétszer szerepel
            var random = new Random();
            cardSet = cardSet.OrderBy(x => random.Next()).ToList();

            return cardSet.Select(number => new Card(number)).ToList();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            // Ha már két kártya ki van választva, akkor ne engedjen újabb kattintást
            if (firstCardButton != null && secondCardButton != null) return;

            var button = sender as Button;
            if (button == null || button == firstCardButton || button.Content.ToString() != "?") return;

            var card = (Card)button.Tag;
            button.Content = card.Number.ToString();  // Megjeleníti a kártya számát

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
                if (matchedPairs == 8) EndGame();
            }
            else
            {
                var delay = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                delay.Tick += (s, args) =>
                {
                    delay.Stop();
                    firstCardButton.Content = secondCardButton.Content = "?"; // Visszaállít kérdőjelre
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
            timer.Tick += (s, e) =>
            {
                elapsedTime++;
                TimerText.Text = $"Time: {elapsedTime} s";
            };
            timer.Start();
        }

        private void EndGame()
        {
            timer.Stop();
            MessageBox.Show($"Gratulálok! Befejezted a játékot {elapsedTime} másodperc alatt.");
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            SetupGame();
            SetupTimer();
        }
    }
}
