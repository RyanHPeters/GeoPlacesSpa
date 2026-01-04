import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface MyLocationDto {
  source: string;
  ip: string | null;
  latitude: number;
  longitude: number;
  city?: string | null;
  region?: string | null;
  country?: string | null;
}

@Injectable({ providedIn: 'root' })
export class LocationService {
  constructor(private http: HttpClient) { }

  getMyLocation(): Observable<MyLocationDto> {
    return this.http.get<MyLocationDto>('/api/v1/location/me');
  }
}
