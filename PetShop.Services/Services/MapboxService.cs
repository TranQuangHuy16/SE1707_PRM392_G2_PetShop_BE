using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class MapboxService : IMapboxService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public MapboxService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _accessToken = configuration["Mapbox:AccessToken"];
        }

        public async Task<CoordinatesAddressResponse> GetCoordinatesFromAddress(string address)
        {
            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{encodedAddress}.json?access_token={_accessToken}&limit=1";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadFromJsonAsync<MapboxGeocodingResponse>();
            if (json?.features == null || json.features.Count == 0)
                return null;

            var coordinates = json.features[0].center;
            return new CoordinatesAddressResponse
            {
                lon = coordinates[0],
                lat = coordinates[1]
            };
        }

        public async Task<MapboxRouteResponse?> GetRoute(RouteRequest req)
        {
            var url = $"https://api.mapbox.com/directions/v5/mapbox/driving/" +
                      $"{req.StartLng},{req.StartLat};{req.EndLng},{req.EndLat}" +
                      $"?geometries=geojson&overview=full&steps=true&access_token={_accessToken}";

            using var client = new HttpClient();

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Mapbox error: {content}");
            }

            return JsonConvert.DeserializeObject<MapboxRouteResponse>(content);
        }

    }
}
