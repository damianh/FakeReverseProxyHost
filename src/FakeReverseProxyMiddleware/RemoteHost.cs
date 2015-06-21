namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class RemoteHost
    {
        internal readonly string Host;
        internal readonly Func<IDictionary<string, object>, Task> AppFunc;

        public RemoteHost(string host, Func<IDictionary<string, object>, Task> appFunc)
        {
            Host = host;
            AppFunc = appFunc;
        }
    }
}