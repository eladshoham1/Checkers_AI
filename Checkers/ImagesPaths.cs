using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public static class ImagesPaths
    {
        private static string beginStr = "..\\..\\Images\\";
        private static string endStr = ".png";
        private static string white = "white";
        private static string black = "black";
        private static string blackKing = "blackKing";
        private static string whiteKing = "whiteKing";

        //הפעולה מקבלת כלי ומחזירה את הכתובת לתמונה שמתאימה לו
        public static string ImageFile(int piece)
        {
            if (piece == 0)
                return null;
            string str = beginStr;
            if (piece == 1)
                str += black;
            if (piece == -1)
                str += white;
            if (piece == 2)
                str += blackKing;
            if (piece == -2)
                str += whiteKing;
            str += endStr;
            return str;
        }
    }
}
