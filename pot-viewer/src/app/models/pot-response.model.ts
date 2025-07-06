import { MonetaryValue, PotInstance } from "./pot-instance.model";

export interface ConversionRate {
    sourceCurrency: string;
    destinationCurrency: string;
    date: string | Date;
    value: number;
}

export interface CurrencyTotalOverview {
    value: MonetaryValue;
    normalizedValue: MonetaryValue;
    percentage: number;
}

export interface PotResponse {
    date: Date;
    potInstances: PotInstance[];
    conversionRates: ConversionRate[];
    total: MonetaryValue;
    currencyTotalOverviews: CurrencyTotalOverview[];
}
