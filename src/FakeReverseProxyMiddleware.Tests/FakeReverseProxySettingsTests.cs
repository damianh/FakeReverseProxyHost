namespace FakeReverseProxyMiddleware
{
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Xunit;

    public class FakeReverseProxySettingsTests
    {
        [Fact]
        public void Verify_guards()
        {
            var guardClauseAssertion = new GuardClauseAssertion(new Fixture());
            guardClauseAssertion.Verify(typeof(FakeReverseProxySettings));
        }
    }
}
