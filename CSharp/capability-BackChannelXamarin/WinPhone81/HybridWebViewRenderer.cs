[assembly: Xamarin.Forms.Platform.WinRT.ExportRenderer(typeof(CustomRenderer.HybridWebView<CustomRenderer.Commands.BaseCommand>), typeof(CustomRenderer.WinPhone81.HybridWebViewRenderer))]

namespace CustomRenderer.WinPhone81
{
    using System;
    using Commands;
    using CustomRenderer;
    using Windows.UI.Xaml.Controls;
    using Xamarin.Forms.Platform.WinRT;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView<BaseCommand>, Windows.UI.Xaml.Controls.WebView>
    {
        private const string JavaScriptFunction = "function invokeCSharpAction(data){window.external.notify(data);}";

        public async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Inject JS script
                await this.Control.InvokeScriptAsync("eval", new[] { JavaScriptFunction });
            }
        }

        public async void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            var cmd = await Element.InvokeHandle(e.Value);

            if (cmd != null)
            {
                if (cmd is InvokeScriptCommand && !((InvokeScriptCommand)cmd).Handled)
                {
                    await this.Control.InvokeScriptAsync("eval", new[] { ((InvokeScriptCommand)cmd).JavaScript });
                    ((InvokeScriptCommand)cmd).Handled = true;
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView<BaseCommand>> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null)
            {
                this.SetNativeControl(new Windows.UI.Xaml.Controls.WebView());
            }

            if (e.OldElement != null)
            {
                this.Control.NavigationCompleted -= this.OnWebViewNavigationCompleted;
                this.Control.ScriptNotify -= this.OnWebViewScriptNotify;
            }

            if (e.NewElement != null)
            {
                this.Control.NavigationCompleted += this.OnWebViewNavigationCompleted;
                this.Control.ScriptNotify += this.OnWebViewScriptNotify;
                this.Control.Source = new Uri(string.Format("ms-appx-web:///Content//{0}", Element.Uri));
            }
        }
    }
}