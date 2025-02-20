# Bede Lottery Game

## Project Overview
A simple console-based lottery simulation built in **C# .NET Core** demonstrating **Dependency Injection (DI)**, **Logging**, and **Unit Testing**.

## How to Run
1️ Install dependencies  
2️ Build and run the project  
3️ Play the game 

##  Features

Players can buy 1 to 10 tickets per round.

Game initializes with a human player and random number of CPU players (10-15).

A ticket draw determines the winners for different prize tiers.

Balance updates after each round.

Displays house profit and total revenue.

Implements Dependency Injection (DI) for modular architecture.

Implements logging for game actions and errors.
Cost of ticket can be configurable as a constant in the program.cs

## Technologies Used

C# (.NET Core)

Dependency Injection (Microsoft.Extensions.DependencyInjection)

Logging (Microsoft.Extensions.Logging)

Unit Testing (xUnit, Moq)

JSON Serialization (Newtonsoft.Json)

## How to Play

The game starts with a balance of $10.00.

The player can purchase tickets (1-10) for $1.00 each.

A lottery draw takes place, determining winners.

The player's balance is updated, and they can choose to play another round.

Game results are displayed.

The game continues until the player chooses to exit.

## Unit Testing

The project includes unit tests to verify core functionalities:

Displaying results correctly (DisplayResults_ShouldOutputCorrectResults)

Ensuring balance is deducted properly after multiple rounds (Replay_ShouldDeductCorrectBalance)