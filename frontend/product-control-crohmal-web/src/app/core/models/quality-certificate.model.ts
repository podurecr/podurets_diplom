import { Batch } from './batch.model';
import { User } from './user.model';

export interface QualityCertificate {
  id: number;
  certificateNumber: string;

  batchId: number;
  batch?: Batch | null;

  createdAt: string;

  createdByUserId: number;
  createdByUser?: User | null;

  conclusion: string;
  pdfPath?: string | null;
}
