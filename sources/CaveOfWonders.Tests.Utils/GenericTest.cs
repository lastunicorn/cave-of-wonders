namespace DustInTheWind.CaveOfWonders.Tests.Utils;

public static class GenericTest
{
	public static GenericTest<TSut> Create<TSut>(ISutFixture<TSut> sutFixture)
	{
		return new GenericTest<TSut>(sutFixture);
	}

	public static GenericTest<TSut, TGateway> Create<TSut, TGateway>(ITestEnvironment<TSut, TGateway> environment)
	{
		return new GenericTest<TSut, TGateway>(environment);
	}
}