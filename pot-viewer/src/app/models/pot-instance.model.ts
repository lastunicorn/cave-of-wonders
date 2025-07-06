export interface PotInstance {
    name: string;
    isActive: boolean;
    balance: number;
    currency: string;
    accountNumber?: string;
    lastUpdated?: Date;
}

