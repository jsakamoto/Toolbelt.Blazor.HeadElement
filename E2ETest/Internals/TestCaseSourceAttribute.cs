global using TestCaseSource = HeadElement.E2ETest.Internals.TestCaseSourceAttribute;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace HeadElement.E2ETest.Internals;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class TestCaseSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
{
    private readonly NUnit.Framework.TestCaseSourceAttribute _TestCaseSourceAttribute;

    public string? Name { get; set; }

    /// <summary>
    /// Construct with the name of the method, property or field that will provide data
    /// </summary>
    /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
    public TestCaseSourceAttribute(string sourceName)
    {
        this._TestCaseSourceAttribute = new(sourceName);
    }

    /// <summary>
    /// Builds any number of tests from the specified method and context.
    /// </summary>
    /// <param name="method">The IMethod for which tests are to be constructed.</param>
    /// <param name="suite">The suite to which the tests will be added.</param>
    IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test? suite)
    {
        foreach (var testMethod in this._TestCaseSourceAttribute.BuildFrom(method, suite))
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                var argsString = string.Join(", ", testMethod.Arguments.Select(arg => arg?.ToString() ?? "null"));
                testMethod.Name = $"{this.Name} ({argsString})";
            }
            yield return testMethod;
        }
    }
}
