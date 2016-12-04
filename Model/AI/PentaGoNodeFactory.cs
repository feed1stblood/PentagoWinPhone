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
using System.Collections.Generic;
using System.Linq;

namespace PentagoWeb.Model.AI
{
    public class PentagoNodeFactory
    {
        public delegate int StateEvaluater(PentagoBoard currentState, Status player);
        public delegate PentagoNodeEx NodeSelecter(PentagoNodeEx node1, PentagoNodeEx node2);
        public delegate AbstractTreeNode[] Expander(PentagoBoard currentState, Status player);

        public static AbstractTreeNode[] Expand(PentagoNodeEx currentNode, Status player)
        {
            var currentState = currentNode.CurrentState;
            LinkedList<PentagoNodeEx> result = new LinkedList<PentagoNodeEx>();
            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (currentState[i, j].State == Status.StateEnum.empty)
                    {
                        for (int k = 0; k < currentState.Sections.Count; k++)
                        {
                            result.AddLast(new PentagoNodeEx(currentState, currentNode)
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
                                result.AddLast(new PentagoNodeEx(currentState, currentNode)
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

        public static int EvaluateCurrent(PentagoBoard currentState, Status player)
        {
            int totalValue = 0;
            int[,] piecesInLine = new int[32, 2];
            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (currentState[i, j].State == player.State)
                    {
                        for (int k = 0; k < PentagoEvaData.LineTable[i, j].Length; k++)
                        {
                            piecesInLine[PentagoEvaData.LineTable[i, j][k] - 1, 0]++;
                        }
                    }
                    else if (currentState[i, j].State != Status.StateEnum.empty)
                    {
                        for (int k = 0; k < PentagoEvaData.LineTable[i, j].Length; k++)
                        {
                            piecesInLine[PentagoEvaData.LineTable[i, j][k] - 1, 1]++;
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
                    totalValue += -PentagoEvaData.Values[theirPieces];
                else if (theirPieces == 0)
                    totalValue += PentagoEvaData.Values[ourPieces];
            }
            return totalValue;
        }

        static PentagoNodeEx SelectOptimalNode(PentagoNodeEx node1, PentagoNodeEx node2)
        {
            if (node1.Value > node2.Value)
                return node1;
            else
                return node2;
        }

        static PentagoNodeEx StochasticallySelect(PentagoNodeEx node1, PentagoNodeEx node2)
        {
            var tem = 100000.0f;
            var rnd = new Random();
            if (rnd.NextDouble() > 1 / (1 + Math.Exp((node1.Value - node2.Value) / tem)))
                return node1;
            else
                return node2;
        }


    }
}
