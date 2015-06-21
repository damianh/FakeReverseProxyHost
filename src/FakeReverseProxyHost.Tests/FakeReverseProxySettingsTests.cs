namespace FakeReverseProxyHost
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

            settings
                .Forward(location)
                .To(env => Task.FromResult(0), new Uri("http://internal.example.com/link/"));

            var forwardEntry = settings.FindForwardEntry("/some/path/page.html");
            var url = forwardEntry.GetUri("/some/path/page.html");

            forwardEntry.Should().NotBeNull();
            url.ToString().Should().Be(expectedProxiedUrl);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/some/")]
        public void Should_not_find_match(string location)
        {
            var settings = new FakeReverseProxySettings();

            settings
                .Forward("/some/path")
                .To(env => Task.FromResult(0), new Uri("http://internal.example.com/link/"));

            var forwardEntry = settings.FindForwardEntry(location);

            forwardEntry.Should().BeNull();
        }
    }
}
