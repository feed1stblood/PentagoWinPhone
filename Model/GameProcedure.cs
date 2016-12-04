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

namespace PentagoWeb.Model
{
    public class GameProcedure
    {
        public enum PrimaryProcedureEnum { White, Black };
        public enum SubProcedureEnum { Placement, Rotation, Checking };

        public PrimaryProcedureEnum CurrentPrimaryStage { get; private set; }
        public SubProcedureEnum CurrentSubStage { get; private set; }

        public SubProcedureEnum Next()
        {
            var prePri = CurrentPrimaryStage;
            var preSub = CurrentSubStage;
            int intPri = (int)CurrentPrimaryStage;
            int intSub = (int)CurrentSubStage;
            intSub = (intSub + 1) % Helper.EnumHelper.GetValues(typeof(SubProcedureEnum)).Length;
            if (intSub == 0)
                intPri = (intPri + 1) % Helper.EnumHelper.GetValues(typeof(PrimaryProcedureEnum)).Length;
            CurrentPrimaryStage = (PrimaryProcedureEnum)intPri;
            CurrentSubStage = (SubProcedureEnum)intSub;
            

            if (GameForward != null)
                GameForward(prePri, preSub, CurrentPrimaryStage, CurrentSubStage);

            return CurrentSubStage;
        }

        public static GameProcedure Instance = new GameProcedure();
        public delegate void ForwardDelegate(PrimaryProcedureEnum previousPrimaryStage, SubProcedureEnum previousSubStage, PrimaryProcedureEnum currentPrimaryStage, SubProcedureEnum currentSubStage);
        public event ForwardDelegate GameForward;
    }
}
