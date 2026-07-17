using DustInTheWind.CaveOfWonders.Adapters.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.FileAccess.FileSystemTests.TestEnvironments;

/// <summary>
/// Owns a temporary sandbox directory on the real file system backing one <c>CreateFile</c> test run. The SUT
/// (the real <see cref="FileSystem"/> adapter) and the back-door <see cref="IFileSystemGateway"/> both operate
/// on the same directory, which is removed once the run completes.
/// </summary>
[TestEnvironment("Real")]
internal sealed class RealFileSystemEnvironment : ITestEnvironment<IFileSystem, IFileSystemGateway>
{
	private readonly string rootDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-filesystem-{Guid.NewGuid()}");

	public IFileSystem Sut { get; private set; }

	public IFileSystemGateway Gateway { get; private set; }

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

	public Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(rootDirectoryPath);
		Gateway = new RealFileSystemGateway(rootDirectoryPath);
		return Task.CompletedTask;
	}

	public Task CloseGatewayAsync(CancellationToken cancellationToken = default)
	{
		Gateway = null;
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
		Gateway = null;
	}

	public override string ToString()
	{
		return "Real";
	}
}
