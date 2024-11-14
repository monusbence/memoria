namespace memoria
{
    internal class Card
    {
        public int Number { get; set; }          // A kártyán lévő szám
        public bool IsMatched { get; set; } = false; // Jelzi, hogy a kártya már párosítva lett-e

        public Card(int number)
        {
            Number = number;
        }
    }
}
