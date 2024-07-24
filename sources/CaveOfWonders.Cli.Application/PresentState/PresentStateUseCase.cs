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
            ConversionRates = conversionRates,
            Total = new CurrencyValue
            {
                Value = potInstances.Sum(x => x.ConvertedValue?.Value ?? x.Value?.Value ?? 0),
                Currency = currency
            }
        };
    }

    private async Task<List<PotInstance>> RetrievePotSnapshots(DateTime date, string currency, IReadOnlyCollection<ConversionRate> conversionRates)
    {
        IEnumerable<PotSnapshot> potSnapshots = await potRepository.GetSnapshot(date);

        return potSnapshots
            .Select(x =>
            {
                PotInstance potInstance = new(x);

                if (x.Pot.Currency != currency)
                {
                    ConversionRate conversionRate = conversionRates.FirstOrDefault(z => z.CanConvert(potInstance.Value?.Currency, currency));

                    if (conversionRate != null)
                    {
                        potInstance.ConvertedValue = new CurrencyValue
                        {
                            Currency = currency,
                            Value = potInstance.Value.Currency == conversionRate.SourceCurrency
                                ? conversionRate.Convert(potInstance.Value.Value)
                                : conversionRate.ConvertBack(potInstance.Value.Value)
                        };
                    }
                }

                return potInstance;
            })
            .ToList();
    }

    private async Task<List<ConversionRate>> RetrieveConversionRates(DateTime date)
    {
        IEnumerable<ConversionRate> conversionRates = await conversionRateRepository.GetAll(date);
        return conversionRates.ToList();
    }
}