using BeloteEngine;
using BeloteEngine.Model;
using BeloteEngine.Model.Strategy;
using System.Text.RegularExpressions;

class Program
    {
        static void Main(string[] args)
        {
        // Création de 4 joueurs avec leur équipe et une stratégie (ici RandomStrategy)
        // On suppose : Player 1 et Player 3 → Team 1, Player 2 et Player 4 → Team 2.
        List<Player> players = new List<Player>
            {
                new Player(1, 1, new RandomStrategy()),
                new Player(2, 2, new RandomStrategy()),
                new Player(3, 1, new RandomStrategy()),
                new Player(4, 2, new RandomStrategy())
            };

        // Définir le seuil de points pour remporter le match (ex. 501 points, paramétrable)
        int pointsThreshold = 501;

        // Création et lancement du match
        var match = new MatchEngine(players, pointsThreshold);
        match.PlayMatch();

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }
}
