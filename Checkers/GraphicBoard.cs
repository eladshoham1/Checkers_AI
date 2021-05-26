using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Checkers
{
    public partial class GraphicBoard : Form
    {
        private LogicBoard logicBoard; //לוח לוגי
        private PictureBoxItem[,] board; //לוח גרפי
        private List<Move> moves1; //מהלכים אפשריים
        private List<EatMove> moves2; //מהלכי אכילה אפשריים

        public GraphicBoard()
        {
            InitializeComponent();
        }

        private void GraphicBoard_Load(object sender, EventArgs e)
        {
            this.logicBoard = new LogicBoard();
            BoardPictures();
        }

        //הפעולה מחזירה את הלוח הלוגי
        public LogicBoard GetLogicBoard()
        {
            return this.logicBoard;
        }

        //הפעולה מדפיסה את לוח המשחק
        private void BoardPictures()
        {
            Square sq;
            board = new PictureBoxItem[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sq = new Square(i, j);
                    board[i, j] = new PictureBoxItem(sq, this.logicBoard.GetBoard()[i, j]);
                    this.Controls.Add(board[i, j]);
                    board[i, j].Click += new System.EventHandler(this.SquareClick);
                }
            }
            this.Refresh();
        }

        //הפעולה מבצעת לחיצה על אבן
        private void SquareClick(object sender, EventArgs e)
        {
            CleanBoard();
            if (this.logicBoard.GetTurn() == -1 && !CheckEnd())
            {
                PictureBoxItem piece = (PictureBoxItem)sender;
                if (PictureBoxItem.originSquare == null)
                    HandleFirstClick(piece);
                else
                {
                    int x = this.logicBoard.GetBoard()[piece.Square.GetRow(), piece.Square.GetCol()];
                    if (x == 0)
                        HandleSecondClick(piece);
                    else
                        if (x < 0)
                        {
                            PictureBoxItem.originSquare = null;
                            HandleFirstClick(piece);
                        }
                }
            }
        }

        //הפעולה בודקת אם הלחיצה הראשונה אפשרית
        private void HandleFirstClick(PictureBoxItem piece)
        {
            if (this.logicBoard.GetBoard()[piece.Square.GetRow(), piece.Square.GetCol()] * this.logicBoard.GetTurn() > 0)
            {
                Square pieceSq = new Square(piece.Square.GetRow(), piece.Square.GetCol());
                bool ok = false;
                if (this.logicBoard.ThereIsAnEatMove(pieceSq))
                    ok = true;
                if (!ok)
                {
                    if (!this.logicBoard.ThereIsAnyEatMove() && this.logicBoard.ThereIsAMove(pieceSq))
                        ok = true;
                }
                if (ok)
                {
                    PictureBoxItem.originSquare = pieceSq;
                    this.NextMoves(piece);
                }
            }
        }

        //הפעולה צובעת את המשבצות שיש לאותו כלי שנבחר לאן ללכת
        private void NextMoves(PictureBoxItem piece)
        {
            board[piece.Square.GetRow(), piece.Square.GetCol()].BackColor = System.Drawing.Color.Peru;
            List<EatMove> eatMoves = new List<EatMove>();
            eatMoves.AddRange(logicBoard.ListOfEatMoves(piece.Square));

            int count = eatMoves.Count;
            if (count != 0)
            {
                this.moves2 = eatMoves;
                foreach (EatMove move in this.moves2)
                    board[move.GetTarget().GetRow(), move.GetTarget().GetCol()].BackColor = System.Drawing.Color.Chocolate;
            }
            if (count == 0)
            {
                List<Move> moves = new List<Move>();
                moves.AddRange(logicBoard.ListOfMoves(piece.Square.GetRow(), piece.Square.GetCol()));
                this.moves1 = moves;
                foreach (Move move in this.moves1)
                    board[move.GetTarget().GetRow(), move.GetTarget().GetCol()].BackColor = System.Drawing.Color.Chocolate;
            }
        }

        //הפעולה בודקת אם הלחיצה השנייה ומבצעת מהלך אם הוא חוקי
        private void HandleSecondClick(PictureBoxItem piece)
        {
           
            bool ok1 = false, ok2 = false;
            Square origin = PictureBoxItem.originSquare;
            Square target = piece.Square;
            if (this.moves2 != null)
            {
                foreach (EatMove move in this.moves2)
                {
                    if (move.GetTarget().GetRow() == piece.Square.GetRow() && move.GetTarget().GetCol() == piece.Square.GetCol())
                    {
                        Square eat = move.GetEat();
                        PieceEatMove(eat, origin, target, piece);
                        ok1 = true;
                        if (!this.CheckEnd())
                            CheckChangeTurn(target);
                    }
                }
            }
            if (!ok1 && this.moves1 != null)
            {
                foreach (Move move in this.moves1)
                {
                    if (move.GetTarget().GetRow() == piece.Square.GetRow() && move.GetTarget().GetCol() == piece.Square.GetCol())
                        ok2 = true;
                }
            }
            if (ok2)
            {
                PieceMove(origin, target, piece);
                if (!this.CheckEnd())
                    ChangeTurn();
            }
            else
                HandleFirstClick(piece);
            CleanBoard();
            this.moves1 = null;
            this.moves2 = null;
        }

        //הפעולה מעבירה את התור לשחקן השני
        private void ChangeTurn()
        {
            CleanBoard();
            this.CheckEnd();
            this.logicBoard.SetTurn(this.logicBoard.GetTurn() * -1);
            if (this.logicBoard.GetTurn() == 1)
                this.DoComputerMove();
        }

        //הפעולה בודקת אם השחקן סיים את התור השלו
        private void CheckChangeTurn(Square target)
        {
            if (this.logicBoard.ThereIsAnEatMove(target))
            {
                if (this.logicBoard.GetTurn() == -1)
                    PictureBoxItem.originSquare = target;
                if (this.logicBoard.GetTurn() == 1) 
                    this.DoComputerMove();
            }
            else
                ChangeTurn();
        }

        //הפעולה מבצעת מהלך של מחשב
        private void DoComputerMove()
        {
            if (!this.CheckEnd())
            {
                Move move = ComputerTurn.ComputerPlay(this);
                PictureBoxItem.originSquare = board[move.GetOrigin().GetRow(), move.GetOrigin().GetCol()].Square;
                PictureBoxItem piece = this.board[move.GetTarget().GetRow(), move.GetTarget().GetCol()];
                if ((Math.Abs(move.GetOrigin().GetRow() - move.GetTarget().GetRow()) > 1 && this.logicBoard.GetBoard()[move.GetOrigin().GetRow(), move.GetOrigin().GetCol()] == 1) 
                    || (IsKingEat(move.GetOrigin(), move.GetTarget()) != null && this.logicBoard.GetBoard()[move.GetOrigin().GetRow(), move.GetOrigin().GetCol()] == 2))
                {
                    EatMove move1 = (EatMove)move;
                    PieceEatMove(move1.GetEat(), move1.GetOrigin(), move1.GetTarget(), piece);
                    Task.Delay(500).Wait();
                    if (!this.CheckEnd())
                        CheckChangeTurn(move.GetTarget());
                }
                else
                {
                    PieceMove(move.GetOrigin(), move.GetTarget(), piece);
                    if (!this.CheckEnd())
                        ChangeTurn();
                }
            }
        }
        
        //הפעולה בודקת אם יש מהלך אכילה של מלך
        private Square IsKingEat(Square origin, Square target)
        {
            int dr = Math.Abs(target.GetRow() - origin.GetRow()) / (target.GetRow() - origin.GetRow());
            int dc = Math.Abs(target.GetCol() - origin.GetCol()) / (target.GetCol() - origin.GetCol());
            int row1 = origin.GetRow() + dr, row2 = target.GetRow();
            int col1 = origin.GetCol() + dc, col2 = target.GetCol();
            while (row1 != row2)
            {
                if (this.logicBoard.GetBoard()[row1, col1] != 0)
                    return new Square(row1, col1);
                row1 += dr;
                col1 += dc;
            }
            return null;
        }

        //הפעולה מבצעת מהלך
        private void PieceMove(Square origin, Square target, PictureBoxItem piece)
        {
            PictureBoxItem.originSquare = null;
            this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()] = this.logicBoard.GetBoard()[origin.GetRow(), origin.GetCol()];
            this.logicBoard.GetBoard()[origin.GetRow(), origin.GetCol()] = 0;
            piece.PutImage(this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()]);
            this.board[origin.GetRow(), origin.GetCol()].RemoveImage();
            this.King(piece);
        }

        //הפעולה מבצעת מהלך אכילה
        private void PieceEatMove(Square eat, Square origin, Square target, PictureBoxItem piece)
        {
            PictureBoxItem.originSquare = null;
            this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()] = this.logicBoard.GetBoard()[origin.GetRow(), origin.GetCol()];
            this.logicBoard.GetBoard()[origin.GetRow(), origin.GetCol()] = 0;
            this.logicBoard.GetBoard()[eat.GetRow(), eat.GetCol()] = 0;
            piece.PutImage(this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()]);
            this.board[origin.GetRow(), origin.GetCol()].RemoveImage();
            this.board[eat.GetRow(), eat.GetCol()].RemoveImage();
            this.King(piece);
        }

        //הפעולה הופכת אבן למלך כאשר הוא מגיע לשורה האחרונה בלוח
        private void King(PictureBoxItem piece)
        {
            Square target = piece.Square;
            if (CanBecomeKing(target))
            {
                this.board[target.GetRow(), target.GetCol()].RemoveImage();
                this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()] *= 2;
                piece.PutImage(this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()]);
            }
        }

        //הפעולה בודקת אם הכלי יכול להפוך למלך
        private bool CanBecomeKing(Square target)
        {
            return (Math.Abs(this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()]) != 2 &&
                (this.logicBoard.GetTurn() == -1 && target.GetRow() == 0 ||
                this.logicBoard.GetTurn() == 1 && target.GetRow() == 7));
        }

        //הפעולה מנקה את הלוח מכל הצעדים
        private void CleanBoard()
        {
            foreach (PictureBoxItem sq in board)
            {
                if ((sq.Square.GetRow() + sq.Square.GetCol()) % 2 != 0)
                    sq.BackColor = System.Drawing.Color.Sienna;
                else
                    sq.BackColor = System.Drawing.Color.Ivory;
            }
        }

        //הפעולה בודקת אם נגמר המשחק ומי ניצח או האם המשחק נגמר בתיקו
        private bool CheckEnd()
        {
            if (this.logicBoard.PlayerWin())
            {
                MessageBox.Show("Player Win");
                return true;
            }
            if (this.logicBoard.ComputerWin())
            {
                MessageBox.Show("Computer Win");
                return true;
            }
            if (this.logicBoard.CheckDraw())
            {
                MessageBox.Show("Draw");
                return true;
            }
            return false;
        }

        //הפעולה מאתחלת את המשחק
        private void resetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to start a new game?", "New Game", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.logicBoard = new LogicBoard();
                int board;
                foreach (PictureBoxItem piece in this.board)
                {
                    board = this.logicBoard.GetBoard()[piece.Square.GetRow(), piece.Square.GetCol()];
                    if (board != 0)
                        piece.PutImage(board);
                    else
                        piece.RemoveImage();
                }
                PictureBoxItem.originSquare = null;
                CleanBoard();
            }
        }
    }
}
