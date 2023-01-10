#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    record Player( string Colour, string Symbol, string Name );
    
    // The `record` is a kind of automatic class in the sense that the compiler generates
    // the fields and constructor and some other things just from this one line.
    // There's a rationale for the capital letters on the variable names (later).
    // For now you can think of it as roughly equivalent to a nonstatic `class` with three
    // public fields and a three-parameter constructor which initializes the fields.
    // It would be like the following. The 'readonly' means you can't change the field value
    // after it is set by the constructor method.
    
    //class Player
    //{
        //public readonly string Colour;
        //public readonly string Symbol;
        //public readonly string Name;
        
        //public Player( string Colour, string Symbol, string Name )
        //{
            //this.Colour = Colour;
            //this.Symbol = Symbol;
            //this.Name = Name;
        //}
    //}
    
    static partial class Program
    {
        // Display common text for the top of the screen.

        static void Welcome( ) 
        { 
            WriteLine();
            Write("***********************************  Welcome to Othello! Good luck and have fun!");
            WriteLine("  ***********************************"); 
            WriteLine();
        }
        
        // Determine which player goes first.
        
        static int GetFirstTurn( Player[ ] players )
        {
            Write("Which player would like to go first? Enter <0> for Player Black ('X') ");
            Write("or <1> for Player White ('O'): ");
            int firstTurn = int.Parse(ReadLine());
            while(firstTurn != 0 && firstTurn != 1)
            {
                Write("Invalid input. Please re-enter your selection of <0> or <1> : ");
                firstTurn = int.Parse(ReadLine());
            }
            
            return firstTurn;
        }
        
        // Get a board size (between 4 and 26 and even).
        
        static int GetBoardSize( string direction )
        {
            Write($"Enter the number of desired board {direction} (4-26, even #): ");
            int boardSize = int.Parse(ReadLine());
            while (boardSize < 4 || boardSize > 26 || boardSize % 2 != 0)
            {
                Write("Invalid board size. Please re-enter a size: ");
                boardSize = int.Parse(ReadLine());
            }
            
            return boardSize;
        }
        
        // Get a move from a player.
        
        static string GetMove( Player player )
        {
            Write($"Player {player.Name}: Enter two letters (row-col) to make a move, ");
            WriteLine("<skip> to skip your turn, or <quit> to quit the game.");
            Write("Enter your choice: ");
            string move = ReadLine();
            
            while(move != "quit" && move != "skip" && move.Length != 2)
            {
                Write("Invalid move. Please re-enter your choice: ");
                move = ReadLine();
            }
            
            if(move == "quit")
                return "quit";
                
            else if(move == "skip")
                return "skip";
            
            else
                return move;
        }
            
        // See if move is valid. Return true if it is.
        
        static bool TryMove( string[ , ] board, Player player, int row, int col)
        {
            //Check if position is within bounds of the board.
            if( row > board.GetLength(0) || col > board.GetLength(1) )
                return false;
            
            //Check if position is available.
            if(board[row, col] == "O" || board[row, col] == "X")
                return false;
                
            //Place piece temporarily at desired position
            board[row, col] = player.Symbol;
            
            //Check if the move is legal in all directions
            int legalCounter = 0; //keeps track of how many legal moves there are
            
            //up
            int deltaRow = -1;
            int deltaCol = 0;
            bool legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //down
            deltaRow = 1;
            deltaCol = 0;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //left
            deltaRow = 0;
            deltaCol = -1;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //right
            deltaRow = 0;
            deltaCol = 1;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //top-left diagonal
            deltaRow = -1;
            deltaCol = -1;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //top-right diagonal
            deltaRow = -1;
            deltaCol = 1;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //bottom-left diagonal
            deltaRow = 1;
            deltaCol = -1;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            //bottom-right diagonal
            deltaRow = 1;
            deltaCol = 1;
            legalMove = TryDirection(board, player, row, col, deltaRow, deltaCol);
            if (legalMove) legalCounter++;
            
            if (legalCounter == 0) {
                board[row, col] = " ";
                return false; //if no directions flipped a piece
            }
            
            return true;
        }
        
        // See if direction is valid and do flips as specified by the row and column delta.
        
        static bool TryDirection( string[ , ] board, Player player, int moveRow, int moveCol,
                                  int deltaRow, int deltaCol )
        {
            //Check if the next row and column are out of bounds
            int nextRow = moveRow + deltaRow;
            if( nextRow < 0 || nextRow >= board.GetLength(0) ) return false;
            int nextCol = moveCol + deltaCol;
            if( nextCol < 0 || nextCol >= board.GetLength(1) ) return false;
            
            //Check if ajacent cell has the player's colour 
            if( board[nextRow, nextCol] == player.Symbol ) return false;
            
            //Check if adjacent cell is empty
            if( board[nextRow, nextCol] == " " ) return false;
            
            bool validMove = true;
            int flips = 2; //counter; 2 because 1 step has already been done above
            
            while( validMove )
            {
                //Can we move another step?
                nextRow = moveRow + (deltaRow * flips);
                if( nextRow < 0 || nextRow >= board.GetLength(0) ) return false;
                nextCol = moveCol + (deltaCol * flips);
                if( nextCol < 0 || nextCol >= board.GetLength(1) ) return false;
                
                //Is the next step empty?
                if( board[nextRow, nextCol] == " " ) return false;
                
                //Is the next step the player's colour? If so, we can flip the opponent pieces now.
                if( board[nextRow, nextCol] == player.Symbol ) 
                {
                    nextRow = moveRow + deltaRow;
                    nextCol = moveCol + deltaCol;
                    
                    for(int i = 1; i <= flips; i++)
                    {
                        board[nextRow, nextCol] = player.Symbol;
                        nextRow = moveRow + (deltaRow * flips);
                        nextCol = moveCol + (deltaCol * flips);
                    }
                    return true;
                }
                flips++;
            }
            return false;
        }
        
        // Count the discs to find the score for a player.
        
        static int GetScore( string[ , ] board, Player player )
        {
            int row = board.GetLength(0);
            int col = board.GetLength(1);
            int score = 0;
            
            for (int i = 0; i < row; i++) {
                for (int  j = 0; j < col; j++ ) {
                    if ( board[i, j] == player.Symbol) score++;
                }
            }
            
            return score;
        }
        
        // Display a line of scores for all players.
        
        static void DisplayScores( string[ , ] board, Player[ ] players )
        {
            WriteLine();
            WriteLine("--------------------------------  SCORES  --------------------------------");
            WriteLine("Player Black: {0}", GetScore(board, players[0]) );
            WriteLine("Player White: {0}", GetScore(board, players[1]) );
            WriteLine("--------------------------------------------------------------------------");
            WriteLine();
        }
        
        // Display winner(s) and categorize their win over the defeated player(s).
        
        static void DisplayWinners( string[ , ] board, Player[ ] players )
        {
            int black = GetScore(board, players[0]);
            int white = GetScore(board, players[1]);
            
            WriteLine();
            Write("******************************************************  WINNER  ");
            WriteLine("*****************************************************");
            WriteLine();
            if(black > white) 
                WriteLine("Player Black wins! Congratulations and better luck next time, White!");
            
            else if (white > black)
                WriteLine("Player White wins! Congratulations and better luck next time, Black!");
            
            else
                WriteLine("It's a tie! Good try to both players! :)");
            WriteLine();
            WriteLine("FINAL SCORES"); WriteLine();
            WriteLine("Player Black: {0}", black);
            WriteLine("Player White: {0}", white);
            WriteLine();
            Write("********************************************  Play again soon! Goodbye!  ");
            WriteLine("********************************************");
        }
        
        static void Main( )
        {
            // Set up the players and game.
            // Note: I used an array of 'Player' objects to hold information about the players.
            // This allowed me to just pass one 'Player' object to methods needing to use
            // the player name, colour, or symbol in 'WriteLine' messages or board operation.
            // The array aspect allowed me to use the index to keep track or whose turn it is.
            // It is also possible to use separate variables or separate arrays instead
            // of an array of objects. It is possible to put the player information in
            // global variables (static field variables of the 'Program' class) so they
            // can be accessed by any of the methods without passing them directly as arguments.
            
            Welcome( );
            
            Player[ ] players = new Player[ ] 
            {
                new Player( Colour: "black", Symbol: "X", Name: "Black" ),
                new Player( Colour: "white", Symbol: "O", Name: "White" ),
            };
            
            int turn = GetFirstTurn( players );
           
            int rows = GetBoardSize( direction: "rows" );
            int cols = GetBoardSize( direction: "columns" );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            
            bool gameOver = false;
            
            while( ! gameOver )
            {
                WriteLine();
                DisplayBoard( game ); 
                DisplayScores( game, players );
                
                string move = GetMove( players[turn] );
                if( move == "quit" ) gameOver = true;
                else if( move == "skip" ) turn = ( turn + 1 ) % players.Length;
                else
                {
                    //Get index values for player's choice of position
                    int moveRow = IndexAtLetter(move.Substring(0,1));
                    int moveCol = IndexAtLetter(move.Substring(1,1));
                    
                    //Check if move is valid
                    bool validMove = TryMove( game, players[turn], moveRow, moveCol );
                    if( validMove ) 
                    {
                        turn = ( turn + 1 ) % players.Length;
                    }
                    else 
                    {
                        Write( "Your choice didn't work!" );
                        Write( " Press <Enter> to try again." );
                        ReadLine( ); 
                    }
                }
            }
            
            // Show the final results.
            
            DisplayWinners( game, players );
            WriteLine( );
        }
    }
}