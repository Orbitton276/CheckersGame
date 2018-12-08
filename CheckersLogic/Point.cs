using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public class Point
    {
        private string m_BoardRow;

        private string m_BoardCol;

        public static int GetPointsColumnsDifference(Point i_Source, Point i_Destination)
        {
            return Convert.ToInt32(Convert.ToChar(i_Destination.GetColCoord)) - Convert.ToInt32(Convert.ToChar(i_Source.GetColCoord));
        }

        public static bool operator ==(Point PointX1, Point PointX2)
        {
            if (object.ReferenceEquals(PointX1, null))
            {
                return object.ReferenceEquals(PointX2, null);
            }
            else if (object.ReferenceEquals(PointX2, null))
            {
                return object.ReferenceEquals(PointX1, null);
            }
            else
            {
                return PointX1.GetColCoord == PointX2.GetColCoord && PointX1.GetRowCoord == PointX2.GetRowCoord;
            }
        }

        public static bool operator !=(Point PointX1, Point PointX2)
        {
            if (object.ReferenceEquals(PointX1, null))
            {
                return !object.ReferenceEquals(PointX2, null);
            }
            else if (object.ReferenceEquals(PointX2, null))
            {
                return !object.ReferenceEquals(PointX1, null);
            }
            else
            {
                return PointX1.GetColCoord != PointX2.GetColCoord || PointX1.GetRowCoord != PointX2.GetRowCoord;
            }
        }

        public Point(int i_RowCoordinate, int i_ColCoordinate)
        {
            m_BoardRow = Convert.ToChar(i_RowCoordinate + Constants.LOWER_CASE_OFFSET).ToString();
            m_BoardCol = Convert.ToChar(i_ColCoordinate + Constants.UPPER_CASE_OFFSET).ToString();
        }

        public Point(char i_ColCoordinate, char i_RowCoordinate)
        {
            m_BoardRow = i_RowCoordinate.ToString();
            m_BoardCol = i_ColCoordinate.ToString();
        }

        public Point(string i_StringToConvert)
        {
            m_BoardCol = i_StringToConvert[0].ToString();
            m_BoardRow = i_StringToConvert[1].ToString();
        }

        public string GetRowCoord
        {
            get
            {
                return m_BoardRow;
            }
        }

        public string GetColCoord
        {
            get
            {
                return m_BoardCol;
            }
        }

        public override string ToString()
        {
            return m_BoardCol + m_BoardRow;
        }

        public void ShiftPoint(eDirection i_Direction)
        {
            char boardCol = Convert.ToChar(m_BoardCol);
            char boardRow = Convert.ToChar(m_BoardRow);
            char shiftColRightSingle = Convert.ToChar(boardCol + 1);
            char shiftColRightDouble = Convert.ToChar(boardCol + 2);
            char shiftColLeftSingle = Convert.ToChar(boardCol - 1);
            char shiftColLeftDouble = Convert.ToChar(boardCol - 2);
            char shiftRowDownSingle = Convert.ToChar(boardRow + 1);
            char shiftRowDownDouble = Convert.ToChar(boardRow + 2);
            char shiftRowUpSingle = Convert.ToChar(boardRow - 1);
            char shiftRowUpDouble = Convert.ToChar(boardRow - 2);

            switch (i_Direction)
            {
                case eDirection.LowerLeft:
                    {
                        m_BoardCol = shiftColLeftSingle.ToString();
                        m_BoardRow = shiftRowDownSingle.ToString();
                        break;
                    }

                case eDirection.LowerRight:
                    {
                        m_BoardCol = shiftColRightSingle.ToString();
                        m_BoardRow = shiftRowDownSingle.ToString();
                        break;
                    }

                case eDirection.JumpLowerLeft:
                    {
                        m_BoardCol = shiftColLeftDouble.ToString();
                        m_BoardRow = shiftRowDownDouble.ToString();
                        break;
                    }

                case eDirection.JumpLowerRight:
                    {
                        m_BoardCol = shiftColRightDouble.ToString();
                        m_BoardRow = shiftRowDownDouble.ToString();
                        break;
                    }

                case eDirection.UpperLeft:
                    {
                        m_BoardCol = shiftColLeftSingle.ToString();
                        m_BoardRow = shiftRowUpSingle.ToString();
                        break;
                    }

                case eDirection.UpperRight:
                    {
                        m_BoardCol = shiftColRightSingle.ToString();
                        m_BoardRow = shiftRowUpSingle.ToString();
                        break;
                    }

                case eDirection.JumpUpperLeft:
                    {
                        m_BoardCol = shiftColLeftDouble.ToString();
                        m_BoardRow = shiftRowUpDouble.ToString();
                        break;
                    }

                case eDirection.JumpUpperRight:
                    {
                        m_BoardCol = shiftColRightDouble.ToString();
                        m_BoardRow = shiftRowUpDouble.ToString();
                        break;
                    }
            }
        }
    }
}