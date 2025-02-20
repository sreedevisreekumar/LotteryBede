using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Models
{
   public class Game
    {
        public string GameId { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrandPrizeAmount { get; set; }
        public decimal SecondTierAmount { get; set; }
        public decimal ThirdTierAmount { get; set; }
        public decimal HouseProfit { get; set; }
        public int noOfGrandPrize { get; set; }
        public int noOfSecondTier { get; set; }
        public int noOfThirdTier { get; set; }
        public decimal NetGrandPrize { get; set; }
        public decimal NetSecondTier { get; set; }
        public decimal NetThirdTier { get; set; }

        public List<Ticket> Tickets { get; set; }
        public List<Player> Players { get; set; }
        public Game()
        {
            Players = new List<Player>();
        }

    }
}
