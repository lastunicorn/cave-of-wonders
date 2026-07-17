using DustInTheWind.CaveOfWonders.Adapters.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.FileAccess.FileSystemTests.TestEnvironments;

/// <summary>
/// Owns a temporary sandbox directory on the real file system backing one <c>CreateFile</c> test run. The SUT
/// (the real <see cref="FileSystem"/> adapter) and the back-door <see cref="IFileSystemBackDoor"/> both operate
/// on the same directory, which is removed once the run completes.
/// </summary>
[TestEnvironment("Default")]
internal sealed class FileSystemEnvironment : ITestEnvironment<IFileSystem, IFileSystemBackDoor>
{
	private readonly string rootDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-filesystem-{Guid.NewGuid()}");

	public IFileSystem Sut { get; private set; }

	public IFileSystemBackDoor BackDoor { get; private set; }

	public Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(rootDirectoryPath);
		Sut = new FileSystem();
		return Task.CompletedTask;
	}

	public Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		Sut = null;
		return Task.CompletedTask;
	}

	public Task CreateBackDoorAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(rootDirectoryPath);
		BackDoor = new FileSystemBackDoor(rootDirectoryPath);
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
		Sut = null;
		BackDoor = null;
	}

	public override string ToString()
	{
		return "Real";
	}
}
