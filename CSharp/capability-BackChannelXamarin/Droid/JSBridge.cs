namespace CustomRenderer.Droid
{
    using System;
    using Android.Webkit;
    using Commands;
    using Java.Interop;

    /// <summary>
    /// Android WebKit JSBridge
    /// </summary>
    public class JSBridge : Java.Lang.Object
    {
        private readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            this.hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public async void InvokeAction(string data)
        {
            HybridWebViewRenderer hybridRenderer;

            if (this.hybridWebViewRenderer != null && this.hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                var cmd = await hybridRenderer.Element.InvokeHandle(data);

                if (cmd != null)
                {
                    if (cmd is InvokeScriptCommand && !((InvokeScriptCommand)cmd).Handled)
                    {
                        hybridRenderer.EvalJs(((InvokeScriptCommand)cmd).JavaScript);
                        ((InvokeScriptCommand)cmd).Handled = true;
                    }
                }
            }
        }
    }
}