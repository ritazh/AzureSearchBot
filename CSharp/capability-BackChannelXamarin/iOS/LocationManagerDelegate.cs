namespace CustomRenderer.iOS
{
    using CoreLocation;
    using Foundation;

    public class LocationManagerDelegate : CLLocationManagerDelegate
    {
        private AppDelegate app;

        public LocationManagerDelegate(AppDelegate appDelegate) : base()
        {
            this.app = appDelegate;
        }

        public override void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            // check that locations are being received
            if (locations.Length > 0)
            {
                if (locations[0] != null)
                {
                    // store current location in custom CLLocation object accessible app-wide
                    this.app.CurrentLocation = locations[0];

                    // now broadcast location change to any observers
                    NSNotificationCenter.DefaultCenter.PostNotificationName("LocationUpdated", this);
                }
            }
        }
    }
}