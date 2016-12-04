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
using System.ComponentModel;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;

namespace PentagoWeb.Model.Board
{
    public class RotatableBoard<T> : Board<T> where T : ICloneable<T>, IComparable<T>
    {
        public RotatableBoard(int width, int height, T defaultValue)
            : base(width, height, defaultValue)
        {
            CmdStartRotatingClockwise = new RelayCommand(StartRotatingClockwise);
            CmdStartRotatingCounterclockwise = new RelayCommand(StartRotatingCounterclockwise);
            CmdAfterRotatingClockwise = new RelayCommand(AfterRotatingClockwise);
            CmdAfterRotatingCounterclockwise = new RelayCommand(AfterRotatingCounterclockwise);
            CmdDrag = new RelayCommand<EventArgs>(Drag); 
        }

        private void Drag(EventArgs oe)
        {
            CmdStartRotatingClockwise.Execute(null);
            var e = (ManipulationCompletedEventArgs)oe;
            //Debug.WriteLine(((FrameworkElement)e.OriginalSource).ActualWidth);
            //Debug.WriteLine(e.ManipulationOrigin);            
            //Debug.WriteLine(e.TotalManipulation.Translation);
            double center = ((FrameworkElement)e.OriginalSource).ActualWidth / 2;
            Point ori = new Point(e.ManipulationOrigin.X - center, e.ManipulationOrigin.Y - center);
            Point end = new Point(e.ManipulationOrigin.X + e.TotalManipulation.Translation.X - center, e.ManipulationOrigin.Y + e.TotalManipulation.Translation.Y - center);
            Debug.WriteLine(ori);
            Debug.WriteLine(end);
            double oriAngle = Math.Atan(ori.Y / ori.X);
            double endAngle = Math.Atan(end.Y / end.X);
            Debug.WriteLine(endAngle - oriAngle);
            //Debug.WriteLine(e.TotalManipulation.Scale);

        }

        private RotatableBoard(int width, int height, Collection<T> content)
            : base(width, height, content)
        {
        }

        public double Angle { get; set; }
        
        public void StartRotatingClockwise()
        {

            //List<T> replica = new List<T>(base.Content);

            //for (int i = 0; i < Width; i++)
            //{
            //    for (int j = 0; j < Height; j++)
            //    {
            //        this[i, j] = replica[(Width - 1 - i) * Height + j];
            //    }
            //}
            if (StartRotating != null)
                StartRotating(this, null);
            if (RotateClockwise != null)
                RotateClockwise(this,null);
            else
            {
                
                AfterRotatingClockwise();
            }

        }

        public void StartRotatingCounterclockwise()
        {
            //List<T> replica = new List<T>(base.Content);
            //for (int i = 0; i < Width; i++)
            //{
            //    for (int j = 0; j < Height; j++)
            //    {
            //        this[i, j] = replica[i * Height + Height - 1 - j];
            //    }
            //}
            if (StartRotating != null)
                StartRotating(this, null);
            if (RotateCounterclockwise != null)
                RotateCounterclockwise(this, null);
            else
            {
                
                AfterRotatingCounterclockwise();
            }
        }

        void AfterRotatingClockwise()
        {
            Angle += 90;
            CorrectSize();
            //for (int i = 0; i < Width; i++)
            //    for (int j = 0; j < Height; j++)
            //        System.Diagnostics.Debug.WriteLine(this[i, j]);
            if (DispatcherHelper.UIDispatcher == Deployment.Current.Dispatcher)
            {
                int a = 0;
            }
            if (BoardRotate != null)
                BoardRotate(this);
        }

        void AfterRotatingCounterclockwise()
        {
            Angle -= 90;
            CorrectSize();
            if (BoardRotate != null)
                BoardRotate(this);
        }

        void CorrectSize()
        {
            int tmp;
            tmp = Width;
            Width = Height;
            Height = tmp;
        }

        public bool IsSameInBothDirection
        {
            get
            {
                if (Width != Height)
                    return false;
                for (int i = 0; i < Width; i++)
                {
                    for (int j = i; j < Width; j++)
                    {
                        //check the rotation corresponding position are same
                        if (!this[i, j].EqualsTo(this[Width - 1 - i, Width - 1 - j]))
                            return false;
                    }
                }
                return true;
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CmdStartRotatingClockwise { get; set; }
        public RelayCommand CmdStartRotatingCounterclockwise { get; set; }
        public RelayCommand CmdAfterRotatingClockwise { get; set; }
        public RelayCommand CmdAfterRotatingCounterclockwise { get; set; }
        public RelayCommand<EventArgs> CmdDrag { get; set; }

        public delegate void RotateEvent(RotatableBoard<T> sender);
        public event RotateEvent BoardRotate;

        public event EventHandler RotateClockwise;
        public event EventHandler RotateCounterclockwise;
        public event EventHandler StartRotating;

        public override AbstractBoard<T> Clone()
        {
            var baseClone = base.Clone() as Board<T>;
            return new RotatableBoard<T>(baseClone.Width, baseClone.Height, baseClone.Content);
        }

        public override T this[int x, int y]
        {
            get
            {
                var angle = this.Angle % 360;
                if (angle == 0)
                {
                    return base[x, y];
                }
                if (angle == -90 || angle == 270)
                {
                    
                    return base[Width - 1 - y, x];
                }
                if (angle == 90 || angle == -270)
                {

                    return base[y, Height - 1 - x];
                }
                else
                    return base[Width - 1 - x, Height - 1 - y];
            }
            set
            {
                var angle = this.Angle % 360;
                if (angle == 0)
                {
                    base[x, y] = value ;
                }
                if (angle == -90 || angle == 270)
                {
                    base[(Width - 1 - x) * Height + y] = value;
                }
                if (angle == 90 || angle == -270)
                {
                   
                    base[x * Height + Height - 1 - y] = value;
                }
                else
                    base[Width - 1 - x, Height - 1 - y] = value;

            }
           
        }

    }
}
