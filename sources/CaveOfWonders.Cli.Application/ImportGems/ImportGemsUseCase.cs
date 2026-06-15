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

using DustInTheWind.CaveOfWanders.Ports.MintosAccess;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

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
        Guid mintosId = new Guid("f48d26ca-ed78-4da4-8d27-ca65676f8ecc");
        Pot pot = unitOfWork.PotRepository.GetById(mintosId).Result;
        
        IAsyncEnumerable<Gem> gemEnumeration = mintosService.GetGemsAsync(request.FilePath, cancellationToken);

        ImportGemsResponse response = new();
        
        await foreach (Gem gem in gemEnumeration)
        {
            response.TotalGemsCount++;
            
            Gem existingGem = await unitOfWork.GemRepository.GetByDate(mintosId, gem.Date);

            if (existingGem != null)
            {
                if (gem != existingGem)
                {
                    response.UpdatedGemsCount++;
                    
                    existingGem.Category = gem.Category;
                    existingGem.Amount = gem.Amount;
                    existingGem.Description = gem.Description;
                    existingGem.Parameters.Clear();
                    existingGem.Parameters.AddRange(gem.Parameters);
                }
            }
            else
            {
                response.AddedGemsCount++;

                gem.Pot = pot;
                unitOfWork.GemRepository.Add(gem);
            }
        }

        await unitOfWork.SaveChanges();
        
        return response;
    }
}