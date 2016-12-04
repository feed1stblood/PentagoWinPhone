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
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using System.ComponentModel;

namespace PentagoWeb.Model
{
    public class GameControl : INotifyPropertyChanged
    {
        public enum PlayerEnum { White, Black };
        public enum SubProcedureEnum { Placement, Rotation, Checking };

        public PlayerEnum CurrentPrimaryStage { get; private set; }
        public SubProcedureEnum CurrentSubStage { get; private set; }

        PentagoBoard board;

        AbstractPlayer PlayerBlack;
        AbstractPlayer PlayerWhite;

        public void Start()
        {
            if (PlayerWhite != null)
            {
                new Thread(new ThreadStart(GetCurrentPlayer().Active)).Start();
            }
            else
            {
                Placeable = true;
            }
        }
        
        public GameControl(PentagoBoard board, AbstractPlayer player1, AbstractPlayer player2)
        {
            this.board = board;
            PlayerBlack = player2;
            PlayerWhite = player1;
            if (PlayerBlack != null)
            {
                PlayerBlack.color = Status.StateEnum.black;
                PlayerBlack.Place += new AbstractPlayer.PlacingDelegate(Place);
                PlayerBlack.RotateClockwise += new AbstractPlayer.RotateDelegate(RotateClockwise);
                PlayerBlack.RotateCounterclockwise += new AbstractPlayer.RotateDelegate(RotateCounterclockwise);
            }
            if (PlayerWhite != null)
            {
                PlayerWhite.color = Status.StateEnum.white;
                PlayerWhite.Place += new AbstractPlayer.PlacingDelegate(Place);
                PlayerWhite.RotateClockwise += new AbstractPlayer.RotateDelegate(RotateClockwise);
                PlayerWhite.RotateCounterclockwise += new AbstractPlayer.RotateDelegate(RotateCounterclockwise);
            }


            foreach (var section in board.Sections)
            {
                section.StartRotating += new EventHandler(section_StartRotating);
                section.BoardRotate += new RotatableBoard<Status>.RotateEvent(HumanRotate);
            }

            Placeable = false;
        }

        void section_StartRotating(object sender, EventArgs e)
        {
            OnRotateButtonSwitch(false);
        }

        void HumanRotate(RotatableBoard<Status> sender)
        {
            Next();
             
        }

        private ManualResetEvent mutex = new ManualResetEvent(true);

        void Next()
        {
            var prePri = CurrentPrimaryStage;
            var preSub = CurrentSubStage;
            int intPri = (int)CurrentPrimaryStage;
            int intSub = (int)CurrentSubStage;
            intSub = (intSub + 1) % Helper.EnumHelper.GetValues(typeof(SubProcedureEnum)).Length;
            if (intSub == 0)
                intPri = (intPri + 1) % Helper.EnumHelper.GetValues(typeof(PlayerEnum)).Length;
            CurrentPrimaryStage = (PlayerEnum)intPri;
            CurrentSubStage = (SubProcedureEnum)intSub;

            if (CurrentSubStage == SubProcedureEnum.Checking)
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
                {
                    OnGameOver(Status.StateEnum.empty);
                }
                else
                {
                    Next();
                    return;
                }
            }

            OnTurnChange();

            if (CurrentSubStage == SubProcedureEnum.Rotation && IsCurrentPlayerHuman())
            {
                OnRotateButtonSwitch(true);
            }
            if (CurrentSubStage == SubProcedureEnum.Placement)
            {
                if (!IsCurrentPlayerHuman())
                {
                    new Thread(new ThreadStart(GetCurrentPlayer().Active)).Start();
                    //GetCurrentPlayer().Active();
                }
                else
                    Placeable = true;
            }

        }

        public void HumanPlace(Status pos)
        {
            if (IsCurrentPlayerHuman())
            {
                Place(pos);
            }
        }

        void Place(int x, int y)
        {
            Place(board[x, y]);
            
        }

        void Place(Status pos)
        {
            mutex.Reset();
            DispatcherHelper.CheckBeginInvokeOnUI(()=>{
                if (CurrentSubStage == SubProcedureEnum.Placement && pos.State == Status.StateEnum.empty)
                {
                    Placeable = false;
                    pos.State = (CurrentPrimaryStage == PlayerEnum.Black) ? Status.StateEnum.black : Status.StateEnum.white;
                    Next();
                    mutex.Set();
                }
            });
            
        }

        //public void HumanRotateClockwise(RotatableBoard<Status> section)
        //{
        //    if (IsCurrentPlayerHuman())
        //    {
        //        RotateClockwise(section);

        //    }
        //}

        //public void HumanRotateCounterclockwise(RotatableBoard<Status> section)
        //{
        //    if ( IsCurrentPlayerHuman())
        //    {
        //        RotateCounterclockwise(section);
        //    }
        //}

        

        void RotateClockwise(RotatableBoard<Status> section)
        {
            mutex.WaitOne();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (CurrentSubStage == SubProcedureEnum.Rotation)
                {                    
                        section.StartRotatingClockwise();
                        //Next();
                }
            });
        }

        void RotateCounterclockwise(RotatableBoard<Status> section)
        {
            mutex.WaitOne();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (CurrentSubStage == SubProcedureEnum.Rotation)
                {

                        section.StartRotatingCounterclockwise();
                        //Next();
                }
            });
        }

        AbstractPlayer GetCurrentPlayer()
        {
            return (CurrentPrimaryStage == PlayerEnum.Black) ? this.PlayerBlack : this.PlayerWhite;
        }
        bool IsCurrentPlayerHuman()
        {
            if (GetCurrentPlayer() == null)
                return true;
            else
                return false;
        }


        public delegate void RotatabilityChangeHandler(bool visible);
        public RotatabilityChangeHandler RotatabilityChange;
        public delegate void GameOverHandler(Status winner);
        public GameOverHandler GameOver;
        public delegate void TurnChangeHandler(PlayerEnum currentPlayer, SubProcedureEnum currentProcedure, bool isAI);
        public TurnChangeHandler TurnChange;
        public event EventHandler Player1sTurn;
        public event EventHandler Player2sTurn;

        void OnRotateButtonSwitch(bool visible)
        {
            if (RotatabilityChange != null)
                RotatabilityChange(visible);
        }

        void OnGameOver(Status winner)
        {
            if (GameOver != null)
                GameOver(winner);
        }

        void OnTurnChange()
        {
            if(TurnChange!=null)
                TurnChange(CurrentPrimaryStage, CurrentSubStage, IsCurrentPlayerHuman());
            if (CurrentSubStage == SubProcedureEnum.Placement)
            {
                if (CurrentPrimaryStage == PlayerEnum.White)
                {
                    if (Player1sTurn != null)
                        Player1sTurn(this, null);
                }
                else
                {
                    if (Player2sTurn != null)
                        Player2sTurn(this, null);
                }

            }
        }

        bool placeable;

        public bool Placeable
        {
            get { return placeable; }
            set
            {
                placeable = value;
                RaisePropertyChanged("Placeable");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string pName)
        {
            if(PropertyChanged!=null)
                PropertyChanged(this, new PropertyChangedEventArgs(pName));
        }
    }
}