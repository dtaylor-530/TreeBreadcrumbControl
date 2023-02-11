using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TreeBreadcrumbControl;

namespace Demo
{
    public class LazyObservableTreeNode<T> : INode<T>, IRefreshable
    {
        private bool _isRefreshing;
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;
        private Func<T, string> _stringFormat;
        private ObservableCollection<INode<T>> _children = new();

        public LazyObservableTreeNode(T content) => Content = content;

        public virtual Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ??= ((LazyObservableTreeNode<T>)Parent)?.ChildrenProvider;
            set => _childrenProvider = value;
        }

        public Func<T, string> StringFormat
        {
            get => _stringFormat ??= ((LazyObservableTreeNode<T>)Parent).StringFormat;
            set => _stringFormat = value;
        }

        public T Content { get; }

        public virtual INode<T> Parent { get; protected set; }

        public virtual IEnumerable<INode<T>> Children
        {
            get => _children;
        }

        public virtual async Task<bool> RefreshAsync()
        {
            if (_isRefreshing) return false;
            _isRefreshing = true;

            if (ChildrenProvider == null) return AbortRefresh();
            var enumerable = await ChildrenProvider(Content);
            if (enumerable == null) return AbortRefresh();

            var collection = enumerable.ToList();
            if (!collection.Any()) return AbortRefresh();

            var children = collection.Select(GenerateLazyTreeNode).ToList();
            children.ForEach(item => item.Parent = this);

            SetChildrenCache(children.AsReadOnly());

            _isRefreshing = false;
            return true;
        }

        protected virtual LazyObservableTreeNode<T> GenerateLazyTreeNode(T content) => new LazyObservableTreeNode<T>(content);

        protected virtual void SetChildrenCache(IReadOnlyList<INode<T>> childrenCache)
        {
            _children.Clear();
            foreach (var child in childrenCache)
            {
                _children.Add(child);
            }
        }

        private bool AbortRefresh()
        {
            _children.Clear();
            _isRefreshing = false;
            return false;
        }

        public override string ToString() => StringFormat?.Invoke(Content) ?? Content.ToString();
    }

}
