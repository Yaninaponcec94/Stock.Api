import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { StockItem } from '../../shared/models/stock-item.model';
import { CreateStockMovementDto } from '../../features/stock/models/create-stock-movement.dto';

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

  getMovements(productId?: number, page = 1, pageSize = 10) {
  const params: any = { page, pageSize };
  if (productId) params.productId = productId;

  return this.http.get<any>(`${this.baseUrl}/movements`, { params });
}
}