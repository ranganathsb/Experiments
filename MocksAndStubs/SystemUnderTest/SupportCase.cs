using System;

namespace MocksAndStubs.SystemUnderTest
{
    public class SupportCase
    {
        private readonly ISystemClock _systemClock;
        public string Description { get; private set; }
        public DateTime DateCreated { get; private set; }
        public bool IsEscalated { get; set; }
        public bool IsFinalized { get; private set; }
        public DateTime? DateFinalized { get; private set; }
        public string Finalizer { get; private set; }
        public string FinalizeMessage { get; private set; }

        public SupportCase(string description, ISystemClock systemClock)
        {
            _systemClock = systemClock;
            Description = description;
            DateCreated = systemClock.Now;
        }

        public void Finalize(string finalizer, string finalizeMessage)
        {
            IsFinalized = true;
            DateFinalized = _systemClock.Now;
            Finalizer = finalizer;
            FinalizeMessage = finalizeMessage;
        }
    }
}
