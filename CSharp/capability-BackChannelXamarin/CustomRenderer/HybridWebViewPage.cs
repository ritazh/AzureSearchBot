namespace CustomRenderer
{
    using System;
    using System.Threading.Tasks;
    using Commands;
    using Xamarin.Forms;

    public class HybridWebViewPage : ContentPage
    {
        public HybridWebViewPage(string content)
        {
            var hybridWebView = new HybridWebView<BaseCommand>
            {
                Uri = content,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            this.Padding = new Thickness(0, 20, 0, 0);
            this.Content = hybridWebView;
        }

        public void RegisterHandle(Func<string, Task<BaseCommand>> handle)
        {
            (this.Content as HybridWebView<BaseCommand>).RegisterHandle(handle);
        }
    }
}