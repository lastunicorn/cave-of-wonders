using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests;

public class NowTests
{
	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockBackDoor>]
	public async Task Now_WhenAccessed_ShouldFallBetweenReferenceTimestampsTakenImmediatelyBeforeAndAfter(ITestEnvironment<ISystemClock, ISystemClockBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				context.Before = backDoor.GetCurrentTime();
			})
			.Act((clock, context) =>
			{
				context.Result = clock.Now;
			})
			.Assert((backDoor, context) =>
			{
				DateTime after = backDoor.GetCurrentTime();

				DateTime before = context.Before;
				DateTime result = context.Result;

				result.Should().BeOnOrAfter(before);
				result.Should().BeOnOrBefore(after);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockBackDoor>]
	public async Task Now_WhenAccessed_ShouldHaveLocalDateTimeKind(ITestEnvironment<ISystemClock, ISystemClockBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((clock, context) =>
			{
				context.Result = clock.Now;
			})
			.Assert((backDoor, context) =>
			{
				DateTime result = context.Result;
				result.Kind.Should().Be(DateTimeKind.Local);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockBackDoor>]
	public async Task Now_WhenAccessedTwiceInImmediateSuccession_ShouldNotMoveBackwards(ITestEnvironment<ISystemClock, ISystemClockBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((clock, context) =>
			{
				context.First = clock.Now;
				context.Second = clock.Now;
			})
			.Assert((backDoor, context) =>
			{
				DateTime first = context.First;
				DateTime second = context.Second;

				second.Should().BeOnOrAfter(first);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockBackDoor>]
	public async Task Now_WhenAccessedAgainAfterADelay_ShouldReturnALaterValueRatherThanACachedOne(ITestEnvironment<ISystemClock, ISystemClockBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (clock, context) =>
			{
				context.First = clock.Now;
				await Task.Delay(50);
				context.Second = clock.Now;
			})
			.Assert((backDoor, context) =>
			{
				DateTime first = context.First;
				DateTime second = context.Second;

				second.Should().BeAfter(first);
			})
			.ExecuteAsync();
	}
}
