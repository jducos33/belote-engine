using BeloteEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model;

public class Deck
{
    public List<Card> Cards { get; private set; } = new List<Card>();

    public Deck()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the deck with all cards.
    /// In belote, typically 32 cards are used (from Seven to Ace).
    /// </summary>
    public void Initialize()
    {
        Cards.Clear();
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                Cards.Add(new Card(suit, rank));
            }
        }
    }

    /// <summary>
    /// Shuffles the deck randomly.
    /// </summary>
    public void Shuffle()
    {
        Random rng = new Random();
        Cards = Cards.OrderBy(c => rng.Next()).ToList();
    }

    /// <summary>
    /// Deals a specified number of cards.
    /// </summary>
    public List<Card> Deal(int cardCount)
    {
        if (cardCount > Cards.Count)
            throw new InvalidOperationException("Not enough cards in the deck.");

        List<Card> hand = Cards.Take(cardCount).ToList();
        Cards.RemoveRange(0, cardCount);
        return hand;
    }
}
