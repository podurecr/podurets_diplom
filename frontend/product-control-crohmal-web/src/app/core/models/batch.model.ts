import { Product } from './product.model';
import { BatchStatus } from './batch-status.enum';
import { AnalysisResult } from './analysis-result.model';
import { User } from './user.model';
import { QualityAssessment } from './quality-assessment.model';
import { QualityCertificate } from './quality-certificate.model';
import { ShipmentDecision } from './shipment-decision.model';

export interface Batch {
  id: number;
  batchNumber: string;
  productionDate: string;
  quantity: number;
  unit: string;
  productionLine?: string;
  comment?: string;
  status: BatchStatus;
  createdAt: string;
  productId: number;
  product?: Product;
  createdByUserId: number;
  createdByUser?: User;
  analysisResults?: AnalysisResult[];
  qualityAssessment?: QualityAssessment;
  qualityCertificate?: QualityCertificate;
  shipmentDecision?: ShipmentDecision;
  isAnalysisCompleted: boolean;
  analysisCompletedAt?: string | null;
  analysisCompletedByUserId?: number | null;
  isFinishing: boolean;
}
