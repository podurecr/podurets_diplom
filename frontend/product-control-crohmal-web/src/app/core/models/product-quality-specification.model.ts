import { Product } from './product.model';
import { QualityParameter } from './quality-parameter.models';

export interface ProductQualitySpecification {
  id: number;

  productId: number;
  product?: Product | null;

  qualityParameterId: number;
  qualityParameter?: QualityParameter | null;

  minValue?: number | null;
  maxValue?: number | null;
  textNorm?: string | null;

  isRequired: boolean;
}
