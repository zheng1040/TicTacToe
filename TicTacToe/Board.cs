using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TicTacToe
{
    public class Board : ICloneable
    {
        public static readonly int COLUMNS = 3;
        public static readonly int ROWS = 3;
        public static readonly int WINNING_LENGTH = 3;

        private Piece winningPiece;
        /// <summary>
        /// 0:whitespace
        /// 1:cross
        /// 2:circle
        /// </summary>
        protected int[,] board;

        public Piece this[int idx]
        {
            get
            {
                if (!IsOccupied(idx))
                    return Piece.Empty;

                if (GetBoardPieceValue(idx) == 1)
                    return Piece.Batu;
                else
                    return Piece.Maru;
            }
            set
            {
                int x, y;
                GetArrayIdx4Pos(idx, out x, out y);
                if(value == Piece.Batu)
                {
                    board[x, y] = 1;
                }
                else if(value == Piece.Maru)
                {
                    board[x, y] = 2;
                }
                else
                {
                    board[x, y] = 0;
                }
            }
        }

        public Board(int[,] gameState) : this()
        {
            for (int i = 0; i <= gameState.GetUpperBound(0); i++)
                for (int j = 0; j <= gameState.GetUpperBound(1); j++)
                {
                    this.board[i, j] = gameState[i, j];
                }
        }
        public Board()
        {
            board = new int[ROWS, COLUMNS];
        }

        public Piece WinningPiece
        {
            get { return winningPiece; }
            set { winningPiece = value; }
        }
        /// <summary>
        /// used to check a empty space
        /// and index not out of range
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsValidSquare(int idx)
        {
            int x, y;
            GetArrayIdx4Pos(idx,out x, out y);
            return IsValidSquare(x, y);
        }
        public bool IsValidSquare(int x, int y)
        {
            if (x >= 0 && x < ROWS && y >= 0 && y < COLUMNS && IsPositionavailable(x,y))
                return true;
            return false;
        }
        public void MakeMove(int pos, Piece piece)
        {
            if (!IsValidSquare(pos))
                throw new InvalidMoveException();

            int pieceNumber = GetPieceNumber(piece);
            int x, y;
            GetArrayIdx4Pos(pos, out x,out y);

            board[x, y] = pieceNumber;
        }
        public int[] EmptyPositions
        {
            get
            {
                List<int> positions = new List<int>();
                for (int i = 0; i < board.Length; i++)
                {
                    if (!IsOccupied(i))
                    {
                        positions.Add(i);
                    }
                }
                return positions.ToArray();
            }
        }
        public Piece GetPieceAtArray(int row, int column)
        {
            return this[GetPositionFromArrayIdx(row, column)];
        }
        public bool HasAWinner()
        {
            for (int i = 0; i < board.Length; i++)
                if (IsWinnerAt(i))
                {
                    SetWinnerAtPosition(i);
                    return true;
                }
            return false;
        }
        private static bool IsValidPosition(int position)
        {
            return position >= 0 && position < Board.COLUMNS * Board.ROWS;
        }

        private void InitBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j] = 0;

        }
        protected void GetArrayIdx4Pos(int position,out int x,out int y)
        {
            x = position / COLUMNS;
            y = position % ROWS;
        }

        protected int GetPieceNumber(Piece p)
        {
            if (p == Piece.Maru)
                return 2;
            else
                return 1;
        }
        protected int GetPositionFromArrayIdx(int x,int y)
        {
            return x * COLUMNS + y;
        }
        private void SetWinnerAtPosition(int position)
        {
            WinningPiece = this[position];
        }
        private int GetBoardPieceValue(int idx)
        {
            int x, y;
            GetArrayIdx4Pos(idx, out x,out y);
            return board[x, y];
        }

        private bool IsPositionavailable(int row, int col)
        {
            return board[row, col] == 0;
        }
        private bool IsOccupied(int idx)
        {
            int x, y;
            GetArrayIdx4Pos(idx, out x,out y);
            return IsOccupied(x, y);
        }
        private bool IsOccupied(int row, int col)
        {
            return !IsPositionavailable(row, col);
        }
        /// <summary>
        /// find a winner from row, column, diagonal
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsWinnerAt(int position)
        {
            if (IsWinnerToTheRight(position) || IsWinnerFromTopToBottom(position)
                || IsWinnerDiagonallyToRightUp(position) || IsWinnerDiagonallyToRightDown(position))
                return true;
            else
                return false;
        }
        public bool IsGameOver()
        {
            return HasAWinner() || IsDraw();
        }
        public static Piece GetOponentPiece(Piece yourPiece)
        {
            if (yourPiece == Piece.Batu)
                return Piece.Maru;
            else if (yourPiece == Piece.Maru)
                return Piece.Batu;
            else
                throw new Exception("Invalid Piece!");
        }
        public bool IsDraw()
        {
            if (HasAWinner())
                return false;
            for (int i = 0; i < board.Length; i++)
            {
                if (!IsOccupied(i))
                    return false;
            }
            return true;
        }
        private bool IsIdxInBounds(int x,int y)
        {
            if (x < 0 || x >= ROWS || y < 0 || y >= COLUMNS)
                return false;

            return true;
        }

        /// <summary>
        /// find a winner in the diagonal starting from
        /// the bottom-left corner of the board to the upper-right corner
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsWinnerDiagonallyToRightUp(int position)
        {

            if (!IsOccupied(position))
                return false;

            Piece piece = this[position];
            int x, y;
            GetArrayIdx4Pos(position,out x,out y);
            for (int i = 1; i < WINNING_LENGTH; i++)
            {
                x -= 1;
                y += 1;
                if (!IsIdxInBounds(x,y))
                    return false;

                if (piece != this[GetPositionFromArrayIdx(x,y)])
                    return false;
            }
            return true;

        }

        /// <summary>
        /// find a winner diagonally from the specified position to the right
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsWinnerDiagonallyToRightDown(int pos)
        {
            if (!IsOccupied(pos))
                return false;

            Piece piece = this[pos];

            int x, y;
            GetArrayIdx4Pos(pos, out x,out y);
            for (int i = 1; i < WINNING_LENGTH; i++)
            {
                x += 1;
                y += 1;
                if (!IsIdxInBounds(x,y))
                    return false;

                if (piece != this[GetPositionFromArrayIdx(x,y)])
                    return false;

            }

            return true;
        }

        /// <summary>
        /// find winner from top to bottom starting the specified position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsWinnerFromTopToBottom(int pos)
        {
            int x, y;
            GetArrayIdx4Pos(pos, out x,out y);

            if (!IsOccupied(pos))
                return false;

            // check if have the room to go down from here
            if (x + WINNING_LENGTH - 1 >= ROWS)
                return false;

            Piece piece = this[pos];

            for (int i = 1; i < WINNING_LENGTH; i++)
            {
                if (piece != this[pos + 3 * i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// find a winner from the specified position to the right
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsWinnerToTheRight(int pos)
        {
            int x, y;
            GetArrayIdx4Pos(pos, out x,out y);

            if (!IsOccupied(pos))
                return false;

            // check if we have room to the right?
            if (y + WINNING_LENGTH - 1 >= COLUMNS)
                return false;

            Piece piece = this[pos];

            for (int i = 1; i < WINNING_LENGTH; i++)
            {
                if (this[pos + i] != piece)
                    return false;
            }
            return true;
        }

        public object Clone()
        {
            Board b = new Board(this.board);
            return b;
        }
    }


}

