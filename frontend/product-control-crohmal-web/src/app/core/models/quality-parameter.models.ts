import { AnalysisResult } from "./analysis-result.model";
import { ProductQualitySpecification } from "./product-quality-specification.model";

export interface QualityParameter {
  id: number;
  name: string;
  unit: string;
  description: string;
  isActive: boolean;
  productQualitySpecifications: ProductQualitySpecification[];
  analysisResults: AnalysisResult[];
}