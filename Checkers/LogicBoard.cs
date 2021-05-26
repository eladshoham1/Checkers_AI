using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class LogicBoard
    {
        private int[,] board = new int[8, 8]; //לוח משחק
        private int turn; //תור
        private int black; //מספר הכלים של השחור
        private int white; //מספר הכלים של הלבן

        //הפעולה בונה לוח משחק 8*8 ובו 24 כלים: 12 לבנים ו12 שחורים
        public LogicBoard()
        {
            this.turn = -1;
            this.black = 12;
            this.white = 12;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0)
                        this.board[i, j] = 1;
                    else
                        this.board[i, j] = 0;
                }
            }
            for (int i = 3; i < 5; i++)
                for (int j = 0; j < 8; j++)
                    this.board[i, j] = 0;
            for (int i = 5; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0)
                        this.board[i, j] = -1;
                    else
                        this.board[i, j] = 0;
                }
            }
        }

        //הפעולה מחזירה את הלוח הלוגי
        public int[,] GetBoard()
        {
            return this.board;
        }

        //הפעולה מחזירה את התור
        public int GetTurn()
        {
            return this.turn;
        }
        
        //הפעולה מקבלת שורה ועמודה ומחזירה את הערך שנמצא במשבצת בלוח
        public int GetSquarePlayer(int i, int j)
        {
            if (this.board[i, j] < 0)
                return -1;
            else if (this.board[i, j] > 0)
                return 1;
            return 0;
        }

        //הפעולה מעדכנת את התור
        public void SetTurn(int turn)
        {
            this.turn = turn;
        }

        //הפעולה בודקת אם יש חריגה מגבולות הלוח
        public bool CheckLocaion(int row, int col)
        {
            return row >= 0 && row < this.board.GetLength(0) &&
                    col >= 0 && col < this.board.GetLength(1);
        }

        //הפעולה בודקת אם המשבצת המקור הגיעה למשבצת היעד
        public bool DiffrentPossition(int row, int col, int row1, int col1)
        {
            return row != row1 || col != col1;
        }

        //הפעולה בודקת אם הצעד חוקי
        public bool SimpleStep(int row, int col, int squareInfo)
        {
            return CheckLocaion(row, col) &&
                (this.board[row, col] == squareInfo || this.board[row, col] == squareInfo * 2);
        }

        //הפעולה בודקת אם יש מהלך
        public bool ThereIsAMove(Square origin)
        {
            int piece = this.board[origin.GetRow(), origin.GetCol()];
            if (Math.Abs(piece) == 1)
                return SimpleStep(origin.GetRow() + this.turn, origin.GetCol() + 1, 0) ||
                    SimpleStep(origin.GetRow() + this.turn, origin.GetCol() - 1, 0);
            else
                return SimpleStep(origin.GetRow() + 1, origin.GetCol() + 1, 0) ||
                       SimpleStep(origin.GetRow() + 1, origin.GetCol() - 1, 0) ||
                       SimpleStep(origin.GetRow() - 1, origin.GetCol() + 1, 0) ||
                       SimpleStep(origin.GetRow() - 1, origin.GetCol() - 1, 0);

        }

        //הפעולה בודקת אם יש מהלך אכילה לשחקן שנבחר
        public bool ThereIsAnEatMove(Square origin)
        {
            int piece = this.board[origin.GetRow(), origin.GetCol()];
            int opp = this.turn * -1;
            if (Math.Abs(piece) == 1)
                return (SimpleStep(origin.GetRow() + this.turn, origin.GetCol() + 1, opp) &&
                        SimpleStep(origin.GetRow() + this.turn * 2, origin.GetCol() + 2, 0)) ||
                       (SimpleStep(origin.GetRow() + this.turn, origin.GetCol() - 1, opp) &&
                        SimpleStep(origin.GetRow() + this.turn * 2, origin.GetCol() - 2, 0));
            else
                return ThereIsAKingEatMove(origin.GetRow(), origin.GetCol(), 1, 1) ||
                ThereIsAKingEatMove(origin.GetRow(), origin.GetCol(), 1, -1) ||
                ThereIsAKingEatMove(origin.GetRow(), origin.GetCol(), -1, 1) ||
                ThereIsAKingEatMove(origin.GetRow(), origin.GetCol(), -1, -1);
        }

        //הפעולה מחזירה את המהלך האפשריים של המלך
        public List<Move> NextKingMoves(int row, int col, int dr, int dc)
        {
            Square origin = new Square(row, col);
            List<Move> moves = new List<Move>();
            row += dr;
            col += dc;
            while (CheckLocaion(row, col) && this.board[row, col] == 0)
            {
                moves.Add(new Move(origin, new Square(row, col)));
                row += dr;
                col += dc;
            }
            return moves;
        }

        //הפעולה מחזירה מהלך ממשבצת המקור
        public List<Move> ListOfMoves(int row, int col)
        {
            List<Move> moves = new List<Move>();
            if (SimpleStep(row + this.turn, col + 1, 0))
                moves.Add(new Move(new Square(row, col), new Square(row + this.turn, col + 1)));
            if (SimpleStep(row + this.turn, col - 1, 0))
                moves.Add(new Move(new Square(row, col), new Square(row + this.turn, col - 1)));
            if (Math.Abs(this.GetBoard()[row, col]) == 2)
            {
                moves.AddRange(NextKingMoves(row, col, 1, 1));
                moves.AddRange(NextKingMoves(row, col, 1, -1));
                moves.AddRange(NextKingMoves(row, col, -1, 1));
                moves.AddRange(NextKingMoves(row, col, -1, -1));
            }
            return moves;
        }

        //הפעולה מחזירה מהלך של אכילה ממשבצת המקור
        public List<EatMove> ListOfEatMoves(Square origin)
        {
            Square target, eat;
            int piece = this.board[origin.GetRow(), origin.GetCol()];
            List<EatMove> moves = new List<EatMove>();
            int opp = this.turn * -1;
            if (Math.Abs(piece) == 1)
            {
                eat = new Square(origin.GetRow() + this.turn, origin.GetCol() + 1);
                target = new Square(origin.GetRow() + this.turn * 2, origin.GetCol() + 2);
                if (SimpleStep(origin.GetRow() + this.turn, origin.GetCol() + 1, opp) &&
                        SimpleStep(origin.GetRow() + this.turn * 2, origin.GetCol() + 2, 0))
                    moves.Add(new EatMove(origin, target, eat));
                eat = new Square(origin.GetRow() + this.turn, origin.GetCol() - 1);
                target = new Square(origin.GetRow() + this.turn * 2, origin.GetCol() - 2);
                if (SimpleStep(origin.GetRow() + this.turn, origin.GetCol() - 1, opp) &&
                        SimpleStep(origin.GetRow() + this.turn * 2, origin.GetCol() - 2, 0))
                    moves.Add(new EatMove(origin, target, eat));
            }
            else if (Math.Abs(piece) == 2)
            {
                target = KingEatMove(origin.GetRow(), origin.GetCol(), 1, 1);
                if (target!=null)
                {
                    eat = new Square(target.GetRow() - 1, target.GetCol() - 1);
                    moves.Add(new EatMove(origin, target, eat)); 
                }
                target = KingEatMove(origin.GetRow(), origin.GetCol(), -1, 1);
                if (target != null)
                {
                    eat = new Square(target.GetRow() + 1, target.GetCol() - 1);
                    moves.Add(new EatMove(origin, target, eat));
                }
                target = KingEatMove(origin.GetRow(), origin.GetCol(), 1, -1);
                if (target != null)
                {
                    eat = new Square(target.GetRow() - 1, target.GetCol() + 1);
                    moves.Add(new EatMove(origin, target, eat));
                }
                target = KingEatMove(origin.GetRow(), origin.GetCol(), -1, -1);
                if (target != null)
                {
                    eat = new Square(target.GetRow() + 1, target.GetCol() + 1);
                    moves.Add(new EatMove(origin, target, eat));
                }
            }
            return moves;
        }

        //הפעולה בודקת אם יש שחקן כלשהו שיש לו מהלך אכילה אפשרי לביצוע
        public bool ThereIsAnyEatMove()
        {
            bool found = false;
            int i = 0, j;
            while (!found && i < this.board.GetLength(0))
            {
                j = 0;
                while (!found && j < this.board.GetLength(1))
                {
                    if (this.GetBoard()[i, j] == this.turn || this.GetBoard()[i, j] == this.turn * 2)
                        found = ThereIsAnEatMove(new Square(i, j));
                    j++;
                }
                i++;
            }
            return found;
        }

        //הפעולה בודקת אם יש מהלך אכילה של מלך
        public bool ThereIsAKingEatMove(int rowOrigin, int colOrigin, int dr, int dc)
        {
            rowOrigin += dr;
            colOrigin += dc;
            while (CheckLocaion(rowOrigin, colOrigin) && this.board[rowOrigin, colOrigin] == 0)
            {
                rowOrigin += dr;
                colOrigin += dc;
            }
            if (!CheckLocaion(rowOrigin, colOrigin))
                return false;
            if (this.board[rowOrigin, colOrigin] == this.turn ||
                this.board[rowOrigin, colOrigin] == this.turn * 2)
                return false;
            rowOrigin += dr;
            colOrigin += dc;
            return SimpleStep(rowOrigin, colOrigin, 0);
        }

        //הפעולה מחזירה משבצת של מלך שיש לו אפשרות לאכול
        public Square KingEat(Square origin)
        {
            Square target = null;
            target = KingEatMove(origin.GetRow(), origin.GetCol(), 1, 1);
            if (target != null) return target;
            target = KingEatMove(origin.GetRow(), origin.GetCol(), 1, -1);
            if (target != null) return target;
            target = KingEatMove(origin.GetRow(), origin.GetCol(), -1, 1);
            if (target != null) return target;
            target = KingEatMove(origin.GetRow(), origin.GetCol(), -1, -1);
            return target;
        }

        //הפעולה מחזירה משבצת שאליה מלך יכול להתקדם כדי לאכול את השחקן היריב
        public Square KingEatMove(int rowOrigin, int colOrigin, int dr, int dc)
        {
            rowOrigin += dr;
            colOrigin += dc;
            while (CheckLocaion(rowOrigin, colOrigin) && this.board[rowOrigin, colOrigin] == 0)
            {
                rowOrigin += dr;
                colOrigin += dc;
            }
            if (!CheckLocaion(rowOrigin, colOrigin))
                return null;
            if (this.board[rowOrigin, colOrigin] == this.turn ||
                this.board[rowOrigin, colOrigin] == this.turn * 2)
                return null;
            rowOrigin += dr;
            colOrigin += dc;
            if (SimpleStep(rowOrigin, colOrigin, 0))
                return new Square(rowOrigin, colOrigin);
            return null;
        }

        //הפעולה בודקת אם השחקן שכליו לבנים ניצח
        public bool PlayerWin()
        {
            if (black == 0)
                return true;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] > 0 && (ThereIsAMove(new Square(i, j)) || ThereIsAnEatMove(new Square(i, j))))
                        return false;
                }
            }
            if (this.turn == 1)
                return true;
            return false;
        }

        //הפעולה בודקת אם המשחק נגמר בתיקו
        public bool CheckDraw()
        {
            if (white == 1 && black == 1)
                return true;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (ThereIsAMove(new Square(i, j)) || ThereIsAnEatMove(new Square(i, j)))
                        return false;
                }
            }
            return true;
        }

        //הפעולה בודקת אם השחקן שכליו שחורים ניצח
        public bool ComputerWin()
        {
            if (white == 0)
                return true;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] < 0 && (ThereIsAMove(new Square(i, j)) || ThereIsAnEatMove(new Square(i, j))))
                        return false;
                }
            }
            if (this.turn == -1)
                return true;
            return false;
        }

        //הפעולה בודקת אם המשחק נגמר
        public bool CheckEnd()
        {
            return PlayerWin() || CheckDraw() || ComputerWin();
        }
    }
}
