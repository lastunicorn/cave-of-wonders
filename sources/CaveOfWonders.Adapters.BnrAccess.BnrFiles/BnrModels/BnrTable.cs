using System.Xml.Serialization;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles.BnrModels;

public class BnrTable
{
    [XmlElement("Row")]
    public List<BnrRow> Rows { get; set; }
}