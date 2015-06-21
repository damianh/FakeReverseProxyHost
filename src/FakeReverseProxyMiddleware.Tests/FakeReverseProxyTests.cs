namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin;
    using Xunit;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using MidFunc = System.Func<
      System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
      System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>;

    public class FakeReverseProxyTests : IDisposable
    {
        private readonly HttpClient _client;
        private OwinContext _sut;

        public FakeReverseProxyTests()
        {
            _sut = null;
            AppFunc exampleComAppFunc = async env =>
            {
                var context = new OwinContext(env);
                _sut = context;
                context.Response.StatusCode = 200;
                context.Response.ReasonPhrase = "OK";
                await context.Response.WriteAsync("Hello");
            };
            var settings = new FakeReverseProxySettings()
                .AddRemoteHost("example.com", "internal1.example.com", exampleComAppFunc);

            AppFunc appFunc = FakeReverseProxy.CreateAppFunc(settings);

            var handler = new OwinHttpMessageHandler(appFunc)
            {
                AllowAutoRedirect = false,
                UseCookies = true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://example.com")
            };
        }

        [Fact]
        public async Task Should_change_request_host()
        {
            await _client.GetAsync("");

            _sut.Request.Uri.Host.Should().Be("internal1.example.com");
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
