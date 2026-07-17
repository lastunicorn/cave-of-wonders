namespace DustInTheWind.CaveOfWonders.Tests.Utils;

public static class GenericTest
{
	public static GenericTest<TSut, TBackDoor> Create<TSut, TBackDoor>(ITestEnvironment<TSut, TBackDoor> environment)
	{
		return new GenericTest<TSut, TBackDoor>(environment);
	}
}