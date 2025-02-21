using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models;
using Newtonsoft.Json;

namespace BusinessLogic.Services
{
    public class GameService : IGameService
    {
        private readonly Game game;
        private static readonly Random random = new Random();
        private readonly IPlayerService _playerService;

        public GameService(Game gameInstance, IPlayerService playerService)
        {
            game = gameInstance ?? throw new ArgumentNullException(nameof(gameInstance));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public void SetGame(Player player1, int numberOfPlayers, decimal cost)
        {
            game.GameId = Guid.NewGuid().ToString();
            game.NumberOfPlayers = numberOfPlayers + 1;
            if (game.Players != null)
            {
                game.Players.AddRange(_playerService.CreateCPUPlayers(numberOfPlayers, cost));
            }
            game.NumberOfTickets = 0;
            game.Tickets = new List<Ticket>();
            foreach (Player player in game.Players)
            {
                game.NumberOfTickets += player.NumberOfTickets;
                game.Tickets.AddRange(player.Tickets);
            }
            game.TotalAmount = game.NumberOfTickets * cost;
            SetPrizes();
        }

        public void SetPrizes()
        {
            game.GrandPrizeAmount = (int)(0.5m * game.TotalAmount);
            game.SecondTierAmount = (int)(0.3m * game.TotalAmount);
            game.ThirdTierAmount = (int)(0.1m * game.TotalAmount);
            game.noOfGrandPrize = 1;
            game.noOfSecondTier = (int)Math.Round(0.1 * game.NumberOfTickets);
            game.noOfThirdTier = (int)Math.Round(0.2 * game.NumberOfTickets);
            game.NetGrandPrize = game.noOfGrandPrize > 0 ? Math.Round(game.GrandPrizeAmount / game.noOfGrandPrize, 2) : 0;
            game.NetSecondTier = game.noOfSecondTier > 0 ? Math.Round(game.SecondTierAmount / game.noOfSecondTier, 2) : 0;
            game.NetThirdTier = game.noOfThirdTier > 0 ? Math.Round(game.ThirdTierAmount / game.noOfThirdTier, 2) : 0;
            game.HouseProfit = game.TotalAmount - (game.GrandPrizeAmount + (game.NetSecondTier * game.noOfSecondTier) + (game.NetThirdTier * game.noOfThirdTier));
        }

        public void PlayGame()
        {
            // Pick winners
            PickWinners(game.noOfGrandPrize, 1, game.NetGrandPrize);
            PickWinners(game.noOfSecondTier, 2, game.NetSecondTier);
            PickWinners(game.noOfThirdTier, 3, game.NetThirdTier);

            // Ensure remaining revenue (due to rounding errors) is accounted in house profit
            decimal totalDistributedPrize = game.Tickets
                .Where(t => t.PrizeTier != null)
                .Sum(t => t.PrizeTier == 1 ? game.NetGrandPrize
                     : t.PrizeTier == 2 ? game.NetSecondTier
                     : t.PrizeTier == 3 ? game.NetThirdTier
                     : 0);

            game.HouseProfit = game.TotalAmount - totalDistributedPrize;
            game.CumulativeHouseProfit += game.HouseProfit;
            game.CumulativeRevenue += game.TotalAmount;
        }

        private void PickWinners(int numberOfItemsToPick, int prize, decimal prizeAmount)
        {
            List<Ticket> selectedItems = PickRandomItemsWithoutPrize(game.Tickets, numberOfItemsToPick);
            foreach (Ticket ticket in selectedItems)
            {
                ticket.PrizeTier = prize;
                Player player = game.Players.FirstOrDefault(x => x.PlayerId == ticket.PlayerId);
                if (player != null)
                {
                    player.Prizes ??= new List<string>();
                    player.Prizes.Add(prize.ToString());
                    player.AmountWon += prizeAmount;
                }
            }
        }

        private List<Ticket> PickRandomItemsWithoutPrize(List<Ticket> tickets, int numberOfItemsToPick)
        {
            List<Ticket> itemsWithoutPrize = tickets.Where(i => i.PrizeTier == null).ToList();
            if (itemsWithoutPrize.Count == 0)
            {
                Console.WriteLine("No available tickets for selection.");
                return new List<Ticket>();
            }

            // Fisher-Yates shuffle using a single random instance
            int n = itemsWithoutPrize.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (itemsWithoutPrize[i], itemsWithoutPrize[j]) = (itemsWithoutPrize[j], itemsWithoutPrize[i]);
            }

            return itemsWithoutPrize.Take(Math.Min(numberOfItemsToPick, itemsWithoutPrize.Count)).ToList();
        }

        public void ResetGame(Player player1, decimal cost)
        {
            game.NumberOfTickets = 0;
            game.Tickets = new List<Ticket>();

            foreach (Player player in game.Players)
            {
                if (player.PlayerId != 1) // Don't reset human player's tickets
                {
                    player.Tickets = new List<Ticket>();

                    if (player.Balance > 0)
                    {
                        int ticketCount = random.Next(1, (int)player.Balance);
                        player.Tickets = _playerService.SetTicketsForPlayer(player, ticketCount, cost);
                        player.Balance -= ticketCount * cost;
                        player.NumberOfTickets = player.Tickets.Count;
                    }
                }
                game.NumberOfTickets += player.NumberOfTickets;
                game.Tickets.AddRange(player.Tickets);
            }

            game.TotalAmount = game.NumberOfTickets * cost;
            SetPrizes();
        }
    }
}
