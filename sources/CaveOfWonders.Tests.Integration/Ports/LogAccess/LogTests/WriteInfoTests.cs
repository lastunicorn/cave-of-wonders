using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests;

public class WriteInfoTests
{
	private const string TimestampPattern = @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]";

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_WithValidText_ShouldCreateLogFile(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((log, context) =>
			{
				log.WriteInfo("Application started");
			})
			.Assert((gateway, context) =>
			{
				gateway.LogFileExists().Should().BeTrue();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_WithValidText_ShouldWriteLinePrefixedWithTimestamp(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((log, context) =>
			{
				log.WriteInfo("Application started");
			})
			.Assert((gateway, context) =>
			{
				List<string> lines = gateway.ReadAllLines();

				lines.Should().HaveCount(1);
				lines[0].Should().MatchRegex($"^{TimestampPattern} Application started$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_CalledMultipleTimes_ShouldAppendEachMessageOnItsOwnLine(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((log, context) =>
			{
				log.WriteInfo("First message");
				log.WriteInfo("Second message");
				log.WriteInfo("Third message");
			})
			.Assert((gateway, context) =>
			{
				List<string> lines = gateway.ReadAllLines();

				lines.Should().HaveCount(3);
				lines[0].Should().MatchRegex($"^{TimestampPattern} First message$");
				lines[1].Should().MatchRegex($"^{TimestampPattern} Second message$");
				lines[2].Should().MatchRegex($"^{TimestampPattern} Third message$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_WithEmptyText_ShouldWriteLineWithTimestampOnly(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((log, context) =>
			{
				log.WriteInfo(string.Empty);
			})
			.Assert((gateway, context) =>
			{
				List<string> lines = gateway.ReadAllLines();

				lines.Should().HaveCount(1);
				lines[0].Should().MatchRegex($"^{TimestampPattern} $");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_WithNullText_ShouldNotThrowAndShouldWriteLineWithTimestampOnly(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((log, context) =>
			{
				log.WriteInfo(null);
			})
			.Assert((gateway, context) =>
			{
				List<string> lines = gateway.ReadAllLines();

				lines.Should().HaveCount(1);
				lines[0].Should().MatchRegex($"^{TimestampPattern} $");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_WhenLogFileAlreadyHasContent_ShouldAppendWithoutOverwritingExistingContent(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((gateway, context) =>
			{
				gateway.SeedLogFile($"[2023-01-01 00:00:00.000] Previous run entry{Environment.NewLine}");
			})
			.Act((log, context) =>
			{
				log.WriteInfo("New entry");
			})
			.Assert((gateway, context) =>
			{
				List<string> lines = gateway.ReadAllLines();

				lines.Should().HaveCount(2);
				lines[0].Should().Be("[2023-01-01 00:00:00.000] Previous run entry");
				lines[1].Should().MatchRegex($"^{TimestampPattern} New entry$");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<ILog, ILogGateway>]
	public async Task WriteInfo_WithSpecialCharactersAndUnicode_ShouldPreserveTextVerbatim(ITestEnvironment<ILog, ILogGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((log, context) =>
			{
				log.WriteInfo("Import finished: 100% – 42 gems imported [ok]");
			})
			.Assert((gateway, context) =>
			{
				List<string> lines = gateway.ReadAllLines();

				lines.Should().HaveCount(1);
				lines[0].Should().MatchRegex($"^{TimestampPattern} Import finished: 100% – 42 gems imported \\[ok\\]$");
			})
			.ExecuteAsync();
	}
}
