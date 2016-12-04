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
using System.Collections.ObjectModel;

namespace PentagoWeb.Model.Board
{
    public abstract class AbstractBoard<T>:ICloneable<AbstractBoard<T>>
    {
        public abstract T this [int x, int y]
        {
            get;
            set;
        }

        public abstract Collection<T> Content { get; }

        public abstract AbstractBoard<T> Clone();


    }
}
