using Xunit;
using Xunit.Abstractions;

namespace HeadElement.E2ETest
{
    [Collection(nameof(TestContext))]
    public class HeadElementServerPreRenderingTest
    {
        private readonly TestContext _TestContext;

        private readonly ITestOutputHelper _TestOutput;

        public HeadElementServerPreRenderingTest(TestContext testContext)
        {
            this._TestContext = testContext;
        }
    }
}
