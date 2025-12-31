export interface StockItem {
  productId: number;
  productName: string;
  quantity: number;
  updatedAt: string;
  minStock: number;
  isBelowMinStock: boolean;
}
