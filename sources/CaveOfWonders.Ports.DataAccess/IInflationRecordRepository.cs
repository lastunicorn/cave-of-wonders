using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IInflationRecordRepository
{
    Task<IEnumerable<InflationRecord>> GetAll();

    Task Add(InflationRecord inflationRecordDto);

    Task<AddOrUpdateResult> AddOrUpdate(InflationRecord inflationRecordDto);
}