using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanner
{
    public class FlightRoute
    {
        public string FlightDeparture { get; set; }
        public string FlightArrival { get; set; }
        public DateTime FlightDate { get; set; }
        public string FlightTime { get; set; }
        public int FlightHour { get; set; }
        public int FlightMinute { get; set; }
        public double FlightCost { get; set; }
        public TimeSpan LayoverToNextFlight { get; set; }
    }
}
