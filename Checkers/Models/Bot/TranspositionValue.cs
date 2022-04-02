

namespace Checkers
{
    /// <summary>
    /// Describes transposition table value
    /// </summary>
    public struct TranspositionValue
    {
        /// <summary>
        /// Move to make in given board situation
        /// </summary>
        public MoveAI move;

        /// <summary>
        /// Depth this move was calculated at
        /// </summary>
        public int depth;

        /// <summary>
        /// flag used to help with alpha beta pruning for transposition table
        /// </summary>
        public TranspositionEnum flag;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="move"> Move to make in given board situation </param>
        /// <param name="depth"> Depth this move was calculated at </param>
        /// <param name="flag"> flag used to help with alpha beta pruning for transposition table </param>
        public TranspositionValue(MoveAI move, int depth, TranspositionEnum flag)
        {
            this.move = move;
            this.depth = depth;
            this.flag = flag;
        }
    }
}
