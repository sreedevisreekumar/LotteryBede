using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public int NumberOfTickets { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<string> Prizes { get; set; }
        public decimal AmountWon { get; set; }

        public Player()
        {
            Tickets = new List<Ticket>();
            Prizes = new List<string>();
        }
    }
}
