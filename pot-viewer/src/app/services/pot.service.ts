import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PotResponse } from '../models/pot-response.model';

@Injectable({
    providedIn: 'root'
})
export class PotService {
    private apiUrl = 'https://localhost:7272/pot';

    constructor(private http: HttpClient) {
    }

    getPots(currency?: string): Observable<PotResponse> {
        let params = new HttpParams();
        
        if (currency) {
            params = params.set('Currency', currency);
        }
        
        return this.http.get<PotResponse>(this.apiUrl, { params });
    }
}