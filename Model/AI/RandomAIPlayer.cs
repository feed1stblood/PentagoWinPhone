using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PentagoWeb.Model.Board;
using System.Linq;
using System.Collections.Generic;

namespace PentagoWeb.Model.AI
{
    public class RandomAIPlayer:AbstractPlayer
    {
        public RandomAIPlayer(PentagoBoard board)
            : base(board)
        {
        }
        public override void Active()
        {
            //Random place
            int x,y;
            Random rnd = new Random();
            do
            {
                x = rnd.Next(board.TotalWidth);
                y = rnd.Next(board.TotalHeight);
            } while (board[x, y].State != Status.StateEnum.empty);
            base.OnPlace(x, y);
            //random rotate
            int direction = rnd.Next(2);
            if(direction==0)
                base.OnRotateClockwise(board.Sections[rnd.Next(board.Sections.Count)]);
            else
                base.OnRotateCounterclockwise(board.Sections[rnd.Next(board.Sections.Count)]);
        }
    }
}
