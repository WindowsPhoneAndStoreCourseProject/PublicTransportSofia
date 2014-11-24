using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace SofiaPublicTransport.DataModel
{
    [Table("FavouriteStations")]
    public class StationDataModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Unique, MaxLength(6)]
        public string Code { get; set; }

        public string Name { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }
    }
}
