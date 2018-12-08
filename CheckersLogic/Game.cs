using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CheckersProgram
{
    public class Game
    {
        private static int[] PlayersScores = new int[Constants.NUM_OF_PLAYERS];
        
        private readonly Board r_GameBoard;

        private readonly Player[] r_Players;

        private Player m_ActivePlayerReference;

        private Step m_LastStep = null;

        public enum eGameStatus
        {
            Draw, Player1Win, Player2Win, GameOn
        }

        private eGameStatus m_GameStatus;

        public Game(string[] i_UserNames, ushort i_SelectedBoardSize, bool i_IsSecondPlayerAI)
        {
            bool initializeGameState;
            r_Players = new Player[] { new Player(ePlayers.Player1, Constants.HUMAN_PLAYER, i_UserNames[(int)ePlayers.Player1]), new Player(ePlayers.Player2, i_IsSecondPlayerAI, i_UserNames[(int)ePlayers.Player2]) };
            r_GameBoard = new Board(i_SelectedBoardSize, r_Players);
            m_ActivePlayerReference = r_Players[(int)ePlayers.Player1];
            m_GameStatus = eGameStatus.GameOn;
            SyncGameState(out initializeGameState);
        }

        public int[] PlayerScores
        {
            get
            {
                return PlayersScores;
            }
        }

        public Player GetActivePlayer
        {
            get
            {
                return m_ActivePlayerReference;
            }
        }

        public Step GetLastStep
        {
            get
            {
                return m_LastStep;
            }
        }

        public void SwitchTurn(bool i_DoubleTurn)
        {
            if (!i_DoubleTurn)
            {
                if (m_ActivePlayerReference == r_Players[(int)ePlayers.Player1])
                {
                    m_ActivePlayerReference = r_Players[(int)ePlayers.Player2];
                }
                else if (m_ActivePlayerReference == r_Players[(int)ePlayers.Player2])
                {
                    m_ActivePlayerReference = r_Players[(int)ePlayers.Player1];
                }
            }
        }

        public bool DetermineGameStatus(bool i_IsLosingPlayerRetired)
        {
            bool isGameInstanceOver = false;
            if (i_IsLosingPlayerRetired)
            {
                isGameInstanceOver = true;
                determineRetirementCaseWinningPlayer();
            }
            else
            {
                isGameInstanceOver = determineNaturalCaseWinningPlayer();
            }

            return isGameInstanceOver;
        }

        public eGameStatus GetGameStatus
        {
            get
            {
                return m_GameStatus;
            }
        }

        public void PrintBoardToScreen()
        {
            r_GameBoard.DrawCheckersBoard();
        }

        public bool IsActivePlayerHuman(ePlayers i_ActivePlayerIndex)
        {
            return !r_Players[(int)i_ActivePlayerIndex].isAI;
        }

        public bool DoesContainAPiece(string i_MoveString)
        {
            string[] sourceAndDest = i_MoveString.Split('>');
            return r_GameBoard.GetBoardSlotFromIdentifierString(sourceAndDest[Constants.MOVE_SOURCE]).DoesContainsPiece();
        }

        public bool IfBelongsToActivePlayer(string i_MoveString)
        {
            string[] sourceAndDest = i_MoveString.Split('>');
            int comparisonValue;
            bool isPieceFound = false;

            foreach (Piece PlayerPiece in m_ActivePlayerReference.PlayerPieces)
            {
                comparisonValue = string.Compare(sourceAndDest[Constants.MOVE_SOURCE], PlayerPiece.convertCoordinatesToBoardSlotIdentifier());
                if (comparisonValue == Constants.MATCH)
                {
                    isPieceFound = true;
                }
            }

            return isPieceFound;
        }

        public void UpdateScores()
        {
            int player1Score, player2Score;
            int[] PlayerPiecesValue = new int[2];
            for (int i = 0; i < Constants.NUM_OF_PLAYERS; i++)
            {
                PlayerPiecesValue[i] = r_Players[i].CalculatePlayerPiecesTotalValue();
            }

            player1Score = PlayerPiecesValue[(int)ePlayers.Player1] - PlayerPiecesValue[(int)ePlayers.Player2];
            player2Score = PlayerPiecesValue[(int)ePlayers.Player2] - PlayerPiecesValue[(int)ePlayers.Player1];
            if (player1Score > 0)
            {
                PlayersScores[(int)ePlayers.Player1] += player1Score;
            }
            else
            {
                PlayersScores[(int)ePlayers.Player2] += player2Score;
            }
        }

        public bool checkIfStepIsInAvailableSteps(string i_MoveToExecute, out bool o_isJump)
        {
            o_isJump = Constants.UNKNOWN;
            bool isStepFound = m_ActivePlayerReference.IsStepAvailable(new Step(i_MoveToExecute), ref o_isJump);
            return isStepFound;
        }

        public void ExecuteStep(string i_StepToExecute, out bool o_isJump)
        {
            Step stepToExecute = new Step(i_StepToExecute);
            r_GameBoard.UpdateStepInBoard(stepToExecute, m_ActivePlayerReference.PlayerIdentifier, GetOpponentPlayerRef());
            o_isJump = stepToExecute.isJump;
            m_LastStep = stepToExecute;
        }

        public void SyncGameState(out bool o_IsJumpExhaustionNeeded)
        {
            o_IsJumpExhaustionNeeded = false;
            foreach (Player player in r_Players)
            {
                player.SyncPlayerPiecesState(r_GameBoard);
            }

            detectDoubleJumpForActivePlayer(out o_IsJumpExhaustionNeeded);
        }

        private void detectDoubleJumpForActivePlayer(out bool o_IsJumpExhaustionNeeded)
        {
            List<Piece> activePlayerPieces = this.m_ActivePlayerReference.PlayerPieces;
            o_IsJumpExhaustionNeeded = false;
            if (this.m_LastStep != null)
            {
                if (this.m_LastStep.isJump)
                {
                    Step lastStep = this.m_LastStep;
                    Point pieceLocation = m_LastStep.GetDestination;
                    Piece piece = r_GameBoard.GetBoardSlotFromIdentifierString(pieceLocation.ToString()).GetBoardSlotPiece();
                    if (piece.IsJumpAvailable())
                    {
                        m_ActivePlayerReference.SweepOtherPiecesJumps(piece);
                        o_IsJumpExhaustionNeeded = true;
                    }
                }
            }
        }

        public Player GetOpponentPlayerRef()
        {
            Player opponentPlayerRef;
            if (m_ActivePlayerReference.PlayerIdentifier == ePlayers.Player1)
            {
                opponentPlayerRef = r_Players[(int)ePlayers.Player2];
            }
            else
            {
                opponentPlayerRef = r_Players[(int)ePlayers.Player1];
            }

            return opponentPlayerRef;
        }

        public string GetLastStepPlayerName()
        {
            string playerName = string.Empty;
            ePlayers StepOf = ePlayers.UndefinedPlayer;
            foreach (Player player in r_Players)
            {
                player.IsPieceAvailable(m_LastStep.GetDestination, out StepOf);
                if (StepOf != ePlayers.UndefinedPlayer)
                {
                    break;
                }
            }

            if (StepOf == ePlayers.Player1)
            {
                playerName = r_Players[(int)ePlayers.Player1].GetPlayerName;
            }
            else
            {
                playerName = r_Players[(int)ePlayers.Player2].GetPlayerName;
            }

            return playerName;
        }

        public bool IsAITurn()
        {
            return m_ActivePlayerReference.isAI;
        }

        public string RandomStep()
        {
            int randomMoveablePieceIndex, stepIndex;
            int numOfStepsForRandomMoveablePiece;
            string randomStepToExecute = string.Empty;
            Random random = new Random();

            List<Piece> piecesWithSteps = m_ActivePlayerReference.GetMoveablePieces();
            randomMoveablePieceIndex = random.Next(0, piecesWithSteps.Count);
                  if (piecesWithSteps.Count != 0)
            {
                numOfStepsForRandomMoveablePiece = piecesWithSteps[randomMoveablePieceIndex].GetAvailableSteps.Count;

                stepIndex = random.Next(0, numOfStepsForRandomMoveablePiece);
                randomStepToExecute = piecesWithSteps[randomMoveablePieceIndex].GetAvailableSteps[stepIndex].ToString();
            }
            else
            {
                randomStepToExecute = string.Empty;
            }

            return randomStepToExecute;
        }

        public bool IsRetirementPossible()
        {
            bool isActivePlayerLowerScore;
            if (m_ActivePlayerReference.PlayerIdentifier == ePlayers.Player1)
            {
                isActivePlayerLowerScore = m_ActivePlayerReference.CalculatePlayerPiecesTotalValue() < r_Players[(int)ePlayers.Player2].CalculatePlayerPiecesTotalValue();
            }
            else
            {
                isActivePlayerLowerScore = m_ActivePlayerReference.CalculatePlayerPiecesTotalValue() < r_Players[(int)ePlayers.Player1].CalculatePlayerPiecesTotalValue();
            }
            
            return isActivePlayerLowerScore;
        }

        private bool determineNaturalCaseWinningPlayer()
        {
            bool player1NoAvailableSteps = r_Players[0].IsStepAvailableForPlayer();
            bool player2NoAvailableSteps = r_Players[1].IsStepAvailableForPlayer();
            bool isGameOverNaturally = false;
            if (player1NoAvailableSteps && player2NoAvailableSteps)
            {
                m_GameStatus = eGameStatus.Draw;
            }
            else if (player1NoAvailableSteps || player2NoAvailableSteps)
            {
                if (player1NoAvailableSteps)
                {
                    m_GameStatus = eGameStatus.Player2Win;
                }
                else
                {
                    m_GameStatus = eGameStatus.Player1Win;
                }
            }

            if (m_GameStatus != eGameStatus.GameOn)
            {
                isGameOverNaturally = true;
            }

            return isGameOverNaturally;
        }

        private void determineRetirementCaseWinningPlayer()
        {
            if (m_ActivePlayerReference.PlayerIdentifier == ePlayers.Player1)
            {
                m_GameStatus = eGameStatus.Player2Win;
            }
            else
            {
                m_GameStatus = eGameStatus.Player1Win;
            }
        }

        public Board GetGameBoard
        {
            get { return r_GameBoard; }
        } 
    }
}
