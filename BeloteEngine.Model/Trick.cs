using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model
{
    public class Trick
    {
        /// <summary>
        /// List of tuples associating each player with the card they played.
        /// </summary>
        public List<(Player Player, Card Card)> PlayedCards { get; private set; } = new List<(Player, Card)>();

        /// <summary>
        /// The trump suit for this trick, if applicable.
        /// </summary>
        public Suit Trump { get; set; }

        public void AddCard(Player player, Card card)
        {
            PlayedCards.Add((player, card));
        }

        /// <summary>
        /// Determines the winner of the trick based on the following rules:
        /// - If one or more trump cards are played, the highest trump wins.
        /// - Otherwise, the highest card among those that follow the lead suit wins.
        /// Comparison is done using Card.GetPoints, which takes the trump suit into account.
        /// </summary>
        public Player DetermineWinner()
        {
            if (PlayedCards == null || PlayedCards.Count == 0)
                throw new InvalidOperationException("No cards have been played in this trick.");

            // La couleur demandée par le premier joueur.
            Suit leadSuit = PlayedCards[0].Card.Suit;

            // Vérifier s'il y a des cartes d'atout jouées.
            var trumpCards = PlayedCards.Where(pc => pc.Card.Suit == this.Trump).ToList();

            IEnumerable<(Player Player, Card Card)> eligibleCards;

            if (trumpCards.Any())
            {
                // Si des atouts ont été joués, on ne considère que ces cartes.
                eligibleCards = trumpCards;
            }
            else
            {
                // Sinon, on ne considère que les cartes de la couleur demandée.
                eligibleCards = PlayedCards.Where(pc => pc.Card.Suit == leadSuit);
            }

            // On détermine le gagnant en sélectionnant la carte ayant le plus de points (en tenant compte de l'atout).
            var winningEntry = eligibleCards.OrderByDescending(pc => pc.Card.GetPoints(this.Trump)).First();

            return winningEntry.Player;
        }
    }
}
