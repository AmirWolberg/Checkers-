using System;

namespace Checkers 
{

    /// <summary>
    /// contains both players and initiates them and also contains king bit board
    /// </summary>
    public class Board
    {

        /// <summary>
        /// the length of a row on the board (how many bits represent each row)
        /// </summary>
        public readonly int RowLength = 4;


        /// <summary>
        /// a variable representing the white player (the player's piece locations, color and first row)
        /// </summary>
        public Player WP = new Player(0b_11111111111100000000000000000000, PlayerColor.WHITE, 0b_00010000000000000000000000000000, 0b_10000000000000000000000000000000); // 1111
                                                                                                                                                                         // 1111
                                                                                                                                                                         // 1111
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000


        /// <summary>
        /// a variable representing the black player (the player's piece locations, color and first row)
        /// </summary>
        public Player BP = new Player(0b_0000000000000000000111111111111, PlayerColor.BLACK, 0b_00000000000000000000000000000001, 0b_00000000000000000000000000001000); // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 0000
                                                                                                                                                                         // 1111
                                                                                                                                                                         // 1111
                                                                                                                                                                         // 1111   


        /// <summary>
        /// a bit board representing the location of all kings alive ( 1 is a king , 0 is empty)
        /// </summary>
        public UInt32 K = 0b_00000000000000000000000000000000; // 0000
                                                               // 0000
                                                               // 0000
                                                               // 0000
                                                               // 0000
                                                               // 0000
                                                               // 0000
                                                               // 0000  

    }
}
