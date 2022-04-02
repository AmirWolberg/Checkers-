using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Checkers
{
    /// <summary>
    /// UserInterface takes care of all interaction of the game with the user and all visuals In addition to connecting the models (the game logic) In the correct way
    /// </summary>
    public partial class UserInterface : Form
    {
        /// <summary>
        /// Constructor initializes component 
        /// </summary>
        public UserInterface()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Screen resulotion used to place widgest in the corrent place
        /// </summary>
        private static Rectangle resolution = Screen.PrimaryScreen.Bounds;

        // Widgets

        /// <summary>
        /// Button designed to offer option to stop chain eating
        /// </summary>
        private Button ChainStopButton = new Button();

        // logical parameters for running a game course

        /// <summary>
        /// Model holding the game's logic
        /// </summary>
        private Model M = new Model();

        /// <summary>
        /// The Color of the current Player (that player that is now making his turn)
        /// </summary>
        private PlayerColor currentPlayer;

        /// <summary>
        /// The game mode chosen (PvP/PvB/BvB)
        /// </summary>
        private string Mode;

        /// <summary>
        /// List of Possible Moves rows and collumns for drawing the board with highlighted possbile moves for latest clicked piece
        /// </summary>
        private List<Coordinates> PossibleMovesCoordinates = new List<Coordinates>();

        /// <summary>
        /// List of logical Possible moves for making a move if a possible move square has been clicked
        /// </summary>
        private List<PieceMove> MovesList = new List<PieceMove>();

        /// <summary>
        /// Latest clicked valid piece (piece to move if possible move square has been clicked)
        /// </summary>
        private UInt32 PieceToMove = 0;

        /// <summary>
        /// True if player is in the middle of making a move (already chosen a piece and is now chossing a possible move) false otherwise
        /// </summary>
        private bool midMove = false;

        /// <summary>
        /// turns true if a player is in the proccess of chain eating
        /// </summary>
        private bool inChain = false;

        /// <summary>
        /// used for chain eating holds the location of the eating piece
        /// </summary>
        private UInt32 eatingPiece = 0;

        // Graphical constants for placement/size

        /// <summary>
        /// size of each square on the board (changes depending on resolution)
        /// 100% size
        /// </summary>
        private static readonly int size = (int)(resolution.Width * 0.033) + (int)(resolution.Height * 0.03);

        /// <summary>
        /// how many squares are in each row and collumn
        /// </summary>
        private static readonly int numOfSquares = 8;

        /// <summary>
        /// distance of board from edge of screen (changes depending on resolution)
        /// 80% of size
        /// </summary>
        private static readonly int distanceFromEdge = (int)(size*0.8);

        /// <summary>
        /// The diameter of each piece (changes depending on resolution)
        /// 40% of size
        /// </summary>
        private static readonly int pieceDiameter = (int)(size*0.4);

        // functions

        /// <summary>
        /// Draw the game board and its pieces as it is to the screen
        /// </summary>
        /// <param name="b"> The game board </param>
        public void DrawBoard(Board b)
        {
            // color of painted pieces/squares
            
            // all back colors
            System.Drawing.SolidBrush myBrushBlackPiece = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.SolidBrush myBrushBlackKing = new System.Drawing.SolidBrush(System.Drawing.Color.DarkRed);
            Pen myPenBlackPiece = new Pen(Brushes.White);
            Pen myPenBlackKing = new Pen(Brushes.Gold);

            // all white colors
            System.Drawing.SolidBrush myBrushWhitePiece = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.SolidBrush myBrushWhiteKing = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);
            Pen myPenWhitePiece = new Pen(Brushes.Black);
            Pen myPenWhiteKing = new Pen(Brushes.Silver);

            // Paints empty squares
            System.Drawing.SolidBrush myBrushEmpty = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            // Paints possible moves squares
            System.Drawing.SolidBrush myBrushPossibleMove = new System.Drawing.SolidBrush(System.Drawing.Color.Green);

            // Set formGraphics
            System.Drawing.Graphics formGraphics = this.CreateGraphics();

            // String form of all game boards used to place pieces on the graphical board
            string black_board = UInt32Funcs.Make32Bit(b.BP.PB), white_board = UInt32Funcs.Make32Bit(b.WP.PB), king_board = UInt32Funcs.Make32Bit(b.K);

            // index on the translated bit board
            int indexBitBoard = 0;

            // draw board
            for (int row = 0; row < numOfSquares; row++)
            {
                for (int col = 0; col < numOfSquares; col++)
                {
                    // if its a black square draw it, otherwise color is already white
                    if (((col % 2 != 0) && (row % 2 == 0)) || ((col % 2 == 0) && (row % 2 != 0)))
                    {

                        // draw black square
                        formGraphics.FillRectangle(myBrushEmpty, new Rectangle(col * size + distanceFromEdge, row * size + distanceFromEdge, size, size));

                        // black piece 
                        if (black_board[indexBitBoard].Equals('1'))
                        {
                            if (king_board[indexBitBoard].Equals('1'))
                            {
                                // Draw Black King
                                formGraphics.DrawCircle(myPenBlackKing, col * size + distanceFromEdge + size / 2, row * size + distanceFromEdge + size / 2, pieceDiameter + 1);
                                formGraphics.FillCircle(myBrushBlackKing, col * size + distanceFromEdge + size / 2, row * size + distanceFromEdge + size / 2, pieceDiameter);
                            }
                            else
                            {
                                // Draw Black Solider
                                formGraphics.DrawCircle(myPenBlackPiece, col * size + distanceFromEdge + size/2, row * size + distanceFromEdge + size/2, pieceDiameter + 1);
                                formGraphics.FillCircle(myBrushBlackPiece, col * size + distanceFromEdge + size/2, row * size + distanceFromEdge + size/2, pieceDiameter);
                            }
                        }

                        // white piece
                        else if (white_board[indexBitBoard].Equals('1'))
                        {
                            if (king_board[indexBitBoard].Equals('1'))
                            {
                                // Draw White King
                                formGraphics.DrawCircle(myPenWhiteKing, col * size + distanceFromEdge + size / 2, row * size + distanceFromEdge + size / 2, pieceDiameter+ 1);
                                formGraphics.FillCircle(myBrushWhiteKing, col * size + distanceFromEdge + size / 2, row * size + distanceFromEdge + size / 2, pieceDiameter);
                            }
                            else
                            {
                                // Draw White Solider
                                formGraphics.DrawCircle(myPenWhitePiece, col * size + distanceFromEdge + size / 2, row * size + distanceFromEdge + size / 2, pieceDiameter+ 1);
                                formGraphics.FillCircle(myBrushWhitePiece, col * size + distanceFromEdge + size / 2, row * size + distanceFromEdge + size / 2, pieceDiameter);
                            }
                        }

                        // Increase bit board index
                        indexBitBoard++;
                    }
                }

            }

            // Dispose all used pens and brushes
            myBrushEmpty.Dispose();
            myBrushBlackPiece.Dispose();
            myBrushWhitePiece.Dispose();
            myPenBlackKing.Dispose();
            myPenWhiteKing.Dispose();
            myPenWhitePiece.Dispose();
            myBrushBlackPiece.Dispose();

            // Draw border around board (black border)
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            formGraphics.DrawLine(pen, distanceFromEdge, distanceFromEdge, size * numOfSquares + distanceFromEdge, distanceFromEdge);
            formGraphics.DrawLine(pen, distanceFromEdge, distanceFromEdge, distanceFromEdge, size * numOfSquares + distanceFromEdge);
            formGraphics.DrawLine(pen, distanceFromEdge, size * numOfSquares + distanceFromEdge, size * numOfSquares + distanceFromEdge, size * numOfSquares + distanceFromEdge);
            formGraphics.DrawLine(pen, size * numOfSquares + distanceFromEdge, distanceFromEdge, size * numOfSquares + distanceFromEdge, size * numOfSquares + distanceFromEdge);

            // Dispose used pen
            pen.Dispose();


            // Draw Possible moves
            foreach(Coordinates move in PossibleMovesCoordinates)
            {
                // draw possible move square
                formGraphics.FillRectangle(myBrushPossibleMove, new Rectangle(move.col * size + distanceFromEdge, move.row * size + distanceFromEdge, size, size));
            }

            // Dispose of brush and formgraphics (finished using them)
            formGraphics.Dispose();
            myBrushPossibleMove.Dispose();
        }

        /// <summary>
        /// When mouse is clicked takes care of what happens
        /// </summary>
        /// <param name="e"> mouse click event </param>
        protected override void OnClick(EventArgs e)
        {
            // call base OnClick
            base.OnClick(e);

            // Save the current Coordinates of the mouse (x and y)
            int CurrentX = MousePosition.X;
            int CurrentY = MousePosition.Y;

            // Translate the current Coordinates to collumn and row in the board
            int col = (CurrentX - distanceFromEdge) / size;
            int row = (CurrentY - distanceFromEdge) / size;

            // Check which game mode is on and act accordingly
            switch (Mode)
            {
                // Player Versus Player mode
                case ("PvP"):

                    PlayerTurn(row, col);

                    // Check if game is over
                    if(M.CheckGameState() != GameState.OnGoing)
                    {
                        // Show game outcome 
                        MessageBox.Show(M.CheckGameState().ToString());

                        // Game over reset mode
                        Mode = "";
                    }

                    break;

                // Player Versus Bot Mode
                case ("PvB"):

                    // If its the player's turn
                    if (currentPlayer == PlayerPvB && row <=7 && col <=7)
                    {
                        PlayerTurn(row, col);
                    }

                    // Check if game is over
                    if (M.CheckGameState() != GameState.OnGoing)
                    {
                        // Show game outcome 
                        MessageBox.Show(M.CheckGameState().ToString());

                        // Game over reset mode
                        Mode = "";
                    }

                    break;

                // Bot Versus Bot Mode
                case ("BvB"):
                    MessageBox.Show("Bot Versus Bot currently running");
                     break;
            
            }
            
        }

        /// <summary>
        /// Takes care of a player's move
        /// </summary>
        /// <param name="row"> row of clicked piece </param>
        /// <param name="col"> collumn of clicked piece </param>
        private void PlayerTurn(int row, int col)
        {
            // If Square pressed is a black square that may contain a moveable piece
            if ((((col % 2 != 0) && (row % 2 == 0)) || ((col % 2 == 0) && (row % 2 != 0))) && (row < 8 && col < 8))
            {
                // Get piece's index using the row and collumn its on index is from right to left buttom to top 0 to 31 only on the black squares
                int index = ((numOfSquares - 1 - row) * numOfSquares / 2) + (numOfSquares / 2 - (col / 2 + 1));

                // If in the middle of chain eating and need to select piece , only possible selectable piece is the one doing the chain eating
                if (inChain && !midMove)
                {
                    // ask if he wants to quit and switch turn with pop up button or something
                    // GADKNGKADHGKADNGJNADJKGADJKGHAJDHGJADHGKHADJKGHJKADHGJKAHGHJK
                    index = System.Numerics.BitOperations.Log2(eatingPiece);
                }

                UInt32 tempPieceToMove = M.GetPlayerBoard(currentPlayer).GetPiece(index);
                // Get Moves List for given square (if the square has no piece on it gets an empty list)
                List<PieceMove> tempMovesList = M.GetPossibleMoves(currentPlayer, tempPieceToMove);

                // Delete all none eating moves when chain eating
                if (inChain)
                {
                    tempMovesList.RemoveAll((PieceMove move) => move.Eat == false);
                }

                // Temp list holding all moves row and col 
                List<Coordinates> tempCoordinatesMovesList = new List<Coordinates>();

                // if click was made midmove check if what was clicked was a green square (meaning in the MovesList already)
                if (midMove)
                {
                    // True if a move is made
                    bool isMove = false;

                    // Check MovesList (from previous click)
                    foreach (PieceMove piece in MovesList)
                    {
                        // Compare to current Clicked Piece (if current clicked piece was a possiblemove of previous click)
                        if (System.Numerics.BitOperations.Log2(piece.Where) == index)
                        {
                            isMove = true;

                            // Make Move with PieceToMove and the Piece From the MovesList
                            if (M.MakeMove(currentPlayer, PieceToMove, piece))
                            {
                                // If player just entered chain eating button (if statement used to prevent multiple buttons being placed)
                                if (!inChain)
                                {
                                    // Create button allowing to quit chain eating
                                    ChainStopButton = new Button();
                                    ChainStopButton.Location = new Point(size * numOfSquares + distanceFromEdge + 5, (size * (numOfSquares / 2) - size / 2) + distanceFromEdge);
                                    ChainStopButton.Height = size;
                                    ChainStopButton.Width = size;

                                    // Set background and foreground
                                    ChainStopButton.BackColor = Color.DarkRed;
                                    ChainStopButton.ForeColor = Color.White;

                                    ChainStopButton.Text = "End turn";
                                    ChainStopButton.Name = "End Chain";
                                    ChainStopButton.Font = new Font("Georgia", 16);

                                    // Add a Button Click Event handler
                                    ChainStopButton.Click += new EventHandler(ChainStopButton_Click);

                                    Controls.Add(ChainStopButton);
                                }

                                // Player in chain eating turn
                                inChain = true;
                                eatingPiece = piece.Where;

                            }
                            else
                            {
                                inChain = false;
                            }

                        }
                    }

                    if (isMove)
                    {
                        EndTurn();
                        return;
                    }
                }

                // Find every move's piece row and collumn
                foreach (PieceMove piece in tempMovesList)
                {
                    int tempIndex = System.Numerics.BitOperations.Log2(piece.Where);
                    Coordinates temp;

                    // move is on an even row
                    if ((tempIndex / 4 - 1) % 2 == 0)
                    {
                        temp = new Coordinates(numOfSquares - 1 - (tempIndex / 4), (numOfSquares - 1) - (tempIndex % 4 * 2));
                    }

                    // move is on an odd row
                    else
                    {
                        temp = new Coordinates(numOfSquares - 1 - (tempIndex / 4), (numOfSquares - 1) - (tempIndex % 4 * 2) - 1);
                    }

                    tempCoordinatesMovesList.Add(temp);
                }

                // Put the tempMovesList in the PossibleMovesCoordinates for printing the board again with the possible moves marked
                PossibleMovesCoordinates = tempCoordinatesMovesList;

                MovesList = tempMovesList;

                PieceToMove = tempPieceToMove;

                // If the given piece has possible moves it can make redraw the game board
                if (PossibleMovesCoordinates.Count > 0)
                {
                    // ReDraw the game board with the possiblemoves 
                    midMove = true;
                    DrawBoard(M.GameBoard);
                }

            }
        }

        /// <summary>
        /// End Turn of a player, reset all variables and switch player
        /// </summary>
        private void EndTurn()
        {
            // Move made reset all Move lists
            PossibleMovesCoordinates = new List<Coordinates>();

            MovesList = new List<PieceMove>();

            PieceToMove = 0;

            midMove = false;

            DrawBoard(M.GameBoard);

            // If in middle of chain eating do not switch turns
            if (!inChain)
            {
                // Switching player remove stop chain eating button
                Controls.Remove(ChainStopButton);

                currentPlayer = currentPlayer switch
                {
                    (PlayerColor.WHITE) => PlayerColor.BLACK,
                    (PlayerColor.BLACK) => PlayerColor.WHITE,
                    _ => throw new Exception("Invalid player"),
                };
            }
        }

        /// <summary>
        /// Stops chain eating, switches turn and destroys itself
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChainStopButton_Click(object sender, EventArgs e)
        {
            // End the player's turn and switch to other player
            inChain = false;
            eatingPiece = 0;
            EndTurn();

            // Destroy the button 
            Controls.Remove((Control)sender);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Player Vs Player
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Variable used to terminate threads , if its true any running thread will shut down
        /// </summary>
        private bool killThreads = false;

        /// <summary>
        /// Handle PvP (Player Versus Player) game course
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PvP_Click(object sender, EventArgs e)
        {
            // Terimate all running threads
            if ((BotTurnThread != null && BotTurnThread.IsAlive) || (BvBGameCourseThread != null && BvBGameCourseThread.IsAlive))
            {
                killThreads = true;
                // Wait until thread resets the command
                while(killThreads != false)
                {
                    Thread.Sleep(250);
                }
            }

            // Reset all variables used in PvP match
            PossibleMovesCoordinates = new List<Coordinates>();

            MovesList = new List<PieceMove>();

            PieceToMove = 0;

            midMove = false;

            inChain = false;

            eatingPiece = 0;

            // Set game mode
            Mode = "PvP";

            // Create game model
            M = new Model();

            // Draw board
            DrawBoard(M.GameBoard);

            // Set starting player (always white)
            currentPlayer = PlayerColor.WHITE;
            
        }

        /// <summary>
        /// Display game instructions to the user for PvP mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstructionsPvP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("PvP (Player versus Player) is a mode where 2 players from the same computer play checkers against each other. ");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Player Versus Bot
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Any Bot Depth Must be below this depth, beyond this depth it takes too long to make a move
        /// </summary>
        private int MaximumBotDepth = 10;

        /// <summary>
        /// The player's Color in PvB (Default black)
        /// </summary>
        private PlayerColor PlayerPvB = PlayerColor.BLACK;

        /// <summary>
        /// The bot's color in PvB (Default white)
        /// </summary>
        private PlayerColor BotPvB = PlayerColor.WHITE;

        /// <summary>
        /// True if the player chose to be white and false if black
        /// </summary>
        private bool playerWhite = false;

        /// <summary>
        /// Bot in PvB
        /// </summary>
        private Ai Bot;

        /// <summary>
        /// Thread responsible for handling the bot's turn in PvB
        /// </summary>
        private Thread BotTurnThread = null;

        /// <summary>
        /// Enum holding the bot difficulty level, Medium by defauly
        /// </summary>
        private BotLevel DifficultyLevel = BotLevel.HARD;

        /// <summary>
        /// Chosen difficulty level is set as easy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EasyDifficulty_CheckedChanged(object sender, EventArgs e)
        {
            DifficultyLevel = BotLevel.EASY;
            BotDepth.Text = "3";
            BotKingWeight.Text = "4"; 
            BotSoliderWeight.Text = "2";
            BotPMWeight.Text= "1";
            BotWinScore.Text = "100";
            BotLoseScore.Text = "-100";
            BotDrawScore.Text = "-90";

        }

        /// <summary>
        /// Chosen difficulty level is set as medium
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediumDifficulty_CheckedChanged(object sender, EventArgs e)
        {
            DifficultyLevel = BotLevel.MEDIUM;
            BotDepth.Text = "5";
            BotKingWeight.Text = "4";
            BotSoliderWeight.Text = "2";
            BotPMWeight.Text = "1";
            BotWinScore.Text = "100";
            BotLoseScore.Text = "-100";
            BotDrawScore.Text = "-90";
        }

        /// <summary>
        /// Chosen difficulty level is set as hard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardDifficulty_CheckedChanged(object sender, EventArgs e)
        {
            DifficultyLevel = BotLevel.HARD;
            BotDepth.Text = "7";
            BotKingWeight.Text = "4";
            BotSoliderWeight.Text = "2";
            BotPMWeight.Text = "1";
            BotWinScore.Text = "100";
            BotLoseScore.Text = "-100";
            BotDrawScore.Text = "-90";
        }

        /// <summary>
        /// Chosen difficulty level is set as Extreme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExtremeDifficulty_CheckedChanged(object sender, EventArgs e)
        {
            DifficultyLevel = BotLevel.EXTREME;
            BotDepth.Text = "7";
            BotKingWeight.Text = "-";
            BotSoliderWeight.Text = "-";
            BotPMWeight.Text = "-";
            BotWinScore.Text = "-";
            BotLoseScore.Text = "-";
            BotDrawScore.Text = "-";
        }

        /// <summary>
        /// Chosen difficulty level is set as Adaptive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PvBAdaptive_CheckedChanged(object sender, EventArgs e)
        {
            DifficultyLevel = BotLevel.ADAPTIVE;
            BotDepth.Text = "7";
            BotKingWeight.Text = "-";
            BotSoliderWeight.Text = "-";
            BotPMWeight.Text = "-";
            BotWinScore.Text = "-";
            BotLoseScore.Text = "-";
            BotDrawScore.Text = "-";
        }

        /// <summary>
        /// User chose to be white player (meaning bot is black player)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerIsWhite_CheckedChanged(object sender, EventArgs e)
        {
            playerWhite = true;
        }

        /// <summary>
        /// User chose to be black player (meaning bot is white player)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerIsBlack_CheckedChanged(object sender, EventArgs e)
        {
            playerWhite = false;
        }

        /// <summary>
        /// Function handling the bot's turn in PvB
        /// </summary>
        private void BotTurn()
        {
            // if command to kill threads is sent or if game is over terminate thread
            while (M.CheckGameState() == GameState.OnGoing && !killThreads)
            {
                Thread.Sleep(100);
                if (currentPlayer == BotPvB)
                {
                    // If game mode changedl or if game is already over or if command to kill threads is sent terimate thread
                    if (Mode != "PvB" || M.CheckGameState() != GameState.OnGoing || killThreads)
                    {
                        break;
                    }

                    Bot.MakeMove(M);
                    DrawBoard(M.GameBoard);

                    // Game ended terminate thread and show winner
                    if (M.CheckGameState() !=GameState.OnGoing)
                    {
                        MessageBox.Show(M.CheckGameState().ToString());
                        break;
                    }

                    currentPlayer = PlayerPvB;
                }
            }

            // This thread has stopped (been killed)
            killThreads = false;
        }

        /// <summary>
        /// Handle setting up PvB (Player versus Bot)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PvB_Click(object sender, EventArgs e)
        {
            // Terimate all running threads
            if ((BotTurnThread != null && BotTurnThread.IsAlive) || (BvBGameCourseThread != null && BvBGameCourseThread.IsAlive))
            {
                killThreads = true;
                // Wait until thread resets the command
                while (killThreads != false)
                {
                    Thread.Sleep(250);
                }
            }

            // Player chose to be white 
            if(playerWhite)
            {
                PlayerPvB = PlayerColor.WHITE;

                BotPvB = PlayerColor.BLACK;
            }
            // Player chose to be black
            else
            {

                PlayerPvB = PlayerColor.BLACK;

                BotPvB = PlayerColor.WHITE;
            }

            // Heuristic function chosen for the bot
            Heuristics.HeuristicFunc BotChosenHeuristic;

            // If a heuristic that only uses depth is chosen
            bool onlyDepthMatters = false;

            // Set up Bot according to chosen difficulty level (difficulty level effects depth of minimax algorithem)
            switch (DifficultyLevel)
            {
                case (BotLevel.EXTREME):
                    // Extreme Uses a different heuristic which takes into account a lot of extra parameters and has its weights already set
                    BotChosenHeuristic = Heuristics.ExtremeHeuristic;
                    onlyDepthMatters = true;
                    break;

                case (BotLevel.ADAPTIVE):
                    // Adaptive Uses a different heuristic which scores things relative to themselves (and changes scoring for parameters throughout the game)
                    BotChosenHeuristic = Heuristics.AdaptiveHeuristic;
                    onlyDepthMatters = true;
                    break;

                default:
                    // Basic Heuristic (Used for easy/medium/hard difficulties)
                    BotChosenHeuristic = Heuristics.BasicHeuristic;
                    break;
            }

            // Message in exception thrown if given depth was below 1
            string depthErrorMessage = "Depth must be 1 or higher And must be below " + MaximumBotDepth.ToString() + " or it will take too long to calculate move";
            // Set Ai with wanted parameters
            try
            {
                // If given depth is below 1 Or above maximum bot depth
                if (int.Parse(BotDepth.Text) <= 0 || int.Parse(BotDepth.Text) >= MaximumBotDepth)
                    throw new Exception(depthErrorMessage);

                if (onlyDepthMatters)
                {
                    // Bot parameters set as user indicated
                    Bot = new Ai(BotPvB, BotChosenHeuristic, int.Parse(BotDepth.Text), 0, 0, 0, 0, 0, 0);
                }
                else
                {
                    // Bot parameters set as user indicated
                    Bot = new Ai(BotPvB, BotChosenHeuristic, int.Parse(BotDepth.Text),
                    int.Parse(BotKingWeight.Text), int.Parse(BotSoliderWeight.Text), int.Parse(BotPMWeight.Text), int.Parse(BotWinScore.Text), int.Parse(BotLoseScore.Text), int.Parse(BotDrawScore.Text));
                }
            }
            catch (Exception except)
            {
                // Inform user of the error cause
                if (except.Message == depthErrorMessage)
                {
                    // Error was due to depth given being below 1
                    MessageBox.Show(depthErrorMessage + " Default ai values set");
                }

                else
                    // Error was due to invalid type of value entered
                    MessageBox.Show("1 or more values entered were invalid, Default ai values set)");

                // Set default values
                BotDepth.Text = "7";
                BotKingWeight.Text = "4";
                BotSoliderWeight.Text = "2";
                BotPMWeight.Text = "1";
                BotWinScore.Text = "100";
                BotLoseScore.Text = "-100";
                BotDrawScore.Text = "-90";

                // default values if 1 or more of the parameter values were invalid
                Bot = new Ai(BotPvB, BotChosenHeuristic, 7, 4, 2, 1, 100, -100, -90);
            }
   
            // Reset all variables used in PvB match
            PossibleMovesCoordinates = new List<Coordinates>();

            MovesList = new List<PieceMove>();

            PieceToMove = 0;

            midMove = false;

            inChain = false;

            eatingPiece = 0;

            // Set game mode
            Mode = "PvB";

            // Create game model
            M = new Model();

            // Draw board
            DrawBoard(M.GameBoard);

            // Set starting player (always white)
            currentPlayer = PlayerColor.WHITE;

            // Set Bot Turn handler thread
            BotTurnThread = new Thread(BotTurn);
            BotTurnThread.Start();

        }

        /// <summary>
        /// Display game instructions to the user for PvB mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstructionsPvB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("PvB (Player Versus Bot) is a mode where a player and a bot play checkers against each other. \n\n" +
                            "To choose what color the player is tick a color below (default color is black for player and white for bot). \n\n" +
                            "To Choose the difficulty of the bot tick a difficulty level below (defauly difficulty is hard)," +
                            " difficulties effect the depth stat of the bot, The difficulties are: \n\n" +
                            "Easy -> Depth 3. \n" +
                            "Medium -> Depth 5. \n" +
                            "Hard -> Depth 7. \n" +
                            "Extreme -> Depth 7, uses Extreme Heuristic (Hardest Difficulty). \n" +
                            "Adaptive -> Depth 7, uses Adaptive Heuristic. \n\n" +
                            "The User can customize all bot stats, Takes stats from  bot stats below, the customizable stats are as follow: \n" +
                            "   Depth - How many moves ahead will the Bot go over, Recommended under 9. \n" +
                            "   King Weight - How many points is a king piece worth for the bot. \n" +
                            "   Solider Weight - How many points is a Solider piece worth for the bot. \n" +
                            "   PM Weight - How many points is a possible move worth for the bot. \n" +
                            "   Win Score - How many points is a win worth for the bot. \n" +
                            "   Lose Score - How many points is a lose worth for the bot. \n" +
                            "   Draw Score - How many points is a draw worth for the bot. \n \n" +
                            "   Max Depth is set as " + (MaximumBotDepth - 1).ToString() + " for preformance reasons.");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Bot Versus Bot
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Thread handling the game course of a bot versus bot in BvB
        /// </summary>
        private Thread BvBGameCourseThread = null;

        /// <summary>
        /// White Bot heuristic function to use 
        /// </summary>
        private BotHeuristics WhiteHeuristicBvB = BotHeuristics.BASIC;

        /// <summary>
        /// Black Bot heuristic function to use 
        /// </summary>
        private BotHeuristics BlackHeuristicBvB = BotHeuristics.BASIC;

        /// <summary>
        /// White Heuristic chosen as basic (regular heuristic with customer parameters)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicWhite_CheckedChanged(object sender, EventArgs e)
        {
            WhiteHeuristicBvB = BotHeuristics.BASIC;
            WhiteDepth.Text = "7";
            WhiteKingWeight.Text = "4";
            WhiteSoliderWeight.Text = "2";
            WhitePMWeight.Text = "1";
            WhiteWinScore.Text = "100";
            WhiteLoseScore.Text = "-100";
            WhiteDrawScore.Text = "-90";
        }

        /// <summary>
        /// White heuristic chosen as extreme (extremeHeuristic without custom parameters except for depth)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExtremeWhite_CheckedChanged(object sender, EventArgs e)
        {
            WhiteHeuristicBvB = BotHeuristics.EXTREME;
            WhiteDepth.Text = "7";
            WhiteKingWeight.Text = "-";
            WhiteSoliderWeight.Text = "-";
            WhitePMWeight.Text = "-";
            WhiteWinScore.Text = "-";
            WhiteLoseScore.Text = "-";
            WhiteDrawScore.Text = "-";
        }

        /// <summary>
        /// White heuristic chosen as adaptive 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BvBWhiteAdaptive_CheckedChanged(object sender, EventArgs e)
        {
            WhiteHeuristicBvB = BotHeuristics.ADAPTIVE;
            WhiteDepth.Text = "7";
            WhiteKingWeight.Text = "-";
            WhiteSoliderWeight.Text = "-";
            WhitePMWeight.Text = "-";
            WhiteWinScore.Text = "-";
            WhiteLoseScore.Text = "-";
            WhiteDrawScore.Text = "-";
        }

        /// <summary>
        /// Black Heuristic chosen as basic (regular heuristic with customer parameters)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicBlack_CheckedChanged(object sender, EventArgs e)
        {
            BlackHeuristicBvB = BotHeuristics.BASIC;
            BlackDepth.Text = "7";
            BlackKingWeight.Text = "4";
            BlackSoliderWeight.Text = "2";
            BlackPMWeight.Text = "1";
            BlackWinScore.Text = "100";
            BlackLoseScore.Text = "-100";
            BlackDrawScore.Text = "-90";
        }

        /// <summary>
        /// Black heuristic chosen as extreme (extremeHeuristic without custom parameters except for depth)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExtremeBlack_CheckedChanged(object sender, EventArgs e)
        {
            BlackHeuristicBvB = BotHeuristics.EXTREME;
            BlackDepth.Text = "7";
            BlackKingWeight.Text = "-";
            BlackSoliderWeight.Text = "-";
            BlackPMWeight.Text = "-";
            BlackWinScore.Text = "-";
            BlackLoseScore.Text = "-";
            BlackDrawScore.Text = "-";
        }

        /// <summary>
        /// Black heuristic chosen as adaptive 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BvBBlackAdaptive_CheckedChanged(object sender, EventArgs e)
        {
            BlackHeuristicBvB = BotHeuristics.ADAPTIVE;
            BlackDepth.Text = "7";
            BlackKingWeight.Text = "-";
            BlackSoliderWeight.Text = "-";
            BlackPMWeight.Text = "-";
            BlackWinScore.Text = "-";
            BlackLoseScore.Text = "-";
            BlackDrawScore.Text = "-";
        }

        /// <summary>
        /// Takes care of the Bot Versus Bot game course
        /// </summary>
        /// <param name="BotWhite"></param>
        /// <param name="BotBlack"></param>
        private void BvBCourse(Ai BotWhite, Ai BotBlack)
        {
            // holds the color of the current player (white player always starts)
            PlayerColor currentPlayer = PlayerColor.WHITE;

            // while the game is ongoing (not a draw, white win or black win) and while game mode wasn't changed and a kill thread command was not received
            while (M.CheckGameState() == GameState.OnGoing && Mode == "BvB" && !killThreads)
            {
                // handle the turn of the bots
                if (currentPlayer == PlayerColor.WHITE)
                    BotWhite.MakeMove(M);
                else
                    BotBlack.MakeMove(M);

                DrawBoard(M.GameBoard);
                // switch the current player to the other player
                currentPlayer = currentPlayer switch
                {
                    (PlayerColor.WHITE) => PlayerColor.BLACK,
                    (PlayerColor.BLACK) => PlayerColor.WHITE,
                    _ => throw new Exception("Invalid player"),
                };
            }

            // Reset Mode
            if (Mode == "BvB")
            {
                Mode = "";
            }

            // Game ened because a result was reached, display the result
            if(M.CheckGameState() != GameState.OnGoing)
            {
                // Game over display result
                MessageBox.Show(M.CheckGameState().ToString());
            }
            // Thread terminated
            killThreads = false;
        }

        /// <summary>
        /// Handle setting up BvB (Bot Versus Bot) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BvB_Click(object sender, EventArgs e)
        {
            // Terimate all running threads
            if ((BotTurnThread != null && BotTurnThread.IsAlive) || (BvBGameCourseThread != null && BvBGameCourseThread.IsAlive))
            {
                killThreads = true;
                // Wait until thread resets the command
                while (killThreads != false)
                {
                    Thread.Sleep(250);
                }
            }

            // Reset all variables used in PvB/PvP match
            PossibleMovesCoordinates = new List<Coordinates>();

            MovesList = new List<PieceMove>();

            PieceToMove = 0;

            midMove = false;

            inChain = false;

            eatingPiece = 0;

            // Set game mode
            Mode = "BvB";

            // Create game model
            M = new Model();

            // Draw board
            DrawBoard(M.GameBoard);

            // Black bot Ai Set with default values
            Ai BotBlack = new Ai(PlayerColor.BLACK, Heuristics.BasicHeuristic, 7, 4, 2, 1, 100, -100, -90);

            // White bot Ai Set with default values
            Ai BotWhite = new Ai(PlayerColor.WHITE, Heuristics.BasicHeuristic, 7, 4, 2, 1, 100, -100, -90);

            // Message in exception thrown if given depth was below 1
            string depthErrorMessage = "Depth must be 1 or higher And must be below " + MaximumBotDepth.ToString() + " or it will take too long to calculate move";

            // BLACK BOT SET UP

            // Heuristic function chosen for the bot
            Heuristics.HeuristicFunc BlackChosenHeuristic;

            // If a heuristic that only uses depth is chosen
            bool BlackOnlyDepthMatters = false;

            // Set Black bot according to heuristic chosen
            switch (BlackHeuristicBvB)
            {

                case (BotHeuristics.BASIC):
                    BlackChosenHeuristic = Heuristics.BasicHeuristic;
                    break;

                case (BotHeuristics.EXTREME):
                    BlackChosenHeuristic = Heuristics.ExtremeHeuristic;
                    BlackOnlyDepthMatters = true;
                    break;


                case (BotHeuristics.ADAPTIVE):
                    BlackChosenHeuristic = Heuristics.AdaptiveHeuristic;
                    BlackOnlyDepthMatters = true;
                    break;

                default:
                    throw new NotSupportedException("Heuristic type is not supported");

                
            }

            // Set black Ai with wanted parameters
            try
            {
                // If given depth is below 1 Or above maximum bot depth
                if (int.Parse(BlackDepth.Text) <= 0 || int.Parse(BlackDepth.Text) >= MaximumBotDepth)
                    throw new Exception(depthErrorMessage);

                if (BlackOnlyDepthMatters)
                {
                    // Bot parameters set as user indicated
                    BotBlack = new Ai(PlayerColor.BLACK, BlackChosenHeuristic, int.Parse(BlackDepth.Text), 0, 0, 0, 0, 0, 0);
                }
                else
                {
                    // Bot parameters set as user indicated
                    BotBlack = new Ai(PlayerColor.BLACK, BlackChosenHeuristic, int.Parse(BlackDepth.Text),
                    int.Parse(BlackKingWeight.Text), int.Parse(BlackSoliderWeight.Text), int.Parse(BlackPMWeight.Text), int.Parse(BlackWinScore.Text), int.Parse(BlackLoseScore.Text), int.Parse(BlackDrawScore.Text));
                }

            }
            catch (Exception except)
            {
                // Inform user of the error cause
                if (except.Message == depthErrorMessage)
                {
                    // Error was due to depth given being below 1
                    MessageBox.Show(depthErrorMessage + " Default ai values set (Black Ai)");
                }

                else
                    // Error was due to invalid type of value entered
                    MessageBox.Show("1 or more values entered were invalid, Default ai values set (Black Ai)");

                // Set values to default
                BlackDepth.Text = "7";
                BlackKingWeight.Text = "4";
                BlackSoliderWeight.Text = "2";
                BlackPMWeight.Text = "1";
                BlackWinScore.Text = "100";
                BlackLoseScore.Text = "-100";
                BlackDrawScore.Text = "-90";

                // default values if 1 or more of the parameter values were invalid
                BotBlack = new Ai(PlayerColor.BLACK, BlackChosenHeuristic, 7, 4, 2, 1, 100, -100, -90);
            }

            // WHITE SET UP

            // Heuristic function chosen for the White bot
            Heuristics.HeuristicFunc WhiteChosenHeuristic;

            // If a heuristic that only uses depth is chosen
            bool WhiteOnlyDepthMatters = false;

            // Set White bot according to heuristic chosen
            switch (WhiteHeuristicBvB)
            {

                case (BotHeuristics.BASIC):
                    WhiteChosenHeuristic = Heuristics.BasicHeuristic;
                    break;

                case (BotHeuristics.EXTREME):
                    WhiteChosenHeuristic = Heuristics.ExtremeHeuristic;
                    WhiteOnlyDepthMatters = true;
                    break;


                case (BotHeuristics.ADAPTIVE):
                    WhiteChosenHeuristic = Heuristics.AdaptiveHeuristic;
                    WhiteOnlyDepthMatters = true;
                    break;

                default:
                    throw new NotSupportedException("Heuristic type is not supported");


            }

            // Set White Ai with wanted parameters
            try
            {
                // If given depth is below 1 Or above maximum bot depth
                if (int.Parse(WhiteDepth.Text) <= 0 || int.Parse(WhiteDepth.Text) >= MaximumBotDepth)
                    throw new Exception(depthErrorMessage);

                if (WhiteOnlyDepthMatters)
                {
                    // Bot parameters set as user indicated
                    BotWhite = new Ai(PlayerColor.WHITE, WhiteChosenHeuristic, int.Parse(WhiteDepth.Text), 0, 0, 0, 0, 0, 0);
                }
                else
                {
                    // Bot parameters set as user indicated
                    BotWhite = new Ai(PlayerColor.WHITE, WhiteChosenHeuristic, int.Parse(WhiteDepth.Text),
                    int.Parse(WhiteKingWeight.Text), int.Parse(WhiteSoliderWeight.Text), int.Parse(WhitePMWeight.Text), int.Parse(WhiteWinScore.Text), int.Parse(WhiteLoseScore.Text), int.Parse(WhiteDrawScore.Text));
                }

            }
            catch (Exception except)
            {
                // Inform user of the error cause
                if (except.Message == depthErrorMessage)
                {
                    // Error was due to depth given being below 1
                    MessageBox.Show(depthErrorMessage + " Default ai values set (White Ai)");
                }

                else
                    // Error was due to invalid type of value entered
                    MessageBox.Show("1 or more values entered were invalid, Default ai values set (White Ai)");

                // Set values to default
                WhiteDepth.Text = "7";
                WhiteKingWeight.Text = "4";
                WhiteSoliderWeight.Text = "2";
                WhitePMWeight.Text = "1";
                WhiteWinScore.Text = "100";
                WhiteLoseScore.Text = "-100";
                WhiteDrawScore.Text = "-90";

                // default values if 1 or more of the parameter values were invalid
                BotWhite = new Ai(PlayerColor.WHITE, WhiteChosenHeuristic, 7, 4, 2, 1, 100, -100, -90);
            }



            BvBGameCourseThread = new Thread(() => BvBCourse(BotWhite, BotBlack));
            BvBGameCourseThread.Start();
        }

        /// <summary>
        /// Display game instructions to the user for BvB mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstructionsBvB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("BvB (Bot Versus Bot) is a mode where 2 Bots play checkers against each other. \n \n" +
                            "Pick Heuristic function type: \n" +
                            "Basic - takes all parameters and only uses them.\n" +
                            "Extreme - only takes depth and takes into consideration more parameters than the ones present.\n" +
                            "Adaptive - only takes depth, Scores relative to the progress of the game, For example the more soliders the bot has the less each solider is worth to it. \n\n" +
                            "It is possible to change the bot's heuristic function stats, the default values are the ones in the text boxes below and may be changed. \n \n" +
                            "Depth - How many moves ahead will the Bot go over, Recommended under 9. \n" +
                            "King Weight - How many points is a king piece worth for the bot. \n" +
                            "Solider Weight - How many points is a Solider piece worth for the bot. \n" +
                            "PM Weight - How many points is a possible move worth for the bot. \n" +
                            "Win Score - How many points is a win worth for the bot. \n" +
                            "Lose Score - How many points is a lose worth for the bot. \n" +
                            "Draw Score - How many points is a draw worth for the bot.\n \n" +
                            "Max Depth is set as " + (MaximumBotDepth - 1).ToString() + " for preformance reasons.");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Other
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     

        /// <summary>
        /// Draws an empty checkers board
        /// </summary>
        private void DrawEmptyBoard(System.Drawing.Graphics formGraphics)
        {
            // Color of painted squares
            System.Drawing.SolidBrush myBrushEmpty = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            // Draw board (black squares)
            for (int row = 0; row < numOfSquares; row++)
            {
                for (int col = 0; col < numOfSquares; col++)
                {
                    // if its a black square draw it, otherwise color is already white (background is white)
                    if (((col % 2 != 0) && (row % 2 == 0)) || ((col % 2 == 0) && (row % 2 != 0)))
                    {
                        // draw black square
                        formGraphics.FillRectangle(myBrushEmpty, new Rectangle(col * size + distanceFromEdge, row * size + distanceFromEdge, size, size));
                    }
                }
            }
            myBrushEmpty.Dispose();
  
            // Draw border around board
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            formGraphics.DrawLine(pen, distanceFromEdge, distanceFromEdge, size * numOfSquares + distanceFromEdge, distanceFromEdge);
            formGraphics.DrawLine(pen, distanceFromEdge, distanceFromEdge, distanceFromEdge, size * numOfSquares + distanceFromEdge);
            formGraphics.DrawLine(pen, distanceFromEdge, size * numOfSquares + distanceFromEdge, size * numOfSquares + distanceFromEdge, size * numOfSquares + distanceFromEdge);
            formGraphics.DrawLine(pen, size * numOfSquares + distanceFromEdge, distanceFromEdge, size * numOfSquares + distanceFromEdge, size * numOfSquares + distanceFromEdge);
            pen.Dispose();
        }

        /// <summary>
        /// Override OnPaint to paint empty checkers board to screen when the Form loads up
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawEmptyBoard(e.Graphics);
        }

        /// <summary>
        /// Display Instructions on how to play checkers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckersInstructions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Checkers Game Instructions \n \n" +
                            "Set Up - \nWhen a gamemode is chosen (BvB/PvB/PvP) the pieces are placed in the 12 dark spots on the bottom and top of the board.\n" +
                            "Each of these three rows has a total of 4 checkers.\n" +
                            "The rows at the top and the bottom are called the King Rows. \n \n" +

                            "Interaction - \nTo move a piece click on it and than click on one of the green squares representing the possible moves the clicked piece can make \n \n" +

                            "Rules- \nThe opponent with the White pieces moves first. \n" +
                            "Solider pieces may only move one diagonal space forward(towards their opponents pieces). \n" +
                            "Pieces must stay on the dark squares.\n" +
                            "To eat an opposing piece, jump over it by moving two diagonal spaces in the direction of the opposing piece. \n" +
                            "A piece may jump forward over an opponent's pieces in multiple parts of the board to eat them (chain eating, may also be stopped at any point using the EndTurn button that will apear). \n" +
                            "the space on the other side of your opponent’s piece must be empty for it to be captured. \n" +
                            "If a piece reaches the last row on its opponent's side, It is crowned as a king piece (black turns red and white turns light gray). \n" +
                            "King pieces may still only move one space at a time during a non - eating move. However, they may move diagonally forward or backwards. \n" + 
                            "There is no limit to how many king pieces a player may have. \n \n" +

                            "The Goal - \n To win one must eat all enemy pieces Or have the enemy have no possible moves left .\n" +
                            "A draw is declared if the number of pieces on the board hasn't changed in 40 turns. ");
        }
    }
}
