using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Domain.Inflation;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ExportInflation;

internal class ExportInflationUseCase : IRequestHandler<ExportInflationRequest>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystem fileSystem;

    public ExportInflationUseCase(IUnitOfWork unitOfWork, IFileSystem fileSystem)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task Handle(ExportInflationRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.OutputPath.Trim()))
            throw new OutputPathNotProvidedException();

        using InflationDocument exportDocument = CreateExportFile(request.OutputPath);

        IEnumerable<Cpi> cpiRecords = await RetrieveCpiRecordsFromStorage(cancellationToken);

        foreach (Cpi inflationRecord in cpiRecords)
            await WriteToExportDocument(exportDocument, inflationRecord);
    }

    private InflationDocument CreateExportFile(string outputPath)
    {
        Stream exportStream = fileSystem.CreateFile(outputPath);
        return new InflationDocument(exportStream);
    }

    private async Task<IEnumerable<Cpi>> RetrieveCpiRecordsFromStorage(CancellationToken cancellationToken)
    {
        List<Cpi> cpiRecords = await unitOfWork.CpiRepository.GetAllAsync(cancellationToken)
            .ToListAsync(cancellationToken);

        return cpiRecords
            .OrderBy(x => x.Year);
    }

    private static async Task WriteToExportDocument(InflationDocument exportDocument, Cpi cpi)
    {
        InflationRecordLine exportInflationRecord = new(cpi.Year, cpi.Value - 100);
        await exportDocument.Write(exportInflationRecord);
    }
}