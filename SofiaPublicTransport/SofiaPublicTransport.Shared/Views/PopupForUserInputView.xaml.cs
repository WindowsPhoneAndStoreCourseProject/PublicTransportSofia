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
    public sealed partial class PopupForUserInputView : UserControl
    {
        public static Popup PopupForUserInput = default(Popup);

        public PopupForUserInputView()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModelPopupForUserInput();
        }

        private void onPopupForUserInputLoaded(object sender, RoutedEventArgs e)
        {
            PopupForUserInputView.PopupForUserInput = sender as Popup;
        }
    }
}
