[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomRenderer.HybridWebView<CustomRenderer.Commands.BaseCommand>), typeof(CustomRenderer.iOS.HybridWebViewRenderer))]

namespace CustomRenderer.iOS
{
    using System.IO;
    using Commands;
    using Foundation;
    using WebKit;
    using Xamarin.Forms.Platform.iOS;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView<BaseCommand>, WKWebView>, IWKScriptMessageHandler
    {
        private const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        private WKUserContentController userController;

        public async void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            var cmd = await Element.InvokeHandle(message.Body.ToString());

            if (cmd != null)
            {
                if (cmd is InvokeScriptCommand && !((InvokeScriptCommand)cmd).Handled)
                {
                    await this.Control.EvaluateJavaScriptAsync(((InvokeScriptCommand)cmd).JavaScript);

                    ((InvokeScriptCommand)cmd).Handled = true;
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView<BaseCommand>> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null)
            {
                this.userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                this.userController.AddUserScript(script);
                this.userController.AddScriptMessageHandler(this, "invokeAction");

                var config = new WKWebViewConfiguration { UserContentController = this.userController };
                var webView = new WKWebView(Frame, config);
                this.SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                this.userController.RemoveAllUserScripts();
                this.userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as HybridWebView<BaseCommand>;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("Content/{0}", Element.Uri));
                Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
            }
        }
    }
}