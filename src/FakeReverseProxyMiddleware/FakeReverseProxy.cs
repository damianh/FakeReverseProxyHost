namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FakeReverseProxyMiddleware.LibOwin;

    public static class FakeReverseProxy
    {
        public static Func<IDictionary<string, object>, Task> CreateAppFunc(FakeReverseProxySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            return async env =>
            {
                var context = new OwinContext(env);

                var host = context.Request.Uri.Host;
                RemoteHost remoteHost;
                if(settings.RemoteHosts.TryGetValue(host, out remoteHost))
                {
                    context.Request.Host = new HostString(remoteHost.Host);
                    await remoteHost.AppFunc(env);
                    return;
                }

                context.Response.StatusCode = 500;
                context.Response.ReasonPhrase = "Internal Server Error";
                await context.Response.WriteAsync("Unknown host");
            };
        }
    }
}