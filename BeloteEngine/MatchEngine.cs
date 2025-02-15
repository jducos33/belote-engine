using BeloteEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine
{
    public class MatchEngine
    {
        public List<Player> Players { get; private set; }
        public int PointsThreshold { get; private set; }
        public Dictionary<int, int> GlobalTeamPoints { get; private set; }
        public List<DealResult> DealResults { get; private set; }

        public MatchEngine(List<Player> players, int pointsThreshold)
        {
            if (players == null || players.Count != 4)
                throw new ArgumentException("A Belote match requires exactly 4 players.");
            Players = players;
            PointsThreshold = pointsThreshold;
            GlobalTeamPoints = new Dictionary<int, int> { { 1, 0 }, { 2, 0 } };
            DealResults = new List<DealResult>();
        }

        public void PlayMatch()
        {
            int dealNumber = 0;
            int currentPlayerStartingIndex = 0;
            while (GlobalTeamPoints[1] < PointsThreshold && GlobalTeamPoints[2] < PointsThreshold)
            {
                dealNumber++;

                Console.WriteLine($"\n=== Starting Deal {dealNumber} ===");
                // Pour chaque donne, on crée un nouvel objet DealEngine
                DealEngine dealEngine = new DealEngine(Players, currentPlayerStartingIndex);
                DealResult result = dealEngine.RunDeal();
                if (result == null)
                {
                    // Aucun contrat n'a été pris.
                    // On incrémente currentDraftStartingIndex et on réessaye.
                    currentPlayerStartingIndex = (currentPlayerStartingIndex + 1) % Players.Count;
                    continue;
                }

                // Si le contrat est pris, on enregistre le résultat.
                DealResults.Add(result);

                // Mise à jour du score global par équipe
                foreach (var kvp in result.TeamPoints)
                {
                    GlobalTeamPoints[kvp.Key] += kvp.Value;
                }

                Console.WriteLine($"Deal {dealNumber} result: Team 1: {result.TeamPoints[1]}, Team 2: {result.TeamPoints[2]}");
                Console.WriteLine($"Global Score: Team 1: {GlobalTeamPoints[1]}, Team 2: {GlobalTeamPoints[2]}");
                currentPlayerStartingIndex = (currentPlayerStartingIndex + 1) % Players.Count;
            }

            // Déclaration de l'équipe gagnante
            if (GlobalTeamPoints[1] >= PointsThreshold)
            {
                Console.WriteLine("\n=== Team 1 wins the match! ===");
            }
            else
            {
                Console.WriteLine("\n=== Team 2 wins the match! ===");
            }
        }
    }
}
