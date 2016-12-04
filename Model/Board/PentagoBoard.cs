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
using System.Collections.Generic;

namespace PentagoWeb.Model.Board
{
    public class PentagoBoard : IncorporateBoard<RotatableBoard<Status>, Status>
    {
        public PentagoBoard():base(ConstructBoard(),2)           
        {
                       
        }

        private PentagoBoard(IEnumerable<RotatableBoard<Status>> sections)
            :base(sections,2)
        {
        }

        private static IEnumerable<RotatableBoard<Status>> ConstructBoard()
        {
            var sections = new Collection<RotatableBoard<Status>>();
            sections.Add(new RotatableBoard<Status>(3, 3, new Status(Status.StateEnum.empty)));
            sections.Add(new RotatableBoard<Status>(3, 3, new Status(Status.StateEnum.empty)));
            sections.Add(new RotatableBoard<Status>(3, 3, new Status(Status.StateEnum.empty)));
            sections.Add(new RotatableBoard<Status>(3, 3, new Status(Status.StateEnum.empty)));
            return sections;
        }

        public override AbstractBoard<Status> Clone()
        {
            var clone = base.Clone();
            return new PentagoBoard((clone as IncorporateBoard<RotatableBoard<Status>, Status>).Sections);
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            for (int j = 0; j < this.TotalHeight; j++)
            {
                for (int i = 0; i < this.TotalWidth; i++)
                {
                    str.Append(this[i, j].ToString()+",");
                }
                str.Append("\r\n");
            }
            return str.ToString();
        }
    }
}
