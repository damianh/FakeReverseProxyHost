
namespace FakeReverseProxy.Example
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FakeReverseProxyHost;
    using FluentAssertions;
    using Microsoft.Owin.Builder;
    using Owin;
    using Xunit;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class ExamplesRoundRobinTests
    {
        
        [Fact]
        public async Task ExampleRoundRobin()
        {
            AppFunc app1 = CreateApplication("instance1");
            AppFunc app2 = CreateApplication("instance2");

            var settings = new FakeReverseProxySettings();

            // Here we're round-robining a path to two seperate apps
            settings.Forward("/path/")
                .To(app1, new Uri("http://backend1.example.com/pathX/"))
                .To(app2, new Uri("http://backend2.example.com/pathY/"));

            var reverseProxy = new FakeReverseProxy(settings);
            var handler = new OwinHttpMessageHandler(reverseProxy.AppFunc)
            {
                AllowAutoRedirect = true,
                UseCookies = true
            };
            using (var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://example.com/")
            })
            {
                // Should hit app1
                var response = await client.GetAsync("/path/");

                var body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance1");

                // Should hit app2
                response = await client.GetAsync("/path/");

                body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance2");
            }
        }

        private static AppFunc CreateApplication(string instance)
        {
            var app = new AppBuilder();
            app.Run(ctx => ctx.Response.WriteAsync(instance));
            return app.Build();
        }
    }
}
