using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogic.Models;
using Newtonsoft.Json;

namespace BusinessLogic.Services
{
    public class GameService : IGameService
    {
        private readonly Game game;
        private static readonly Random random = new Random();
        public GameService(Game gameInstance)
        {
            game = gameInstance ?? throw new ArgumentNullException(nameof(gameInstance));
        }
        public List<Player> CreateCPUPlayers(int numberOfPlayers)
        {
         return  GeneratePlayers(2, numberOfPlayers);
        }
        public Player CreatePlayer1(int numberOfTickets, decimal maxamount)
        {
            Console.WriteLine($"Player 1 with {numberOfTickets}");
            Player player1 = new Player();
            player1.PlayerId = 1;
            player1.Name = "Player" + player1.PlayerId;
            player1.Balance = maxamount;
            if (numberOfTickets <= player1.Balance)
            {
                player1.NumberOfTickets = numberOfTickets;
                player1.Tickets = SetTicketsForPlayer(player1, numberOfTickets);
                player1.Balance = player1.Balance - numberOfTickets;
                
            }
            else
            {
                Console.WriteLine($"Invalid input! You dont have enough balance to buy {numberOfTickets} tickets.");
            }
            game.Players.Add(player1);
            return player1;
        }
        public void SetGame(Player player1, int numberOfPlayers)
        {
            
            game.GameId = Guid.NewGuid().ToString();
            game.NumberOfPlayers = numberOfPlayers + 1;
            if (game.Players != null)
            {
                game.Players.AddRange(CreateCPUPlayers(numberOfPlayers));
            }
            game.NumberOfTickets = 0;
            game.Tickets = new List<Ticket>();
            foreach (Player player in game.Players)
            {
                game.NumberOfTickets += player.NumberOfTickets;
                game.Tickets.AddRange(player.Tickets);
            }
            game.TotalAmount = game.NumberOfTickets;
            SetPrizes();
           
        }
        public void SetPrizes()
        {
            
            game.GrandPrizeAmount =(int)(0.5m * game.TotalAmount);
            game.SecondTierAmount = (int)(0.3m * game.TotalAmount);
            game.ThirdTierAmount =(int)(0.1m * game.TotalAmount);
            game.noOfGrandPrize = 1;
            game.noOfSecondTier = (int)Math.Round(0.1 * game.NumberOfTickets);
            game.noOfThirdTier = (int)Math.Round(0.2 * game.NumberOfTickets);
            game.NetGrandPrize = game.noOfGrandPrize > 0?  Math.Round(game.GrandPrizeAmount / game.noOfGrandPrize,2):0;
            game.NetSecondTier = game.noOfSecondTier > 0 ? Math.Round(game.SecondTierAmount / game.noOfSecondTier,2):0;
            game.NetThirdTier = game.noOfThirdTier > 0 ? Math.Round(game.ThirdTierAmount / game.noOfThirdTier,2):0;
            game.HouseProfit = game.TotalAmount - (game.GrandPrizeAmount + (game.NetSecondTier* game.noOfSecondTier) + (game.NetThirdTier* game.noOfThirdTier));
           
        }
        public List<Ticket> SetTicketsForPlayer(Player player, int numberOfTickets)
        {
            List<Ticket> tickets = game?.Tickets;
            tickets = GenerateTickets(numberOfTickets, player);
            return tickets;
        }
        static List<Ticket> GenerateTickets(int count, Player player)
        {
            List<Ticket> objectList = new List<Ticket>();

            for (int i = 0; i < count; i++)
            {
                objectList.Add(new Ticket { TicketId = Guid.NewGuid(), Name = $"Ticket_{player.PlayerId}_{i}", PlayerId = player.PlayerId, Cost = 1 });
            }

            return objectList;
        }
        public List<Player> GeneratePlayers(int startId, int count)
        {
            List<Player> objectList = new List<Player>();

            for (int i = 0; i < count; i++)
            {
                
                Player player1 = new Player { PlayerId = startId + i, Name = $"Player{startId + i}", Balance = 10.00M };
                if (player1.Balance > 0)
                {
                    int randomNumber = random.Next(1, (int)player1.Balance);
                    player1.Tickets = SetTicketsForPlayer(player1, randomNumber);
                    player1.Balance -= randomNumber;
                    player1.NumberOfTickets = player1.Tickets.Count;
                }

                objectList.Add(player1);
            }

            return objectList;
        }
        public void PlayGame()
        {
            //pick up grand,second,thirdprizes , set pricing in tickets
          
            //pick grandprize1 item          
            PickWinners(game, game.noOfGrandPrize, 1, game.GrandPrizeAmount);

            //pick second tier
            PickWinners(game, game.noOfSecondTier, 2, game.SecondTierAmount);

            //pick third tier
            PickWinners(game, game.noOfThirdTier, 3, game.ThirdTierAmount);

            // Ensure remaining revenue (due to unclaimed or rounding errors) goes to house profit
            decimal totalDistributedPrize = game.Tickets
                .Where(t => t.PrizeTier != null)
                .Sum(t => t.PrizeTier == 1 ? game.NetGrandPrize
                     : t.PrizeTier == 2 ? game.NetSecondTier
                     : t.PrizeTier == 3 ? game.NetThirdTier
                     : 0);

            game.HouseProfit = game.TotalAmount - totalDistributedPrize;

        }
        private void PickWinners(Game game, int numberOfItemsToPick, int prize, decimal prizeAmount)
        {
            // Pick random items
            List<Ticket> selectedItems = PickRandomItemsWithoutPrize(game.Tickets, numberOfItemsToPick);
            foreach (Ticket ticket in selectedItems)
            {
                ticket.PrizeTier = prize;
                Player player = game.Players.FirstOrDefault(x => x.PlayerId == ticket.PlayerId);
                if (player != null)
                {
                    if(player.Prizes == null)
                    {
                        player.Prizes = new List<string>();
                    }
                    player.Prizes.Add(prize.ToString());
                    player.AmountWon += prizeAmount;
                }

            }
        }
        private List<Ticket> PickRandomItemsWithoutPrize(List<Ticket> tickets, int numberOfItemsToPick)
        {
            // Filter items with no prize
            List<Ticket> itemsWithoutPrize = tickets.Where(i => i.PrizeTier == null).ToList();

            // Check if we have enough items to pick
            if (itemsWithoutPrize.Count == 0)
            {
                Console.WriteLine("Required number of items not available");
                return new List<Ticket>();
            }

            // Apply Fisher-Yates Shuffle (Better randomization)
            Random rng = new Random();
            int n = itemsWithoutPrize.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (itemsWithoutPrize[i], itemsWithoutPrize[j]) = (itemsWithoutPrize[j], itemsWithoutPrize[i]);
            }

            // Pick the required number of items (or all available if fewer exist)
            return itemsWithoutPrize.Take(Math.Min(numberOfItemsToPick, itemsWithoutPrize.Count)).ToList();
        }
        public void DisplayResults()
        {
            Console.WriteLine("Players with number of tickets");
            Console.WriteLine("=================================");
            foreach (var player1 in game.Players)
            {
                string ticketDisplay = player1.Tickets.Any() ? player1.NumberOfTickets.ToString() : "No Tickets";
                Console.WriteLine($"Player: {player1.PlayerId}, Name: {player1.Name}, Tickets: {ticketDisplay}");
            }
            Console.WriteLine("=================================");

            Console.WriteLine("Ticket Draw Results");
            Console.WriteLine("=================================");

            Ticket ticket1 = game.Tickets.FirstOrDefault(x => x.PrizeTier == 1);
            if (ticket1 != null)
            {
                Player playerG = game.Players.FirstOrDefault(x => x.PlayerId == ticket1.PlayerId);
                Console.WriteLine($"GrandPrize : Player: {playerG.PlayerId} wins ${game.NetGrandPrize}!");
            }
            else
            {
                Console.WriteLine("No Grand Prize winner.");
            }

            DisplayTiers(2,game.NetSecondTier,game.SecondTierAmount);
            DisplayTiers(3,game.NetThirdTier,game.ThirdTierAmount);

            Console.WriteLine("Congratulations to the winners!");

            Console.WriteLine($"House Revenue : ${game.HouseProfit}");

            Console.WriteLine($"Total Revenue : ${game.TotalAmount}");

            Console.WriteLine("=================================");
            string gameString = JsonConvert.SerializeObject(game,Formatting.Indented);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            // Example: 1708251045


            string filePath = $"output_{timestamp}.json"; // File path

            File.WriteAllText(filePath, gameString);

            Console.WriteLine("JSON file written successfully!");
        }
        private void DisplayTiers(int prizeTier,decimal netAmount,decimal tierAmount)
        {
            var playerIds = game.Tickets
             .Where(t => t.PrizeTier == prizeTier)
             .Select(t => t.PlayerId)           
             .OrderBy(id => id) // Sort by Player ID
             .ToList();

            // Display output in the required format
            if (playerIds.Any())
            {
                string playersList = string.Join(",", playerIds);
                Console.WriteLine($"{prizeTier} Tier :{playerIds.Count} Players {playersList} win ${netAmount} ");
            }
            else
            {
                Console.WriteLine($"No players won prize {prizeTier}");
            }
        }
        public void ResetGame(Player player1)
        {
          
            game.NumberOfTickets = 0;
            game.Tickets = new List<Ticket>();
            foreach (Player player in game.Players)
            {
                if (player.PlayerId != 1)
                {
                    player.Tickets = new List<Ticket>();

                    if (player.Balance > 0)
                    {
                        int randomNumber = random.Next(1, (int)player.Balance);
                        player.Tickets = SetTicketsForPlayer(player, randomNumber);
                        player.Balance -= randomNumber;
                        player.NumberOfTickets = player.Tickets.Count;
                    }                  
                }
                game.NumberOfTickets += player.NumberOfTickets;
                game.Tickets.AddRange(player.Tickets);
            }
            game.TotalAmount = game.NumberOfTickets;
            SetPrizes();
        }
        public Player ResetPlayer1(int number)
        {
            Player player1 = game.Players.FirstOrDefault(x => x.PlayerId == 1);
         
            if (number <= player1.Balance)
            {
                player1.NumberOfTickets = number;
                player1.Tickets = SetTicketsForPlayer(player1, number);
                player1.Balance = player1.Balance - number;

            }
            else
            {
                Console.WriteLine($"Invalid input! You dont have enough balance to buy {number} tickets.");
            }
            
            return player1;
        }


    }
}
