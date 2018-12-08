using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public class BoardSlot
    {
        private Piece m_Piece = null;
        private Point m_SlotCoordinates;
        private string m_SlotIdentifier;

        public BoardSlot(Point i_SlotCoordinates)
        {
            m_SlotCoordinates = i_SlotCoordinates;
            m_SlotIdentifier = i_SlotCoordinates.GetColCoord + i_SlotCoordinates.GetRowCoord;
        }

        public void AttachPieceToBoardSlotAndPlayer(Player[] i_Players, string i_PieceSign, ePlayers i_PlayerIdentifier)
        {
            m_Piece = new Piece(m_SlotCoordinates, i_PlayerIdentifier);
            i_Players[(int)i_PlayerIdentifier].AddPiece(m_Piece);
        }

        public void DrawBoardSlot()
        {
            string signToDraw;
            Console.Write("|");
            if (m_Piece == null)
            {
                signToDraw = " ";
            }
            else
            {
                signToDraw = m_Piece.GetPieceSignFromPlayerAssociation();
            }

            Console.Write(" " + signToDraw + " ");
        }

        public string GetSlotIdentifier
        {
            get
            {
                return m_SlotIdentifier;
            }
        }

        public bool DoesContainsPiece()
        {
            bool isPieceAlive;
            if (m_Piece == null)
            {
                isPieceAlive = false;
            }
            else
            {
                isPieceAlive = true;
            }

            return isPieceAlive;
        }

        public ePlayers GetSlotPieceAssociation()
        {
            if (m_Piece != null)
            {
                return m_Piece.PlayerAssociation;
            }
            else
            {
                return ePlayers.UndefinedPlayer;
            }
        }

        public Piece GetBoardSlotPiece()
        {
            return m_Piece;
        }

        public void InsertPieceInBoardSlot(ePlayers i_PlayerIdentifier, Piece i_PieceToInsert)
        {
            if (m_Piece == null)
            {
                m_Piece = i_PieceToInsert;
                this.syncPieceCoordinates();
            }
        }

        public void RemovePieceFromBoardSlot()
        {
            if (m_Piece != null)
            {
                m_Piece = null;
            }
        }

        public Piece GetPieceRef()
        {
            return m_Piece;
        }

        private void syncPieceCoordinates()
        {
            m_Piece.Coordinates = m_SlotCoordinates;
        }
    }
}
