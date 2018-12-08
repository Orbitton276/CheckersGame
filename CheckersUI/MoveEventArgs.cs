using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    internal class MoveEventArgs : EventArgs
    {
        private string m_Destination;
        private string m_Source;
        private ushort m_SourceRowIndex;
        private ushort m_SourceColIndex;
        private ushort m_DestRowIndex;
        private ushort m_DestColIndex;

        public MoveEventArgs(CheckersButton i_Destination, CheckersButton i_Source)
        {
            m_Source = i_Source.GetPositionIdentifier;
            m_Destination = i_Destination.GetPositionIdentifier;
            m_SourceRowIndex = i_Source.GetRowCoord;
            m_SourceColIndex = i_Source.GetColCoord;
            m_DestRowIndex = i_Destination.GetRowCoord;
            m_DestColIndex = i_Destination.GetColCoord;
        }

        public string GetDestination
        {
            get { return m_Destination; }
        }

        public string GetSource
        {
            get { return m_Source; }
        }

        public ushort GetSourceColIndex
        {
            get { return m_SourceColIndex; }
        }

        public ushort GetSourceRowIndex
        {
            get { return m_SourceRowIndex; }
        }

        public ushort GetDestColIndex
        {
            get { return m_DestColIndex; }
        }

        public ushort GetDestRowIndex
        {
            get { return m_DestRowIndex; }
        }
    }
}
