
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

    public class ExampleTest
    {
        [Fact]
        public async Task ExamplePartion()
        {
            // Applications should be entirely seperate instances. If they are not, you are not 
            // performing a simulation.
            AppFunc app1 = CreateApplication("instance1");
            AppFunc app2 = CreateApplication("instance2");

            var settings = new FakeReverseProxySettings()
                .Forward("/path1/", new Uri("http://backend1.example.com/app1/"), app1)
                .Forward("/path2/", new Uri("http://backend2.example.com/app2/"), app2);

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
                // Should hit the app1
                var response = await client.GetAsync("/path1/");

                var body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance1");

                // Should hit the app2
                response = await client.GetAsync("/path1/");

                body = await response.Content.ReadAsStringAsync();
                body.Should().Be("instance1");
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
