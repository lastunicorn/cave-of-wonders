using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FintownAccess;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

/// <summary>
/// For now, this use case is hard-coded to import only Mintos gems (transactions).
/// </summary>
internal class ImportGemsUseCase : IRequestHandler<ImportGemsRequest, ImportGemsResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMintosService mintosService;
    private readonly IFintownService fintownService;

    public ImportGemsUseCase(IUnitOfWork unitOfWork, IMintosService mintosService, IFintownService fintownService)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.mintosService = mintosService ?? throw new ArgumentNullException(nameof(mintosService));
        this.fintownService = fintownService ?? throw new ArgumentNullException(nameof(fintownService));
    }

    public async Task<ImportGemsResponse> Handle(ImportGemsRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await FindPot(request.PotFlexId, cancellationToken);
        
        IAsyncEnumerable<Gem> gemEnumeration = request.FileType switch
        {
            FileType.Mintos => mintosService.GetGemsAsync(request.FilePath, cancellationToken),
            FileType.Fintown => fintownService.GetGemsAsync(request.FilePath, cancellationToken),
            FileType.Unknown => throw new NotImplementedException($"Unknown file type '{request.FileType}'."),
            _ => throw new NotImplementedException($"Unknown file type '{request.FileType}'.")
        };
        
        ImportGemsResponse response = await ImportGems(gemEnumeration, pot, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    private async Task<Pot> FindPot(PotFlexId potFlexId, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.PotRepository.GetByIdOrNameAsync(potFlexId, cancellationToken)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new CaveOfWandersException($"The specified pot identifier must match a single pot: '{potFlexId}'", ex);
        }
    }

    private async Task<ImportGemsResponse> ImportGems(IAsyncEnumerable<Gem> gemEnumeration, Pot pot, CancellationToken cancellationToken)
    {
        ImportGemsResponse response = new();

        await foreach (Gem gem in gemEnumeration.WithCancellation(cancellationToken))
        {
            response.TotalGemCount++;

            gem.Pot = pot;
            Gem existingGem = await FindExistingGem(gem, cancellationToken);

            if (existingGem != null)
            {
                if (gem != existingGem)
                {
                    response.UpdatedGemCount++;

                    existingGem.Category = gem.Category;
                    existingGem.Amount = gem.Amount;
                    existingGem.Description = gem.Description;
                    existingGem.Pot = gem.Pot;
                    existingGem.Parameters.Clear();
                    existingGem.Parameters.AddRange(gem.Parameters);
                }
                else
                {
                    response.SkippedGemCount++;
                }
            }
            else
            {
                response.AddedGemCount++;
                unitOfWork.GemRepository.Add(gem);
            }
        }
        
        return response;
    }

    private async Task<Gem> FindExistingGem(Gem gem, CancellationToken cancellationToken)
    {
        if (gem.Pot == null)
            return null;
        
        return await unitOfWork.GemRepository.GetByExternalIdAsync(gem.Pot.Id, gem.ExternalId, cancellationToken)
            .ConfigureAwait(false);
    }
}