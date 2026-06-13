using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Domain.Inflation;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ExportCpi;

internal class ExportCpiUseCase : IRequestHandler<ExportCpiRequest>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystem fileSystem;

    public ExportCpiUseCase(IUnitOfWork unitOfWork, IFileSystem fileSystem)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task Handle(ExportCpiRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.OutputPath.Trim()))
            throw new OutputPathNotProvidedException();

        using CpiDocument exportDocument = CreateExportFile(request.OutputPath);

        IEnumerable<Cpi> inflationRecords = await RetrieveInflationRecordsFromStorage();

        foreach (Cpi inflationRecord in inflationRecords)
            await WriteToExportDocument(exportDocument, inflationRecord);
    }

    private CpiDocument CreateExportFile(string outputPath)
    {
        Stream exportStream = fileSystem.CreateFile(outputPath);
        return new CpiDocument(exportStream);
    }

    private async Task<IEnumerable<Cpi>> RetrieveInflationRecordsFromStorage()
    {
        IEnumerable<Cpi> inflationRecords = await unitOfWork.CpiRepository.GetAll();

        return inflationRecords
            .OrderBy(x => x.Year);
    }

    private static async Task WriteToExportDocument(CpiDocument exportDocument, Cpi cpi)
    {
        CpiRecordLine exportCpiRecord = new(cpi.Year, cpi.Value);
        await exportDocument.Write(exportCpiRecord);
    }
}
