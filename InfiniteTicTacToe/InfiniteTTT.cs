using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    // We track which positions are occupied by X or O in a 9-element array:
    // " " means empty, "X" or "O" means occupied.
    static string[] boardState = new string[9];

    // We track the positions each player has placed *in the order they were placed*.
    // The front of each list is the oldest piece, the back is the newest.
    static List<int> xPositions = new List<int>();
    static List<int> oPositions = new List<int>();

    static void Main()
    {
        while (true)
        {
            PlayGame();

            // Prompt for replay
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

    static void PlayGame()
    {
        // Reset all per-game data
        for (int i = 0; i < boardState.Length; i++)
            boardState[i] = " ";          // all spots empty
        xPositions.Clear();
        oPositions.Clear();

        // Visual board with numbers 1..9
        // We'll overwrite these spots with X/O as we go.
        string[,] displayBoard = new string[,]
        {
            { " 1 ", "|", " 2 ", "|", " 3 " },
            { "---", "+", "---", "+", "---" },
            { " 4 ", "|", " 5 ", "|", " 6 " },
            { "---", "+", "---", "+", "---" },
            { " 7 ", "|", " 8 ", "|", " 9 " }
        };

        // Prompt: choose X or O
        Console.WriteLine("Do you want to play as X or O? (Enter X or O):");
        string player;
        while (true)
        {
            player = Console.ReadLine()?.ToUpper();
            if (player == "X" || player == "O") break;
            Console.WriteLine("Invalid choice. Please enter X or O:");
        }

        // Computer gets the other
        string computer = (player == "X") ? "O" : "X";
        Console.WriteLine($"You are {player}. The computer is {computer}.");
        Console.WriteLine("Press Enter to start the game.");
        Console.ReadLine();

        // Show initial board
        PrintBoard(displayBoard);

        // X always goes first in this setup
        string currentPlayer = "X";

        // Keep going until there's a winner
        while (true)
        {
            if (currentPlayer == player)
            {
                // Player's turn
                Console.WriteLine("Your turn! Choose a position (1-9):");
                int position;
                while (true)
                {
                    // Validate input is an integer in [1..9]
                    if (int.TryParse(Console.ReadLine(), out position) &&
                        position >= 1 && position <= 9)
                    {
                        int index = position - 1;
                        // Check if it's free
                        if (boardState[index] == " ")
                        {
                            // Place the piece
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
                Thread.Sleep(1000); // small delay for realism
                int compIndex = GetComputerMove(currentPlayer, (player == "X" ? "X" : "O"));
                PlacePiece(compIndex, currentPlayer, displayBoard);
            }

            // Show updated board
            PrintBoard(displayBoard);

            // Check if there's a winner
            string winner = CheckWinner();
            if (winner != null)
            {
                Console.WriteLine(winner == "T" ? "It's a tie!" : $"{winner} wins!");
                break;
            }

            // Switch players
            currentPlayer = (currentPlayer == "X") ? "O" : "X";
        }
    }

    /// <summary>
    /// Places a piece for the current player.
    /// If that player already has 3 pieces, remove their oldest first.
    /// </summary>
    static void PlacePiece(int index, string symbol, string[,] displayBoard)
    {
        // If it's X's turn
        if (symbol == "X")
        {
            // If X already has 3 positions, remove the oldest
            if (xPositions.Count == 3)
            {
                int oldestIndex = xPositions[0];
                xPositions.RemoveAt(0);
                // Clear from boardState
                boardState[oldestIndex] = " ";
                // Reset the display to its original number
                ClearDisplaySpot(oldestIndex, displayBoard);
            }
            // Add the new position
            xPositions.Add(index);
            boardState[index] = "X";
            OverwriteDisplaySpot(index, symbol, displayBoard);
        }
        else // It's O's turn
        {
            // If O already has 3 positions, remove the oldest
            if (oPositions.Count == 3)
            {
                int oldestIndex = oPositions[0];
                oPositions.RemoveAt(0);
                // Clear from boardState
                boardState[oldestIndex] = " ";
                // Reset the display to its original number
                ClearDisplaySpot(oldestIndex, displayBoard);
            }
            // Add the new position
            oPositions.Add(index);
            boardState[index] = "O";
            OverwriteDisplaySpot(index, symbol, displayBoard);
        }
    }

    /// <summary>
    /// Clears the board's display spot (overwrites it with the original number).
    /// </summary>
    static void ClearDisplaySpot(int index, string[,] displayBoard)
    {
        // Convert 0..8 => row,col in the 2D display
        int row = (index / 3) * 2;  // 0->0,1->0,2->0,3->2,4->2,5->2,6->4,7->4,8->4
        int col = (index % 3) * 2;  // 0->0,1->2,2->4

        // Original number was index+1
        displayBoard[row, col] = $" {index + 1} ";
    }

    /// <summary>
    /// Places the symbol (X or O) in the display board.
    /// </summary>
    static void OverwriteDisplaySpot(int index, string symbol, string[,] displayBoard)
    {
        int row = (index / 3) * 2;
        int col = (index % 3) * 2;
        displayBoard[row, col] = $" {symbol} ";
    }

    /// <summary>
    /// Computer tries to find a winning move, then a blocking move, then random.
    /// </summary>
    static int GetComputerMove(string computerSym, string playerSym)
    {
        // 1) Try winning
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == " ")
            {
                // Temporarily place
                boardState[i] = computerSym;
                // Also track it in oPositions or xPositions if needed to check winner
                if (computerSym == "X")
                {
                    xPositions.Add(i);
                    if (CheckWinner() == "X")
                    {
                        // revert
                        xPositions.Remove(i);
                        boardState[i] = " ";
                        return i; // found winning spot
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
                        return i; // found winning spot
                    }
                    oPositions.Remove(i);
                }
                // revert
                boardState[i] = " ";
            }
        }

        // 2) Try blocking
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == " ")
            {
                // Temporarily place
                boardState[i] = playerSym;
                if (playerSym == "X")
                {
                    xPositions.Add(i);
                    if (CheckWinner() == "X")
                    {
                        xPositions.Remove(i);
                        boardState[i] = " ";
                        return i; // block
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
                        return i; // block
                    }
                    oPositions.Remove(i);
                }
                // revert
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
    /// Checks if X or O has formed a 3-in-a-row among the current boardState.
    /// Returns "X", "O", or null if no winner. 
    /// 
    /// Since each player can keep removing the oldest piece,
    /// there's technically no "tie" by board fill, 
    /// so if we wanted a "T" for tie, we'd need a different logic. 
    /// We'll just return null if no one has 3 in a row.
    /// </summary>
    static string CheckWinner()
    {
        int[,] combos = new int[,]
        {
            {0,1,2}, {3,4,5}, {6,7,8},
            {0,3,6}, {1,4,7}, {2,5,8},
            {0,4,8}, {2,4,6}
        };

        // Check each combo
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

        // No winner
        return null;
    }
}