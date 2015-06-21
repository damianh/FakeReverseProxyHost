### FakeReverseProxyHost
An OWIN host that can host one or more OWIN applications to allow simulating of a reverse proxy. Intention is be able to acceptance test _application behavior_ in load balancing (round-robin) and partioning scenarios.

Mostly a sunday afternoon hack; not yet available as nuget package. If I or others find it useful, I may make one. [Ping me if you do](https://twitter.com/randompunter).

#### Using

See example project:

 - [Partioning](https://github.com/damianh/FakeReverseProxyHost/blob/master/src/FakeReverseProxy.Example/ExamplePartionTests.cs)
 - [Round Robbin](https://github.com/damianh/FakeReverseProxyHost/blob/master/src/FakeReverseProxy.Example/ExamplesRoundRobinTests.cs)

TODOs:

 - Support [rfc7239](https://tools.ietf.org/html/rfc7239)
 - Allow modifications of headers (and passthrough of HOST). See [nginx docs](http://nginx.com/resources/admin-guide/reverse-proxy/) as example.
