using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class Move
    {
        private Square origin; //משבצת המקור
        private Square target; //משבצת היעד

        //הפעולה בונה מהלך ממשבצת מקור למשבצת יעד
        public Move(Square origin, Square target)
        {
            this.origin = origin;
            this.target = target;
        }

        //הפעולה מחזירה את משבצת המקור
        public Square GetOrigin()
        {
            return this.origin;
        }

        //הפעולה מחזירה את משבצת היעד
        public Square GetTarget()
        {
            return this.target;
        }

        //הפעולה מדפיסה את המהלך
        public override string ToString()
        {
            return "[" + this.origin + "," + this.target + "]";
        }
    }
}
