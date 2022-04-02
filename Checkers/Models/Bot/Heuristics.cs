using System;
using System.Collections.Generic;

namespace Checkers
{
    /// <summary>
    /// Contains Delegeate definition for a heuritstic function and all heuristic functions made 
    /// </summary>
    static class Heuristics
    {

        /// <summary>
        /// Template for herustic function (also Heuristic function type
        /// Gets the Model representing the game and the color of the bot and returns an integer scoring how good the situation is for the bot
        /// </summary>
        public delegate int HeuristicFunc(Model m, PlayerColor currentPlayer, int KingRatio, int SoliderRatio, int PossibleMoveRatio, int WinScore, int LoseScore, int DrawScore);


        /// <summary>
        /// Score the game situation when called in favor of the bot, Basic heuristic modifyable by user (only uses 6 basic parameters)
        /// </summary>
        /// <param name="m"> The Model representing the game </param>
        /// <param name="currentPlayer"> The current player to score the points of </param>
        /// <param name="KingRatio"> Importance of king piece parameter </param>
        /// <param name="SoliderRatio"> Importance of solider piece parameter </param>
        /// <param name="PossibleMovesRatio"> Importance of a possible move that can be made by the user after the turn parameter </param>
        /// <param name="WinScore"> The Score given to winning the game </param>
        /// <param name="LoseScore"> The score given to Losing the game</param>
        /// <param name="DrawScore"> The score give to a draw in the game </param>
        /// <returns> a score suggesting how good the situation is for the bot (positive is good, negative is bad) </returns>
        public static int BasicHeuristic(Model m, PlayerColor currentPlayer, int KingRatio, int SoliderRatio, int PossibleMovesRatio, int WinScore, int LoseScore, int DrawScore)
        {
            // The score of the game at its current situation
            int Score = 0,
                SoliderScore, KingScore;

            // The color of the currentPlayer's enemy
            PlayerColor enemyPlayer;

            switch (currentPlayer)
            {
                // player is black player
                case (PlayerColor.BLACK):

                    // Set enemyplayer color
                    enemyPlayer = PlayerColor.WHITE;

                    // Total soliders bot has alive minus total soliders player has alive
                    SoliderScore = UInt32Funcs.CountSetBits(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB)) -
                        UInt32Funcs.CountSetBits(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB));

                    // Total kings bot has alive minus total pieces player has alive
                    KingScore = UInt32Funcs.CountSetBits(m.GameBoard.BP.PB & m.GameBoard.K) - UInt32Funcs.CountSetBits(m.GameBoard.WP.PB & m.GameBoard.K);

                    // Check the game state and score accordingly
                    switch (m.CheckGameState())
                    {
                        case (GameState.Draw):
                            Score += DrawScore;
                            break;

                        case (GameState.WhiteWon):
                            Score += LoseScore;
                            break;

                        case (GameState.BlackWon):
                            Score += WinScore;
                            break;

                    }
                    break;

                // player is white player 
                case (PlayerColor.WHITE):

                    // Set enemyplayer color
                    enemyPlayer = PlayerColor.BLACK;

                    // Total soliders bot has alive minus total soliders player has alive
                    SoliderScore = UInt32Funcs.CountSetBits(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB)) - UInt32Funcs.CountSetBits(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB));

                    // Total kings bot has alive minus total pieces player has alive
                    KingScore = UInt32Funcs.CountSetBits(m.GameBoard.WP.PB & m.GameBoard.K) - UInt32Funcs.CountSetBits(m.GameBoard.BP.PB & m.GameBoard.K);

                    // Check the game state and score accordingly
                    switch (m.CheckGameState())
                    {
                        case (GameState.Draw):
                            Score += DrawScore;
                            break;

                        case (GameState.BlackWon):
                            Score += LoseScore;
                            break;

                        case (GameState.WhiteWon):
                            Score += WinScore;
                            break;

                    }
                    break;

                default:
                    throw new Exception("Player color not supported");
            }

            // Multiplied by each respective ratio 
            Score += KingScore * KingRatio + SoliderScore * SoliderRatio;

            // Add possible moves that can be made by given player minus possible moves that can be made by enemy player
            Score += (CountPossibleMoves(m, currentPlayer) - CountPossibleMoves(m, enemyPlayer)) * PossibleMovesRatio;

            return Score;
        }


        /// <summary>
        /// Score the game situation when called in favor of the bot, Extreme heuristic uses a lot more paremeters and takes into consideration more factors
        /// at the cost of being slower, used for extreme gamemode and bot level (Parameters given dont matter)
        /// </summary>
        /// <param name="m"> The Model representing the game </param>
        /// <param name="currentPlayer"> The current player to score the points of  </param>
        /// <param name="KingRatio"> Importance of king piece parameter </param>
        /// <param name="SoliderRatio"> Importance of solider piece parameter </param>
        /// <param name="PossibleMovesRatio"> Importance of a possible move that can be made by the user after the turn parameter </param>
        /// <param name="WinScore"> The Score given to winning the game </param>
        /// <param name="LoseScore"> The score given to Losing the game</param>
        /// <param name="DrawScore"> The score give to a draw in the game </param>
        /// <returns> a score suggesting how good the situation is for the bot (positive is good, negative is bad) </returns>
        public static int ExtremeHeuristic(Model m, PlayerColor currentPlayer, int KingRatio, int SoliderRatio, int PossibleMovesRatio, int WinScore, int LoseScore, int DrawScore)
        {
            DrawScore = -400;
            LoseScore = -500;
            WinScore = 500;
            PossibleMovesRatio = 2;
            SoliderRatio = 5;
            KingRatio = 10;
            int DefendingPromotionLine = 3, KingSafeFromEating = 2, SoliderSafeFromEating = 1;


            // The score of the game at its current situation
            int Score = 0,
                SoliderScore, KingScore;

            // The color of the currentPlayer's enemy
            PlayerColor enemyPlayer;

            switch (currentPlayer)
            {
                // player is black player
                case (PlayerColor.BLACK):

                    // Set enemyplayer color
                    enemyPlayer = PlayerColor.WHITE;

                    // Total soliders bot has alive minus total soliders player has alive
                    SoliderScore = UInt32Funcs.CountSetBits(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB)) -
                        UInt32Funcs.CountSetBits(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB));

                    // Total kings bot has alive minus total pieces player has alive
                    KingScore = UInt32Funcs.CountSetBits(m.GameBoard.BP.PB & m.GameBoard.K) - UInt32Funcs.CountSetBits(m.GameBoard.WP.PB & m.GameBoard.K);

                    // Check the game state and score accordingly
                    switch (m.CheckGameState())
                    {
                        case (GameState.Draw):
                            Score += DrawScore;
                            break;

                        case (GameState.WhiteWon):
                            Score += LoseScore;
                            break;

                        case (GameState.BlackWon):
                            Score += WinScore;
                            break;

                    }

                    // Add how many pieces are safe from being ate (sitting on edge of row) minus how many enemy pieces are safe from being eaten
                    Score += (CountSafePawns(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB)) - CountSafePawns(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB))) * SoliderSafeFromEating;
                    Score += (CountSafePawns(m.GameBoard.BP.PB & m.GameBoard.K) - CountSafePawns(m.GameBoard.WP.PB & m.GameBoard.K)) * KingSafeFromEating;

                    break;

                // player is white player 
                case (PlayerColor.WHITE):

                    // Set enemyplayer color
                    enemyPlayer = PlayerColor.BLACK;

                    // Total soliders bot has alive minus total soliders player has alive
                    SoliderScore = UInt32Funcs.CountSetBits(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB)) - UInt32Funcs.CountSetBits(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB));

                    // Total kings bot has alive minus total pieces player has alive
                    KingScore = UInt32Funcs.CountSetBits(m.GameBoard.WP.PB & m.GameBoard.K) - UInt32Funcs.CountSetBits(m.GameBoard.BP.PB & m.GameBoard.K);

                    // Check the game state and score accordingly
                    switch (m.CheckGameState())
                    {
                        case (GameState.Draw):
                            Score += DrawScore;
                            break;

                        case (GameState.BlackWon):
                            Score += LoseScore;
                            break;

                        case (GameState.WhiteWon):
                            Score += WinScore;
                            break;

                    }

                    // Add how many pieces are safe from being ate (sitting on edge of row) minus how many enemy pieces are safe from being eaten
                    Score += (CountSafePawns(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB)) - CountSafePawns(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB))) * SoliderSafeFromEating;
                    Score += (CountSafePawns(m.GameBoard.WP.PB & m.GameBoard.K) - CountSafePawns(m.GameBoard.BP.PB & m.GameBoard.K)) * KingSafeFromEating;

                    break;

                default:
                    throw new Exception("Player color not supported");
            }

            // Multiplied by each respective ratio 
            Score += KingScore * KingRatio + SoliderScore * SoliderRatio;

            // Add possible moves that can be made by given player minus possible moves that can be made by enemy player
            Score += (CountPossibleMoves(m, currentPlayer) - CountPossibleMoves(m, enemyPlayer)) * PossibleMovesRatio;

            // Add how many pieces are defending the promotion line minus how many pieces are defending the enemy promotion line
            Score += (CountPromotionLine(m, currentPlayer) - CountPromotionLine(m, enemyPlayer)) * DefendingPromotionLine;

            return Score;
        }

        /// <summary>
        /// Score the game situation in favor of the bot in an adaptive manner, for example the less pieces a player has the more valueable the pieces are to the player (Parameters given dont matter)
        /// </summary>
        /// <param name="m"> The Model representing the game </param>
        /// <param name="currentPlayer"> The current player to score the points of </param>
        /// <param name="KingRatio"> Importance of king piece parameter </param>
        /// <param name="SoliderRatio"> Importance of solider piece parameter </param>
        /// <param name="PossibleMovesRatio"> Importance of a possible move that can be made by the user after the turn parameter </param>
        /// <param name="WinScore"> The Score given to winning the game </param>
        /// <param name="LoseScore"> The score given to Losing the game</param>
        /// <param name="DrawScore"> The score give to a draw in the game </param>
        /// <returns> a score suggesting how good the situation is for the bot (positive is good, negative is bad) </returns>
        public static int AdaptiveHeuristic(Model m, PlayerColor currentPlayer, int KingRatio, int SoliderRatio, int PossibleMovesRatio, int WinScore, int LoseScore, int DrawScore)
        {
            DrawScore = -4000;
            LoseScore = -5000;
            WinScore = 5000;
            PossibleMovesRatio = 8;
            SoliderRatio = 5;
            KingRatio = 10;

            // The score of the game at its current situation
            int Score = 0;
            float
                WhiteSoliderScore = UInt32Funcs.CountSetBits(m.GameBoard.WP.PB ^ (m.GameBoard.K & m.GameBoard.WP.PB))
                , WhiteKingScore = UInt32Funcs.CountSetBits(m.GameBoard.WP.PB & m.GameBoard.K)
                , BlackSoliderScore = UInt32Funcs.CountSetBits(m.GameBoard.BP.PB ^ (m.GameBoard.K & m.GameBoard.BP.PB))
                , BlackKingScore = UInt32Funcs.CountSetBits(m.GameBoard.BP.PB & m.GameBoard.K);

            // [PlayerColor]State Is the ratio representing how much a piece is worth to the bot which depends on how many pieces the bot has left
            float WhiteState = (12 / (WhiteSoliderScore + WhiteKingScore + 1)), BlackState = (12 / (BlackSoliderScore + BlackKingScore + 1));
            WhiteSoliderScore *= WhiteState;
            WhiteKingScore *= WhiteState;
            BlackSoliderScore *= BlackState;
            BlackKingScore *= BlackState;

            // The color of the currentPlayer's enemy
            PlayerColor enemyPlayer;

            switch (currentPlayer)
            {
                // player is black player
                case (PlayerColor.BLACK):

                    // Set enemyplayer color
                    enemyPlayer = PlayerColor.WHITE;

                    // Add King + Solider Scores with their ratios
                    Score += (int)((BlackKingScore - WhiteKingScore) * KingRatio + (BlackSoliderScore - WhiteSoliderScore) * SoliderRatio);

                    // Check the game state and score accordingly
                    switch (m.CheckGameState())
                    {
                        case (GameState.Draw):
                            Score += DrawScore;
                            break;

                        case (GameState.WhiteWon):
                            Score += LoseScore;
                            break;

                        case (GameState.BlackWon):
                            Score += WinScore;
                            break;

                    }

                    break;

                // player is white player 
                case (PlayerColor.WHITE):

                    // Set enemyplayer color
                    enemyPlayer = PlayerColor.BLACK;

                    // Add King + Solider Scores with their ratios
                    Score += (int)((WhiteKingScore - BlackKingScore) * KingRatio + (WhiteSoliderScore - BlackSoliderScore) * SoliderRatio);

                    // Check the game state and score accordingly
                    switch (m.CheckGameState())
                    {
                        case (GameState.Draw):
                            Score += DrawScore;
                            break;

                        case (GameState.BlackWon):
                            Score += LoseScore;
                            break;

                        case (GameState.WhiteWon):
                            Score += WinScore;
                            break;

                    }
                    break;

                default:
                    throw new Exception("Player color not supported");
            }

            // Add possible moves that can be made by given player minus possible moves that can be made by enemy player
            Score += (CountPossibleMoves(m, currentPlayer) - CountPossibleMoves(m, enemyPlayer)) * PossibleMovesRatio;

            return Score;
        }

        /// <summary>
        /// Counts how many legal moves the given player can preform
        /// </summary>
        /// <param name="m"> The game model to check </param>
        /// <param name="color"> the given player color </param>
        /// <returns> the number of legal moves the given player can preform </returns>
        private static int CountPossibleMoves(Model m, PlayerColor color)
        {
            // List of all moveable pieces by the bot
            List<UInt32> MoveAblePiecesList;
            // The number of legal moves the bot can make (doesn't count all diffrent types of chain eating moves)
            int NumOfLegalMoves = 0;

            // Get the list of all moveale pieces by the given bot playerColor
            switch (color)
            {
                case (PlayerColor.BLACK):
                    MoveAblePiecesList = m.GetMoveAblePieces(m.GameBoard.BP);
                    break;
                case (PlayerColor.WHITE):
                    MoveAblePiecesList = m.GetMoveAblePieces(m.GameBoard.WP);
                    break;
                default:
                    throw new Exception("Player color not supported");
            }

            // Go over all possiblemoves and update the legal moves counter accordingly
            foreach (UInt32 MoveAblePiece in MoveAblePiecesList)
            {
                NumOfLegalMoves += m.GetPossibleMoves(color, MoveAblePiece).Count;
            }

            return NumOfLegalMoves;
        }

        /// <summary>
        /// Count how many pieces are on the player's promotion line
        /// </summary>
        /// <param name="m"> the game model holding the board </param>
        /// <param name="player"> the player color </param>
        /// <returns></returns>
        private static int CountPromotionLine(Model m, PlayerColor player)
        {
            // Counter of how many pieces are on the promition line
            int counterPiecesOnLine = 0;

            // Act differently depending on player
            switch (player)
            {
                // Need to count right most 4 bits on black board
                case (PlayerColor.BLACK):

                    UInt32 checkMaskBlack = 0b_00000000000000000000000000000001;
                    for(int Piece = 0; Piece < 4; Piece++)
                    {
                        if((m.GameBoard.BP.PB & checkMaskBlack) != 0)
                        {
                            counterPiecesOnLine++;
                        }

                        // Move mask 1 bit to the left
                        checkMaskBlack <<= 1;
                    }
                    break;
                
                // Need to count left most 4 bits on white board
                case (PlayerColor.WHITE):
                    UInt32 checkMaskWhite = 0b_10000000000000000000000000000000;
                    for (int Piece = 0; Piece < 4; Piece++)
                    {
                        if ((m.GameBoard.WP.PB & checkMaskWhite) != 0)
                        {
                            counterPiecesOnLine++;
                        }

                        // Move mask 1 bit to the left
                        checkMaskWhite >>= 1;
                    }
                    break;

                default:
                    throw new Exception("Player color not supported");
            }

            return counterPiecesOnLine;

        }

        /// <summary>
        /// Count how many pawns on the given board are safe (cannot be eaten)
        /// </summary>
        /// <param name="Board"> Uint32 Representing the board of the player </param>
        /// <returns> how many pawns are safe and cannot be eaten </returns>
        private static int CountSafePawns(UInt32 Board)
        {
            // counts how many pawns are in the corner and cannot be eaten therefor are safe
            int safePawnCounter = 0;

            UInt32 safePawnMask = 0b_00000000000000000000000000001000;

            // Go over board and count safe pawns, go couple of rows at a time one odd and one even (rows are going form 0 - 7)
            for (int rowCoupleIndex = 0; rowCoupleIndex<4; rowCoupleIndex++)
            {
                // Even row left most is safe
                if((Board & safePawnMask) != 0)
                {
                    safePawnCounter++;
                }

                safePawnMask <<= 1;

                // Odd row right most is safe
                if ((Board & safePawnMask) != 0)
                {
                    safePawnCounter++;
                }

                safePawnMask <<= 7;
            }

            return safePawnCounter;
        }
    }
}
