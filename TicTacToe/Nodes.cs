using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TicTacToe
{
    public abstract class NodeBase
    {
        protected List<NodeBase> children;

        // the value associated with the node
        // this is the result of the evaluatio function for leaf nodes
        protected double value;

        // The child node that represents the best move for a node.
        protected NodeBase bestMoveNode;

        protected Board board;

        // the evaluation function 
        protected static EvaluationFunction evaluator;

        protected Piece myPiece;
        protected Piece opponentPiece;

        // the move that was made from the parent node that created this node
        TicTacToeMove move;
        NodeBase parent = null;

        /// <summary>
        /// Constructs a new Node
        /// </summary>
        /// <param name="b">The board that the Node will use to evaluate itself and generate its children</param>
        /// <param name="parent">The parent of this node</param>
        /// <param name="move">The move from the parent's board that generated this node's board</param>
        public NodeBase(Board b, NodeBase parent, TicTacToeMove move)
        {
            this.board = b;
            this.parent = parent;
            this.move = move;
            if (parent != null)
                myPiece = Board.GetOponentPiece(parent.MyPiece);
            children = new List<NodeBase>();
        }
        public Piece MyPiece
        {
            get { return myPiece; }
            set
            {
                myPiece = value;
                opponentPiece = Board.GetOponentPiece(value);
            }
        }
        public EvaluationFunction Evaluator
        {
            set { evaluator = value; }
        }
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        protected abstract void Evaluate();

        /// <summary>
        /// Selects the best move node for the node
        /// </summary>
        public void SelectBestMove()
        {
            // if no children there is no best move for the node
            if (children.Count == 0)
            {
                bestMoveNode = null;
                return;
            }

            // sort the children so that the first element contains the 'best' node
            List<NodeBase> sortedChildren = SortChildren(this.children);

            this.bestMoveNode = sortedChildren[0];
            Value = bestMoveNode.Value;
        }


        /// <summary>
        /// Finds the best move for the node by doing a pseudo-depth-f
        /// </summary>
        /// <param name="depth">The depth to search</param>
        public virtual void FindBestMove(int depth)
        {
            if (depth > 0)
            {
                GenerateChildren();

                EvaluateChildren();

                // check for a winner
                bool haveWinningChild = children.Exists(c => c.IsGameEndingNode());

                if (haveWinningChild)
                {
                    SelectBestMove();
                    return;
                }
                else
                {
                    foreach (NodeBase child in children)
                    {
                        child.FindBestMove(depth - 1);
                    }
                    SelectBestMove();
                }
            }

        }


        /// <summary>
        /// Generate the nodes children
        /// </summary>
        protected abstract void GenerateChildren();

        /// <summary>
        /// Checks to see if the node is either a winner or MAX or MIN
        /// </summary>
        /// <returns>true if either 'X' or 'O'</returns>
        public virtual bool IsGameEndingNode()
        {
            return Value == double.PositiveInfinity || Value == double.NegativeInfinity;
        }


        /// <summary>
        /// The best move from this node's board configuration
        /// </summary>
        public TicTacToeMove BestMove
        {
            get { return this.bestMoveNode.move; }
        }


        // evaluate the child nodes
        protected void EvaluateChildren()
        {
            foreach (NodeBase child in this.children)
            {
                child.Evaluate();
            }
        }
        protected abstract List<NodeBase> SortChildren(List<NodeBase> unsortedChildren);

        protected abstract bool IsWinningNode();

    }

    /// <summary>
    /// This class represents a MAX node in the game tree.
    /// </summary>
    public class MaxNode : NodeBase
    {
        /// <summary>
        /// Constructs a new max node
        /// </summary>
        /// <param name="b">The Board that this node represents</param>
        /// <param name="parent">The node's parent</param>
        /// <param name="m">The move made to create this node's board</param>
        public MaxNode(Board b, NodeBase parent, TicTacToeMove m)
            : base(b, parent, m)
        {
        }

        /// <summary>
        /// Generate Children.  MAX Nodes have MIN children
        /// </summary>
        protected override void GenerateChildren()
        {
            int[] openPositions = board.EmptyPositions;
            foreach (int i in openPositions)
            {
                Board b = (Board)board.Clone();
                TicTacToeMove m = new TicTacToeMove(i, myPiece);
                b.MakeMove(i, myPiece);
                children.Add(new MinNode(b, this, m));
            }
        }

        // Evaluates how favorable the board configuration is for this node
        protected override void Evaluate()
        {
            this.Value = evaluator.Evaluate(this.board, myPiece);
        }


        // does this node have a winning game configuration
        protected override bool IsWinningNode()
        {
            return this.Value == double.PositiveInfinity;
        }

        // returns a List of this nodes children sorted in descending order 
        protected override List<NodeBase> SortChildren(List<NodeBase> unsortedChildren)
        {
            List<NodeBase> sortedChildren = unsortedChildren.OrderByDescending(n => n.Value).ToList();
            return sortedChildren;
        }

    }



    /// <summary>
    /// This class represents a MIN node in the game tree
    /// </summary>
    public class MinNode : NodeBase
    {

        /// <summary>
        /// Constructs a MIN node
        /// </summary>
        /// <param name="b">The board that this node represents</param>
        /// <param name="parent">This node's parent</param>
        /// <param name="m">The move that was made from the parent to lead to this node's board</param>
        public MinNode(Board b, NodeBase parent, TicTacToeMove m)
            : base(b, parent, m)
        {
        }

        // Generates the node's children.  MIN nodes have MAX children
        protected override void GenerateChildren()
        {
            int[] openPositions = board.EmptyPositions;
            foreach (int i in openPositions)
            {
                Board b = (Board)board.Clone();
                TicTacToeMove m = new TicTacToeMove(i, myPiece);

                b.MakeMove(i, myPiece);
                children.Add(new MaxNode(b, this, m));
            }
        }
        protected override bool IsWinningNode()
        {
            return this.value == double.NegativeInfinity;

        }


        // returns a list of the child nodes in ascending order
        // the first node in the list will be the best node for the min node
        protected override List<NodeBase> SortChildren(List<NodeBase> unsortedChildren)
        {
            List<NodeBase> sortedChildren = unsortedChildren.OrderBy(n => n.Value).ToList();
            return sortedChildren;
        }

        /// <summary>
        /// evalutes the value of the node using the evaluation function
        /// </summary>
        protected override void Evaluate()
        {
            Value = evaluator.Evaluate(board, Board.GetOponentPiece(myPiece));
        }
    }


    /// <summary>
    /// The value is computed by summing number of game pieces in the rows, columns, and diagonals 
    /// for those rows, columns and diagonals that are still winnable
    /// </summary>
    public class EvaluationFunction
    {
        public EvaluationFunction()
        {
        }

        /// <summary>
        /// Evaluates the favorability of the current board configuration for maxPiece.
        /// Higher values indicate better configuration for maxPiece
        /// </summary>
        /// <param name="b">the game board to evaluate</param>
        /// <param name="maxPiece">the piece representing MAX</param>
        /// <returns></returns>
        public double Evaluate(Board b, Piece maxPiece)
        {
            if (b.HasAWinner())
            {
                if (b.WinningPiece == maxPiece)
                    return double.PositiveInfinity;
                else
                    return double.NegativeInfinity;
            }

            double maxValue = EvaluatePiece(b, maxPiece);
            double minValue = EvaluatePiece(b, Board.GetOponentPiece(maxPiece));

            return maxValue - minValue;
        }

        // sums up all the possible ways to win for the specified board piece
        private double EvaluatePiece(Board b, Piece p)
        {
            return EvaluateRows(b, p) + EvaluateColumns(b, p) + EvaluateDiagonals(b, p);
        }


        // over all rows sums the number of pieces in the row if 
        // the specified piece can still win in that row i.e. the row
        // does not contain an opponent's piece
        private double EvaluateRows(Board b, Piece p)
        {
            double score = 0.0;
            int count;
            // check the rows
            for (int i = 0; i < Board.ROWS; i++)
            {
                count = 0;
                bool rowClean = true;
                for (int j = 0; j < Board.COLUMNS; j++)
                {
                    Piece boardPiece = b.GetPieceAtArray(i, j);

                    if (boardPiece == p)
                        count++;
                    else if (boardPiece == Board.GetOponentPiece(p))
                    {
                        rowClean = false;
                        break;
                    }
                }

                if (rowClean && count != 0)
                    score += count;
            }

            return score;
        }


        // over all rows sums the number of pieces in the row if 
        // the specified piece can still win in that row i.e. the row
        // does not contain an opponent's piece
        private double EvaluateColumns(Board b, Piece p)
        {
            double score = 0.0;
            int count;
            // check the columns
            for (int j = 0; j < Board.COLUMNS; j++)
            {
                count = 0;
                bool rowClean = true;
                for (int i = 0; i < Board.ROWS; i++)
                {
                    Piece boardPiece = b.GetPieceAtArray(i, j);

                    if (boardPiece == p)
                        count++;
                    else if (boardPiece == Board.GetOponentPiece(p))
                    {
                        rowClean = false;
                        break;
                    }
                }

                if (rowClean && count != 0)
                    score += count;

            }

            return score;
        }


        // over both diagonals sums the number of pieces in the diagonal if 
        // the specified piece can still win in that diagonal i.e. the diagonal
        // does not contain an opponent's piece
        private double EvaluateDiagonals(Board b, Piece p)
        {
            // go down and to the right diagonal first
            int count = 0;
            bool diagonalClean = true;

            double score = 0.0;

            for (int i = 0; i < Board.COLUMNS; i++)
            {
                Piece boardPiece = b.GetPieceAtArray(i, i);

                if (boardPiece == p)
                    count++;

                if (boardPiece == Board.GetOponentPiece(p))
                {
                    diagonalClean = false;
                    break;
                }
            }

            if (diagonalClean && count > 0)
                score += count;

            // now try the other way

            int row = 0;
            int col = Board.COLUMNS -1;
            count = 0;
            diagonalClean = true;

            while (row < Board.ROWS && col >= 0)
            {
                Piece boardPiece = b.GetPieceAtArray(row, col);

                if (boardPiece == p)
                    count++;

                if (boardPiece == Board.GetOponentPiece(p))
                {
                    diagonalClean = false;
                    break;
                }

                row++;
                col--;
            }

            if (count > 0 && diagonalClean)
                score += count;

            return score;
        }
    }
}
