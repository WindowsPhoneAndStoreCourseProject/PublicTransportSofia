namespace SofiaPublicTransport.DataModel
{
    using System.Collections.Generic;

    public class ScheduleDataModel
    {
        public string VehicleType { get; set; }

        public List<ArrivalTimeDataModel> ArrivalTimes { get; set; }
    }
}
