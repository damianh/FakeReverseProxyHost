namespace FakeReverseProxyHost
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FakeReverseProxyHost.LibOwin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class ForwardEntry
    {
        private readonly string _location;
        private volatile int _index;
        private readonly List<Tuple<AppFunc, Uri>> _tuples = new List<Tuple<AppFunc, Uri>>();
        
        internal ForwardEntry(string location)
        {
            _location = location;
        }

        public ForwardEntry To(Func<IDictionary<string, object>, Task> appFunc, Uri baseUri)
        {
            _tuples.Add(Tuple.Create(appFunc, baseUri));
            return this;
        }

        public Uri GetUri(string path)
        {
            path = path.Substring(_location.Length);
            var tuple = GetNextRoudRobin();
            var uri = new Uri(tuple.Item2, path);
            return uri;
        }

        private Tuple<AppFunc, Uri> GetNextRoudRobin()
        {
            var tuple = _tuples[_index];
            _index++;
            if (_index > _tuples.Count)
            {
                _index = 0;
            }
            return tuple;
        }

        internal Task Forward(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);    
            var originalUri = context.Request.Uri;
            var path = context.Request.Uri.AbsolutePath.Substring(_location.Length);
            var tuple = GetNextRoudRobin();
            var uri = new Uri(tuple.Item2, path);
            env[OwinConstants.RequestPath] = uri.AbsolutePath;
            env[OwinConstants.RequestPathBase] = string.Empty;
            env[OwinConstants.RequestScheme] = uri.Scheme;
            context.Request.Headers["Host"] = uri.Authority;
            context.Request.Headers["X-Forwarded-Proto"] = originalUri.Scheme;

            return tuple.Item1(env);
        }
    }
}