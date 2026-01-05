import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

export interface PlaceDto {
  id: string;
  name: string;
  category: string;
  latitude: number;
  longitude: number;
  createdAt: string;
}

export interface NearbyPlaceDto {
  id: string;
  name: string;
  category: string;
  distance_meters: number | null;
}

export interface CreatePlaceRequest {
  name: string;
  category: string;
  latitude: number;
  longitude: number;
}

export interface GetPlacesQuery {
  page?: number;
  pageSize?: number;
  category?: string | null;
  q?: string | null;
  sort?: string | null;   // e.g. 'name' | 'category' | 'createdAt'
  order?: string | null;  // 'asc' | 'desc'
}

export interface PagedPlacesResult {
  items: PlaceDto[];
  totalCount: number;
  linkHeader: string | null;
}

@Injectable({ providedIn: 'root' })
export class PlacesService {
  private readonly baseUrl = '/api/v1/places';

  constructor(private http: HttpClient) { }

  getAll(query: GetPlacesQuery = {}): Observable<PagedPlacesResult> {
    let params = new HttpParams();

    if (query.page != null) params = params.set('page', query.page);
    if (query.pageSize != null) params = params.set('pageSize', query.pageSize);

    if (query.category) params = params.set('category', query.category);
    if (query.q) params = params.set('q', query.q);

    if (query.sort) params = params.set('sort', query.sort);
    if (query.order) params = params.set('order', query.order);

    // Backend returns PlaceDto[] in the body and paging metadata in headers.
    return this.http.get<PlaceDto[]>(`${this.baseUrl}/all`, { params, observe: 'response' })
      .pipe(
        map((resp: HttpResponse<PlaceDto[]>) => {
          const total = Number(resp.headers.get('X-Total-Count') ?? '0');
          const link = resp.headers.get('Link');
          return {
            items: resp.body ?? [],
            totalCount: Number.isFinite(total) ? total : 0,
            linkHeader: link
          };
        })
      );
  }

  getNearby(lat: number, lng: number, radiusMeters: number): Observable<NearbyPlaceDto[]> {
    const params = new HttpParams()
      .set('latitude', lat)
      .set('longitude', lng)
      .set('radiusMeters', radiusMeters);

    return this.http.get<NearbyPlaceDto[]>(`${this.baseUrl}/nearby`, { params });
  }

  createPlace(req: CreatePlaceRequest): Observable<PlaceDto> {
    return this.http.post<PlaceDto>(this.baseUrl, req);
  }
}
