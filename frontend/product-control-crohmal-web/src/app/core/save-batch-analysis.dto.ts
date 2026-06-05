export interface SaveBatchAnalysisRequest {
  batchId: number;
  results: SaveAnalysisResultItem[];
}

export interface SaveAnalysisResultItem {
  qualityParameterId: number;
  numericValue?: number | null;
  textValue?: string | null;
  isWithinNorm?: boolean | null;
  resultStatus?: string | null;
  comment?: string | null;
}
