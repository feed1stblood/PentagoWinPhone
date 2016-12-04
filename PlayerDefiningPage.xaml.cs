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
    public partial class PlayerDefiningPage : PhoneApplicationPage
    {
        public PlayerDefiningPage()
        {
            InitializeComponent();
            player2List.SelectedIndex = 1;
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            ViewModel.ViewModelLocator.MainStatic.StartNewGame(player1List.SelectedIndex, player2List.SelectedIndex);
            ViewModel.ViewModelLocator.MainStatic.Player1Icon = player1List.SelectedIndex;
            ViewModel.ViewModelLocator.MainStatic.Player2Icon = player2List.SelectedIndex;
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));                         

        }
    }
}