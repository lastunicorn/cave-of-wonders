// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
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
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentState;

public class PresentStateUseCase : IRequestHandler<PresentStateRequest, PresentStateResponse>
{
    private readonly IPotRepository potRepository;
    private readonly IConversionRateRepository conversionRateRepository;

    public PresentStateUseCase(IPotRepository potRepository, IConversionRateRepository conversionRateRepository)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.conversionRateRepository = conversionRateRepository ?? throw new ArgumentNullException(nameof(conversionRateRepository));
    }

    public async Task<PresentStateResponse> Handle(PresentStateRequest request, CancellationToken cancellationToken)
    {
        DateTime date = request.Date ?? DateTime.Today;
        string currency = request.Currency ?? "RON";

        List<ConversionRate> conversionRates = await RetrieveConversionRates(date);
        List<PotInstance> potInstances = await RetrievePotSnapshots(date, currency, conversionRates);

        return new PresentStateResponse
        {
            Date = date,
            Values = potInstances,
            ConversionRates = conversionRates
                .Select(x => new ConversionRateInfo(x))
                .ToList(),
            Total = new CurrencyValue
            {
                Value = potInstances.Sum(x => x.ConvertedValue?.Value ?? x.OriginalValue?.Value ?? 0),
                Currency = currency
            }
        };
    }

    private async Task<List<ConversionRate>> RetrieveConversionRates(DateTime date)
    {
        IEnumerable<ConversionRate> conversionRates = await conversionRateRepository.GetAll(date);
        return conversionRates.ToList();
    }

    private async Task<List<PotInstance>> RetrievePotSnapshots(DateTime date, string currency, List<ConversionRate> conversionRates)
    {
        IEnumerable<PotSnapshot> potSnapshots = await potRepository.GetSnapshot(date);

        SnapshotTransformation snapshotTransformation = new()
        {
            Snapshots = potSnapshots.ToList(),
            DestinationCurrency = currency,
            ConversionRates = conversionRates
        };

        return snapshotTransformation.Transform().ToList();
    }
}