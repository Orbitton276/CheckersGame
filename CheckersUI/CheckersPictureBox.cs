using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersProgram
{
    public class CheckersButton : PictureBox
    {
        private string m_PositionInCheckersBoard;
        private ushort m_RowCoord;
        private ushort m_ColCoord;
        
        public CheckersButton(ushort i_RowCoords, ushort i_ColCoords, string i_PositionInCheckersBoard)
        {
            m_PositionInCheckersBoard = i_PositionInCheckersBoard;
            m_RowCoord = i_RowCoords;
            m_ColCoord = i_ColCoords;
        } 

        public string GetPositionIdentifier
        {
            get { return m_PositionInCheckersBoard; }
        }

        public ushort GetRowCoord
        {
            get { return m_RowCoord; }
        }

        public ushort GetColCoord
        {
            get { return m_ColCoord; }
        }
    }
}
