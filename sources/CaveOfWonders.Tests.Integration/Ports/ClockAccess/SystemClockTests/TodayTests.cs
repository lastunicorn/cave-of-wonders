using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests;

public class TodayTests
{
	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockGateway>]
	public async Task Today_WhenAccessed_ShouldFallBetweenReferenceDatesTakenImmediatelyBeforeAndAfter(ITestEnvironment<ISystemClock, ISystemClockGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((gateway, context) =>
			{
				context.Before = gateway.GetCurrentDate();
			})
			.Act((clock, context) =>
			{
				context.Result = clock.Today;
			})
			.Assert((gateway, context) =>
			{
				DateOnly after = gateway.GetCurrentDate();

				DateOnly before = context.Before;
				DateOnly result = context.Result;

				result.Should().BeOnOrAfter(before);
				result.Should().BeOnOrBefore(after);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockGateway>]
	public async Task Today_WhenAccessedTwiceInImmediateSuccession_ShouldReturnEqualValues(ITestEnvironment<ISystemClock, ISystemClockGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((clock, context) =>
			{
				context.First = clock.Today;
				context.Second = clock.Today;
			})
			.Assert((gateway, context) =>
			{
				DateOnly first = context.First;
				DateOnly second = context.Second;

				second.Should().Be(first);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ISystemClock, ISystemClockGateway>]
	public async Task Today_WhenAccessed_ShouldEqualDateComponentOfNowFromTheSameInstance(ITestEnvironment<ISystemClock, ISystemClockGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((clock, context) =>
			{
				context.Now = clock.Now;
				context.Today = clock.Today;
			})
			.Assert((gateway, context) =>
			{
				DateTime now = context.Now;
				DateOnly today = context.Today;

				today.Should().Be(DateOnly.FromDateTime(now));
			})
			.ExecuteAsync();
	}
}
