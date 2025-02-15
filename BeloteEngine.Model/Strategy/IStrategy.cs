using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model.Strategy
{
    public interface IStrategy
    {
        /// <summary>
        /// Décide quelle carte jouer à partir des cartes autorisées.
        /// </summary>
        /// <param name="player">Le joueur qui doit jouer.</param>
        /// <param name="state">L'état courant du jeu.</param>
        /// <param name="allowedCards">La liste des cartes que le joueur est autorisé à jouer.</param>
        /// <returns>La carte choisie.</returns>
        Card DecideCard(Player player, GameState state, List<Card> allowedCards);

        /// <summary>
        /// Determines whether the player accepts the candidate card during the draft phase.
        /// </summary>
        /// <param name="player">The current player.</param>
        /// <param name="candidate">The candidate card proposed.</param>
        /// <param name="state">The current game state.</param>
        /// <returns>True if the player accepts the candidate card, false otherwise.</returns>
        bool AcceptCandidateCard(Player player, Card candidate, GameState state);
    }

}
