using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.FileAccess.FileSystemTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.FileAccess.FileSystemTests;

public class CreateFileTests
{
	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WithValidPath_ShouldCreateFileOnDisk(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				context.FilePath = backDoor.GetFullPath("new-file.txt");
			})
			.Act((fileSystem, context) =>
			{
				string filePath = context.FilePath;

				using Stream stream = fileSystem.CreateFile(filePath);
			})
			.Assert((backDoor, context) =>
			{
				backDoor.FileExists("new-file.txt").Should().BeTrue();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WithValidPath_ShouldCreateEmptyFile(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				context.FilePath = backDoor.GetFullPath("empty-file.txt");
			})
			.Act((fileSystem, context) =>
			{
				string filePath = context.FilePath;

				using Stream stream = fileSystem.CreateFile(filePath);
			})
			.Assert((backDoor, context) =>
			{
				backDoor.ReadAllText("empty-file.txt").Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_ShouldReturnWritableStream(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				context.FilePath = backDoor.GetFullPath("written-file.txt");
			})
			.Act(async (fileSystem, context) =>
			{
				string filePath = context.FilePath;

				await using Stream stream = fileSystem.CreateFile(filePath);
				await using StreamWriter writer = new(stream);
				await writer.WriteAsync("gem data");
			})
			.Assert((backDoor, context) =>
			{
				backDoor.ReadAllText("written-file.txt").Should().Be("gem data");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WhenFileAlreadyExists_ShouldOverwriteExistingContent(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				backDoor.WriteAllText("existing-file.txt", "old content that is longer than the new one");
				context.FilePath = backDoor.GetFullPath("existing-file.txt");
			})
			.Act(async (fileSystem, context) =>
			{
				string filePath = context.FilePath;

				await using Stream stream = fileSystem.CreateFile(filePath);
				await using StreamWriter writer = new(stream);
				await writer.WriteAsync("new");
			})
			.Assert((backDoor, context) =>
			{
				backDoor.ReadAllText("existing-file.txt").Should().Be("new");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WithNestedExistingDirectory_ShouldCreateFile(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				backDoor.CreateDirectory(Path.Combine("level1", "level2"));
				context.FilePath = backDoor.GetFullPath(Path.Combine("level1", "level2", "nested-file.txt"));
			})
			.Act((fileSystem, context) =>
			{
				string filePath = context.FilePath;

				using Stream stream = fileSystem.CreateFile(filePath);
			})
			.Assert((backDoor, context) =>
			{
				backDoor.FileExists(Path.Combine("level1", "level2", "nested-file.txt")).Should().BeTrue();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WhenParentDirectoryDoesNotExist_ShouldThrowDirectoryNotFoundException(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				context.FilePath = backDoor.GetFullPath(Path.Combine("missing-directory", "file.txt"));
			})
			.Act((fileSystem, context) =>
			{
				string filePath = context.FilePath;

				fileSystem.CreateFile(filePath);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<DirectoryNotFoundException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WithNullPath_ShouldThrowArgumentException(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((fileSystem, context) =>
			{
				fileSystem.CreateFile(null);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WithEmptyPath_ShouldThrowArgumentException(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((fileSystem, context) =>
			{
				fileSystem.CreateFile(string.Empty);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_WithWhitespacePath_ShouldThrowArgumentException(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((fileSystem, context) =>
			{
				fileSystem.CreateFile("   ");
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IFileSystem, IFileSystemBackDoor>]
	public async Task CreateFile_CalledForTwoDifferentPaths_ShouldCreateBothFilesIndependently(ITestEnvironment<IFileSystem, IFileSystemBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange((backDoor, context) =>
			{
				context.FilePath1 = backDoor.GetFullPath("file1.txt");
				context.FilePath2 = backDoor.GetFullPath("file2.txt");
			})
			.Act(async (fileSystem, context) =>
			{
				string filePath1 = context.FilePath1;
				string filePath2 = context.FilePath2;

				await using (Stream stream1 = fileSystem.CreateFile(filePath1))
				await using (StreamWriter writer1 = new(stream1))
					await writer1.WriteAsync("content 1");

				await using (Stream stream2 = fileSystem.CreateFile(filePath2))
				await using (StreamWriter writer2 = new(stream2))
					await writer2.WriteAsync("content 2");
			})
			.Assert((backDoor, context) =>
			{
				backDoor.ReadAllText("file1.txt").Should().Be("content 1");
				backDoor.ReadAllText("file2.txt").Should().Be("content 2");
			})
			.ExecuteAsync();
	}
}
