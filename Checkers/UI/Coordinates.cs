

namespace Checkers
{
    /// <summary>
    /// Coordinates consisting of row to describe row number on the board and col to describe collumn number on the board
    /// </summary>
    public struct Coordinates
    {
        /// <summary>
        /// Row number on the board (0 - 7)
        /// </summary>
        public int row;

        /// <summary>
        /// Collumn number on the board (0 - 7)
        /// </summary>
        public int col;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row"> row number on the board </param>
        /// <param name="col"> collumn number on the board </param>
        public Coordinates(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
}
