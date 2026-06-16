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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportAverageWage;

internal class WageImportUseCase : IRequestHandler<WageImportRequest, WageImportResponse>
{
    private readonly IInsService insService;
    private readonly IUnitOfWork unitOfWork;

    public WageImportUseCase(IInsService insService, IUnitOfWork unitOfWork)
    {
        this.insService = insService ?? throw new ArgumentNullException(nameof(insService));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<WageImportResponse> Handle(WageImportRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<AverageWage> averageWages = await insService.GetAverageWagesAsync();

        ImportResult importResult = new();
        foreach (AverageWage averageWage in averageWages)
        {
            importResult.TotalCount++;
            
            AverageWage existingAverageWage = await unitOfWork.AverageWageRepository.GetAsync(averageWage.Year, cancellationToken);

            if (existingAverageWage != null)
            {
                if(averageWage.IsEmpty)
                {
                    importResult.DeletedCount++;
                    
                    unitOfWork.AverageWageRepository.Delete(existingAverageWage);
                }
                else if(existingAverageWage != averageWage)
                {
                    importResult.UpdatedCount++;
                    
                    existingAverageWage.GrossValue = averageWage.GrossValue;
                    existingAverageWage.NetValue = averageWage.NetValue;
                }
            }
            else if( !averageWage.IsEmpty)
            {
                importResult.AddedCount++;
                
                existingAverageWage = new AverageWage
                {
                    Year = averageWage.Year,
                    GrossValue = averageWage.GrossValue,
                    NetValue = averageWage.NetValue
                };

                unitOfWork.AverageWageRepository.Add(existingAverageWage);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new WageImportResponse
        {
            TotalCount = importResult.TotalCount,
            AddedCount = importResult.AddedCount,
            UpdatedCount = importResult.UpdatedCount,
            DeletedCount = importResult.DeletedCount
        };
    }
}