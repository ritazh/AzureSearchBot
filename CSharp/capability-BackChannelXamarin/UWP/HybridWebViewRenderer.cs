[assembly: Xamarin.Forms.Platform.UWP.ExportRenderer(typeof(CustomRenderer.HybridWebView<CustomRenderer.Commands.BaseCommand>), typeof(CustomRenderer.UWP.HybridWebViewRenderer))]

namespace CustomRenderer.UWP
{
    using System;
    using Commands;
    using CustomRenderer;
    using Windows.UI.Xaml.Controls;
    using Xamarin.Forms.Platform.UWP;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView<BaseCommand>, Windows.UI.Xaml.Controls.WebView>
    {
        private const string JavaScriptFunction = "function invokeCSharpAction(data){ window.external.notify(data) }";

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

        private async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Inject JS script
                await Control.InvokeScriptAsync("eval", new[] { JavaScriptFunction });
            }
        }

        private async void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            var cmd = await Element.InvokeHandle(e.Value);

            if (cmd != null)
            {
                if (cmd is InvokeScriptCommand && !((InvokeScriptCommand)cmd).Handled)
                {
                    await Control.InvokeScriptAsync("eval", new[] { ((InvokeScriptCommand)cmd).JavaScript });
                    ((InvokeScriptCommand)cmd).Handled = true;
                }
            }
        }
    }
}