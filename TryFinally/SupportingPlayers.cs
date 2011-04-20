using System;

namespace TryFinally
{
    public class Worker
    {
        private readonly IInteract _interactor;
        private readonly INeedClosing _closer;

        public Worker(IInteract interactor, INeedClosing closer)
        {
            _interactor = interactor;
            _closer = closer;
        }

        public ExitedFrom ReturnDirectly()
        {
            try
            {
                _interactor.React();
                return ExitedFrom.Try;
            }
            catch(Exception)
            {
                return ExitedFrom.Catch;
            }
            finally
            {
                _closer.Close();
                //can't return from a finally block; this won't compile: return ExitedFrom.Finally;
            }
            return ExitedFrom.EndOfMethod; //this code is unreachable.
        }

        public ExitedFrom ReturnAtEnd_FavoringFinally()
        {
            bool finishedTry = false;
            bool caught = false;
            bool reachedFinally = false;
            try
            {
                _interactor.React();
                finishedTry = true;
            }
            catch (Exception)
            {
                caught = true;
            }
            finally
            {
                _closer.Close();
                reachedFinally = true;
            }
            if (finishedTry)
            {
                return ExitedFrom.Try;
            }
            if (reachedFinally)
            {
                return ExitedFrom.Finally; //always true
            }
            if (caught)
            {
                return ExitedFrom.Catch; //code is unreachable
            }
            return ExitedFrom.EndOfMethod; //code is unreachable
        }

        public ExitedFrom ReturnAtEnd_FavoringCatch()
        {
            bool finishedTry = false;
            bool caught = false;
            bool reachedFinally = false;
            try
            {
                _interactor.React();
                finishedTry = true;
            }
            catch (Exception)
            {
                caught = true;
            }
            finally
            {
                _closer.Close();
                reachedFinally = true;
            }
            if (finishedTry)
            {
                return ExitedFrom.Try;
            }
            if (caught && !reachedFinally)
            {
                return ExitedFrom.Catch; //always false
            }
            if (reachedFinally)
            {
                return ExitedFrom.Finally; //always true
            }
            return ExitedFrom.EndOfMethod; //code is unreachable
        }

        public ExitedFrom ClosingTwice()
        {
            try
            {
                _interactor.React();
                return ExitedFrom.Try;
            }
            catch (Exception)
            {
                _closer.Close();
                return ExitedFrom.Catch;
            }
            finally
            {
                _closer.Close(); //closing a second time if an exception was caught
            }
            return ExitedFrom.EndOfMethod; //code is unreachable
        }

        public int Incrementor()
        {
            int i = 0;
            try
            {
                i++;
                _interactor.React();
                i++;
            }
            catch (Exception)
            {
                i++;
            }
            finally
            {
                i++;
            }
            return i;
        }
    }

    public enum ExitedFrom
    {
        Try,
        Catch,
        Finally,
        EndOfMethod
    }

    public interface IInteract
    {
        bool React();
    }

    public interface INeedClosing
    {
        void Close();
    }
}
