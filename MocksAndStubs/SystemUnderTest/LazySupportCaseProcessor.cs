namespace MocksAndStubs.SystemUnderTest
{
    public class LazySupportCaseProcessor : SupportCaseProcessor
    {
        public LazySupportCaseProcessor(IMessagingService messagingService, IVerifier verifier, ISystemClock systemClock)
            : base(messagingService, verifier, systemClock)
        {
        }

        public override void Process(SupportCase supportCase)
        {
            _messagingService.RespondToClient(supportCase);
        }

        public override void MarkAsClosed(SupportCase supportCase, string finalizer, string finalizeMessage)
        {
        }
    }
}