using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public class EatMove : Move
    {
        private Square eat; //משבצת האכילה

        //הפעולה בונה מהלך ממשבצת מקור למשבצת יעד
        public EatMove(Square origin, Square target, Square eat)
            : base(origin, target)
        {
            this.eat = eat;
        }

        //הפעולה מחזירה את משבצת האכילה
        public Square GetEat()
        {
            return this.eat;
        }
    }
}
