using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TicTacToe
{
    public class GameController
    {

        public enum Players { Player1, Player2 };

        protected bool isDraw = false;
        protected bool haveWinner;


        protected Board board;
        protected Players currentTurn = Players.Player1;
        protected Piece player1Piece = Piece.Batu;
        protected Piece player2Piece = Piece.Maru;

        protected PlayerBase p1;
        protected PlayerBase p2;
        protected List<PlayerBase> players = new List<PlayerBase>();

        protected Piece winningPiece = Piece.Empty;
        protected Players winningPlayer;

        protected bool gameOver = false;


        /// <summary>
        /// Constructs a new TicTacToeGame using the default board pieces for player one and two
        /// </summary>
        public GameController() : this(Piece.Batu, Piece.Maru)
        {

        }


        /// <summary>
        /// Constructs a new TicTacToe game using the specified player's pieces.
        /// 
        /// </summary>
        /// <param name="player1Piece">Player one's piece</param>
        /// <param name="player2Piece">Player two's piece</param>
        public GameController(Piece player1Piece, Piece player2Piece)
        {
            this.player1Piece = player1Piece;
            this.player2Piece = player2Piece;
            board = new Board();
        }

        /// <summary>
        /// Gets the Board associated with this game
        /// </summary>
        public Board GameBoard
        {
            get { return board; }
        }


        /// <summary>
        /// gets number of columns on the board
        /// </summary>
        public int Columns
        {
            get { return Board.COLUMNS; }
        }


        /// <summary>
        /// gets the number of rows on the game board
        /// </summary>
        public int Rows
        {
            get { return Board.ROWS; }
        }


        /// <summary>
        /// If there currently is a winner, this returns the the piece that has
        /// won. Otherwise it returns Pieces.Empty if there is no winner.
        /// </summary>
        public Piece WinningPiece
        {
            get { return winningPiece; }
        }


        /// <summary>
        /// Returns true if the game is over (if there is a winner or there is a draw)
        /// </summary>
        /// <returns>true if the game is over or false otherwise</returns>
        public bool IsGameOver()
        {
            return board.IsGameOver();
        }

        /// <summary>
        /// gets or sets Player 1's game piece
        /// </summary>
        public Piece Player1Piece
        {
            get { return player1Piece; }
            set { player1Piece = value; }
        }


        /// <summary>
        /// Gets or sets Player 2's game piece
        /// </summary>
        public Piece Player2Piece
        {
            get { return player2Piece; }
            set { player2Piece = value; }
        }



        /// <summary>
        /// Returns the player for whose turn it is
        /// </summary>
        public Players CurrentPlayerTurn
        {
            get { return this.currentTurn; }
        }


        /// <summary>
        /// Makes the specified move
        /// </summary>
        /// <param name="m">The TicTacToe move to be made</param>
        /// 
        public void MakeMove(TicTacToeMove m)
        {
            MakeMove(m, GetPlayerWhoHasPiece(m.Piece));
        }


        /// <summary>
        /// Makes the move for the specified player
        /// </summary>
        /// <param name="m">The move to make</param>
        /// <param name="p">The player making the move</param>
        public void MakeMove(TicTacToeMove m, Players p)
        {

            if (currentTurn != p)
            {
                throw new InvalidMoveException("You went out of turn!");
            }

            if (!board.IsValidSquare(m.Position))
                throw new InvalidMoveException("Pick a square on the board!");

            board.MakeMove(m.Position, m.Piece);

            SwapTurns();

        }

        // Returns the game piece for the specified player
        protected Piece GetPlayersPiece(Players p)
        {

            if (p == Players.Player1)
                return player1Piece;
            else
                return player2Piece;
        }


        // returns the Player who has the specified piece
        protected GameController.Players GetPlayerWhoHasPiece(Piece piece)
        {
            if (piece == player1Piece)
                return Players.Player1;
            else
                return Players.Player2;
        }

        // Swap whose turn it is.
        // If X just moved we make it O's turn and
        // vice versa
        private void SwapTurns()
        {
            if (currentTurn == Players.Player1)
                currentTurn = Players.Player2;

            else
                currentTurn = Players.Player1;
        }
        public void Run()
        {
            bool b = Confirm("Are you first?:");
            p1 = b ? new HumanPlayer("Joe", Piece.Batu) : new RobotPlayer("Computer", Piece.Batu, 5);
            p2 = b ? new RobotPlayer("Computer", Piece.Maru, 5) : new HumanPlayer("Joe", Piece.Maru);

            players.Add(p1);
            players.Add(p2);
            PrintBoard(board, players);
            while (!IsGameOver())
            {
                for (int i = 0; i < players.Count; i++)
                {
                    PrintInput();
                    PlayerBase p = players[i];
                    p.Move(board);
                    TicTacToeMove playerMove = p.CurrentMove;

                    MakeMove(new TicTacToeMove(playerMove.Position, p.PlayerPiece));
                    PrintBoard(board, players);
                    if (IsGameOver())
                    {
                        ShowEndOfGameMessage(players[i]);
                        break;
                    }
                }
            }
        }
        private void ShowEndOfGameMessage(PlayerBase lastPlayerToAct)
        {
            string msg = "Game Over! ";

            if (board.HasAWinner())
                msg += lastPlayerToAct.Name + " wins!";
            else
                msg += "It's a draw.";

            Console.WriteLine(msg);
        }

        private void PrintBoard(Board board, List<PlayerBase> players)
        {
            Console.Clear();
            Console.WriteLine("You: {0}\n", GetHumanPlayerPiece(players));

            for (int i = 0; i < Board.ROWS*Board.COLUMNS; i++)
            {
                Console.Write(board[i].Value + "  ");
                if (i % Board.COLUMNS == Board.COLUMNS-1)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                }
            }
        }
        private char GetHumanPlayerPiece(List<PlayerBase> players)
        {
            foreach(var player in players)
            {
                if (player is HumanPlayer)
                {
                    return player.PlayerPiece.Value;
                }
            }
            return Piece.Empty.Value;
        }

        // Display input information
        private void PrintInput()
        {
            Console.WriteLine("\nPlease input the following index number");
            for (int i = 0; i < Board.ROWS*Board.COLUMNS; i++)
            {
                Console.Write(i + " ");
                if (i % Board.COLUMNS == Board.COLUMNS-1)
                    Console.WriteLine("");
            }
            Console.Write($"\nYour position? (0-{Board.ROWS * Board.COLUMNS-1}):");
        }
        public static bool Confirm(string message)
        {
            Console.Write(message);
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            try
            {
                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.KeyChar == 'y')
                        return true;
                    if (key.KeyChar == 'n')
                        return false;
                    Console.CursorLeft = left;
                    Console.CursorTop = top;
                }
            }
            finally
            {
                Console.WriteLine();
            }
        }

    }

    /// <summary>
    /// Represents a tic-tac-toe move
    /// </summary>
    public class TicTacToeMove
    {

        /// <summary>
        /// Constructs a TicTacToeMove
        /// </summary>
        /// <param name="position">The position to move to</param>
        /// <param name="piece">The piece that is moving</param>
        public TicTacToeMove(int position, Piece piece)
        {
            this.Position = position;
            this.Piece = piece;
        }


        /// <summary>
        /// gets or sets the position on the board
        /// </summary>
        public int Position { get; set; }


        /// <summary>
        /// Gets or sets the piece making this move
        /// </summary>
        public Piece Piece { get; set; }

    }

    /// <summary>
    /// An Exception representing an invalid move
    /// </summary>
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException()
            : base()
        {
        }

        public InvalidMoveException(string msg)
            : base(msg)
        {
        }
    }
}


