using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class QualityAssessmentResultDecisionDTO
    {
        public int QualityParameterId { get; set; }

        public bool? IsWithinNorm { get; set; }
    }
}
