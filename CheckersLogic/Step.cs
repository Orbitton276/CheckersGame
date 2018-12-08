using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public class Step
    {
        private Point m_Source;

        private Point m_Destination;

        private bool m_IsJump = false;

        public static bool operator ==(Step StepX1, Step StepX2)
        {
            if (object.ReferenceEquals(StepX1, null))
            {
                return object.ReferenceEquals(StepX2, null);
            }
            else if (object.ReferenceEquals(StepX2, null))
            {
                return object.ReferenceEquals(StepX1, null);
            }
            else
            {
                return StepX1.m_Source.ToString() == StepX2.m_Source.ToString() && StepX1.m_Destination.ToString() == StepX2.m_Destination.ToString();
            }
        }

        public static bool operator !=(Step StepX1, Step StepX2)
        {
            if (object.ReferenceEquals(StepX1, null))
            {
                return !object.ReferenceEquals(StepX2, null);
            }
            else if (object.ReferenceEquals(StepX2, null))
            {
                return !object.ReferenceEquals(StepX1, null);
            }
            else
            {
                return StepX1.m_Source != StepX2.m_Source || StepX1.m_Destination != StepX2.m_Destination;
            }
        }

        public enum eLookOrientation
        {
            DOWN, UP, MULTI_DIR
        }

        public bool isJump
        {
            get { return m_IsJump; }
        }

        public Step(string i_MoveToExecute)
        {
            string[] sourceAndDest = i_MoveToExecute.Split('>');
            m_Source = new Point(sourceAndDest[0]);
            m_Destination = new Point(sourceAndDest[1]);
            if (Math.Abs(Point.GetPointsColumnsDifference(m_Source, m_Destination)) == Constants.JUMP_OFFSET)
            {
                m_IsJump = true;
            }
        }

        public Step(Point i_Source, Point i_Destination, bool i_IsJump)
        {
            m_Source = i_Source;
            m_Destination = i_Destination;
            m_IsJump = i_IsJump;
        }

        public override string ToString()
        {
            return m_Source.ToString() + ">" + m_Destination.ToString();
        }

        public Point GetSource
        {
            get { return m_Source; }
        }

        public Point GetDestination
        {
            get { return m_Destination; }
        }
    }
}
