using System.Xml.Serialization;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;

public class NbrRate
{
    [XmlAttribute("currency")]
    public string Currency { get; set; }

    [XmlAttribute("multiplier")]
    public string Multiplier { get; set; }

    [XmlText]
    public string Value { get; set; }
}