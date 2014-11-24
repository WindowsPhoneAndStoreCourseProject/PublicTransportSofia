using System;
using System.Windows.Input;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Popups;

using SofiaPublicTransport.Utils;
using SofiaPublicTransport.Commands;
using SofiaPublicTransport.DataModel;
using SofiaPublicTransport.Views;

namespace SofiaPublicTransport.ViewModels
{
    public class ViewModelPopupForUserInput : ViewModelBase
    {
        private string captchaURL;
        private string captchaText;
        private string stationCode;
        private Visibility captchaVisibility;
        private Visibility favouritesButtonVisibility;
        private bool isPopupForSchedulesOpen;
        private bool isPopupForUserInputOpen;
        private string stationName;
        private ICommand checkCommand;
        private ICommand hidePopupCommand;
        private ICommand addToFavouritesCommand;
       // private 

        public ViewModelPopupForUserInput()
            : this("", "", "", Visibility.Collapsed)
        {
        }

        public ViewModelPopupForUserInput(string captchaURL, string captchaText, string stationCode, Visibility captchaVisibility)
        {
            this.SetCaptchaURLPropertyAsync();
            this.CaptchaURL = captchaURL;
            this.CaptchaText = captchaText;
            this.StationCode = "2326";//stationCode;
            this.CaptchaVisibility = captchaVisibility;
            this.FavouritesButtonVisibility = Visibility.Collapsed;
            this.stationName = "";
            this.IsPopupForSchedulesOpen = false;
            this.IsPopupForUserInputOpen = false;
            CommandBarView.ShowUserInputPopup += this.ShowUserInputPopupNow;
        }

        public string CaptchaURL
        {
            get
            {
                return this.captchaURL;
            }
            set
            {
                this.captchaURL = value;
                this.OnPropertyChanged("CaptchaURL");
            }
        }

        public string CaptchaText
        {
            get
            {
                return this.captchaText;
            }
            set
            {
                this.captchaText = value;
                this.OnPropertyChanged("CaptchaText");
            }
        }

        public string StationCode
        {
            get
            {
                return this.stationCode;
            }
            set
            {
                this.stationCode = value;
                this.OnPropertyChanged("StationCode");
            }
        }

        public Visibility CaptchaVisibility
        {
            get
            {
                return this.captchaVisibility;
            }
            set
            {
                this.captchaVisibility = value;
                this.OnPropertyChanged("CaptchaVisibility");
            }
        }

        public Visibility FavouritesButtonVisibility
        {
            get
            {
                return this.favouritesButtonVisibility;
            }
            set
            {
                this.favouritesButtonVisibility = value;
                this.OnPropertyChanged("FavouritesButtonVisibility");
            }
        }

        public bool IsPopupForSchedulesOpen
        {
            get
            {
                return this.isPopupForSchedulesOpen;
            }
            set
            {
                this.isPopupForSchedulesOpen = value;
                this.OnPropertyChanged("IsPopupForSchedulesOpen");
            }
        }

        public bool IsPopupForUserInputOpen
        {
            get
            {
                return this.isPopupForUserInputOpen;
            }
            set
            {
                this.isPopupForUserInputOpen = value;
                this.OnPropertyChanged("IsPopupForUserInputOpen");
            }
        }

        public ICommand Check
        {
            get
            {
                if (this.checkCommand == null)
                {
                    this.checkCommand = new DelegateCommand(this.CheckSchedulesAsync);
                }

                return this.checkCommand;
            }
        }

        public ICommand HidePopup
        {
            get
            {
                if (this.hidePopupCommand == null)
                {
                    this.hidePopupCommand = new DelegateCommand(this.HidePopups);
                }

                return this.hidePopupCommand;
            }
        }

        public ICommand AddToFavourites
        {
            get
            {
                if (this.addToFavouritesCommand == null)
                {
                    this.addToFavouritesCommand = new DelegateCommand(this.AddStationToFavourites);
                }

                return this.addToFavouritesCommand;
            }
        }

        private async void AddStationToFavourites()
        {
            var stationForFavourites = new StationDataModel()
            {
                Code = this.StationCode,
                Name = this.stationName,
                Lat = 0,
                Lon = 0
            };

            try
            {
                await SQLiteRequester.Instance.AddFavouriteStationAsync(stationForFavourites);
                MessageDialog msgDialog = new MessageDialog(String.Format("Спирка '{0}' беше успешно добавена към любимите Ви спирки.", this.stationName), "Успешно добавяне");
                msgDialog.Commands.Add(new UICommand("OK"));
                msgDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                MessageDialog msgDialog = new MessageDialog(String.Format("Съжаляваме, но спирка '{0}' не беше добавена към любимите Ви спирки", this.stationName), "Неуспешно добавяне");
                msgDialog.Commands.Add(new UICommand("OK"));
                msgDialog.ShowAsync();
            }
         }

        private void ShowUserInputPopupNow(object sender, EventArgs e)
        {
            if ((string)sender == "CommandBarView")
            {
                this.IsPopupForUserInputOpen = true;
            }
        }

        private void HidePopups()
        {
            var d = 1;
            this.IsPopupForUserInputOpen = false;
            this.IsPopupForSchedulesOpen = false;
            var deb = 2;
        }

        private async void CheckSchedulesAsync()
        {
            ScheduleDataModel schedules = await HttpRequester.Instance.GetSchedulesForStationAsync(this.StationCode, this.CaptchaText);
       //     var viewModelSchedule = new ViewModelSchedule(schedules);
      //      viewModelSchedule.Test = "How about now";
          //  var scheduleView = new SchedulePopupView(viewModelSchedule);
       //     SchedulePopupView.ScheduleDataContext = viewModelSchedule;
       //     SchedulePopupView.PopupSchedule.Visibility = Visibility.Visible;
      //      SchedulePopupView.PopupSchedule.IsOpen = true;
          //  scheduleView.popup
            //scheduleView.Visibility = Visibility.Visible;
         //   this.IsPopupForUserInputOpen = true;
            this.stationName = schedules.StationName;
            bool isStationInFavourites = await SQLiteRequester.Instance.IsStationInFavouritesAsync(this.StationCode);
            this.favouritesButtonVisibility = isStationInFavourites ? Visibility.Collapsed : Visibility.Visible;
            this.IsPopupForSchedulesOpen = true;
            this.IsPopupForUserInputOpen = false;
            await SetCaptchaURLPropertyAsync();
            var debug = 1;
        }

        private async Task SetCaptchaURLPropertyAsync()
        {
            this.CaptchaURL = await HttpRequester.Instance.GetCaptchaURLAsync();
           // this.IsPopupForUserInputOpen = true;
            if (string.IsNullOrEmpty(this.CaptchaURL))
            {
                this.CaptchaVisibility = Visibility.Collapsed;
            }
            else
            {
                this.CaptchaVisibility = Visibility.Visible;
            }
        }
    }
}
