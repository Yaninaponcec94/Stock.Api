import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface StockItem {
  productId: number;
  productName: string;
  quantity: number;
  updatedAt: string;
  minStock: number;
  isBelowMinStock: boolean;
}

export type MovementType = 'Entry' | 'Exit' | 'Adjustment';

export interface CreateStockMovementDto {
  productId: number;
  type: MovementType;
  quantity: number;
  reason?: string;
}

@Injectable({ providedIn: 'root' })
export class StockService {
  private readonly baseUrl = `${environment.apiUrl}/api/stock`;

  constructor(private http: HttpClient) {}

  getStock() {
    return this.http.get<StockItem[]>(this.baseUrl);
  }

  entry(dto: Omit<CreateStockMovementDto, 'type'> & { type?: never }) {
    return this.http.post(`${this.baseUrl}/entry`, { ...dto, type: 'Entry' as const });
  }

  exit(dto: Omit<CreateStockMovementDto, 'type'> & { type?: never }) {
    return this.http.post(`${this.baseUrl}/exit`, { ...dto, type: 'Exit' as const });
  }

  adjustment(dto: Omit<CreateStockMovementDto, 'type'> & { type?: never }) {
    return this.http.post(`${this.baseUrl}/adjustment`, { ...dto, type: 'Adjustment' as const });
  }

}
