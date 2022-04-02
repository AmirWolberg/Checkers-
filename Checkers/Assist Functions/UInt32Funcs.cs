using System;

namespace Checkers
{
    /// <summary>
    /// Contains all selfmade used UInt32 functions/delegates
    /// </summary>
    public static class UInt32Funcs
    {
        /// <summary>
        /// preforms shift operation on given piece
        /// </summary>
        /// <param name="piece"> given piece to preform shift operation on </param>
        /// <param name="bits"> how many bits to shift </param>
        /// <returns> needed shift operation on given piece </returns>
        public delegate UInt32 shift_operation(UInt32 piece, int bits);


        /// <summary>
        /// converts UInt32 to a 32 character string adds 0 to the left of the board until its length is 32
        /// </summary>
        /// <param name="num"> UInt32 value to convert to string </param>
        /// <returns> num as a 32 character long string </returns>
        public static string Make32Bit(UInt32 num)
        {
            string strNum = (Convert.ToString(num, toBase: 2)); // convert to string 
            while (strNum.Length != 32) // make sure string contains all 32 bits
            {
                strNum = "0" + strNum;
            }
            return strNum;
        }


        /// <summary>
        /// Count how many set bits are in given UInt32 , O(1) complexity
        /// </summary>
        /// <param name="number"> number to count set bytes of </param>
        /// <returns> return count of set bits </returns>
        public static int CountSetBits(UInt32 number)
        {
            number = number - ((number >> 1) & 0x55555555);
            number = (number & 0x33333333) + ((number >> 2) & 0x33333333);
            return (int)(((number + (number >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}
