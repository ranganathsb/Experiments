using System;

namespace MocksAndStubs.SystemUnderTest
{
    public class CorrectSupportCaseProcessor : SupportCaseProcessor
    {
        public CorrectSupportCaseProcessor(IMessagingService messagingService, IVerifier verifier, ISystemClock systemClock)
            : base(messagingService, verifier, systemClock)
        {
        }

        public override void Process(SupportCase supportCase)
        {
            _messagingService.RespondToClient(supportCase);
            if (supportCase.IsEscalated)
            {
                _messagingService.EscalateToManagement(supportCase);
            }
        }

        public override void MarkAsClosed(SupportCase supportCase, string finalizer, string finalizeMessage)
        {
            if (!_verifier.IsResolved(supportCase))
            {
                throw new ApplicationException("Case has not been verified as resolved.");
            }
            if (supportCase.IsFinalized)
            {
                return;
            }
            supportCase.Finalize(finalizer, finalizeMessage);
            _messagingService.RespondToClient(supportCase);
            _messagingService.WriteToLog(supportCase);
        }
    }
}