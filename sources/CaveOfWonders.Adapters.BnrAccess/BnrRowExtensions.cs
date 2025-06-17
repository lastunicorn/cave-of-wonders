// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

using System.Globalization;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles.BnrModels;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

internal static class BnrRowExtensions
{
    public static IEnumerable<BnrExchangeRate> ToExchangeRates(this BnrRow bnrRow)
    {
        CultureInfo cultureInfo = new("ro-RO");

        if (bnrRow.CursAud.Value != null && bnrRow.CursAud.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "AUDRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursAud.Value, cultureInfo)
            };
        }

        if (bnrRow.CursCad.Value != null && bnrRow.CursCad.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "CADRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursCad.Value, cultureInfo)
            };
        }

        if (bnrRow.CursChf.Value != null && bnrRow.CursChf.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "CHFRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursChf.Value, cultureInfo)
            };
        }

        if (bnrRow.CursCzk.Value != null && bnrRow.CursCzk.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "CZKRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursCzk.Value, cultureInfo)
            };
        }

        if (bnrRow.CursDkk.Value != null && bnrRow.CursDkk.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "DKKRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursDkk.Value, cultureInfo)
            };
        }

        if (bnrRow.CursEgp.Value != null && bnrRow.CursEgp.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "EGPRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursEgp.Value, cultureInfo)
            };
        }

        if (bnrRow.CursEur.Value != null && bnrRow.CursEur.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "EURRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursEur.Value, cultureInfo)
            };
        }

        if (bnrRow.CursGbp.Value != null && bnrRow.CursGbp.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "GBPRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursGbp.Value, cultureInfo)
            };
        }

        if (bnrRow.CursHuf.Value != null && bnrRow.CursHuf.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "HUFRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursHuf.Value, cultureInfo)
            };
        }

        if (bnrRow.CursJpy.Value != null && bnrRow.CursJpy.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "JPYRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursJpy.Value, cultureInfo)
            };
        }

        if (bnrRow.CursMdl.Value != null && bnrRow.CursMdl.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "MDLRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursMdl.Value, cultureInfo)
            };
        }

        if (bnrRow.CursNok.Value != null && bnrRow.CursNok.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "NOKRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursNok.Value, cultureInfo)
            };
        }

        if (bnrRow.CursPln.Value != null && bnrRow.CursPln.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "PLNRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursPln.Value, cultureInfo)
            };
        }

        if (bnrRow.CursSek.Value != null && bnrRow.CursSek.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "SEKRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursSek.Value, cultureInfo)
            };
        }

        if (bnrRow.CursTry.Value != null && bnrRow.CursTry.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "TRYRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursTry.Value, cultureInfo)
            };
        }

        if (bnrRow.CursUsd.Value != null && bnrRow.CursUsd.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "USDRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursUsd.Value, cultureInfo)
            };
        }

        if (bnrRow.CursXau.Value != null && bnrRow.CursXau.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "XAURON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursXau.Value, cultureInfo)
            };
        }

        if (bnrRow.CursXdr.Value != null && bnrRow.CursXdr.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "XDRRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursXdr.Value, cultureInfo)
            };
        }

        if (bnrRow.CursRub.Value != null && bnrRow.CursRub.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "RUBRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursRub.Value, cultureInfo)
            };
        }

        if (bnrRow.CursSkk.Value != null && bnrRow.CursSkk.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "SKKRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursSkk.Value, cultureInfo)
            };
        }

        if (bnrRow.CursBgn.Value != null && bnrRow.CursBgn.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "BGNRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursBgn.Value, cultureInfo)
            };
        }

        if (bnrRow.CursZar.Value != null && bnrRow.CursZar.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "ZARRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursZar.Value, cultureInfo)
            };
        }

        if (bnrRow.CursBrl.Value != null && bnrRow.CursBrl.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "BRLRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursBrl.Value, cultureInfo)
            };
        }

        if (bnrRow.CursCny.Value != null && bnrRow.CursCny.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "CNYRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursCny.Value, cultureInfo)
            };
        }

        if (bnrRow.CursInr.Value != null && bnrRow.CursInr.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "INRRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursInr.Value, cultureInfo)
            };
        }

        if (bnrRow.CursKrw.Value != null && bnrRow.CursKrw.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "KRWRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursKrw.Value, cultureInfo)
            };
        }

        if (bnrRow.CursMxn.Value != null && bnrRow.CursMxn.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "MXNRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursMxn.Value, cultureInfo)
            };
        }

        if (bnrRow.CursNzd.Value != null && bnrRow.CursNzd.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "NZDRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursNzd.Value, cultureInfo)
            };
        }

        if (bnrRow.CursRsd.Value != null && bnrRow.CursRsd.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "RSDRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursRsd.Value, cultureInfo)
            };
        }

        if (bnrRow.CursUah.Value != null && bnrRow.CursUah.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "UAHRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursUah.Value, cultureInfo)
            };
        }

        if (bnrRow.CursAed.Value != null && bnrRow.CursAed.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "AEDRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursAed.Value, cultureInfo)
            };
        }

        if (bnrRow.CursHrk.Value != null && bnrRow.CursHrk.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "HRKRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursHrk.Value, cultureInfo)
            };
        }

        if (bnrRow.CursThb.Value != null && bnrRow.CursThb.Value != "-")
        {
            yield return new BnrExchangeRate
            {
                CurrencyPair = "THBRON",
                Date = DateTime.Parse(bnrRow.Data, cultureInfo),
                Value = decimal.Parse(bnrRow.CursThb.Value, cultureInfo)
            };
        }
    }
}