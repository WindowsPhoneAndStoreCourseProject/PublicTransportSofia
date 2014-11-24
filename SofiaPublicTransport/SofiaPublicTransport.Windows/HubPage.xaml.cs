using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

using SofiaPublicTransport.Data;
using SofiaPublicTransport.DataModel;
using SofiaPublicTransport.Common;
using SofiaPublicTransport.ViewModels;
using SofiaPublicTransport.Utils;


// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace SofiaPublicTransport
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public static event EventHandler ShowUserInputPopup;

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.DataContext = new ViewModelHubPage();
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>


        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
     
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        bool singleTap;

        private async void OnListBoxTapped(object sender, TappedRoutedEventArgs e)
        {
            this.singleTap = true;
            await Task.Delay(200);
            if (this.singleTap)
            {
                var listBox = sender as ListBox;
                var selectedItem = listBox.SelectedItem as StationDataModel;
                //var eventArguments = new EventArgs();
                //eventArguments.
                try
                {
                    ShowUserInputPopup(selectedItem.Code, new EventArgs());
                }
                catch (Exception)
                {
                }
                var debug = 2;
            }
        }

        private async void OkBtnClick(IUICommand command)
        {
            try
            {
                await SQLiteRequester.Instance.DeleteFavouriteStationAsync(command.Id.ToString());
                MessageDialog msgDialog = new MessageDialog("Спирката вече не е в любими", "Успешно изтриване");
                msgDialog.Commands.Add(new UICommand("ОК"));
                msgDialog.ShowAsync();
            }
            catch(Exception e)
            {
                MessageDialog msgDialog = new MessageDialog("За съжаление не успяхме да изтрием тази спирка", "Неуспешно изтриване");
                msgDialog.Commands.Add(new UICommand("Ще го преживея"));
                msgDialog.ShowAsync();
            }
        }

        private void OnListBoxDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.singleTap = false;
            var listBox = sender as ListBox;
            var selectedItem = listBox.SelectedItem as StationDataModel;

            MessageDialog msgDialog = new MessageDialog("Искате ли да изтриете тази спирка от любими", "Сигурни ли сте");

            //OK Button
            UICommand okBtn = new UICommand("Да");
            okBtn.Id = int.Parse(selectedItem.Code);
            okBtn.Invoked = OkBtnClick;
            msgDialog.Commands.Add(okBtn);

            //Cancel Button
            msgDialog.Commands.Add(new UICommand("Не"));

            //Show message
            msgDialog.ShowAsync();
        }
    }
}
