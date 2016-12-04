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
using System.Collections.Generic;
using PentagoWeb.Model;
using PentagoWeb.Model.Board;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;


namespace PentagoWeb.Model.AI
{
    public class AIPlayer:AbstractPlayer
    {
        int depth;
        public AIPlayer(PentagoBoard board,  int depth)
            : base(board)
        {
            this.depth = depth;
        }

        public override void Active()
        {
            //if(color.State == Status.StateEnum.black)
            //    PentagoNode<Status>.Values = new int[] { 0, 1, 3, 9, 30, 256 };
            //else
                
            //PentagoNode<Status>.Values = new int[]{0,1,4,16,64,256};
            var optimalNode = FindOptimalNode2(board, depth);
            //if(depth == 3)
                Debug.WriteLine(String.Format("({0},{1}), section:{2} clockwise:{3}", optimalNode.PlaceX, optimalNode.PlaceY, optimalNode.SectionNo,optimalNode.Clockwise));
            OnPlace(optimalNode.PlaceX, optimalNode.PlaceY);
            if (optimalNode.Clockwise)
                OnRotateClockwise(board.Sections[optimalNode.SectionNo]);
            else
                OnRotateCounterclockwise(board.Sections[optimalNode.SectionNo]);
            return;
        }

        public virtual PentagoNode FindOptimalNode2(PentagoBoard board, int depth)
        {
            var clone = board.Clone() as PentagoBoard;
            var root = new PentagoNode(clone, base.color, depth);
            root.Evaluate(this.color);
            return root.BestChild;
        }

        //public PentagoNode FindOptimalNode(PentagoBoard board, int depth)
        //{
        //    var root = new PentagoNode(board, base.color, depth);
        //    AbstractTreeNode optimal = null;
        //    Stack<AbstractTreeNode> nodeStack = new Stack<AbstractTreeNode>();
        //    nodeStack.Push(root);
        //    while (nodeStack.Count > 0)
        //    {
        //        var node = nodeStack.Pop();
        //        if (node.Depth == depth)
        //        {
        //            node.Evaluate(color);
        //            if (optimal == null)
        //            {
        //                optimal = node;
        //            }
        //            else
        //            {
        //                optimal = (node.Value > optimal.Value) ? node : optimal;
        //            }
        //        }
        //        else
        //        {
        //            var children = node.Expand();
        //            foreach (var child in children)
        //            {
        //                nodeStack.Push(child);
        //            }
        //        }
        //    }
        //    var optimalInFirstPly = optimal as PentagoNode;
        //    while (optimalInFirstPly.Depth != 1)
        //        optimalInFirstPly = optimalInFirstPly.Parent;
        //    return optimalInFirstPly;

        //}
    }
}
