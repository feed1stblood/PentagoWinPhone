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
namespace PentagoWeb.Model
{
    public abstract class AbstractPlayer
    {
        public delegate void PlacingDelegate(int x, int y);
        public event PlacingDelegate Place;
        public enum RotateDirection{Clockwise, Counterclockwise}
        public delegate void RotateDelegate(RotatableBoard<Status> section);
        public event RotateDelegate RotateClockwise;
        public event RotateDelegate RotateCounterclockwise;

        protected PentagoBoard board;
        internal Status color;
        public AbstractPlayer(PentagoBoard board)
        {
            this.board = board;
        }

        public abstract void Active();

        protected void OnPlace(int x,int y)
        {
            if(Place != null)
                Place(x,y);
        }

        protected void OnRotateClockwise(RotatableBoard<Status> section)
        {
            if (RotateClockwise != null)
                RotateClockwise(section);
        }

        protected void OnRotateCounterclockwise(RotatableBoard<Status> section)
        {
            if (RotateCounterclockwise != null)
                RotateCounterclockwise(section);
        }
    }
}
