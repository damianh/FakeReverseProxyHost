
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

    public class Examples
    {
        [Fact]
        public async Task ExamplePartion()
        {
            // Applications should be entirely seperate instances. If they are not, you are not 
            // performing a simulation.
            AppFunc app1 = CreateApplication("instance1");
            AppFunc app2 = CreateApplication("instance2");

            var settings = new FakeReverseProxySettings();
            settings.Forward("/path1/").To(app1, new Uri("http://backend1.example.com/app1/"));
            settings.Forward("/path2/").To(app2, new Uri("http://backend2.example.com/app2/"));

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
                var response = await client.GetAsync("/path1/");

                var body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance1");

                // Should hit app2
                response = await client.GetAsync("/path2/");

                body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance2");
            }
        }

        [Fact]
        public async Task ExampleRoundRobin()
        {
            AppFunc app1 = CreateApplication("instance1");
            AppFunc app2 = CreateApplication("instance2");

            var settings = new FakeReverseProxySettings();
            settings.Forward("/")
                .To(app1, new Uri("http://backend1.example.com/"))
                .To(app2, new Uri("http://backend2.example.com/"));

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
                var response = await client.GetAsync("/");

                var body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance1");

                // Should hit app2
                response = await client.GetAsync("/");

                body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance2");
            }
        }

        public static AppFunc CreateApplication(string instance)
        {
            var app = new AppBuilder();
            app.Run(ctx => ctx.Response.WriteAsync(instance));
            return app.Build();
        }
    }
}
