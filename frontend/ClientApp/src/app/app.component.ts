import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { PlacesService, PlaceDto, NearbyPlaceDto, CreatePlaceRequest } from './places.service';
import { LocationService, MyLocationDto } from './location.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  myLocation: MyLocationDto | null = null;

  allPlaces: PlaceDto[] = [];
  nearbyPlaces: NearbyPlaceDto[] = [];

  radiuses = [
    { label: '250 m', value: 250 },
    { label: '500 m', value: 500 },
    { label: '1 km', value: 1000 },
    { label: '2 km', value: 2000 },
    { label: '5 km', value: 5000 }
  ];
  selectedRadius = 1000;

  loadingAll = false;
  loadingNearby = false;
  totalCount = 0;
  page = 1;
  pageSize = 10;

  // --- Create form state ---
  createName = '';
  createCategory = '';
  createLatitude: number | null = null;
  createLongitude: number | null = null;
  creating = false;
  createError: string | null = null;
  createSuccess = false;

  get totalPages(): number {
    const pages = Math.ceil((this.totalCount || 0) / this.pageSize);
    return Math.max(1, pages);
  }

  constructor(
    private places: PlacesService,
    private location: LocationService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Show location immediately on load.
    this.loadMyLocation();

    // Common UX: also load the first page automatically.
    this.loadAllPlaces();
  }

  prefillFromMyLocation(): void {
    if (!this.myLocation) return;
    this.createLatitude = this.myLocation.latitude;
    this.createLongitude = this.myLocation.longitude;
    this.cdr.detectChanges();
  }

  createPlace(): void {
    this.createError = null;
    this.createSuccess = false;

    const name = (this.createName ?? '').trim();
    const category = (this.createCategory ?? '').trim();
    const lat = this.createLatitude;
    const lng = this.createLongitude;

    if (!name || !category || lat == null || lng == null) {
      this.createError = 'Name, category, latitude, and longitude are required.';
      this.cdr.detectChanges();
      return;
    }

    if (lat < -90 || lat > 90 || lng < -180 || lng > 180) {
      this.createError = 'Latitude must be between -90 and 90; longitude must be between -180 and 180.';
      this.cdr.detectChanges();
      return;
    }

    const req: CreatePlaceRequest = {
      name,
      category,
      latitude: lat,
      longitude: lng
    };

    this.creating = true;
    this.cdr.detectChanges();

    this.places.createPlace(req).subscribe({
      next: _ => {
        this.createSuccess = true;
        this.createName = '';
        this.createCategory = '';
        // Keep lat/lng as-is so user can add multiple nearby points quickly.
        // If you prefer clearing, set them to null here.

        // Refresh current page so the new record appears.
        this.loadAllPlaces();
      },
      error: err => {
        console.error(err);
        // Try to surface server-provided ProblemDetails message if present.
        const detail = err?.error?.detail;
        this.createError = typeof detail === 'string' && detail.length ? detail : 'Failed to create place.';
      },
      complete: () => {
        this.creating = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadMyLocation() {
    this.location.getMyLocation().subscribe({
      next: loc => {
        this.myLocation = loc;
        // Defensive: ensure the UI updates even if something runs outside Angular.
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.cdr.detectChanges();
      }
    });
  }

  loadAllPlaces() {
    this.loadingAll = true;
    this.places.getAll({ page: this.page, pageSize: this.pageSize }).subscribe({
      next: data => {
        this.allPlaces = data.items;
        this.totalCount = data.totalCount; 

        // Keep page in range if total count changes (e.g., filters later).
        if (this.page > this.totalPages) {
          this.page = this.totalPages;
        }

        this.cdr.detectChanges();
      },
      error: err => { console.error(err); },
      complete: () => {
        this.loadingAll = false;
        this.cdr.detectChanges();
      }
    });
  }

  prevPage(): void {
    if (this.page <= 1) return;
    this.page--;
    this.loadAllPlaces();
  }

  nextPage(): void {
    if (this.page >= this.totalPages) return;
    this.page++;
    this.loadAllPlaces();
  }

  onPageSizeChanged(): void {
    this.page = 1;
    this.loadAllPlaces();
  }

  loadNearbyPlaces() {
    this.loadingNearby = true;

    const runQuery = (loc: MyLocationDto) => {
      this.places.getNearby(loc.latitude, loc.longitude, this.selectedRadius).subscribe({
        next: data => {
          this.nearbyPlaces = data;
          this.cdr.detectChanges();
        },
        error: err => {
          console.error(err);
          this.cdr.detectChanges();
        },
        complete: () => {
          this.loadingNearby = false;
          this.cdr.detectChanges();
        }
      });
    };

    // If we already loaded location on init, reuse it.
    if (this.myLocation) {
      runQuery(this.myLocation);
      return;
    }

    // Otherwise fetch it once and then query.
    this.location.getMyLocation().subscribe({
      next: loc => {
        this.myLocation = loc;
        this.cdr.detectChanges();
        runQuery(loc);
      },
      error: err => {
        console.error(err);
        this.loadingNearby = false;
        this.cdr.detectChanges();
      }
    });
  }
}
