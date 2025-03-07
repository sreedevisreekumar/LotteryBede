﻿# Bede Lottery Game

## Project Overview
A simple console-based lottery simulation builtin **C# .NET Core** demonstrating **Dependency Injection (DI)**, **Logging**, and **Unit Testing**.

## How to Run
1️ Install dependencies  
2️ Build and run the project  
3️ Play the game 

##  Features

Players can buy 1 to 10 tickets per round(configurable).

Game initializes with a human player and random number of CPU players (10-15)(configurable).

A ticket draw determines the winners for different prize tiers.

Balance updates after each round. Cumulative Total Revenue, House Profit, Amount Revenue are calculated.

Guaranteed uniqueness for ticket numbers.

Displays house profit and total revenue.

Implements logging for game actions and errors.

The cost of a ticket can be configurable as a constant in the program.cs

# Readability
1. Code is well structured with formatting, comments
2. Logically grouped into classes, interfaces, services
3. Sufficient comments and meaningful naming conventions
# Testability
1. 20 unit tests following proper naming conventions covering important logic
2. Proper logging, and comments to user.
# Extensibility
1. As modelled using Dependency Injection, services can be extended easily.
	I started with a single game mode and it was easy to incorporate multiple rounds of play and cumulative calculation as the classes were correctly specified.
2. Player can be allowed to increase their balance, pricing criteria can be changed
3. Database integration can be added later easily
# Configurability
1. Criteria like number of players, number of tickets, and costs are already configurable other things like prize criteria can be added easily.

## Technologies Used

C# (.NET Core)

Dependency Injection (Microsoft.Extensions.DependencyInjection)

Modular architecture

Logging (Microsoft.Extensions.Logging)

Unit Testing (xUnit, Moq)

JSON Serialization (Newtonsoft.Json)

## How to Play

The game starts with a balance of $10.00.

The player can purchase tickets (1-10) for $1.00 each.

A lottery draw takes place, determining winners.

The player's balance is updated, and they can choose to play another round. Total revenue, House profit, and each player balance would keep accumulating. The player can see his net win at the end.

Game results are displayed.

The game continues until the player chooses to exit. All boundary conditions are checked and invalid inputs won't cause program to crash.

## Unit Testing

The project includes unit tests to verify core functionalities:

Displaying results correctly (DisplayResults_ShouldOutputCorrectResults)

Ensuring balance is deducted properly after multiple rounds (Replay_ShouldDeductCorrectBalance)
