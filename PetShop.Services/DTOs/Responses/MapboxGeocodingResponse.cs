using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Responses
{
    public class MapboxGeocodingResponse
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }

    public class Feature
    {
        public string id { get; set; }
        public string type { get; set; }
        public List<double> center { get; set; }
        public string place_name { get; set; }
    }
}
