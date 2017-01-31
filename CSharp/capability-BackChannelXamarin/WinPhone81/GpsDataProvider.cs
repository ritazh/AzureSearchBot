[assembly: Xamarin.Forms.Dependency(typeof(CustomRenderer.WinPhone81.GpsDataProvider))]

namespace CustomRenderer.WinPhone81
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using Models;

    public class GpsDataProvider : IGpsDataProvider
    {
        public async Task<Position> GetNativeGpsData()
        {
            var locator = new Windows.Devices.Geolocation.Geolocator();
            var position = await locator.GetGeopositionAsync();

            return new Position()
            {
                Latitude = position.Coordinate.Point.Position.Latitude,
                Longitude = position.Coordinate.Point.Position.Longitude
            };
        }
    }
}
