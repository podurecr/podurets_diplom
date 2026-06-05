export interface CreateBatchRequest {
  batchNumber: string;
  productionDate: string;
  quantity: number;
  unit: string;
  productionLine?: string;
  comment?: string;
  productId: number;
  createdByUserId: number;
}
