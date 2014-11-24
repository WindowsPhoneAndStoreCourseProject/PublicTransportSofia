using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SofiaPublicTransport.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SofiaPublicTransport.Views
{
    public sealed partial class CommandBarView : CommandBar
    {
        public static event EventHandler ShowUserInputPopup;

        public CommandBarView()
        {
            this.InitializeComponent();
            //this.DataContext = new ViewModelPopupForUserInput();
        }

        private void OnFindAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            //var appBarButton = sender as AppBarButton;

            //var commandBar = appBarButton.Parent;// as AppBar;
            //var test = appBarButton.Parent as CommandBar;
            //if(commandBar == null)
            //{
            //    return;
            //}

            //var page = commandBar.Parent as HubPage;
            //PopupForUserInputView.PopupForUserInput.Visibility = Visibility.Visible;
           // PopupForUserInputView.PopupForUserInput.IsOpen = true;
            try
            {
                ShowUserInputPopup("CommandBarView", new EventArgs());
            }
            catch (Exception)
            {


            }   
        }
    }
}
