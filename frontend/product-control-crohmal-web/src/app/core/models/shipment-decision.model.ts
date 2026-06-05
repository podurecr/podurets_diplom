import { Batch } from './batch.model';
import { User } from './user.model';

export interface ShipmentDecision {
  id: number;

  batchId: number;
  batch?: Batch | null;

  status: string | number;

  decisionText: string;

  createdAt: string;

  createdByUserId: number;
  createdByUser?: User | null;
}
