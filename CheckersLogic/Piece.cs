using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public class Piece
    {
        private Point m_Coordinates;

        private bool m_isKing = false;

        private List<Step> m_AvailableSteps = new List<Step>();

        public List<Step> GetAvailableSteps
        {
            get
            {
                return m_AvailableSteps;
            }
        }

        private ePlayers m_PlayerAssociation;

        private Step.eLookOrientation m_Orientation;

        private int m_Weight = Constants.MEN_PIECE;

        public Piece(Point i_PieceCoordinates, ePlayers i_PlayerAssociation)
        {
            m_Coordinates = i_PieceCoordinates;
            m_PlayerAssociation = i_PlayerAssociation;
            m_Orientation = (Step.eLookOrientation)i_PlayerAssociation;
        }

        public int GetWeight
        {
            get
            {
                return m_Weight;
            }
        }

        public ePlayers PlayerAssociation
        {
            get
            {
                return m_PlayerAssociation;
            }
        }

        public void SetPieceToKing()
        {
            m_isKing = true;
            m_Weight = Constants.KING_PIECE;
            m_Orientation = Step.eLookOrientation.MULTI_DIR;
        }

        public bool GetIsKingBoolean
        {
            get { return m_isKing; }
        }

        public string GetPieceSignFromPlayerAssociation()
        {
            string pieceSign;
            if (m_PlayerAssociation == ePlayers.Player1)
            {
                if (m_isKing == true)
                {
                    pieceSign = "U";
                }
                else
                {
                    pieceSign = "O";
                }
            }
            else
            {
                if (m_isKing == true)
                {
                    pieceSign = "K";
                }
                else
                {
                    pieceSign = "X";
                }
            }

            return pieceSign;
        }

        public Point Coordinates
        {
            get
            {
                return m_Coordinates;
            }

            set
            {
                m_Coordinates = value;
            }
        }

        public string convertCoordinatesToBoardSlotIdentifier()
        {
            return m_Coordinates.GetColCoord + m_Coordinates.GetRowCoord;
        }

        public void UpdatePossibleMovesPool(Board i_GameBoard, ref bool o_isJumpFound)
        {
            Point Destination;
            eDirection currentDirection;
            List<Point> possibleDestinations = getPossibleDestinations();
            List<Step> updatedSteps = new List<Step>();
            for (int i = possibleDestinations.Count - 1; i >= 0; i--)
            {
                Destination = possibleDestinations[i];
                if (!destinationInBorders(Destination, i_GameBoard))
                {
                    possibleDestinations.Remove(Destination);
                }
                else
                {
                    if (!destinationHasPiece(Destination, i_GameBoard))
                    {
                        updatedSteps.Add(new Step(m_Coordinates, Destination, Constants.NOT_JUMP));
                    }
                    else
                    {
                        if (!isOpponentPiece(Destination, i_GameBoard))
                        {
                            possibleDestinations.Remove(Destination);
                        }
                        else
                        {
                            currentDirection = getDirectionBySourceAndDest(m_Coordinates, Destination);
                            if (checkIfJumpIsPossible(currentDirection, Destination, i_GameBoard))
                            {
                                o_isJumpFound = true;
                                updatedSteps.Add(new Step(m_Coordinates, Destination, Constants.JUMP));
                            }
                            else
                            {
                                possibleDestinations.Remove(Destination);
                            }
                        }
                    }
                }
            }

            m_AvailableSteps = updatedSteps;
        }

        private bool checkIfJumpIsPossible(eDirection i_JumpDirection, Point i_OpponentPieceLocation, Board i_GameBoard)
        {
            Point locationAfterJumpOver = i_OpponentPieceLocation;
            locationAfterJumpOver.ShiftPoint(i_JumpDirection);
            bool jumpIsPossible;
            string opponentBoardSlot;
            BoardSlot destinationBoardSlot;
            if (!destinationInBorders(locationAfterJumpOver, i_GameBoard))
            {
                jumpIsPossible = false;
            }
            else
            {
                opponentBoardSlot = i_OpponentPieceLocation.GetColCoord + i_OpponentPieceLocation.GetRowCoord;
                destinationBoardSlot = i_GameBoard.GetBoardSlotFromIdentifierString(opponentBoardSlot);
                if (destinationBoardSlot.DoesContainsPiece())
                {
                    jumpIsPossible = false;
                }
                else
                {
                    jumpIsPossible = true;
                }
            }

            return jumpIsPossible;
        }

        private List<Point> getPossibleDestinations()
        {
            char upperCaseOrigin = Convert.ToChar(m_Coordinates.GetColCoord);
            char lowerCaseOrigin = Convert.ToChar(m_Coordinates.GetRowCoord);

            char upperCaseInc = Convert.ToChar(upperCaseOrigin + 1);
            char upperCaseDec = Convert.ToChar(upperCaseOrigin - 1);
            char lowerCaseInc = Convert.ToChar(lowerCaseOrigin + 1);
            char lowerCaseDec = Convert.ToChar(lowerCaseOrigin - 1);

            List<Point> possibleDestinations = new List<Point>();

            switch (m_Orientation)
            {
                case Step.eLookOrientation.DOWN:
                    {
                        possibleDestinations.Add(new Point(upperCaseDec, lowerCaseInc));
                        possibleDestinations.Add(new Point(upperCaseInc, lowerCaseInc));
                        break;
                    }

                case Step.eLookOrientation.UP:
                    {
                        possibleDestinations.Add(new Point(upperCaseDec, lowerCaseDec));
                        possibleDestinations.Add(new Point(upperCaseInc, lowerCaseDec));
                        break;
                    }

                case Step.eLookOrientation.MULTI_DIR:
                    {
                        possibleDestinations.Add(new Point(upperCaseDec, lowerCaseInc));
                        possibleDestinations.Add(new Point(upperCaseInc, lowerCaseInc));
                        possibleDestinations.Add(new Point(upperCaseDec, lowerCaseDec));
                        possibleDestinations.Add(new Point(upperCaseInc, lowerCaseDec));
                        break;
                    }
            }

            return possibleDestinations;
        }

        private bool destinationInBorders(Point i_Destination, Board i_GameBoard)
        {
            string destString = i_Destination.ToString();
            string[] DestPatternLettersDelimiters = new string[2];
            ushort boardSize = i_GameBoard.GetBoardSize;
            DestPatternLettersDelimiters[0] = Convert.ToChar(boardSize + Constants.UPPER_CASE_OFFSET - 1).ToString();
            DestPatternLettersDelimiters[1] = Convert.ToChar(boardSize + Constants.LOWER_CASE_OFFSET - 1).ToString();
            string regExPattern = @"^[A-{0}][a-{1}]$";
            regExPattern = string.Format(regExPattern, DestPatternLettersDelimiters);
            Match isDestOutOfBorders;
            isDestOutOfBorders = Regex.Match(destString, regExPattern);
            return isDestOutOfBorders.Success;
        }

        private bool destinationHasPiece(Point i_Destination, Board i_GameBoard)
        {
            BoardSlot destSlot = i_GameBoard.GetBoardSlotFromIdentifierString(i_Destination.ToString());
            return destSlot.DoesContainsPiece();
        }

        private bool isOpponentPiece(Point i_Destination, Board i_GameBoard)
        {
            return i_GameBoard.GetBoardSlotFromIdentifierString(i_Destination.ToString()).GetSlotPieceAssociation() != m_PlayerAssociation;
        }

        private eDirection getDirectionBySourceAndDest(Point i_Source, Point i_Dest)
        {
            int colDifference, rowDifference;
            colDifference = Convert.ToChar(i_Source.GetColCoord) - Convert.ToChar(i_Dest.GetColCoord);
            rowDifference = Convert.ToChar(i_Source.GetRowCoord) - Convert.ToChar(i_Dest.GetRowCoord);
            eDirection calculatedDirection = eDirection.Undefined;
            switch (colDifference)
            {
                case 1:
                    {
                        switch (rowDifference)
                        {
                            case 1:
                                {
                                    calculatedDirection = eDirection.UpperLeft;
                                    break;
                                }

                            case -1:
                                {
                                    calculatedDirection = eDirection.LowerLeft;
                                    break;
                                }
                        }

                        break;
                    }

                case -1:
                    {
                        switch (rowDifference)
                        {
                            case 1:
                                {
                                    calculatedDirection = eDirection.UpperRight;
                                    break;
                                }

                            case -1:
                                {
                                    calculatedDirection = eDirection.LowerRight;
                                    break;
                                }
                        }

                        break;
                    }
            }

            return calculatedDirection;
        }

        public bool SearchStepInStepList(Step i_StepToSearch, ref bool io_IsJump)
        {
            bool isFound = false;
            foreach (Step stepInList in m_AvailableSteps)
            {
                if (i_StepToSearch == stepInList)
                {
                    io_IsJump = stepInList.isJump;
                    isFound = true;
                    break;
                }
            }

            return isFound;
        }

        public void SweepBasicMoves()
        {
            if (!(m_AvailableSteps.Count == Constants.EMPTY))
            {
                for (int i = m_AvailableSteps.Count - 1; i >= 0; i--)
                {
                    if (!m_AvailableSteps[i].isJump)
                    {
                        m_AvailableSteps.Remove(m_AvailableSteps[i]);
                    }
                }
            }
        }

        public bool IsJumpAvailable()
        {
            bool jumpFound = false;
            foreach (Step playerStep in m_AvailableSteps)
            {
                if (playerStep.isJump)
                {
                    jumpFound = true;
                    break;
                }
            }

            return jumpFound;
        }

        public void PromoteToKing()
        {
            m_isKing = true;
            m_Weight = Constants.KING_PIECE;
            m_Orientation = Step.eLookOrientation.MULTI_DIR;
        }

        public void SweepJumpMoves()
        {
            if (m_AvailableSteps != null)
            {
                foreach (Step step in m_AvailableSteps)
                {
                    if (step != null)
                    {
                        m_AvailableSteps.Remove(step);
                    }
                }
            }
        }
    }
}
