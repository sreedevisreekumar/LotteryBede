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
        public Player CreateHumanPlayer(int numberOfTickets, decimal maxamount,decimal cost)
        {
            Console.WriteLine($"Player 1 with {numberOfTickets}");
            Player player1 = new Player();
            player1.PlayerId = 1;
            player1.Name = "Player" + player1.PlayerId;
            player1.Balance = maxamount;
            if (numberOfTickets*cost <= player1.Balance)
            {
                player1.NumberOfTickets = numberOfTickets;
                player1.Tickets = SetTicketsForPlayer(player1, numberOfTickets,cost);
                player1.Balance = player1.Balance - (numberOfTickets*cost);

            }
            else
            {
                Console.WriteLine($"Invalid input! You dont have enough balance to buy {numberOfTickets} tickets.");
            }
            game.Players.Add(player1);
            return player1;
        }
        public List<Ticket> SetTicketsForPlayer(Player player, int numberOfTickets,decimal cost)
        {
            List<Ticket> tickets = game?.Tickets;
            tickets = GenerateTickets(numberOfTickets, player,cost);
            return tickets;
        }
        

        static List<Ticket> GenerateTickets(int count, Player player, decimal cost)
        {
            List<Ticket> objectList = new List<Ticket>();
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int lastFourDigits = (int)(timestamp % 10000);
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                int randomSuffix = random.Next(1000, 9999); // 4-digit random number
                long uniqueTicketNumber = long.Parse($"{lastFourDigits}{randomSuffix}");

                objectList.Add(new Ticket
                {
                    TicketId = Guid.NewGuid(), // Unique Ticket ID
                    Name = uniqueTicketNumber.ToString(),
                    PlayerId = player.PlayerId,
                    Cost = cost
                });
            }

            return objectList;
        }
        public List<Player> CreateCPUPlayers(int numberOfPlayers,decimal cost)
        {
           
            return GeneratePlayers(2, numberOfPlayers,cost);
        }
        public List<Player> GeneratePlayers(int startId, int count,decimal cost)
        {
            List<Player> objectList = new List<Player>();

            for (int i = 0; i < count; i++)
            {

                Player player1 = new Player { PlayerId = startId + i, Name = $"Player{startId + i}", Balance = 10.00M };
                if (player1.Balance > 0)
                {
                    int randomNumber = random.Next(1, (int)player1.Balance);
                    player1.Tickets = SetTicketsForPlayer(player1, randomNumber,cost);
                    player1.Balance -= randomNumber*cost;
                    player1.NumberOfTickets = player1.Tickets.Count;
                }

                objectList.Add(player1);
            }

            return objectList;
        }
        public Player ResetHumanPlayer(int number,decimal cost)
        {
            Player player1 = game.Players.FirstOrDefault(x => x.PlayerId == 1);

            if (number <= player1.Balance)
            {
                player1.NumberOfTickets = number;
                player1.Tickets = SetTicketsForPlayer(player1, number,cost);
                player1.Balance = player1.Balance - (number*cost);

            }
            else
            {
                Console.WriteLine($"Invalid input! You dont have enough balance to buy {number} tickets.");
            }

            return player1;
        }
    }
}
