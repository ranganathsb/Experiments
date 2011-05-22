using System;

namespace MocksAndStubs.SystemUnderTest
{
    public class CaseLog
    {
        public bool IsFinalized { get; set; }
        public string Finalizer { get; set; }
        public DateTime? DateFinalized { get; set; }
        public string Description { get; set; }
        public bool IsEscalated { get; set; }
        public string FinalizeMessage { get; set; }
    }
}