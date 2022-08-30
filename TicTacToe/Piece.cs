using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public class Piece
    {
        public static readonly Piece Batu = new Piece { Value = 'X' };
        public static readonly Piece Maru = new Piece { Value = 'O' };
        public static readonly Piece Empty = new Piece { Value = '.' };

        public char Value { get; private set; }
    }
}
