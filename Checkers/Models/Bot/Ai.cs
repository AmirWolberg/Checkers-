using System;
using System.Collections.Generic;

namespace Checkers
{
    /// <summary>
    /// Object Representing the AI With the given Heuristic, parameters and Ai color
    /// </summary>
    class Ai
    {
        /// <summary>
        /// Value reaching for inifinity to assign to defaultly assign
        /// </summary>
        private int Infinity = 999999999;


        /// <summary>
        /// Value reaching for -inifinity to assign to defaultly assign
        /// </summary>
        private int NegInfinity = -999999999;

        /// <summary>
        /// The color of the bot
        /// </summary>
        private PlayerColor AiColor;

        /// <summary>
        /// The heuristic function that scores the board in the minimax algorithem
        /// </summary>
        private Heuristics.HeuristicFunc Heuristic;

        /// <summary>
        /// The depth the game tree will reach
        /// </summary>
        private int Depth;

        // The Heuristic funcion's parameters (Scores for diffrent situatons and weights for diffrent pieces)

        /// <summary>
        /// heuristic parameter The weight of a king
        /// </summary>
        private int KingWeight;

        /// <summary>
        /// heuristic parameter The weight of a solider
        /// </summary>
        private int SoliderWeight;

        /// <summary>
        /// heuristic parameter the weight of a possible move
        /// </summary>
        private int PossibleMoveWeight;

        /// <summary>
        /// heuristic parameter The score for winning
        /// </summary>
        private int WinScore;

        /// <summary>
        /// heuristic parameter The score for losing
        /// </summary>
        private int LoseScore;

        /// <summary>
        /// heuristic parameter The score for draw
        /// </summary>
        private int DrawScore;

        /// <summary>
        /// Transposition table saves board states values
        /// size set to 1 million so it wont waste time reallocating memory in smaller depths
        /// </summary>
        private Dictionary<string, TranspositionValue> TranspositionTable = new Dictionary<string, TranspositionValue>(1000000);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AColor"> The color of the bot </param>
        /// <param name="HeuristicFunction"> The heuristic function that scores the board in the minimax algorithem </param>
        /// <param name="DepthTree"> how many moves forwards the bot will predict </param>
        /// <param name="kingWeight"> heuristic parameter The weight of a king </param>
        /// <param name="soliderWeight"> heuristic parameter The weight of a solider </param>
        /// <param name="possibleMoveWeight"> heuristc parameter the weight of a possible move </param>
        /// <param name="winScore"> heuristic parameter The score for winning </param>
        /// <param name="loseScore"> heuristic parameter The score for losing </param>
        /// <param name="drawScore"> heuristic parameter The score for a draw</param>
        public Ai(PlayerColor AColor, Heuristics.HeuristicFunc HeuristicFunction, int DepthTree, int kingWeight, int soliderWeight, int possibleMoveWeight,
            int winScore, int loseScore, int drawScore)
        {
            AiColor = AColor;

            Heuristic = HeuristicFunction;

            Depth = DepthTree;

            KingWeight = kingWeight;

            SoliderWeight = soliderWeight;

            PossibleMoveWeight = possibleMoveWeight;

            WinScore = winScore;

            LoseScore = loseScore;

            DrawScore = drawScore;
        }

        /// <summary>
        /// gets the board and makes the best move it found
        /// </summary>
        /// <param name="m"> the game model</param>
        public void MakeMove(Model m)
        {
            // predict Depth turns into the future 
            MoveAI moveToMake = NegaMax(Depth, AiColor, m, NegInfinity, Infinity);

            m.MakeMove(AiColor, moveToMake.Piece, moveToMake.Where[0]);

            // If turn contains a chain eat, preform chain eating
            int moveNumber = moveToMake.Where.Count;
            for (int move = 1; move < moveNumber; move++)
            {
                m.MakeMove(AiColor, moveToMake.Where[move - 1].Where, moveToMake.Where[move]);
            }
        }

        /// <summary>
        /// Finds best move the bot can make using negamax algorithem (which goes over a game tree representing all possible moves in each given turn in the game up to a certain depth)
        /// </summary>
        /// <param name="depth"> current depth in the game tree (starts as max depth and goes down to 0) </param>
        /// <param name="currentPlayer"> current player being rated </param>
        /// <param name="m"> the model of the game </param>
        /// <param name="alpha"> the lower bound </param>
        /// <param name="beta"> the upper bound </param>
        /// <returns> Returns the optimal value a maximizer can obtain and the move leading to it </returns>
        private MoveAI NegaMax(int depth, PlayerColor currentPlayer, Model m, int alpha,int beta)
        {
            // Get the transposition key for given board
            string TranspositionKey = TranspositionFuncs.GetTranspositionKey(m);

            // If Heuristic For Board was already calculated at an equal or further distance from the leafes as the current depth, Return what has been calculated
            if (TranspositionTable.ContainsKey(TranspositionKey) && TranspositionTable[TranspositionKey].depth >= depth)
            {
                TranspositionValue tableMove = TranspositionTable[TranspositionKey];

                // Preform alpha beta pruning with transposition table
                if (tableMove.flag == TranspositionEnum.EXACT)
                {
                    return tableMove.move;
                }
                else if (tableMove.flag == TranspositionEnum.LOWERBOUND)
                {
                    alpha = Math.Max(alpha, tableMove.move.Heuristic);
                }
                else if (tableMove.flag == TranspositionEnum.UPPERBOUND)
                {
                    beta = Math.Min(beta, tableMove.move.Heuristic);
                }
                if (alpha >= beta)
                {
                    return tableMove.move;
                }
            }

            // Terminating condition. i.e leaf node is reached OR the game is over
            if ((depth <= 0) || (m.CheckGameState() != GameState.OnGoing))
            {
                //Program.UI.DrawBoard(m.GameBoard);
                return new MoveAI(0, new List<PieceMove>(), Heuristic(m, currentPlayer, KingWeight, SoliderWeight, PossibleMoveWeight, WinScore, LoseScore, DrawScore));
            }

            // Define Transposition flag used to do alpha beta pruning with transposition table values
            TranspositionEnum Transflag;

            // Save alpha value at start of function
            int alphaorigin = alpha;

            // Color of next player (for next negamax iteration)
            PlayerColor nextPlayer;

            // Save the model as it was before changes
            Model SaveM = new Model();
            m.CopyModel(SaveM);

            // Keeps details of turn made
            MoveAI turn;

            // List of all moveable pieces by the the current player
            List<UInt32> MoveAblePiecesList;

            // Bestvalue out of all possible moves return values 
            MoveAI BestVal = new MoveAI(0, new List<PieceMove>(), NegInfinity);

            // Get all moveable pieces of current player
            switch (currentPlayer)
            {
                case (PlayerColor.BLACK):
                    MoveAblePiecesList = m.GetMoveAblePieces(m.GameBoard.BP);
                    nextPlayer = PlayerColor.WHITE;
                    break;
                case (PlayerColor.WHITE):
                    MoveAblePiecesList = m.GetMoveAblePieces(m.GameBoard.WP);
                    nextPlayer = PlayerColor.BLACK;
                    break;
                default:
                    throw new Exception("Player color not supported");
            }

            // Go over all possiblemoves and check them out
            foreach (UInt32 MoveAblePiece in MoveAblePiecesList)
            {
                foreach (PieceMove WherePiece in m.GetPossibleMoves(currentPlayer, MoveAblePiece))
                {


                    List<List<PieceMove>> ListOfTurnMoves = new List<List<PieceMove>>();
                    List<PieceMove> tempList = new List<PieceMove>();

                    // Create a duplicate for getting possible moves from
                    Model DuplicateM = new Model();
                    m.CopyModel(DuplicateM);

                    GetPossibleMovesTurn(currentPlayer, DuplicateM, WherePiece, MoveAblePiece, tempList, ListOfTurnMoves);

                    // Get Best value possible on perticular piece moving to a perticular index
                    foreach (List<PieceMove> MoveSequence in ListOfTurnMoves)
                    {

                        // Change module according to move sequence
                        m.MakeMove(currentPlayer, MoveAblePiece, MoveSequence[0]);

                        // Problem here the length of move sequence is not as expected
                        for (int moveIndex = 1; moveIndex < MoveSequence.Count; moveIndex++)
                        {
                            m.MakeMove(currentPlayer, MoveSequence[moveIndex - 1].Where, MoveSequence[moveIndex]);
                        }

                        turn = new MoveAI(MoveAblePiece, MoveSequence, -NegaMax(depth - 1, nextPlayer, m, -beta, -alpha).Heuristic);

                        // Undo move by copying m back to how it was 
                        SaveM.CopyModel(m);

                        // Bestval = max(turn, Bestval)
                        if (turn.Heuristic > BestVal.Heuristic)
                        {
                            BestVal = turn;
                        }

                        // alpha = max(alpha, Bestval)
                        alpha = Math.Max(alpha, BestVal.Heuristic);

                        // alpha beta pruning 
                        if (alpha >= beta)
                        {
                            break;
                        }

                    }
                    // alpha beta pruning 
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
               // alpha beta pruning 
                if (alpha >= beta)
                {
                    break;
                }
            }

            // Set up lookup table with alphabeta pruning of current turn
            if (BestVal.Heuristic <= alphaorigin)
                Transflag = TranspositionEnum.UPPERBOUND;
            else if (BestVal.Heuristic >= beta)
                Transflag = TranspositionEnum.LOWERBOUND;
            else
                Transflag = TranspositionEnum.EXACT;

            // This is the move to make at given depth for the bot, goes into transposition table with the depth it describes
            TranspositionTable[TranspositionKey] = new TranspositionValue(BestVal, depth, Transflag);

            return BestVal;
        }

        /// <summary>
        /// Puts in ListOfTurnMoves given to it all the possible plays (chain eats and regular moves) that can be made with the given MoveAblePiece to the specific WherePiece
        /// Using Recursion makes list of every available combination of paths in the chain eating "tree/graph" 
        /// </summary>
        /// <param name="m"> The module representing the state of the game currently </param>
        /// <param name="WherePiece"> Where to move the piece to </param>
        /// <param name="MoveAblePiece"> The piece to move </param>
        /// <param name="MovesList"> List Of current path of chain eating </param>
        /// <param name="ListOfTurnMoves"> List of all combinations of chain eating/moving routes </param>
        /// <param name="Player"> The current player's color </param>
        public void GetPossibleMovesTurn(PlayerColor Player, Model m, PieceMove WherePiece, UInt32 MoveAblePiece, List<PieceMove> MovesList, List<List<PieceMove>> ListOfTurnMoves)
        {
            List<PieceMove> tempList;

            // If next move cannot be chain eaten, save current move and terminate recursion
            if (!m.MakeMove(Player, MoveAblePiece, WherePiece))
            {
                // Add where to piece moving route
                MovesList.Add(WherePiece);

                // save copy of MovesList (dont want to save the same pointer every time)
                tempList = ListFuncs<PieceMove>.CloneList(MovesList);
                ListOfTurnMoves.Add(tempList);

                return;
            }

            // Save the model as it was before changes
            Model SaveM = new Model();
            m.CopyModel(SaveM);

            // Add where to piece moving route
            MovesList.Add(WherePiece);

            // save copy of MovesList (dont want to save the same pointer every time)
            tempList = ListFuncs<PieceMove>.CloneList(MovesList);
            ListOfTurnMoves.Add(tempList);

            // Save the MovesList as it was before adding moves
            List<PieceMove> SaveList = ListFuncs<PieceMove>.CloneList(MovesList);

            // Update the MoveAblePiece to where it has been moved
            MoveAblePiece = WherePiece.Where;

            // All possible Next moves for given new location of piece
            List<PieceMove> PossibleNextEats = m.GetPossibleMoves(Player, MoveAblePiece);

            // all possible eats in possiblemoves list for chain eating
            PossibleNextEats.RemoveAll(move => move.Eat == false);

            // No further eats in the chain, terminate recursion
            if (PossibleNextEats.Count == 0)
            {
                return;
            }

            // Go over each PossibleEat and finds its full eating route 
            foreach (PieceMove tempWherePiece in PossibleNextEats)
            {
                // Recurse until eating route ends
                GetPossibleMovesTurn(Player, m, tempWherePiece, MoveAblePiece, MovesList, ListOfTurnMoves);

                // restore moveslist
                MovesList = ListFuncs<PieceMove>.CloneList(SaveList);

                // restore model
                SaveM.CopyModel(m);
            }

        }
    }
}