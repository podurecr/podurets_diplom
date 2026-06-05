import { Batch } from './batch.model';
import { QualityParameter } from './quality-parameter.models';
import { User } from './user.model';

export interface AnalysisResult {
  id: number;

  batchId: number;
  batch?: Batch | null;

  qualityParameterId: number;
  qualityParameter?: QualityParameter | null;

  numericValue?: number | null;
  textValue?: string | null;

  isWithinNorm?: boolean | null;
  resultStatus?: string | null;

  comment?: string | null;

  analyzedAt: string;

  enteredByUserId: number;
  enteredByUser?: User | null;
}
