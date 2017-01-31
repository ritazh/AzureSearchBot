namespace CustomRenderer
{
    using System;
    using System.Threading.Tasks;
    using Commands;
    using Xamarin.Forms;

    public class HybridWebView<Command> : View where Command : BaseCommand
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView<Command>),
            defaultValue: default(string));

        private Func<string, Task<Command>> handle;

        public string Uri
        {
            get { return (string)this.GetValue(UriProperty); }
            set { this.SetValue(UriProperty, value); }
        }

        public void RegisterHandle(Func<string, Task<Command>> handle)
        {
            this.handle = handle;
        }

        public void Cleanup()
        {
            this.handle = null;
        }

        public async Task<Command> InvokeHandle(string data)
        {
            if (this.handle == null || data == null)
            {
                return null;
            }

            var cmd = await this.handle.Invoke(data);

            // TODO: can handle command here?
            return cmd;
        }
    }
}