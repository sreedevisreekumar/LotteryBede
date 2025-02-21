using System;
using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LotteryGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure Dependency Injection
            var serviceProvider = new ServiceCollection()
                .AddLogging(config =>
                {
                    config.AddConsole();  // Logs to Console
                    config.SetMinimumLevel(LogLevel.Information); // Log Level
                })
                .AddSingleton<Game>()
                .AddSingleton<IGameService, GameService>()
                .AddSingleton<IPlayerService, PlayerService>()
                .AddSingleton<IDisplayService, DisplayService>()
                .BuildServiceProvider();

            // Resolve Logger
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Bede Lottery Program Started.");

                // Resolve Services
                var gameService = serviceProvider.GetRequiredService<IGameService>();
                var playerService = serviceProvider.GetRequiredService<IPlayerService>();
                var displayService = serviceProvider.GetRequiredService<IDisplayService>();

                Console.WriteLine("Welcome to the Bede Lottery, Player1!");
                const decimal TicketPrice = 1.00M;
                decimal startingBalance = 10.00M;
                int minNumberOfTickets = 1;
                int maxNumberOfTickets = 10;
                int minNumberOfCPUPlayers = 9;
                int maxNumberOfCPUPlayers = 15;

                Console.WriteLine($"Your digital balance: ${startingBalance}");
                Console.WriteLine($"Ticket price: ${TicketPrice} each");

                string userInput = "y";
                Random random = new Random();
                bool initial = true; // Set for the first run

                do
                {
                    Console.WriteLine("How many tickets do you want to buy, Player1?");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number) && number >= minNumberOfTickets && number <= maxNumberOfTickets)
                    {
                        if (startingBalance < number * TicketPrice)
                        {
                            logger.LogWarning("Insufficient balance. Player attempted to buy {number} ticket(s) but has only ${startingBalance}.", number, startingBalance);
                            Console.WriteLine("Insufficient balance! Try a lower number of tickets.");
                            continue;
                        }

                        logger.LogInformation($"Player chose to buy {number} ticket(s).", number);

                        // Set game with random CPU players (9-14)
                        int numberOfCPUPlayers = random.Next(minNumberOfCPUPlayers, maxNumberOfCPUPlayers);
                        if (initial)
                        {
                            logger.LogInformation($"Initializing new game with {numberOfCPUPlayers} CPU players.", numberOfCPUPlayers);
                            Player player1 = playerService.CreateHumanPlayer(number, startingBalance, TicketPrice);
                            gameService.SetGame(player1, numberOfCPUPlayers, TicketPrice);
                        }
                        else
                        {
                            logger.LogInformation("Resetting game for replay.");
                            Player player1 = playerService.ResetHumanPlayer(number, TicketPrice);
                            gameService.ResetGame(player1, TicketPrice);
                        }

                        // Play the game
                        gameService.PlayGame();

                        // Display results
                        displayService.DisplayResults();

                        // Deduct ticket cost
                        startingBalance -= number * TicketPrice;
                        logger.LogInformation($"New player balance after ticket purchase: ${startingBalance}.", startingBalance);

                        // Reset tickets
                        initial = false;
                    }
                    else
                    {
                        logger.LogWarning("Invalid input detected for ticket purchase.");
                        Console.WriteLine($"Invalid input! Please enter a number between {minNumberOfTickets} and {maxNumberOfTickets}.");
                    }

                    // Ensure valid input
                    do
                    {
                        Console.WriteLine("Do you want to continue? (y/n): ");
                        userInput = Console.ReadLine()?.Trim().ToLower();

                        if (userInput != "y" && userInput != "n")
                        {
                            Console.WriteLine("Invalid input! Please enter 'y' for Yes or 'n' for No.");
                        }
                    } while (userInput != "y" && userInput != "n");

                } while (userInput == "y");  // Correct exit condition

                logger.LogInformation("Program terminated successfully.");
                Console.WriteLine("Program terminated.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in the program.");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
