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
using System.Collections.Generic;

namespace PentagoWeb.Model.AI
{
    public abstract class AbstractTreeNode
    {
        public int Value { get; protected set; } // indicates this node have not been evaluated
        //public abstract AbstractTreeNode[] Expand();
        public abstract int Evaluate(object info);
        public int Depth { get; private set; }
        public AbstractTreeNode(int depth)
        {
            Value = int.MinValue;
            this.Depth = depth;
        }
    }
}
