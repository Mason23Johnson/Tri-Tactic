Tri-Tactic is a console-based Tic Tac Toe variant built with C#. 
Each player can only have three pieces on the board at once. Placing a fourth piece automatically removes that player’s oldest piece, creating an infinite game until someone wins!

An infinite and tactical version of Tic-Tac-Toe with only 3 pieces at a time.
      3 Pieces, 3 Rules:
    1) X always goes first.
    2) Each player can have only 3 pieces on the board at once.
        - If you place a 4th piece, your oldest piece will be removed.
    3) The board never fills up, so the game continues until someone lines up 3 in a row!

  **Why the 3-Piece Mechanic?**
Standard Tic Tac Toe quickly ends in a tie with moderate skill.
By limiting each player to just 3 pieces, the board is never fully “clogged,” so the match continues indefinitely until a line of 3 is formed.

  **How to download?**
In the folder where you opened this README.md, click the TriTactic.exe and download the .exe file to your computer.
Open the .exe file and continue to play the game.
  - If prompted the program is not safe, select 'More Info' and 'Run Anyway'

**Engineering Domain{**
  Board Representation:
A 1D array boardState[9] storing "X", "O", or " " for each of the 9 positions.
A 2D array displayBoard[5, 5] for printing a user-friendly grid (numbers, lines, etc.).

  **Limited Piece Mechanic:**
Lists: xPositions and oPositions track each player’s occupied indices in chronological order.
If the player tries to place a 4th piece, we remove the first (oldest) index from that list and revert the board display at that position.

  **Computer AI:**
Checks if it can win immediately.
If not, tries to block the player’s winning move.
Otherwise, places a piece randomly in an available spot.
}

License: This project is licensed under the [MIT License](LICENSE).
	More info in License.txt can be found in the TriTactic folder.
