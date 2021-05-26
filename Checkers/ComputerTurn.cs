using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public static class ComputerTurn
    {
        //הפעולה מחזירה את המהלך של המחשב
        public static Move ComputerPlay(GraphicBoard gb)
        {
            LogicBoard board = gb.GetLogicBoard();
            AlphaBetaBoard b = new AlphaBetaBoard(board);
            Move move = AlphaBeta.BestMove(b);
            return move;
        }
    }
}
