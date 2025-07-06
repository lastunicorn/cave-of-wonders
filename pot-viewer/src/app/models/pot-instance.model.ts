export interface MonetaryValue {
    value: number;
    currency: string;
    date: string | Date;
}

export interface PotInstance {
    id: string;
    name: string;
    isActive: boolean;
    value: MonetaryValue;
    normalizedValue: MonetaryValue;
    accountNumber?: string;
}

