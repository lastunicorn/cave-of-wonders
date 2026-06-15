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
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;

internal class ImportCpiUseCase : IRequestHandler<ImportCpiRequest, ImportCpiResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ICpiImportExportFactory cpiImportExportFactory;
	private ImportCpiResponse response;

	public ImportCpiUseCase(IUnitOfWork unitOfWork, ICpiImportExportFactory cpiImportExportFactory)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.cpiImportExportFactory = cpiImportExportFactory ?? throw new ArgumentNullException(nameof(cpiImportExportFactory));
	}

	public async Task<ImportCpiResponse> Handle(ImportCpiRequest request, CancellationToken cancellationToken)
	{
		response = new ImportCpiResponse();

		IEnumerable<CpiRecordDto> cpiRecordDtos = await RetrieveInflationValues(request);
		await AddOrUpdateCpiRecordsToStore(cpiRecordDtos);

		await unitOfWork.SaveChanges();

		return response;
	}

	private async Task<IEnumerable<CpiRecordDto>> RetrieveInflationValues(ImportCpiRequest request)
	{
		CpiImportType cpiImportType = request.ImportSource switch
		{
			ImportSource.File => CpiImportType.File,
			ImportSource.Web => CpiImportType.Web,
			_ => throw new ArgumentOutOfRangeException()
		};

		CpiImportParameters parameters = new()
		{
			{ "FilePath", request.SourceFilePath }
		};

		ICpiImportExport cpiImportExport = cpiImportExportFactory.Create(cpiImportType, parameters);
		return await cpiImportExport.ImportAsync().ToListAsync();
	}

	private async Task AddOrUpdateCpiRecordsToStore(IEnumerable<CpiRecordDto> cpiRecordDtos)
	{
		try
		{
			IAsyncEnumerable<AddOrUpdateResult> results = AddOrUpdateCpiRecordsToStoreUnsafe(cpiRecordDtos);

			await foreach (AddOrUpdateResult result in results)
				response.AddResult(result);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}

	private async IAsyncEnumerable<AddOrUpdateResult> AddOrUpdateCpiRecordsToStoreUnsafe(IEnumerable<CpiRecordDto> cpiRecordDtos)
	{
		foreach (CpiRecordDto cpiRecordDto in cpiRecordDtos)
		{
			Cpi cpi = new()
			{
				Year = cpiRecordDto.Year,
				Value = cpiRecordDto.Value
			};

			yield return await unitOfWork.CpiRepository.AddOrUpdate(cpi);
		}
	}
}