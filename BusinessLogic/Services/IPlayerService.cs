using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
   public interface IPlayerService
    {
        Player CreateHumanPlayer(int numberOfTickets, decimal maxamount,decimal cost);
        List<Ticket> SetTicketsForPlayer(Player player, int numberOfTickets,decimal cost);
        Player ResetHumanPlayer(int number,decimal cost);
        List<Player> CreateCPUPlayers(int numberOfPlayers,decimal cost);
    }
}
