namespace FakeReverseProxyHost
{
    using System;
    using FakeReverseProxyHost.LibOwin;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class FakeReverseProxy
    {
        public readonly AppFunc AppFunc;
        private const string HostHeaderKey = "Host";

        public FakeReverseProxy(FakeReverseProxySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            AppFunc = async env =>
            {
                var context = new OwinContext(env);
                ForwardEntry forwardEntry = settings.FindForwardEntry(context.Request.Uri.AbsolutePath);
                if (forwardEntry != null)
                {
                    await forwardEntry.Forward(env);
                    return;
                }

                context.Response.StatusCode = 500;
                context.Response.ReasonPhrase = "Internal Server Error";
                await context.Response.WriteAsync("Unknown host");
            };
        }
    }
}