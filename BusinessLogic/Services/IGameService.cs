using BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface IGameService
    {
       
        void SetGame(Player player1, int numberOfPlayers,decimal cost);
        void PlayGame();       
        void ResetGame(Player player,decimal cost);
       
    }
}
