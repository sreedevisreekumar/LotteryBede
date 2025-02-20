using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
   public interface IPlayerService
    {
        Player CreatePlayer1(int numberOfTickets, decimal maxamount);
        List<Ticket> SetTicketsForPlayer(Player player, int numberOfTickets);
        Player ResetPlayer1(int number);
        List<Player> CreateCPUPlayers(int numberOfPlayers);
    }
}
