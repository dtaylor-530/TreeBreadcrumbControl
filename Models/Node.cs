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
        private Func<object, Task<IEnumerable>> _childrenProvider;
        private Func<object, string> _stringFormat;
        private Collection _children = new();
        private Flow flow;

        public Node(object content) => Content = content;

        public abstract Task<object?> GetChildren(object value);

        public abstract string StringFormat(object value);
        public abstract Node ToNode(object value);

        public object Content { get; }

        public INode Parent { get; protected set; }

        public virtual IObservable Children
        {
            get
            {
                RefreshAsync();
                return _children;
            }
        }

        public Flow Flow { get => flow; set => flow = value; }

        public virtual IEnumerable Ancestors
        {
            get
            {
                return GetAncestors();
            }

        }

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

            _isRefreshing = true;

            try
            {
                var output = await this.GetChildren(Content);
                if (output is IEnumerable enumerable)
                    SetChildrenCache(ToNodes(enumerable).ToList());
                return true;
            }
            catch
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


        public override string ToString() => StringFormat(Content) ?? Content.ToString();
    }

}
