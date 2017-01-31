[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomRenderer.HybridWebView<CustomRenderer.Commands.BaseCommand>), typeof(CustomRenderer.Droid.HybridWebViewRenderer))]

namespace CustomRenderer.Droid
{
    using Android.Webkit;
    using Commands;
    using CustomRenderer;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    /// <summary>
    /// Android specific WebView renderer
    /// </summary>
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView<BaseCommand>, Android.Webkit.WebView>, IValueCallback
    {
        private const string JavaScriptFunction = "function invokeCSharpAction(data){ jsBridge.invokeAction(data); }";

        public void EvalJs(string script)
        {
            if (this.Control != null)
            {
                this.Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }
         
        public void OnReceiveValue(Java.Lang.Object value)
        {
            // TODO: handle response if required
        }

        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView<BaseCommand>> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null)
            {
                var webView = new Android.Webkit.WebView(Forms.Context);
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.UseWideViewPort = true;
                this.SetNativeControl(webView);
                this.Control.Settings.DomStorageEnabled = true;
            }

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView<BaseCommand>;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadUrl(string.Format("file:///android_asset/Content/{0}", Element.Uri));
                this.InjectJS(JavaScriptFunction);
            }
        }

        protected void InjectJS(string script)
        {
            if (this.Control != null)
            {
                this.Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }        
    }
}