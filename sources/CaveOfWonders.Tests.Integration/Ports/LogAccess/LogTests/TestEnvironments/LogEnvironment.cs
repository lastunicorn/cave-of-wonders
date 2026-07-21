using DustInTheWind.CaveOfWonders.Adapters.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests.TestEnvironments;

/// <summary>
/// Owns a temporary sandbox directory on the real file system backing one <c>WriteInfo</c> test run. The real
/// <see cref="Log"/> adapter derives its log file path from <see cref="Environment.CurrentDirectory"/> rather than
/// an injected path, so this environment briefly points the process's current directory at the sandbox only for
/// the moment the SUT is constructed, then restores it - under a lock, since <see cref="Environment.CurrentDirectory"/>
/// is global process state.
/// </summary>
[TestEnvironment("Default")]
internal sealed class LogEnvironment : ITestEnvironment<ILog, ILogBackDoor>
{
	private static readonly object CurrentDirectoryLock = new();

	private readonly string rootDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}");

	public ILog Sut { get; private set; }

	public ILogBackDoor BackDoor { get; private set; }

	public Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(rootDirectoryPath);

		lock (CurrentDirectoryLock)
		{
			string originalDirectoryPath = Environment.CurrentDirectory;

			try
			{
				Environment.CurrentDirectory = rootDirectoryPath;
				Sut = new Log();
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectoryPath;
			}
		}

		return Task.CompletedTask;
	}

	public Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		(Sut as IDisposable)?.Dispose();
		Sut = null;
		return Task.CompletedTask;
	}

	public Task CreateBackDoorAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(rootDirectoryPath);
		BackDoor = new LogBackDoor(rootDirectoryPath);
		return Task.CompletedTask;
	}

	public Task CloseBackDoorAsync(CancellationToken cancellationToken = default)
	{
		BackDoor = null;
		return Task.CompletedTask;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		if (Directory.Exists(rootDirectoryPath))
			Directory.Delete(rootDirectoryPath, true);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		(Sut as IDisposable)?.Dispose();
		Sut = null;
		BackDoor = null;
	}

	public override string ToString()
	{
		return "Real";
	}
}