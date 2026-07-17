using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Supplies one <see cref="ITestEnvironment{TSut,TBackDoor}"/> per adapter configured for
/// <c>typeof(TSut).FullName</c> in <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per
/// configured adapter.
/// </summary>
public class TestEnvironmentsAttribute<TSut, TBackDoor> : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<string> labels = TestsConfig.GetEnvironmentLabels(typeof(TSut).FullName);

		foreach (string label in labels)
		{
			ITestEnvironment<TSut, TBackDoor> environment = TestEnvironmentFactory.Create<TSut, TBackDoor>(label);

			yield return [environment];
		}
	}
}