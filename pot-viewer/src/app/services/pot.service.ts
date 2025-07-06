import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pot } from '../models/pot.model';

@Injectable({
    providedIn: 'root'
})
export class PotService {
    private apiUrl = 'https://localhost:7272/pot';

    constructor(private http: HttpClient) {
    }

    getPots(): Observable<Pot[]> {
        return this.http.get<Pot[]>(this.apiUrl);
    }

    getPot(id: number): Observable<Pot> {
        return this.http.get<Pot>(`${this.apiUrl}/${id}`);
    }
}
