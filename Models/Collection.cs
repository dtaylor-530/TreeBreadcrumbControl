using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TreeBreadcrumbControl;

namespace Models
{

    public enum ChangeType
    {
        Insert, Remove, Update
    }

    public class Change
    {

        public Change(object value, int index, ChangeType type)
        {
            Value = value;
            Index = index;
            Type = type;
        }

        public object Value { get; }
        public int Index { get; }
        public ChangeType Type { get; }
    }




    public class Collection : IList, IObservable, INotifyCollectionChanged
    {
        private readonly List<object> collection = new();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public Collection()
        {
        }

        public List<IObserver> Observers { get; } = new();

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public int Count => collection.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public object? this[int index] { get => collection[index]; set => collection[index] = value; }

        public IDisposable Subscribe(IObserver observer)
        {
            return new Disposer(Observers, observer);
        }

        public IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public int Add(object? value)
        {
            collection.Add(value);
            foreach (var observer in Observers)
            {
                observer.OnNext(new Change(value, collection.Count - 1, ChangeType.Insert));
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, collection.Count - 1));
            return collection.Count;
        }

        //protected override void SetItem(int index, T item)
        //{
        //    CheckReentrancy();
        //    T originalItem = this[index];
        //    base.SetItem(index, item);

        //    //OnIndexerPropertyChanged();
        //    OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        //}

        public void Clear()
        {
            collection.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object? value)
        {
            return collection.Contains(value);
        }

        public int IndexOf(object? value)
        {
            return collection.IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            foreach (var observer in Observers)
                observer.OnNext(new Change(value, index, ChangeType.Insert));
            collection.Insert(index, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public void Remove(object? value)
        {
            var index = collection.IndexOf(value);
            foreach (var observer in Observers)
            {
                observer.OnNext(new Change(value, index, ChangeType.Remove));
            }
            collection.Remove(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        public void RemoveAt(int index)
        {
            var value = collection[index];
            foreach (var observer in Observers)
            {
                observer.OnNext(new Change(value, index, ChangeType.Remove));
            }
            collection.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException("f 4");
            //Array.Copy(collection, 0, array, index, collection.Count);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler? handler = CollectionChanged;
            if (handler != null)
            {
                // Not calling BlockReentrancy() here to avoid the SimpleMonitor allocation.
                //  _blockReentrancyCount++;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    //    _blockReentrancyCount--;
                }
            }
        }

        internal void Complete()
        {
            foreach(var observer in Observers.ToArray())
            {
                observer.OnCompleted();
            }
        }
    }
}