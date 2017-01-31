namespace CustomRenderer.Droid
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;

    /// <summary>
    /// Android Sample main activity
    /// </summary>
    [Activity(Label = "CustomRenderer.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        /// <summary>
        /// Activity OnCreate
        /// </summary>
        /// <param name="bundle">the bundle is provide by Android</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            this.LoadApplication(new App());
        }
    }
}
