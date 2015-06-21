namespace FakeReverseProxyHost
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ForwardEntry
    {
        public readonly string Location;
        internal readonly Uri BaseUri;
        internal readonly Func<IDictionary<string, object>, Task> AppFunc;

        internal ForwardEntry(string location, Uri baseUri, Func<IDictionary<string, object>, Task> appFunc)
        {
            Location = location;
            BaseUri = baseUri;
            AppFunc = appFunc;
        }

        public Uri GetUrl(string path)
        {
            path = path.Substring(Location.Length);

            var uri = new Uri(BaseUri, path);

            return uri;
        }
    }
}