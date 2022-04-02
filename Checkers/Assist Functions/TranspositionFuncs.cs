using System;

namespace Checkers
{
    /// <summary>
    /// All supporting functions for the transposition table
    /// </summary>
    public static class TranspositionFuncs
    {
        /// <summary>
        /// Get Unique key made out of the details of the given board (the position of pieces, number of turns since last eating and who'se turn it is now)
        /// </summary>
        /// <param name="m"> model representing the game </param>
        /// <param name="futureView"> How much further does the bot normally see </param>
        /// <returns> String representing a Zobrist key for the transposition table </returns>
        public static string GetTranspositionKey(Model m)
        {
            char[] chars = new char[16];

            Board b = m.GameBoard;
         
            UInt32 BB = b.BP.PB;
            UInt32 WB = b.WP.PB;
            UInt32 KB = b.K;
            int draw = m.TurnsToDraw;

            // board
            chars[0] = (char)(BB & 0xFF);
            chars[1] = (char)(BB >> 8 & 0xFF);
            chars[2] = (char)(BB >> 16 & 0xFF);
            chars[3] = (char)(BB >> 24 & 0xFF);
            chars[4] = (char)(WB & 0xFF);
            chars[5] = (char)(WB >> 8 & 0xFF);
            chars[6] = (char)(WB >> 16 & 0xFF);
            chars[7] = (char)(WB >> 24 & 0xFF);
            chars[8] = (char)(KB & 0xFF);
            chars[9] = (char)(KB >> 8 & 0xFF);
            chars[10] = (char)(KB >> 16 & 0xFF);
            chars[11] = (char)(KB >> 24 & 0xFF);

            // Add How close the game is to a draw as part of the board key
            chars[12] = (char)(draw & 0xFF);
            chars[13] = (char)(draw >> 8 & 0xFF);
            chars[14] = (char)(draw >> 16 & 0xFF);
            chars[15] = (char)(draw >> 24 & 0xFF);

            return new string(chars);
            
        }
    }
}
