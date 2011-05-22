using System;

namespace MocksAndStubs.SystemUnderTest
{
    public interface ISystemClock
    {
        DateTime Now { get; }
    }
}