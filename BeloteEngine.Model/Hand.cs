using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model
{
    public class Hand
    {
        public List<Card> CardsInHand { get; private set; } = new List<Card>();

        public void AddCard(Card card)
        {
            CardsInHand.Add(card);
        }

        public void RemoveCard(Card card)
        {
            CardsInHand.Remove(card);
        }

        public override string ToString()
        {
            return string.Join(", ", CardsInHand.Select(c => c.ToString()));
        }
    }
}
