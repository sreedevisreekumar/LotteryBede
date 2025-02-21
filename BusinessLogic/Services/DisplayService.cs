using BusinessLogic.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Services
{
    public class DisplayService : IDisplayService
    {
        private readonly Game game;
     

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
                if (playerG != null)
                {
                    Console.WriteLine($"Grand Prize: Player: {playerG.PlayerId} wins ${game.NetGrandPrize} for ticket {ticket1.Name}!");
                   
                }
            }
            else
            {
                Console.WriteLine("No Grand Prize winner.");
               
            }

            DisplayTiers(2, game.NetSecondTier);
            DisplayTiers(3, game.NetThirdTier);
            Console.WriteLine("=================================");

            Console.WriteLine("Congratulations to the winners!");
            Console.WriteLine($"House Revenue: ${game.HouseProfit}");
            Console.WriteLine($"Total Revenue: ${game.TotalAmount}");
            Console.WriteLine("=================================");

            Console.WriteLine("Cumulative Results!");
            Console.WriteLine($"Cumulative House Revenue: ${game.CumulativeHouseProfit}");
            Console.WriteLine($"Cumulative Revenue: ${game.CumulativeRevenue}");

            Player playerHuman = game.Players.FirstOrDefault(x => x.PlayerId == 1);
            Console.WriteLine($"You won: ${playerHuman?.AmountWon ?? 0}!!!");
            Console.WriteLine("=================================");

          
        }

        private void DisplayTiers(int prizeTier, decimal netAmount)
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

