using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    // We track the occupant of each of the 9 board positions: " " (empty), "X", or "O"
    static string[] boardState = new string[9];

    // We track X's placed positions (0-based) in the order they were placed
    static List<int> xPositions = new List<int>();

    // We track O's placed positions (0-based) in the order they were placed
    static List<int> oPositions = new List<int>();

    static void Main()
    {
        ShowWelcomeMessage(); // Display the welcome and rule explanation

        while (true)
        {
            PlayGame();

            // Ask if the user wants to play again
            Console.WriteLine("Would you like to play again? (y/n):");
            string response;
            while (true)
            {
                response = Console.ReadLine()?.ToLower();
                if (response == "y" || response == "n") break;
                Console.WriteLine("Invalid input. Please enter 'y' or 'n':");
            }
            if (response == "n")
            {
                Console.WriteLine("Thanks for playing! Goodbye!");
                break;
            }
        }
    }

    /// <summary>
    /// Displays a welcome message and explains the rules of 3-Piece Tic-Tac-Toe.
    /// </summary>
    static void ShowWelcomeMessage()
    {
        Console.WriteLine("Welcome to Mason's Tri-Tactic!");
		Console.WriteLine("An infinite and tactical version of Tic-Tac-Toe with only 3 pieces at a time.\n");
        Console.WriteLine("\t3 Pieces, 3 Rules:");
        Console.WriteLine("1) X always goes first.");
        Console.WriteLine("2) Each player can have only 3 pieces on the board at once.");
        Console.WriteLine("   - If you place a 4th piece, your oldest piece will be removed.");
        Console.WriteLine("3) The board never fills up, so the game continues until someone lines up 3 in a row!\n");
    }

    static void PlayGame()
    {
        // Reset all game-specific data
        for (int i = 0; i < boardState.Length; i++)
            boardState[i] = " "; // Make all spots empty again
        xPositions.Clear();
        oPositions.Clear();

        // Create our 2D board display with positions labeled 1..9
        string[,] displayBoard = new string[,]
        {
            { " 1 ", "|", " 2 ", "|", " 3 " },
            { "---", "+", "---", "+", "---" },
            { " 4 ", "|", " 5 ", "|", " 6 " },
            { "---", "+", "---", "+", "---" },
            { " 7 ", "|", " 8 ", "|", " 9 " }
        };

        // Let the user pick whether to play as X or O
        Console.WriteLine("Do you want to play as X or O? (Enter X or O):");
        string player;
        while (true)
        {
            player = Console.ReadLine()?.ToUpper();
            if (player == "X" || player == "O") break;
            Console.WriteLine("Invalid choice. Please enter X or O:");
        }

        // The computer gets whichever symbol the player didn't choose
        string computer = (player == "X") ? "O" : "X";
        Console.WriteLine($"You are {player}. The computer is {computer}.");
        Console.WriteLine("Press Enter to start the game.");
        Console.ReadLine();

        // Display the initial board
        PrintBoard(displayBoard);

        // X always starts
        string currentPlayer = "X";

        while (true)
        {
            if (currentPlayer == player)
            {
                // Player's turn
                Console.WriteLine("Your turn! Choose a position (1-9):");
                int position;
                while (true)
                {
                    // Make sure the user enters a valid number
                    if (int.TryParse(Console.ReadLine(), out position) &&
                        position >= 1 && position <= 9)
                    {
                        int index = position - 1;
                        // Spot must be free
                        if (boardState[index] == " ")
                        {
                            PlacePiece(index, currentPlayer, displayBoard);
                            break;
                        }
                    }
                    Console.WriteLine("Invalid input. Spot is taken or out of range, try again:");
                }
            }
            else
            {
                // Computer's turn
                Console.WriteLine("Computer's turn...");
                Thread.Sleep(1000); // 1-second delay for realism
                int compIndex = GetComputerMove(currentPlayer, player);
                PlacePiece(compIndex, currentPlayer, displayBoard);
            }

            // Show updated board
            PrintBoard(displayBoard);

            // Check for winner
            string winner = CheckWinner();
            if (winner != null)
            {
                Console.WriteLine(winner == "T" ? "It's a tie!" : $"{winner} wins!");
                break;
            }

            // Switch player
            currentPlayer = (currentPlayer == "X") ? "O" : "X";
        }
    }

    /// <summary>
    /// Places a piece for the given player. If that player already has 3 pieces,
    /// remove the oldest one first.
    /// </summary>
    static void PlacePiece(int index, string symbol, string[,] displayBoard)
    {
        // If X's turn
        if (symbol == "X")
        {
            if (xPositions.Count == 3)
            {
                int oldestIndex = xPositions[0];
                xPositions.RemoveAt(0);
                // Clear that spot in boardState
                boardState[oldestIndex] = " ";
                // Restore the original numbered display
                ClearDisplaySpot(oldestIndex, displayBoard);
            }
            xPositions.Add(index);
            boardState[index] = "X";
            OverwriteDisplaySpot(index, symbol, displayBoard);
        }
        else // O's turn
        {
            if (oPositions.Count == 3)
            {
                int oldestIndex = oPositions[0];
                oPositions.RemoveAt(0);
                boardState[oldestIndex] = " ";
                ClearDisplaySpot(oldestIndex, displayBoard);
            }
            oPositions.Add(index);
            boardState[index] = "O";
            OverwriteDisplaySpot(index, symbol, displayBoard);
        }
    }

    /// <summary>
    /// Resets the display position with its original number (index+1).
    /// </summary>
    static void ClearDisplaySpot(int index, string[,] displayBoard)
    {
        int row = (index / 3) * 2; // 0->0,1->0,2->0,3->2,etc.
        int col = (index % 3) * 2; // 0->0,1->2,2->4
        displayBoard[row, col] = $" {index + 1} ";
    }

    /// <summary>
    /// Overwrites the display position with X or O.
    /// </summary>
    static void OverwriteDisplaySpot(int index, string symbol, string[,] displayBoard)
    {
        int row = (index / 3) * 2;
        int col = (index % 3) * 2;
        displayBoard[row, col] = $" {symbol} ";
    }

    /// <summary>
    /// The computer checks for a winning move, then a blocking move, otherwise picks randomly.
    /// </summary>
    static int GetComputerMove(string computerSym, string playerSym)
    {
        // 1) Try to win
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == " ")
            {
                boardState[i] = computerSym;
                if (computerSym == "X")
                {
                    xPositions.Add(i);
                    if (CheckWinner() == "X")
                    {
                        xPositions.Remove(i);
                        boardState[i] = " ";
                        return i; // Winning move
                    }
                    xPositions.Remove(i);
                }
                else
                {
                    oPositions.Add(i);
                    if (CheckWinner() == "O")
                    {
                        oPositions.Remove(i);
                        boardState[i] = " ";
                        return i; // Winning move
                    }
                    oPositions.Remove(i);
                }
                boardState[i] = " ";
            }
        }

        // 2) Try to block
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == " ")
            {
                boardState[i] = playerSym;
                if (playerSym == "X")
                {
                    xPositions.Add(i);
                    if (CheckWinner() == "X")
                    {
                        xPositions.Remove(i);
                        boardState[i] = " ";
                        return i; // Blocking move
                    }
                    xPositions.Remove(i);
                }
                else
                {
                    oPositions.Add(i);
                    if (CheckWinner() == "O")
                    {
                        oPositions.Remove(i);
                        boardState[i] = " ";
                        return i; // Blocking move
                    }
                    oPositions.Remove(i);
                }
                boardState[i] = " ";
            }
        }

        // 3) Otherwise, pick a random free spot
        Random rand = new Random();
        int move;
        do
        {
            move = rand.Next(0, 9);
        } while (boardState[move] != " ");
        return move;
    }

    /// <summary>
    /// Prints the current 2D display board.
    /// </summary>
    static void PrintBoard(string[,] displayBoard)
    {
        Console.Clear();
        for (int i = 0; i < displayBoard.GetLength(0); i++)
        {
            for (int j = 0; j < displayBoard.GetLength(1); j++)
            {
                Console.Write(displayBoard[i, j]);
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Checks the boardState for a winner (3 in a row).
    /// Returns "X", "O", or null if no winner.
    /// We do not track ties since the board never fully fills.
    /// </summary>
    static string CheckWinner()
    {
        int[,] combos = new int[,]
        {
            {0,1,2}, {3,4,5}, {6,7,8},
            {0,3,6}, {1,4,7}, {2,5,8},
            {0,4,8}, {2,4,6}
        };

        for (int i = 0; i < combos.GetLength(0); i++)
        {
            int a = combos[i,0];
            int b = combos[i,1];
            int c = combos[i,2];
            if (boardState[a] != " " &&
                boardState[a] == boardState[b] &&
                boardState[b] == boardState[c])
            {
                return boardState[a]; // "X" or "O"
            }
        }

        // No winner yet
        return null;
    }
}
