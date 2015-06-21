namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class ReverseProxyItem
    {
        internal readonly Uri Remote;
        internal readonly Func<IDictionary<string, object>, Task> AppFunc;

        public ReverseProxyItem(Uri remote, Func<IDictionary<string, object>, Task> appFunc)
        {
            Remote = remote;
            AppFunc = appFunc;
        }
    }
}