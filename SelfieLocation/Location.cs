using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfieLocation
{
    class Location
    {
        double latitude;
        public double Latitude{
            get{return this.latitude; }
            set{this.latitude = value; } }
        double longitude;
        public double Longitude
        {
            get { return this.longitude; }
            set { this.longitude = value; }
        }

        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

    }
}
