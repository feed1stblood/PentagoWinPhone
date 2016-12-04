using PentagoWeb.Model.Board;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;

namespace PentagoWeb.Model.AI
{
    public class PentagoNode : AbstractTreeNode
    {
        /// <summary>
        /// values for 5-in-a-line, 4-in-a-line, 3, 2, 1 and 0
        /// </summary>
        protected readonly int[] Values = { 0, 1, 3, 9, 30, 100 };
        //public static int[] Values = { 0, 1, 2, 4, 8, 256 };

        protected PentagoBoard currentState;
        protected Status currentTurn;
        protected int maxDepth;

        public enum NodeTypeEnum { MAX, MIN }
        NodeTypeEnum nodeType;

        public PentagoNode Parent { get; protected set; }
        public PentagoNode BestChild { get; protected set; }
        //record the action
        public int PlaceX { get; protected set; }
        public int PlaceY { get; protected set; }
        public int SectionNo { get; protected set; }
        public bool Clockwise { get; protected set; }

        /// <summary>
        /// constructor for root node
        /// </summary>
        /// <param name="currentState">current board</param>
        /// <param name="currentTurn">current turn(who is moving now)</param>
        public PentagoNode(PentagoBoard currentState, Status currentTurn, int maxDepth)
            : base(0)
        {
            this.currentState = currentState;
            this.currentTurn = currentTurn;
            this.currentTurn = this.NextTurn();
            this.Parent = null;
            this.maxDepth = maxDepth;
            this.nodeType = NodeTypeEnum.MAX;
            System.Diagnostics.Debug.WriteLine(Depth);
        }

        public PentagoNode(PentagoBoard currentState, PentagoNode parent)
            : base(parent.Depth + 1)
        {
            this.currentState = currentState;
            this.currentTurn = parent.NextTurn();
            this.Parent = parent;
            this.maxDepth = parent.maxDepth;
            this.nodeType = (parent.nodeType == NodeTypeEnum.MAX) ? NodeTypeEnum.MIN : NodeTypeEnum.MAX;
            //System.Diagnostics.Debug.WriteLine(Depth+" "+currentTurn.State);
        }

        public  AbstractTreeNode[] Expand()
        {
            LinkedList<PentagoNode> result = new LinkedList<PentagoNode>();
            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (currentState[i, j].State == Status.StateEnum.empty)
                    {
                        for (int k = 0; k < currentState.Sections.Count; k++)
                        {
                            result.AddLast(new PentagoNode(currentState, this)
                            {
                                PlaceX = i,
                                PlaceY = j,
                                SectionNo = k,
                                Clockwise = true,
                            });
                            //if the board state remains the same after rotating clockwise, it will be the same after rotating counterclockwise
                            //so the node can be eliminated
                            if (!currentState.Sections[k].IsSameInBothDirection)
                            {
                                result.AddLast(new PentagoNode(currentState, this)
                                {
                                    PlaceX = i,
                                    PlaceY = j,
                                    SectionNo = k,
                                    Clockwise = false,
                                });
                            }
                        }
                    }
                }
            return result.ToArray();
        }

        public virtual int EvaluateCurrent(object info)
        {
            var player = info as Status;
            int totalValue = 0;
            int[,] piecesInLine = new int[32, 2];
            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (currentState[i, j].State == player.State)
                    {
                        for (int k = 0; k < posTable[i, j].Length; k++)
                        {
                            piecesInLine[posTable[i, j][k] - 1, 0]++;
                        }
                    }
                    else if (currentState[i, j].State != Status.StateEnum.empty)
                    {
                        for (int k = 0; k < posTable[i, j].Length; k++)
                        {
                            piecesInLine[posTable[i, j][k] - 1, 1]++;
                        }
                    }
                }
            for (int i = 0; i < piecesInLine.GetLength(0); i++)
            {
                int ourPieces = piecesInLine[i, 0];
                int theirPieces = piecesInLine[i, 1];
                //if (ourPieces > 3 || theirPieces > 3)
                //Debug.WriteLine(i + ":" + ourPieces + "," + theirPieces);
                if (ourPieces == 0)
                    totalValue += -Values[theirPieces];
                else if (theirPieces == 0)
                    totalValue += Values[ourPieces];
            }
            Value = totalValue;
            return totalValue;
        }

        public override int Evaluate(object info)
        {
            if (Depth == maxDepth || currentState.IsFull(Status.StateEnum.empty))
            {
                return EvaluateCurrent(info);
                //List<Status> consideringLine = new List<Status>(n);
                //for (int i = 0; i < currentState.TotalWidth; i++)
                //    for (int j = 0; j < currentState.TotalHeight; j++)
                //    {
                //        //check for horizontal case
                //        if (i + n <= currentState.TotalWidth)
                //        {
                //            consideringLine.Clear();
                //            for (int k = 0; k < n; k++)
                //                consideringLine.Add(currentState[i + k, j]);
                //            var value = GetValueForALine(consideringLine, player);
                //            totalValue += value;
                //        }

                //        //check for vertical case
                //        if (j + n <= currentState.TotalWidth)
                //        {
                //            consideringLine.Clear();
                //            for (int k = 0; k < n; k++)
                //                consideringLine.Add(currentState[i, j + k]);
                //            var value = GetValueForALine(consideringLine, player);
                //            totalValue += value;
                //        }

                //        //check for diagonal case
                //        if (i + n <= currentState.TotalWidth && j + n <= currentState.TotalWidth)
                //        {
                //            consideringLine.Clear();
                //            for (int k = 0; k < n; k++)
                //                consideringLine.Add(currentState[i + k, j + k]);
                //            var value = GetValueForALine(consideringLine, player);
                //            totalValue += value;
                //        }
                //        if (i + n <= currentState.TotalWidth && j - n >= -1)
                //        {
                //            consideringLine.Clear();
                //            for (int k = 0; k < n; k++)
                //                consideringLine.Add(currentState[i + k, j - k]);
                //            var value = GetValueForALine(consideringLine, player);
                //            totalValue += value;
                //        }
                //    }

            }
            else
            {
                bool win = currentState.HasConsecutiveNPiece(currentTurn,5);
                bool lose = currentState.HasConsecutiveNPiece(NextTurn(), 5);
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


                var children = this.Expand().Cast<PentagoNode>();

                if (this.nodeType == NodeTypeEnum.MAX)
                {
                    foreach (var child in children)
                    {
                        child.DoAction();
                        child.Evaluate(info);
                        BestChild = (BestChild == null) ? child : ((BestChild.Value > child.Value) ? BestChild : child);
                        child.Rollback();
                    }
                }
                else
                {
                    foreach (var child in children)
                    {

                        child.DoAction();
                        child.Evaluate(info);
                        BestChild = (BestChild == null) ? child : ((BestChild.Value < child.Value) ? BestChild : child);
                        child.Rollback();
                    }
                }
                Value = BestChild.Value;
                return BestChild.Value;
            }
        }



        private int GetValueForALine(List<Status> line, Status player)
        {
            if (line.Count < 5)
                return 0;
            //our color
            Status we = player;
            int ourPieces = line.Count(s => s.EqualsTo(we));
            int theirPieces = 5 - ourPieces - line.Count(s => s.State == Status.StateEnum.empty);

            if (ourPieces == 0)
                return -Values[theirPieces];
            else if (theirPieces == 0)
                return Values[ourPieces];
            else
                return 0;
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
            currentState[PlaceX, PlaceY].State = currentTurn.State;

            if (Clockwise)
                currentState.Sections[SectionNo].StartRotatingClockwise();
            else
                currentState.Sections[SectionNo].StartRotatingCounterclockwise();
        }

        protected void Rollback()
        {
            if (Clockwise)
                currentState.Sections[SectionNo].StartRotatingCounterclockwise();
            else
                currentState.Sections[SectionNo].StartRotatingClockwise();
            currentState[PlaceX, PlaceY].State = Status.StateEnum.empty;


        }

        readonly int[,][] posTable ={
            {new int[]{1,13,25},new int[]{2,13,19,29},new int[]{3,13,19},new int[]{4,13,19},new int[]{5,13,19,32},new int[]{6,19,28}},
            {new int[]{1,7,14,30},new int[]{2,8,14,20,25,26},new int[]{3,9,14,20,29},new int[]{4,10,14,20,32},new int[]{5,11,14,20,27,28},new int[]{6,12,20,31}},
            {new int[]{1,7,15},new int[]{2,8,15,21,30},new int[]{3,9,15,21,25,26,32},new int[]{4,10,15,21,27,28,29},new int[]{5,11,15,21,31},new int[]{6,12,21}},
            {new int[]{1,7,16},new int[]{2,8,16,22,32},new int[]{3,9,16,22,27,28,30},new int[]{4,10,16,22,25,26,31},new int[]{5,11,16,22,29},new int[]{6,12,22}},
            {new int[]{1,7,17,32},new int[]{2,8,17,23,27,28},new int[]{3,9,17,23,31},new int[]{4,10,17,23,30},new int[]{5,11,17,23,25,26},new int[]{6,12,23,29}},
            {new int[]{7,18,27},new int[]{8,18,24,31},new int[]{9,18,24},new int[]{10,18,24},new int[]{11,18,24,30},new int[]{12,24,26}}
        };
    }
}
