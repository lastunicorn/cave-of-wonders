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

using DustInTheWind.CaveOfWonders.Domain.Inflation;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;

internal class PresentInflationUseCase : IRequestHandler<PresentInflationRequest, PresentInflationResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystem fileSystem;

    public PresentInflationUseCase(IUnitOfWork unitOfWork, IFileSystem fileSystem)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task<PresentInflationResponse> Handle(PresentInflationRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.InflationRecord> inflationRecordDtos = await unitOfWork.InflationRecordRepository.GetAll();

        if (request.OutputPath != null)
            await WriteToFile(request.OutputPath, inflationRecordDtos);

        return new PresentInflationResponse
        {
            InflationRecords = inflationRecordDtos
                .Select(x => new InflationRecord(x))
                .ToList()
        };
    }

    private async Task WriteToFile(string outputPath, IEnumerable<Domain.InflationRecord> inflationRecordDtos)
    {
        Stream stream = fileSystem.CreateFile(outputPath);
        using InflationDocument inflationDocument = new(stream);


        IEnumerable<InflationRecordLine> inflationRecordLines = inflationRecordDtos
            .Select(x => new InflationRecordLine(x.Year, x.Value));

        await inflationDocument.Write(inflationRecordLines);
    }
}
