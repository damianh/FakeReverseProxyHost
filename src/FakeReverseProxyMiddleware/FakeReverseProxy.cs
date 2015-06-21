namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using FakeReverseProxyMiddleware.LibOwin;

    public static class FakeReverseProxy
    {
        private const string HostHeaderKey = "Host";

        public static Func<IDictionary<string, object>, Task> CreateAppFunc(FakeReverseProxySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            return async env =>
            {
                var context = new OwinContext(env);
                var originalUri = context.Request.Uri;
                ForwardEntry forwardEntry = settings.FindForwardEntry(context.Request.Uri.AbsolutePath);
                if(forwardEntry != null)
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