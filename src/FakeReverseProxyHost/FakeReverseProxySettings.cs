namespace FakeReverseProxyHost
{
    using System;
    using System.Collections.Generic;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class FakeReverseProxySettings
    {
        private readonly Node _root = new Node();

        public ForwardEntry Forward(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentNullException("location");
            }
            if (!location.StartsWith("/"))
            {
                throw new ArgumentException("locations should begine with a forward slash '/'", "location");
            }

            var node = _root;
            for (var i = 0; i < location.Length; i++)
            {
                var c = location[i];
                node = node.GetOrAddNode(c);
            }
            node.ForwardEntry = new ForwardEntry(location);
            return node.ForwardEntry;
        }

        public ForwardEntry FindForwardEntry(string location)
        {
            var node = _root;
            for (int i = 0; i < location.Length; i++)
            {
                Node child;
                if (!node.TryGetNode(location[i], out child))
                {
                    return node.ForwardEntry;
                }
                node = child;
            }
            return node.ForwardEntry;
        }

        private class Node
        {
            private readonly Dictionary<char, Node> _children = new Dictionary<char, Node>();

            internal ForwardEntry ForwardEntry { get; set; }

            internal Node GetOrAddNode(char c)
            {
                if (_children.ContainsKey(c))
                {
                    return _children[c];
                }
                var node = new Node();
                _children.Add(c, node);
                return node;
            }

            public bool TryGetNode(char c, out Node node)
            {
                return _children.TryGetValue(c, out node);
            }
        }

    }
}