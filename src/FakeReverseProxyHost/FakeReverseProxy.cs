namespace FakeReverseProxyHost
{
    using System;
    using FakeReverseProxyHost.App_Packages.LibOwin._1._0;
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
                var originalUri = context.Request.Uri;
                ForwardEntry forwardEntry = settings.FindForwardEntry(context.Request.Uri.AbsolutePath);
                if (forwardEntry != null)
                {
                    var url = forwardEntry.GetUrl(context.Request.Uri.AbsolutePath);
                    env[OwinConstants.RequestPath] = url.AbsolutePath;
                    env[OwinConstants.RequestPathBase] = string.Empty;
                    env[OwinConstants.RequestScheme] = forwardEntry.BaseUri.Scheme;
                    context.Request.Headers["Host"] = url.Authority;
                    context.Request.Headers["X-Forwarded-Proto"] = originalUri.Scheme;
                    await forwardEntry.AppFunc(env);
                    return;
                }

                context.Response.StatusCode = 500;
                context.Response.ReasonPhrase = "Internal Server Error";
                await context.Response.WriteAsync("Unknown host");
            };
        }
    }
}