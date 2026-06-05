export interface Product {
  id?: number;
  code: string;
  name: string;
  unit: string;
  description?: string;
  isActive: boolean;
}