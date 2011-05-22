using System;

namespace MocksAndStubs.SystemUnderTest
{
    public abstract class SupportCaseProcessor
    {
        protected IMessagingService _messagingService;
        protected IVerifier _verifier;
        private ISystemClock _systemClock;

        public SupportCaseProcessor(IMessagingService messagingService, IVerifier verifier, ISystemClock systemClock)
        {
            _messagingService = messagingService;
            _verifier = verifier;
            _systemClock = systemClock;
        }

        public SupportCase InitiateSupportCase(string description)
        {
            return new SupportCase(description, _systemClock);
        }

        public abstract void Process(SupportCase supportCase);
        public abstract void MarkAsClosed(SupportCase supportCase, string finalizer, string finalizeMessage);
    }
}