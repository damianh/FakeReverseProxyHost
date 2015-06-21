namespace FakeReverseProxyMiddleware
{
    using System.Collections.Generic;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class FakeReverseProxySettings
    {
        private readonly Dictionary<string, RemoteHost> _remoteHosts = new Dictionary<string, RemoteHost>();

        internal Dictionary<string, RemoteHost> RemoteHosts
        {
            get { return _remoteHosts; }
        }

        public FakeReverseProxySettings AddRemoteHost(string host, string remoteHost, AppFunc appFunc)
        {
            _remoteHosts.Add(host, new RemoteHost(remoteHost, appFunc));
            return this;
        }
    }
}