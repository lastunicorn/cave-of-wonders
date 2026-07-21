using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;
using DustInTheWind.CaveOfWonders.Ports.BcrAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.FintownAccess;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using DustInTheWind.CaveOfWonders.Ports.PeerBerryAccess;
using DustInTheWind.CaveOfWonders.Ports.QuanloopAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

internal class ImportGemsUseCase : IRequestHandler<ImportGemsRequest, ImportGemsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IMintosService mintosService;
	private readonly IFintownService fintownService;
	private readonly IBcrService bcrService;
	private readonly IPeerBerryService peerBerryService;
	private readonly IQuanloopService quanloopService;
	private readonly IFileSystem fileSystem;

	public ImportGemsUseCase(IUnitOfWork unitOfWork, IMintosService mintosService, IFintownService fintownService, IBcrService bcrService, IPeerBerryService peerBerryService, IQuanloopService quanloopService, IFileSystem fileSystem)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.mintosService = mintosService ?? throw new ArgumentNullException(nameof(mintosService));
		this.fintownService = fintownService ?? throw new ArgumentNullException(nameof(fintownService));
		this.bcrService = bcrService ?? throw new ArgumentNullException(nameof(bcrService));
		this.peerBerryService = peerBerryService ?? throw new ArgumentNullException(nameof(peerBerryService));
		this.quanloopService = quanloopService ?? throw new ArgumentNullException(nameof(quanloopService));
		this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
	}

	public Task<ImportGemsResponse> Handle(ImportGemsRequest request, CancellationToken cancellationToken)
	{
		return Measure
			.Action(async () =>
			{
				Pot pot = await FindPot(request.PotFlexId, cancellationToken);
				List<string> filePaths = fileSystem.EnumerateFiles(request.FilePath).ToList();

				if (filePaths.Count == 0)
					throw new NoMatchingFilesFoundException(request.FilePath);

				ImportGemsResponse response = new();

				foreach (string filePath in filePaths)
				{
					IAsyncEnumerable<Gem> gemEnumeration = GetGemsFromSource(filePath, request.FileType, cancellationToken);
					FileImportResult fileImportResult = await ImportGems(gemEnumeration, pot, cancellationToken);

					response.FileImportResults.Add(new FileImportResult
					{
						FilePath = filePath,
						AddedGemCount = fileImportResult.AddedGemCount,
						UpdatedGemCount = fileImportResult.UpdatedGemCount,
						SkippedGemCount = fileImportResult.SkippedGemCount
					});

					response.UpdatedGemCount += fileImportResult.UpdatedGemCount;
					response.AddedGemCount += fileImportResult.AddedGemCount;
					response.SkippedGemCount += fileImportResult.SkippedGemCount;
					response.TotalGemCount += fileImportResult.AddedGemCount + fileImportResult.UpdatedGemCount + fileImportResult.SkippedGemCount;
				}

				await unitOfWork.SaveChangesAsync(cancellationToken);
				return response;
			})
			.OnFinished((measurement, response) =>
			{
				response.Duration = measurement.Time;
			})
			.Response();
	}

	private async Task<Pot> FindPot(PotFlexId potFlexId, CancellationToken cancellationToken)
	{
		IAsyncEnumerable<Pot> pots = unitOfWork.PotRepository.GetAsync(potFlexId, cancellationToken)
			.Where(x => x != null);

		Pot foundPot = null;

		await foreach (Pot pot in pots)
		{
			if (foundPot != null)
				throw new MultiplePotsException(potFlexId);

			foundPot = pot;
		}

		if (foundPot == null)
			throw new PotNotFoundException(potFlexId);

		return foundPot;
	}

	private IAsyncEnumerable<Gem> GetGemsFromSource(string filePath, FileType fileType, CancellationToken cancellationToken)
	{
		IAsyncEnumerable<Gem> gemEnumeration = fileType switch
		{
			FileType.Mintos => mintosService.GetGemsAsync(filePath, cancellationToken),
			FileType.Fintown => fintownService.GetGemsAsync(filePath, cancellationToken),
			FileType.Bcr => bcrService.GetGemsAsync(filePath, cancellationToken),
			FileType.PeerBerry => peerBerryService.GetGemsAsync(filePath, cancellationToken),
			FileType.Quanloop => quanloopService.GetGemsAsync(filePath, cancellationToken),
			FileType.Unknown => throw new UnknownFileTypeException(fileType),
			_ => throw new UnknownFileTypeException(fileType)
		};
		return gemEnumeration;
	}

	private async Task<FileImportResult> ImportGems(IAsyncEnumerable<Gem> gemEnumeration, Pot pot, CancellationToken cancellationToken)
	{
		FileImportResult fileImportResult = new();

		await foreach (Gem gem in gemEnumeration.WithCancellation(cancellationToken))
		{
			gem.Pot = pot;
			Gem existingGem = await unitOfWork.GemRepository.GetByExternalIdAsync(pot.Id, gem.ExternalId, cancellationToken)
				.ConfigureAwait(false);

			if (existingGem != null)
			{
				if (gem == existingGem)
					SkipGem(fileImportResult);
				else
					UpdateGem(fileImportResult, existingGem, gem);
			}
			else
			{
				AddGem(fileImportResult, gem);
			}
		}

		return fileImportResult;
	}

	private static void SkipGem(FileImportResult fileImportResult)
	{
		fileImportResult.SkippedGemCount++;
	}

	private static void UpdateGem(FileImportResult fileImportResult, Gem existingGem, Gem gem)
	{
		fileImportResult.UpdatedGemCount++;

		existingGem.Category = gem.Category;
		existingGem.Amount = gem.Amount;
		existingGem.Description = gem.Description;
		existingGem.Pot = gem.Pot;
		existingGem.Parameters.Clear();
		existingGem.Parameters.AddRange(gem.Parameters);
	}

	private void AddGem(FileImportResult fileImportResult, Gem gem)
	{
		fileImportResult.AddedGemCount++;

		unitOfWork.GemRepository.Add(gem);
	}
}