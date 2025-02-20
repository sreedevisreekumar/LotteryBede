﻿using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;


namespace LotteryGame
{
    class Program
    {
        private static ILogger<Program> _logger;
        static void Main(string[] args)
        {
            try
            {
                // Configure Dependency Injection
                var serviceProvider = new ServiceCollection()
                    .AddLogging(config =>
                    {
                        config.AddConsole();
                        config.SetMinimumLevel(LogLevel.Information);
                    })
                    .AddSingleton<Game>()
                    .AddSingleton<IGameService, GameService>()
                    .AddSingleton<IPlayerService,PlayerService>()
                    .AddSingleton<IDisplayService, DisplayService>()
                    .BuildServiceProvider();
                // Resolve Logger
                _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    _logger.LogInformation("Bede Lottery Program Started.");

                    // Resolve the service
                    var gameService = serviceProvider.GetRequiredService<IGameService>();
                    var playerService = serviceProvider.GetRequiredService<IPlayerService>();
                    var displayService = serviceProvider.GetRequiredService<IDisplayService>();

                    Console.WriteLine("Welcome to the Bede Lottery, Player1!");
                    const decimal TicketPrice = 1.00M;  // Use constant for ticket price
                    decimal startingBalance = 10.00M;

                    Console.WriteLine($"Your digital balance: ${startingBalance}");
                    Console.WriteLine($"Ticket price: ${TicketPrice} each");

                    string userInput = "y";
                    Random random = new Random();
                    bool initial = true;// set for the first run

                    while (userInput == "y")
                    {
                        Console.WriteLine("How many tickets do you want to buy, Player1?");
                        string input = Console.ReadLine();

                        if (int.TryParse(input, out int number) && number >= 1 && number <= 10)
                        {
                            if (startingBalance < number * TicketPrice)
                            {
                                Console.WriteLine("Insufficient balance! Try a lower number of tickets.");
                                Console.WriteLine("Do you want to continue? (y/n): ");
                                userInput = Console.ReadLine()?.Trim().ToLower();
                                continue;
                            }

                            Console.WriteLine($"You entered: {number}");

                            // Set game with random CPU players (9-14)
                            int numberOfCPUPlayers = random.Next(9, 15);
                            if (initial)
                            {
                                // Initialize game
                                Player player1 = playerService.CreateHumanPlayer(number, startingBalance, TicketPrice);
                                gameService.SetGame(player1, numberOfCPUPlayers, TicketPrice);
                            }
                            else
                            {
                                //reset game
                                Player player1 = playerService.ResetHumanPlayer(number, TicketPrice);
                                gameService.ResetGame(player1, TicketPrice);
                            }

                            // Play the game
                            gameService.PlayGame();

                            // Display results
                            displayService.DisplayResults();
                            // Deduct ticket cost
                            startingBalance -= number * TicketPrice;
                            //Reset tickets
                            initial = false;
                            Console.WriteLine($"Remaining balance: ${startingBalance}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input! Please enter a number between 1 and 10.");
                        }

                        Console.WriteLine("Do you want to continue? (y/n): ");
                        userInput = Console.ReadLine()?.Trim().ToLower();
                    }

                    Console.WriteLine("Program terminated.");
                    _logger.LogInformation("Program terminated successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred.,{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
