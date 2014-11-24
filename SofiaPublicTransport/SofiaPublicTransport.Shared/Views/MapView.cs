using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using System.Collections.Generic;
using Windows.UI;

#if WINDOWS_PHONE_APP
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
#elif WINDOWS_APP
using Bing.Maps;
#endif

namespace SofiaPublicTransport.Views
{
    public class MapView : Grid, INotifyPropertyChanged
    {
#if WINDOWS_APP
        private Map _map;
#elif WINDOWS_PHONE_APP
        private MapControl _map;
#endif

        public MapView()
        {
#if WINDOWS_APP
            _map = new Map();
#elif WINDOWS_PHONE_APP
            _map = new MapControl();
#endif

            this.Children.Add(_map);
        }

        public double Zoom
        {
            get
            {
                return _map.ZoomLevel;
            }
            set
            {
                _map.ZoomLevel = value;
                OnPropertyChanged("Zoom");
            }
        }

        public Geopoint Center
        {
            get
            {
#if WINDOWS_APP
                return _map.Center.ToGeopoint();
#elif WINDOWS_PHONE_APP
                return _map.Center;
#endif
            }
            set
            {
#if WINDOWS_APP
                _map.Center = value.ToLocation();
#elif WINDOWS_PHONE_APP
                _map.Center = value;
#endif

                OnPropertyChanged("Center");
            }
        }

        public string Credentials
        {
            get
            {
#if WINDOWS_APP
                return _map.Credentials;
#elif WINDOWS_PHONE_APP 
                return string.Empty; 
#endif
            }
            set
            {
#if WINDOWS_APP
                if (!string.IsNullOrEmpty(value))
                {
                    _map.Credentials = value;
                }
#endif

                OnPropertyChanged("Credentials");
            }
        }

//        public string MapServiceToken
//        {
//            get
//            {
//#if WINDOWS_APP
//                return string.Empty;
//#elif WINDOWS_PHONE_APP 
//                return _map.MapServiceToken; 
//#endif
//            }
//            set
//            {
//#if WINDOWS_PHONE_APP 
//                if (!string.IsNullOrEmpty(value)) 
//                { 
//                    _map.MapServiceToken = value; 
//                } 
//#endif

//                OnPropertyChanged("MapServiceToken");
//            }
//        }

        public bool ShowTraffic
        {
            get
            {
#if WINDOWS_APP
                return _map.ShowTraffic;
#elif WINDOWS_PHONE_APP
                return _map.TrafficFlowVisible;
#endif
            }
            set
            {
#if WINDOWS_APP
                _map.ShowTraffic = value;
#elif WINDOWS_PHONE_APP
                _map.TrafficFlowVisible = value;
#endif

                OnPropertyChanged("ShowTraffic");
            }
        }

        public void SetView(BasicGeoposition center, double zoom)
        {
#if WINDOWS_APP
            _map.SetView(center.ToLocation(), zoom);
            OnPropertyChanged("Center");
            OnPropertyChanged("Zoom");
#elif WINDOWS_PHONE_APP
            _map.Center = new Geopoint(center);
            _map.ZoomLevel = zoom;
#endif
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}