using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public class Player
    {
        private readonly List<Piece> r_PlayerPieces = new List<Piece>();
        private string m_Name;
        private string m_PlayerSign;
        
        private bool m_AI = false;
        ePlayers m_PlayerIdentifier;

        public ePlayers PlayerIdentifier
        {
            get
            {
                return m_PlayerIdentifier;
            }

            set
            {
                m_PlayerIdentifier = value;
            }
        }

        public Player(ePlayers i_PlayerIdentifier, bool i_isAI, string i_Name)
        {
            m_Name = i_Name;
            m_AI = i_isAI;
            m_PlayerIdentifier = i_PlayerIdentifier;
            decidePlayerSign(i_PlayerIdentifier);
        }

        public string GetPlayerName
        {
            get
            {
                return m_Name;
            }
        }

        public string GetPlayerSign
        {
            get
            {
                return m_PlayerSign;
            }
        }

        public void AddPiece(Piece i_PlayerPiece)
        {
            r_PlayerPieces.Add(i_PlayerPiece);
        }

        public List<Piece> PlayerPieces
        {
            get
            {
                return r_PlayerPieces;
            }
        }

        public bool isAI
        {
            get
            {
                return m_AI;
            }
        }

        private void decidePlayerSign(ePlayers i_PlayerIdentifier)
        {
            if (i_PlayerIdentifier == ePlayers.Player1)
            {
                m_PlayerSign = "O";
            }
            else
            {
                m_PlayerSign = "X";
            }
        }

        public int CalculatePlayerPiecesTotalValue()
        {
            int playerPiecesTotalValue = 0;
            foreach (Piece PlayerPiece in r_PlayerPieces)
            {
                playerPiecesTotalValue += PlayerPiece.GetWeight;
            }

            return playerPiecesTotalValue;
        }

        public void SyncPlayerPiecesState(Board i_GameBoard)
        {
            bool jumpFound = false;
            bool isFirstJump = true;
            bool SweepBasicStepsNeccessary = false;
            foreach (Piece PlayerPiece in r_PlayerPieces)
            {
                checkIfPromoteToKing(PlayerPiece, i_GameBoard);
                PlayerPiece.UpdatePossibleMovesPool(i_GameBoard, ref jumpFound);
                if (jumpFound && isFirstJump)
                {
                    isFirstJump = false;
                    SweepBasicStepsNeccessary = true; 
                }
            }

            if (SweepBasicStepsNeccessary)
            {
                this.sweepBasicStepsFromPieces();
            }
        }

        public bool IsJumpPossible()
        {
            bool IsJumpPossible = false;
            foreach (Piece playerPiece in r_PlayerPieces)
            {
                if (playerPiece.IsJumpAvailable())
                {
                    IsJumpPossible = true;
                    break;
                }
            }

            return IsJumpPossible;
        }

        public bool IsStepAvailable(Step i_StepToCheck, ref bool o_isJump)
        {
            bool isStepFound = false;
            foreach (Piece piece in r_PlayerPieces)
            {
                isStepFound = piece.SearchStepInStepList(i_StepToCheck, ref o_isJump);
                if (isStepFound == true)
                {
                    break;
                }
            }

            return isStepFound;
        }

        private void sweepBasicStepsFromPieces()
        {
            foreach (Piece piece in r_PlayerPieces)
            {
                piece.SweepBasicMoves();
            }
        }

        private void checkIfPromoteToKing(Piece playerPiece, Board i_GameBoard)
        {
            Point isInOpponentBase = playerPiece.Coordinates;
            ushort boardSize = i_GameBoard.GetBoardSize;
            string firstRow = "a";
            string Player1BaseRow = firstRow;
            string Player2BaseRow = Convert.ToChar(Convert.ToChar(firstRow) + (boardSize - 1)).ToString();
            if (playerPiece.PlayerAssociation == ePlayers.Player1)
            {
                if (isInOpponentBase.GetRowCoord == Player2BaseRow)
                {
                    playerPiece.PromoteToKing();
                }
            }
            else
            {
                if (isInOpponentBase.GetRowCoord == Player1BaseRow)
                {
                    playerPiece.PromoteToKing();
                }
            }
        }

        public void RemovePiece(BoardSlot i_BoardSlotThatContainsThePiece)
        {
            r_PlayerPieces.Remove(i_BoardSlotThatContainsThePiece.GetPieceRef());
        }

        public void IsPieceAvailable(Point i_PieceLocation, out ePlayers o_StepOf)
        {
            o_StepOf = ePlayers.UndefinedPlayer;
            foreach (Piece piece in r_PlayerPieces)
            {
                if (i_PieceLocation == piece.Coordinates)
                {
                    o_StepOf = m_PlayerIdentifier;
                }
            }
        }

        public bool IsStepAvailableForPlayer()
        {
            bool noAvailableSteps = true;

            foreach (Piece piece in r_PlayerPieces)
            {
                if (piece.GetAvailableSteps.Count != Constants.EMPTY)
                {
                    noAvailableSteps = false;
                    break;
                }
            }

            return noAvailableSteps;
        }

        public void SweepOtherPiecesJumps(Piece i_AllowedPieceJump)
        {
            foreach (Piece piece in r_PlayerPieces)
            {
                if (piece != i_AllowedPieceJump)
                {
                    piece.SweepJumpMoves();
                }
            }
        }

        public List<Piece> GetMoveablePieces()
        {
            List<Piece> moveablePiecesOnly = new List<Piece>();
            foreach (Piece piece in r_PlayerPieces)
            {
                if (piece.GetAvailableSteps.Count != Constants.EMPTY)
                {
                    moveablePiecesOnly.Add(piece);
                }
            }

            return moveablePiecesOnly;
        }
    }
}
