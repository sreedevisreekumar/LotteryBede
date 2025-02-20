using BusinessLogic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BusinessLogic.Services
{
    public class DisplayService : IDisplayService
    {
        private readonly Game game;
        private static readonly Random random = new Random();
        public DisplayService(Game gameInstance)
        {
            game = gameInstance ?? throw new ArgumentNullException(nameof(gameInstance));
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

            DisplayTiers(2, game.NetSecondTier, game.SecondTierAmount);
            DisplayTiers(3, game.NetThirdTier, game.ThirdTierAmount);

            Console.WriteLine("Congratulations to the winners!");

            Console.WriteLine($"House Revenue : ${game.HouseProfit}");

            Console.WriteLine($"Total Revenue : ${game.TotalAmount}");

            Console.WriteLine("=================================");
           
        }
        private void DisplayTiers(int prizeTier, decimal netAmount, decimal tierAmount)
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
    }
}
