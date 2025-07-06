export interface Pot {
    id: number;
    name: string;
    description?: string;
    type?: string;
    size?: number;
    material?: string;
    color?: string;
    createdDate?: Date;
    // Add any other properties as needed based on the actual API response
}
