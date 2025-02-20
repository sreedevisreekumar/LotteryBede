using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LotteryGame
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Configure Dependency Injection
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<Game>()
                    .AddSingleton<IGameService, GameService>()
                    .BuildServiceProvider();

                // Resolve the service
                var gameService = serviceProvider.GetRequiredService<IGameService>();

                Console.WriteLine("Welcome to the Bede Lottery, Player1!");
                const decimal TicketPrice = 1.00M;  // Use constant for ticket price
                decimal startingBalance = 10.00M;

                Console.WriteLine($"Your digital balance: ${startingBalance}");
                Console.WriteLine($"Ticket price: ${TicketPrice} each");

                string userInput = "y";
                Random random = new Random(); // Move outside loop
                bool initial = true;

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
                            Player player1 = gameService.CreatePlayer1(number, startingBalance);
                            gameService.SetGame(player1, numberOfCPUPlayers);
                        }
                        else
                        {
                            //reset game
                            Player player1 = gameService.ResetPlayer1(number);
                            gameService.ResetGame(player1);
                        }

                        // Play the game
                        gameService.PlayGame();

                        // Display results
                        gameService.DisplayResults();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
