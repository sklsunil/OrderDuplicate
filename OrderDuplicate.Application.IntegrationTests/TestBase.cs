namespace OrderDuplicate.Application.IntegrationTests
{
    using static Testing;
    public class TestBase
    {              
        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();
        }
    }
}
