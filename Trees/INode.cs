using System;
using System.Collections;
using System.Collections.Generic;

namespace TreeBreadcrumbControl
{

    public interface IObserver
    {
        void OnNext(object value);

        void OnCompleted();

        void OnError(Exception error);
    }

    public interface IObservable : IEnumerable
    {
        List<IObserver> Observers { get; }

        IDisposable Subscribe(IObserver value)
        {
            return new Disposer(Observers, value);
        }
    }

    public class Disposer : IDisposable
    {
        private readonly IList observers;
        private readonly IObserver observer;

        public Disposer(IList observers, IObserver observer)
        {
            (this.observers = observers).Add(observer);
            this.observer = observer;
        }

        public IEnumerable Observers => observers;
        public IObserver Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }


    public interface INode
    {
        object Content { get; }

        INode Parent { get; }

        IObservable Children { get; }

        IEnumerable Properties { get; }

        IEnumerable Ancestors { get; }
    }
}
