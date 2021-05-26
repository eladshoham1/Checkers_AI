using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public class AlphaBetaBoard : LogicBoard
    {
        private LogicBoard parent; //הלוח הקודם
        private int depth; //עומק העץ
        private int val; //ערך הלוח
        private Move selectedMove; //המהלך הנבחר

        //הפעולה מחזירה או מעדכנת את הלוח הקודם
        public LogicBoard Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        //הפעולה מחזירה או מעדכנת את עומק הלוח
        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        //הפעולה מחזירה או מעדכנת את ערך הלוח
        public int Val
        {
            get { return val; }
            set { val = value; }
        }

        //הפעולה מחזירה או מעדכנת את המהלך הנבחר
        public Move SelectedMove
        {
            get { return selectedMove; }
            set { selectedMove = value; }
        }

        //הפעולה בונה עץ מינימקס בעומק 3
        public AlphaBetaBoard(LogicBoard b)
            : base()
        {
            this.parent = b;
            this.SetTurn(b.GetTurn());
            this.depth = 3;
            this.CopyParentBoardData(); 
        }

        //הפעולה בונה עץ מינימקס ויורדת בעומק
        public AlphaBetaBoard(AlphaBetaBoard b)
            : base()
        {
            this.parent = b;
            this.SetTurn(b.GetTurn());
            this.depth = b.depth - 1;
            this.CopyParentBoardData();
        }

        //הפעולה מעתיקה את הלוח המקורי ללוח חדש
        public void CopyParentBoardData()
        {
            for (int i = 0; i < this.GetBoard().GetLength(0); i++)
            {
                for (int j = 0; j < this.GetBoard().GetLength(1); j++)
                    this.GetBoard()[i, j] = parent.GetBoard()[i, j];
            }
        }

        //הפעולה מחזירה את כל הלוחות האפשריים מהלוח הנוכחי
        public List<AlphaBetaBoard> Children()
        {
            if (this.depth == 0)
                return null;
            List<AlphaBetaBoard> children = new List<AlphaBetaBoard>();
            for (int i = 0; i < GetBoard().GetLength(0); i++)
            {
                for (int j = 0; j < GetBoard().GetLength(1); j++)
                {
                    if (this.GetSquarePlayer(i, j) == this.GetTurn())
                        children.AddRange(EatMoveBoard(i, j));
                }
            }
            if (children.Count == 0)
            {
                for (int i = 0; i < GetBoard().GetLength(0); i++)
                {
                    for (int j = 0; j < GetBoard().GetLength(1); j++)
                    {
                        if (this.GetSquarePlayer(i, j) == this.GetTurn())
                            children.AddRange(MoveBoard(i, j));
                    }
                }
            }
            return children;
        }

        //הפעולה מחזירה את כל המהלכים האפשריים
        public List<AlphaBetaBoard> MoveBoard(int row, int col)
        {
            List<AlphaBetaBoard> boards = new List<AlphaBetaBoard>();
            AlphaBetaBoard abBoard;
            List<Move> moves = ListOfMoves(row, col);
            while (moves.Count != 0)
            {
                abBoard = new AlphaBetaBoard(this);
                abBoard.SetTurn(this.GetTurn());
                abBoard.GetBoard()[moves.First().GetTarget().GetRow(), moves.First().GetTarget().GetCol()] = 
                                            this.GetBoard()[row, col];
                abBoard.GetBoard()[row, col] = 0;
                abBoard.selectedMove = moves.First();
                moves.Remove(moves.First());
                boards.Add(abBoard);
            }
            return boards;
        }

        //הפעולה מחזירה את כל מהלכי האכילות האפשריים
        public List<AlphaBetaBoard> EatMoveBoard(int row, int col)
        {
            List<AlphaBetaBoard> boards = new List<AlphaBetaBoard>();
            AlphaBetaBoard abBoard;
            List<EatMove> eatMoves = ListOfEatMoves(new Square(row, col));
            while (eatMoves.Count != 0)
            {
                abBoard = new AlphaBetaBoard(this);
                abBoard.SetTurn(this.GetTurn());
                abBoard.GetBoard()[eatMoves.First().GetTarget().GetRow(), eatMoves.First().GetTarget().GetCol()] = this.GetBoard()[row, col];
                abBoard.GetBoard()[eatMoves.First().GetEat().GetRow(), eatMoves.First().GetEat().GetCol()] = 0;
                abBoard.GetBoard()[row, col] = 0;
                abBoard.selectedMove = eatMoves.First();
                eatMoves.Remove(eatMoves.First());
                boards.Add(abBoard);
            }
            return boards;
        }

        //הפעולה מחשבת את ערך הלוח
        public int GetTotalScore()
        {
            if (this.ComputerWin())
                return 100000;
            if (this.PlayerWin())
                return -100000;

            int val = 0, player = this.GetTurn();
            val += EvalPieces(player) * 100;
            val -= EvalPieces(player * -1) * 100;

            int row = this.selectedMove.GetTarget().GetRow();
            int col = this.selectedMove.GetTarget().GetCol();

            if (this.GetTurn() == 1) val += row * 5;
            if (this.GetTurn() == -1) val += (7 - row) * 5;

            val += CountNeighbours(player, row, col) * 2;

            val += FrameEval(player);
            val -= FrameEval(player*-1);

            if (BecomeKing(row))
                val += 100;

            if (InRisk(row, col)) val -= 100;
            val -= BoardInRisk() * 10;

            return val * player;
        }

        //הפעולה מחזירה את הפרש בין הכלים
        public int EvalPieces(int player)
        {
            int count1 = 0, count2 = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (this.GetBoard()[i, j] == player) count1++;
                    else if (this.GetBoard()[i, j] == player * 2) count2++;
                }
            return count2 * 10 + count1;
        }

        //הפעולה בודקת בכמה משבצות מהמשבצות השכנות יש כלי ששייך לאותו שחקן
        private int CountNeighbours(int player, int row, int col)
        {
            int count = 0;
            if (PlayerPiece(player, row - player, col - 1)) count++;
            if (PlayerPiece(player, row - player, col + 1)) count++;
            if (PlayerPiece(player, row + player, col - 1)) count++;
            if (PlayerPiece(player, row + player, col - 1)) count++;
            return count;
        }

        //הפעולה בודקת אם נוצר מצב של משולש בין הכלים
        private bool CheckTriangle(int player, int row1, int col1, int row2, int col2)
        {
            return PlayerPiece(player,row1, col1) &&
                   PlayerPiece(player,row2, col2);                
        }

        //הפעולה בודקת אם הכלי במשבצת הוא שייך לאותו שחקן
        private bool PlayerPiece(int player, int row, int col)
        {
            return CheckLocaion(row, col) &&
            (this.GetBoard()[row, col] == player || 
             this.GetBoard()[row, col] == player * 2);
        }

        //הפעולה בודקת כמה כלים נמצאים במסגרת הלוח ומוסיפה לכל אחד מהם ניקוד
        public int FrameEval(int player)
        {
           int eval = 0;
           int myRow = 0;
           if (player == 1) myRow = 7;
           for (int i = 0; i < 8; i++)
           {
               if (PlayerPiece(player, myRow, i)) eval += 15;
               if (PlayerPiece(player, i, 0)) eval += 4;
               if (PlayerPiece(player, i, 7)) eval += 4;
           }
           return eval;
        }

        //הפעולה בודקת אם האבן יכולה להפוך למלך
        public bool BecomeKing(int row)
        {
            return (this.GetTurn() == 1 && row == 7) ||
                (this.GetTurn() == -1 && row == 0);
        }

        public int BoardInRisk()
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (this.GetBoard()[i, j] == this.GetTurn()
                        && InRisk(i, j))
                        count++;
                    else
                        if (this.GetBoard()[i, j] == this.GetTurn() * 2
                            && InRisk(i, j))
                            count += 10;
                }   
            }
            return count;
        }
        
        public bool InRisk(int row, int col)
        {
            return InRiskA(row, col) || InRiskB(row, col);
        }

        public bool InRiskA(int row, int col)
        {
            return InRiskA(row, col, 1) || InRiskA(row, col, -1);
        }

        public bool InRiskA(int row, int col, int d)
        {
            int player = this.GetTurn();
            return PlayerPiece(player * -1, row + player, col + d) &&
                   PlayerPiece(0, row - player, col - d);
        }

        public bool InRiskB(int row, int col)
        {
            return InRiskB(1, row, col, 1) || InRiskB(1, row, col, -1) ||
                   InRiskB(-1, row, col, 1) || InRiskB(-1, row, col, -1);
        }

        public bool InRiskB(int dr, int row, int col, int dc)
        {
            int player = this.GetTurn();
            bool ok = PlayerPiece(0, row - dr, col - dc);
            row += dr;
            col += dc;
            while (ok && PlayerPiece(0, row, col))
            {
                row += dr;
                col += dc;
            }
            if (!CheckLocaion(row, col) ||
                !(this.GetBoard()[row, col] == player * -2))
                ok = false;
            return ok;
        }
    }
}
