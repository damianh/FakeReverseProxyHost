namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class FakeReverseProxy
    {
        public static Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>> CreateMiddleware(FakeReverseProxySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            return next => env =>
            {
                throw new NotImplementedException();
            };
        }
    }
}