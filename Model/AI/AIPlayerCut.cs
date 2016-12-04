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

namespace PentagoWeb.Model.AI
{
    public class AIPlayerCut:AIPlayer
    {
        public AIPlayerCut(PentagoBoard board, int depth)
            : base(board, depth)
        {
        }

        public override PentagoNode FindOptimalNode2(PentagoBoard board, int depth)
        {
            var clone = board.Clone() as PentagoBoard;
            var root = new PentagoNodeEva2(clone, base.color, depth);
            root.Evaluate(this.color);
            return root.BestChild;
        }
    }
}
