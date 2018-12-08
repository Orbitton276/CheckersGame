using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersProgram
{
    public enum ePlayingMode
    {
        PlayerVsPlayer = 1, PlayerVsComputer = 2
    }

    public enum eBoardSize
    {
        _6x6 = 6, _8x8 = 8, _10x10 = 10
    }

    public enum ePlayers
    {
        Player1, Player2, UndefinedPlayer
    }

    public enum eDirection
    {
        UpperLeft, JumpUpperLeft, UpperRight, JumpUpperRight, LowerLeft, JumpLowerLeft, LowerRight, JumpLowerRight, Undefined
    }

    public class Constants
    {
        public const int UPPER_CASE_OFFSET = 65;

        public const int LOWER_CASE_OFFSET = 97;

        public const bool HUMAN_PLAYER = false;

        public const int MOVE_SOURCE = 0;

        public const int MOVE_DEST = 1;

        public const int MATCH = 0;

        public const int MEN_PIECE = 1;

        public const int KING_PIECE = 4;

        public const bool JUMP = true;

        public const bool NOT_JUMP = false;

        public const bool UNKNOWN = false;

        public const int JUMP_OFFSET = 2;

        public const int YES_OR_NO = 2;

        public const int NUM_OF_PLAYERS = 2;

        public const int EMPTY = 0;

        public const bool INITIALIZE = true;

        public const int TURN_INTERVAL = 1500;
    }
}
