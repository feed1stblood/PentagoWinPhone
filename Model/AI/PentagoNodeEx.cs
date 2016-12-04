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

namespace PentagoWeb.Model.AI
{
    public class PentagoNodeEx : AbstractTreeNode
    {
        internal PentagoWeb.Model.AI.PentagoNodeFactory.StateEvaluater EvaluateState;
        internal PentagoWeb.Model.AI.PentagoNodeFactory.NodeSelecter SelectNode;
        internal PentagoWeb.Model.AI.PentagoNodeFactory.Expander Expand;

        public PentagoBoard CurrentState { get; protected set; }
        protected Status currentTurn;
        protected int maxDepth;

        public enum NodeTypeEnum { MAX, MIN }
        NodeTypeEnum nodeType;

        public PentagoNodeEx Parent { get; protected set; }
        public PentagoNodeEx BestChild { get; protected set; }
        //record the action
        public int PlaceX { get; internal set; }
        public int PlaceY { get; internal set; }
        public int SectionNo { get; internal set; }
        public bool Clockwise { get; internal set; }

        /// <summary>
        /// constructor for root node
        /// </summary>
        /// <param name="currentState">current board</param>
        /// <param name="currentTurn">current turn(who is moving now)</param>
        public PentagoNodeEx(PentagoBoard currentState, Status currentTurn, int maxDepth)
            : base(0)
        {
            this.CurrentState = currentState;
            this.currentTurn = currentTurn;
            this.currentTurn = this.NextTurn();
            this.Parent = null;
            this.maxDepth = maxDepth;
            this.nodeType = NodeTypeEnum.MAX;
            System.Diagnostics.Debug.WriteLine(Depth);
        }

        public PentagoNodeEx(PentagoBoard currentState, PentagoNodeEx parent)
            : base(parent.Depth + 1)
        {
            this.CurrentState = currentState;
            this.currentTurn = parent.NextTurn();
            this.Parent = parent;
            this.maxDepth = parent.maxDepth;
            this.nodeType = (parent.nodeType == NodeTypeEnum.MAX) ? NodeTypeEnum.MIN : NodeTypeEnum.MAX;
            this.EvaluateState = parent.EvaluateState;
            this.Expand = parent.Expand;
            this.SelectNode = parent.SelectNode;
            //System.Diagnostics.Debug.WriteLine(Depth+" "+currentTurn.State);
        }

        public override int Evaluate(object info)
        {
            if (Depth == maxDepth || CurrentState.IsFull(Status.StateEnum.empty))
            {
                return this.EvaluateState(CurrentState, info as Status);
            }
            else
            {
                bool win = CurrentState.HasConsecutiveNPiece(currentTurn,5);
                bool lose = CurrentState.HasConsecutiveNPiece(NextTurn(), 5);
                if (win && lose)
                {
                    this.Value = 0;
                    return this.Value;
                }
                if (win)
                {
                    this.Value = 10000;
                    return this.Value;                    
                }
                if (lose)
                {
                    this.Value = -10000;
                    return this.Value;
                }


                var children = this.Expand(CurrentState, info as Status).Cast<PentagoNodeEx>();

                if (this.nodeType == NodeTypeEnum.MAX)
                {
                    foreach (var child in children)
                    {
                        child.DoAction();
                        child.Evaluate(info);
                        BestChild = SelectNode(BestChild, child);
                        child.Rollback();
                    }
                }
                else
                {
                    foreach (var child in children)
                    {

                        child.DoAction();
                        child.Evaluate(info);
                        BestChild = SelectNode(BestChild, child);
                        child.Rollback();
                    }
                }
                Value = BestChild.Value;
                return BestChild.Value;
            }
        }

        protected Status NextTurn()
        {
            if (currentTurn.State == Status.StateEnum.black)
                return new Status(Status.StateEnum.white);
            else
                return new Status(Status.StateEnum.black);
        }

        protected void DoAction()
        {
            CurrentState[PlaceX, PlaceY].State = currentTurn.State;

            if (Clockwise)
                CurrentState.Sections[SectionNo].StartRotatingClockwise();
            else
                CurrentState.Sections[SectionNo].StartRotatingCounterclockwise();
        }

        protected void Rollback()
        {
            if (Clockwise)
                CurrentState.Sections[SectionNo].StartRotatingCounterclockwise();
            else
                CurrentState.Sections[SectionNo].StartRotatingClockwise();
            CurrentState[PlaceX, PlaceY].State = Status.StateEnum.empty;


        }
    }
}
