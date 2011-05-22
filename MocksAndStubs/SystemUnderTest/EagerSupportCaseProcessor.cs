namespace MocksAndStubs.SystemUnderTest
{
    public class EagerSupportCaseProcessor : SupportCaseProcessor
    {
        public EagerSupportCaseProcessor(IMessagingService messagingService, IVerifier verifier, ISystemClock systemClock)
            : base(messagingService, verifier, systemClock)
        {
        }

        public override void Process(SupportCase supportCase)
        {
            _messagingService.Lock(true);
            _messagingService.RespondToClient(supportCase);
            _messagingService.EscalateToManagement(supportCase);
            _messagingService.Lock(false);
        }

        public override void MarkAsClosed(SupportCase supportCase, string finalizer, string finalizeMessage)
        {
            supportCase.Finalize(finalizer, finalizeMessage);
            _messagingService.RespondToClient(supportCase);
            _messagingService.WriteToLog(supportCase);
        }
    }
}