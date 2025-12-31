export type MovementType = 'Entry' | 'Exit' | 'Adjustment';

export interface CreateStockMovementDto {
  productId: number;
  type: MovementType;
  quantity: number;
  reason?: string;
}