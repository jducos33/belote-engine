using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model.Strategy
{
    public class RandomStrategy : IStrategy
    {
        private Random _random = new Random();


        public Card DecideCard(Player player, GameState state, List<Card> allowedCards)
        {
            // Si une liste de cartes autorisées est fournie et non vide, choisir parmi celles-ci.
            List<Card> choices = allowedCards != null && allowedCards.Any()
                ? allowedCards
                : player.Hand.CardsInHand;

            int index = _random.Next(choices.Count);
            return choices[index];
        }

        public bool AcceptCandidateCard(Player player, Card candidate, GameState state)
        {
            // Exemple de logique : le joueur accepte si sa main contient déjà une carte de la même couleur
            return player.Hand.CardsInHand.Exists(card => card.Suit == candidate.Suit);
        }
    }
}
