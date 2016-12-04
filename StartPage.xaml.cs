using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Pentago
{
    public partial class StartPage : PhoneApplicationPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
			this.NavigationService.Navigate(new Uri("/PlayerDefiningPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            var result = MessageBox.Show("Sure to exit?", "Confirmation", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
            }
            else
            {
                e.Cancel = true;
            }
        }

        public class ExitException : Exception { }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/IntroPage.xaml", UriKind.Relative));
        }

        private void BtnExample_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            ViewModel.ViewModelLocator.MainStatic.Endgame = 1;
            ViewModel.ViewModelLocator.MainStatic.StartNewGame(0, 0);
            ViewModel.ViewModelLocator.MainStatic.Player1Icon = 0;
            ViewModel.ViewModelLocator.MainStatic.Player2Icon = 4;
			this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}