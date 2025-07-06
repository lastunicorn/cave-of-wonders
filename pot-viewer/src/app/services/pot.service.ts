import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PotResponse } from '../models/pot-response.model';

@Injectable({
    providedIn: 'root'
})
export class PotService {
    private apiUrl = 'https://localhost:7272/pot';

    constructor(private http: HttpClient) {
    }

    getPots(): Observable<PotResponse> {
        return this.http.get<PotResponse>(this.apiUrl);
    }
}