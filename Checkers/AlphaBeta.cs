using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    //המחלקה סטטית ולא מייצרת עצמים והמטרה של המחלקה היא לחשב את המהלך הטוב ביותר עבור המחשב
    //המחלקה משתמשת בעצמים מסוג alphabetaboard
    //אם המחלקה היא סטטית כל הפעולות הם סטטיות
    public static class AlphaBeta
    {
        const int MAXPLAYER = 1;
        const int MINPLAYER = -1;

        //טענת כניסה: הפעולה מקבלת לוח מסוג AlphaBetaBoard
        //טענת יציאה: הפעולה מחזירה עצם מטיפוס מהלך שהוא המהלך הטוב ביותר שיש מהלוח הזה
        public static Move BestMove(AlphaBetaBoard b)
        {
            Move move = null; //איתחול ל null כדי שיהיה מה להחזיר
            List<AlphaBetaBoard> children = b.Children(); //רשימה של כל הלוחות הבאים ללוח הנוכחי

            foreach (AlphaBetaBoard child in children)
            {
                child.Val = child.GetTotalScore();
                child.Val += Iterate(child, child.Depth, -999999, 999999); //חישוב ציון הלוח
            }
            //אם השחקן הוא המקסימום הוא צריך את הלוח עם הציון הכי גבוה
            if (b.GetTurn() == MAXPLAYER)
            {
                int bestVal = children.ElementAt(0).Val; //מקסימום
                foreach (AlphaBetaBoard child in children)
                {
                    int val = child.Val;
                    if (val >= bestVal)
                    {
                        bestVal = val;
                        move = child.SelectedMove;
                    }
                }
            }
            else
            {
                int bestVal = children.ElementAt(0).Val; //מינימום
                foreach (AlphaBetaBoard child in children)
                {
                    int val = child.Val;
                    if (val <= bestVal)
                    {
                        bestVal = val;
                        move = child.SelectedMove;
                    }
                }
            }
            return move;
        }

        private static int Iterate(AlphaBetaBoard node, int depth, int alpha, int beta)
        {
            if (node.CheckEnd()) //במקרה שהמשחק נגמר לא ממשיך לפתח את עץ המשחק
            {
                node.Val = node.GetTotalScore();
                return node.Val * 1000; 
            }

            if (depth == 0) //לא מפתחים יותר את הלוח אלא מחשבים את הערך שלו לפי הערכים הנוכחיים
            {
                node.Val = node.GetTotalScore();
                return node.Val;
            }
            //אם הגענו לכאן זה אומר שאנחנו באמצע העץ והשחקן יכול להיות מקסימום או מינימום
            if (node.Parent.GetTurn() == MAXPLAYER)
            {
                foreach (AlphaBetaBoard child in node.Children())
                {
                    alpha = Math.Max(alpha, Iterate(child, depth - 1, alpha, beta));
                    if (beta < alpha)
                    {
                        break;
                    }

                }
                return alpha;
            }
            else //MINPLAYER
            {
                foreach (AlphaBetaBoard child in node.Children())
                {
                    beta = Math.Min(beta, Iterate(child, depth - 1, alpha, beta));
                    if (beta < alpha)
                    {
                        break;
                    }
                }
                return beta;
            }
        }
    }
}
