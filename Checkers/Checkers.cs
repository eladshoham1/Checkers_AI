using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public partial class Checkers : Form
    {
        private LogicBoard logicBoard;
        private PictureBoxItem[,] board;

        public Checkers()
        {
            InitializeComponent();
        }

        private void Checkers_Load(object sender, EventArgs e)
        {
            this.logicBoard = new LogicBoard();
            BoardPictures();
        }

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
                    board[i, j].Click += new System.EventHandler(this.PlayerClick);
                }
            }
            this.Refresh();
        }

        private void PlayerClick(object sender, EventArgs e)
        {
            PictureBoxItem piece = (PictureBoxItem)sender;
            if (PictureBoxItem.OriginFrog < 0)
            {
                if (frogColor != 0)
                    OriginChoose(frogPic, frogColor);
            }
            else
                if (frogColor == 0)
                    TargetChoose(frogPic);
                else //התחרט
                {
                    int oldOrigin = PictureBoxItem.OriginFrog;
                    this.frogs[oldOrigin].
                        PutFrogImg(this.logicBoard.GetFrogsArr()[oldOrigin]);
                    PictureBoxItem.OriginFrog = -1;
                    if (PictureBoxItem.OriginFrog != frogPic.Index)
                        OriginChoose(frogPic, frogColor);
                }





            PictureBoxItem player = (PictureBoxItem)sender;
            if (this.logicBoard.GetBoard()[player.Sq.GetRow(), player.Sq.GetCol()] != 0)
                PictureBoxItem.originSq = new Square(player.Sq.GetRow(), player.Sq.GetCol());
            else
            {
                Square origin = PictureBoxItem.originSq;
                Square target = player.Sq;
                if (this.logicBoard.CheckMove(origin, target))
                {
                    PictureBoxItem.originSq = target;
                    player.PutPlayerImg(this.logicBoard.GetBoard()[target.GetRow(), target.GetCol()]);
                    this.board[origin.GetRow(), origin.GetCol()].RemovePlayerImg();
                }
                else if (this.logicBoard.CheckRightEatMove(origin, target))
                {
                    this.board[origin.GetRow() + this.logicBoard.GetPlayer(), origin.GetCol() - this.logicBoard.GetPlayer()].RemovePlayerImg();
                }
                else if (this.logicBoard.CheckLeftEatMove(origin, target))
                {
                    this.board[origin.GetRow() + this.logicBoard.GetPlayer(), origin.GetCol() + this.logicBoard.GetPlayer()].RemovePlayerImg();
                }
                else if (this.logicBoard.CheckKingMove(origin, target))
                {
                }
                this.King(origin, target, piece);
                this.logicBoard.SetPlayer(this.logicBoard.GetPlayer() * -1);
                if (this.logicBoard.GetPlayer() == -1 || this.logicBoard.GetPlayer() == -2)
                    this.playerLab.Visible = true;
                else
                    this.playerLab.Visible = false;
                this.CheckEnd();
            }
        }

        private void CheckEnd()
        {
            if (this.logicBoard.IsWin())
                MessageBox.Show("You Win!");
            if (this.logicBoard.IsDraw())
                MessageBox.Show("Draw!");
            if (this.logicBoard.IsLost())
                MessageBox.Show("You Lost!");
        }
        
        private void resetBtn_Click(object sender, EventArgs e)
        {
            this.logicBoard = new LogicBoard();
            int board;
            foreach (PictureBoxItem pb in this.board)
            {
                board = this.logicBoard.GetBoard()[pb.Sq.GetRow(), pb.Sq.GetCol()];
                if (board != 0)
                    pb.PutPlayerImg(board);
                else
                    pb.RemovePlayerImg();
            }
            PictureBoxItem.originSq = null;
        }
    }
}
