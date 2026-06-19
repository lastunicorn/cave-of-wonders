// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

/// <summary>
/// For now, this use case is hard-coded to import only Mintos gems (transactions).
/// </summary>
internal class ImportGemsUseCase : IRequestHandler<ImportGemsRequest, ImportGemsResponse>
{
    private readonly IMintosService mintosService;
    private readonly IUnitOfWork unitOfWork;

    public ImportGemsUseCase(IMintosService mintosService, IUnitOfWork unitOfWork)
    {
        this.mintosService = mintosService ?? throw new ArgumentNullException(nameof(mintosService));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ImportGemsResponse> Handle(ImportGemsRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await FindPot(request.PotId, cancellationToken);

        IAsyncEnumerable<Gem> gemEnumeration = mintosService.GetGemsAsync(request.FilePath, cancellationToken);

        ImportGemsResponse response = new();

        await foreach (Gem gem in gemEnumeration)
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    private async Task<Pot> FindPot(PotIdentifier potId, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.PotRepository.GetByIdOrName(potId, cancellationToken)
                .SingleAsync();
        }
        catch (Exception ex)
        {
            throw new CaveOfWandersException($"The specified pot identifier must match a single pot: '{potId}'", ex);
        }
    }

    private async Task<Gem> FindExistingGem(Gem gem, CancellationToken cancellationToken)
    {
        if (gem.Pot == null)
            return null;

        IAsyncEnumerable<Gem> existingGems = unitOfWork.GemRepository.GetByDateAsync(gem.Pot.Id, gem.Date, cancellationToken);

        await foreach (Gem existingGem in existingGems)
        {
            if (existingGem.Date != gem.Date)
                continue;

            const string parameterName = "TransactionId";

            string existingTransactionId = existingGem.Parameters.GetValueOrDefault(parameterName);

            if (existingTransactionId == null)
                continue;

            string newTransactionId = gem.Parameters.GetValueOrDefault(parameterName);

            if (newTransactionId == existingTransactionId)
                return existingGem;
        }

        return null;
    }
}