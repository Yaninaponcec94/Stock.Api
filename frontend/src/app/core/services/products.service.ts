import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product } from '../../shared/models/product.model';
import { PagedResult } from '../../shared/models/paged-result.model';
import { CreateProductDto } from '../../features/products/models/create-product.dto';
import { UpdateProductDto } from '../../features/products/models/update-product.dto';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly baseUrl = `${environment.apiUrl}/api/products`;

  constructor(private http: HttpClient) {}

  getProducts(params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
  }) {
    let httpParams = new HttpParams();

    if (params?.page != null) httpParams = httpParams.set('page', params.page);
    if (params?.pageSize != null) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params?.search) httpParams = httpParams.set('name', params.search);

    if (params?.isActive != null) httpParams = httpParams.set('isActive', params.isActive);

    return this.http.get<PagedResult<Product>>(this.baseUrl, { params: httpParams });
  }

  createProduct(dto: CreateProductDto) {
  return this.http.post<Product>(this.baseUrl, dto);
  }
  
  updateProduct(id: number, dto: UpdateProductDto) {
      return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  softDeleteProduct(id: number) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getProductById(id: number) {
  return this.http.get<Product>(`${this.baseUrl}/${id}`);
}


}
