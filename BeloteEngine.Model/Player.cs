using BeloteEngine.Model.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model
{
    public class Player
    {
        public int Id { get; private set; }
        public int Team { get; set; }  // 1 ou 2
        public Hand Hand { get; set; } = new Hand();
        public IStrategy Strategy { get; set; }

        public Player(int id, int team, IStrategy strategy)
        {
            Id = id;
            Team = team;
            Strategy = strategy;
        }

        /// <summary>
        /// Utilise la stratégie pour choisir la carte à jouer, en se limitant aux cartes autorisées.
        /// </summary>
        public Card ChooseCard(GameState state, List<Card> allowedCards)
        {
            return Strategy.DecideCard(this, state, allowedCards);
        }
    }
}
