using System.Xml.Serialization;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;

[XmlRoot("DataSet", Namespace = "http://www.bnr.ro/xsd")]
public class NbrDataSet
{
    public NbrHeader Header { get; set; }

    public NbrBody Body { get; set; }
}