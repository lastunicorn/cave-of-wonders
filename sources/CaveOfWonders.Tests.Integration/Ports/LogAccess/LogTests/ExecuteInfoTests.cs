using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests;

public class ExecuteInfoTests
{
	private const string TimestampPattern = @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]";

	private static readonly string Separator = new('-', 100);

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WithValidTitleAndAction_ShouldWriteOpeningSeparatorTitleAndClosingSeparator(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				await log.ExecuteInfo("Starting import", () => Task.FromResult(42));
			})
			.Assert((backDoor, context) =>
			{
				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(3);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Starting import$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WithValidAction_ShouldReturnActionResult(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				context.Result = await log.ExecuteInfo("Computing", () => Task.FromResult(42));
			})
			.Assert((backDoor, context) =>
			{
				int result = context.Result;
				result.Should().Be(42);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WithReferenceTypeResult_ShouldReturnActionResult(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				context.Result = await log.ExecuteInfo("Computing", () => Task.FromResult("done"));
			})
			.Assert((backDoor, context) =>
			{
				string result = context.Result;
				result.Should().Be("done");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WhenActionWritesSynchronously_ShouldInterleaveActionLogBetweenSeparators(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				await log.ExecuteInfo("Title", () =>
				{
					log.WriteInfo("From action");
					return Task.FromResult(1);
				});
			})
			.Assert((backDoor, context) =>
			{
				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(4);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Title$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} From action$");
				lines[3].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WhenActionYieldsBeforeLogging_ShouldWriteClosingSeparatorBeforeActionsDeferredLog(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		// ExecuteInfo is not itself async: it invokes the action, then runs its closing-separator logic in a
		// `finally` before the returned task is awaited by the caller. So when the action suspends (e.g. on a
		// real await) before logging anything itself, the closing separator lands in the file BEFORE the
		// action's own deferred log line, even though the action's line is conceptually "inside" the block.
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				await log.ExecuteInfo("Title", async () =>
				{
					await Task.Delay(20);
					log.WriteInfo("Deferred");
					return 1;
				});
			})
			.Assert((backDoor, context) =>
			{
				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(4);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Title$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[3].Should().MatchRegex($"^{TimestampPattern} Deferred$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WhenActionThrowsSynchronously_ShouldWriteClosingSeparatorAndPropagateException(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				Func<Task<int>> action = () => throw new InvalidOperationException("boom");

				try
				{
					await log.ExecuteInfo("Title", action);
				}
				catch (Exception ex)
				{
					context.CaughtException = ex;
				}
			})
			.Assert((backDoor, context) =>
			{
				Exception caughtException = context.CaughtException;
				caughtException.Should().BeOfType<InvalidOperationException>();
				caughtException.Message.Should().Be("boom");

				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(3);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Title$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WhenActionThrowsAsynchronously_ShouldWriteClosingSeparatorAndPropagateException(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				Func<Task<int>> action = async () =>
				{
					await Task.Delay(20);
					throw new InvalidOperationException("async boom");
				};

				try
				{
					await log.ExecuteInfo("Title", action);
				}
				catch (Exception ex)
				{
					context.CaughtException = ex;
				}
			})
			.Assert((backDoor, context) =>
			{
				Exception caughtException = context.CaughtException;
				caughtException.Should().BeOfType<InvalidOperationException>();
				caughtException.Message.Should().Be("async boom");

				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(3);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Title$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WithNullTitle_ShouldWriteEmptyTitleLineBetweenSeparators(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				await log.ExecuteInfo(null, () => Task.FromResult(1));
			})
			.Assert((backDoor, context) =>
			{
				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(3);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} $");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WithNullAction_ShouldWriteBothSeparatorsAndThrowNullReferenceException(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				try
				{
					await log.ExecuteInfo<int>("Title", null);
				}
				catch (Exception ex)
				{
					context.CaughtException = ex;
				}
			})
			.Assert((backDoor, context) =>
			{
				Exception caughtException = context.CaughtException;
				caughtException.Should().BeOfType<NullReferenceException>();

				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(3);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Title$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_CalledMultipleTimes_ShouldAppendEachBlockSequentially(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (log, context) =>
			{
				await log.ExecuteInfo("First block", () => Task.FromResult(1));
				await log.ExecuteInfo("Second block", () => Task.FromResult(2));
			})
			.Assert((backDoor, context) =>
			{
				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(6);
				lines[0].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} First block$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[3].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[4].Should().MatchRegex($"^{TimestampPattern} Second block$");
				lines[5].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogBackDoor>]
	public async Task ExecuteInfo_WhenLogFileAlreadyHasContent_ShouldAppendWithoutOverwritingExistingContent(ITestEnvironment<ILog, ILogBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				backDoor.SeedLogFile($"[2023-01-01 00:00:00.000] Previous run entry{Environment.NewLine}");
			})
			.Act(async (log, context) =>
			{
				await log.ExecuteInfo("New block", () => Task.FromResult(1));
			})
			.Assert((backDoor, context) =>
			{
				List<string> lines = backDoor.ReadAllLines();

				lines.Should().HaveCount(4);
				lines[0].Should().Be("[2023-01-01 00:00:00.000] Previous run entry");
				lines[1].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} New block$");
				lines[3].Should().MatchRegex($"^{TimestampPattern} {Separator}$");
			})
			.ExecuteAsync();
	}
}
