namespace CustomRenderer.iOS
{
    using CoreLocation;
    using Foundation;
    using UIKit;

    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public CLLocation CurrentLocation { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            this.LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}