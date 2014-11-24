using Windows.Devices.Geolocation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if WINDOWS_APP
using Bing.Maps;
#endif

namespace SofiaPublicTransport
{
    public static class Extensions
    {
#if WINDOWS_APP

        public static LocationCollection ToLocationCollection(this IList<BasicGeoposition> pointList)
        {
            var locs = new LocationCollection();

            foreach (var p in pointList)
            {
                locs.Add(p.ToLocation());
            }

            return locs;
        }

        public static Geopoint ToGeopoint(this Location location)
        {
            return new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude });
        }

        public static Location ToLocation(this Geopoint location)
        {
            return new Location(location.Position.Latitude, location.Position.Longitude);
        }

        public static Location ToLocation(this BasicGeoposition location)
        {
            return new Location(location.Latitude, location.Longitude);
        }

#elif WINDOWS_PHONE_APP

        //Add any required Windows Phone Extensions

#endif

        public static void AddRange<T>(this ObservableCollection<T> collection,
           IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
