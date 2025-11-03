using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Responses
{
    public class MapboxRouteResponse
    {
        public List<Route> routes { get; set; }
    }

    public class Route
    {
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<double>> coordinates { get; set; }
    }

}
