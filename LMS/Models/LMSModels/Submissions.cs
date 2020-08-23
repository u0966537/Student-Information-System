using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public uint SubmissionId { get; set; }
        public DateTime Time { get; set; }
        public ushort Score { get; set; }
        public string Contents { get; set; }
        public uint StudentId { get; set; }
        public uint AssignmentId { get; set; }

        public virtual Assignments Assignment { get; set; }
        public virtual Students Student { get; set; }
    }
}
