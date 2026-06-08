using System.Xml.Serialization;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles.BnrModels;

[XmlRoot("DataSet", Namespace = "http://www.bnr.ro/xsd")]
public class BnrDataSet
{
    public string NumeClasaStatistica { get; set; }

    public string ObservatiiClasaStatistica { get; set; }

    public string NotaClasaStatistica { get; set; }

    public string Metodologie { get; set; }

    public BnrTable Table { get; set; }
}