using System.Xml.Serialization;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles;

public class NbrDocument
{
    public NbrDataSet DataSet { get; private set; }

    public static NbrDocument Load(Stream stream)
    {
        XmlSerializer xmlSerializer = new(typeof(NbrDataSet));

        return new NbrDocument
        {
            DataSet = (NbrDataSet)xmlSerializer.Deserialize(stream)
        };
    }
}