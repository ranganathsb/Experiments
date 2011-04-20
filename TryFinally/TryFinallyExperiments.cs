using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace TryFinally
{
    public abstract class TryFinallyExperiments
    {
        private IInteract _interactor;
        private INeedClosing _closer;
        private Worker _worker;

        [TestFixture]
        public class When_the_interactor_works_successfully : TryFinallyExperiments
        {
            [SetUp]
            public void Setup()
            {
                _interactor = MockRepository.GenerateStub<IInteract>();
                _closer = MockRepository.GenerateMock<INeedClosing>();
                _worker = new Worker(_interactor, _closer);
            }

            [Test]
            public void ReturnDirectly_will_return_from_the_try_and_close_the_closer()
            {
                var result = _worker.ReturnDirectly();

                Assert.That(result, Is.EqualTo(ExitedFrom.Try));
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFF_will_return_from_the_try_and_close_the_closer()
            {
                var result = _worker.ReturnAtEnd_FavoringFinally();

                Assert.That(result, Is.EqualTo(ExitedFrom.Try));
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFC_will_return_from_the_try_and_close_the_closer()
            {
                var result = _worker.ReturnAtEnd_FavoringCatch();

                Assert.That(result, Is.EqualTo(ExitedFrom.Try));
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ClosingTwice_will_close_only_once()
            {
                var result = _worker.ClosingTwice();

                Assert.That(result, Is.EqualTo(ExitedFrom.Try));
                _closer.AssertWasCalled(c => c.Close(), opt => opt.Repeat.Once());
            }

            [Test]
            public void Incrementor_will_increment_three_times()
            {
                var result = _worker.Incrementor();

                Assert.That(result, Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class When_the_interactor_throws : TryFinallyExperiments
        {
            [SetUp]
            public void Setup()
            {
                _interactor = MockRepository.GenerateStub<IInteract>();
                _interactor.Stub(i => i.React()).Throw(new InteractorException());
                _closer = MockRepository.GenerateMock<INeedClosing>();
                _worker = new Worker(_interactor, _closer);
            }

            [Test]
            public void ReturnDirectly_will_return_from_the_catch_and_close_the_closer()
            {
                var result = _worker.ReturnDirectly();

                Assert.That(result, Is.EqualTo(ExitedFrom.Catch));
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFF_will_return_from_the_finally_and_close_the_closer()
            {
                var result = _worker.ReturnAtEnd_FavoringFinally();

                Assert.That(result, Is.EqualTo(ExitedFrom.Finally));
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFC_will_return_from_the_finally_and_close_the_closer()
            {
                var result = _worker.ReturnAtEnd_FavoringCatch();

                Assert.That(result, Is.EqualTo(ExitedFrom.Finally));
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ClosingTwice_will_close_twice()
            {
                var result = _worker.ClosingTwice();

                Assert.That(result, Is.EqualTo(ExitedFrom.Catch));
                _closer.AssertWasCalled(c => c.Close(), opt => opt.Repeat.Twice());
            }

            [Test]
            public void Incrementor_will_increment_three_times()
            {
                var result = _worker.Incrementor();

                Assert.That(result, Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class When_interactor_succeeds_but_the_closer_throws : TryFinallyExperiments
        {
            [SetUp]
            public void Setup()
            {
                _interactor = MockRepository.GenerateStub<IInteract>();
                _closer = MockRepository.GenerateMock<INeedClosing>();
                _closer.Stub(c => c.Close()).Throw(new CloserException());
                _worker = new Worker(_interactor, _closer);
            }

            [Test]
            public void ReturnDirectly_will_bubble_up_the_exception_but_close_was_called()
            {
                Assert.Throws<CloserException>(() => _worker.ReturnDirectly());
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFF_will_throw_but_close_the_closer()
            {
                Assert.Throws<CloserException>(() => _worker.ReturnAtEnd_FavoringFinally());
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFC_will_throw_but_close_the_closer()
            {
                Assert.Throws<CloserException>(() => _worker.ReturnAtEnd_FavoringCatch());
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ClosingTwice_will_close_once_because_it_skipped_the_catch_block()
            {
                Assert.Throws<CloserException>(() => _worker.ClosingTwice());
                _closer.AssertWasCalled(c => c.Close(), opt => opt.Repeat.Once());
            }
        }

        [TestFixture]
        public class When_both_interactor_and_closer_throw : TryFinallyExperiments
        {
            [SetUp]
            public void Setup()
            {
                _interactor = MockRepository.GenerateStub<IInteract>();
                _interactor.Stub(i => i.React()).Throw(new InteractorException());
                _closer = MockRepository.GenerateMock<INeedClosing>();
                _closer.Stub(c => c.Close()).Throw(new CloserException());
                _worker = new Worker(_interactor, _closer);
            }

            [Test]
            public void ReturnDirectly_will_catch_the_interactor_exception_and_bubble_up_the_closer_exception_and_call_close()
            {
                Assert.Throws<CloserException>(() => _worker.ReturnDirectly());
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFF_will_throw_but_close_the_closer()
            {
                Assert.Throws<CloserException>(() => _worker.ReturnAtEnd_FavoringFinally());
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ReturnAtEndFC_will_throw_but_close_the_closer()
            {
                Assert.Throws<CloserException>(() => _worker.ReturnAtEnd_FavoringCatch());
                _closer.AssertWasCalled(c => c.Close());
            }

            [Test]
            public void ClosingTwice_will_close_twice_because_it_visited_the_catch_and_finally_blocks()
            {
                Assert.Throws<CloserException>(() => _worker.ClosingTwice());
                _closer.AssertWasCalled(c => c.Close(), opt => opt.Repeat.Twice());
            }
        }

        private class InteractorException : Exception
        { }

        private class CloserException : Exception
        { }
    }
}