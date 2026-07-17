using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Supplies one <see cref="ITestEnvironment{TSut,TGateway}"/> per adapter configured for
/// <c>typeof(TSut).FullName</c> in <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per
/// configured adapter.
/// </summary>
public class TestEnvironmentsAttribute<TSut, TGateway> : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<string> labels = TestsConfig.GetEnvironmentLabels(typeof(TSut).FullName);

		foreach (string label in labels)
		{
			ITestEnvironment<TSut, TGateway> environment = TestEnvironmentFactory.Create<TSut, TGateway>(label);

			yield return [environment];
		}
	}
}
