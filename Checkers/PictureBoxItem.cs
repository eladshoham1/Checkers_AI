using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Checkers
{
    public class PictureBoxItem : PictureBox
    {
        private Square square; //משבצת
        public static Square originSquare = null; //משבצת בחירה

        //הפעולה בונה תמונה של הכלים בלוח
        public PictureBoxItem(Square square, int piece)
            : base()
        {
            this.square = square;
            this.PutPicture(piece);
            int x = 106 + square.GetCol() * 93;
            int y = 63 + square.GetRow() * 93;
            this.Location = new System.Drawing.Point(x, y);
            this.Size = new System.Drawing.Size(90, 90);
            if ((square.GetRow() + square.GetCol()) % 2 != 0)
                this.BackColor = System.Drawing.Color.Sienna;
            else
                this.BackColor = System.Drawing.Color.Ivory;
        }

        //הפעולה מקבלת את כתובת של התמונה של השחקן ומדפיסה אותו
        private void PutPicture(int piece)
        {
            string imgFile = ImagesPaths.ImageFile(piece);
            if (imgFile == null)
                this.Image = null;
            else
            {
                Image im = Image.FromFile(imgFile);
                this.Image = new Bitmap(im, 72, 72);
                this.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        //הפעולה מחזירה את המשבצת
        public Square Square
        {
            get { return this.square; }
            set { this.square = value; }
        }

        //הפעולה מדפיסה תמונה
        public void PutImage(int piece)
        {
            PutPicture(piece);
        }

        //הפועלה מוחקת תמונה
        public void RemoveImage()
        {
            this.Image = null;
        }
    }
}
