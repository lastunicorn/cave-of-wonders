namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Tags an <see cref="ITestEnvironment{TSut,TGateway}"/> implementation with the label used to select it from
/// <c>tests-config.json</c>. <see cref="TestEnvironmentFactory"/> matches configured labels against this attribute
/// via reflection.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TestEnvironmentAttribute : Attribute
{
	public string Label { get; }

	public TestEnvironmentAttribute(string label)
	{
		Label = label;
	}
}
