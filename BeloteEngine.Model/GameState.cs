using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model
{
    public class GameState
    {
        /// <summary>
        /// History of completed tricks.
        /// </summary>
        public List<Trick> CompletedTricks { get; set; } = new List<Trick>();

        /// <summary>
        /// The trick that is currently in progress.
        /// </summary>
        public Trick CurrentTrick { get; set; }

        /// <summary>
        /// The trump suit for the game.
        /// </summary>
        public Suit Trump { get; set; }

        // Additional properties (such as scores, remaining cards, etc.) can be added here.
    }
}
