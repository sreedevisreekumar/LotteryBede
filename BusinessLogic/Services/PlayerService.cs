using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Services
{
   public class PlayerService:IPlayerService
    {
        private readonly Game game;
        private static readonly Random random = new Random();
        public PlayerService(Game gameInstance)
        {
            game = gameInstance ?? throw new ArgumentNullException(nameof(gameInstance));
        }
        public Player CreateHumanPlayer(int numberOfTickets, decimal maxamount)
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
        public List<Player> CreateCPUPlayers(int numberOfPlayers)
        {
            return GeneratePlayers(2, numberOfPlayers);
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
        public Player ResetHumanPlayer(int number)
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
