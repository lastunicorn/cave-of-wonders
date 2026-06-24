// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;

internal class ImportCpiUseCase : IRequestHandler<ImportCpiRequest, ImportCpiResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly CpiImportExportPool cpiImportExportPool;
    private ImportCpiResponse response;

    public ImportCpiUseCase(IUnitOfWork unitOfWork, CpiImportExportPool cpiImportExportPool)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.cpiImportExportPool = cpiImportExportPool ?? throw new ArgumentNullException(nameof(cpiImportExportPool));
    }

    public async Task<ImportCpiResponse> Handle(ImportCpiRequest request, CancellationToken cancellationToken)
    {
        response = new ImportCpiResponse();

        IEnumerable<CpiRecordDto> cpiRecordDtos = await RetrieveInflationValues(request);
        await AddOrUpdateCpiRecordsToStore(cpiRecordDtos, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    private async Task<IEnumerable<CpiRecordDto>> RetrieveInflationValues(ImportCpiRequest request)
    {
        ICpiImportExport cpiImportExport = cpiImportExportPool.Get(request.ImportSource switch
        {
            ImportSource.File => new Guid("bb7590ef-6126-4529-8012-b6a8a4c6f903"),
            ImportSource.Web => new Guid("3ff33b30-a149-4f08-b545-e524fd3e4384"),
            _ => throw new ArgumentOutOfRangeException()
        });

        CpiImportParameters parameters = new()
        {
            { "FilePath", request.SourceFilePath }
        };

        return await cpiImportExport.ImportAsync(parameters)
            .ToListAsync();
    }

    private async Task AddOrUpdateCpiRecordsToStore(IEnumerable<CpiRecordDto> cpiRecordDtos, CancellationToken cancellationToken)
    {
        try
        {
            foreach (CpiRecordDto cpiRecordDto in cpiRecordDtos)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                response.TotalCount++;

                Cpi cpi = new()
                {
                    Year = cpiRecordDto.Year,
                    Value = cpiRecordDto.Value
                };

                Cpi existingCpi = await unitOfWork.CpiRepository.GetByYear(cpi.Year);

                if (existingCpi == null)
                {
                    unitOfWork.CpiRepository.Add(cpi);
                    response.AddedCount++;
                }
                else if (existingCpi.Value != cpi.Value)
                {
                    existingCpi.Value = cpi.Value;
                    response.UpdatedCount++;
                }
            }
        }
        catch (Exception ex)
        {
            throw new DataStorageException(ex);
        }
    }
}