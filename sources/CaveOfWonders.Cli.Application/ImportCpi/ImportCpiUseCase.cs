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
	private readonly IInsService insService;
	private readonly IUnitOfWork unitOfWork;
	private readonly ICpiImportFactory cpiImportFactory;
	private ImportCpiResponse response;

	public ImportCpiUseCase(IInsService insService, IUnitOfWork unitOfWork, ICpiImportFactory cpiImportFactory)
	{
		this.insService = insService ?? throw new ArgumentNullException(nameof(insService));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.cpiImportFactory = cpiImportFactory ?? throw new ArgumentNullException(nameof(cpiImportFactory));
	}

	public async Task<ImportCpiResponse> Handle(ImportCpiRequest request, CancellationToken cancellationToken)
	{
		response = new ImportCpiResponse();

		IEnumerable<CpiRecordDto> inflationRecordDtos = await RetrieveInflationValues(request);
		await AddOrUpdateInflationRecordsToStore(inflationRecordDtos);

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

		ICpiImport cpiImport = cpiImportFactory.Create(cpiImportType, parameters);
		return await cpiImport.ImportAsync().ToListAsync();

		// switch (request.ImportSource)
		// {
		// 	case ImportSource.File:
		// 		if (string.IsNullOrEmpty(request.SourceFilePath))
		// 			throw new InflationFileNotProvidedException();
		//
		// 		try
		// 		{
		// 			return await insService.GetInflationValuesFromFile(request.SourceFilePath);
		// 		}
		// 		catch (Exception ex)
		// 		{
		// 			throw new InsFileException(ex);
		// 		}
		//
		// 	case ImportSource.Web:
		// 		try
		// 		{
		// 			return await insService.GetInflationValuesFromWeb();
		// 		}
		// 		catch (Exception ex)
		// 		{
		// 			throw new InsWebPageException(ex);
		// 		}
		//
		// 	default:
		// 		throw new InvalidImportSourceException(request.ImportSource);
	}

	private async Task AddOrUpdateInflationRecordsToStore(IEnumerable<CpiRecordDto> inflationRecordDtos)
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

	private async IAsyncEnumerable<AddOrUpdateResult> AddOrUpdateInflationRecordsToStoreUnsafe(IEnumerable<CpiRecordDto> inflationRecordDtos)
	{
		foreach (CpiRecordDto insInflationRecordDto in inflationRecordDtos)
		{
			Cpi cpiDto = new()
			{
				Year = insInflationRecordDto.Year,
				Value = insInflationRecordDto.Value
			};

			yield return await unitOfWork.CpiRepository.AddOrUpdate(cpiDto);
		}
	}
}