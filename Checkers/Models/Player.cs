using System;

namespace Checkers
{
    /// <summary>
    /// enum representing the player color
    /// </summary>
    public enum PlayerColor
    {
        WHITE,
        BLACK
    }

    /// <summary>
    /// contains the player's piece locations, color and first row
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The player's color
        /// </summary>
        public PlayerColor Color;

        /// <summary>
        /// The player's board , a 32 bit integer where every 1 bit represents this player's piece and every 0 bit represents empty board/ other player piece
        /// </summary>
        public UInt32 PB;

        /// <summary>
        /// the value representing where the first row of the player starts
        /// </summary>
        public UInt32 FirstRowStart;


        /// <summary>
        /// the value representing where the first row of the player ends
        /// </summary>
        public UInt32 FirstRowEnd;


        /// <summary>
        /// Initiate PB, FirstRow and Color properties
        /// </summary>
        /// <param name="PB"></param>
        /// <param name="Color"></param>
        /// <param name="FirstRowStart"></param>
        /// <param name="FirstRowEnd"></param>
        public Player(UInt32 PB, PlayerColor Color, UInt32 FirstRowStart, UInt32 FirstRowEnd)
        {
            this.PB = PB;
            this.Color = Color;
            this.FirstRowStart = FirstRowStart;
            this.FirstRowEnd = FirstRowEnd;
        }

        /// <summary>
        /// gives the user a piece at a certain index (index from right to left)
        /// if there is no piece at that index returns 0
        /// </summary>
        /// <param name="selectedPieceIndex"> the index of the selected piece from right to left (0 - 31) </param>
        /// <returns> an UInt32 that only the bit at its i location (from right to left) is 1 and the rest are 0 IF a piece exists for player in that index , otherwise returns 0 </returns>
        public UInt32 GetPiece(int selectedPieceIndex)
        {
            // UInt32 representing the given piece (all bits are 0 except bit where the piece is which is 1)
            UInt32 piece = 1;
            for (int bitIndex = 0; bitIndex < selectedPieceIndex; bitIndex++)
            {
                piece *= 2;  
            }

            // check if there is a piece at the given location , if not it resets piece to 0
            piece &= PB;

            return piece;
        }
    }
}
