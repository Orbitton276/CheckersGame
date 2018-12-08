using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersProgram
{
    public partial class GameWindowForm : Form
    {
        private const int k_TileSize = 60;

        private enum eInputCheckTypes
        {
            Format, Destination, IsPiece, Belongs, SatisfyJump, Retirment, NewInput, InputOk
        }

        private readonly string[] m_UserNames = new string[2];
        private readonly CheckersButton[,] m_FormBoard;
        private ushort m_SelectedBoardSize;
        private ePlayingMode m_SelectedPlayingMode;
        private Game m_Game;

        public GameWindowForm()
        {
            getUserDefinedSettings();
            m_Game = new Game(m_UserNames, m_SelectedBoardSize, isSecondPlayerAI());
            m_FormBoard = new CheckersButton[m_SelectedBoardSize, m_SelectedBoardSize];
            InitializeComponent();
            initializeGameWindow();
        }

        private void initializeGamePlayerAndStats()
        {
            this.label4.Text = m_UserNames[0];
            this.label5.Text = m_UserNames[1];
            this.label7.Text = m_Game.PlayerScores[0].ToString();
            this.label8.Text = m_Game.PlayerScores[1].ToString();
        }

        private void initializeGameWindow()
        {
            initializeGamePlayerAndStats();
            string currentSlotID = string.Empty;
            for (ushort rowIndex = 2; rowIndex < m_SelectedBoardSize + 2; rowIndex++)
            {
                for (ushort colIndex = 0; colIndex < m_SelectedBoardSize; colIndex++)
                {
                    currentSlotID = m_Game.GetGameBoard.GetBoardSlotIdentifier((ushort)(rowIndex - 2), (ushort)colIndex);
                    CheckersButton newPictureBox = new CheckersButton((ushort)(rowIndex - 2), (ushort)colIndex, currentSlotID)
                    {
                        Size = new Size(k_TileSize, k_TileSize),
                        Location = new System.Drawing.Point(k_TileSize * colIndex, k_TileSize * rowIndex)
                    };
                    newPictureBox.Font = new Font(newPictureBox.Font.FontFamily, 15);
                    newPictureBox.Click += new System.EventHandler(this.OnClicked);
                    Controls.Add(newPictureBox);
                    m_FormBoard[rowIndex - 2, colIndex] = newPictureBox;
                    newPictureBox.BorderStyle = BorderStyle.Fixed3D;
                    if (rowIndex % 2 == 0)
                    {
                        newPictureBox.BackgroundImage = colIndex % 2 != 0 ? global::CheckersUI.Properties.Resources.BackColorBright : global::CheckersUI.Properties.Resources.BackColorDark;
                        newPictureBox.BackColor = colIndex % 2 != 0 ? Color.White : Color.DarkGray;
                    }
                    else
                    {
                        newPictureBox.BackgroundImage = colIndex % 2 != 0 ? global::CheckersUI.Properties.Resources.BackColorDark : global::CheckersUI.Properties.Resources.BackColorBright;
                    }
                        
                    if (newPictureBox.BackgroundImage == global::CheckersUI.Properties.Resources.BackColorDark)
                    {
                        newPictureBox.Enabled = false;
                    }
                }
            }
        }

        public void SetPieceByCoordinates(ushort i_XCoord, ushort i_YCoord, string i_Sign)
        {
            // GameBoardSlotPanels[i_YCoord, i_XCoord].Text = i_Sign;
            m_FormBoard[i_XCoord, i_YCoord].SizeMode = PictureBoxSizeMode.StretchImage;
            if (i_Sign == "X")
            {
                m_FormBoard[i_XCoord, i_YCoord].Image = global::CheckersUI.Properties.Resources.WhitePawn;
                m_FormBoard[i_XCoord, i_YCoord].Update();
            }
            else if (i_Sign == "O")
            {
                m_FormBoard[i_XCoord, i_YCoord].Image = global::CheckersUI.Properties.Resources.BlackPawn;
                m_FormBoard[i_XCoord, i_YCoord].Update();
            }
        }

        private CheckersButton m_LastButtonClicked;

        private void getUserDefinedSettings()
        {
            PropertiesForm.userPropertiesForm Form = new PropertiesForm.userPropertiesForm();
            Form.ShowDialog();
            m_SelectedBoardSize = Form.BoardSizeOption;
            m_UserNames[0] = Form.Player1Name;
            m_UserNames[1] = Form.Player2Name;

            if (Form.isSecondPlayerHuman)
            {
                m_UserNames[1] = Form.Player2Name;
                m_SelectedPlayingMode = ePlayingMode.PlayerVsPlayer;
            }
            else
            {
                m_SelectedPlayingMode = ePlayingMode.PlayerVsComputer;
            }

            if (m_UserNames[0] == string.Empty)
            {
                m_UserNames[0] = "Unnamed 1";
            }

            if (m_UserNames[1] == string.Empty)
            {
                m_UserNames[1] = "Unnamed 2";
            }
        }

        private bool isSecondPlayerAI()
        {
            bool isAI = false;
            if (m_SelectedPlayingMode == ePlayingMode.PlayerVsComputer)
            {
                isAI = true;
            }
            else
            {
                isAI = false;
            }

            return isAI;
        }

        public void RunGame()
        {
            drawPlayersOnGameWindow();
            this.ShowDialog();
        }

        private void drawPlayersOnGameWindow()
        {
            foreach (Piece piece in m_Game.GetActivePlayer.PlayerPieces)
            {
                Point currentLocation = piece.Coordinates;
                char xCoord, yCoord;
                xCoord = Convert.ToChar(currentLocation.GetRowCoord);
                xCoord -= 'a';
                yCoord = Convert.ToChar(currentLocation.GetColCoord);
                yCoord -= 'A';

                this.SetPieceByCoordinates(Convert.ToUInt16(xCoord), Convert.ToUInt16(yCoord), piece.GetPieceSignFromPlayerAssociation());
            }

            foreach (Piece piece in m_Game.GetOpponentPlayerRef().PlayerPieces)
            {
                Point currentLocation = piece.Coordinates;
                char xCoord, yCoord;
                xCoord = Convert.ToChar(currentLocation.GetRowCoord);
                xCoord -= 'a';
                yCoord = Convert.ToChar(currentLocation.GetColCoord);
                yCoord -= 'A';

                this.SetPieceByCoordinates(Convert.ToUInt16(xCoord), Convert.ToUInt16(yCoord), piece.GetPieceSignFromPlayerAssociation());
            }
        }

        private bool checkIfPieceClicked()
        {
            return m_Game.GetGameBoard.GetBoardSlotFromIdentifierString(m_LastButtonClicked.GetPositionIdentifier).GetPieceRef() != null;
        }

        private bool checkIfClickedPieceBelongsToOpponent(out bool o_IsPieceClicked)
        {
            o_IsPieceClicked = true;
            bool opponentPieceClicked = false;
            if (checkIfPieceClicked())
            {
                opponentPieceClicked = m_Game.GetGameBoard.GetBoardSlotFromIdentifierString(m_LastButtonClicked.GetPositionIdentifier).GetPieceRef().PlayerAssociation != m_Game.GetActivePlayer.PlayerIdentifier;
            }
            else
            {
                o_IsPieceClicked = false;
            }

            return opponentPieceClicked;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            MoveEventArgs moveWasSpotted;
            bool isPieceClicked;
            if (m_LastButtonClicked == null) 
            {
                m_LastButtonClicked = sender as CheckersButton;
                if (checkIfClickedPieceBelongsToOpponent(out isPieceClicked))
                {
                    MessageBox.Show("Selected piece belongs to opponent", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    m_LastButtonClicked = null;
                }
                else if (!isPieceClicked)
                {
                    MessageBox.Show("Selected tile does not contain a piece", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    m_LastButtonClicked = null;
                }
                else
                {
                    (sender as CheckersButton).BorderStyle = BorderStyle.FixedSingle;
                }
            }
            else
            {
                m_LastButtonClicked.BorderStyle = BorderStyle.Fixed3D;
                if (m_LastButtonClicked == sender)
                {
                    m_LastButtonClicked.BorderStyle = BorderStyle.Fixed3D;
                    m_LastButtonClicked = null;
                }
                else
                {
                    moveWasSpotted = new MoveEventArgs(sender as CheckersButton, m_LastButtonClicked);
                    m_LastButtonClicked = null;
                    OnMoveClicked(this, moveWasSpotted);
                }
            }
        }

        private void gameRoutine(string i_MoveToCheck, MoveEventArgs e, out bool o_IsRoundOver)
        {
            bool ifDoubleTurn;

            bool isJump = new Step(i_MoveToCheck).isJump;

            m_Game.ExecuteStep(i_MoveToCheck, out ifDoubleTurn);
            m_Game.SyncGameState(out ifDoubleTurn);
            updateMoveInWindow(e, isJump);
            m_Game.SwitchTurn(ifDoubleTurn);
            o_IsRoundOver = roundOver(false);

            if (o_IsRoundOver)
            {
                endGameControl();
            }
        }

        private DialogResult declareGameWinner()
        {
            Game.eGameStatus currentGameStatus = m_Game.GetGameStatus;
            DialogResult dialogResultToReturn;

            if (currentGameStatus == Game.eGameStatus.Draw)
            {
                dialogResultToReturn = MessageBox.Show("Draw!\nWould you like to play another game?", "Game over", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            }
            else if (currentGameStatus == Game.eGameStatus.Player1Win)
            {
                dialogResultToReturn = MessageBox.Show(string.Format("Game status: {0} Won\nWould you like to play another game?", m_UserNames[0]), "Game over", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            }
            else
            {
                dialogResultToReturn = MessageBox.Show(string.Format("Game status: {0} Won\nWould you like to play another game?", m_UserNames[1]), "Game over", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            }

            return dialogResultToReturn;
        }

        private void getPBoxByIdentifier(string i_Identifier, out CheckersButton i_PictureBox)
        {
            char xCoord, yCoord;
            xCoord = i_Identifier[1];
            xCoord -= 'a';
            yCoord = i_Identifier[0];
            yCoord -= 'A';
            i_PictureBox = m_FormBoard[xCoord, yCoord];
        }

        private void getSourceAndDestPBoxByMoveString(string i_MoveToAnalyze, out MoveEventArgs AI_MoveEvent)
        {
            CheckersButton sourcePictureBox, destPictureBox;
            string[] sourceAndDest = i_MoveToAnalyze.Split('>');
            getPBoxByIdentifier(sourceAndDest[0], out sourcePictureBox);
            getPBoxByIdentifier(sourceAndDest[1], out destPictureBox);

            AI_MoveEvent = new MoveEventArgs(destPictureBox, sourcePictureBox);
        }

        private void OnMoveClicked(object sender, MoveEventArgs e)
        {
            bool didJumpOverOccured;
            bool isRoundOver = false;
            MoveEventArgs AI_MoveEvent;
            string AI_MoveEventString = string.Empty;
            string MoveToCheck = e.GetSource + '>' + e.GetDestination;

            if (getAndValidateMove(ref MoveToCheck, out didJumpOverOccured))
            {
                gameRoutine(MoveToCheck, e, out isRoundOver);
            }

            if (m_SelectedPlayingMode == ePlayingMode.PlayerVsComputer && !isRoundOver)
            {
                while (m_Game.IsAITurn())
                {
                    System.Threading.Thread.Sleep(750);
                    getAndValidateMove(ref AI_MoveEventString, out didJumpOverOccured);
                    getSourceAndDestPBoxByMoveString(AI_MoveEventString, out AI_MoveEvent);
                    gameRoutine(AI_MoveEventString, AI_MoveEvent, out isRoundOver);
                }
            }
        }

        private void updateMoveInWindow(MoveEventArgs i_MoveArgsElement, bool i_DidJumpOverOccured)
        {
            CheckersButton sourcePBox = m_FormBoard[i_MoveArgsElement.GetSourceRowIndex, i_MoveArgsElement.GetSourceColIndex];
            CheckersButton destPBox = m_FormBoard[i_MoveArgsElement.GetDestRowIndex, i_MoveArgsElement.GetDestColIndex];
            if (i_DidJumpOverOccured)
            {
                CheckersButton pictureBoxToRemovePiece;
                string MoveExecuted = i_MoveArgsElement.GetSource + ">" + i_MoveArgsElement.GetDestination;
                Board gameBoard = m_Game.GetGameBoard;
                BoardSlot slotToRemove = gameBoard.GetBoardSlotFromIdentifierString(gameBoard.GetJumpedOverPieceBoardSlotIdentifier(new Step(MoveExecuted)));
                getPBoxByIdentifier(slotToRemove.GetSlotIdentifier, out pictureBoxToRemovePiece);
                pictureBoxToRemovePiece.Image = null;
                pictureBoxToRemovePiece.Update();
            }

            determineImageAccordingToPieceSign(sourcePBox, destPBox);
        }

        private bool getAndValidateMove(ref string i_MoveToCheck, out bool o_IsJumpOver)
        {
            eInputCheckTypes checkType = eInputCheckTypes.NewInput;
            bool isJumpStep = false;
            string inputString = string.Empty;
            bool isMoveValid = false;

            switch (checkType)
            {
                case eInputCheckTypes.IsPiece:
                    {
                        if (!m_Game.DoesContainAPiece(i_MoveToCheck))
                        {
                            MessageBox.Show("You must select a piece", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            break;
                        }

                        goto case eInputCheckTypes.Belongs;
                    }

                case eInputCheckTypes.Belongs:
                    {
                        if (!m_Game.IfBelongsToActivePlayer(i_MoveToCheck))
                        {
                            MessageBox.Show("Not your piece", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            break;
                        }

                        goto case eInputCheckTypes.Destination;
                    }

                case eInputCheckTypes.Destination:
                    {
                        if (!m_Game.checkIfStepIsInAvailableSteps(i_MoveToCheck, out isJumpStep))
                        {
                            if (m_Game.GetActivePlayer.IsJumpPossible())
                            {
                                goto case eInputCheckTypes.SatisfyJump;
                            }
                            else
                            {
                                MessageBox.Show("Bad destinatiom", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                break;
                            }
                        }
                        else
                        {
                            goto case eInputCheckTypes.InputOk;
                        }
                    }

                case eInputCheckTypes.SatisfyJump:
                    {
                        MessageBox.Show("You must jump over the opponent piece", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }

                case eInputCheckTypes.InputOk:
                    {
                        isMoveValid = true;
                        break;
                    }

                case eInputCheckTypes.NewInput:
                    {
                        if (!m_Game.IsAITurn())
                        {
                            goto case eInputCheckTypes.IsPiece;
                        }
                        else
                        {
                            i_MoveToCheck = m_Game.RandomStep();
                            isMoveValid = true;
                        }

                        break;
                    }
            }

            o_IsJumpOver = isJumpStep;
            return isMoveValid;
        }

        private void determineImageAccordingToPieceSign(CheckersButton i_SourcePBox, CheckersButton i_DestPBox)
        {
            Point sourceDest = m_Game.GetLastStep.GetDestination;
            string sourceDestination = sourceDest.GetColCoord + sourceDest.GetRowCoord;
            BoardSlot sourceBoardSlot = m_Game.GetGameBoard.GetBoardSlotFromIdentifierString(sourceDestination);

            if (m_Game.GetActivePlayer.GetPlayerSign == "X")
            {
                if (sourceBoardSlot.GetPieceRef().GetIsKingBoolean)
                {
                    i_DestPBox.Image = global::CheckersUI.Properties.Resources.WhiteKing;
                }
                else
                {
                    i_DestPBox.Image = global::CheckersUI.Properties.Resources.WhitePawn;
                }
            }
            else
            {
                if (sourceBoardSlot.GetPieceRef().GetIsKingBoolean)
                {
                    i_DestPBox.Image = global::CheckersUI.Properties.Resources.BlackKing;
                }
                else
                {
                    i_DestPBox.Image = global::CheckersUI.Properties.Resources.BlackPawn;
                }
            }

            i_SourcePBox.Image = null;
            i_DestPBox.SizeMode = PictureBoxSizeMode.StretchImage;
            i_DestPBox.Update();
            i_SourcePBox.Update();
        }

        private void endGameControl()
        {
            DialogResult userChoice = declareGameWinner();
            if(userChoice == DialogResult.Yes)
            {
                resetGame();
            }
            else if (userChoice == DialogResult.No)
            {
                this.Close();
                MessageBox.Show("Goodbye!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool roundOver(bool i_IsLosingPlayerRetired)
        {
            bool isGameOver = m_Game.DetermineGameStatus(i_IsLosingPlayerRetired);
            return isGameOver;
        }

        private void resetGame()
        {
            m_Game.UpdateScores();
            m_Game = new Game(m_UserNames, m_SelectedBoardSize, isSecondPlayerAI());
            initializeGamePlayerAndStats();
            resetGameBoardWindow();          
        }

        private void resetGameBoardWindow()
        {
            foreach (var item in m_FormBoard)
            {
                item.Image = null;
            }

            drawPlayersOnGameWindow();
        }
    }
}
