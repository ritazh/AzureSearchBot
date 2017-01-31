namespace CustomRenderer
{
    using Commands;
    using Interfaces;
    using Models;
    using Xamarin.Forms;

    public class App : Application
    {
        public App()
        {
            var webViewPage = new HybridWebViewPage("index.html");
            webViewPage.RegisterHandle(async data =>
            {
                var gpsProvider = DependencyService.Get<IGpsDataProvider>();

                var position = await gpsProvider.GetNativeGpsData();

                return new InvokeScriptCommand
                {
                    JavaScript = "gpsCallback({ lat: " + position.Latitude + ", lon: " + position.Longitude + " })"
                };
            });

            this.MainPage = webViewPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}