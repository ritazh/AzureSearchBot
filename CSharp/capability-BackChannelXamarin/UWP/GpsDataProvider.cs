[assembly: Xamarin.Forms.Dependency(typeof(CustomRenderer.UWP.GpsDataProvider))]

namespace CustomRenderer.UWP
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using Models;
    using Windows.Devices.Geolocation;

    public class GpsDataProvider : IGpsDataProvider
    {
        public async Task<Position> GetNativeGpsData()
        {
            Geolocator geolocator = new Geolocator();
            var position = await geolocator.GetGeopositionAsync(); 

            return new Position
            {
                Latitude = position.Coordinate.Point.Position.Latitude,
                Longitude = position.Coordinate.Point.Position.Longitude
            };     
        }
    }
}