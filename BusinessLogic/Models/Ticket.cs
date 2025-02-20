using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Models
{
   public class Ticket
    {
        public Guid TicketId { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int? PrizeTier { get; set; }
        public int PlayerId { get; set; }
        public Ticket()
        {

        }
    }
}
