import { AnalysisResult } from "./analysis-result.model";
import { Batch } from "./batch.model";
import { QualityAssessment } from "./quality-assessment.model";
import { QualityCertificate } from "./quality-certificate.model";
import { Role } from "./role.model";
import { ShipmentDecision } from "./shipment-decision.model";

export interface User {
  id: number;
  fullName: string;
  login: string;
  email: string; // TODO 
  passwordHash: string;
  isActive: boolean;
  createdAt: Date;
  role: Role;
  roleName: string;
  createdBatches: Batch[];
  analysisResults: AnalysisResult[];
  qualityAssessments: QualityAssessment[];
  qualityCertificates: QualityCertificate[];
  shipmentDecisions: ShipmentDecision[];
}