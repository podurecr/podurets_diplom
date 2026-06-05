import { Batch } from './batch.model';
import { User } from './user.model';

export interface QualityAssessment {
  id: number;
  batchId: number;
  batch: Batch;
  isApproved: boolean;
  conclusion: string;
  assessedAt: Date;
  assessedByUserId: number;
  assessedByUser: User;
  isFinalizing: boolean;
}
