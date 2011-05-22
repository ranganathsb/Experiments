using System;
using System.Collections.Generic;
using MocksAndStubs.SystemUnderTest;
using NUnit.Framework;
using Rhino.Mocks;

namespace MocksAndStubs
{
    public abstract class SupportCaseProcessorTester
    {
        protected ISystemClock SystemClock;
        protected IMessagingService MessagingService;
        protected IVerifier Verifier;
        protected SupportCase SupportCase;
        protected SupportCaseProcessor SupportCaseProcessor;

        [Test]
        public void Should_notify_management_when_case_is_escalated()
        {
            SupportCase = SupportCaseProcessor.InitiateSupportCase("Make the corners rounded.");
            SupportCase.IsEscalated = true;

            SupportCaseProcessor.Process(SupportCase);

            MessagingService.AssertWasCalled(m => m.EscalateToManagement(SupportCase));
        }

        [Test, Explicit("Expect this to fail when using mocks, and it used to pass (incorrectly) when using stubs, but stubs now verify their assertions in RM 3.6.")]
        public void Should_not_notify_management_when_case_is_not_escalated()
        {
            SupportCase = SupportCaseProcessor.InitiateSupportCase("Make the corners rounded.");
            SupportCaseProcessor.Process(SupportCase);

            MessagingService.AssertWasNotCalled(m => m.EscalateToManagement(Arg<SupportCase>.Is.Anything));
        }
    }

    [TestFixture]
    public class When_using_stubs : SupportCaseProcessorTester
    {
        [SetUp]
        public void SetUp()
        {
            MessagingService = MockRepository.GenerateStub<IMessagingService>();
            Verifier = MockRepository.GenerateStub<IVerifier>();
            SystemClock = MockRepository.GenerateStub<ISystemClock>();
//            SupportCaseProcessor = new CorrectSupportCaseProcessor(MessagingService, Verifier, SystemClock);
            SupportCaseProcessor = new EagerSupportCaseProcessor(MessagingService, Verifier, SystemClock);
//            SupportCaseProcessor = new LazySupportCaseProcessor(MessagingService, Verifier, SystemClock);
        }
     }
    [TestFixture]
    public class When_using_mocks : SupportCaseProcessorTester
    {
        [SetUp]
        public void SetUp()
        {
            MessagingService = MockRepository.GenerateMock<IMessagingService>();
            Verifier = MockRepository.GenerateMock<IVerifier>();
            SystemClock = MockRepository.GenerateMock<ISystemClock>();
//            SupportCaseProcessor = new CorrectSupportCaseProcessor(MessagingService, Verifier, SystemClock);
            SupportCaseProcessor = new EagerSupportCaseProcessor(MessagingService, Verifier, SystemClock);
//            SupportCaseProcessor = new LazySupportCaseProcessor(MessagingService, Verifier, SystemClock);
        }

        [Test]
        public void Multiple_calls_without_caring_about_args()
        {
            SupportCase = SupportCaseProcessor.InitiateSupportCase("Make the corners rounded.");

            SupportCaseProcessor.Process(SupportCase);

            MessagingService.AssertWasCalled(m => m.Lock(Arg<bool>.Is.Anything), options => options.Repeat.Twice());
        }

        [Test]
        public void Multiple_calls_with_distinct_args_in_any_order()
        {
            SupportCase = SupportCaseProcessor.InitiateSupportCase("Make the corners rounded.");

            SupportCaseProcessor.Process(SupportCase);

            //They're actually called in the other order, m.Lock(true) coming first.
            MessagingService.AssertWasCalled(m => m.Lock(false));
            MessagingService.AssertWasCalled(m => m.Lock(true));
        }

        [Test]
        public void Multiple_calls_with_distinct_args_in_specific_order()
        {
            SupportCase = SupportCaseProcessor.InitiateSupportCase("Make the corners rounded.");

            SupportCaseProcessor.Process(SupportCase);

            IList<object[]> args = MessagingService.GetArgumentsForCallsMadeOn(m => m.Lock(Arg<bool>.Is.Anything));
            Assert.That(args[0][0], Is.True);
            Assert.That(args[1][0], Is.False);
        }
    }
}