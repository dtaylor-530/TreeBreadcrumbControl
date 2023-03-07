using Enums;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeBreadcrumbControl;

namespace Demo
{
    public abstract class Node : INode
    {
        private bool _isRefreshing;
        private Collection _children = new();
        public abstract Task<object?> GetChildren();

        public abstract Task<bool> HasMoreChildren();
        public abstract Node ToNode(object value);

        public abstract object Content { get; }

        public INode Parent { get; protected set; }

        public virtual IObservable Children
        {
            get
            {
                _ = RefreshAsync();
                return _children;
            }
        }
        public virtual IEnumerable Ancestors
        {
            get
            {
                return GetAncestors();
            }

        }

        public abstract IEnumerable Properties { get; }

        private IEnumerable GetAncestors()
        {
            INode parent = this;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }


        async Task<bool> RefreshAsync()
        {
            if (_isRefreshing)
                return false;

            if (await HasMoreChildren() == false)
                return false;

            _isRefreshing = true;

            try
            {
                var output = await this.GetChildren();
                if (output is IEnumerable enumerable)
                    SetChildrenCache(ToNodes(enumerable).ToList());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }

            IEnumerable<Node> ToNodes(IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    var lazyNode = ToNode(item);
                    lazyNode.Parent = this;
                    yield return lazyNode;
                }
            }

            void SetChildrenCache(IReadOnlyList<INode> childrenCache)
            {
                _children.Clear();
                foreach (var child in childrenCache)
                {
                    _children.Add(child);
                }
                _children.Complete();
            }
        }
    }

}
