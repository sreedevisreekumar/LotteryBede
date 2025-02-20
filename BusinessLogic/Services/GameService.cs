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
        private readonly IPlayerService _playerService;
        public GameService(Game gameInstance,IPlayerService playerService)
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
                game.Players.AddRange(_playerService.CreateCPUPlayers(numberOfPlayers,cost));
            }
            game.NumberOfTickets = 0;
            game.Tickets = new List<Ticket>();
            foreach (Player player in game.Players)
            {
                game.NumberOfTickets += player.NumberOfTickets;
                game.Tickets.AddRange(player.Tickets);
            }
            game.TotalAmount = game.NumberOfTickets *cost;
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
       
        public void ResetGame(Player player1,decimal cost)
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
                        player.Tickets = _playerService.SetTicketsForPlayer(player, randomNumber,cost);
                        player.Balance -= randomNumber*cost;
                        player.NumberOfTickets = player.Tickets.Count;
                    }                  
                }
                game.NumberOfTickets += player.NumberOfTickets;
                game.Tickets.AddRange(player.Tickets);
            }
            game.TotalAmount = game.NumberOfTickets;
            SetPrizes();
        }
       


    }
}
