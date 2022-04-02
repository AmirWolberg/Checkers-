using System;
using System.Collections.Generic;

namespace Checkers
{
    /// <summary>
    /// Struct Used to represent an AI move (contains its heuristic value and the details of the move itself)
    /// </summary>
    public struct MoveAI
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="Piece"> piece to be moved </param>
        /// <param name="Where"> list of where to move the piece in current turn </param>
        /// <param name="Heuristic"> score of herustic function on game board </param>
        public MoveAI(UInt32 Piece, List<PieceMove> Where, int Heuristic)
        {
            this.Piece = Piece;
            // Copy by value not by refrence
            this.Where = ListFuncs<PieceMove>.CloneList(Where);
            this.Heuristic = Heuristic;
        }

        /// <summary>
        /// What piece to move
        /// </summary>
        public UInt32 Piece { get; }

        /// <summary>
        /// List of where to move the piece in the current turn 
        /// </summary>
        public List<PieceMove> Where { get; }

        /// <summary>
        /// Score of the heuristic function on the game board 
        /// </summary>
        public int Heuristic { get; }
    }
}
