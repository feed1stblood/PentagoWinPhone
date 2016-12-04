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
using System.Diagnostics;

namespace PentagoWeb.Model.AI
{
    public class AIPlayerEx : AbstractPlayer
    {
        internal AIFactory.DecisionMaker Decide;
        internal AIFactory.StateEvaluater Evaluater;
        internal AIFactory.NodeSelecter Selecter;
        int depth;
        public AIPlayerEx(PentagoBoard board, int depth)
            :base(board)
        {
            this.depth = depth;
        }

        public override void Active()
        {
            var optimalNode = Decide(board.Clone() as PentagoBoard, base.color.State, depth, Selecter, Evaluater);
            //if(depth == 3)
            //Debug.WriteLine(String.Format("({0},{1}), section:{2} clockwise:{3}", optimalNode.Plc.X, optimalNode.Plc.Y, optimalNode.Rtt.SectionNo, optimalNode.Rtt.Clockwise));
            OnPlace(optimalNode.Plc.X, optimalNode.Plc.Y);
            if (optimalNode.Rtt.Clockwise)
                OnRotateClockwise(board.Sections[optimalNode.Rtt.SectionNo]);
            else
                OnRotateCounterclockwise(board.Sections[optimalNode.Rtt.SectionNo]);
            return;
        }

    }
}
