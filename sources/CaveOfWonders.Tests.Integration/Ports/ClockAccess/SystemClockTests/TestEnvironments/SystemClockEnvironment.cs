using DustInTheWind.CaveOfWonders.Adapters.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;

/// <summary>
/// Backs one <c>ISystemClock</c> test run with the real <see cref="SystemClock"/> adapter and an independent
/// <see cref="SystemClockBackDoor"/> reading the same real system clock. Unlike storage-backed ports, there is
/// no external resource to create or tear down: both access paths simply read the machine's current time.
/// </summary>
[TestEnvironment("Default")]
internal sealed class SystemClockEnvironment : ITestEnvironment<ISystemClock, ISystemClockBackDoor>
{
	public ISystemClock Sut { get; private set; }

	public ISystemClockBackDoor BackDoor { get; private set; }

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

	public Task CreateBackDoorAsync(CancellationToken cancellationToken = default)
	{
		BackDoor = new SystemClockBackDoor();
		return Task.CompletedTask;
	}

	public Task CloseBackDoorAsync(CancellationToken cancellationToken = default)
	{
		BackDoor = null;
		return Task.CompletedTask;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		Sut = null;
		BackDoor = null;
	}

	public override string ToString()
	{
		return "Real";
	}
}