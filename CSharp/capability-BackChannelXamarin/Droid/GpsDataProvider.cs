[assembly: Xamarin.Forms.Dependency(typeof(CustomRenderer.Droid.GpsDataProvider))]

namespace CustomRenderer.Droid
{
    using System.Threading.Tasks;
    using Android.Content;
    using Android.Locations;
    using CustomRenderer.Interfaces;
    using CustomRenderer.Models;    
    using Xamarin.Forms;
        
    /// <summary>
    /// Gps data provider for Android
    /// </summary>
    public class GpsDataProvider : IGpsDataProvider
    {
        public Task<Position> GetNativeGpsData()
        {
            var locationManager = (LocationManager)Forms.Context?.GetSystemService(Context.LocationService);

            // Tries to get the fix from the GSP satellites
            Location location = locationManager?.GetLastKnownLocation(LocationManager.GpsProvider);

            // If no GPS provider available, tries to get the fix from the cell towers/wifi pots.
            if (location == null)
            {
                location = locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
            }

            if (location != null)
            {
                return Task.FromResult(new Position { Latitude = (float)location.Latitude, Longitude = (float)location.Longitude });
            }

            return Task.FromResult(new Position { Latitude = 0, Longitude = 0 });
        }        
    }
}