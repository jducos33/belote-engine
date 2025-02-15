namespace BeloteEngine.Model
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        /// <summary>
        /// Renvoie la valeur de la carte en fonction de son rang et de si elle est de l'atout.
        /// - Pour l'atout : Jack=20, Nine=14, Ace=11, Ten=10, King=4, Queen=3, sinon 0.
        /// - Hors atout : Ace=11, Ten=10, King=4, Queen=3, Jack=2, sinon 0.
        /// </summary>
        public int GetPoints(Suit trump)
        {
            if (this.Suit == trump)
            {
                switch (this.Rank)
                {
                    case Rank.Jack: return 20;
                    case Rank.Nine: return 14;
                    case Rank.Ace: return 11;
                    case Rank.Ten: return 10;
                    case Rank.King: return 4;
                    case Rank.Queen: return 3;
                    default: return 0;
                }
            }
            else
            {
                switch (this.Rank)
                {
                    case Rank.Ace: return 11;
                    case Rank.Ten: return 10;
                    case Rank.King: return 4;
                    case Rank.Queen: return 3;
                    case Rank.Jack: return 2;
                    default: return 0;
                }
            }
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }

}
