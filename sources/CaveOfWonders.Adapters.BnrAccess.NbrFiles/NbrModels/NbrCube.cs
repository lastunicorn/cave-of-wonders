using System.Xml.Serialization;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;

public class NbrCube
{
    [XmlAttribute("date")]
    public string Date { get; set; }

    [XmlElement("Rate")]
    public List<NbrRate> Rates { get; set; }
}