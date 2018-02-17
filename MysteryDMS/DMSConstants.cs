using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS
{
    public class DMSConstants
    {
        public const string wf_draft = "Draft";
        public const string wf_signed = "Signed";
        public const string wf_reviewed = "Reviewed";
        public const string wf_disapproved = "Disapproved";

        public const string wf_in_work = "In work";
        public const string wf_finalized = "Finalized";
        public const string wf_under_review = "Under review";
        public const string wf_reviewed_OK = "Reviewed - OK";
        public const string wf_reviewed_KO = "Reviewed - KO";
        public const string wf_approved = "Approved";
        public const string wf_approved_w_comments = "Approved with comments";
        public const string wf_disapproved_w_comments = "Disapproved with comments";
        public const string wf_obsolete = "Obsolete";

        public const string obsolete = "Obsolete";
        public const string draft = "draft";
        public const string current = "current";
        public const string superseded = "superseded";

        public const string simple_wf = "Simple Workflow";
        public const string complex_wf = "Complex Workflow";
    }
}
