using System;
using System.Collections.Generic;

namespace Checkers
{

    /// <summary>
    /// class containing the game's logic
    /// </summary>
    public class Model
    {
        /// <summary>
        /// constant representing how many turns without a piece being eaten can pass until the game is declared a draw
        /// </summary>
        public readonly int TurnsToDraw = 40;


        /// <summary>
        /// holds how many turns its been since a piece on the board was eaten
        /// </summary>
        public int LastTurnEaten = 0;


        /// <summary>
        /// holds the game board
        /// </summary>
        public Board GameBoard { get; private set; }


        /// <summary>
        /// initiates model properties 
        /// </summary>
        public Model()
        {
            GameBoard = new Board();
        }


        /// <summary>
        /// Copys this model into given model
        /// </summary>
        /// <param name="newM"> Other given model </param>
        public void CopyModel(Model newM)
        {
            // Copy Kings
            newM.GameBoard.K = GameBoard.K;

            // Copy Black player
            newM.GameBoard.BP.Color = GameBoard.BP.Color;
            newM.GameBoard.BP.PB = GameBoard.BP.PB;
            newM.GameBoard.BP.FirstRowStart = GameBoard.BP.FirstRowStart;
            newM.GameBoard.BP.FirstRowEnd = GameBoard.BP.FirstRowEnd;

            // Copy White player
            newM.GameBoard.WP.Color = GameBoard.WP.Color;
            newM.GameBoard.WP.PB = GameBoard.WP.PB;
            newM.GameBoard.WP.FirstRowStart = GameBoard.WP.FirstRowStart;
            newM.GameBoard.WP.FirstRowEnd = GameBoard.WP.FirstRowEnd;

            // Others
            newM.LastTurnEaten = LastTurnEaten;

        }


        /// <summary>
        /// finds the current game state
        /// </summary>
        /// <returns> returns the current game state, whether its a draw/a player has won/the game is still ongoing </returns>
        public GameState CheckGameState()
        {
            // if its been 30 turns since someone ate a piece or if both players have no  pieces left its a DRAW
            if ((LastTurnEaten >= TurnsToDraw) || (GameBoard.BP.PB == 0 && GameBoard.WP.PB == 0))
                return GameState.Draw;

            // if black player has no pieces left OR black player cannot move any piece white won
            if ((GameBoard.BP.PB == 0) || GetMoveAblePieces(this.GameBoard.BP).Count == 0)
                return GameState.WhiteWon;

            // if white player has no pieces left OR white payer cannot move any piece black won
            if ((GameBoard.WP.PB == 0) || GetMoveAblePieces(this.GameBoard.WP).Count == 0)
                return GameState.BlackWon;

            // game is not over yet, no one has won and its not a draw
            return GameState.OnGoing;
        }


        /// <summary>
        /// Gets a list of all moveable pieces by given player
        /// </summary>
        /// <param name="currentPlayer"> The current player </param>
        /// <returns> List of all moveable pieces by given player </returns>
        public List<UInt32> GetMoveAblePieces(Player currentPlayer)
        {
            List<UInt32> moveAblePieces = new List<UInt32>();

            // Get all pieces
            for (int piece = 0; piece < 32; piece++)
            {
                moveAblePieces.Add(currentPlayer.GetPiece(piece));
            }

            // Remove all 0 where there are no pieces
            moveAblePieces.RemoveAll(piece => piece == 0);

            // Remove all pieces that cannot move
            moveAblePieces.RemoveAll(piece => GetPossibleMoves(currentPlayer.Color, piece).Count == 0);

            return moveAblePieces;
        }
     

        /// <summary>
        /// kings given piece at given location
        /// </summary>
        /// <param name="kinged"> piece of the player to king </param>
        private void MakeKing(UInt32 kinged)
        {
            GameBoard.K |= kinged;
        }


        /// <summary>
        /// gets the board belonging to the given player
        /// </summary>
        /// <param name="player"> black or white the player's color </param>
        /// <returns> returns the board belonging to the given player color </returns>
        public Player GetPlayerBoard(PlayerColor player)
        {
            return player switch
            {
                (PlayerColor.WHITE) => GameBoard.WP,
                (PlayerColor.BLACK) => GameBoard.BP,
                _ => throw new Exception("Invalid player"),
            };
        }


        /// <summary>
        /// makes a move on the board
        /// </summary>
        /// <param name="player"> PlayerColor.WHITE/PlayerColor.BLACK </param>
        /// <param name="piece"> piece is an UInt32 with all bits 0 except for the one representing the piece the player wants to move </param>
        /// <param name="whereMask"> whereMask is an UInt32 with all bits 0 except for the one representing where the player wants the piece to move </param>
        /// <returns> returns true if current move was an eat and there is another possible eat for the same piece in the next move (so if chain eating is possible returns true) otherwise returns false </returns>
        public bool MakeMove(PlayerColor player, UInt32 piece, PieceMove whereMask)
        {

            switch (player)
            { 
                case (PlayerColor.WHITE):

                    GameBoard.WP.PB ^= piece; // turns the bit at the location of the piece from 1 to 0 (indicating there is no longer a piece there) 
                    GameBoard.WP.PB |= whereMask.Where; // turns the bit at the location of where you move the piece from 0 to 1 (indicating there is now a piece there) 

                    // reached first row of black (enemy) player, kinged piece
                    if (whereMask.Where <= GameBoard.BP.FirstRowEnd && whereMask.Where >= GameBoard.BP.FirstRowStart)
                    {
                        MakeKing(whereMask.Where);
                    }

                    // if piece is a king change king board as well
                    if ((GameBoard.K & piece) != 0)
                    {
                        GameBoard.K ^= piece;
                        GameBoard.K |= whereMask.Where;
                    }

                    // if the move is an eating move
                    if (whereMask.Eat)
                    {
                        LastTurnEaten = 0;
                        GameBoard.BP.PB ^= whereMask.WhereEaten;

                        // If eaten piece was a king need to reset it on king board
                        if((GameBoard.K & whereMask.WhereEaten) !=0)
                            GameBoard.K ^= whereMask.WhereEaten;

                        // get list of all possible move after eating move
                        List<PieceMove> futureMoves = GetPossibleMoves(PlayerColor.WHITE, whereMask.Where);

                        // check if any of the possible moves is an eat move
                        bool possibleFutureEat = false;
                        foreach (PieceMove move in futureMoves)
                        {
                            if (move.Eat)
                            {
                                possibleFutureEat = true;
                            }
                        }

                        // returns true if there is a possible move in the future that is an eating move and this move was also an eating move so chain eating is possible
                        return possibleFutureEat;
                    }

                    else
                    {
                        LastTurnEaten += 1;
                        return false;
                    }

                case (PlayerColor.BLACK):

                    GameBoard.BP.PB ^= piece; // turns the bit at the location of the piece from 1 to 0 (indicating there is no longer a piece there) 
                    GameBoard.BP.PB |= whereMask.Where; // turns the bit at the location of where you move the piece from 0 to 1 (indicating there is now a piece there) 
               
                    // reached first row of white (enemy) player, kinged piece
                    if (whereMask.Where <= GameBoard.WP.FirstRowEnd && whereMask.Where >= GameBoard.WP.FirstRowStart)
                    {
                        MakeKing(whereMask.Where);
                    }

                    // if piece is a king change king board as well
                    if ((GameBoard.K & piece) != 0)
                    {
                        GameBoard.K ^= piece;
                        GameBoard.K |= whereMask.Where;
                    }

                    // if the move is an eating move
                    if (whereMask.Eat)
                    {
                        LastTurnEaten = 0;
                        GameBoard.WP.PB ^= whereMask.WhereEaten;

                        // If eaten piece was a king need to reset it on king board
                        if ((GameBoard.K & whereMask.WhereEaten) != 0)
                            GameBoard.K ^= whereMask.WhereEaten;

                        // get list of all possible move after eating move
                        List<PieceMove> futureMoves = GetPossibleMoves(PlayerColor.BLACK, whereMask.Where);

                        // check if any of the possible moves is an eat move
                        bool possibleFutureEat = false;
                        foreach (PieceMove move in futureMoves)
                        {
                            if (move.Eat)
                            {
                                possibleFutureEat = true;
                            }
                        }

                        // returns true if there is a possible move in the future that is an eating move and this move was also an eating move so chain eating is possible
                        return possibleFutureEat;
                    }

                    else
                    {
                        LastTurnEaten += 1;
                        return false;
                    }

                default:
                    throw new Exception("Invalid player");
            }

        }


        /// <summary>
        /// finds all the possible locations a certain piece can be moved to for given player
        /// </summary>
        /// <param name="player"> PlayerColor.WHITE/PlayerColor.BLACK </param>
        /// <param name="piece"> piece is an UInt32 with all bits 0 except for the one representing the piece the player wants to move (a mask) </param>
        /// <returns> List of all possible moves the player can make </returns>
        public List<PieceMove> GetPossibleMoves(PlayerColor player, UInt32 piece)
        {

            return player switch
            {
                (PlayerColor.WHITE) => PossibleMovesSpecificPlayer(piece, GameBoard.WP.PB, GameBoard.BP.PB, PlayerColor.WHITE, 4, 3, 5, (piece, bits) => { return piece >> bits; }, (piece, bits) => { return piece << bits; }),
                (PlayerColor.BLACK) => PossibleMovesSpecificPlayer(piece, GameBoard.BP.PB, GameBoard.WP.PB, PlayerColor.BLACK, 4, 5, 3, (piece, bits) => { return piece << bits; }, (piece, bits) => { return piece >> bits; }),
                _ => throw new Exception("Invalid player"),
            };
        }


        /// <summary>
        /// Gets the possible moves for given piece of current player by using bit operations 
        /// </summary>
        /// <param name="piece"> given piece represented as a sequence of bits </param>
        /// <param name="playerBoard"> the current player's bit board </param>
        /// <param name="enemyBoard"> the enemy player's bit board </param>
        /// <param name="player"> the player's color PlayerColor.WHITE/PlayerColor.BLACK </param>
        /// <param name="nextRowMove"> the amount of bits needed for basic next row move </param>
        /// <param name="nextRowDiagonalMoveEven"> the amount of bits needed for additional next row move ( a diagonal move) if the current row the piece is on is even </param>
        /// <param name="nextRowDiagonalMoveOdd"> the amount of bits needed for additional next row move ( a diagonal move) if the current row the piece is on is odd </param>
        /// <param name="op"> type of operation to preform on the piece with given bits to find a possible move (shift operation - shift left/shift right) </param>
        /// <param name="kingBackOp"> type of operation to preform on the piece with given bits to find a possible move (shift operation- shift left/right) the opposite operation of 
        /// op used for king piece to move backwards </param>
        /// <returns> Possible moves of given piece of current player </returns>
        private List<PieceMove> PossibleMovesSpecificPlayer(UInt32 piece, UInt32 playerBoard, UInt32 enemyBoard, PlayerColor player, int nextRowMove, int nextRowDiagonalMoveEven,
            int nextRowDiagonalMoveOdd, UInt32Funcs.shift_operation op, UInt32Funcs.shift_operation kingBackOp)
        {
            // holds all possible moves the given piece can make, also contains 0s represnting unavailable certain moves
            List<PieceMove> possibleMoves = new List<PieceMove>();

            // the give piece's index (right to left 0 to 31) and row (buttom to top 0 to 7)
            int pieceIndex, pieceRow;

            // index from right to left , right most is 0 , left most is 31
            pieceIndex = System.Numerics.BitOperations.Log2(piece); // holds what number the 1 bit is in the piece 

            // get what row of the board the piece is on (stats from buttom row as row 0 up to the top as row 7)
            pieceRow = pieceIndex / 4;

            // check if possible move for next row is available
            possibleMoves.Add(CheckPossibleMove(piece, 1, op, playerBoard, enemyBoard, player, nextRowMove, false));


            // if the piece is a king it can also walk backwards
            if ((piece & GameBoard.K) != 0)
            {
                // check if possible move for back row is available
                possibleMoves.Add(CheckPossibleMove(piece, 1, kingBackOp, playerBoard, enemyBoard, player, (GameBoard.RowLength * 2) - nextRowMove, true));
            }


            // check diagonale possible move for piece, changes depending if its on an even or an odd row
            switch (pieceRow % 2)
            {
                // even row
                case (0):
                    // check if possible move for diagonale next row (if the piece is on an even row) is available
                    possibleMoves.Add(CheckPossibleMove(piece, pieceIndex + 1, op, playerBoard, enemyBoard, player, nextRowDiagonalMoveEven, false));

                    // if the piece is a king it can also walk backwards
                    if ((piece & GameBoard.K) != 0)
                    {
                        // check if possible move for diagonale back row (if the piece is on an even row) is available
                        possibleMoves.Add(CheckPossibleMove(piece, pieceIndex + 1, kingBackOp, playerBoard, enemyBoard, player, (GameBoard.RowLength * 2) - nextRowDiagonalMoveEven, true));
                    }

                    break;

                // odd row
                case (1):
                    // check if possible move for diagonale next row (if the piece is on an odd row) is available
                    possibleMoves.Add(CheckPossibleMove(piece, pieceIndex, op, playerBoard, enemyBoard, player, nextRowDiagonalMoveOdd, false));

                    // if the piece is a king it can also walk backwards
                    if ((piece & GameBoard.K) != 0)
                    {
                        // check if possible move for diagonale back row (if the piece is on an odd row) is available
                        possibleMoves.Add(CheckPossibleMove(piece, pieceIndex, kingBackOp, playerBoard, enemyBoard, player, (GameBoard.RowLength * 2) - nextRowDiagonalMoveOdd, true));
                    }

                    break;
            }

            // remove all 0s reprenseing no unavailable/impossible moves for piece (other values are possible moves)
            possibleMoves.RemoveAll(move => move.Where == 0);

            return possibleMoves;

        }


        /// <summary>
        /// checks if a certain move is possible for given piece 
        /// </summary>
        /// <param name="piece"> UInt32 represnting the piece to check move for </param>
        /// <param name="pieceIndexCheck"> used for checking the location of the piece (if it divides by 4 with no remainder the move is not possible for the given piece) </param>
        /// <param name="op"> bit shift operation to be made on the piece for the move </param>
        /// <param name="playerBoard"> the player that the piece belongs to board </param>
        /// <param name="enemyBoard"> the enemy player's board </param>
        /// <param name="player"> the player's color PlayerColor.WHITE/PlayerColor.BLACK </param>
        /// <param name="move"> the number of bits to move the piece to in the op direction if given move is possible </param>
        /// <param name="backWardsMove"> Specifies if the move being checked if a move forwards or a king's move backwards(Important for sending parameters to CheckPossibleEat function) </param>
        /// <returns> returns a mask representing the possible move for given piece or 0 if no possible move for the given piece in the given direction is available </returns>
        private PieceMove CheckPossibleMove(UInt32 piece, int pieceIndexCheck, UInt32Funcs.shift_operation op, UInt32 playerBoard, UInt32 enemyBoard, PlayerColor player, int move, bool backWardsMove)
        {
            UInt32 possibleMove = op(piece, move);
            if (((pieceIndexCheck) % 4 != 0) && ((possibleMove & playerBoard) == 0)) // Possible and location free of player pieces
            {
                // check if there is an enemy piece already in the location
                if ((possibleMove & enemyBoard) == 0) // location free of enemy pieces
                {
                    return new PieceMove(possibleMove, false, 0); // return possible available regular move 
                }
                else // check if enemy piece is edible
                {
                    // If regular forwards eat is made
                    if (!backWardsMove)
                    {
                        // return possbile eating move
                        return player switch
                        {
                            (PlayerColor.WHITE) => CheckPossibleEat(playerBoard, enemyBoard, possibleMove, move - 1, move + 1, op),
                            (PlayerColor.BLACK) => CheckPossibleEat(playerBoard, enemyBoard, possibleMove, move + 1, move - 1, op),
                            _ => throw new Exception("Invalid player"),
                        };
                    }
                    // If its a backwards eat

                    // return possbile eating move
                    return player switch
                    {
                        (PlayerColor.WHITE) => CheckPossibleEat(playerBoard, enemyBoard, possibleMove, move + 1, move - 1, op),
                        (PlayerColor.BLACK) => CheckPossibleEat(playerBoard, enemyBoard, possibleMove, move - 1, move + 1, op),
                        _ => throw new Exception("Invalid player"),
                    };


                }
            }

            // move not possible/available, return 0 representing no available location on the board for the move
            return new PieceMove(0, false, 0);
        }


        /// <summary>
        /// checks if the given enemy piece (possibleMove) is edible 
        /// </summary>
        /// <param name="playerBoard"> the current player's board </param>
        /// <param name="enemyBoard"> the enemy player's board </param>
        /// <param name="possiblyEdible"> the enemy piece that might be edible </param>
        /// <param name="moveEven"> how many bits to move past the possiblyEdible enemy piece to check if its truly edible if the possiblyEdible piece is on an even row </param>
        /// <param name="moveOdd"> how many bits to move past the possiblyEdible enemy piece to check if its truly edible if the possiblyEdible piece is on an odd row </param>
        /// <param name="op"> bit shift operation to be made on the possiblyEdible piece to check if its truly edible and a mask representing where the eating piece will mvoe to if its edible </param>
        /// <returns> if the piece is edible returns where a mask representing where the eating piece will move to and if its not returns 0 representing piece cannot be eaten </returns>
        private PieceMove CheckPossibleEat(UInt32 playerBoard, UInt32 enemyBoard, UInt32 possiblyEdible, int moveEven, int moveOdd, UInt32Funcs.shift_operation op)
        {
            // holds the index of the possibleMove (what index the 1 bit is in it)
            int possibleMoveIndex = System.Numerics.BitOperations.Log2(possiblyEdible);

            // holds the row of the possibleMove (0 to 7)
            int possibleMoveRow = possibleMoveIndex / 4; 

            switch (possibleMoveRow % 2)
            {
                // even row
                case (0):
                    // check if there is an empty square in the next row so this piece can be eaten
                    if (((possibleMoveIndex + 1) % 4 != 0) && ((op(possiblyEdible, moveEven) & enemyBoard) == 0) && ((op(possiblyEdible, moveEven) & playerBoard) == 0)) // enemy piece edible 
                    {
                        return new PieceMove((op(possiblyEdible, moveEven)), true, possiblyEdible); // return where the piece has to move after eating 
                    }
                    break;

                // odd row
                case (1):
                    // check if there is an empty square in the next row so this piece can be eaten
                    if ((possibleMoveIndex % 4 != 0) && ((op(possiblyEdible, moveOdd) & enemyBoard) == 0) && ((op(possiblyEdible, moveOdd) & playerBoard) == 0)) // enemy piece edible 
                    {
                        return new PieceMove((op(possiblyEdible, moveOdd)), true, possiblyEdible); // return where the piece has to move after eating 
                    }
                    break;
            }

            // move not possible/available, return 0 representing no available location on the board for the move
            return new PieceMove(0, false, 0);
        }

    }
}
