import { PotInstance } from "./pot-instance.model";

export interface PotResponse {
    date: Date;
    potInstances: PotInstance[];
}
