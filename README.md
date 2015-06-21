### FakeReverseProxyHost
An OWIN host that can host one or more OWIN applications and that simulates reverse proxy behaviour by changing request URL and adding headers. Intention is for testing load balanced and partioning scenarios. Currently only supports round-robin.

Mostly a sunday afternoon hack; not available as nuget package. If I or others find it useful, I may do that. [Ping me if you do](https://twitter.com/randompunter).

#### Using

See example project.


TODOs:

 - Support [rfc7239](https://tools.ietf.org/html/rfc7239)
 - Allow modifications of headers (and passthrough of HOST). See [nginx docs](http://nginx.com/resources/admin-guide/reverse-proxy/) as example.
