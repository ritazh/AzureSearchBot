[assembly: Xamarin.Forms.Dependency(typeof(CustomRenderer.iOS.GpsDataProvider))]

namespace CustomRenderer.iOS
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CoreLocation;
    using Foundation;
    using Interfaces;
    using Models;
    using UIKit;

    public class GpsDataProvider : IGpsDataProvider
    {
        private CLLocationManager locationManager;
        private LocationManagerDelegate locationDelegate = null;

        private NSObject observer;
        private SemaphoreSlim semaphore;
        private double latitude;
        private double longitude;

        public GpsDataProvider()
        {
            this.semaphore = new SemaphoreSlim(0);
        }

        public CLLocation CurrentLocation { get; set; }

        public async Task<Position> GetNativeGpsData()
        {
            // get reference to AppDelegate
            AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

            // start up location tracking
            this.locationManager = new CLLocationManager();
            this.locationDelegate = new LocationManagerDelegate(app);
            this.locationManager.Delegate = this.locationDelegate;
            this.locationManager.DesiredAccuracy = 1;

            // if using iOS8, extra step for authorisation
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                this.locationManager.RequestWhenInUseAuthorization();
            }

            this.locationManager.StartUpdatingLocation();

            // we will now ask NSNotification to receive broadcasts which use the indentifier 'LocationsUpdated'
            // once the 'observer' object receives a broadcast it will initiate a method, in this case 'UpdateLocationInformation'
            this.observer = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LocationUpdated", this.UpdateLocationInformation);
            
            // Wait until the location.
            await this.semaphore.WaitAsync();

            return new Position()
            {
                Latitude = this.latitude,
                Longitude = this.longitude
            };
        }

        public void UpdateLocationInformation(NSNotification notification)
        {
            // get reference to AppDelegate
            AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

            this.latitude = app.CurrentLocation.Coordinate.Latitude;
            this.longitude = app.CurrentLocation.Coordinate.Longitude;

            this.locationManager.StopUpdatingLocation();

            this.semaphore.Release();
        }
    }
}