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
        private ScheduleDataModel scheduleData;
        private ICommand checkCommand;
        private ICommand hidePopupCommand;
        private ICommand addToFavouritesCommand;
        private string schedulesToString;

        public ViewModelPopupForUserInput()
            : this("", "", "", Visibility.Collapsed)
        {
        }

        public ViewModelPopupForUserInput(string captchaURL, string captchaText, string stationCode, Visibility captchaVisibility)
        {
            this.SetCaptchaURLPropertyAsync();
            this.CaptchaURL = captchaURL;
            this.CaptchaText = captchaText;
            this.StationCode = "1903";
            this.CaptchaVisibility = captchaVisibility;
            this.FavouritesButtonVisibility = Visibility.Collapsed;
            this.stationName = "";
            this.SchedulesToString = "";
            this.IsPopupForSchedulesOpen = false;
            this.IsPopupForUserInputOpen = false;
            this.ScheduleData = new ScheduleDataModel();
            CommandBarView.ShowUserInputPopup += this.ShowUserInputPopupNow;
            HubPage.ShowUserInputPopup += this.ShowUserInputPopupNow;
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

        public string SchedulesToString
        {
            get
            {
                return this.schedulesToString;
            }
            set
            {
                this.schedulesToString = value;
                this.OnPropertyChanged("SchedulesToString");
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
        public ScheduleDataModel ScheduleData
        {
            get
            {
                return this.scheduleData;
            }
            set
            {
                this.scheduleData = value;
                this.OnPropertyChanged("ScheduleData");
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
                this.FavouritesButtonVisibility = Visibility.Collapsed;
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
            else
            {
                this.CheckSchedulesAsync((string)sender);
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
            try
            {
                await this.CheckSchedulesAsync(this.stationCode);
            }
            catch(Exception e)
            {

            }
        }

        private async Task CheckSchedulesAsync(string code)
        {
            int checkIfCodeIsValid;
            if (!int.TryParse(code, out checkIfCodeIsValid))
            {
                MessageDialog msgDialog = new MessageDialog(String.Format("Кода на спирката се състои само от цифри!", this.stationName), "Невалиден код на спирка");
                msgDialog.Commands.Add(new UICommand("OK"));
                msgDialog.ShowAsync();
                return;
            }

            try
            {
                EntireScheduleDataModel schedules = await HttpRequester.Instance.GetSchedulesForStationAsync(code, this.CaptchaText);
                this.stationName = schedules.StationName;
                this.SchedulesToString = schedules.ToString();
                bool isStationInFavourites = true;
                if(!string.IsNullOrEmpty(this.stationName))
                {
                    isStationInFavourites = await SQLiteRequester.Instance.IsStationInFavouritesAsync(code);
                }
                this.FavouritesButtonVisibility = isStationInFavourites ? Visibility.Collapsed : Visibility.Visible;
                this.IsPopupForSchedulesOpen = true;
                this.IsPopupForUserInputOpen = false;
            }
            catch(Exception ex)
            {
                MessageDialog msgDialog =
                    new MessageDialog(String.Format(StringResources.OnUnhandledExceptionUserMessage,
                        this.stationName),
                    "Възникна проблем");
                msgDialog.Commands.Add(new UICommand("Не е яко!"));
                msgDialog.ShowAsync();
            }

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
