using BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface IGameService
    {
        Player CreatePlayer1(int numberOfTickets, decimal maxamount);
        List<Ticket> SetTicketsForPlayer(Player player, int numberOfTickets);
        void SetGame(Player player1, int numberOfPlayers);
        void PlayGame();
        void DisplayResults();
        void ResetGame(Player player);
        Player ResetPlayer1(int number);
    }
}
