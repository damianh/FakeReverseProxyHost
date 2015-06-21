namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Collections.Generic;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class FakeReverseProxySettings
    {
        private readonly Dictionary<string, ReverseProxyItem> _locations = new Dictionary<string, ReverseProxyItem>();

        internal Dictionary<string, ReverseProxyItem> Locations
        {
            get { return _locations; }
        }

        public FakeReverseProxySettings AddEntry(string location, Uri remote, AppFunc appFunc)
        {
            if(string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentNullException("location");
            }
            if (remote == null)
            {
                throw new ArgumentNullException("remote");
            }
            if(appFunc == null)
            {
                throw new ArgumentNullException("appFunc");
            }
            _locations.Add(location, new ReverseProxyItem(remote, appFunc));
            return this;
        }
    }
}