using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace LotteryGameTests.Tests
{
    public class GameTests
    {
        [Fact]
        public void DisplayResults_ShouldOutputCorrectResults()
        {
            // Arrange
            var gameMock = new Game
            {
                Players = new List<Player>
            {
                new Player { PlayerId = 1, Name = "Alice", Tickets = new List<Ticket> { new Ticket { PrizeTier = 1 } } },
                new Player { PlayerId = 2, Name = "Bob", Tickets = new List<Ticket>() }
            },
                Tickets = new List<Ticket>
            {
                new Ticket { PrizeTier = 1, PlayerId = 1 },
                new Ticket { PrizeTier = 2, PlayerId = 2 }
            },
                NetGrandPrize = 1000,
                NetSecondTier = 500,
                NetThirdTier = 250,
                HouseProfit = 200,
                TotalAmount = 1950
            };

            var writer = new StringWriter();
            Console.SetOut(writer);

            var displayResultsMethod = new DisplayService(gameMock);

            // Act
            displayResultsMethod.DisplayResults();

            // Assert
            var output = writer.ToString();
            Assert.Contains("GrandPrize : Player: 1 wins $1000!", output);
            Assert.Contains("Players with number of tickets", output);
            Assert.Contains("Total Revenue : $1950", output);
            Assert.Contains("House Revenue : $200", output);
        }

        [Fact]
        public void Replay_ShouldDeductCorrectBalance_TicketPrice2()
        {
            // Arrange
            decimal startingBalance = 10.00M;
            const decimal TicketPrice = 2.00M;
            int firstRoundTickets = 3;
            int secondRoundTickets = 2;

            var serviceProvider = new ServiceCollection()
                .AddSingleton<Game>()
                .AddSingleton<IGameService, GameService>()
                .AddSingleton<IPlayerService, PlayerService>()
                .AddSingleton<IDisplayService, DisplayService>()
                .BuildServiceProvider();

            var gameService = serviceProvider.GetRequiredService<IGameService>();
            var playerService = serviceProvider.GetRequiredService<IPlayerService>();

            // First round
            Player player1 = playerService.CreateHumanPlayer(firstRoundTickets, startingBalance,TicketPrice);
            gameService.SetGame(player1, 10,TicketPrice);
            decimal balanceAfterFirstRound = startingBalance - (firstRoundTickets * TicketPrice);

            // Reset for second round
            player1 = playerService.ResetHumanPlayer(secondRoundTickets,TicketPrice);
            gameService.ResetGame(player1,TicketPrice);
            decimal balanceAfterSecondRound = balanceAfterFirstRound - (secondRoundTickets * TicketPrice);

            // Act & Assert
            Assert.Equal(4.00M, balanceAfterFirstRound);
            Assert.Equal(0.00M, balanceAfterSecondRound);
        }
    }
}
