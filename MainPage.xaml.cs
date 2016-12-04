using Microsoft.Phone.Controls;
using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace Pentago
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DispatcherHelper.Initialize();
        }

        

        private void phoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Abort this game?", "Confirmation", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
            }
            else
            {
                e.Cancel = true;
            }
        }

       

    
    }
}
