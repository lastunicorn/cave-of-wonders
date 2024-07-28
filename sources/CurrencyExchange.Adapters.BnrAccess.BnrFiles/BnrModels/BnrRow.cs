// Currency Exchange
// Copyright (C) 2023 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Xml.Serialization;

namespace DustInTheWind.CurrencyExchange.Adapters.BnrAccess.BnrFiles.BnrModels;

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