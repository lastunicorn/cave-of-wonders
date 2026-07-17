using DustInTheWind.CaveOfWonders.Adapters.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;

/// <summary>
/// Backs one <c>ISystemClock</c> test run with the real <see cref="SystemClock"/> adapter and an independent
/// <see cref="RealSystemClockGateway"/> reading the same real system clock. Unlike storage-backed ports, there is
/// no external resource to create or tear down: both access paths simply read the machine's current time.
/// </summary>
[TestEnvironment("Real")]
internal sealed class RealSystemClockEnvironment : ITestEnvironment<ISystemClock, ISystemClockGateway>
{
	public ISystemClock Sut { get; private set; }

	public ISystemClockGateway Gateway { get; private set; }

	public Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		Sut = new SystemClock();
		return Task.CompletedTask;
	}

	public Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		Sut = null;
		return Task.CompletedTask;
	}

	public Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		Gateway = new RealSystemClockGateway();
		return Task.CompletedTask;
	}

	public Task CloseGatewayAsync(CancellationToken cancellationToken = default)
	{
		Gateway = null;
		return Task.CompletedTask;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		Sut = null;
		Gateway = null;
	}

	public override string ToString()
	{
		return "Real";
	}
}
