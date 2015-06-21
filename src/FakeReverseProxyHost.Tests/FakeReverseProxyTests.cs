namespace FakeReverseProxyHost
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin;
    using Xunit;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class FakeReverseProxyTests : IDisposable
    {
        private readonly HttpClient _client;
        private OwinContext _sut;

        public FakeReverseProxyTests()
        {
            _sut = null;
            AppFunc proxiedAppFunc = async env =>
            {
                var context = new OwinContext(env);
                _sut = context;
                context.Response.StatusCode = 200;
                context.Response.ReasonPhrase = "OK";
                await context.Response.WriteAsync("Hello");
            };
            var settings = new FakeReverseProxySettings();
            settings
                .Forward("/some/path/")
                .To(proxiedAppFunc, new Uri("http://internal1.example.com:8080/link/"));

            var handler = new OwinHttpMessageHandler(new FakeReverseProxy(settings).AppFunc)
            {
                AllowAutoRedirect = false,
                UseCookies = true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.com")
            };
        }

        [Fact]
        public async Task Should_change_request_host()
        {
            var response = await _client.GetAsync("/some/path/page.html?q=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _sut.Request.Uri.ToString().Should().Be("http://internal1.example.com:8080/link/page.html?q=1");
        }

        [Fact]
        public async Task Should_have_x_forwarded_proto_header() //TODO Forwarded header
        {
            var response = await _client.GetAsync("/some/path/page.html?q=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _sut.Request.Headers["X-Forwarded-Proto"].Should().Be("https");
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
