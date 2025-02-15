using BeloteEngine.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine
{
    public class DealEngine
    {
        public List<Player> Players { get; private set; }
        public Deck Deck { get; private set; }
        public List<Trick> Tricks { get; private set; }
        public Suit Trump { get; private set; }
        public GameState State { get; private set; }

        public int StartingIndex { get; set; }

        public DealEngine(List<Player> players, int DraftStartingIndex)
        {
            if (players == null || players.Count != 4)
                throw new ArgumentException("A Belote deal requires exactly 4 players.");
            Players = players;
            Deck = new Deck();
            Tricks = new List<Trick>();
            State = new GameState();
            this.StartingIndex = DraftStartingIndex;
        }

        /// <summary>
        /// Exécute la donne (deal) complète et retourne le résultat (points obtenus par chaque équipe dans cette donne).
        /// </summary>
        public DealResult RunDeal()
        {
            // Initialisation du deck et mélange
            Deck.Initialize();
            Deck.Shuffle();

            // Distribution initiale : 5 cartes par joueur.
            DistributeInitialCards();

            // Phase de draft : révélation et décision sur la candidate card.
            bool contractTaken = PerformDraft();

            if (!contractTaken)
            {
                Console.WriteLine("No player took the contract. The deal is canceled.");
                return null; // Indique que le contrat n'a pas été pris.
            }

            // Distribution finale : compléter la main à 8 cartes.
            FinishDistribution();

            // Jeu de la donne : on joue 8 tricks
            PlayDeal();

            // Calcul du score de la donne et retour du résultat
            return CalculateDealResult();
        }

        /// <summary>
        /// Distribue 5 cartes à chaque joueur.
        /// </summary>
        private void DistributeInitialCards()
        {
            foreach (var player in Players)
            {
                player.Hand = new Hand();
                var fiveCards = Deck.Deal(5);
                foreach (var card in fiveCards)
                {
                    player.Hand.AddCard(card);
                }
            }
            Console.WriteLine("=== Draft Phase: Initial 5 Cards Distribution ===");
            foreach (var player in Players)
            {
                Console.WriteLine($"Player {player.Id} (Team {player.Team}): {player.Hand}");
            }
        }

        /// <summary>
        /// Révèle la candidate card et interroge les joueurs en ordre tournant pour savoir qui l'accepte.
        /// </summary>
        private bool PerformDraft()
        {
            Console.WriteLine($"\nFirst player to talk is Player {Players[StartingIndex].Id}");

            // Révéler la candidate card.
            Card candidateCard = Deck.Deal(1)[0];
            Console.WriteLine($"Candidate Card: {candidateCard}");

            Player draftWinner = null;
            int count = Players.Count;
            // Les joueurs sont interrogés à partir de DraftStartingIndex.
            for (int i = 0; i < count; i++)
            {
                int index = (StartingIndex + i) % count;
                Player player = Players[index];
                if (player.Strategy.AcceptCandidateCard(player, candidateCard, State))
                {
                    draftWinner = player;
                    Console.WriteLine($"Player {player.Id} (Team {player.Team}) accepts the candidate card. Trump suit is {candidateCard.Suit}.");
                    player.Hand.AddCard(candidateCard);
                    break;
                }
            }
            if (draftWinner == null)
            {
                return false;
            }
            // L'atout est défini par la candidate.
            Trump = candidateCard.Suit;
            State.Trump = Trump;
            return true;
        }

        /// <summary>
        /// Distribue les cartes restantes pour que chaque joueur ait 8 cartes en main.
        /// </summary>
        private void FinishDistribution()
        {
            Console.WriteLine("\n=== Finishing Distribution (Completing Hands to 8 Cards) ===");
            foreach (var player in Players)
            {
                int cardsNeeded = 8 - player.Hand.CardsInHand.Count;
                if (cardsNeeded > 0)
                {
                    var additionalCards = Deck.Deal(cardsNeeded);
                    foreach (var card in additionalCards)
                    {
                        player.Hand.AddCard(card);
                    }
                }
                Console.WriteLine($"Player {player.Id} (Team {player.Team}): {player.Hand}");
            }
        }
        private void DealRemainingCards()
        {
            Console.WriteLine("\n=== Dealing Remaining Cards ===");
            // Chaque joueur doit avoir 8 cartes
            foreach (var player in Players)
            {
                int cardsNeeded = 8 - player.Hand.CardsInHand.Count;
                if (cardsNeeded > 0)
                {
                    var additionalCards = Deck.Deal(cardsNeeded);
                    foreach (var card in additionalCards)
                    {
                        player.Hand.AddCard(card);
                    }
                }
                Console.WriteLine($"Player {player.Id} (Team {player.Team}): {player.Hand}");
            }
        }

        private void PlayDeal()
        {
            Console.WriteLine($"\n=== Playing Deal with Trump {Trump} ===");

            for (int trickIndex = 0; trickIndex < 8; trickIndex++)
            {
                Trick trick = new Trick { Trump = State.Trump };
                State.CurrentTrick = trick;
                Console.WriteLine($"\n-- Trick {trickIndex + 1} (Starting with Player {Players[StartingIndex].Id}) --");

                for (int i = 0; i < Players.Count; i++)
                {
                    int currentPlayerIndex = (StartingIndex + i) % Players.Count;
                    Player currentPlayer = Players[currentPlayerIndex];

                    // Calcul de la liste des cartes autorisées (allowedCards) selon la règle de la belote.
                    List<Card> allowedCards = new List<Card>();

                    if (trick.PlayedCards.Count == 0)
                    {
                        // Pas encore de carte jouée dans le trick : aucune contrainte.
                        allowedCards = new List<Card>(currentPlayer.Hand.CardsInHand);
                    }
                    else
                    {
                        // Déterminer la couleur demandée (lead suit)
                        Suit leadSuit = trick.PlayedCards.First().Card.Suit;
                        // Si le joueur a la couleur demandée, il doit la jouer.
                        if (currentPlayer.Hand.CardsInHand.Any(card => card.Suit == leadSuit))
                        {
                            allowedCards = currentPlayer.Hand.CardsInHand
                                .Where(card => card.Suit == leadSuit)
                                .ToList();
                        }
                        // Sinon, s'il a des atouts, il doit couper.
                        else if (currentPlayer.Hand.CardsInHand.Any(card => card.Suit == State.Trump))
                        {
                            // Récupérer tous les atouts en main.
                            allowedCards = currentPlayer.Hand.CardsInHand
                                .Where(card => card.Suit == State.Trump)
                                .ToList();

                            // Si un ou plusieurs atouts ont déjà été joués dans le trick,
                            // vérifier s'il existe des atouts capables de surcouper le plus fort.
                            var trumpPlayed = trick.PlayedCards.Where(pc => pc.Card.Suit == State.Trump).ToList();
                            if (trumpPlayed.Any())
                            {
                                var highestTrump = trumpPlayed
                                    .OrderByDescending(pc => pc.Card.GetPoints(State.Trump))
                                    .First().Card;
                                // Filtrer ceux qui battent le plus fort atout déjà joué.
                                var overtrumpCards = allowedCards
                                    .Where(card => card.GetPoints(State.Trump) > highestTrump.GetPoints(State.Trump))
                                    .ToList();
                                if (overtrumpCards.Any())
                                {
                                    allowedCards = overtrumpCards;
                                }
                                // Sinon, le joueur devra jouer l'un de ses atouts, même s'il ne peut pas surcouper.
                            }
                        }
                        // Sinon, le joueur ne possède ni la couleur demandée ni d'atout.
                        // Il est libre de jouer n'importe quelle carte.
                        else
                        {
                            allowedCards = new List<Card>(currentPlayer.Hand.CardsInHand);
                        }
                    }

                    // La stratégie choisit la carte parmi les allowedCards.
                    Card chosenCard = currentPlayer.ChooseCard(State, allowedCards);
                    currentPlayer.Hand.RemoveCard(chosenCard);
                    trick.AddCard(currentPlayer, chosenCard);
                    Console.WriteLine($"Player {currentPlayer.Id} played: {chosenCard}");
                }

                State.CompletedTricks.Add(trick);
                Tricks.Add(trick);

                Player trickWinner = trick.DetermineWinner();
                Console.WriteLine($"Winner of Trick {trickIndex + 1}: Player {trickWinner.Id}");
                StartingIndex = Players.IndexOf(trickWinner);
            }

        }
        /// <summary>
        /// Calcule le résultat de la donne en attribuant, pour chaque trick, le total des points des cartes jouées 
        /// à l'équipe du joueur ayant remporté le trick.
        /// </summary>
        private DealResult CalculateDealResult()
        {
            // Initialisation des points pour chaque équipe.
            Dictionary<int, int> teamPoints = new Dictionary<int, int> { { 1, 0 }, { 2, 0 } };

            // Pour chaque trick de la donne...
            foreach (var trick in Tricks)
            {
                int trickPoints = 0;
                // On calcule la somme des points de toutes les cartes jouées dans le trick.
                foreach (var played in trick.PlayedCards)
                {
                    trickPoints += played.Card.GetPoints(State.Trump);
                }
                // Le gagnant du trick remporte les points accumulés.
                Player winner = trick.DetermineWinner();
                teamPoints[winner.Team] += trickPoints;
            }

            //Dix de der
            Player lastTrickWinner = Tricks.Last().DetermineWinner();
            teamPoints[lastTrickWinner.Team] += 10;
            Console.WriteLine($"\nDix de der bonus: 10 points awarded to Team {lastTrickWinner.Team}");

            AddBeloteRebeloteBonus(teamPoints);

            return new DealResult(teamPoints);
        }

        /// <summary>
        /// Parcourt les tricks pour vérifier si un joueur a joué le Roi et la Dame de l'atout,
        /// et ajoute un bonus de 20 points à son équipe dans ce cas.
        /// </summary>
        /// <param name="teamPoints">Dictionnaire des points par équipe.</param>
        private void AddBeloteRebeloteBonus(Dictionary<int, int> teamPoints)
        {
            // Dictionnaire pour enregistrer, pour chaque joueur, les cartes d'atout de rang King et Queen jouées.
            Dictionary<Player, HashSet<Rank>> beloteCards = new Dictionary<Player, HashSet<Rank>>();

            foreach (var trick in Tricks)
            {
                foreach (var played in trick.PlayedCards)
                {
                    // Ne considérer que les cartes de la couleur d'atout qui sont King ou Queen.
                    if (played.Card.Suit == State.Trump && (played.Card.Rank == Rank.King || played.Card.Rank == Rank.Queen))
                    {
                        if (!beloteCards.ContainsKey(played.Player))
                        {
                            beloteCards[played.Player] = new HashSet<Rank>();
                        }
                        beloteCards[played.Player].Add(played.Card.Rank);
                    }
                }
            }

            // Pour chaque joueur ayant joué à la fois le King et la Queen, ajouter le bonus.
            foreach (var kvp in beloteCards)
            {
                if (kvp.Value.Contains(Rank.King) && kvp.Value.Contains(Rank.Queen))
                {
                    teamPoints[kvp.Key.Team] += 20;
                    Console.WriteLine($"Belote/Rebelote bonus: 20 points awarded to Team {kvp.Key.Team} (Player {kvp.Key.Id})");
                }
            }
        }
    }
}