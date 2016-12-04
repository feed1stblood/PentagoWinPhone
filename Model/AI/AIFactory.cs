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
    public class AIFactory
    {
        public delegate Action DecisionMaker(PentagoBoard board, Status.StateEnum color, int para, NodeSelecter sel, StateEvaluater eva);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentState">the current board</param>
        /// <param name="player">the player we evaluate for</param>
        /// <returns>the value</returns>
        public delegate int StateEvaluater(PentagoBoard currentState, Status.StateEnum player);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originValue"></param>
        /// <param name="currentValue"></param>
        /// <returns>greater than 0 if  the latter one is better</returns>
        public delegate int NodeSelecter(int originValue, int currentValue);

        public static AIPlayerEx NextAI(PentagoBoard board, int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return AIFactory.NextAI(board, 2, true, 2);
                case 1:
                    return AIFactory.NextAI(board, 2, true, 1);
                case 2:
                    return AIFactory.NextAI(board, 2, false, 1);                
                case 3:
                    return AIFactory.NextAI(board, 2, true, 0);
                default:
                    return AIFactory.NextAI(board, 2, false, 0);

            }
        }

        public static AIPlayerEx NextAI(PentagoBoard board, int para, bool localSearch, int rndSel)
        {
            var result = new AIPlayerEx(board, para)
            {
                Evaluater = EvaluateNoRotation,
            };
            if (localSearch)
                result.Decide = MinimaxLocal;
            else
                result.Decide = Minimax;
            switch (rndSel)
            {
                case 0:
                    result.Selecter = NormallySelect;
                    break;
                case 1:
                    result.Selecter = StochasticallySelect;
                    break;
                case 2:
                    result.Selecter = StochasticallySelect2;
                    break;
            }
            return result;
        }

        public static int NormallySelect(int originValue, int currentValue)
        {
            return currentValue.CompareTo(originValue);
        }

        public static int StochasticallySelect(int originValue, int currentValue)
        {
            var tem = 25.0f;
            var rnd = new Random();
            var gen = rnd.NextDouble();
            var prob = 1 / (1 + Math.Exp((originValue - currentValue) / tem));
            if (gen < prob)
                return 1;
            else
                return -1;
        }

        public static int StochasticallySelect2(int originValue, int currentValue)
        {
            var tem = 1000.0f;
            var rnd = new Random();
            var gen = rnd.NextDouble();
            var prob = 1 / (1 + Math.Exp((originValue - currentValue) / tem));
            if (gen < prob)
                return 1;
            else
                return -1;
        }

        public static int EvaluateNoRotation(PentagoBoard currentState, Status.StateEnum player)
        {
            int totalValue = 0;
            int[,] piecesInLine = new int[32, 2];
            var posTable = PentagoEvaData.LineTable;
            var Values = PentagoEvaData.Values;

            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (currentState[i, j].State == player)
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
            return totalValue;
        }

        public static Action MinimaxLocal(PentagoBoard currentState, Status.StateEnum color, int depth, NodeSelecter sel, StateEvaluater eva)
        {
            Action bestAction = null;
            Placement curPlc = new Placement()
            {
                Player = color,
            };
            Rotation curRtt = new Rotation();

            var searchSpace = new bool[6,6];
            
            if(currentState.IsEmpty(Status.StateEnum.empty))
            {
                for (int i = 0; i < currentState.TotalWidth; i++)
                    for (int j = 0; j < currentState.TotalHeight; j++)
                        searchSpace[i, j] = true;
            }
            else if(currentState.CountSteps(Status.StateEnum.empty) < 5)
            {
                for (int i = 0; i < currentState.TotalWidth; i++)
                    for (int j = 0; j < currentState.TotalHeight; j++)
                    {
                        if (currentState[i, j].State == GetOppoColor(color))
                        {
                            for (int k = (i == 0) ? 0 : -1; k <= ((i == currentState.TotalWidth - 1) ? 0 : 1); k++)
                                for (int l = (j == 0) ? 0 : -1; l <= ((j == currentState.TotalHeight - 1) ? 0 : 1); l++)
                                    if (!(k == 0 && l == 0) && !searchSpace[i + k, j + l] && currentState[i + k, j + l].State == Status.StateEnum.empty)
                                        searchSpace[i + k, j + l] = true;
                        }
                    }
            }
            else 
            {
                for (int i = 0; i < currentState.TotalWidth; i++)
                    for (int j = 0; j < currentState.TotalHeight; j++)
                    {
                        if (currentState[i, j].State != Status.StateEnum.empty)
                        {
                            for (int k = (i == 0) ? 0 : -1; k <= ((i == currentState.TotalWidth - 1) ? 0 : 1); k++)
                                for (int l = (j == 0) ? 0 : -1; l <= ((j == currentState.TotalHeight - 1) ? 0 : 1); l++)
                                    if (!(k == 0 && l == 0) && !searchSpace[i + k, j + l] && currentState[i + k, j + l].State == Status.StateEnum.empty)
                                        searchSpace[i + k, j + l] = true;
                        }
                    }
            }

            var nospace = searchSpace.Cast<bool>().All(e => !e);
            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (searchSpace[i, j])
                    {
                        //curPlc = new Placement();
                        curPlc.X = i;
                        curPlc.Y = j;
                        curPlc.Do(currentState);

                        for (int k = 0; k < currentState.Sections.Count; k++)
                        {
                            //curRtt = new Rotation();
                            curRtt.SectionNo = k;
                            curRtt.Clockwise = true;
                            curRtt.Do(currentState);

                            if (depth <= 1 || currentState.IsFull(Status.StateEnum.empty)
                                || currentState.HasConsecutiveNPiece(color, 5)
                                || currentState.HasConsecutiveNPiece(GetOppoColor(color), 5))
                            {
                                int value = eva(currentState, color);
                                if (bestAction == null)
                                {
                                    bestAction = new Action(curPlc, curRtt);
                                    bestAction.Value = value;
                                }
                                else if (sel(bestAction.Value, value) > 0)
                                {
                                    bestAction.SetAction(curPlc, curRtt);
                                    bestAction.Value = value;
                                }
                            }
                            else
                            {
                                var subAction = Minimax(currentState, GetOppoColor(color), depth - 1, sel, eva);
                                if (bestAction == null)
                                {
                                    bestAction = new Action(curPlc, curRtt);
                                    bestAction.Value = -subAction.Value;
                                }
                                else if (sel(bestAction.Value, -subAction.Value) > 0)
                                {
                                    bestAction.SetAction(curPlc, curRtt);
                                    bestAction.Value = -subAction.Value;
                                }
                            }
                            curRtt.RollBack(currentState);

                            //if the board state remains the same after rotating clockwise, it will be the same after rotating counterclockwise
                            //so the node can be eliminated
                            if (!currentState.Sections[k].IsSameInBothDirection)
                            {
                                curRtt.Clockwise = false;
                                curRtt.Do(currentState);

                                if (depth == 1 || currentState.IsFull(Status.StateEnum.empty)
                                        || currentState.HasConsecutiveNPiece(color, 5)
                                        || currentState.HasConsecutiveNPiece(GetOppoColor(color), 5))
                                {
                                    int value = eva(currentState, color);
                                    if (bestAction == null)
                                    {
                                        bestAction = new Action(curPlc, curRtt);
                                        bestAction.Value = value;
                                    }
                                    else if (sel(bestAction.Value, value) > 0)
                                    {
                                        bestAction.SetAction(curPlc, curRtt);
                                        bestAction.Value = value;
                                    }
                                }
                                else
                                {
                                    var subAction = Minimax(currentState, GetOppoColor(color), depth - 1, sel, eva);
                                    if (sel(bestAction.Value, -subAction.Value) > 0)
                                    {
                                        bestAction.SetAction(curPlc, curRtt);
                                        bestAction.Value = -subAction.Value;
                                    }
                                }
                                curRtt.RollBack(currentState);
                            }

                        }
                        curPlc.RollBack(currentState);
                    }
                }
            return bestAction;
        }

        public static Action Minimax(PentagoBoard currentState, Status.StateEnum color, int depth, NodeSelecter sel, StateEvaluater eva)
        {
            Action bestAction = null;
            Placement curPlc = new Placement()
            {
                Player = color,
            };
            Rotation curRtt = new Rotation();


            for (int i = 0; i < currentState.TotalWidth; i++)
                for (int j = 0; j < currentState.TotalHeight; j++)
                {
                    if (currentState[i, j].State == Status.StateEnum.empty)
                    {
                        //curPlc = new Placement();
                        curPlc.X = i;
                        curPlc.Y = j;
                        curPlc.Do(currentState);

                        for (int k = 0; k < currentState.Sections.Count; k++)
                        {
                            //curRtt = new Rotation();
                            curRtt.SectionNo = k;
                            curRtt.Clockwise = true;
                            curRtt.Do(currentState);

                            if (depth <= 1 || currentState.IsFull(Status.StateEnum.empty)
                                || currentState.HasConsecutiveNPiece(color, 5)
                                || currentState.HasConsecutiveNPiece(GetOppoColor(color), 5))
                            {
                                int value = eva(currentState, color);
                                if (bestAction == null)
                                {
                                    bestAction = new Action(curPlc, curRtt);
                                    bestAction.Value = value;
                                }
                                else if (sel(bestAction.Value, value) > 0)
                                {
                                    bestAction.SetAction(curPlc, curRtt);
                                    bestAction.Value = value;
                                }
                            }
                            else
                            {
                                var subAction = Minimax(currentState, GetOppoColor(color), depth - 1, sel, eva);
                                if (bestAction == null)
                                {
                                    bestAction = new Action(curPlc, curRtt);
                                    bestAction.Value = -subAction.Value;
                                }
                                else if (sel(bestAction.Value, -subAction.Value) > 0)
                                {
                                    bestAction.SetAction(curPlc, curRtt);
                                    bestAction.Value = -subAction.Value;
                                }
                            }
                            curRtt.RollBack(currentState);

                            //if the board state remains the same after rotating clockwise, it will be the same after rotating counterclockwise
                            //so the node can be eliminated
                            if (!currentState.Sections[k].IsSameInBothDirection)
                            {
                                curRtt.Clockwise = false;
                                curRtt.Do(currentState);

                                if (depth == 1 || currentState.IsFull(Status.StateEnum.empty)
                                        || currentState.HasConsecutiveNPiece(color, 5)
                                        || currentState.HasConsecutiveNPiece(GetOppoColor(color), 5))
                                {
                                    int value = eva(currentState, color);
                                    if (bestAction == null)
                                    {
                                        bestAction = new Action(curPlc, curRtt);
                                        bestAction.Value = value;
                                    }
                                    else if (sel(bestAction.Value, value) > 0)
                                    {
                                        bestAction.SetAction(curPlc, curRtt);
                                        bestAction.Value = value;
                                    }
                                }
                                else
                                {
                                    var subAction = Minimax(currentState, GetOppoColor(color), depth - 1, sel, eva);
                                    if (sel(bestAction.Value, -subAction.Value) > 0)
                                    {
                                        bestAction.SetAction(curPlc, curRtt);
                                        bestAction.Value = -subAction.Value;
                                    }
                                }
                                curRtt.RollBack(currentState);
                            }
                            
                        }
                        curPlc.RollBack(currentState);
                    }
                }
            return bestAction;
        }

        public static Status.StateEnum GetOppoColor(Status.StateEnum color)
        {
            if (color == Status.StateEnum.black)
                return Status.StateEnum.white;
            else
                return Status.StateEnum.black;
        }
        public class Placement
        {
            public Status.StateEnum Player { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public void Do(PentagoBoard board)
            {
                board[X, Y].State = Player;
            }
            public void RollBack(PentagoBoard board)
            {
                board[X, Y].State = Status.StateEnum.empty;
            }
        }

        public class Rotation
        {
            public int SectionNo { get; set; }
            public bool Clockwise { get; set; }
            public void Do(PentagoBoard board)
            {
                if (Clockwise)
                    board.Sections[SectionNo].StartRotatingClockwise();
                else
                    board.Sections[SectionNo].StartRotatingCounterclockwise();
            }
            public void RollBack(PentagoBoard board)
            {
                if (!Clockwise)
                    board.Sections[SectionNo].StartRotatingClockwise();
                else
                    board.Sections[SectionNo].StartRotatingCounterclockwise();
            }
        }

        public class Action
        {
            public Placement Plc { get; set; }
            public Rotation Rtt { get; set; }
            public int Value { get; set; }
            public void Do(PentagoBoard board)
            {
                Plc.Do(board);
                Rtt.Do(board);
            }
            public void RollBack(PentagoBoard board)
            {
                Rtt.RollBack(board);
                Plc.RollBack(board);
            }
            public Action(Placement plc, Rotation rtt)
            {
                this.Plc = new Placement();
                this.Rtt = new Rotation();
                SetAction(plc, rtt);
            }
            public void SetAction(Placement plc, Rotation rtt)
            {
                this.Plc.X = plc.X;
                this.Plc.Y = plc.Y;
                this.Rtt.Clockwise = rtt.Clockwise;
                this.Rtt.SectionNo = rtt.SectionNo;
            }
        }


    }
}
