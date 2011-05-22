using System;
using MocksAndStubs.SystemUnderTest;
using NUnit.Framework;
using Rhino.Mocks;

namespace MocksAndStubs
{
    [TestFixture]
    public class Interaction_based_tests
    {
        private ISystemClock _systemClock;
        private ILogger _logger;
        private IMessagingService _messagingService;
        private SupportCase _supportCase;

        [SetUp]
        public void Setup()
        {
            _systemClock = MockRepository.GenerateStub<ISystemClock>();
            _logger = MockRepository.GenerateMock<ILogger>(); //a Mock, because it's key to the test assertions
            _messagingService = new MessagingService(_logger);
            _supportCase = new SupportCase("Interacting with the logger", _systemClock);
        }

        [Test]
        public void Should_send_case_to_logger()
        {
            _messagingService.WriteToLog(_supportCase);

            _logger.AssertWasCalled(l => l.WriteCase(Arg<CaseLog>.Is.Anything));
        }

        [Test]
        public void Should_send_case_log_containing_values_from_case_to_logger()
        {
            var expectedFinalizeDate = new DateTime(2009, 7, 21);
            _systemClock.Stub(c => c.Now).Return(expectedFinalizeDate);
            _supportCase.Finalize("finalizer", "this is done");

            _messagingService.WriteToLog(_supportCase);

            _logger.AssertWasCalled(
                l => l.WriteCase(Arg<CaseLog>
                                     .Matches(c =>
                                              c.DateFinalized == expectedFinalizeDate &&
                                              c.Description == _supportCase.Description &&
                                              c.FinalizeMessage == _supportCase.FinalizeMessage &&
                                              c.Finalizer == _supportCase.Finalizer &&
                                              c.IsFinalized == true &&
                                              c.IsEscalated == _supportCase.IsEscalated)));
        }
    }

    [TestFixture]
    public class State_based_with_hand_made_mock
    {
        private ISystemClock _systemClock;
        private MockLogger _logger;
        private IMessagingService _messagingService;
        private SupportCase _supportCase;
        
        [SetUp]
        public void Setup()
        {
            _systemClock = MockRepository.GenerateStub<ISystemClock>();
            _logger = new MockLogger();
            _messagingService = new MessagingService(_logger);
            _supportCase = new SupportCase("Looking at the logs", _systemClock);
        }

        [Test]
        public void Should_send_a_case_log_created_from_the_case_to_the_logger()
        {
            var expectedFinalizeDate = new DateTime(2009, 7, 21);
            _systemClock.Stub(c => c.Now).Return(expectedFinalizeDate);
            _supportCase.Finalize("finalizer", "this is done");

            _messagingService.WriteToLog(_supportCase);

            Assert.That(_logger.WriteCaseWasCalled, Is.EqualTo(1));
            Assert.That(_logger.DateFinalized, Is.EqualTo(expectedFinalizeDate));
            Assert.That(_logger.Description, Is.EqualTo(_supportCase.Description));
            Assert.That(_logger.FinalizeMessage, Is.EqualTo(_supportCase.FinalizeMessage));
            Assert.That(_logger.Finalizer, Is.EqualTo(_supportCase.Finalizer));
            Assert.That(_logger.IsFinalized, Is.True);
            Assert.That(_logger.IsEscalated, Is.EqualTo(_supportCase.IsEscalated));
        }

        private class MockLogger : ILogger
        {
            public int WriteCaseWasCalled { get; set; }
            public bool IsFinalized { get { return _caseLog.IsFinalized; } }
            public string Finalizer { get { return _caseLog.Finalizer; } }
            public DateTime? DateFinalized { get { return _caseLog.DateFinalized; } }
            public string Description { get { return _caseLog.Description; } }
            public bool IsEscalated { get { return _caseLog.IsEscalated; } }
            public string FinalizeMessage { get { return _caseLog.FinalizeMessage; } }

            private CaseLog _caseLog;

            public void WriteCase(CaseLog caseLog)
            {
                WriteCaseWasCalled++;
                _caseLog = caseLog;
            }
        }
    }
}