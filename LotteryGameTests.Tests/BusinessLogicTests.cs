using System;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Services;
using BusinessLogic.Models;

namespace LotteryGameTests.Tests
{
    public class BusinessLogicTests
    {
        private readonly GameService _gameService;
        private readonly Game _game;

        public BusinessLogicTests()
        {
            _game = new Game();
            _gameService = new GameService(_game);
        }
        [Fact]
        public void CreatePlayer1_ShouldCreatePlayerWithCorrectTicketsAndBalance()
        {
            // Arrange
            int numberOfTickets = 5;
            decimal initialBalance = 10.00M;

            // Act
            var player = _gameService.CreatePlayer1(numberOfTickets, initialBalance);

            // Assert
            Assert.NotNull(player);
            Assert.Equal(1, player.PlayerId);
            Assert.Equal("Player1", player.Name);
            Assert.Equal(numberOfTickets, player.NumberOfTickets);
            Assert.Equal(initialBalance - numberOfTickets, player.Balance);
        }

        [Fact]
        public void CreatePlayer1_ShouldNotCreatePlayerIfBalanceIsLow()
        {
            // Arrange
            int numberOfTickets = 15; // More than balance
            decimal initialBalance = 10.00M;

            // Act
            var player = _gameService.CreatePlayer1(numberOfTickets, initialBalance);

            // Assert
            Assert.NotNull(player);
            Assert.Equal(0, player.NumberOfTickets); // No tickets assigned
            Assert.Equal(initialBalance, player.Balance); // Balance remains unchanged
        }

        [Fact]
        public void GeneratePlayers_ShouldCreateCorrectNumberOfPlayers()
        {
            // Arrange
            int numberOfPlayers = 3;

            // Act
            var players = _gameService.CreateCPUPlayers(numberOfPlayers);

            // Assert
            Assert.Equal(numberOfPlayers, players.Count);
            Assert.All(players, p => Assert.NotNull(p.Tickets));
        }

        [Fact]
        public void SetGame_ShouldInitializeGameCorrectly()
        {
            // Arrange
            int numberOfTickets = 5;
            decimal initialBalance = 10.00M;
            int cpuPlayers = 3;
            var player1 = _gameService.CreatePlayer1(numberOfTickets, initialBalance);

            // Act
            _gameService.SetGame(player1, cpuPlayers);

            // Assert
            Assert.Equal(cpuPlayers + 1, _game.NumberOfPlayers); // Player1 + CPU Players
            Assert.True(_game.Players.Count > 1);
            Assert.True(_game.NumberOfTickets > 0);
            Assert.True(_game.Tickets.Count > 0);
        }

        [Fact]
        public void SetPrizes_ShouldCalculatePrizesCorrectly()
        {
            // Arrange
            _game.TotalAmount = 100;
            _game.NumberOfTickets = 100;

            // Act
            _gameService.SetPrizes();

            // Assert
            Assert.Equal(50, _game.GrandPrizeAmount); // 50% of total amount
            Assert.Equal(30, _game.SecondTierAmount); // 30% of total amount
            Assert.Equal(10, _game.ThirdTierAmount);  // 10% of total amount
        }

        [Fact]
        public void PlayGame_ShouldAssignWinners()
        {
            // Arrange
            _game.TotalAmount = 100;
            _game.NumberOfTickets = 100;
            _gameService.SetPrizes();

            var player1 = _gameService.CreatePlayer1(5, 10.00M);
            _gameService.SetGame(player1, 3);

            // Act
            _gameService.PlayGame();

            // Assert
            Assert.True(_game.Tickets.Any(t => t.PrizeTier.HasValue));
            Assert.True(_game.Players.Any(p => p.AmountWon > 0));
        }

        [Fact]
        public void ResetGame_ShouldClearTicketsAndRetainPlayers()
        {
            // Arrange
            var player1 = _gameService.CreatePlayer1(5, 10.00M);
            _gameService.SetGame(player1, 3);

            // Act
            _gameService.ResetGame(player1);

            // Assert
            Assert.True(_game.Tickets.Count > 0);
            Assert.Equal(_game.NumberOfTickets, _game.Tickets.Count);
            Assert.Contains(player1, _game.Players);
        }

        [Fact]
        public void ResetPlayer1_ShouldUpdateTicketCountAndBalance()
        {
            // Arrange
            var player1 = _gameService.CreatePlayer1(5, 10.00M);
            _gameService.SetGame(player1, 3);

            // Act
            var updatedPlayer = _gameService.ResetPlayer1(3);

            // Assert
            Assert.Equal(3, updatedPlayer.NumberOfTickets);
            Assert.Equal(10.00M - 8, updatedPlayer.Balance);
        }

        [Fact]
        public void PickWinners_ShouldDistributePrizesCorrectly()
        {
            // Arrange
            _game.TotalAmount = 100;
            _game.NumberOfTickets = 100;
            _gameService.SetPrizes();

            var player1 = _gameService.CreatePlayer1(5, 10.00M);
            _gameService.SetGame(player1, 3);
            _gameService.PlayGame();

            // Act
            var winners = _game.Tickets.Where(t => t.PrizeTier.HasValue).ToList();

            // Assert
            Assert.NotEmpty(winners);
            Assert.All(winners, w => Assert.True(w.PrizeTier > 0));
        }

        [Theory]
        [InlineData(3)] // Test with 3 CPU players
        [InlineData(5)] // Test with 5 CPU players
        [InlineData(0)] // Test with 0 players (edge case)
        public void CreateCPUPlayers_ShouldCreateSpecifiedNumberOfPlayers(int numberOfPlayers)
        {
            // Act
            List<Player> players = _gameService.CreateCPUPlayers(numberOfPlayers);

            // Assert
            Assert.NotNull(players);
            Assert.Equal(numberOfPlayers, players.Count); // Check if the correct number of players is created

            if (numberOfPlayers > 0)
            {
                Assert.All(players, p => Assert.NotNull(p.Tickets)); // Ensure each player has tickets
                Assert.All(players, p => Assert.True(p.NumberOfTickets > 0)); // Ensure players have at least one ticket
                Assert.All(players, p => Assert.True(p.PlayerId >= 2)); // Ensure player IDs start from 2 (per method logic)
            }
        }

        [Theory]
        [InlineData(3)] // Test with 3 tickets
        [InlineData(5)] // Test with 5 tickets
        [InlineData(0)] // Edge case: 0 tickets
        public void SetTicketsForPlayer_ShouldGenerateCorrectNumberOfTickets(int numberOfTickets)
        {
            // Arrange
            Player player = new Player { PlayerId = 1, Name = "TestPlayer", Balance = 10.00M };

            // Act
            List<Ticket> tickets = _gameService.SetTicketsForPlayer(player, numberOfTickets);

            // Assert
            Assert.NotNull(tickets);
            Assert.Equal(numberOfTickets, tickets.Count); // Ensure the correct number of tickets is generated

            if (numberOfTickets > 0)
            {
                Assert.All(tickets, t => Assert.NotEqual(Guid.Empty, t.TicketId)); // Ensure all tickets have a unique ID
                Assert.All(tickets, t => Assert.Equal(player.PlayerId, t.PlayerId)); // Ensure tickets are assigned to the correct player
                Assert.All(tickets, t => Assert.Contains($"Ticket_{player.PlayerId}_", t.Name)); // Check naming pattern
            }
        }

      

        [Fact]
        public void GeneratePlayers_ShouldAssignTicketsProperly()
        {
            // Arrange
            int startId = 10;
            int count = 3;

            // Act
            List<Player> players = _gameService.GeneratePlayers(startId, count);

            // Assert
            Assert.All(players, player =>
            {
                Assert.InRange(player.Balance, 0, 10.00M); // Balance should be between 0 and 10
                Assert.InRange(player.NumberOfTickets, 1, 9); // Tickets should be at least 1 (based on random logic)
            });
        }

        [Fact]
        public void PlayGame_ShouldExcludePreviouslyWonTicketsFromFurtherPrizeTiers()
        {
            // Arrange
            _game.TotalAmount = 100;
            _game.NumberOfTickets = 10;
            _game.noOfGrandPrize = 1;
            _game.noOfSecondTier = 2;
            _game.noOfThirdTier = 3;
            _game.GrandPrizeAmount = 50;
            _game.SecondTierAmount = 30;
            _game.ThirdTierAmount = 10;

            // Create players
            var player1 = new Player { PlayerId = 1, Name = "Player1", Prizes = new List<string>() };
            var player2 = new Player { PlayerId = 2, Name = "Player2", Prizes = new List<string>() };
            _game.Players.AddRange(new List<Player> { player1, player2 });

            // Create tickets
            var ticketList = new List<Ticket>();
            for (int i = 0; i < 10; i++)
            {
                ticketList.Add(new Ticket { TicketId = Guid.NewGuid(), PlayerId = (i % 2) + 1, PrizeTier = null });
            }
            _game.Tickets = new List<Ticket>();
            _game.Tickets.AddRange(ticketList);

            // Act
            _gameService.PlayGame();

            // Assert
            var grandPrizeWinners = _game.Tickets.Where(t => t.PrizeTier == 1).ToList();
            var secondTierWinners = _game.Tickets.Where(t => t.PrizeTier == 2).ToList();
            var thirdTierWinners = _game.Tickets.Where(t => t.PrizeTier == 3).ToList();

            // Ensure grand prize winner count is correct
            Assert.Equal(1, grandPrizeWinners.Count);

            // Ensure second-tier winners count is correct and that grand prize winners are not repeated
            Assert.Equal(2, secondTierWinners.Count);
            Assert.DoesNotContain(secondTierWinners, t => grandPrizeWinners.Contains(t));

            // Ensure third-tier winners count is correct and that grand/second prize winners are not repeated
            Assert.Equal(3, thirdTierWinners.Count);
            Assert.DoesNotContain(thirdTierWinners, t => grandPrizeWinners.Contains(t));
            Assert.DoesNotContain(thirdTierWinners, t => secondTierWinners.Contains(t));
        }
        [Fact]
        public void PlayGame_ShouldAddRemainingPrizeAmountToHouseProfit()
        {
            // Arrange
            _game.TotalAmount = 100;
            _game.NumberOfTickets = 10;
            _game.noOfGrandPrize = 1;
            _game.noOfSecondTier = 2;
            _game.noOfThirdTier = 3;
            _game.GrandPrizeAmount = 50;
            _game.SecondTierAmount = 30;
            _game.ThirdTierAmount = 10;

            // Expected total prize distribution (should be rounded fairly)
            decimal expectedGrandPrizeSplit = Math.Round(_game.GrandPrizeAmount / _game.noOfGrandPrize, 2);
            decimal expectedSecondTierSplit = Math.Round(_game.SecondTierAmount / _game.noOfSecondTier, 2);
            decimal expectedThirdTierSplit = Math.Round(_game.ThirdTierAmount / _game.noOfThirdTier, 2);

            decimal totalDistributedPrize = (expectedGrandPrizeSplit * _game.noOfGrandPrize)
                                            + (expectedSecondTierSplit * _game.noOfSecondTier)
                                            + (expectedThirdTierSplit * _game.noOfThirdTier);

            decimal expectedHouseProfit = _game.TotalAmount - totalDistributedPrize;

            // Create players
            var player1 = new Player { PlayerId = 1, Name = "Player1", Prizes = new List<string>() };
            var player2 = new Player { PlayerId = 2, Name = "Player2", Prizes = new List<string>() };
            _game.Players.AddRange(new List<Player> { player1, player2 });

            // Create tickets
            var ticketList = new List<Ticket>();
            for (int i = 0; i < 10; i++)
            {
                ticketList.Add(new Ticket { TicketId = Guid.NewGuid(), PlayerId = (i % 2) + 1, PrizeTier = null });
            }
            _game.Tickets = new List<Ticket>();
            _game.Tickets.AddRange(ticketList);

           _game.NetGrandPrize = _game.noOfGrandPrize > 0 ? Math.Round(_game.GrandPrizeAmount / _game.noOfGrandPrize, 2) : 0;
            _game.NetSecondTier = _game.noOfSecondTier > 0 ? Math.Round(_game.SecondTierAmount / _game.noOfSecondTier, 2) : 0;
            _game.NetThirdTier = _game.noOfThirdTier > 0 ? Math.Round(_game.ThirdTierAmount / _game.noOfThirdTier, 2) : 0;
            // Act
            _gameService.PlayGame();

            // Assert
            Assert.Equal(expectedHouseProfit, _game.HouseProfit);
        }
    }
}
