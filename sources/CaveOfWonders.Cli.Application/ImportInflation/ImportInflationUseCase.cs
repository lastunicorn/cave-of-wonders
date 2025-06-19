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

using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using MediatR;
using InflationRecordDto = DustInTheWind.CaveOfWonders.Ports.DataAccess.InflationRecordDto;
using InsInflationRecordDto = DustInTheWind.CaveOfWonders.Ports.InsAccess.InflationRecordDto;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;

internal class ImportInflationUseCase : IRequestHandler<ImportInflationRequest, ImportInflationResponse>
{
    private readonly IIns ins;
    private readonly IUnitOfWork unitOfWork;

    public ImportInflationUseCase(IIns ins, IUnitOfWork unitOfWork)
    {
        this.ins = ins ?? throw new ArgumentNullException(nameof(ins));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ImportInflationResponse> Handle(ImportInflationRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<InsInflationRecordDto> inflationRecordDtos = await RetrieveInflationValues(request);
        IAsyncEnumerable<AddOrUpdateResult> addOrUpdateResults = AddOrUpdateInflationRecordsToStore(inflationRecordDtos);

        ImportInflationResponse response = new();
        await response.AddResultsAsync(addOrUpdateResults);

        await unitOfWork.SaveChanges();

        return response;
    }

    private async Task<IEnumerable<InsInflationRecordDto>> RetrieveInflationValues(ImportInflationRequest request)
    {
        switch (request.ImportSource)
        {
            case ImportSource.File:
                if (string.IsNullOrEmpty(request.SourceFilePath))
                    throw new InflationFileNotProvidedException();

                return await ins.GetInflationValuesFromFile(request.SourceFilePath);

            case ImportSource.Web:
                return await ins.GetInflationValuesFromWeb();

            default:
                throw new InvalidImportSourceException(request.ImportSource);
        }
    }

    private async IAsyncEnumerable<AddOrUpdateResult> AddOrUpdateInflationRecordsToStore(IEnumerable<InsInflationRecordDto> inflationRecordDtos)
    {
        foreach (InsInflationRecordDto insInflationRecordDto in inflationRecordDtos)
        {
            InflationRecordDto inflationRecordDto = new()
            {
                Year = insInflationRecordDto.Year,
                Value = insInflationRecordDto.Value
            };

            yield return await unitOfWork.InflationRecordRepository.AddOrUpdate(inflationRecordDto);
        }
    }
}