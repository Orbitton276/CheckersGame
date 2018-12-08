using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public class Board
    {
        private ushort m_BoardSize;

        private BoardSlot[,] m_AllBoardSlots;

        public Board(ushort i_BoardSizeFromUI, Player[] i_Players)
        {
            m_BoardSize = i_BoardSizeFromUI;
            m_AllBoardSlots = new BoardSlot[m_BoardSize, m_BoardSize];
            initializeBoardPieces(i_Players);
        }

        private void initializeBoardPieces(Player[] i_Players)
        {
            string pieceSign;
            ePlayers playerIdentifier;
            for (int i = 0; i < m_BoardSize; i++)
            {
                determinePlayerAndSignAttachmentForPotentialPiece(i, out playerIdentifier, out pieceSign);
                for (int j = 0; j < m_BoardSize; j++)
                {
                    m_AllBoardSlots[i, j] = new BoardSlot(new Point(i, j));
                    if (isPlayerRow(i))
                    {
                        if (isPieceBoardSlot(i, j))
                        {
                            m_AllBoardSlots[i, j].AttachPieceToBoardSlotAndPlayer(i_Players, pieceSign, playerIdentifier);
                        }
                    }
                }
            }
        }

        private bool isPlayerRow(int i_BoardRow)
        {
            return i_BoardRow < (m_BoardSize / 2) - 1 || i_BoardRow > m_BoardSize / 2;
        }

        private void determinePlayerAndSignAttachmentForPotentialPiece(int i_BoardRow, out ePlayers o_CurrentPlayerToAttachPiece, out string o_PieceSign)
        {
            if (i_BoardRow < (m_BoardSize / 2) - 1)
            {
                o_PieceSign = "O";
                o_CurrentPlayerToAttachPiece = ePlayers.Player1;
            }
            else if (i_BoardRow > m_BoardSize / 2)
            {
                o_PieceSign = "X";
                o_CurrentPlayerToAttachPiece = ePlayers.Player2;
            }
            else
            {
                o_PieceSign = string.Empty;
                o_CurrentPlayerToAttachPiece = ePlayers.UndefinedPlayer;
            }
        }

        public void DrawCheckersBoard()
        {
            drawUpperLine();
            for (int i = 0; i < m_BoardSize; i++)
            {
                drawBufferLine();
                for (int j = 0; j < m_BoardSize; j++)
                {
                    if (j == 0)
                    {
                        Console.Write(Convert.ToChar(i + Constants.LOWER_CASE_OFFSET));
                    }

                    m_AllBoardSlots[i, j].DrawBoardSlot();
                    if (j == m_BoardSize - 1)
                    {
                        Console.Write("|");
                    }
                }

                Console.Write(Environment.NewLine);
            }

            drawBufferLine();
        }

        private void drawUpperLine()
        {
            for (int i = 0; i < m_BoardSize; i++)
            {
                Console.Write("   " + Convert.ToChar(i + Constants.UPPER_CASE_OFFSET));
            }

            Console.Write(Environment.NewLine);
        }

        private void drawBufferLine()
        {
            Console.Write(" ");
            Console.Write("=");
            for (int i = 0; i < 4 * m_BoardSize; i++)
            {
                Console.Write("=");
            }

            Console.Write(Environment.NewLine);
        }

        private bool isPieceBoardSlot(int i_BoardRow, int i_BoardCol)
        {
            return (i_BoardRow % 2 == 0 && i_BoardCol % 2 == 1) || (i_BoardRow % 2 == 1 && i_BoardCol % 2 == 0);
        }

        public BoardSlot GetBoardSlotFromIdentifierString(string i_SlotIdentifier)
        {
            int boardCol = Convert.ToInt32(i_SlotIdentifier[0] - Constants.UPPER_CASE_OFFSET);
            int boardRow = Convert.ToInt32(i_SlotIdentifier[1] - Constants.LOWER_CASE_OFFSET);
            return m_AllBoardSlots[boardRow, boardCol];
        }

        public ushort GetBoardSize
        {
            get
            {
                return m_BoardSize;
            }
        }

        public void UpdateStepInBoard(Step i_StepToUpdate, ePlayers i_PlayerIdentifierForMovedPiece, Player i_OpponentRefForJumpedOverPiece)
        {
            string destination, source;
            BoardSlot boardSlotToRemovePiece;
            BoardSlot boardSlotToInsertPiece;
            source = i_StepToUpdate.GetSource.ToString();
            destination = i_StepToUpdate.GetDestination.ToString();
            if (i_StepToUpdate.isJump)
            {
                boardSlotToRemovePiece = GetBoardSlotFromIdentifierString(GetJumpedOverPieceBoardSlotIdentifier(i_StepToUpdate));
                i_OpponentRefForJumpedOverPiece.RemovePiece(boardSlotToRemovePiece);
                boardSlotToRemovePiece.RemovePieceFromBoardSlot();
            }

            boardSlotToRemovePiece = GetBoardSlotFromIdentifierString(source);
            boardSlotToInsertPiece = GetBoardSlotFromIdentifierString(destination);
            boardSlotToInsertPiece.InsertPieceInBoardSlot(i_PlayerIdentifierForMovedPiece, boardSlotToRemovePiece.GetPieceRef());
            boardSlotToRemovePiece.RemovePieceFromBoardSlot();
        }

        public string GetJumpedOverPieceBoardSlotIdentifier(Step i_Destination)
        {
            int colDifference, rowDifference;
            colDifference = Convert.ToChar(i_Destination.GetSource.GetColCoord) - Convert.ToChar(i_Destination.GetDestination.GetColCoord);
            rowDifference = Convert.ToChar(i_Destination.GetSource.GetRowCoord) - Convert.ToChar(i_Destination.GetDestination.GetRowCoord);
            string jumpedOverPieceBoardSlotIdentifier = string.Empty;
            int columnIncSingle = Convert.ToChar(i_Destination.GetSource.GetColCoord) + 1;
            int columnDecSingle = Convert.ToChar(i_Destination.GetSource.GetColCoord) - 1;
            int rowIncSingle = Convert.ToChar(i_Destination.GetSource.GetRowCoord) + 1;
            int rowDecSingle = Convert.ToChar(i_Destination.GetSource.GetRowCoord) - 1;
            switch (colDifference)
            {
                case 2:
                    {
                        switch (rowDifference)
                        {
                            case 2:
                                {
                                    jumpedOverPieceBoardSlotIdentifier = Convert.ToChar(columnDecSingle).ToString() + Convert.ToChar(rowDecSingle).ToString();
                                    break;
                                }

                            case -2:
                                {
                                    jumpedOverPieceBoardSlotIdentifier = Convert.ToChar(columnDecSingle).ToString() + Convert.ToChar(rowIncSingle).ToString();
                                    break;
                                }
                        }

                        break;
                    }

                case -2:
                    {
                        switch (rowDifference)
                        {
                            case 2:
                                {
                                    jumpedOverPieceBoardSlotIdentifier = Convert.ToChar(columnIncSingle).ToString() + Convert.ToChar(rowDecSingle).ToString();
                                    break;
                                }

                            case -2:
                                {
                                    jumpedOverPieceBoardSlotIdentifier = Convert.ToChar(columnIncSingle).ToString() + Convert.ToChar(rowIncSingle).ToString();
                                    break;
                                }
                        }

                        break;
                    }
            }

            return jumpedOverPieceBoardSlotIdentifier;
        }

        public string GetBoardSlotIdentifier(ushort i_YCoord, ushort i_XCoord)
        {
            return m_AllBoardSlots[i_YCoord, i_XCoord].GetSlotIdentifier;
        }
    }
}