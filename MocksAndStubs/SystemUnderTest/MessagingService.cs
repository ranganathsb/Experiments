using System;

namespace MocksAndStubs.SystemUnderTest
{
    public class MessagingService : IMessagingService
    {
        private readonly ILogger _logger;
        private bool _isLocked;

        public MessagingService(ILogger logger)
        {
            _logger = logger;
        }

        public void RespondToClient(SupportCase supportCase)
        {
            throw new NotImplementedException();
        }

        public void EscalateToManagement(SupportCase supportCase)
        {
            throw new NotImplementedException();
        }

        public void WriteToLog(SupportCase supportCase)
        {
            _logger.WriteCase(
                new CaseLog
                    {
                        IsFinalized = supportCase.IsFinalized,
                        DateFinalized = supportCase.DateFinalized,
                        Description = supportCase.Description,
                        FinalizeMessage = supportCase.FinalizeMessage,
                        Finalizer = supportCase.Finalizer,
                        IsEscalated = supportCase.IsEscalated
                    });
        }

        public void Lock(bool isLocked)
        {
            _isLocked = isLocked;
        }
    }
}