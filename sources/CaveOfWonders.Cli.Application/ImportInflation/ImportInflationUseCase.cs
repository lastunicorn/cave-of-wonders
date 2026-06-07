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

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;

internal class ImportInflationUseCase : IRequestHandler<ImportInflationRequest, ImportInflationResponse>
{
    private readonly IIns ins;
    private readonly IUnitOfWork unitOfWork;
    private ImportInflationResponse response;

    public ImportInflationUseCase(IIns ins, IUnitOfWork unitOfWork)
    {
        this.ins = ins ?? throw new ArgumentNullException(nameof(ins));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ImportInflationResponse> Handle(ImportInflationRequest request, CancellationToken cancellationToken)
    {
        response = new ImportInflationResponse();

        IEnumerable<InflationRecordDto> inflationRecordDtos = await RetrieveInflationValues(request);
        await AddOrUpdateInflationRecordsToStore(inflationRecordDtos);

        await unitOfWork.SaveChanges();

        return response;
    }

    private async Task<IEnumerable<InflationRecordDto>> RetrieveInflationValues(ImportInflationRequest request)
    {
        switch (request.ImportSource)
        {
            case ImportSource.File:
                if (string.IsNullOrEmpty(request.SourceFilePath))
                    throw new InflationFileNotProvidedException();

                try
                {
                    return await ins.GetInflationValuesFromFile(request.SourceFilePath);
                }
                catch (Exception ex)
                {
                    throw new InsFileException(ex);
                }

            case ImportSource.Web:
                try
                {
                    return await ins.GetInflationValuesFromWeb();
                }
                catch (Exception ex)
                {
                    throw new InsWebPageException(ex);
                }

            default:
                throw new InvalidImportSourceException(request.ImportSource);
        }
    }

    private async Task AddOrUpdateInflationRecordsToStore(IEnumerable<InflationRecordDto> inflationRecordDtos)
    {
        try
        {
            IAsyncEnumerable<AddOrUpdateResult> results = AddOrUpdateInflationRecordsToStoreUnsafe(inflationRecordDtos);

            await foreach (AddOrUpdateResult result in results)
                response.AddResult(result);
        }
        catch (Exception ex)
        {
            throw new DataStorageException(ex);
        }
    }

    private async IAsyncEnumerable<AddOrUpdateResult> AddOrUpdateInflationRecordsToStoreUnsafe(IEnumerable<InflationRecordDto> inflationRecordDtos)
    {
        foreach (InflationRecordDto insInflationRecordDto in inflationRecordDtos)
        {
            InflationRecord inflationRecordDto = new()
            {
                Year = insInflationRecordDto.Year,
                Value = insInflationRecordDto.Value
            };

            yield return await unitOfWork.InflationRecordRepository.AddOrUpdate(inflationRecordDto);
        }
    }
}