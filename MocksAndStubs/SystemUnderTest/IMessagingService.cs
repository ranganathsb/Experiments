namespace MocksAndStubs.SystemUnderTest
{
    public interface IMessagingService
    {
        void RespondToClient(SupportCase supportCase);
        void EscalateToManagement(SupportCase supportCase);
        void WriteToLog(SupportCase supportCase);
        void Lock(bool isLocked);
    }
}