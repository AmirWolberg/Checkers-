using System;

namespace Checkers
{
    /// <summary>
    /// struct describing a possible move for a certain piece
    /// </summary>
    public struct PieceMove
    {
        /// <summary>
        /// where the move leads to (as a mask)
        /// </summary>
        public UInt32 Where { get; }


        /// <summary>
        /// whether the move is an eating move or a regular move (eating move means an enemy piece is being eaten during the move)
        /// </summary>
        public bool Eat { get; }

        /// <summary>
        /// if the move is an eating move this holds where the eaten piece is (as a mask), if no piece was eaten 0 is to be inserted
        /// </summary>
        public UInt32 WhereEaten { get; }


        /// <summary>
        /// Constructor of PieceMove struct
        /// </summary>
        /// <param name="where"> where the move leads to (as a mask) </param>
        /// <param name="eat"> whether the move is an eating move or a regular move (eating move means an enemy piece is being eaten during the move) </param>
        /// <param name="whereEaten"> if the move is an eating move this holds where the eaten piece is (as a mask), if no piece was eaten 0 is to be inserted </param>
        public PieceMove(UInt32 where, bool eat, UInt32 whereEaten)
        {
            this.Where = where;
            this.Eat = eat;
            this.WhereEaten = whereEaten;
        }
    }
}
