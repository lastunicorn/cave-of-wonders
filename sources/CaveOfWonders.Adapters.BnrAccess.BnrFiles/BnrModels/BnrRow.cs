using System.Xml.Serialization;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles.BnrModels;

public class BnrRow
{
    public string Data { get; set; }

    [XmlElement("CURSZ_AUD")]
    public BnrCursEur CursAud { get; set; }

    [XmlElement("CURSZ_CAD")]
    public BnrCursEur CursCad { get; set; }

    [XmlElement("CURSZ_CHF")]
    public BnrCursEur CursChf { get; set; }

    [XmlElement("CURSZ_CZK")]
    public BnrCursEur CursCzk { get; set; }

    [XmlElement("CURSZ_DKK")]
    public BnrCursEur CursDkk { get; set; }

    [XmlElement("CURSZ_EGP")]
    public BnrCursEur CursEgp { get; set; }

    [XmlElement("CURSZ_EUR")]
    public BnrCursEur CursEur { get; set; }

    [XmlElement("CURSZ_GBP")]
    public BnrCursEur CursGbp { get; set; }

    [XmlElement("CURSZ_HUF")]
    public BnrCursEur CursHuf { get; set; }

    [XmlElement("CURSZ_JPY")]
    public BnrCursEur CursJpy { get; set; }

    [XmlElement("CURSZ_MDL")]
    public BnrCursEur CursMdl { get; set; }

    [XmlElement("CURSZ_NOK")]
    public BnrCursEur CursNok { get; set; }

    [XmlElement("CURSZ_PLN")]
    public BnrCursEur CursPln { get; set; }

    [XmlElement("CURSZ_SEK")]
    public BnrCursEur CursSek { get; set; }

    [XmlElement("CURSZ_TRY")]
    public BnrCursEur CursTry { get; set; }

    [XmlElement("CURSZ_USD")]
    public BnrCursEur CursUsd { get; set; }

    [XmlElement("CURSZ_XAU")]
    public BnrCursEur CursXau { get; set; }

    [XmlElement("CURSZ_XDR")]
    public BnrCursEur CursXdr { get; set; }

    [XmlElement("CURSZ_RUB")]
    public BnrCursEur CursRub { get; set; }

    [XmlElement("CURSZ_SKK")]
    public BnrCursEur CursSkk { get; set; }

    [XmlElement("CURSZ_BGN")]
    public BnrCursEur CursBgn { get; set; }

    [XmlElement("CURSZ_ZAR")]
    public BnrCursEur CursZar { get; set; }

    [XmlElement("CURSZ_BRL")]
    public BnrCursEur CursBrl { get; set; }

    [XmlElement("CURSZ_CNY")]
    public BnrCursEur CursCny { get; set; }

    [XmlElement("CURSZ_INR")]
    public BnrCursEur CursInr { get; set; }

    [XmlElement("CURSZ_KRW")]
    public BnrCursEur CursKrw { get; set; }

    [XmlElement("CURSZ_MXN")]
    public BnrCursEur CursMxn { get; set; }

    [XmlElement("CURSZ_NZD")]
    public BnrCursEur CursNzd { get; set; }

    [XmlElement("CURSZ_RSD")]
    public BnrCursEur CursRsd { get; set; }

    [XmlElement("CURSZ_UAH")]
    public BnrCursEur CursUah { get; set; }

    [XmlElement("CURSZ_AED")]
    public BnrCursEur CursAed { get; set; }

    [XmlElement("CURSZ_HRK")]
    public BnrCursEur CursHrk { get; set; }

    [XmlElement("CURSZ_THB")]
    public BnrCursEur CursThb { get; set; }
}