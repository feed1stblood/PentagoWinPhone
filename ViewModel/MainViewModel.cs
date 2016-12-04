using GalaSoft.MvvmLight;


using System.Windows;
using PentagoWeb.Model.Board;
using GalaSoft.MvvmLight.Command;
using PentagoWeb.Model.AI;
using PentagoWeb.Model;
using System.Linq;
using System;
using System.Windows.Controls;
namespace Pentago.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public string Welcome
        {
            get
            {
                return "Welcome to MVVM Light";
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"

            }
            RotateButtonVisibility = Visibility.Collapsed;

            WholeBoard = new PentagoBoard();
            //WholeBoard[0, 0] = new Status(Status.StateEnum.black);
            //WholeBoard[1, 3] = new Status(Status.StateEnum.black);
            //WholeBoard[4, 5] = new Status(Status.StateEnum.black);
            //WholeBoard[5, 2] = new Status(Status.StateEnum.white);
            //WholeBoard[3, 0] = new Status(Status.StateEnum.white);



            //control = new GameControl(this.WholeBoard, null ,new AIPlayer(WholeBoard,Status.StateEnum.black));
            //control = new GameControl(this.WholeBoard, null, null);

            CmdStartHumanVsHuman = new RelayCommand(HumanVsHuman);
            CmdStartComputerVsHuman = new RelayCommand(ComputerVsHuman);
            CmdStartComputerVsComputer = new RelayCommand(ComputerVsComputer);
            CmdStartChallenge = new RelayCommand(StartChallenge);
            CmdStartTestLevel = new RelayCommand(this.StartTestLevels);
            CmdNextGame = new RelayCommand(NextGame);
            CmdVsRandom = new RelayCommand(HumanVsRandomPlayer);
            CmdCloseMessage = new RelayCommand(OnClosingMessage);

            HumanVsHuman();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                WholeBoard[1, 0].State = Status.StateEnum.white;
                WholeBoard[2, 0].State = Status.StateEnum.white;
                WholeBoard[0, 2].State = Status.StateEnum.white;
                WholeBoard[5, 1].State = Status.StateEnum.white;
                WholeBoard[4, 2].State = Status.StateEnum.white;

                WholeBoard[1, 3].State = Status.StateEnum.white;
                WholeBoard[0, 4].State = Status.StateEnum.white;
                WholeBoard[2, 4].State = Status.StateEnum.white;
                WholeBoard[3, 5].State = Status.StateEnum.white;

                WholeBoard[0, 0].State = Status.StateEnum.black;
                WholeBoard[1, 1].State = Status.StateEnum.black;
                WholeBoard[2, 1].State = Status.StateEnum.black;
                WholeBoard[2, 2].State = Status.StateEnum.black;
                WholeBoard[3, 1].State = Status.StateEnum.black;
                WholeBoard[4, 1].State = Status.StateEnum.black;
                WholeBoard[0, 5].State = Status.StateEnum.black;
                WholeBoard[1, 4].State = Status.StateEnum.black;
                WholeBoard[2, 3].State = Status.StateEnum.black;
                WholeBoard[4, 4].State = Status.StateEnum.black;
            }
        }

    

        void Endgame1()
        {
            WholeBoard[1, 0].State = Status.StateEnum.white;
            WholeBoard[2, 0].State = Status.StateEnum.white;
            WholeBoard[0, 2].State = Status.StateEnum.white;
            WholeBoard[5, 1].State = Status.StateEnum.white;
            WholeBoard[4, 2].State = Status.StateEnum.white;

            WholeBoard[1, 3].State = Status.StateEnum.white;
            WholeBoard[0, 4].State = Status.StateEnum.white;
            WholeBoard[2, 4].State = Status.StateEnum.white;
            WholeBoard[3, 5].State = Status.StateEnum.white;

            WholeBoard[0, 0].State = Status.StateEnum.black;
            WholeBoard[1, 1].State = Status.StateEnum.black;
            WholeBoard[2 ,1].State = Status.StateEnum.black;
            WholeBoard[2, 2].State = Status.StateEnum.black;
            WholeBoard[3, 1].State = Status.StateEnum.black;
            WholeBoard[4, 1].State = Status.StateEnum.black;
            WholeBoard[0, 5].State = Status.StateEnum.black;
            WholeBoard[1, 4].State = Status.StateEnum.black;
            WholeBoard[2, 3].State = Status.StateEnum.black;
            WholeBoard[4, 4].State = Status.StateEnum.black;
        }

        void OnClosingMessage()
        {
            MessagePanelVisibililty =  Visibility.Collapsed;
            var result = MessageBox.Show("Play another game with same player settings?", "Replay?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                StartNewGame(this.Player1Icon, this.Player2Icon);
            }
        }

        void RotateAction(bool on)
        {
            RotateButtonVisibility = (on) ? Visibility.Visible : Visibility.Collapsed;
        }

        public GameControl Control { get; protected set; }

        void GameOver(Status winner)
        {
            switch (winner.State)
            {
                case Status.StateEnum.empty:
                    GameMessage = "Draw";
                    break;
                case Status.StateEnum.black:
                    GameMessage = "Black Win";
                    break;
                case Status.StateEnum.white:
                    GameMessage = "White Win";
                    break;
            }
        }

        string title;
        public String Title
        { get { return title; } protected set { title = value; RaisePropertyChanged("Title"); } }


        public string[] LevelTestResult { get; protected set; }
        void GameOverTestLevelMode(Status winner)
        {
            string result;
            if (winner.State == Status.StateEnum.empty)
            {
                result = "Draw";
            }
            //player won
            else if ((winner.State == Status.StateEnum.black && IsPlayerBlack) || ((winner.State == Status.StateEnum.white && !IsPlayerBlack)))
            {
                result = "Win";
            }
            else
            {
                result = "Lose";
            }
            LevelTestResult[Difficulty / 3] = result;
            RaisePropertyChanged("LevelTestResult");
            if (Difficulty == 9)
            {
            }
            else
            {
                NextGameButtonVisibility = Visibility.Visible;
                Difficulty += 3;
            }
        }

        void GameOverChallengeMode(Status winner)
        {
            if (winner.State == Status.StateEnum.empty)
            {
                PlayerDraw();
            }
            //player won
            else if ((winner.State == Status.StateEnum.black && IsPlayerBlack) || ((winner.State == Status.StateEnum.white && !IsPlayerBlack)))
            {
                PlayerWon();
            }
            else
            {
                PlayerLose();
            }

            NextGameButtonVisibility = Visibility.Visible;

        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        Status boardContentSelectedItem = null;

        public Status BoardContentSelectedItem
        {
            get { return boardContentSelectedItem; }
            set
            {

                if (value != null)
                {
                    Control.Placeable = false;
                    RaisePropertyChanged("BoardContentSelectedItem");
                    Control.HumanPlace(value);
                }
            }
        }




        Visibility rotateButtonVisibility;
        public Visibility RotateButtonVisibility
        {
            get { return rotateButtonVisibility; }
            private set
            {
                rotateButtonVisibility = value;
                base.RaisePropertyChanged("RotateButtonVisibility");
            }
        }

        string gameMessage;

        public string GameMessage
        {
            get { return gameMessage; }
            private set
            {
                gameMessage = value;
                base.RaisePropertyChanged("GameMessage");
                if(MessageComing != null)
                    MessageComing(this, null);
            }
        }
        public PentagoBoard WholeBoard { get; private set; }

        public void HideRotateButton(object sender, EventArgs e)
        {
            RotateButtonVisibility = Visibility.Collapsed;

        }

        void HumanVsHuman()
        {
            WholeBoard = new PentagoBoard();

            //WholeBoard[1, 1].State = Status.StateEnum.black;
            //WholeBoard[1, 2].State = Status.StateEnum.black;
            //WholeBoard[1, 4].State = Status.StateEnum.black;

            //WholeBoard[4, 1].State = Status.StateEnum.white;
            //WholeBoard[4, 3].State = Status.StateEnum.white;
            //WholeBoard[4, 4].State = Status.StateEnum.white;

            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, null, null);
            //Control = new GameControl(this.WholeBoard, null, new AIPlayerCut(WholeBoard, 1));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        void ComputerVsHuman()
        {
            WholeBoard = new PentagoBoard();
            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, null, AIFactory.NextAI(WholeBoard, 2, false, 1));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        void HumanVsRandomPlayer()
        {
            WholeBoard = new PentagoBoard();
            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, null, new RandomAIPlayer(WholeBoard));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }



        void ComputerVsComputer()
        {
            WholeBoard = new PentagoBoard();
            //WholeBoard[1, 2].State = Status.StateEnum.black;
            //WholeBoard[1, 1].State = Status.StateEnum.black;
            //WholeBoard[1, 4].State = Status.StateEnum.black;
            //WholeBoard[4, 1].State = Status.StateEnum.white;
            //WholeBoard[4, 4].State = Status.StateEnum.white;
            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, AIFactory.NextAI(WholeBoard, 2, false, 0), new AIPlayer(WholeBoard, 2));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        void StartChallenge()
        {
            //MenuButtonVisibility = Visibility.Collapsed;
            Difficulty = 1;
            RaisePropertyChanged("Difficulty");
            postGameProcess = new GameControl.GameOverHandler(this.GameOverChallengeMode);
            this.ChaModeVisibility = Visibility.Visible;
            NextGame();


        }

        void StartTestLevels()
        {
            //MenuButtonVisibility = Visibility.Collapsed;
            Difficulty = 0;
            postGameProcess = new GameControl.GameOverHandler(this.GameOverTestLevelMode);
            this.TstModeVisibility = Visibility.Visible;
            LevelTestResult = Enumerable.Repeat("To be determined", 4).ToArray();
            RaisePropertyChanged("LevelTestResult");
            NextGame();
        }

        GameControl.GameOverHandler postGameProcess;

        void NextGame()
        {
            NextGameButtonVisibility = Visibility.Collapsed;
            GameMessage = "";
            Title = "Diff " + Difficulty;
            WholeBoard = new PentagoBoard();
            base.RaisePropertyChanged("WholeBoard");
            var AI = AIFactory.NextAI(WholeBoard, Difficulty / 2);
            if (Difficulty % 2 == 1)
            {
                Control = new GameControl(WholeBoard, AI, null);
                Control.GameOver = postGameProcess;
                Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
                Control.Start();
            }
            else
            {
                Title += " - You go first";
                Control = new GameControl(WholeBoard, null, AI);
                Control.GameOver = postGameProcess;
                Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
                Control.Start();
            }
        }



        public RelayCommand CmdStartHumanVsHuman { get; set; }
        public RelayCommand CmdStartComputerVsHuman { get; set; }
        public RelayCommand CmdStartComputerVsComputer { get; set; }
        public RelayCommand CmdStartChallenge { get; set; }
        public RelayCommand CmdNextGame { get; set; }
        public RelayCommand CmdVsRandom { get; set; }
        public RelayCommand CmdStartTestLevel { get; set; }
        public RelayCommand CmdCloseMessage { get; set; }
        

        Visibility nextGameButtonVisibility = Visibility.Collapsed;
        public Visibility NextGameButtonVisibility
        {
            get { return nextGameButtonVisibility; }
            private set
            {
                nextGameButtonVisibility = value;
                base.RaisePropertyChanged("NextGameButtonVisibility");
            }
        }

        Visibility messagePanelVisibililty = Visibility.Collapsed;
        public Visibility MessagePanelVisibililty
        {
            get { return messagePanelVisibililty; }
            private set
            {
                messagePanelVisibililty = value;
                base.RaisePropertyChanged("MessagePanelVisibililty");
            }
        }

        Visibility chaModeVisibility = Visibility.Collapsed;
        public Visibility ChaModeVisibility
        {
            get { return chaModeVisibility; }
            private set
            {
                chaModeVisibility = value;
                base.RaisePropertyChanged("ChaModeVisibility");
            }
        }

        Visibility tstModeVisibility = Visibility.Collapsed;
        public Visibility TstModeVisibility
        {
            get { return tstModeVisibility; }
            private set
            {
                tstModeVisibility = value;
                base.RaisePropertyChanged("TstModeVisibility");
            }
        }

        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lose { get; set; }
        public int Difficulty { get; set; }

        int GamePlayed { get { return Win + Draw + Lose; } }

        bool IsPlayerBlack { get { return Difficulty % 2 == 1; } }

        void PlayerWon()
        {
            GameMessage = "You win";
            Difficulty = Math.Min(Difficulty + 2, 9);
            RaisePropertyChanged("Difficulty");
            Win++;
            RaisePropertyChanged("Win");
        }

        void PlayerDraw()
        {
            GameMessage = "Draw";
            Difficulty = Math.Min(Difficulty + new Random().Next(2), 9);
            RaisePropertyChanged("Difficulty");
            Draw++;
            RaisePropertyChanged("Draw");
        }

        void PlayerLose()
        {
            GameMessage = "You lose";
            Difficulty = Math.Max(0, Difficulty - 1);
            RaisePropertyChanged("Difficulty");
            Lose++;
            RaisePropertyChanged("Lose");
        }

        public event EventHandler MessageComing;

        public void StartNewGame(int player1Kind, int player2Kind)
        {
            WholeBoard = new PentagoWeb.Model.Board.PentagoBoard();
            RaisePropertyChanged("WholeBoard");

            AbstractPlayer player1, player2;
            if (Endgame == 1)
            {
                Endgame1();
                player1 = null;
                player2 = PentagoWeb.Model.AI.AIFactory.NextAI(WholeBoard, 4);
            }
            else
            {
                player1 = player1Kind == 0 ? null : PentagoWeb.Model.AI.AIFactory.NextAI(WholeBoard, player1Kind - 1);
                player2 = player2Kind == 0 ? null : PentagoWeb.Model.AI.AIFactory.NextAI(WholeBoard, player2Kind - 1);
            }
            Control = new PentagoWeb.Model.GameControl(WholeBoard, player1, player2);
            
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        public int Player1Icon { get; set; }
        public int Player2Icon { get; set; }

        public int Endgame { get; set; }
    }
}