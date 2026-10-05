using System;
using BullsAndCowsApp.Models;

namespace BullsAndCowsApp
{
    /// <summary>
    /// The CLI driver. Talks to the keyboard and screen, and uses the Model classes.
    /// Contains no game rules itself.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine(" BULLS AND COWS (CLI)");
            Console.WriteLine("=======================================");

            // Ask for the player's name
            Console.Write("Enter your name: ");
            string playerName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Player";
            }

            // Choose the difficulty before starting the game
            int difficulty = ChooseDifficulty();

            BullsAndCowsGame game = new BullsAndCowsGame(difficulty);

            // Best score is remembered across all games in this session.
            // A lower number of attempts is a better score.
            int bestScore = 0;

            PrintWelcome(game.CodeLength);

            while (true)
            {
                Console.Write(
                    $"Your guess ({game.CodeLength} digits), " +
                    "'n' = new game, 'd' = difficulty, 'q' = quit: ");

                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (IsQuit(input))
                {
                    Console.WriteLine("Thanks for playing. Goodbye!");
                    break;
                }
                else if (IsNewGame(input))
                {
                    game.StartNewGame();

                    Console.WriteLine("Starting a new game...");
                    continue;
                }
                else if (IsDifficulty(input))
                {
                    difficulty = ChooseDifficulty();

                    game.StartNewGame(difficulty);

                    Console.WriteLine(
                        $"New game started with {difficulty}-digit difficulty.");

                    continue;
                }
                else if (game.IsGameOver)
                {
                    Console.WriteLine(
                        "The game is already over. Type 'n' for a new game or 'q' to quit.");
                    continue;
                }
                else
                {
                    string errorMessage;

                    bool isValid = game.IsValidGuess(input, out errorMessage);

                    if (!isValid)
                    {
                        Console.WriteLine(errorMessage);
                        continue;
                    }

                    GuessResult result = game.SubmitGuess(input);

                    Console.WriteLine(
                        $"-> {result.Bulls} Bulls, {result.Cows} Cows");

                    if (result.IsWinningGuess)
                    {
                        // Check whether this is the best score of the session.
                        if (bestScore == 0 || game.AttemptCount < bestScore)
                        {
                            bestScore = game.AttemptCount;

                            Console.WriteLine(
                                $"*** New best score: {bestScore} attempts! ***");
                        }

                        // Include the player's name in the victory message.
                        Console.WriteLine(
                            $"*** Correct, {playerName}! " +
                            $"You guessed it in {game.AttemptCount} attempts. ***");

                        Console.WriteLine(
                            $"Best score this session: {bestScore} attempts.");

                        Console.WriteLine(
                            "Type 'n' for a new game, 'd' for difficulty, or 'q' to quit.");
                    }
                }
            }
        }

        /// <summary>
        /// Lets the player choose between 4, 5, and 6 digit difficulty.
        /// </summary>
        private static int ChooseDifficulty()
        {
            while (true)
            {
                Console.Write("Choose difficulty (4, 5, or 6 digits): ");
                string input = Console.ReadLine();

                if (input == "4" || input == "5" || input == "6")
                {
                    return int.Parse(input);
                }

                Console.WriteLine("Please choose 4, 5, or 6.");
            }
        }

        /// <summary>
        /// Checks if the player's input matches the quit command (case-insensitive).
        /// </summary>
        private static bool IsQuit(string input)
        {
            string lowered = input.ToLower();

            return lowered == "q"
                || lowered == "quit"
                || lowered == "exit";
        }

        /// <summary>
        /// Checks if the player's input matches the new-game command (case-insensitive).
        /// </summary>
        private static bool IsNewGame(string input)
        {
            string lowered = input.ToLower();

            return lowered == "n"
                || lowered == "new";
        }

        /// <summary>
        /// Checks if the player's input matches the difficulty command.
        /// </summary>
        private static bool IsDifficulty(string input)
        {
            string lowered = input.ToLower();

            return lowered == "d"
                || lowered == "difficulty";
        }

        /// <summary>
        /// Prints the game's title and a short explanation of the rules, once.
        /// </summary>
        private static void PrintWelcome(int codeLength)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"I picked a {codeLength}-digit number with no repeated digits.");

            Console.WriteLine("Bulls = right digit, right position.");
            Console.WriteLine("Cows = right digit, wrong position.");

            Console.WriteLine();
        }
    }
}