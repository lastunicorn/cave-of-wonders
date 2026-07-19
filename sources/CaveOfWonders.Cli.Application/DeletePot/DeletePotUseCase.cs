using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeletePot;

internal class DeletePotUseCase : IRequestHandler<DeletePotRequest, DeletePotResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public DeletePotUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<DeletePotResponse> Handle(DeletePotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request.PotId, cancellationToken);

        if (pot == null)
        {
	        return new DeletePotResponse
	        {
		        PotFound = false
	        };
        }

        await DeletePotWithRelatedData(pot, cancellationToken);

        return new DeletePotResponse
        {
            PotFound = true,
            PotName = pot.Name
        };
    }

    private async Task<Pot> RetrievePot(PotFlexId potId, CancellationToken cancellationToken)
    {
        IAsyncEnumerable<Pot> pots = unitOfWork.PotRepository.GetAsync(potId, cancellationToken);

        Pot matchedPot = null;

        await foreach (Pot pot in pots)
        {
            if (matchedPot != null)
                throw new MultiplePotsException(potId);

            if (pot != null)
                matchedPot = pot;
        }

        return matchedPot;
    }

    private async Task DeletePotWithRelatedData(Pot pot, CancellationToken cancellationToken)
    {
        try
        {
            List<Gem> gems = await unitOfWork.GemRepository.GetByPotIdAsync(pot.Id, cancellationToken)
                .ToListAsync(cancellationToken);

            foreach (Gem gem in gems)
                unitOfWork.GemRepository.Remove(gem);

            unitOfWork.PotRepository.Remove(pot);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new StorageInaccessibleException(ex);
        }
    }
}
