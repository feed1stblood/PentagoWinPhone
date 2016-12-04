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
using PentagoWeb.Model.AI;
using PentagoWeb.Model.Board;
using System.Linq;
using System.Diagnostics;

namespace PentagoWeb.Model
{
    public class Tournament
    {
        const int N = 5;
        string[,] result = new string[N, N];
        PentagoBoard board;
        int i = 0, j = 0;
        Status.StateEnum curPlayer;
        AbstractPlayer PlayerBlack;
        AbstractPlayer PlayerWhite;
        public void Start()
        {
            Next();
        }

        public void Next()
        {
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {
                    board = new PentagoBoard();
                    PlayerBlack = Produce(board, i);
                    PlayerWhite = Produce(board, j);

                    PlayerBlack.color = Status.StateEnum.black;
                    PlayerBlack.Place += new AbstractPlayer.PlacingDelegate(Place);
                    PlayerBlack.RotateClockwise += new AbstractPlayer.RotateDelegate(RotateClockwise);
                    PlayerBlack.RotateCounterclockwise += new AbstractPlayer.RotateDelegate(RotateCounterclockwise);
                    PlayerWhite.color = Status.StateEnum.white;
                    PlayerWhite.Place += new AbstractPlayer.PlacingDelegate(Place);
                    PlayerWhite.RotateClockwise += new AbstractPlayer.RotateDelegate(RotateClockwise);
                    PlayerWhite.RotateCounterclockwise += new AbstractPlayer.RotateDelegate(RotateCounterclockwise);
                    curPlayer = Status.StateEnum.white;
                    PlayerWhite.Active();
                }
            }
            for (i = 0; i < N; i++)
            {
                string line = "";
                for (j = 0; j < N; j++)
                {
                    line += result[i, j];
                }
                Debug.WriteLine(line);
            }
            
        }

        void Place(int x, int y)
        {
            Place(board[x, y]);
        }

        void Place(Status pos)
        {
            if (pos.State != Status.StateEnum.empty)
                throw new Exception();
             pos.State = curPlayer;
        }

        void RotateClockwise(RotatableBoard<Status> section)
        {
             section.StartRotatingClockwise();
             Turn();      
        }

        void RotateCounterclockwise(RotatableBoard<Status> section)
        {
            section.StartRotatingCounterclockwise();
            Turn();
        }

        void Turn()
        {
            bool blackWin = board.HasConsecutiveNPiece(Status.StateEnum.black, 5);
            bool whiteWin = board.HasConsecutiveNPiece(Status.StateEnum.white, 5);
            if (blackWin && whiteWin)
                OnGameOver(Status.StateEnum.empty);
            else if (blackWin)
                OnGameOver(Status.StateEnum.black);
            else if (whiteWin)
                OnGameOver(Status.StateEnum.white);
            else if (!board.Content.Any(e => e.State == Status.StateEnum.empty))
                OnGameOver(Status.StateEnum.empty);
            else if (curPlayer == Status.StateEnum.black)
            {
                curPlayer = Status.StateEnum.white;
                PlayerWhite.Active();
            }
            else
            {
                curPlayer = Status.StateEnum.black;
                PlayerBlack.Active();
            }
        }

        void OnGameOver(Status.StateEnum winner)
        {
            result[i, j] = winner == Status.StateEnum.empty ? "♢" : winner == Status.StateEnum.black ? "←" : "↑";
            //Next();
        }

        public AbstractPlayer Produce(PentagoBoard board, int level)
        {
            return AIFactory.NextAI(board, level);
        }
    }
}
