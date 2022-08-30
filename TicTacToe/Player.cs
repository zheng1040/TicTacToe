using System;

namespace TicTacToe
{
    /// <summary>
    /// This class abstracts the idea of a Player and includes some common functionality.
    /// It includes an event for clients to be notified when a move is made
    /// </summary>
    public abstract class PlayerBase
    {
        protected TicTacToeMove currentMove;
        public PlayerBase(string name, Piece p)
        {
            this.Name = name;
            this.PlayerPiece = p;
        }

        public abstract void Move(object gameBoard);

        public TicTacToeMove CurrentMove
        {
            get { return currentMove; }
        }

        /// <summary>
        /// Get or Set the player's piece
        /// </summary>
        public Piece PlayerPiece { get; set; }

        /// <summary>
        /// Get or set the player's name
        /// </summary>
        public string Name { get; set; }

    }


    /// <summary>
    /// This class represents a "comuter" player.  
    /// It determines moves using minmax decision rules
    /// </summary>
    public class RobotPlayer : PlayerBase
    {
        public const int DEFAULT_SEARCH_DEPTH = 2;


        /// <summary>
        /// Constructs a new computer player.  The DEFAULT_SEARCH_DEPTH is used
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="p">The piece this player is using in the came</param>
        public RobotPlayer(string name, Piece p) : this(name,
            p, DEFAULT_SEARCH_DEPTH)
        {
        }


        /// <summary>
        /// Constructs a new computer player
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="p">The piece the player is using</param>
        /// <param name="searchDepth">The depth to search for moves in the game tree.</param>
        public RobotPlayer(string name, Piece p, int searchDepth) : base(name, p)
        {
            this.SearchDepth = searchDepth;
        }


        /// <summary>
        /// gets or sets the search depth which is the number of moves
        /// the computer player will look ahead to determine it's move
        /// Greater values yield better computer play
        /// </summary>
        public int SearchDepth { get; set; }

        /// <summary>
        /// Start the computer searching for a move
        /// </summary>
        /// <param name="gameBoard">The current game board</param>
        public override void Move(object gameBoard)
        {
            Board b = (Board)gameBoard;

            //to make things interesting we move randomly if the board we
            //are going first (i.e. the board is empty)
            if (b.EmptyPositions.Length == 9)
            {
                this.currentMove = GetRandomMove((Board)gameBoard);
                return;
            }

            NodeBase root = new MaxNode(b, null, null);
            root.MyPiece = this.PlayerPiece;
            root.Evaluator = new EvaluationFunction();
            root.FindBestMove(DEFAULT_SEARCH_DEPTH);
            currentMove = root.BestMove;
        }

        protected TicTacToeMove GetRandomMove(Board b)
        {
            int openPositions = b.EmptyPositions.Length;
            Random rGen = new Random();

            int squareToMoveTo = rGen.Next(openPositions);

            TicTacToeMove move = new TicTacToeMove(squareToMoveTo, this.PlayerPiece);
            return move;
        }


    }

    /// <summary>
    /// This class represents a Human Player 
    /// </summary>
    public class HumanPlayer : PlayerBase
    {
        public HumanPlayer(string name, Piece p)
            : base(name, p)
        {
        }

        public override void Move(object gameBoard)
        {
            Board b = (Board)gameBoard;
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line.Length != 1)
                    continue;
                var index = line[0] - '0';
                if (b.IsValidSquare(index))
                {
                    this.currentMove = new TicTacToeMove(index, this.PlayerPiece);
                    return;
                }
                Console.WriteLine("Please input a valid number (0-8)");
            }
        }

    }

}