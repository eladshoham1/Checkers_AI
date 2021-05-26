using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class Square
    {
        private int row; //שורה
        private int col; //עמודה

        //הפעולה בונה משבצת משורה ועמודה
        public Square(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        //הפעולה מחזירה את השורה
        public int GetRow()
        {
            return this.row;
        }

        //הפעולה מחזירה את העמודה
        public int GetCol()
        {
            return this.col;
        }

        //הפעולה מדפיסה את מיקום המשבצת במערך
        public override string ToString()
        {
            return "<" + this.row + ", " + this.col + ">";
        }
    }
}
