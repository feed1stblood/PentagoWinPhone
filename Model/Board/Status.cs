using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace PentagoWeb.Model.Board
{
    /// <summary>
    /// This class is for avoiding the bug with selection of listbox.
    /// </summary> 
    public class Status : ICloneable<Status>,INotifyPropertyChanged ,IComparable<Status>
    {
        public enum StateEnum { empty, black, white }

        StateEnum state;

        public StateEnum State
        {
            get { return state; }
            set 
            { 
                state = value;
               
                if(PropertyChanged!=null)
                    PropertyChanged(this, new PropertyChangedEventArgs("State"));
                
            }
        }

        public Status():this(StateEnum.empty)
        {
        }

        public Status(StateEnum state)
        {
            State = state;
        }

        public Status Opponent
        {
            get
            {
                return this.State == StateEnum.black ? StateEnum.white : StateEnum.black;
            }
        }

        public Status Clone()
        {
            //return this.MemberwiseClone() as Status;
            return this.State;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public bool EqualsTo(Status obj)
        {
            if (obj == null)
                return false;

            if (this.State != obj.State)
                return false;
            return true;
        }

        public static implicit operator Status ( StateEnum operand)
        {
            return new Status(operand);
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }
}
