namespace FakeReverseProxyMiddleware
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class FakeReverseProxySettingsTests
    {
        [Theory]
        [InlineData("/", "http://internal.example.com/link/some/path/page.html")]
        [InlineData("/some/", "http://internal.example.com/link/path/page.html")]
        [InlineData("/some/path/", "http://internal.example.com/link/page.html")]
        public void Should_find_match(string location, string expectedProxiedUrl)
        {
            var settings = new FakeReverseProxySettings();

            settings.Forward(location, new Uri("http://internal.example.com/link/"), env => Task.FromResult(0));

            var forwardEntry = settings.FindForwardEntry("/some/path/page.html");
            var url = forwardEntry.GetUrl("/some/path/page.html");

            forwardEntry.Should().NotBeNull();
            url.ToString().Should().Be(expectedProxiedUrl);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/some/")]
        public void Should_not_find_match(string location)
        {
            var settings = new FakeReverseProxySettings();

            settings.Forward("/some/path", new Uri("http://internal.example.com/link/"), env => Task.FromResult(0));

            var forwardEntry = settings.FindForwardEntry(location);

            forwardEntry.Should().BeNull();
        }
    }
}
