using System.Xml.Serialization;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles.BnrModels;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles;

public class BnrDocument
{
    public BnrDataSet DataSet { get; private set; }

    public static BnrDocument Load(Stream stream)
    {
        XmlSerializer xmlSerializer = new(typeof(BnrDataSet));

        return new BnrDocument
        {
            DataSet = (BnrDataSet)xmlSerializer.Deserialize(stream)
        };
    }
}